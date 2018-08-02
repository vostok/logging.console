using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.ConsoleLog.Tests
{
    [TestFixture]
    public class ConsoleLog_Tests
    {
        [Test]
        public void UpdateGlobalSettings_should_throw_exception_if_new_settings_is_null()
        {
            new Action(() => ConsoleLog.UpdateGlobalSettings(null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void UpdateGlobalSettings_should_update_validated_settings()
        {
            var settings = new ConsoleLogGlobalSettings
            {
                EventsQueueCapacity = 100,
                OutputBufferSize = 200,
            };

            ConsoleLogMuxer.Settings.Should().NotBe(settings);
            ConsoleLog.UpdateGlobalSettings(settings);
            ConsoleLogMuxer.Settings.Should().Be(settings);
        }

        [Test]
        public void UpdateGlobalSettings_should_validate_settings()
        {
            var settings = new ConsoleLogGlobalSettings
            {
                EventsQueueCapacity = -100,
            };

            new Action(() => ConsoleLog.UpdateGlobalSettings(settings)).Should().Throw<ValidationException>();
            ConsoleLogMuxer.Settings.Should().NotBe(settings);
        }

        [Test]
        public void ConsoleLog_should_get_not_null_settings()
        {
            new Action(() => new ConsoleLog(null)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ConsoleLog_should_validate_settings()
        {
            var settings = new ConsoleLogSettings()
            {
                OutputTemplate = null,
            };

            new Action(() => new ConsoleLog(settings)).Should().Throw<ValidationException>();
            ConsoleLogMuxer.Settings.Should().NotBe(settings);
        }
    }
}