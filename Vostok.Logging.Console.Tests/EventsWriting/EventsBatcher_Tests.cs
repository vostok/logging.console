using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.EventsWriting;

namespace Vostok.Logging.Console.Tests.EventsWriting
{
    [TestFixture]
    internal class EventsBatcher_Tests
    {
        private EventsBatcher batcher;

        [SetUp]
        public void TestSetup()
        {
            batcher = new EventsBatcher();
        }

        [Test]
        public void Should_not_group_events_with_different_settings_instances()
        {
            var events = new[]
            {
                new LogEventInfo(CreateEvent(), new ConsoleLogSettings()),
                new LogEventInfo(CreateEvent(), new ConsoleLogSettings())
            };

            batcher.BatchEvents(events, 2).Should().HaveCount(2);
        }

        [Test]
        public void Should_not_group_events_with_different_log_levels_if_colors_enabled()
        {
            var settings = new ConsoleLogSettings();

            var events = new[]
            {
                new LogEventInfo(CreateEvent(LogLevel.Info), settings),
                new LogEventInfo(CreateEvent(LogLevel.Warn), settings)
            };

            batcher.BatchEvents(events, 2).Should().HaveCount(2);
        }

        [Test]
        public void Should_group_events_with_different_log_levels_if_colors_disabled()
        {
            var settings = new ConsoleLogSettings {ColorsEnabled = false};

            var events = new[]
            {
                new LogEventInfo(CreateEvent(LogLevel.Info), settings),
                new LogEventInfo(CreateEvent(LogLevel.Warn), settings)
            };

            batcher.BatchEvents(events, 2).Should().HaveCount(1);
        }

        [Test]
        public void Should_group_events_with_same_log_level_and_settings()
        {
            var settings = new ConsoleLogSettings();

            var events = new[]
            {
                new LogEventInfo(CreateEvent(), settings),
                new LogEventInfo(CreateEvent(), settings)
            };

            batcher.BatchEvents(events, 2).Should().HaveCount(1);
        }

        [Test]
        public void Should_create_correct_batches()
        {
            var settings = new ConsoleLogSettings();

            var events = new[]
            {
                new LogEventInfo(CreateEvent(LogLevel.Info), settings),
                new LogEventInfo(CreateEvent(LogLevel.Info), settings),
                new LogEventInfo(CreateEvent(LogLevel.Warn), settings),
                new LogEventInfo(CreateEvent(LogLevel.Warn), settings),
                new LogEventInfo(CreateEvent(LogLevel.Error), settings),
                new LogEventInfo(CreateEvent(LogLevel.Error), settings)
            };

            var batches = batcher.BatchEvents(events, events.Length).ToArray();

            batches.Should().HaveCount(3);
            batches[0].Should().Equal(events.Take(2));
            batches[1].Should().Equal(events.Skip(2).Take(2));
            batches[2].Should().Equal(events.Skip(4).Take(2));
        }

        [Test]
        public void Should_only_group_consequent_events()
        {
            var settings = new ConsoleLogSettings();

            var events = new[]
            {
                new LogEventInfo(CreateEvent(LogLevel.Info), settings),
                new LogEventInfo(CreateEvent(LogLevel.Warn), settings),
                new LogEventInfo(CreateEvent(LogLevel.Info), settings)
            };
            
            batcher.BatchEvents(events, events.Length).Should().HaveCount(3);
        }

        [Test]
        public void Should_return_single_batch_for_single_event()
        {
            var settings = new ConsoleLogSettings();

            var events = new[]
            {
                new LogEventInfo(CreateEvent(LogLevel.Info), settings)
            };

            batcher.BatchEvents(events, events.Length).Should().HaveCount(1);
        }

        [Test]
        public void Should_return_no_batches_for_empty_array()
        {
            batcher.BatchEvents(Array.Empty<LogEventInfo>(), 0).Should().HaveCount(0);
        }

        private static LogEvent CreateEvent(LogLevel level = LogLevel.Info)
        {
            return new LogEvent(level, DateTimeOffset.Now, "");
        }
    }
}