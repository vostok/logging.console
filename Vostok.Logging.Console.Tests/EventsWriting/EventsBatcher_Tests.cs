using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.EventsWriting;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console.Tests.EventsWriting
{
    [TestFixture]
    internal class EventsBatcher_Tests
    {
        private EventsBatcher batcher;
        private EventsBatcher batcherWithRedirectedOutput;

        [SetUp]
        public void TestSetup()
        {
            batcher = new EventsBatcher(CreateDetector(true));
            batcherWithRedirectedOutput = new EventsBatcher(CreateDetector(false));
        }

        [Test]
        public void Should_not_group_events_with_different_log_levels_if_colors_are_enabled()
        {
            var settings = new ConsoleLogSettings {ColorsEnabled = true};

            var events = new[]
            {
                new LogEventInfo(CreateEvent(LogLevel.Info), settings),
                new LogEventInfo(CreateEvent(LogLevel.Warn), settings)
            };

            batcher.BatchEvents(events, 2).Should().HaveCount(2);
        }

        [Test]
        public void Should_group_events_with_different_log_levels_if_colors_are_disabled()
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
            var settings = new ConsoleLogSettings {ColorsEnabled = true};

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
            var settings = new ConsoleLogSettings {ColorsEnabled = true};

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
            var settings = new ConsoleLogSettings {ColorsEnabled = true};

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
            var settings = new ConsoleLogSettings {ColorsEnabled = true};

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

        // TODO(krait): split this test into multiple smaller tests for each case
        [Test]
        public void Should_group_all_log_events_in_one_batch()
        {
            var sets1 = GetSettings();
            var sets2 = GetSettings();
            sets2.ColorMapping[LogLevel.Debug] = sets1.ColorMapping[LogLevel.Info];

            var logEventInfo = CreateEvent(LogLevel.Info);
            var logEventDebug = CreateEvent(LogLevel.Debug);
            var logEventError = CreateEvent(LogLevel.Error);

            var logInfos = new[]
            {
                new LogEventInfo(logEventInfo, sets1),
                new LogEventInfo(logEventInfo, sets1),      //same settings and level
                new LogEventInfo(logEventInfo, sets2),      //different settings but same color
                new LogEventInfo(logEventDebug, sets2),     //same settings and different levels but same colors in mapping
            };
            batcher.BatchEvents(logInfos, logInfos.Length).Should().HaveCount(1);

            sets1.ColorsEnabled = false;

            logInfos = new[]
            {
                new LogEventInfo(logEventInfo, sets1),
                new LogEventInfo(logEventError, sets1),     //same default settings, colors are disabled
            };
            batcher.BatchEvents(logInfos, logInfos.Length).Should().HaveCount(1);
        }

        [Test]
        public void Should_use_gray_as_default_color()
        {
            var sets1 = GetSettings();
            sets1.ColorMapping.Remove(LogLevel.Info);
            sets1.ColorMapping[LogLevel.Debug] = ConsoleColor.Gray;
            var sets2 = GetSettings();
            sets2.ColorMapping.Remove(LogLevel.Debug);
            sets2.ColorMapping[LogLevel.Info] = ConsoleColor.Gray;

            var logEventInfo = CreateEvent(LogLevel.Info);
            var logEventDebug = CreateEvent(LogLevel.Debug);

            var logInfos = new[]
            {
                new LogEventInfo(logEventInfo, sets1),
                new LogEventInfo(logEventDebug, sets2),     //both absent
                new LogEventInfo(logEventDebug, sets1),     //absent and gray
                new LogEventInfo(logEventInfo, sets2),      //same colors
                new LogEventInfo(logEventDebug, sets2),     //gray and absent
            };
            batcher.BatchEvents(logInfos, logInfos.Length).Should().HaveCount(1);
        }

        [Test]
        public void Should_disregard_log_level_when_colors_are_not_supported()
        {
            var settings = GetSettings();

            var logEventInfo = CreateEvent(LogLevel.Info);
            var logEventDebug = CreateEvent(LogLevel.Debug);
            var logEventError = CreateEvent(LogLevel.Error);
            var logEventWarn = CreateEvent(LogLevel.Warn);
            var logEventFatal = CreateEvent(LogLevel.Fatal);

            var logInfos = new[]
            {
                new LogEventInfo(logEventInfo, settings),
                new LogEventInfo(logEventDebug, settings),
                new LogEventInfo(logEventError, settings),
                new LogEventInfo(logEventWarn, settings),
                new LogEventInfo(logEventFatal, settings),
            };
            batcherWithRedirectedOutput.BatchEvents(logInfos, logInfos.Length).Should().HaveCount(1);
        }

        private static IConsoleFeaturesDetector CreateDetector(bool colorsAreSupported)
        {
            var consoleFeaturesDetector = Substitute.For<IConsoleFeaturesDetector>();
            consoleFeaturesDetector.AreColorsSupported.Returns(colorsAreSupported);
            return consoleFeaturesDetector;
        }

        private static ConsoleLogSettings GetSettings() =>
            new ConsoleLogSettings { OutputTemplate = OutputTemplate.Parse($"{{{WellKnownTokens.Message}}}") };

        private static LogEvent CreateEvent(LogLevel level = LogLevel.Info)
        {
            return new LogEvent(level, DateTimeOffset.Now, "");
        }
    }
}