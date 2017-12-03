using System.ComponentModel;
using Malware.MDKModules;

namespace Malware.MDKUI.Options
{
    public interface IMDKOptionsControlModel : IMDKOptions, INotifyPropertyChanged
    {
        string HelpPageUrl { get; }
        new bool UseManualGameBinPath { get; set; }
        new string GameBinPath { get; set; }
        new bool UseManualOutputPath { get; set; }
        new string OutputPath { get; set; }
        new bool Minify { get; set; }
        new bool PromoteMDK { get; set; }
        new bool NotifyUpdates { get; set; }
        new bool NotifyPrereleaseUpdates { get; set; }
    }
}
