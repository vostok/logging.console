using System;
using System.Collections.Generic;

namespace Vostok.Logging.ConsoleLog
{
    internal class GlobalSettingsComparer : IEqualityComparer<ConsoleLogGlobalSettings>
    {
        public bool Equals(ConsoleLogGlobalSettings x, ConsoleLogGlobalSettings y) =>
            x?.EventsQueueCapacity == y?.EventsQueueCapacity;

        public int GetHashCode(ConsoleLogGlobalSettings obj) => throw new NotSupportedException();
    }
}