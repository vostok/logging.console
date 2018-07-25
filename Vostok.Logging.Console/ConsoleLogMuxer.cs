using System.Collections.Concurrent;
using System.Threading.Tasks;
using Vostok.Commons.Synchronization;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.MessageWriters;
using Vostok.Logging.Core;

namespace Vostok.Logging.Console
{
    internal static class ConsoleLogMuxer
    {
        private static readonly ConcurrentDictionary<ConsoleLogSettings, IMessageWriter> MessageWriters;
        private static readonly AtomicBoolean IsInitialized;

        private static volatile ConsoleLogGlobalState state;

        static ConsoleLogMuxer()
        {
            MessageWriters = new ConcurrentDictionary<ConsoleLogSettings, IMessageWriter>();
            IsInitialized = new AtomicBoolean(false);

            Settings = new ConsoleLogGlobalSettings();
            state = new ConsoleLogGlobalState(Settings);
        }

        public static void Log(LogEvent @event, ConsoleLogSettings settings)
        {
            if (!IsInitialized)
                Initialize();

            ConsoleLogGlobalState currentState;

            do
            {
                currentState = state;
            } while (currentState.IsClosedForWriting);

            var writer = MessageWriters.GetOrAdd(settings, MessageWriterFactory.Create);
            currentState.Events.TryAdd(new LogEventInfo(@event, writer));
        }

        public static ConsoleLogGlobalSettings Settings { get; set; }

        private static void StartLoggingTask()
        {
            Task.Run(
                async () =>
                {
                    while (true)
                    {
                        LogEvents(); // TODO(krait): handle errors

                        if (state.Events.Count == 0)
                            await state.Events.WaitForNewItemsAsync();
                    }
                });
        }

        private static void LogEvents()
        {
            var newSettings = Settings;

            var currentState = state;
            if (!Equals(newSettings, state.Settings))
            {
                state.IsClosedForWriting = true;
                state = new ConsoleLogGlobalState(newSettings);
            }

            var eventsCount = currentState.Events.Drain(currentState.TemporaryBuffer, 0, currentState.TemporaryBuffer.Length);
            for (var i = 0; i < eventsCount; i++)
            {
                var currentEvent = currentState.TemporaryBuffer[i];
                currentEvent.Writer.Write(currentEvent.Event);
            }

            foreach (var writer in MessageWriters)
                writer.Value.Flush();
        }

        private static void Initialize()
        {
            if (IsInitialized.TrySetTrue())
                StartLoggingTask();
        }

        private class ConsoleLogGlobalState
        {
            public ConsoleLogGlobalState(ConsoleLogGlobalSettings settings)
            {
                Settings = settings;

                TemporaryBuffer = new LogEventInfo[settings.EventsQueueCapacity];
                Events = new BoundedBuffer<LogEventInfo>(settings.EventsQueueCapacity);
            }

            public ConsoleLogGlobalSettings Settings { get; }

            public LogEventInfo[] TemporaryBuffer { get; }

            public BoundedBuffer<LogEventInfo> Events { get; }

            public bool IsClosedForWriting { get; set; }
        }
    }
}