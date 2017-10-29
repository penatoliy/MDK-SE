using System;

namespace Malware.MDKModules
{
    /// <summary>
    /// A base class for disposable objects
    /// </summary>
    public abstract class Disposable : IDisposable, INotifyDisposing
    {
        int _dispose;

        /// <summary>
        /// Fired when this object is about to be disposed
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Determines whether this object has been disposed
        /// </summary>
        public bool IsDisposed => _dispose > 1;

        /// <summary>
        /// Finalizes the object, making sure the dispose gets run
        /// if it hasn't already
        /// </summary>
        ~Disposable()
        {
            OnDisposing(false);
        }

        /// <summary>
        /// Manually dispose of this object
        /// </summary>
        public void Dispose()
        {
            if (_dispose > 0)
                return;
            _dispose++;
            Disposing?.Invoke(this, EventArgs.Empty);
            OnDisposing(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called to dispose of this object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void OnDisposing(bool disposing)
        {
        }
    }
}