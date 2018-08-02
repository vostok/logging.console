using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.ConsoleLog.Tests
{
    [TestFixture]
    public class ConsoleLogGlobalSettingsComparer_Tests
    {
        private ConsoleLogGlobalSettingsComparer comparer;

        [SetUp]
        public void TestSetup()
        {
            comparer = new ConsoleLogGlobalSettingsComparer();
        }

        [Test]
        public void Should_be_equal_by_fields()
        {
            var s1 = new ConsoleLogGlobalSettings();
            var s2 = new ConsoleLogGlobalSettings();

            comparer.Equals(s1, s2).Should().BeTrue();
        }

        [Test]
        public void Should_be_equal_by_nulls()
        {
            comparer.Equals(null, null).Should().BeTrue();
        }

        [Test]
        public void Should_not_be_equal_by_difference_in_EventsQueueCapacity()
        {
            var s1 = new ConsoleLogGlobalSettings { EventsQueueCapacity = 1 };
            var s2 = new ConsoleLogGlobalSettings();

            comparer.Equals(s1, s2).Should().BeFalse();
        }

        [Test]
        public void Should_not_be_equal_by_difference_in_OutputBufferSize()
        {
            var s1 = new ConsoleLogGlobalSettings { OutputBufferSize = 1 };
            var s2 = new ConsoleLogGlobalSettings();

            comparer.Equals(s1, s2).Should().BeFalse();
        }
    }
}