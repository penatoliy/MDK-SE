using System;
using Malware.MDKModules;

namespace Malware.MDKUI.Options
{
    /// <summary>
    /// A model representing a selectable module
    /// </summary>
    public class ModuleModel : Model
    {
        /// <summary>
        /// Creates a new <see cref="ModuleModel"/>
        /// </summary>
        public ModuleModel()
        {
            Title = "None";
        }

        /// <summary>
        /// Creates a new <see cref="ModuleModel"/> from the given <see cref="ModuleIdentity"/>
        /// </summary>
        /// <param name="module"></param>
        public ModuleModel(ModuleIdentity module)
        {
            Id = module.Id;
            Title = $"{module.Title} {module.Version}";
            Description = module.Description;
            Author = module.Author;
        }

        /// <summary>
        /// The ID of the module to use
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The author of the module
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The module description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The title (and version) of this module
        /// </summary>
        public string Title { get; set; }
    }
}