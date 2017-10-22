using System;

namespace Malware.MDKModules
{
    /// <summary>
    /// An optional standardized base class for modules, provided for simplicity
    /// </summary>
    public abstract class Module : Disposable, IModule
    {
        /// <inheritdoc />
        public abstract ModuleIdentity Identity { get; }

        /// <summary>
        /// The MDK system
        /// </summary>
        public IMDK MDK { get; private set; }

        /// <summary>
        /// Determines whether this batch is in a faulted state.
        /// </summary>
        public bool IsBatchFaulted => BatchException != null;

        /// <summary>
        /// Contains the exception that caused this batch to fault.
        /// </summary>
        public Exception BatchException { get; private set; }

        /// <inheritdoc />
        public void BeginBatch(IMDK mdk)
        {
            BatchException = null;
            MDK = mdk;
            OnBeginBatch();
        }

        /// <inheritdoc />
        public void EndBatch(Exception error = null)
        {
            BatchException = error;
            OnEndBatch();
        }

        /// <summary>
        /// Called when a processing batch is beginning. This is called before any module is invoked.
        /// </summary>
        protected virtual void OnBeginBatch()
        { }

        /// <summary>
        /// Called when a processing batch is completed. This is called after all modules have been invoked.
        /// Remember to check the <see cref="IsBatchFaulted"/> property to see if the batch was successful.
        /// </summary>
        protected virtual void OnEndBatch()
        { }
    }
}