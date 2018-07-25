using Vostok.Logging.Abstractions;
using Vostok.Logging.Console.MessageWriters;

namespace Vostok.Logging.Console
{
    public class ConsoleLog : ILog
    {
        public static void UpdateGlobalSettings(ConsoleLogGlobalSettings newSettings)
        {
            ConsoleLogSettingsValidator.Validate(newSettings);
            ConsoleLogMuxer.Settings = newSettings;
        }

        private readonly ConsoleLogSettings settings;

        public ConsoleLog(ConsoleLogSettings settings)
        {
            this.settings = settings;
        }

        public ConsoleLog()
            : this(new ConsoleLogSettings())
        {
        }

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            ConsoleLogMuxer.Log(@event, settings);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        public ILog ForContext(string context) => this;
    }

    internal class LogEventInfo
    {
        public LogEventInfo(LogEvent @event, IMessageWriter writer)
        {
            Event = @event;
            Writer = writer;
        }

        public LogEvent Event { get; }

        public IMessageWriter Writer { get; }
    }
}