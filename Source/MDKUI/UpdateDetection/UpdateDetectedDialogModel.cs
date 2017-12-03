using System;
using Malware.MDKUI.Options;

namespace Malware.MDKUI.UpdateDetection
{
    /// <summary>
    /// The view model for <see cref="ScriptOptionsDialog"/>
    /// </summary>
    public class UpdateDetectedDialogModel : DialogViewModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScriptOptionsDialogModel"/>
        /// </summary>
        /// <param name="version">A valid version available for download, or <c>null</c> if no new version is available.</param>
        /// <param name="helpPageUrl"></param>
        /// <param name="releasePageUrl"></param>
        public UpdateDetectedDialogModel(Version version, string helpPageUrl, string releasePageUrl)
            : base(helpPageUrl)
        {
            Version = version;
            ReleasePageUrl = releasePageUrl;
        }

        /// <summary>
        /// Determines whether there's a new version to download.
        /// </summary>
        public bool HasNewVersion => Version != null;

        /// <summary>
        /// Gets the currently detected extension version
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// The release page URL
        /// </summary>
        public string ReleasePageUrl { get; }

        /// <summary>
        /// Saves any changed options
        /// </summary>
        /// <returns></returns>
        protected override bool OnSave()
        {
            System.Diagnostics.Process.Start(ReleasePageUrl);
            return true;
        }
    }
}
