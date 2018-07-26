using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.ConsoleLog.Tests
{
    [TestFixture]
    internal class SettingsValidator_Tests // TODO(krait): write updated tests for validation
    {
        [Test]
        public void Default_ConsoleLogSettings_should_be_valid()
        {
            new Action(() => ConsoleLogSettingsValidator.Validate(new ConsoleLogGlobalSettings()))
                .Should().NotThrow();
        }

        [SetUp]
        public void SetUp()
        {
            consoleLogSettings = new ConsoleLogSettings();
        }
        
        private ConsoleLogSettings consoleLogSettings;
    }
}