using System;

namespace Vostok.Logging.ConsoleLog
{
    internal class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}