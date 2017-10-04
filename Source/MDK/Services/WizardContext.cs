using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Malware.MDKUtilities;
using MDK.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;

namespace MDK.Services
{
    /// <summary>
    /// A context object containing information about the current wizard run.
    /// </summary>
    public class WizardContext
    {
        readonly SpaceEngineers _spaceEngineers;

        /// <summary>
        /// Creates a new instance of <see cref="WizardContext"/>
        /// </summary>
        /// <param name="automationObject"></param>
        /// <param name="replacementsDictionary"></param>
        public WizardContext(object automationObject, Dictionary<string, string> replacementsDictionary)
        {
            _spaceEngineers = new SpaceEngineers();
            ReplacementsDictionary = replacementsDictionary;
            ServiceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)automationObject);

            if (!TryGetProperties(ServiceProvider, out var props))
                throw new WizardCancelledException();
            Properties = props;

            if (!TryGetFinalBinPath(ServiceProvider, props, out var binPath))
                throw new WizardCancelledException();
            GameBinPath = binPath;

            if (!TryGetFinalOutputPath(ServiceProvider, props, out var outputPath))
                throw new WizardCancelledException();
            OutputPath = outputPath;

            if (!TryGetFinalInstallPath(ServiceProvider, out var installPath))
                throw new WizardCancelledException();
            InstallPath = installPath;

            if (!TryGetFinalMinify(props, out var minify))
                throw new WizardCancelledException();
            Minify = minify;

            if (!TryGetFinalPromoteMDK(props, out var promoteMDK))
                PromoteMDK = promoteMDK;

            ReplacementsDictionary["$mdkversion$"] = MDKPackage.Version.ToString();
        }

        /// <summary>
        /// Gets the installation path of the MDK extension
        /// </summary>
        public string InstallPath
        {
            get
            {
                ReplacementsDictionary.TryGetValue("$mdkinstallpath$", out var value);
                return value;
            }
            private set => ReplacementsDictionary["$mdkinstallpath$"] = value;
        }

        /// <summary>
        /// Gets the replacement dictionary for the template macros.
        /// </summary>
        public Dictionary<string, string> ReplacementsDictionary { get; }

        /// <summary>
        /// Whether MDK promotions should be injected into the finished script project.
        /// </summary>
        public bool PromoteMDK { get; set; }

        /// <summary>
        /// The Visual Studio service provider
        /// </summary>
        public ServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Whether the minifier should be active during the ingame script deployments
        /// </summary>
        public bool Minify
        {
            get
            {
                ReplacementsDictionary.TryGetValue("$mdkminify$", out var value);
                return string.Equals(value, "yes", StringComparison.CurrentCultureIgnoreCase);
            }
            set => ReplacementsDictionary["$mdkminify$"] = value ? "yes" : "no";
        }

        /// <summary>
        /// Where the finished ingame scripts should be output
        /// </summary>
        public string OutputPath
        {
            get
            {
                ReplacementsDictionary.TryGetValue("$mdkoutputpath$", out var value);
                return value;
            }
            set => ReplacementsDictionary["$mdkoutputpath$"] = value;
        }

        /// <summary>
        /// The path to the game binaries
        /// </summary>
        public string GameBinPath
        {
            get
            {
                ReplacementsDictionary.TryGetValue("$mdkgamebinpath$", out var value);
                return value;
            }
            set => ReplacementsDictionary["$mdkgamebinpath$"] = value;
        }

        /// <summary>
        /// The extension property bag
        /// </summary>
        public Properties Properties { get; }

        /// <summary>
        /// The current version of the MDK extension
        /// </summary>
        public string Version => ReplacementsDictionary["$mdkversion$"];

        /// <summary>
        /// Whether <see cref="GameBinPath"/> should be used rather than the automatically determined
        /// location
        /// </summary>
        public bool UseManualGameBinPath
        {
            get
            {
                ReplacementsDictionary.TryGetValue("$mdkusemanualgamebinpath$", out var value);
                return string.Equals(value, "yes", StringComparison.CurrentCultureIgnoreCase);
            }
            set => ReplacementsDictionary["$mdkusemanualgamebinpath$"] = value ? "yes" : "no";
        }

        bool TryGetProperties(IServiceProvider serviceProvider, out Properties props)
        {
            while (true)
            {
                try
                {
                    var dte = (DTE)serviceProvider.GetService(typeof(DTE));
                    props = dte.Properties["MDK/SE", "Options"];
                }
                catch (COMException)
                {
                    var res = VsShellUtilities.ShowMessageBox(serviceProvider, Text.IngameScriptWizard_TryGetProperties_MDKSettingsNotFoundDescription, Text.IngameScriptWizard_TryGetProperties_MDKSettingsNotFound, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
                    if (res == 4)
                        continue;
                    props = null;
                    return false;
                }
                return true;
            }
        }

        bool TryGetFinalBinPath(IServiceProvider serviceProvider, Properties props, out string binPath)
        {
            while (true)
            {
                var useBinPath = (bool)props.Item(nameof(MDKOptions.UseManualGameBinPath)).Value;
                binPath = ((string)props.Item(nameof(MDKOptions.GameBinPath))?.Value)?.Trim() ?? "";
                if (!useBinPath || binPath == "")
                    binPath = _spaceEngineers.GetInstallPath("Bin64");

                var binDirectory = new DirectoryInfo(binPath);
                if (!binDirectory.Exists)
                {
                    var res = VsShellUtilities.ShowMessageBox(serviceProvider, Text.IngameScriptWizard_TryGetFinalBinPath_SEBinPathNotFoundDescription, Text.IngameScriptWizard_TryGetFinalBinPath_SEBinPathNotFound, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
                    if (res == 4)
                        continue;
                    return false;
                }
                binPath = binDirectory.ToString().TrimEnd('\\');
                return true;
            }
        }


        bool TryGetFinalOutputPath(IServiceProvider serviceProvider, Properties props, out string outputPath)
        {
            while (true)
            {
                var useOutputPath = (bool)props.Item(nameof(MDKOptions.UseManualOutputPath)).Value;
                outputPath = ((string)props.Item(nameof(MDKOptions.OutputPath))?.Value)?.Trim() ?? "";
                if (!useOutputPath || outputPath == "")
                    outputPath = _spaceEngineers.GetDataPath("IngameScripts", "local");
                var outputDirectory = new DirectoryInfo(outputPath);
                try
                {
                    if (!outputDirectory.Exists)
                        outputDirectory.Create();
                }
                catch
                {
                    var res = VsShellUtilities.ShowMessageBox(serviceProvider, string.Format(Text.IngameScriptWizard_TryGetFinalOutputPath_CannotCreateOutputPathDescription, outputDirectory), Text.IngameScriptWizard_TryGetFinalOutputPath_CannotCreateOutputPath, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
                    if (res == 4)
                        continue;
                    return false;
                }
                outputPath = outputDirectory.ToString().TrimEnd('\\');
                return true;
            }
        }

        bool TryGetFinalInstallPath(IServiceProvider serviceProvider, out string installPath)
        {
            while (true)
            {
                installPath = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath)?.Trim() ?? "ERROR";
                var installDirectory = new DirectoryInfo(installPath);
                if (!installDirectory.Exists)
                {
                    var res = VsShellUtilities.ShowMessageBox(serviceProvider, Text.IngameScriptWizard_TryGetFinalInstallPath_CannotFindMDKPathDescription, Text.IngameScriptWizard_TryGetFinalInstallPath_CannotMDKPath, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
                    if (res == 4)
                        continue;
                    return false;
                }
                installPath = installDirectory.ToString().TrimEnd('\\');
                return true;
            }
        }

        bool TryGetFinalMinify(Properties props, out bool minify)
        {
            minify = (bool)(props.Item(nameof(MDKOptions.Minify))?.Value ?? false);
            return true;
        }

        bool TryGetFinalPromoteMDK(Properties props, out bool promoteMDK)
        {
            promoteMDK = (bool)(props.Item(nameof(MDKOptions.PromoteMDK))?.Value ?? false);
            return true;
        }
    }
}