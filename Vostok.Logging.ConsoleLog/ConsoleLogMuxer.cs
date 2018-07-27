using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Synchronization;
using Vostok.Logging.Abstractions;
using Vostok.Logging.ConsoleLog.MessageWriters;
using Vostok.Logging.Core;

#pragma warning disable 420

namespace Vostok.Logging.ConsoleLog
{
    internal static class ConsoleLogMuxer
    {
        private static readonly IEqualityComparer<ConsoleLogGlobalSettings> SettingsComparer = new GlobalSettingsComparer();

        private static readonly ConcurrentDictionary<ConsoleLogSettings, IMessageWriter> MessageWriters;
        private static readonly AtomicBoolean IsInitialized;

        private static volatile GlobalState state;

        static ConsoleLogMuxer()
        {
            MessageWriters = new ConcurrentDictionary<ConsoleLogSettings, IMessageWriter>();
            IsInitialized = new AtomicBoolean(false);

            Settings = new ConsoleLogGlobalSettings();
            state = new GlobalState(Settings);
        }

        public static void Log(LogEvent @event, ConsoleLogSettings settings)
        {
            if (!IsInitialized)
                Initialize();

            // ReSharper disable once ConvertClosureToMethodGroup
            var writer = MessageWriters.GetOrAdd(settings, logSettings => MessageWriterFactory.Create(logSettings));
            var eventInfo = new LogEventInfo(@event, writer);

            var currentState = state;
            while (!currentState.TryAddEvent(eventInfo))
                currentState = state;
        }

        public static int LostEvents => state.LostEvents;

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

        private class DrainInfo
        {
            private int sum;
            private int count;

            public double Average => sum/(double)count;

            public DrainInfo(int count, int sum)
            {
                this.count = count;
                this.sum = sum;
            }

            public DrainInfo Add(int s)
            {
                return new DrainInfo(count + 1, sum + s);
            }
        }
        private static volatile DrainInfo drainSizeInfo = new DrainInfo(0, 0);
        private static volatile DrainInfo drainCountInfo = new DrainInfo(0, 0);

        public static double AverageDrainSize => drainSizeInfo.Average;
        public static double AverageDrainAttempts => drainCountInfo.Average;

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

            int cnt = 0;
            int eventsCount;
            do
            {
                eventsCount = currentState.Events.Drain(currentState.TemporaryBuffer, 0, currentState.TemporaryBuffer.Length);
                for (var i = 0; i < eventsCount; i++)
                    WriteEvent(currentState.TemporaryBuffer[i]);
                drainSizeInfo = drainSizeInfo.Add(eventsCount);
                cnt++;
            } while (eventsCount > 0);

            drainCountInfo = drainCountInfo.Add(cnt);

            foreach (var writer in MessageWriters)
                writer.Value.Flush();
        }

        private static void WriteEvent(LogEventInfo currentEvent)
        {
            try
            {
                currentEvent.Writer.Write(currentEvent.Event);
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
            private volatile int lostEvents;

            public GlobalState(ConsoleLogGlobalSettings settings)
            {
                Settings = settings;

                TemporaryBuffer = new LogEventInfo[settings.EventsQueueCapacity];
                Events = new BoundedBuffer<LogEventInfo>(settings.EventsQueueCapacity);
            }

            public ConsoleLogGlobalSettings Settings { get; }

            public LogEventInfo[] TemporaryBuffer { get; }

            public BoundedBuffer<LogEventInfo> Events { get; }

            public int LostEvents => lostEvents;

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
                        Interlocked.Increment(ref lostEvents);
                }

                Interlocked.Decrement(ref writers);

                return willAdd;
            }

            public void CloseForWriting() => isClosedForWriting = true;
        }
    }
}