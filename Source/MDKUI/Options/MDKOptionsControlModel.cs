using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Malware.MDKModules;

namespace Malware.MDKUI.Options
{
    public class MDKOptionsControlModel : DialogViewModel
    {
        readonly IMDKWriteableOptions _options;

        public MDKOptionsControlModel(IMDKWriteableOptions options, string helpPageUrl) : base(helpPageUrl)
        {
            _options = options;
            PluginLocations = new ObservableCollection<string>(_options.PluginLocations.Select(l => l.ToString()));
            PluginLocations.CollectionChanged += PluginLocationsOnCollectionChanged;
            RemovePluginLocationCommand = new ModelCommand(RemovePluginLocation, false);
            AddPluginLocationCommand = new ModelCommand(AddPluginLocation);
        }

        public bool IsPrerelease => _options.IsPrerelease;

        public string Version => IsPrerelease ? $"v{_options.Version}-pre" : $"v{_options.Version}";

        public ModelCommand RemovePluginLocationCommand { get; }
        public ModelCommand AddPluginLocationCommand { get; }

        public bool UseManualGameBinPath
        {
            get => _options.UseManualGameBinPath;
            set
            {
                if (value == _options.UseManualGameBinPath)
                    return;
                _options.UseManualGameBinPath = value;
                OnPropertyChanged();
            }
        }

        public string GameBinPath
        {
            get => _options.GameBinPath;
            set
            {
                if (value == _options.GameBinPath)
                    return;
                _options.GameBinPath = value;
                OnPropertyChanged();
            }
        }

        public bool UseManualOutputPath
        {
            get => _options.UseManualOutputPath;
            set
            {
                if (value == _options.UseManualOutputPath)
                    return;
                _options.UseManualOutputPath = value;
                OnPropertyChanged();
            }
        }

        public string OutputPath
        {
            get => _options.OutputPath;
            set
            {
                if (value == _options.OutputPath)
                    return;
                _options.OutputPath = value;
                OnPropertyChanged();
            }
        }

        public bool PromoteMDK
        {
            get => _options.PromoteMDK;
            set
            {
                if (value == _options.PromoteMDK)
                    return;
                _options.PromoteMDK = value;
                OnPropertyChanged();
            }
        }

        public bool NotifyUpdates
        {
            get => _options.NotifyUpdates;
            set
            {
                if (value == _options.NotifyUpdates)
                    return;
                _options.NotifyUpdates = value;
                OnPropertyChanged();
            }
        }

        public bool NotifyPrereleaseUpdates
        {
            get => _options.NotifyPrereleaseUpdates;
            set
            {
                if (value == _options.NotifyPrereleaseUpdates)
                    return;
                _options.NotifyPrereleaseUpdates = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PluginLocations { get; }

        void AddPluginLocation()
        { }

        void RemovePluginLocation()
        { }

        void PluginLocationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _options.PluginLocations.Clear();
            foreach (var item in PluginLocations.OrderBy(l => l))
                _options.PluginLocations.Add(new Uri(item));
        }

        protected override bool OnSave()
        {
            return true;
        }
    }
}
