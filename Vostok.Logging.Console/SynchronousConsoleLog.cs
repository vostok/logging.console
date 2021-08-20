using System;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console
{
    /// <summary>
    /// <para>A log which outputs events to console.</para>
    /// <para>In contrast to <see cref="ConsoleLog"/>, this implementation is synchronous: logged messages are immediately rendered and written to console.</para>
    /// <para>It has much lower throughput than <see cref="ConsoleLog"/> and is only recommended for testing and debugging purposes.</para>
    /// </summary>
    [PublicAPI]
    public class SynchronousConsoleLog : ILog
    {
        private static readonly ConsoleLogSettings DefaultSettings = new ConsoleLogSettings();

        private static readonly object Sync = new object();

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

            var str = LogEventFormatter.Format(@event, settings.OutputTemplate, settings.FormatProvider);

            lock (Sync)
                System.Console.Out.Write(str);
        }

        /// <inheritdoc />
        public bool IsEnabledFor(LogLevel level) => true;

        /// <inheritdoc />
        public ILog ForContext(string context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return new SourceContextWrapper(this, context);
        }
    }
}