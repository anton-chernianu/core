using System;
using ReactUnity.Dispatchers;

namespace ReactUnity.Helpers
{
    internal class DisposableHandle : IDisposable
    {
        IDispatcher Dispatcher;
        int Handle;

        public DisposableHandle(IDispatcher dispatcher, int handle)
        {
            Dispatcher = dispatcher;
            Handle = handle;
        }

        public void Dispose()
        {
            Dispatcher.StopDeferred(Handle);
        }
    }
}
