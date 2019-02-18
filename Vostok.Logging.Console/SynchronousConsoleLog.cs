using System;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;
using Vostok.Logging.Console.EventsWriting;

namespace Vostok.Logging.Console
{
    /// <summary>
    /// <para>A log which outputs events to console.</para>
    /// <para>
    ///     The implementation is synchronous: logged messages are immediately rendered and written to console.
    /// </para>
    /// </summary>
    [PublicAPI]
    public class SynchronousConsoleLog : ILog
    {
        private const int OutputBufferSize = 65536;

        private static readonly ConsoleLogSettings DefaultSettings = new ConsoleLogSettings();

        private readonly ConsoleLogSettings settings;

        /// <summary>
        /// <para>Create a new console log with the given settings.</para>
        /// <para>An exception will be thrown if the provided <paramref name="settings" /> are invalid.</para>
        /// </summary>
        public SynchronousConsoleLog([NotNull] ConsoleLogSettings settings)
        {
            this.settings = SettingsValidator.ValidateInstanceSettings(settings);
        }

        /// <summary>
        /// Create a new console log with default settings. Colors are enabled by default.
        /// </summary>
        public SynchronousConsoleLog()
            : this(DefaultSettings)
        {
        }

        /// <inheritdoc />
        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            CreateEventsWriter(OutputBufferSize).WriteLogEvent(new LogEventInfo(@event, settings));
        }

        /// <inheritdoc />
        public bool IsEnabledFor(LogLevel level) => true;

        /// <summary>
        /// Returns a log based on this <see cref="ConsoleLog" /> instance that puts given <paramref name="context" /> string into
        /// <see
        ///     cref="F:Vostok.Logging.Abstractions.WellKnownProperties.SourceContext" />
        /// property of all logged events.
        /// </summary>
        public ILog ForContext(string context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return new SourceContextWrapper(this, context);
        }

        private static IConsoleWriter CreateEventsWriter(int outputBufferSize)
        {
            var consoleFeaturesDetector = new ConsoleFeaturesDetector();
            var consoleWriterFactory = new ConsoleWriterFactory(consoleFeaturesDetector, outputBufferSize);
            return consoleWriterFactory.CreateWriter();
        }
    }
}