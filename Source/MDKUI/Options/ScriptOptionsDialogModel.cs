using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Malware.MDKModules;
using Malware.MDKServices;

namespace Malware.MDKUI.Options
{
    /// <summary>
    ///     The view model for <see cref="ScriptOptionsDialog" />
    /// </summary>
    public class ScriptOptionsDialogModel : DialogViewModel
    {
        MDKProjectOptions _activeProject;
        ModuleModel _composer;
        ModuleModel _publisher;

        /// <summary>
        ///     Creates a new instance of <see cref="ScriptOptionsDialogModel" />
        /// </summary>
        /// <param name="mdk"></param>
        /// <param name="moduleManager"></param>
        /// <param name="mdkOptions"></param>
        /// <param name="helpPageUrl"></param>
        public ScriptOptionsDialogModel([NotNull] IMDK mdk, [NotNull] ModuleManager moduleManager, [NotNull] MDKProjectOptions mdkOptions, string helpPageUrl)
            : base(helpPageUrl)
        {
            Mdk = mdk ?? throw new ArgumentNullException(nameof(mdk));
            ModuleManager = moduleManager ?? throw new ArgumentNullException(nameof(moduleManager));
            ActiveProject = mdkOptions ?? throw new ArgumentNullException(nameof(mdkOptions));
            var modules = ModuleManager.LoadDeploymentPlugins(false);
            Composer = new ModuleModel(ModuleManager.DefaultComposer);
            Composers = new ReadOnlyCollection<ModuleModel>(ModuleModelsWithNone(Composer, modules.Where(m => m.ModuleType == ModuleType.Composer)));
            Publisher = new ModuleModel(ModuleManager.DefaultPublisher);
            Publishers = new ReadOnlyCollection<ModuleModel>(ModuleModelsWithNone(Publisher, modules.Where(m => m.ModuleType == ModuleType.Publisher)));
        }

        /// <summary>
        /// Reference to the MDK main service
        /// </summary>
        public IMDK Mdk { get; }

        /// <summary>
        /// Reference to the module manager
        /// </summary>
        public ModuleManager ModuleManager { get; }

        /// <summary>
        ///     A list of available composers
        /// </summary>
        public ReadOnlyCollection<ModuleModel> Composers { get; }

        public ModuleModel Composer
        {
            get => _composer;
            set
            {
                if (Equals(value, _composer))
                    return;
                _composer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     A list of available publishers
        /// </summary>
        public ReadOnlyCollection<ModuleModel> Publishers { get; }

        public ModuleModel Publisher
        {
            get => _publisher;
            set
            {
                if (Equals(value, _publisher))
                    return;
                _publisher = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     The currently selected project
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
        ///     Saves any changed options
        /// </summary>
        /// <returns></returns>
        protected override bool OnSave()
        {
            if (ActiveProject.HasChanges)
                ActiveProject.Save(Mdk);
            return true;
        }
    }
}
