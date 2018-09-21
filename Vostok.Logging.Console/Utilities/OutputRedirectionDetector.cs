using System;
using System.IO;
using System.Linq.Expressions;

namespace Vostok.Logging.Console.Utilities
{
    internal static class OutputRedirectionDetector
    {
        private static Func<TextWriter, TextWriter> action;

        public static bool IsOutputRedirected()
        {
            Init();
            
            try
            {
                var internalTextWriter = action(System.Console.Out);
                
                if (!(internalTextWriter is StreamWriter streamWriter))
                    return true;
                
                var name = streamWriter.BaseStream?.GetType().FullName;
                if (name == null)
                    return false;
                
                return !(name.StartsWith("System.") && name.EndsWith("ConsoleStream"));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void Init()
        {
            if (action != null)
                return;

            try
            {
                var parameter = Expression.Parameter(typeof(TextWriter));
                action = Expression.Lambda<Func<TextWriter, TextWriter>>(
                        Expression.Field(
                            Expression.Convert(parameter, System.Console.Out.GetType()),
                            "_out"),
                        parameter)
                    .Compile();
            }
            catch
            {
                action = _ => null;
            }
        }
    }
}