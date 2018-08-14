using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Console.Tests
{
    [TestFixture]
    internal class ConsoleLogSettings_Tests
    {
        private ConsoleLogSettings settings;

        [SetUp]
        public void TestSetup()
        {
            settings = new ConsoleLogSettings();
        }

        [Test]
        public void Default_settings_should_have_colors_mapped_to_all_log_levels()
        {
            foreach (var level in Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>())
            {
                settings.ColorMapping.Should().ContainKey(level);
            }
        }

        [Test]
        public void Default_settings_should_have_no_format_provider()
        {
            settings.FormatProvider.Should().BeNull();
        }
    }
}