﻿using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Commons.Testing;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.EventsWriting;

namespace Vostok.Logging.Console.Tests
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
            muxer = new ConsoleLogMuxer(eventsWriter, 1, 1);
        }

        [Test]
        public void EventsLost_should_be_incremented_after_losing_an_event()
        {
            muxer = new ConsoleLogMuxer(eventsWriter, 1, 1);

            for (var i = 0; i < 100; i++)
                muxer.TryLog(CreateEvent(), new ConsoleLogSettings());

            muxer.EventsLost.Should().BeGreaterThan(50);
        }

        [Test]
        public void TryLog_should_return_true_if_event_was_added_successfully()
        {
            muxer.TryLog(CreateEvent(), new ConsoleLogSettings()).Should().BeTrue();
        }

        [Test]
        public void TryLog_should_return_false_if_event_was_not_added()
        {
            muxer = new ConsoleLogMuxer(eventsWriter, 1, 1);

            var added = 0;
            
            for (var i = 0; i < 100; i++)
                if (muxer.TryLog(CreateEvent(), new ConsoleLogSettings()))
                    added++;

            added.Should().BeLessThan(50);
        }

        [Test]
        public void Should_eventually_write_added_events()
        {
            var e = CreateEvent();

            muxer.TryLog(e, new ConsoleLogSettings());

            new Action(() => eventsWriter.Received().WriteEvents(
                Arg.Is<LogEventInfo[]>(events => 
                    events.Length == 1 && ReferenceEquals(events[0].Event, e)), 1))
                .ShouldPassIn(10.Seconds());
        }

        [Test]
        public void Should_write_events_with_correct_settings()
        {
            var settings = new ConsoleLogSettings();

            muxer.TryLog(CreateEvent(), settings);

            new Action(() => eventsWriter.Received().WriteEvents(
                    Arg.Is<LogEventInfo[]>(events =>
                        events.Length == 1 && ReferenceEquals(events[0].Settings, settings)), 1))
                .ShouldPassIn(10.Seconds());
        }

        [Test]
        public void Flush_should_wait_until_current_events_are_written()
        {
            var e = CreateEvent();

            muxer.TryLog(e, new ConsoleLogSettings());
            muxer.FlushAsync().Wait();

            eventsWriter.Received().WriteEvents(
                Arg.Is<LogEventInfo[]>(events =>
                    events.Length == 1 && ReferenceEquals(events[0].Event, e)), 1);
        }

        [Test]
        public void Flush_should_complete_immediately_when_muxer_is_not_initialized_yet()
        {
            muxer.FlushAsync().IsCompleted.Should().BeTrue();
        }

        private static LogEvent CreateEvent()
        {
            return new LogEvent(LogLevel.Info, DateTimeOffset.Now, "");
        }
    }
}