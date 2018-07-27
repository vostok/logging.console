using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.ConsoleLog.Tests
{
    [TestFixture]
    internal class SettingsValidator_Tests
    {
        [Test]
        public void ValidateGlobalSettings_should_consider_default_settings_valid()
        {
            new Action(() => SettingsValidator.ValidateGlobalSettings(new ConsoleLogGlobalSettings()))
                .Should().NotThrow();
        }

        // TODO(krait): More tests.
    }
}