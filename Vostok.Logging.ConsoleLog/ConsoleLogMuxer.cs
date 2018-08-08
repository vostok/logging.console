using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Collections;
using Vostok.Logging.Abstractions;

#pragma warning disable 420

namespace Vostok.Logging.ConsoleLog
{
    internal class ConsoleLogMuxer
    {
        private readonly object initLock = new object();
        private readonly IEventsWriter eventsWriter;

        private readonly ConcurrentBoundedQueue<LogEventInfo> events;
        private readonly LogEventInfo[] temporaryBuffer;

        private bool isInitialized;
        private long eventsLost;

        public ConsoleLogMuxer(IEventsWriter eventsWriter, int eventsQueueCapacity)
        {
            this.eventsWriter = eventsWriter;
            temporaryBuffer = new LogEventInfo[eventsQueueCapacity];
            events = new ConcurrentBoundedQueue<LogEventInfo>(eventsQueueCapacity);
        }

        public long EventsLost => Interlocked.Read(ref eventsLost);

        public bool TryLog(LogEvent @event, ConsoleLogSettings settings)
        {
            if (!isInitialized)
                Initialize();

            var eventInfo = new LogEventInfo(@event, settings);

            if (!events.TryAdd(eventInfo))
            {
                Interlocked.Increment(ref eventsLost);
                return false;
            }

            return true;
        }

        private void StartLoggingTask()
        {
            Task.Run(
                async () =>
                {
                    while (true)
                    {
                        LogEvents();

                        if (events.Count == 0)
                            await events.WaitForNewItemsAsync();
                    }
                });
        }

        private void LogEvents()
        {
            var eventsCount = events.Drain(temporaryBuffer, 0, temporaryBuffer.Length);

            eventsWriter.WriteEvents(temporaryBuffer, eventsCount);
        }

        private void Initialize()
        {
            lock (initLock)
            {
                if (!isInitialized)
                {
                    StartLoggingTask();
                    isInitialized = true;
                }
            }
        }
    }
}