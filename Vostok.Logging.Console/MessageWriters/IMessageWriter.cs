using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Console.MessageWriters
{
    internal interface IMessageWriter
    {
        void Write(LogEvent @event);

        void Flush();
    }
}