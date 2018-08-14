using System;

namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IConsoleColorChanger : IDisposable
    {
        IDisposable ChangeColor(ConsoleColor newColor);
    }
}