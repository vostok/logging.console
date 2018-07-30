using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.ConsoleLog.Tests
{
    [TestFixture]
    public class GlobalSettingsComparer_Tests
    {
        [Test]
        public void Should_be_equal_by_fields()
        {
            var s1 = new ConsoleLogGlobalSettings();
            var s2 = new ConsoleLogGlobalSettings();

            new GlobalSettingsComparer().Equals(s1, s2).Should().BeTrue();
        }

        [Test]
        public void Should_be_equal_by_nulls()
        {
            new GlobalSettingsComparer().Equals(null, null).Should().BeTrue();
        }

        [Test]
        public void Should_not_be_equal_by_difference_in_EventsQueueCapacity()
        {
            var s1 = new ConsoleLogGlobalSettings { EventsQueueCapacity = 1 };
            var s2 = new ConsoleLogGlobalSettings();

            new GlobalSettingsComparer().Equals(s1, s2).Should().BeFalse();
        }

        [Test]
        public void Should_not_be_equal_by_difference_in_OutputBufferSize()
        {
            var s1 = new ConsoleLogGlobalSettings { OutputBufferSize = 1 };
            var s2 = new ConsoleLogGlobalSettings();

            new GlobalSettingsComparer().Equals(s1, s2).Should().BeFalse();
        }
    }
}