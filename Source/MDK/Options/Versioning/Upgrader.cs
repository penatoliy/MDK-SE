namespace MDK.Options.Versioning
{
    /// <summary>
    /// Base class for upgrading MDK options from a specific previous version to a higher specific version
    /// </summary>
    public abstract class Upgrader
    {
        /// <summary>
        /// Upgrade the package
        /// </summary>
        /// <param name="package"></param>
        /// <param name="options"></param>
        public abstract void Upgrade(MDKPackage package, MDKOptions options);
    }
}
