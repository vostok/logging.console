using System.Threading.Tasks;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Console
{
    internal interface IConsoleLogMuxer
    {
        long EventsLost { get; }

        Task FlushAsync();

        bool TryLog(LogEvent @event, ConsoleLogSettings settings);
    }
}