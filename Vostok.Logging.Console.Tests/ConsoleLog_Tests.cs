using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console.Tests
{
    [TestFixture]
    public class StrangeBug_Tests
    {
        private static volatile string testname = null;
        private static volatile TestExecutionContext Ctx;
        private Task logger;

        [SetUp]
        public void setup()
        {
            if (logger != null)
                return;
            
            logger =  Task.Run(
                () =>
                {
                    try
                    {
                        while (true)
                        {
                            var n = testname;
                            if (n != null)
                            {
                                Ctx = TestExecutionContext.CurrentContext;
                                TestContext.WriteLine(n);
                            }

                            Thread.Sleep(1000);
                        }

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                    }
                });
        }

        [TestCase("A")]
        [TestCase("B")]
        public void A(string x)
        {
            System.Console.WriteLine("start");
            Task.Run(() => System.Console.WriteLine("from " + x + " task")).GetAwaiter().GetResult();
            testname = x;
            Thread.Sleep(2000);
            System.Console.WriteLine(Ctx.GetType().FullName);
            System.Console.WriteLine(Ctx.CurrentTest.FullName);
            // (TestExecutionContext.CurrentContext == Ctx).Should().BeTrue();
            Thread.Sleep(2000);
        }
    }

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