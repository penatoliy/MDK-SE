using System;

namespace Malware.MDKModules
{
    /// <summary>
    /// Represents a build module.
    /// </summary>
    public interface IModule : IDisposable, INotifyDisposing
    {
        /// <summary>
        /// Gets the identity of this particular module
        /// </summary>
        ModuleIdentity Identity { get; }

        /// <summary>
        /// Called when MDK is preparing to run a batch. This method is called before any module is invoked.
        /// </summary>
        /// <param name="mdk"></param>
        void BeginBatch(IMDK mdk);

        /// <summary>
        /// Called when a batch is complete.
        /// </summary>
        /// <param name="error"><c>null</c> if the build was successful, otherwise the exception which caused the build to fail.</param>
        void EndBatch(Exception error = null);
    }
}
