using System.Collections.Concurrent;
using System.Threading.Tasks;
using Vostok.Commons.Synchronization;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    internal static class ConsoleLogMuxer
    {
        private static readonly ConcurrentDictionary<ConsoleLogSettings, IMessageWriter> messageWriters;
        private static readonly AtomicBoolean isInitialized;

        private static volatile ConsoleLogGlobalState state;

        static ConsoleLogMuxer()
        {
            messageWriters = new ConcurrentDictionary<ConsoleLogSettings, IMessageWriter>();
            isInitialized = new AtomicBoolean(false);

            Settings = new ConsoleLogGlobalSettings();
            state = new ConsoleLogGlobalState(Settings);
        }

        public static void Log(LogEvent @event, ConsoleLogSettings settings)
        {
            if (!isInitialized)
                Initialize();

            ConsoleLogGlobalState currentState;

            do
            {
                currentState = state;
            } while (currentState.IsClosedForWriting);

            var writer = messageWriters.GetOrAdd(settings, MessageWriterFactory.Create);
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

            foreach (var writer in messageWriters)
                writer.Value.Flush();
        }

        private static void Initialize()
        {
            if (isInitialized.TrySetTrue())
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