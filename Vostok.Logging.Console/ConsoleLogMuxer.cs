﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Collections;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.EventsWriting;

namespace Vostok.Logging.Console
{
    internal class ConsoleLogMuxer
    {
        private readonly AsyncManualResetEvent iterationCompleted = new AsyncManualResetEvent(true);
        private readonly object initLock = new object();
        private readonly IEventsWriter eventsWriter;

        private readonly ConcurrentBoundedQueue<LogEventInfo> events;
        private readonly LogEventInfo[] temporaryBuffer;

        private bool isInitialized;
        private long eventsLost;
        private long eventsLostSinceLastIteration;

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

            iterationCompleted.Reset();

            var eventInfo = new LogEventInfo(@event, settings);

            if (!events.TryAdd(eventInfo))
            {
                Interlocked.Increment(ref eventsLost);
                return false;
            }

            return true;
        }

        public Task FlushAsync() => iterationCompleted.WaitAsync();

        private void StartLoggingTask()
        {
            Task.Run(
                async () =>
                {
                    while (true)
                    {
                        LogEvents();

                        iterationCompleted.Set();

                        if (events.Count == 0)
                            await events.WaitForNewItemsAsync();
                    }
                });
        }

        private void LogEvents()
        {
            var eventsCount = events.Drain(temporaryBuffer, 0, temporaryBuffer.Length);

            eventsWriter.WriteEvents(temporaryBuffer, eventsCount);

            var currentEventsLost = EventsLost;
            if (currentEventsLost > eventsLostSinceLastIteration)
            {
                temporaryBuffer[0] = CreateOverflowEvent(currentEventsLost - eventsLostSinceLastIteration);
                eventsWriter.WriteEvents(temporaryBuffer, 1);

                eventsLostSinceLastIteration = currentEventsLost;
            }
        }

        private LogEventInfo CreateOverflowEvent(long discardedEvents)
        {
            var message = $"[{nameof(ConsoleLog)}] Buffer overflow. {discardedEvents} log events were lost (events queue capacity = {events.Capacity}).";

            return new LogEventInfo(new LogEvent(LogLevel.Warn, DateTimeOffset.Now, message), new ConsoleLogSettings());
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