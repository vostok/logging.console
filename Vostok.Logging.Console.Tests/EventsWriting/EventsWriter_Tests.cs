using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.EventsWriting;

namespace Vostok.Logging.Console.Tests.EventsWriting
{
    [TestFixture]
    internal class EventsWriter_Tests
    {
        private EventsWriter writer;
        private IEventsBatcher batcher;
        private IConsoleWriter consoleWriter;
        private IDisposable colorReleaser;

        [SetUp]
        public void TestSetup()
        {
            batcher = Substitute.For<IEventsBatcher>();
            batcher.BatchEvents(Arg.Any<LogEventInfo[]>(), Arg.Any<int>()).Returns(MakeBatch);

            consoleWriter = Substitute.For<IConsoleWriter>();

            colorReleaser = Substitute.For<IDisposable>();
            consoleWriter.ChangeColor(Arg.Any<ConsoleColor>()).Returns(_ => colorReleaser);

            writer = new EventsWriter(batcher, consoleWriter);
        }

        [Test]
        public void Should_ignore_exceptions_in_event_rendering()
        {
            consoleWriter.When(r => r.WriteLogEvent(Arg.Any<LogEventInfo>())).Throw<Exception>();

            new Action(() => writer.WriteEvents(new []{CreateLogEventInfo()}, 1))
                .Should().NotThrow();
        }

        [Test]
        public void Should_continue_rendering_batch_after_exception_for_one_event()
        {
            var badEvent = CreateLogEventInfo();
            consoleWriter.When(r => r.WriteLogEvent(badEvent)).Throw<Exception>();

            writer.WriteEvents(new[] {badEvent, CreateLogEventInfo()}, 2);

            consoleWriter.Received(2).WriteLogEvent(Arg.Any<LogEventInfo>());
        }

        [Test]
        public void Should_flush_after_each_batch()
        {
            batcher.BatchEvents(Arg.Any<LogEventInfo[]>(), Arg.Any<int>())
                .Returns(callInfo => MakeBatch(callInfo).Concat(MakeBatch(callInfo)));

            writer.WriteEvents(new[] { CreateLogEventInfo(), CreateLogEventInfo() }, 2);

            Received.InOrder(
                () =>
                {
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    consoleWriter.Flush();
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    consoleWriter.Flush();
                });
        }

        [Test]
        public void Should_change_color_around_batch_if_colors_enabled()
        {
            batcher.BatchEvents(Arg.Any<LogEventInfo[]>(), Arg.Any<int>())
                .Returns(callInfo => MakeBatch(callInfo).Concat(MakeBatch(callInfo)));

            writer.WriteEvents(new[] { CreateLogEventInfo(), CreateLogEventInfo() }, 2);

            Received.InOrder(
                () =>
                {
                    consoleWriter.ChangeColor(ConsoleColor.White);
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    colorReleaser.Dispose();
                    consoleWriter.ChangeColor(ConsoleColor.White);
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    consoleWriter.WriteLogEvent(Arg.Any<LogEventInfo>());
                    colorReleaser.Dispose();
                });
        }

        [Test]
        public void Should_not_change_color_around_batch_if_colors_disabled()
        {
            var settings = new ConsoleLogSettings {ColorsEnabled = false};
            writer.WriteEvents(new[] { CreateLogEventInfo(settings), CreateLogEventInfo(settings) }, 2);

            consoleWriter.DidNotReceive().ChangeColor(Arg.Any<ConsoleColor>());
        }

        [Test]
        public void Should_not_write_anything_if_there_are_no_batches()
        {
            batcher.BatchEvents(Arg.Any<LogEventInfo[]>(), Arg.Any<int>())
                .Returns(callInfo => Array.Empty<ArraySegment<LogEventInfo>>());

            writer.WriteEvents(new LogEventInfo[] { }, 0);

            consoleWriter.DidNotReceive().WriteLogEvent(Arg.Any<LogEventInfo>());
        }

        private static LogEventInfo CreateLogEventInfo(ConsoleLogSettings settings = null)
        {
            return new LogEventInfo(new LogEvent(LogLevel.Info, DateTimeOffset.Now, ""), settings ?? new ConsoleLogSettings {ColorsEnabled = true});
        }

        private static IEnumerable<ArraySegment<LogEventInfo>> MakeBatch(CallInfo callInfo)
        {
            var array = callInfo.Arg<LogEventInfo[]>();
            var length = callInfo.Arg<int>();

            return array == null ? Enumerable.Empty<ArraySegment<LogEventInfo>>() : new[] {new ArraySegment<LogEventInfo>(array, 0, length)};
        }
    }
}