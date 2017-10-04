using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using MDK.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;

namespace MDK.Services
{
    /// <summary>
    /// Provides a base class for all ingame script wizards.
    /// </summary>
    public abstract class IngameScriptWizardBase<TContext> : IWizard where TContext: WizardContext
    {
        const string SourceWhitelistSubPath = @"Analyzers\whitelist.cache";
        const string TargetWhitelistSubPath = @"MDK\whitelist.cache";
        TContext _context;

        /// <summary>
        /// Create a wizard context.
        /// </summary>
        /// <param name="automationObject"></param>
        /// <param name="replacementsDictionary"></param>
        /// <param name="runKind"></param>
        /// <param name="customParams"></param>
        /// <returns></returns>
        protected abstract TContext CreateContext(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams);

        /// <inheritdoc />
        void IWizard.RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _context = CreateContext(automationObject, replacementsDictionary, runKind, customParams);
            OnRunWizard(_context);
        }

        /// <summary>
        /// Makes modifications to the context, shows input dialogs etc.
        /// </summary>
        /// <param name="context"></param>
        protected abstract void OnRunWizard(TContext context);

        void IWizard.ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            OnProjectItemFinishedGenerating(_context, projectItem);
        }

        /// <summary>
        /// A project item has finished generating.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="projectItem"></param>
        protected virtual void OnProjectItemFinishedGenerating(TContext context, ProjectItem projectItem)
        { }

        /// <inheritdoc />
        void IWizard.ProjectFinishedGenerating(Project project)
        {
            if (_context == null)
                throw new InvalidOperationException("Unexpected finish call without an existing wizard context");

            var context = _context;
            OnFinishWizard(context, project);
        }

        void IWizard.BeforeOpeningFile(ProjectItem projectItem)
        {
            OnBeforeOpeningFile(_context, projectItem);
        }

        /// <summary>
        /// A project item file is about to be opened.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="projectItem"></param>
        protected virtual void OnBeforeOpeningFile(TContext context, ProjectItem projectItem)
        { }

        void IWizard.RunFinished()
        {
            OnRunFinished(_context);
            _context = null;
        }

        /// <summary>
        /// A wizard run has just been completed.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnRunFinished(TContext context)
        { }

        bool IWizard.ShouldAddProjectItem(string filePath)
        {
            return OnShouldAddProjectItem(_context, filePath);
        }

        /// <summary>
        /// Determines whether the given file path is eligible to be added the project as a project item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected virtual bool OnShouldAddProjectItem(TContext context, string filePath)
        {
            switch (filePath.ToLowerInvariant())
            {
                case "thumb.png":
                    return !(_context?.PromoteMDK ?? false);

                case "thumbwithpromotion.png":
                    return _context?.PromoteMDK ?? false;
            }
            return true;
        }

        /// <summary>
        /// Called when the wizard has been completed. makes further modifications to the project.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="project"></param>
        protected virtual void OnFinishWizard(TContext context, Project project)
        {
            var sourceWhitelistFile = Path.Combine(context.InstallPath, SourceWhitelistSubPath);
            if (!File.Exists(sourceWhitelistFile))
            {
                VsShellUtilities.ShowMessageBox(context.ServiceProvider, Text.IngameScriptWizard_TryGetFinalInstallPath_CannotFindMDKPathDescription, Text.IngameScriptWizard_TryGetFinalInstallPath_CannotMDKPath, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
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
                    var res = VsShellUtilities.ShowMessageBox(context.ServiceProvider, string.Format(Text.IngameScriptWizard_ProjectItemFinishedGenerating_CannotWriteWhitelistCacheDescription, e.Message), Text.IngameScriptWizard_ProjectItemFinishedGenerating_CannotWriteWhitelistCache, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND);
                    if (res == 4)
                        continue;
                    throw new WizardCancelledException();
                }
            }
        }
    }

    /// <inheritdoc />
    public abstract class IngameScriptWizardBase : IngameScriptWizardBase<WizardContext>
    {
        /// <inheritdoc />
        protected override WizardContext CreateContext(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            return new WizardContext(automationObject, replacementsDictionary);
        }
    }
}
