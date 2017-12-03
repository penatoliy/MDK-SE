using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Malware.MDKModules;
using Malware.MDKServices;
using Malware.MDKUI.Properties;

namespace Malware.MDKUI.ProjectIntegrity
{
    /// <summary>
    /// The view model for the <see cref="RequestUpgradeDialog"/> view.
    /// </summary>
    public class RequestUpgradeDialogModel : DialogViewModel
    {
        /// <summary>
        /// Creates a new instance of this view model.
        /// </summary>
        /// <param name="package"></param>
        /// <param name="analysisResults"></param>
        /// <param name="helpPageUrl"></param>
        public RequestUpgradeDialogModel([NotNull] IMDK package, [NotNull] ScriptSolutionAnalysisResult analysisResults, string helpPageUrl)
            : base(helpPageUrl)
        {
            Mdk = package ?? throw new ArgumentNullException(nameof(package));

            AnalysisResults = analysisResults ?? throw new ArgumentNullException(nameof(analysisResults));
            if (analysisResults.BadProjects.IsDefaultOrEmpty)
                Projects = new ReadOnlyCollection<MDKProjectOptions>(new List<MDKProjectOptions>());
            else
                Projects = new ReadOnlyCollection<MDKProjectOptions>(analysisResults.BadProjects.Select(p => p.Options).ToArray());
        }

        /// <summary>
        /// The analysis reults
        /// </summary>
        public ScriptSolutionAnalysisResult AnalysisResults { get; set; }

        /// <summary>
        /// The associated MDK package
        /// </summary>
        public IMDK Mdk { get; }

        /// <summary>
        /// Contains the list of projects to examine.
        /// </summary>
        public ReadOnlyCollection<MDKProjectOptions> Projects { get; }


        /// <summary>
        /// Invoked when this dialog saves
        /// </summary>
        public Action SaveCallback { get; set; }

        /// <summary>
        /// Upgrades the projects.
        /// </summary>
        protected override bool OnSave()
        {
            try
            {
                SaveCallback?.Invoke();
            }
            catch (Exception e)
            {
                Mdk.Dialogs.ShowError(Resources.RequestUpgradeDialogModel_OnSave_Error, Resources.RequestUpgradeDialogModel_OnSave_Error_Description, e);
            }
            return true;
        }
    }
}
