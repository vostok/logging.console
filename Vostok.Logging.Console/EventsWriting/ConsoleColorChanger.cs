using System;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleColorChanger : IConsoleColorChanger
    {
        private ConsoleColor oldColor;

        public IDisposable ChangeColor(ConsoleColor newColor)
        {
            oldColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = newColor;
            return this;
        }

        public void Dispose() =>
            System.Console.ForegroundColor = oldColor;
    }
}