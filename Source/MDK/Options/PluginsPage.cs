using System.Windows;
using Malware.MDKUI.Options;
using Microsoft.VisualStudio.Shell;

namespace MDK.Options
{
    /// <summary>
    /// The general options page
    /// </summary>
    public class PluginsPage : UIElementDialogPage
    {
        UIElement _child;
        MDKOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="GeneralPage"/>
        /// </summary>
        public PluginsPage()
        {
            _options = new MDKOptions(ServiceProvider.GlobalProvider);
        }

        /// <inheritdoc />
        protected override UIElement Child
        {
            get
            {
                if (_child == null)
                    _child = CreatePage();
                return _child;
            }
        }

        UIElement CreatePage()
        {
            return new MDKPluginOptionsControl
            {
                Model = new MDKPluginOptionsControlModel(_options, MDKPackage.HelpPageUrl)
            };
        }

        /// <inheritdoc />
        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            _options.Save();
        }

        /// <inheritdoc />
        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
            _options.Revert();
        }
    }
}
