using Vostok.Logging.Abstractions;
using Vostok.Logging.Core.ConversionPattern;

namespace Vostok.Logging.Console.MessageWriters
{
    internal class ColorlessMessageWriter : IMessageWriter
    {
        private readonly TextWriter writer;
        private readonly ConversionPattern pattern;

        private bool isDirty;

        public ColorlessMessageWriter(ConversionPattern pattern, int bufferSize)
        {
            this.pattern = pattern;

            var stream = Console.OpenStandardOutput(bufferSize);
            writer = new StreamWriter(stream);
        }

        public void Write(LogEvent @event)
        {
            ConversionPatternRenderer.Render(pattern, @event, writer);
            isDirty = true;
        }

        public void Flush()
        {
            if (isDirty)
                writer.Flush();
        }
    }
}