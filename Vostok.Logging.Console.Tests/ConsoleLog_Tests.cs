using System;
using System.IO;
using FluentAssertions;
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
        public void Should_log_formatted_messages_to_console()
        {
            var settings = new ConsoleLogSettings {OutputTemplate = OutputTemplate.Parse("{Message}")};
            CaptureOutput(() => new ConsoleLog(settings).Info("Test."))
                .Should()
                .Be("Test.");
        }

        [Test]
        public void ForContext_should_add_SourceContext_property()
        {
            var settings = new ConsoleLogSettings {OutputTemplate = OutputTemplate.Parse($"{{{WellKnownProperties.SourceContext}}}")};
            CaptureOutput(() => new ConsoleLog(settings).ForContext("ctx").Info("Test."))
                .Should()
                .Be("ctx");
        }

        [Test]
        public void ForContext_should_replace_SourceContext_property()
        {
            var settings = new ConsoleLogSettings { OutputTemplate = OutputTemplate.Parse($"{{{WellKnownProperties.SourceContext}}}") };
            CaptureOutput(() => new ConsoleLog(settings)
                    .ForContext("ctx")
                    .ForContext("ctx2")
                    .ForContext("ctx3")
                    .Info("Test."))
                .Should()
                .Be("ctx3");
        }

        [Test]
        public void ForContext_should_support_null_context()
        {
            var settings = new ConsoleLogSettings { OutputTemplate = OutputTemplate.Parse($"{{{WellKnownProperties.SourceContext}}}") };
            CaptureOutput(() => new ConsoleLog(settings)
                    .ForContext("ctx")
                    .ForContext(null)
                    .Info("Test."))
                .Should()
                .Be("");
        }

        private static string CaptureOutput(Action action)
        {
            var writer = new StringWriter();
            var oldOut = System.Console.Out;

            System.Console.SetOut(writer);

            action();
            ConsoleLog.FlushAsync().GetAwaiter().GetResult();

            System.Console.SetOut(oldOut);

            return writer.ToString();
        }
    }
}