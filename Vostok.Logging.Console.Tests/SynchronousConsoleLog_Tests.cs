using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Console.Tests
{
    [TestFixture]
    public class SynchronousConsoleLog_Tests
    {
        [Test]
        public void Should_not_throw()
        {
            var log = new SynchronousConsoleLog();
            new Action(() => log.Info("123")).Should().NotThrow();
        }

        [Test]
        public void Should_write_some_text()
        {
            var log = new SynchronousConsoleLog();
            var writer = new StringWriter();
            var oldOut = System.Console.Out;
            try
            {
                System.Console.SetOut(writer);
                log.Log(new LogEvent(LogLevel.Info, new DateTimeOffset(
                    2019, 2, 18, 15, 43, 9, 494, TimeSpan.Zero),
                    "template"));
                writer.GetStringBuilder().ToString().Trim().Should().BeEquivalentTo("2019-02-18 15:43:09,494 INFO  template");
            }
            finally
            {
                System.Console.SetOut(oldOut);
            }
        }
    }
}