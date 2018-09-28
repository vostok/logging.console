using System;
using System.IO;
using System.Linq.Expressions;

namespace Vostok.Logging.Console.Utilities
{
    internal static class OutputRedirectionDetector
    {
        private static Func<TextWriter, TextWriter> extractBaseWriter;

        public static bool IsOutputRedirected()
        {
            Init();

            try
            {
                var internalTextWriter = extractBaseWriter(System.Console.Out);

                if (internalTextWriter == null)
                    return false;

                if (!(internalTextWriter is StreamWriter streamWriter))
                    return true;

                var name = streamWriter.BaseStream?.GetType().FullName;
                if (name == null)
                    return false;

                return !(name.StartsWith("System.", StringComparison.Ordinal) && name.EndsWith("ConsoleStream", StringComparison.Ordinal));
            }
            catch
            {
                return false;
            }
        }

        private static void Init()
        {
            if (extractBaseWriter != null)
                return;

            try
            {
                var wrappedWriter = Expression.Parameter(typeof(TextWriter));

                extractBaseWriter = Expression.Lambda<Func<TextWriter, TextWriter>>(
                    Expression.Field(Expression.Convert(wrappedWriter, System.Console.Out.GetType()), "_out"), wrappedWriter)
                    .Compile();
            }
            catch
            {
                extractBaseWriter = _ => null;
            }
        }
    }
}