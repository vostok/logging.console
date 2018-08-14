using System.IO;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleWriter : IConsoleWriter
    {
        private readonly StreamWriter writer;

        public ConsoleWriter(int bufferSize)
        {
            var stream = System.Console.OpenStandardOutput(bufferSize);
            writer = new StreamWriter(stream, System.Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
        }

        public void WriteLogEvent(LogEventInfo eventInfo) =>
            LogEventFormatter.Format(eventInfo.Event, writer, eventInfo.Settings.OutputTemplate, eventInfo.Settings.FormatProvider);

        public void Flush() => writer.Flush();
    }
}