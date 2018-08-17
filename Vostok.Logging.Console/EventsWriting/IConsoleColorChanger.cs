using System;

namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IConsoleColorChanger
    {
        IDisposable ChangeColor(ConsoleColor newColor);
    }
}