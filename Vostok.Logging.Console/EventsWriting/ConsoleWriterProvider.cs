using System.IO;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleWriterProvider : IConsoleWriterProvider
    {
        private readonly int bufferSize;
        private (TextWriter stdout, IConsoleWriter writer) currentState;

        public ConsoleWriterProvider(int bufferSize) =>
            this.bufferSize = bufferSize;

        public IConsoleWriter ObtainWriter()
        {
            var newStdout = System.Console.IsOutputRedirected ? System.Console.Out : null;
            if (currentState == default || currentState.stdout != newStdout)
                currentState = (newStdout, CreateWriter(newStdout));

            return currentState.writer;
        }

        private IConsoleWriter CreateWriter(TextWriter newStdout)
        {
            if (newStdout != null)
            {
                return new ConsoleWriter(newStdout, new FakeConsoleColorChanger());
            }

            var stream = System.Console.OpenStandardOutput(bufferSize);
            var writer = new StreamWriter(stream, System.Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
            return new ConsoleWriter(writer, new ConsoleColorChanger());
        }
    }
}