using System;

namespace Vostok.Logging.Console
{
    internal struct ConsoleColorChanger : IDisposable
    {
        private readonly ConsoleColor oldColor;

        public ConsoleColorChanger(ConsoleColor newColor)
        {
            oldColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = newColor;
        }

        public void Dispose() =>
            System.Console.ForegroundColor = oldColor;
    }
}