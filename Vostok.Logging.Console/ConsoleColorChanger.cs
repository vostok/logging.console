namespace Vostok.Logging.Console
{
    internal struct ConsoleColorChanger : IDisposable
    {
        private readonly ConsoleColor oldColor;

        public ConsoleColorChanger(ConsoleColor newColor)
        {
            oldColor = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
        }

        public void Dispose() =>
            Console.ForegroundColor = oldColor;
    }
}