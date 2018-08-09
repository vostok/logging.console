using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 420

namespace Vostok.Logging.ConsoleLog
{
    internal class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> state = new TaskCompletionSource<bool>();

        public AsyncManualResetEvent(bool isSetInitially)
        {
            if (isSetInitially)
                Set();
        }

        public void Set() => state.TrySetResult(true);

        public void Reset()
        {
            while (true)
            {
                var currentState = state;
                if (!currentState.Task.IsCompleted)
                    return;
                if (Interlocked.CompareExchange(ref state, new TaskCompletionSource<bool>(), currentState) == currentState)
                    return;
            }
        }

        public Task WaitAsync() => state.Task;
    }
}