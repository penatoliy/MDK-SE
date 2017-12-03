using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Malware.MDKModules;
using MDK.UI;

namespace MDK.Views.Options
{
    /// <summary>
    /// The view model for <see cref="ScriptOptionsDialog"/>
    /// </summary>
    public class ScriptOptionsDialogModel : DialogViewModel
    {
        MDKProjectOptions _activeProject;

        /// <summary>
        /// Creates a new instance of <see cref="ScriptOptionsDialogModel"/>
        /// </summary>
        /// <param name="package"></param>
        /// <param name="mdkOptions"></param>
        public ScriptOptionsDialogModel([NotNull] MDKPackage package, [NotNull] MDKProjectOptions mdkOptions)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            ActiveProject = mdkOptions ?? throw new ArgumentNullException(nameof(mdkOptions));
            var modules = package.ModuleManager.LoadDeploymentPlugins(false);
            Preprocessors = new ReadOnlyCollection<ModuleModel>(ModuleModelsWithNone(new ModuleModel(),  modules.Where(m => m.ModuleType == ModuleType.Preprocessor)));
            Composers = new ReadOnlyCollection<ModuleModel>(ModuleModelsWithNone(new ModuleModel(package.ModuleManager.DefaultComposer),  modules.Where(m => m.ModuleType == ModuleType.Composer)));
            Postprocessors = new ReadOnlyCollection<ModuleModel>(ModuleModelsWithNone(new ModuleModel(),  modules.Where(m => m.ModuleType == ModuleType.Postprocessor)));
            Publishers = new ReadOnlyCollection<ModuleModel>(ModuleModelsWithNone(new ModuleModel(package.ModuleManager.DefaultPublisher),  modules.Where(m => m.ModuleType == ModuleType.Publisher)));
        }

        /// <summary>
        /// The MDK package this dialog belongs to
        /// </summary>
        [NotNull]
        public MDKPackage Package { get; }

        /// <summary>
        /// A list of available preprocessor modules
        /// </summary>
        public ReadOnlyCollection<ModuleModel> Preprocessors { get; }

        /// <summary>
        /// A list of available composers
        /// </summary>
        public ReadOnlyCollection<ModuleModel> Composers { get; }

        /// <summary>
        /// A list of available postprocessors
        /// </summary>
        public ReadOnlyCollection<ModuleModel> Postprocessors { get; }

        /// <summary>
        /// A list of available publishers
        /// </summary>
        public ReadOnlyCollection<ModuleModel> Publishers { get; }

        /// <summary>
        /// The currently selected project
        /// </summary>
        public MDKProjectOptions ActiveProject
        {
            get => _activeProject;
            set
            {
                if (Equals(value, _activeProject))
                    return;
                _activeProject = value;
                OnPropertyChanged();
            }
        }

        IList<ModuleModel> ModuleModelsWithNone(ModuleModel noneRef, IEnumerable<ModuleIdentity> modules)
        {
            return new[] {noneRef}
                .Concat(modules.OrderBy(m => m.Title).Select(m => new ModuleModel(m)))
                .ToArray();
        }

        /// <summary>
        /// Saves any changed options
        /// </summary>
        /// <returns></returns>
        protected override bool OnSave()
        {
            if (ActiveProject.HasChanges)
                ActiveProject.Save(Package);
            return true;
        }
    }
}
