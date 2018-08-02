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

        [Test]
        public void ValidateGlobalSettings_should_throw_exception_on_null_settings()
        {
            new Action(() => SettingsValidator.ValidateGlobalSettings(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ValidateGlobalSettings_should_throw_exception_if_EventsQueueCapacity_is_not_positive()
        {
            var settings = new ConsoleLogGlobalSettings { EventsQueueCapacity = 0 };
            new Action(() => SettingsValidator.ValidateGlobalSettings(settings))
                .Should().Throw<ArgumentOutOfRangeException>();

            settings.EventsQueueCapacity = -1;
            new Action(() => SettingsValidator.ValidateGlobalSettings(settings))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void ValidateInstanceSettings_should_throw_exception_on_null_settings()
        {
            new Action(() => SettingsValidator.ValidateInstanceSettings(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ValidateInstanceSettings_should_throw_exception_if_OutputTemplate_is_null()
        {
            var settings = new ConsoleLogSettings { OutputTemplate = null };
            new Action(() => SettingsValidator.ValidateInstanceSettings(settings))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ValidateInstanceSettings_should_throw_exception_if_ColorMapping_is_null()
        {
            var settings = new ConsoleLogSettings { ColorMapping = null };
            new Action(() => SettingsValidator.ValidateInstanceSettings(settings))
                .Should().Throw<ArgumentNullException>();
        }
    }
}