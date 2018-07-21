namespace Vostok.Logging.ConsoleLog
{
    internal static class MessageWriterFactory
    {
        public static IMessageWriter Create(ConsoleLogSettings settings)
        {
            if (settings.ColorsEnabled)
                return new ColoredMessageWriter(settings.ConversionPattern, settings.ColorMapping);
            
            return new ColorlessMessageWriter(settings.ConversionPattern, settings.StreamBufferSize);
        }
    }
}