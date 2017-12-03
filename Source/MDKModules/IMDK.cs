namespace Malware.MDKModules
{
    /// <summary>
    /// Interface to the MDK services
    /// </summary>
    public interface IMDK
    {
        /// <summary>
        /// Gets the global MDK options
        /// </summary>
        IMDKOptions Options { get; }

        /// <summary>
        /// Utilities to show common dialogs
        /// </summary>
        IMDKDialogs Dialogs { get; }

        /// <summary>
        /// Gets access to the output pane
        /// </summary>
        IOutputPane OutputPane { get; }

        /// <summary>
        /// Expands project macros.
        /// </summary>
        /// <param name="build"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        string ExpandMacros(Build build, string source);

        /// <summary>
        /// Wraps the given script in its outer Program class.
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        string WrapScript(string script);
    }
}