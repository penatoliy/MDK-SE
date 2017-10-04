using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MDK.Views.Wizard;
using Microsoft.VisualStudio.TemplateWizard;

namespace MDK.Services
{
    /// <summary>
    /// A project template wizard designed to augment the ingame script templates with MDK information macros
    /// </summary>
    [ComVisible(true)]
    [Guid("0C84F679-2E43-491E-B9A6-75599C2C4AE5")]
    [ProgId("MDK.Services.IngameScriptWizard")]
    public class IngameScriptWizard : IngameScriptWizardBase
    {
        /// <inheritdoc />
        protected override void OnRunWizard(WizardContext context)
        {
            var model = new NewScriptWizardDialogModel
            {
                GameBinPath = context.GameBinPath,
                OutputPath = context.OutputPath,
                Minify = context.Minify,
                PromoteMDK = context.PromoteMDK
            };
            var result = NewScriptWizardDialog.ShowDialog(model);
            if (result == false)
                throw new WizardCancelledException();

            context.UseManualGameBinPath = !string.Equals(model.GameBinPath, context.GameBinPath, StringComparison.CurrentCultureIgnoreCase);
            context.GameBinPath = model.GameBinPath;
            context.OutputPath = model.OutputPath;
            context.Minify = model.Minify;
            context.PromoteMDK = model.PromoteMDK;
        }
    }
}
