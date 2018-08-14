using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;

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

        // TODO(krait): ForContext
    }
}