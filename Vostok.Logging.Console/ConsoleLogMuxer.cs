using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Collections;
using Vostok.Commons.Threading;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.EventsWriting;
using Waiter = System.Threading.Tasks.TaskCompletionSource<bool>;

namespace Vostok.Logging.Console
{
    internal class ConsoleLogMuxer : IConsoleLogMuxer
    {
        private static readonly TimeSpan NewEventsTimeout = TimeSpan.FromSeconds(1);

        private readonly AsyncManualResetEvent flushSignal = new AsyncManualResetEvent(true);
        private readonly List<Waiter> flushWaiters = new List<Waiter>();
        private readonly object initLock = new object();
        private readonly IEventsWriter eventsWriter;

        private readonly ConcurrentBoundedQueue<LogEventInfo> events;
        private readonly LogEventInfo[] temporaryBuffer;

        private bool isInitialized;
        private long eventsLost;
        private long eventsLostSinceLastIteration;

        public ConsoleLogMuxer(IEventsWriter eventsWriter, int eventsQueueCapacity, int temporaryBufferCapacity)
        {
            this.eventsWriter = eventsWriter;
            temporaryBuffer = new LogEventInfo[temporaryBufferCapacity];
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

        public Task FlushAsync()
        {
            var waiter = new Waiter();

            lock (flushWaiters)
                flushWaiters.Add(waiter);

            flushSignal.Set();

            return waiter.Task;
        }

        private void StartLoggingTask()
        {
            Task.Run(
                async () =>
                {
                    while (true)
                    {
                        try
                        {
                            List<Waiter> currentWaiters;
                            lock (flushWaiters)
                            {
                                flushWaiters.RemoveAll(w => w.Task.IsCompleted);
                                currentWaiters = flushWaiters.ToList();
                            }

                            LogEvents();

                            foreach (var waiter in currentWaiters)
                                waiter.TrySetResult(true);
                        }
                        catch
                        {
                            await Task.Delay(100);
                        }

                        await Task.WhenAny(events.TryWaitForNewItemsAsync(NewEventsTimeout), flushSignal.WaitAsync());
                        flushSignal.Reset();
                    }
                });
        }

        private void LogEvents()
        {
            var eventsCount = events.Drain(temporaryBuffer, 0, temporaryBuffer.Length);

            try
            {
                eventsWriter.WriteEvents(temporaryBuffer, eventsCount);
            }
            catch
            {
                Interlocked.Add(ref eventsLost, eventsCount);
                throw;
            }

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