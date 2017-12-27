using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Malware.MDKModules;

namespace Malware.MDKUI.Options
{
    public class UriRequestedEventArgs : CancelEventArgs
    {
        public string Uri { get; set; }
    }

    public class MDKPluginOptionsControlModel : DialogViewModel
    {
        readonly IMDKWriteableOptions _options;
        int _pluginLocationIndex;

        public MDKPluginOptionsControlModel(IMDKWriteableOptions options, string helpPageUrl) : base(helpPageUrl)
        {
            _options = options;
            PluginLocations = new ObservableCollection<string>(_options.PluginLocations.Select(l => l.ToString()));
            PluginLocations.CollectionChanged += PluginLocationsOnCollectionChanged;
            RemovePluginLocationCommand = new ModelCommand(RemovePluginLocation, false);
            AddPluginLocationCommand = new ModelCommand(AddPluginLocation);
        }

        public bool IsPrerelease => _options.IsPrerelease;

        public string Version => IsPrerelease ? $"v{_options.Version}-pre" : $"v{_options.Version}";

        public event EventHandler<UriRequestedEventArgs> UriRequested;

        public ModelCommand RemovePluginLocationCommand { get; }

        public ModelCommand AddPluginLocationCommand { get; }

        public int PluginLocationIndex
        {
            get => _pluginLocationIndex;
            set
            {
                if (value == _pluginLocationIndex)
                    return;
                _pluginLocationIndex = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PluginLocations { get; }

        void AddPluginLocation()
        {
            var args = new UriRequestedEventArgs { Cancel = true };
            UriRequested?.Invoke(this, args);
            if (args.Cancel)
                return;


        }

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
