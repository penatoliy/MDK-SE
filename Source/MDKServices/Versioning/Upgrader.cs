using Malware.MDKModules;

namespace Malware.MDKServices.Versioning
{
    /// <summary>
    /// Base class for upgrading MDK projects from a specific previous version to a higher specific version
    /// </summary>
    public abstract class Upgrader
    {
        /// <summary>
        /// Upgrade the project represented by the given analysis results
        /// </summary>
        /// <param name="projectOptions"></param>
        public abstract void Upgrade(MDKProjectOptions projectOptions);
    }
}
