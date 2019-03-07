using System;
using System.IO;
using JetBrains.Annotations;
using Vostok.Commons.Collections;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;
using Vostok.Logging.Console.EventsWriting;

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

        private static readonly object sync = new object();

        private readonly ConsoleLogSettings settings;

        private readonly CachingTransform<TextWriter, IEventsWriter> transform
            = new CachingTransform<TextWriter, IEventsWriter>(_ => CreateEventsWriter(), () => System.Console.Out);

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

            lock (sync)
            {
                transform.Get()
                    .WriteEvents(
                        new[]
                        {
                            new LogEventInfo(@event, settings)
                        },
                        1);
            }
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

        private static EventsWriter CreateEventsWriter()
        {
            var consoleFeaturesDetector = new ConsoleFeaturesDetector();
            var consoleWriterFactory = new ConsoleWriterFactory(consoleFeaturesDetector, 0);
            var consoleWriter = consoleWriterFactory.CreateWriter(true);
            var eventsBatcher = new EventsBatcher(consoleFeaturesDetector);
            return new EventsWriter(eventsBatcher, consoleWriter, consoleFeaturesDetector);
        }
    }
}