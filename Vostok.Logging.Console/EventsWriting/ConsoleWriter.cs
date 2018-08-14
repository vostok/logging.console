using System;
using System.IO;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleWriter : IConsoleWriter
    {
        private readonly TextWriter writer;
        private readonly IConsoleColorChanger colorChanger;

        public ConsoleWriter(TextWriter writer, IConsoleColorChanger colorChanger)
        {
            this.writer = writer;
            this.colorChanger = colorChanger;
        }

        public void WriteLogEvent(LogEventInfo eventInfo) =>
            LogEventFormatter.Format(eventInfo.Event, writer, eventInfo.Settings.OutputTemplate, eventInfo.Settings.FormatProvider);

        public IDisposable ChangeColor(ConsoleColor newColor) => colorChanger.ChangeColor(newColor);

        public void Flush() => writer.Flush();
    }
}