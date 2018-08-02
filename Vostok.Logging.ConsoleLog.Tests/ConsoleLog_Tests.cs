using System;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable ObjectCreationAsStatement
// ReSharper disable AssignNullToNotNullAttribute

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

            ConsoleLogMuxer.Settings.Should().NotBeSameAs(settings);

            ConsoleLog.UpdateGlobalSettings(settings);

            ConsoleLogMuxer.Settings.Should().BeSameAs(settings);
        }

        [Test]
        public void UpdateGlobalSettings_should_validate_settings()
        {
            var settings = new ConsoleLogGlobalSettings
            {
                EventsQueueCapacity = -100
            };

            new Action(() => ConsoleLog.UpdateGlobalSettings(settings)).Should().Throw<ArgumentOutOfRangeException>();

            ConsoleLogMuxer.Settings.Should().NotBeSameAs(settings);
        }

        [Test]
        public void ConsoleLog_should_validate_settings()
        {
            var settings = new ConsoleLogSettings
            {
                OutputTemplate = null,
            };

            new Action(() => new ConsoleLog(settings)).Should().Throw<ArgumentNullException>();
            ConsoleLogMuxer.Settings.Should().NotBe(settings);
        }
    }
}