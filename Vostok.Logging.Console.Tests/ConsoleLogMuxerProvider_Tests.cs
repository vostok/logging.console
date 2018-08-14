using System;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.Console.Tests
{
    [TestFixture]
    public class ConsoleLogMuxerProvider_Tests
    {
        private ConsoleLogMuxerProvider muxerProvider;

        [SetUp]
        public void TestSetup()
        {
            muxerProvider = new ConsoleLogMuxerProvider();
        }

        [Test]
        public void Should_validate_settings()
        {
            var invalidSettings = new ConsoleLogGlobalSettings {EventsQueueCapacity = -1};

            new Action(() => muxerProvider.UpdateSettings(invalidSettings))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Should_create_muxer_only_once()
        {
            var muxer1 = muxerProvider.ObtainMuxer();

            muxerProvider.UpdateSettings(new ConsoleLogGlobalSettings { EventsQueueCapacity = 10 });

            var muxer2 = muxerProvider.ObtainMuxer();

            muxer2.Should().BeSameAs(muxer1);
        }
    }
}