using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;
using Malware.MDKModules;
using Malware.MDKServices;

namespace Malware.MDKUI.Options
{
    public class MDKGeneralOptionsControlModel : DialogViewModel
    {
        readonly IMDKWriteableOptions _options;
        SpaceEngineers _spaceEngineers = new SpaceEngineers();

        public MDKGeneralOptionsControlModel(IMDKWriteableOptions options, string helpPageUrl) : base(helpPageUrl)
        {
            _options = options;
        }

        public bool IsPrerelease => _options.IsPrerelease;

        public string Version => IsPrerelease ? $"v{_options.Version}-pre" : $"v{_options.Version}";

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

        public string ManualGameBinPath
        {
            get => _options.GetActualGameBinPath();
            set
            {
                if (value == ManualGameBinPath)
                    return;
                _options.ManualGameBinPath = value?.Trim('\\');
                OnPropertyChanged();
                var gameBinPath = _spaceEngineers.GetInstallPath("Bin64")?.Trim('\\');
                if (!_options.UseManualGameBinPath && string.Equals(_options.ManualGameBinPath, gameBinPath, StringComparison.CurrentCultureIgnoreCase))
                    _options.ManualGameBinPath = null;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => OnPropertyChanged(nameof(ManualGameBinPath))));
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

        public string ManualOutputPath
        {
            get => _options.GetActualOutputPath();
            set
            {
                if (value == ManualOutputPath)
                    return;
                _options.ManualOutputPath = value?.Trim('\\');
                OnPropertyChanged();
                var outputPath = _spaceEngineers.GetInstallPath("Bin64")?.Trim('\\');
                if (!_options.UseManualOutputPath && string.Equals(_options.ManualOutputPath, outputPath, StringComparison.CurrentCultureIgnoreCase))
                    _options.ManualOutputPath = null;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => OnPropertyChanged(nameof(ManualOutputPath))));
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

        protected override bool OnSave()
        {
            return true;
        }
    }
}
