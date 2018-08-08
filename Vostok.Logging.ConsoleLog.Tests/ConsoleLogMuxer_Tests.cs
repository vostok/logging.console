﻿using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Commons.Testing.Testing;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog.Tests
{
    [TestFixture]
    internal class ConsoleLogMuxer_Tests
    {
        private IEventsWriter eventsWriter;
        private ConsoleLogMuxer muxer;

        [SetUp]
        public void TestSetup()
        {
            eventsWriter = Substitute.For<IEventsWriter>();
            muxer = new ConsoleLogMuxer(eventsWriter, 1);
        }

        [Test]
        public void EventsLost_should_be_incremented_after_losing_an_event()
        {
            muxer = new ConsoleLogMuxer(eventsWriter, 0);

            muxer.TryLog(CreateEvent(), new ConsoleLogSettings());
            muxer.TryLog(CreateEvent(), new ConsoleLogSettings());

            muxer.EventsLost.Should().Be(2);
        }

        [Test]
        public void TryLog_should_return_true_if_event_was_added_successfully()
        {
            muxer.TryLog(CreateEvent(), new ConsoleLogSettings()).Should().BeTrue();
        }

        [Test]
        public void TryLog_should_return_false_if_event_was_not_added()
        {
            muxer = new ConsoleLogMuxer(eventsWriter, 0);

            muxer.TryLog(CreateEvent(), new ConsoleLogSettings()).Should().BeFalse();
        }

        [Test]
        public void Should_eventually_write_added_events()
        {
            var e = CreateEvent();

            muxer.TryLog(e, new ConsoleLogSettings());

            new Action(() => eventsWriter.Received().WriteEvents(
                Arg.Is<LogEventInfo[]>(events => 
                    events.Length == 1 && ReferenceEquals(events[0].Event, e)), 1))
                .ShouldPassIn(1.Seconds());
        }

        [Test]
        public void Should_write_events_with_correct_settings()
        {
            var settings = new ConsoleLogSettings();

            muxer.TryLog(CreateEvent(), settings);

            new Action(() => eventsWriter.Received().WriteEvents(
                    Arg.Is<LogEventInfo[]>(events =>
                        events.Length == 1 && ReferenceEquals(events[0].Settings, settings)), 1))
                .ShouldPassIn(1.Seconds());
        }

        private static LogEvent CreateEvent()
        {
            return new LogEvent(LogLevel.Info, DateTimeOffset.Now, "");
        }
    }
}