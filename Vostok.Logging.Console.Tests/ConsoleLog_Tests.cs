using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console.Tests
{
    [TestFixture]
    public class ConsoleLog_Tests
    {
        [Test]
        public void Should_validate_settings()
        {
            var settings = new ConsoleLogSettings
            {
                OutputTemplate = null
            };

            new Action(() => new ConsoleLog(settings)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Should_be_enabled_for_all_levels([Values] LogLevel level)
        {
            new ConsoleLog().IsEnabledFor(level).Should().BeTrue();
        }

        [Test]
        public void Should_log_messages()
        {
            CaptureEvents(log => log.Info("Test."))
                .Should()
                .ContainSingle(e => e.MessageTemplate == "Test.");
        }

        [Test]
        public void ForContext_should_add_SourceContext_property()
        {
            CaptureEvents(log => log.ForContext("ctx").Info("Test."))
                .Should()
                .ContainSingle(e => (string)e.Properties[WellKnownProperties.SourceContext] == "ctx");
        }

        [Test]
        public void ForContext_should_replace_SourceContext_property()
        {
            CaptureEvents(
                    log => log
                        .ForContext("ctx")
                        .ForContext("ctx2")
                        .ForContext("ctx3")
                        .Info("Test."))
                .Should()
                .ContainSingle(e => (string)e.Properties[WellKnownProperties.SourceContext] == "ctx3");
        }

        [Test]
        public void ForContext_should_support_null_context()
        {
            CaptureEvents(
                    log => log
                        .ForContext("ctx")
                        .ForContext(null)
                        .Info("Test."))
                .Should()
                .ContainSingle(e => e.Properties == null || !e.Properties.ContainsKey(WellKnownProperties.SourceContext));
        }

        private static IEnumerable<LogEvent> CaptureEvents(Action<ConsoleLog> action)
        {
            var events = new List<LogEvent>();

            var muxer = Substitute.For<IConsoleLogMuxer>();
            muxer.TryLog(Arg.Do<LogEvent>(e => events.Add(e)), Arg.Any<ConsoleLogSettings>()).Returns(true);

            var muxerProvider = Substitute.For<IConsoleLogMuxerProvider>();
            muxerProvider.ObtainMuxer().Returns(muxer);

            var log = new ConsoleLog(muxerProvider, new ConsoleLogSettings {OutputTemplate = OutputTemplate.Parse("{Message}")});

            action(log);

            return events;
        }
    }
}