using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Synchronization;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;

#pragma warning disable 420

namespace Vostok.Logging.ConsoleLog
{
    internal static class ConsoleLogMuxer
    {
        private static readonly IEqualityComparer<ConsoleLogGlobalSettings> SettingsComparer = new GlobalSettingsComparer();
        private static readonly MessageWriter MessageWriter = new MessageWriter(256);

        private static readonly AtomicBoolean IsInitialized;

        private static volatile GlobalState state;

        static ConsoleLogMuxer()
        {
            IsInitialized = new AtomicBoolean(false);

            Settings = new ConsoleLogGlobalSettings();
            state = new GlobalState(Settings);
        }

        public static void Log(LogEvent @event, ConsoleLogSettings settings)
        {
            if (!IsInitialized)
                Initialize();

            var eventInfo = new LogEventInfo(@event, settings);

            var currentState = state;
            while (!currentState.TryAddEvent(eventInfo))
                currentState = state;
        }

        public static int EventsLost => state.EventsLost;

        public static ConsoleLogGlobalSettings Settings { get; set; }

        private static void StartLoggingTask()
        {
            Task.Run(
                async () =>
                {
                    while (true)
                    {
                        LogEvents();

                        if (state.Events.Count == 0)
                            await state.Events.WaitForNewItemsAsync();
                    }
                });
        }

        private static void LogEvents()
        {
            var newSettings = Settings;

            var currentState = state;
            if (!SettingsComparer.Equals(newSettings, state.Settings))
            {
                currentState.CloseForWriting();
                state = new GlobalState(newSettings);
                currentState.WaitForNoWriters();
            }

            int eventsCount;
            do
            {
                eventsCount = currentState.Events.Drain(currentState.TemporaryBuffer, 0, currentState.TemporaryBuffer.Length);
                for (var i = 0; i < eventsCount; i++)
                    WriteEvent(currentState.TemporaryBuffer[i]);
            } while (eventsCount > 0);
        }

        private static void WriteEvent(LogEventInfo currentEvent)
        {
            try
            {
                MessageWriter.Write(currentEvent.Event, currentEvent.Settings);
            }
            catch
            {
                // ignored
            }
        }

        private static void Initialize()
        {
            if (IsInitialized.TrySetTrue())
                StartLoggingTask();
        }

        private class GlobalState
        {
            private volatile int writers;
            private volatile bool isClosedForWriting;
            private volatile int eventsLost;

            public GlobalState(ConsoleLogGlobalSettings settings)
            {
                Settings = settings;

                TemporaryBuffer = new LogEventInfo[settings.EventsQueueCapacity];
                Events = new BoundedBuffer<LogEventInfo>(settings.EventsQueueCapacity);
            }

            public ConsoleLogGlobalSettings Settings { get; }

            public LogEventInfo[] TemporaryBuffer { get; }

            public BoundedBuffer<LogEventInfo> Events { get; }

            public int EventsLost => eventsLost;

            public void WaitForNoWriters()
            {
                var spinWait = new SpinWait();

                while (writers > 0)
                    spinWait.SpinOnce();
            }

            public bool TryAddEvent(LogEventInfo eventInfo)
            {
                Interlocked.Increment(ref writers);

                var willAdd = !isClosedForWriting;
                if (willAdd)
                {
                    if (!Events.TryAdd(eventInfo))
                        Interlocked.Increment(ref eventsLost);
                }

                Interlocked.Decrement(ref writers);

                return willAdd;
            }

            public void CloseForWriting() => isClosedForWriting = true;
        }
    }
}