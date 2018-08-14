using System;
using System.IO;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleWriter : IConsoleWriter
    {
        private readonly IConsoleColorChanger colorChanger;
        private readonly TextWriter writer;

        public ConsoleWriter(int bufferSize)
        {
            if (System.Console.IsOutputRedirected)
            {
                writer = System.Console.Out;
                colorChanger = new FakeConsoleColorChanger();
            }
            else
            {
                var stream = System.Console.OpenStandardOutput(bufferSize);
                writer = new StreamWriter(stream, System.Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
                colorChanger = new ConsoleColorChanger();
            }
        }

        public void WriteLogEvent(LogEventInfo eventInfo) =>
            LogEventFormatter.Format(eventInfo.Event, writer, eventInfo.Settings.OutputTemplate, eventInfo.Settings.FormatProvider);

        public IDisposable ChangeColor(ConsoleColor newColor) => colorChanger.ChangeColor(newColor);

        public void Flush() => writer.Flush();
    }
}