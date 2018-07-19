using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Commons.Conversions;
using Vostok.Commons.Synchronization;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;
using Console = Vostok.Logging.Core.Console;

namespace Vostok.Logging.ConsoleLog
{
    public class ConsoleLog : ILog
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> levelToColor = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };

        private static readonly AtomicBoolean isInitialized;

        private static volatile LogEvent[] currentEventsBuffer;
        private static volatile BoundedBuffer<LogEvent> eventsBuffer;

        private static ConsoleLogSettings settings;

        static ConsoleLog()
        {
            settings = new ConsoleLogSettings();
            isInitialized = new AtomicBoolean(false);
        }

        public static void Configure(ConsoleLogSettings newSettings)
        {
            var validationResult = new ConsoleLogSettingsValidator().TryValidate(newSettings);
            if (!validationResult.IsSuccessful)
            {
                Console.TryWriteLine(validationResult);
                return;
            }

            settings = newSettings;
        }

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if (!isInitialized)
                Initialize();

            eventsBuffer.TryAdd(@event);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        public ILog ForContext(string context) => this;

        private static void StartLoggingTask()
        {
            Task.Run(
                async () =>
                {
                    while (true)
                    {
                        var currentSettings = settings;

                        try
                        {
                            WriteEventsToConsole(currentSettings);
                        }
                        catch (Exception exception)
                        {
                            Console.TryWriteLine(exception);
                            await Task.Delay(300.Milliseconds());
                        }

                        if (eventsBuffer.Count == 0)
                        {
                            await eventsBuffer.WaitForNewItemsAsync();
                        }
                    }
                });
        }

        private static void WriteEventsToConsole(ConsoleLogSettings currentSettings)
        {
            var buffer = eventsBuffer;
            var eventsToWrite = currentEventsBuffer;

            if (currentSettings.EventsQueueCapacity != currentEventsBuffer.Length)
                ReinitEventsQueue(currentSettings);

            var eventsCount = buffer.Drain(eventsToWrite, 0, eventsToWrite.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = eventsToWrite[i];
                using (new ConsoleColorChanger(levelToColor[currentEvent.Level]))
                {
                    Console.TryWrite(currentSettings.ConversionPattern.Format(currentEvent));
                }
            }
        }

        private static void Initialize()
        {
            if (isInitialized.TrySetTrue())
            {
                ReinitEventsQueue(settings);
                StartLoggingTask();
            }
        }

        private static void ReinitEventsQueue(ConsoleLogSettings newSettings)
        {
            currentEventsBuffer = new LogEvent[newSettings.EventsQueueCapacity];
            eventsBuffer = new BoundedBuffer<LogEvent>(newSettings.EventsQueueCapacity);
        }
    }
}