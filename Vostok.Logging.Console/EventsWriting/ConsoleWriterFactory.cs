using System;
using System.IO;
using System.Text;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleWriterFactory : IConsoleWriterFactory
    {
        private readonly IConsoleFeaturesDetector featuresDetector;
        private readonly int bufferSize;

        public ConsoleWriterFactory(IConsoleFeaturesDetector featuresDetector, int bufferSize)
        {
            this.featuresDetector = featuresDetector;
            this.bufferSize = bufferSize;
        }

        public IConsoleWriter CreateWriter()
        {
            var colorChanger = featuresDetector.AreColorsSupported ? 
                new ConsoleColorChanger() as IConsoleColorChanger : 
                new FakeConsoleColorChanger();

            // var stream = System.Console.OpenStandardOutput();
            // var writer = new StreamWriter(stream, System.Console.OutputEncoding, bufferSize, true) {AutoFlush = false};

            var writer = new BufferedTextWriter(() => System.Console.Out, bufferSize);
            
            // var writer = ObtainWriter();
            return new ConsoleWriter(writer, colorChanger);
        }

        public TextWriter ObtainWriter()
        {
            return (System.Console.IsOutputRedirected)
                ? System.Console.Out
                : new StreamWriter(System.Console.OpenStandardOutput(), System.Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
        }
    }
    
    public class BufferedTextWriter : TextWriter
    {
        private readonly Func<TextWriter> getWriter;
        public override Encoding Encoding { get; }
        
        private readonly char[] arr;
        private int position = 0;

        private int Remaining => arr.Length - position;

        public BufferedTextWriter(Func<TextWriter> getWriter, int bufferSize)
        {
            this.getWriter = getWriter;
            arr = new char[bufferSize];
        }

        public override void Flush()
        {
            var writer = getWriter();
            writer.Write(arr, 0, position);
            position = 0;
            writer.Flush();
        }

        public override void Write(char[] buffer)
            => Write(new ArraySegment<char>(buffer, 0, buffer.Length));
        
        private void Write(ArraySegment<char> segment)
        {
            var first = Math.Min(Remaining, segment.Count);
            if (first != 0)
                Array.Copy(segment.Array, segment.Offset, arr, position, first);
            position += first;
            var second = segment.Count - first;
            if (second == 0)
                return;
            Flush();
            Array.Copy(segment.Array, segment.Offset + first, arr, position, second);
            position += second;
        }
        

        public override void Write(char value)
        {
            if (Remaining == 0)
                Flush();
            arr[position++] = value;
        }

        public override void Write(char[] buffer, int index, int count)
            => Write(new ArraySegment<char>(buffer, index, count));

        public override void Write(string value)
        {
            var first = Math.Min(Remaining, value.Length);
            if (first != 0)
                CopyStringIntoArray(value, 0, arr, position, first);
            position += first;
            var second = value.Length - first;
            if (second == 0)
                return;
            Flush();
            CopyStringIntoArray(value, first, arr, position, second);
            position += second;
        }

        private static void CopyStringIntoArray(string s, int srcIdx, char[] array, int dstIdx, int length)
        {
            var upper = length - length % 4;
            for (var i = 0; i < upper; i += 4)
            {
                array[dstIdx + i] = s[srcIdx + i];
                array[dstIdx + i + 1] = s[srcIdx + i + 1];
                array[dstIdx + i + 2] = s[srcIdx + i + 2];
                array[dstIdx + i + 3] = s[srcIdx + i + 3];
            }
            
            for (var i = upper; i < length; ++i)
                array[dstIdx + i] = s[srcIdx + i];
        }
    }
}