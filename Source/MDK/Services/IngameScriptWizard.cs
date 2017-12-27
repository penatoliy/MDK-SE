using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Malware.MDKUI.Wizard;
using MDK.Options;
using MDK.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;

namespace MDK.Services
{
    /// <summary>
    /// A project template wizard designed to augment the ingame script templates with MDK information macros
    /// </summary>
    [ComVisible(true)]
    [Guid("0C84F679-2E43-491E-B9A6-75599C2C4AE5")]
    [ProgId("MDK.Services.IngameScriptWizard")]
    public class IngameScriptWizard : IWizard
    {
        const string SourceWhitelistSubPath = @"Analyzers\whitelist.cache";
        const string TargetWhitelistSubPath = @"MDK\whitelist.cache";

        bool _promoteMDK = true;
        MDKOptions _options;

        /// <inheritdoc />
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)automationObject);

            if (!TryGetFinalBinPath(serviceProvider, out var binPath))
                throw new WizardCancelledException();

            if (!TryGetFinalOutputPath(serviceProvider, out var outputPath))
                throw new WizardCancelledException();

            if (!TryGetFinalInstallPath(serviceProvider, out var installPath))
                throw new WizardCancelledException();

            _promoteMDK = _options.PromoteMDK;
            var model = new NewScriptWizardDialogModel(MDKPackage.HelpPageUrl)
            {
                GameBinPath = binPath,
                OutputPath = outputPath,
                PromoteMDK = _options.PromoteMDK
            };
            var result = NewScriptWizardDialog.ShowDialog(model);
            if (result == false)
                throw new WizardCancelledException();

            replacementsDictionary["$mdkusemanualgamebinpath$"] = !string.Equals(model.GameBinPath, binPath, StringComparison.CurrentCultureIgnoreCase) ? "yes" : "no";
            replacementsDictionary["$mdkgamebinpath$"] = model.GameBinPath;
            replacementsDictionary["$mdkoutputpath$"] = model.OutputPath;
            replacementsDictionary["$mdkinstallpath$"] = installPath;
            replacementsDictionary["$mdkminify$"] = model.Minify ? "yes" : "no";
            replacementsDictionary["$mdkversion$"] = MDKPackage.Version.ToString();
            _promoteMDK = model.PromoteMDK;
        }

        void IWizard.ProjectItemFinishedGenerating(ProjectItem projectItem)
        { }

        /// <inheritdoc />
        public void ProjectFinishedGenerating(Project project)
        {
            var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)project.DTE);
            _options = new Options.MDKOptions(serviceProvider);

            if (!TryGetFinalInstallPath(serviceProvider, out var installPath))
                throw new WizardCancelledException();

            var sourceWhitelistFile = Path.Combine(installPath, SourceWhitelistSubPath);
            if (!File.Exists(sourceWhitelistFile))
            {
                VsShellUtilities.ShowMessageBox(serviceProvider, Text.IngameScriptWizard_TryGetFinalInstallPath_CannotFindMDKPathDescription, Text.IngameScriptWizard_TryGetFinalInstallPath_CannotMDKPath, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                throw new WizardCancelledException();
            }

            while (true)
            {
                try
                {
                    var projectFileInfo = new FileInfo(project.FullName);
                    var targetWhitelistFileInfo = new FileInfo(Path.Combine(projectFileInfo.Directory.FullName, TargetWhitelistSubPath));
                    if (!targetWhitelistFileInfo.Directory.Exists)
                        targetWhitelistFileInfo.Directory.Create();
                    File.Copy(sourceWhitelistFile, targetWhitelistFileInfo.FullName, true);
                    break;
                }
                catch (Exception e)
                {
                    var res = VsShellUtilities.ShowMessageBox(serviceProvider, string.Format(Text.IngameScriptWizard_ProjectItemFinishedGenerating_CannotWriteWhitelistCacheDescription, e.Message), Text.IngameScriptWizard_ProjectItemFinishedGenerating_CannotWriteWhitelistCache, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
                    if (res == 4)
                        continue;
                    throw new WizardCancelledException();
                }
            }
        }

        void IWizard.BeforeOpeningFile(ProjectItem projectItem)
        { }

        void IWizard.RunFinished()
        { }

        bool IWizard.ShouldAddProjectItem(string filePath)
        {
            switch (filePath.ToLowerInvariant())
            {
                case "thumb.png":
                    return !_promoteMDK;

                case "thumbwithpromotion.png":
                    return _promoteMDK;
            }
            return true;
        }

        //bool TryGetProperties(IServiceProvider serviceProvider, out Properties props)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            var dte = (DTE)serviceProvider.GetService(typeof(DTE));
        //            props = dte.Properties["MDK/SE", "Options"];
        //        }
        //        catch (COMException)
        //        {
        //            var res = VsShellUtilities.ShowMessageBox(serviceProvider, Text.IngameScriptWizard_TryGetProperties_MDKSettingsNotFoundDescription, Text.IngameScriptWizard_TryGetProperties_MDKSettingsNotFound, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
        //            if (res == 4)
        //                continue;
        //            props = null;
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        bool TryGetFinalBinPath(IServiceProvider serviceProvider, out string binPath)
        {
            while (true)
            {
                binPath = _options.GetActualGameBinPath();
                if (string.IsNullOrWhiteSpace(binPath))
                {
                    // We don't have a path. Just exit, let the dialog take care of it
                    return true;
                }

                var binDirectory = new DirectoryInfo(binPath);
                if (!binDirectory.Exists)
                {
                    // We have a configured path, but it fails.
                    var res = VsShellUtilities.ShowMessageBox(serviceProvider, Text.IngameScriptWizard_TryGetFinalBinPath_SEBinPathNotFoundDescription, Text.IngameScriptWizard_TryGetFinalBinPath_SEBinPathNotFound, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
                    if (res == 4)
                        continue;
                    binPath = null;
                    return false;
                }
                binPath = binDirectory.ToString().TrimEnd('\\');
                return true;
            }
        }


        bool TryGetFinalOutputPath(IServiceProvider serviceProvider, out string outputPath)
        {
            while (true)
            {
                outputPath = _options.GetActualOutputPath();
                if (outputPath == null)
                {
                    // We don't have a path. Just exit, let the dialog take care of it
                    return true;
                }
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
    }
}
