using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Malware.MDKServices;
using Microsoft.VisualStudio.Shell;

namespace MDK.Services
{
    /// <summary>
    /// Options page for the MDK extension
    /// </summary>
    [CLSCompliant(false)]
    [ComVisible(true)]
    [Obsolete("This type remains here purely for backwards compatibility reasons. Use " + nameof(MDK.Options.MDKOptions) + " instead.")]
    [SharedSettings("MDK.Services.ObsoleteMDKOptions", false)]
    public class ObsoleteMDKOptions : UIElementDialogPage
    {
        string _gameBinPath;
        string _outputPath;
        SpaceEngineers _spaceEngineers;

        /// <summary>
        /// Creates an instance of <see cref="ObsoleteMDKOptions" />
        /// </summary>
        public ObsoleteMDKOptions()
        {
            _spaceEngineers = new SpaceEngineers();
            _gameBinPath = _spaceEngineers.GetInstallPath("Bin64");
            _outputPath = _spaceEngineers.GetDataPath("IngameScripts", "local");
        }

        /// <summary>
        /// Determines whether the extension is a prerelease version
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPrerelease => MDKPackage.IsPrerelease;

        /// <summary>
        /// Gets the current package version
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Version => IsPrerelease ? $"v{MDKPackage.Version}-pre" : $"v{MDKPackage.Version}";

        //string IMDKWriteableOptions.HelpPageUrl => MDKPackage.HelpPageUrl;

        /// <summary>
        /// Determines whether <see cref="ManualGameBinPath"/> should be used rather than the automatically retrieved one.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Use manual binary path")]
        [Description("If checked, use the manually specified binary path")]
        public bool UseManualGameBinPath { get; set; }

        /// <summary>
        /// If <see cref="UseManualGameBinPath"/> is <c>true</c>, this value is used instead of the automatically retrieved path.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Space Engineers binary path")]
        [Description("A manual assignment of the path to the binary files of Space Engineers. Does not affect existing projects.")]
        public string ManualGameBinPath
        {
            get => _gameBinPath;
            set
            {
                _gameBinPath = value;
                if (string.IsNullOrWhiteSpace(_gameBinPath))
                    _gameBinPath = _spaceEngineers.GetInstallPath("Bin64");
            }
        }

        /// <summary>
        /// Determines whether <see cref="ManualOutputPath"/> should be used rather than the automatically retrieved path.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Use manual output path")]
        [Description("If checked, use the manually specified output path")]
        public bool UseManualOutputPath { get; set; }

        /// <summary>
        /// If <see cref="UseManualOutputPath"/> is <c>true</c>, this value is used rather than the automatically retreived path.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Script Output Path")]
        [Description("A manual assignment of the path to the default output path for the final scripts. Does not affect existing projects.")]
        public string ManualOutputPath
        {
            get => _outputPath;
            set
            {
                _outputPath = value;
                if (string.IsNullOrWhiteSpace(_outputPath))
                    _outputPath = _spaceEngineers.GetDataPath("IngameScripts", "local");
            }
        }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Minify scripts")]
        [Description("Determines whether script projects should default to generating minified scripts. Does not affect existing projects.")]
        [Obsolete("This member is obsolete and ignored from 1.1.0 forward. Please use the composer module instead.")]
        public bool Minify { get; set; }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Promote MDK on thumbnail")]
        [Description("Whether to use a variant of the game's thumbnail which promotes MDK or the default game one.")]
        public bool PromoteMDK { get; set; } = true;

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Notify me about updates")]
        [Description("Checks for new releases on the GitHub repository, and shows a Visual Studio notification if a new version is detected.")]
        public bool NotifyUpdates { get; set; } = true;

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Include prerelease versions")]
        [Description("Include prerelease versions when checking for new releases on the GitHub repository.")]
        public bool NotifyPrereleaseUpdates { get; set; }

        /// <inheritdoc />
        protected sealed override UIElement Child => null;

        /// <summary>
        /// The location of plugins to use with MDK
        /// </summary>
        [Category("MDK/SE")]
        [DisplayName("Plugin Locations")]
        [Description("The location of plugins")]
        public List<Uri> PluginLocations { get; } = new List<Uri>();

        /// <summary>
        /// Returns the actual game bin path which should be used, taking all settings into account.
        /// </summary>
        /// <returns></returns>
        public string GetActualGameBinPath()
        {
            return UseManualGameBinPath ? ManualGameBinPath : _spaceEngineers.GetInstallPath("Bin64");
        }

        /// <summary>
        /// Returns the actual output path which should be used, taking all settings into account.
        /// </summary>
        /// <returns></returns>
        public string GetActualOutputPath()
        {
            return UseManualOutputPath ? ManualOutputPath : _spaceEngineers.GetDataPath("IngameScripts", "local");
        }
    }
}
