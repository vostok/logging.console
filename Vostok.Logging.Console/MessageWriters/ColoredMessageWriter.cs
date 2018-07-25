using Vostok.Logging.Abstractions;
using Vostok.Logging.Core.ConversionPattern;

namespace Vostok.Logging.Console.MessageWriters
{
    internal class ColoredMessageWriter : IMessageWriter
    {
        private readonly ConversionPattern pattern;
        private readonly Dictionary<LogLevel, ConsoleColor> colorMapping;

        public ColoredMessageWriter(ConversionPattern pattern, Dictionary<LogLevel, ConsoleColor> colorMapping)
        {
            this.pattern = pattern;
            this.colorMapping = colorMapping;
        }

        public void Write(LogEvent @event)
        {
            if (!colorMapping.TryGetValue(@event.Level, out var color))
                color = ConsoleColor.Gray;

            using (new ConsoleColorChanger(color))
                ConversionPatternRenderer.Render(pattern, @event, Console.Out);
        }

        public void Flush()
        {
        }
    }
}