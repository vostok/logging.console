﻿using System;
using System.Collections.Generic;

namespace Vostok.Logging.ConsoleLog
{
    // TODO(krait): Tests.
    internal class GlobalSettingsComparer : IEqualityComparer<ConsoleLogGlobalSettings>
    {
        public bool Equals(ConsoleLogGlobalSettings x, ConsoleLogGlobalSettings y) =>
            x?.EventsQueueCapacity == y?.EventsQueueCapacity &&
            x?.OutputBufferSize == y?.OutputBufferSize;

        public int GetHashCode(ConsoleLogGlobalSettings obj) => throw new NotSupportedException();
    }
}