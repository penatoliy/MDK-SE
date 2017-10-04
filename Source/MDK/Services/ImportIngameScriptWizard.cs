using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using EnvDTE;
using MDK.Views.BlueprintManager;
using MDK.Views.Wizard;
using Microsoft.VisualStudio.TemplateWizard;

namespace MDK.Services
{
    /// <summary>
    /// A project template wizard designed to augment the ingame script templates with MDK information macros
    /// </summary>
    [ComVisible(true)]
    [Guid("9F6A5405-29C3-440D-83F4-FB59D1BA0749")]
    [ProgId("MDK.Services.ImportIngameScriptWizard")]
    public class ImportIngameScriptWizard : IngameScriptWizardBase<ImportIngameScriptWizard.ImportContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// A wizard context containing the necessary information to complete this wizard.
        /// </summary>
        public class ImportContext : WizardContext
        {
            /// <inheritdoc />
            public ImportContext(object automationObject, Dictionary<string, string> replacementsDictionary) : base(automationObject, replacementsDictionary)
            { }

            /// <summary>
            /// The blueprint to be imported
            /// </summary>
            public BlueprintModel Blueprint { get; set; }
        }

        /// <inheritdoc />
        protected override ImportContext CreateContext(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            return new ImportContext(automationObject, replacementsDictionary);
        }

        /// <inheritdoc />
        protected override void OnRunWizard(ImportContext context)
        {
            var newScriptModel = new NewScriptWizardDialogModel
            {
                GameBinPath = context.GameBinPath,
                OutputPath = context.OutputPath,
                Minify = context.Minify,
                PromoteMDK = context.PromoteMDK
            };
            var result = NewScriptWizardDialog.ShowDialog(newScriptModel);
            if (result == false)
                throw new WizardCancelledException();
            context.UseManualGameBinPath = !string.Equals(newScriptModel.GameBinPath, context.GameBinPath, StringComparison.CurrentCultureIgnoreCase);
            context.GameBinPath = newScriptModel.GameBinPath;
            context.OutputPath = newScriptModel.OutputPath;
            context.Minify = newScriptModel.Minify;
            context.PromoteMDK = newScriptModel.PromoteMDK;

            var blueprintModel = new BlueprintManagerDialogModel
            {
                BlueprintPath = context.OutputPath
            };
            if (!(BlueprintManagerDialog.ShowDialog(blueprintModel) ?? false))
                throw new WizardCancelledException();
            var model = blueprintModel.SelectedBlueprint;
            context.Blueprint = model ?? throw new WizardCancelledException();
        }

        /// <inheritdoc />
        protected override void OnFinishWizard(ImportContext context, Project project)
        {
            base.OnFinishWizard(context, project);
        }
    }
}
