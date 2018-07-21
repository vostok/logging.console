using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    internal interface IMessageWriter
    {
        void Write(LogEvent @event);

        void Flush();
    }
}