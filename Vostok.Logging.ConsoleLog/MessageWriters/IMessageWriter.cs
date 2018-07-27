using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog.MessageWriters
{
    internal interface IMessageWriter
    {
        void Write(LogEvent @event);
    }
}