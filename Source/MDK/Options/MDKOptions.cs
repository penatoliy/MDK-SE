using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Malware.MDKModules;
using Malware.MDKServices;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

#pragma warning disable 618

namespace MDK.Options
{
    /// <summary>
    /// Contains the
    /// </summary>
    public class MDKOptions : IMDKWriteableOptions
    {
        const int VersionOrdinal = 1;
        const string CollectionPath = "MDK/SE";

        readonly IServiceProvider _serviceProvider;
        SpaceEngineers _spaceEngineers;
        ObservableCollection<Uri> _pluginLocations = new ObservableCollection<Uri>();
        int _optionsVersion;
        bool _useManualGameBinPath;
        string _manualGameBinPath;
        bool _useManualOutputPath;
        string _manualOutputPath;
        bool _promoteMDK = true;
        bool _notifyUpdates = true;
        bool _notifyPrereleaseUpdates;
        MDKModuleReference _defaultComposer;
        MDKModuleReference _defaultPublisher;
        bool _showBlueprintManagerOnDeploy = true;
        HashSet<string> _changedProperties = new HashSet<string>();

        /// <summary>
        /// Creates an instance of <see cref="MDKOptions" />
        /// </summary>
        public MDKOptions([NotNull] IServiceProvider serviceProvider, bool writeable = true)
            : this()
        {
            IsWriteable = writeable;
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _pluginLocations.CollectionChanged += OnPluginLocationsCollectionChanged;
            Revert();
        }

        MDKOptions()
        {
            _spaceEngineers = new SpaceEngineers();
            ManualGameBinPath = _spaceEngineers.GetInstallPath("Bin64");
            ManualOutputPath = _spaceEngineers.GetDataPath("IngameScripts", "local");
        }

        /// <summary>
        /// Determines whether the extension is a prerelease version
        /// </summary>
        public bool IsPrerelease => MDKPackage.IsPrerelease;

        /// <summary>
        /// Gets the current package version in a human readable format.
        /// </summary>
        public string VersionString => IsPrerelease ? $"v{MDKPackage.Version}-pre" : $"v{MDKPackage.Version}";

        /// <summary>
        /// Gets the current package version.
        /// </summary>
        public Version Version => MDKPackage.Version;

        /// <summary>
        /// Gets the web URL of the help page.
        /// </summary>
        public string HelpPageUrl => MDKPackage.HelpPageUrl;

        /// <summary>
        /// Gets the version ordinal of the saved options.
        /// </summary>
        public int OptionsVersion
        {
            get => _optionsVersion;
            set
            {
                if (value == _optionsVersion)
                    return;
                _optionsVersion = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Determines whether <see cref="ManualGameBinPath"/> should be used rather than the automatically retrieved one.
        /// </summary>
        public bool UseManualGameBinPath
        {
            get => _useManualGameBinPath;
            set
            {
                if (value == _useManualGameBinPath)
                    return;
                _useManualGameBinPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// If <see cref="UseManualGameBinPath"/> is <c>true</c>, this value is used instead of the automatically retrieved path.
        /// </summary>
        public string ManualGameBinPath
        {
            get => _manualGameBinPath;
            set
            {
                if (value == _manualGameBinPath)
                    return;
                _manualGameBinPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Determines whether <see cref="ManualOutputPath"/> should be used rather than the automatically retrieved path.
        /// </summary>
        public bool UseManualOutputPath
        {
            get => _useManualOutputPath;
            set
            {
                if (value == _useManualOutputPath)
                    return;
                _useManualOutputPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// If <see cref="UseManualOutputPath"/> is <c>true</c>, this value is used rather than the automatically retreived path.
        /// </summary>
        public string ManualOutputPath
        {
            get => _manualOutputPath;
            set
            {
                if (value == _manualOutputPath)
                    return;
                _manualOutputPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        public bool PromoteMDK
        {
            get => _promoteMDK;
            set
            {
                if (value == _promoteMDK)
                    return;
                _promoteMDK = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        public bool NotifyUpdates
        {
            get => _notifyUpdates;
            set
            {
                if (value == _notifyUpdates)
                    return;
                _notifyUpdates = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether script projects should default to generating minified scripts.
        /// </summary>
        public bool NotifyPrereleaseUpdates
        {
            get => _notifyPrereleaseUpdates;
            set
            {
                if (value == _notifyPrereleaseUpdates)
                    return;
                _notifyPrereleaseUpdates = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The reference to the default composer. A value of <c>null</c> means the built-in composer.
        /// </summary>
        public MDKModuleReference DefaultComposer
        {
            get => _defaultComposer;
            set
            {
                if (Equals(value, _defaultComposer))
                    return;
                _defaultComposer = value;
                OnPropertyChanged();
            }
        }

        MDKModuleReference IMDKOptions.DefaultComposer => DefaultComposer;

        /// <summary>
        /// The reference to the default publisher. A value of <c>null</c> means the built-in publisher.
        /// </summary>
        public MDKModuleReference DefaultPublisher
        {
            get => _defaultPublisher;
            set
            {
                if (Equals(value, _defaultPublisher))
                    return;
                _defaultPublisher = value;
                OnPropertyChanged();
            }
        }

        MDKModuleReference IMDKOptions.DefaultPublisher => DefaultPublisher;

        /// <summary>The location of plugins to use with MDK</summary>
        public IList<Uri> PluginLocations => _pluginLocations;

        IReadOnlyList<Uri> IMDKOptions.PluginLocations => _pluginLocations;

        /// <summary>
        /// Determines whether the blueprint manager window will be shown after a deployment.
        /// </summary>
        public bool ShowBlueprintManagerOnDeploy
        {
            get => _showBlueprintManagerOnDeploy;
            set
            {
                if (value == _showBlueprintManagerOnDeploy)
                    return;
                _showBlueprintManagerOnDeploy = value;
                OnPropertyChanged();
            }
        }

        bool IMDKOptions.ShowBlueprintManagerOnDeploy => ShowBlueprintManagerOnDeploy;

        /// <summary>
        /// Determines whether this options instance is actually writeable.
        /// </summary>
        public bool IsWriteable { get; }

        void OnPluginLocationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PluginLocations));
        }

        /// <summary>
        /// Returns the actual game bin path which should be used, taking all settings into account.
        /// </summary>
        /// <returns></returns>
        public string GetActualGameBinPath()
        {
            var defaultPath = _spaceEngineers.GetInstallPath("Bin64");
            return UseManualGameBinPath ? ManualGameBinPath ?? defaultPath : defaultPath;
        }

        /// <summary>
        /// Returns the actual output path which should be used, taking all settings into account.
        /// </summary>
        /// <returns></returns>
        public string GetActualOutputPath()
        {
            var defaultPath = _spaceEngineers.GetDataPath("IngameScripts", "local");
            return UseManualOutputPath ? ManualOutputPath ?? defaultPath : defaultPath;
        }

        /// <summary>
        /// Saves the current settings
        /// </summary>
        public void Save()
        {
            if (!IsWriteable)
                throw new NotSupportedException("This is a readonly options store.");
            var settingsManager = new ShellSettingsManager(_serviceProvider);
            var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!store.CollectionExists(CollectionPath))
                store.CreateCollection(CollectionPath);

            if (IsChanged(nameof(OptionsVersion)) || OptionsVersion != VersionOrdinal)
            {
                OptionsVersion = VersionOrdinal;
                store.SetInt32(CollectionPath, nameof(OptionsVersion), OptionsVersion);
            }

            if (IsChanged(nameof(UseManualGameBinPath)))
                store.SetBoolean(CollectionPath, nameof(UseManualGameBinPath), UseManualGameBinPath);
            if (IsChanged(nameof(ManualGameBinPath)))
            {
                if (string.IsNullOrWhiteSpace(ManualGameBinPath))
                    store.DeleteProperty(CollectionPath, nameof(ManualGameBinPath));
                else
                    store.SetString(CollectionPath, nameof(ManualGameBinPath), ManualGameBinPath);
            }

            if (IsChanged(nameof(UseManualOutputPath)))
                store.SetBoolean(CollectionPath, nameof(UseManualOutputPath), UseManualOutputPath);
            if (IsChanged(nameof(ManualOutputPath)))
            {
                if (string.IsNullOrWhiteSpace(ManualOutputPath))
                    store.DeleteProperty(CollectionPath, nameof(ManualOutputPath));
                else
                    store.SetString(CollectionPath, nameof(ManualOutputPath), ManualOutputPath);
            }

            if (IsChanged(nameof(PromoteMDK)))
                store.SetBoolean(CollectionPath, nameof(PromoteMDK), PromoteMDK);
            if (IsChanged(nameof(NotifyUpdates)))
                store.SetBoolean(CollectionPath, nameof(NotifyUpdates), NotifyUpdates);
            if (IsChanged(nameof(NotifyPrereleaseUpdates)))
                store.SetBoolean(CollectionPath, nameof(NotifyPrereleaseUpdates), NotifyPrereleaseUpdates);
            if (IsChanged(nameof(ShowBlueprintManagerOnDeploy)))
                store.SetBoolean(CollectionPath, nameof(ShowBlueprintManagerOnDeploy), ShowBlueprintManagerOnDeploy);
            if (IsChanged(nameof(PluginLocations)))
            {
                var pluginLocations = string.Join(";", PluginLocations.Select(l => l.ToString()));
                store.SetString(CollectionPath, nameof(PluginLocations), pluginLocations);
            }

            Revert();
        }

        /// <summary>
        /// Reverts the settings to the most recently saved state.
        /// </summary>
        public void Revert()
        {
            var settingsManager = new ShellSettingsManager(_serviceProvider);
            var store = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            if (!store.CollectionExists(CollectionPath))
                return;

            OptionsVersion = store.GetInt32(CollectionPath, nameof(OptionsVersion), 0);
            UseManualGameBinPath = store.GetBoolean(CollectionPath, nameof(UseManualGameBinPath), false);
            ManualGameBinPath = store.GetString(CollectionPath, nameof(ManualGameBinPath), null);
            UseManualOutputPath = store.GetBoolean(CollectionPath, nameof(UseManualOutputPath), false);
            ManualOutputPath = store.GetString(CollectionPath, nameof(ManualOutputPath), null);
            PromoteMDK = store.GetBoolean(CollectionPath, nameof(PromoteMDK), true);
            NotifyUpdates = store.GetBoolean(CollectionPath, nameof(NotifyUpdates), true);
            NotifyPrereleaseUpdates = store.GetBoolean(CollectionPath, nameof(NotifyPrereleaseUpdates), false);
            ShowBlueprintManagerOnDeploy = store.GetBoolean(CollectionPath, nameof(ShowBlueprintManagerOnDeploy), true);

            PluginLocations.Clear();
            var pluginLocations = store.GetString(CollectionPath, nameof(PluginLocations), null);
            if (pluginLocations == null)
                AddDefaultPluginLocations();
            else
            {
                var uriParts = pluginLocations.Split(';');
                foreach (var part in uriParts)
                {
                    if (Uri.TryCreate(part, UriKind.Absolute, out var uri))
                        PluginLocations.Add(uri);
                }
            }

            _changedProperties.Clear();
        }

        void AddDefaultPluginLocations()
        {
            PluginLocations.Add(new Uri("https://github.com/malware-dev/MDK-SE-Minifier"));
        }

        /// <summary>
        /// Determines whether a given property has changed (case sensitive).
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsChanged(string propertyName)
        {
            return _changedProperties.Contains(propertyName);
        }

        /// <summary>
        /// Called when a tracked property is changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            _changedProperties.Add(propertyName);
        }
    }
}
