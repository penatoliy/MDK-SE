using System;
using JetBrains.Annotations;
using Malware.MDKModules;
using Malware.MDKServices;
using MDK.Services;

namespace MDK.Views.Options
{
    /// <summary>
    /// The view model for <see cref="ScriptOptionsDialog"/>
    /// </summary>
    public class ScriptOptionsDialogModel : DialogViewModel
    {
        [NotNull]
        public MDKPackage Package { get; }
        MDKProjectOptions _activeProject;

        /// <summary>
        /// Creates a new instance of <see cref="ScriptOptionsDialogModel"/>
        /// </summary>
        /// <param name="package"></param>
        /// <param name="mdkOptions"></param>
        public ScriptOptionsDialogModel([NotNull] MDKPackage package, [NotNull] MDKProjectOptions mdkOptions)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            Package = package;
            ActiveProject = mdkOptions ?? throw new ArgumentNullException(nameof(mdkOptions));
        }

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
