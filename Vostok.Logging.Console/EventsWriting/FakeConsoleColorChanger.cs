using System;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class FakeConsoleColorChanger : IConsoleColorChanger
    {
        public IDisposable ChangeColor(ConsoleColor newColor) => this;

        public void Dispose()
        {
        }
    }
}