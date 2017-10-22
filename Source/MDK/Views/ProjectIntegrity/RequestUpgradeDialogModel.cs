using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Malware.MDKModules;
using Malware.MDKServices;
using MDK.Resources;

namespace MDK.Views.ProjectIntegrity
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
        public RequestUpgradeDialogModel([NotNull] MDKPackage package, [NotNull] ScriptSolutionAnalysisResult analysisResults)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));

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
        public MDKPackage Package { get; }

        /// <summary>
        /// Contains the list of projects to examine.
        /// </summary>
        public ReadOnlyCollection<MDKProjectOptions> Projects { get; }

        /// <summary>
        /// Upgrades the projects.
        /// </summary>
        protected override bool OnSave()
        {
            try
            {
                Package.ScriptUpgrades.Repair(AnalysisResults);
            }
            catch (Exception e)
            {
                Package.ShowError(Text.RequestUpgradeDialogModel_OnSave_Error, Text.RequestUpgradeDialogModel_OnSave_Error_Description, e);
            }
            return true;
        }
    }
}
