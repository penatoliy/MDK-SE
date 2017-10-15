using System;

namespace MDK.Modularity
{
    /// <summary>
    /// Adds common members to inform about when or whether a disposable object has been disposed.
    /// </summary>
    public interface INotifyDisposing
    {
        /// <summary>
        /// Fired when the object is about to be disposed
        /// </summary>
        event EventHandler Disposing;

        /// <summary>
        /// <c>true</c> if this object has been disposed
        /// </summary>
        bool IsDisposed { get; }
    }
}