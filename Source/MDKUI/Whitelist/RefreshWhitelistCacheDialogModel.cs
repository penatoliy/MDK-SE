using Malware.MDKServices;
using Malware.MDKUI.Options;

namespace Malware.MDKUI.Whitelist
{
    /// <summary>
    /// The view model for <see cref="ScriptOptionsDialog"/>
    /// </summary>
    public class RefreshWhitelistCacheDialogModel : DialogViewModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScriptOptionsDialogModel"/>
        /// </summary>
        /// <param name="installPath"></param>
        /// <param name="dte"></param>
        /// <param name="helpPageUrl"></param>
        public RefreshWhitelistCacheDialogModel(string installPath, string helpPageUrl)
            : base(helpPageUrl)
        {
            InstallPath = installPath;
        }

        /// <summary>
        /// The current install path for the package
        /// </summary>
        public string InstallPath { get; }

        /// <summary>
        /// Saves any changed options
        /// </summary>
        /// <returns></returns>
        protected override bool OnSave()
        {
            var cache = new WhitelistCache();
            cache.Refresh(InstallPath);
            return true;
        }
    }
}
