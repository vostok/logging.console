using System.Collections.Generic;
using Vostok.Commons.Conversions;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    public class ConsoleLog : ILog
    {
        private readonly ConsoleLogSettings settings;

        public ConsoleLog(ConsoleLogSettings settings)
        {
            this.settings = settings;
        }

        public ConsoleLog()
            : this(new ConsoleLogSettings())
        {
        }

        public static void UpdateGlobalSettings(ConsoleLogGlobalSettings newSettings)
        {
            ConsoleLogSettingsValidator.Validate(newSettings);

            ConsoleLogMuxer.Settings = newSettings;
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