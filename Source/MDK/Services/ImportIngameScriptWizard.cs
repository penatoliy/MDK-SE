using System;
using System.Runtime.InteropServices;
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
    public class ImportIngameScriptWizard : IngameScriptWizardBase
    {
        /// <inheritdoc />
        protected override void OnRunWizard(Context context)
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
            if (model == null)
                throw new WizardCancelledException();
        }

        /// <inheritdoc />
        protected override void OnFinishWizard(Project project, Context context)
        {
            base.OnFinishWizard(project, context);
        }
    }
}
