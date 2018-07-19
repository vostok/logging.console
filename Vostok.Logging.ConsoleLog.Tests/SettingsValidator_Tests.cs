using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.ConsoleLog.Tests
{
    [TestFixture]
    internal class SettingsValidator_Tests
    {
        [Test]
        public void Default_ConsoleLogSettings_should_be_valid()
        {
            consoleLogSettings.Validate().IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void Null_ConsoleLogSettings_should_not_be_valid()
        {
            consoleLogSettings = null;

            consoleLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [Test]
        public void ConsoleLogSettings_with_null_ConversionPattern_should_not_be_valid()
        {
            consoleLogSettings.ConversionPattern = null;

            consoleLogSettings.Validate().IsSuccessful.Should().BeFalse();
        }

        [SetUp]
        public void SetUp()
        {
            consoleLogSettings = new ConsoleLogSettings();
        }
        
        private ConsoleLogSettings consoleLogSettings;
    }
}