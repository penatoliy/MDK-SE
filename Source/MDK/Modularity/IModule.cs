using System;

namespace MDK.Modularity
{
    /// <summary>
    /// Represents a build module.
    /// </summary>
    public interface IModule : IDisposable, INotifyDisposing
    {
        /// <summary>
        /// Called when preparing a build
        /// </summary>
        /// <param name="mdk"></param>
        void Begin(IMDK mdk);

        /// <summary>
        /// Called when a build is complete.
        /// </summary>
        /// <param name="error"><c>null</c> if the build was successful, otherwise the exception which caused the build to fail.</param>
        void End(Exception error = null);
    }
}
