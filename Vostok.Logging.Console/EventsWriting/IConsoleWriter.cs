﻿using System;

namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IConsoleWriter
    {
        void WriteLogEvent(LogEventInfo eventInfo);

        IDisposable ChangeColor(ConsoleColor newColor);

        void Flush();
    }
}