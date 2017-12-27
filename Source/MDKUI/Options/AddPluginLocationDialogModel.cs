using System;
using System.IO;

namespace Malware.MDKUI.Options
{
    /// <summary>
    /// The view model for <see cref="ScriptOptionsDialog"/>
    /// </summary>
    public class AddPluginLocationDialogModel : DialogViewModel
    {
        static readonly Uri GitHubPath = new Uri("https://github.com/");

        string _uri;
        string _error;

        /// <summary>
        /// Creates a new instance of <see cref="ScriptOptionsDialogModel"/>
        /// </summary>
        /// <param name="helpPageUrl"></param>
        public AddPluginLocationDialogModel(string helpPageUrl)
            : base(helpPageUrl)
        { }

        public string Error
        {
            get => _error;
            set
            {
                if (value == _error)
                    return;
                _error = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current install path for the package
        /// </summary>
        public string Uri
        {
            get => _uri;
            set
            {
                if (value == _uri)
                    return;
                _uri = value;
                OnPropertyChanged();
                Validate();
            }
        }

        void Validate()
        {
            ClearErrors(nameof(Uri));
            if (!System.Uri.TryCreate(Uri, UriKind.Absolute, out var uri))
            {
                AddError(nameof(Uri), "This is not a valid, absolute URI");
                return;
            }

            if (uri.IsFile)
            {
                ValidateFilePath(uri.LocalPath);
            }
            else 
            {
                ValidateGitHub(uri);
            }
        }

        void ValidateGitHub(Uri uri)
        {
            if (!GitHubPath.IsBaseOf(uri))
            {
                AddError(nameof(Uri), "Expected a GitHub URL");
                return;
            }
        }

        void ValidateFilePath(string uriLocalPath)
        {
            var pluginPath = Path.Combine(uriLocalPath, "plugin.xml");
            if (!File.Exists(pluginPath))
            {
                AddError(nameof(Uri), "Cannot find a plugin at this location");
                return;
            }
        }

        protected override void OnIsValidChanged()
        {
            Error = string.Join(Environment.NewLine, GetErrors(nameof(Uri)));
            base.OnIsValidChanged();
        }

        /// <summary>
        /// Saves any changed options
        /// </summary>
        /// <returns></returns>
        protected override bool OnSave()
        {
            return IsValid;
        }
    }
}
