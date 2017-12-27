using System;
using Malware.MDKModules;
using Malware.MDKServices;
using MDK.Services;

#pragma warning disable 618

namespace MDK.Options.Versioning
{
    // ReSharper disable once InconsistentNaming
    sealed class UpgradeTo1 : Upgrader
    {
        public override void Upgrade(MDKPackage package, MDKOptions options)
        {
            var spaceEngineers = new SpaceEngineers();
            var oldOptions = new ObsoleteMDKOptions();

            options.UseManualGameBinPath = oldOptions.UseManualGameBinPath;
            options.ManualGameBinPath = oldOptions.ManualGameBinPath;
            var gameBinPath = spaceEngineers.GetInstallPath("Bin64")?.Trim('\\');
            if (!options.UseManualGameBinPath && string.Equals(options.ManualGameBinPath, gameBinPath, StringComparison.CurrentCultureIgnoreCase))
                options.ManualGameBinPath = null;

            options.UseManualOutputPath = oldOptions.UseManualOutputPath;
            options.ManualOutputPath = oldOptions.ManualOutputPath;
            var outputPath = spaceEngineers.GetDataPath("IngameScripts", "local")?.Trim('\\');
            if (!options.UseManualOutputPath && string.Equals(options.ManualOutputPath, outputPath, StringComparison.CurrentCultureIgnoreCase))
                options.ManualOutputPath = null;

            if (oldOptions.Minify)
            {
                options.DefaultComposer = new MDKModuleReference(new Guid("F565AF35-15DF-4C64-B83A-2B5F8510D948"));
            }
            options.PromoteMDK = oldOptions.PromoteMDK;
            options.NotifyUpdates = oldOptions.NotifyUpdates;
            options.NotifyPrereleaseUpdates = oldOptions.NotifyPrereleaseUpdates;
            options.OptionsVersion = 1;
        }
    }
}
