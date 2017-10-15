using System;

namespace MDK.Modularity
{
    public abstract class Disposable : IDisposable, INotifyDisposing
    {
        int _dispose;

        public event EventHandler Disposing;

        public bool IsDisposed => _dispose > 1;

        ~Disposable()
        {
            OnDisposing(false);
        }

        public void Dispose()
        {
            if (_dispose > 0)
                return;
            _dispose++;
            Disposing?.Invoke(this, EventArgs.Empty);
            OnDisposing(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDisposing(bool disposing)
        {
        }
    }
}