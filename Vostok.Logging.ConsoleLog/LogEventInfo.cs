using Vostok.Logging.Abstractions;
using Vostok.Logging.ConsoleLog.MessageWriters;

namespace Vostok.Logging.ConsoleLog
{
    internal class LogEventInfo
    {
        public LogEventInfo(LogEvent @event, IMessageWriter writer)
        {
            Event = @event;
            Writer = writer;
        }

        public LogEvent Event { get; }

        public IMessageWriter Writer { get; }
    }
}