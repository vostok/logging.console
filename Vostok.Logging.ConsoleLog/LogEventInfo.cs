﻿using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    internal class LogEventInfo
    {
        public LogEventInfo(LogEvent @event, ConsoleLogSettings settings)
        {
            Event = @event;
            Settings = settings;
        }

        public LogEvent Event { get; }

        public ConsoleLogSettings Settings { get; }
    }
}