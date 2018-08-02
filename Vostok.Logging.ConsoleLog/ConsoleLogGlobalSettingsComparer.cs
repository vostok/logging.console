using System;
using System.Collections.Generic;

namespace Vostok.Logging.ConsoleLog
{
    internal class ConsoleLogGlobalSettingsComparer : IEqualityComparer<ConsoleLogGlobalSettings>
    {
        public static readonly ConsoleLogGlobalSettingsComparer Instance
            = new ConsoleLogGlobalSettingsComparer();

        public bool Equals(ConsoleLogGlobalSettings x, ConsoleLogGlobalSettings y) =>
            x?.EventsQueueCapacity == y?.EventsQueueCapacity &&
            x?.OutputBufferSize == y?.OutputBufferSize;

        public int GetHashCode(ConsoleLogGlobalSettings settings) => throw new NotSupportedException();
    }
}
