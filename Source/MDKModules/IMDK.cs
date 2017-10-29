using System;

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
        /// Shows an error dialog.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="exception"></param>
        void ShowError(string title, string description, Exception exception);

        /// <summary>
        /// Shows a simple message box.
        /// </summary>
        /// <param name="title">The message box title</param>
        /// <param name="description">A description</param>
        /// <param name="type">The message type</param>
        /// <returns>The response to the message</returns>
        MessageResponse ShowMessage(string title, string description, MessageType type);

        /// <summary>
        /// Shows a dialog to select (or just manage) blueprints.
        /// </summary>
        /// <param name="projectOptions">The project options containing the required paths</param>
        /// <param name="customDescription">An optional custom description</param>
        /// <returns>Information about the selected blueprint. May be <see cref="BlueprintInfo.Empty"/></returns>
        BlueprintInfo ShowBlueprintDialog(MDKProjectOptions projectOptions, string customDescription = null);
    }
}