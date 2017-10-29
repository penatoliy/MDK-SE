using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Malware.MDKModules
{
    /// <summary>
    /// Represents the identity of a module
    /// </summary>
    public struct ModuleIdentity
    {
        /// <summary>
        /// Creates a new instance of <see cref="ModuleIdentity"/>
        /// </summary>
        /// <param name="module">The module instance to create an identity for</param>
        /// <param name="title">The human-readable title of this module</param>
        /// <param name="version">The current module version</param>
        /// <param name="author">The author of this module</param>
        public static ModuleIdentity For(IModule module, string title, string version, string author) =>
            new ModuleIdentity(module.GetModuleType(), IdOf(module), title, version, author);

        static Guid IdOf(IModule module)
        {
            var guid = module.GetType().GetCustomAttribute<GuidAttribute>()?.Value;
            if (guid == null)
                throw new InvalidOperationException($"The module type {module.GetType().FullName} does not have a {typeof(GuidAttribute).FullName} tag");
            return new Guid(guid);
        }

        /// <summary>
        /// What type of module this is
        /// </summary>
        public ModuleType ModuleType { get; }

        /// <summary>
        /// The unique ID of this module
        /// </summary>
        public readonly Guid Id;

        /// <summary>
        /// The human-readable title of this module
        /// </summary>
        public readonly string Title;

        /// <summary>
        /// The current module version
        /// </summary>
        public readonly string Version;

        /// <summary>
        /// The author of this module
        /// </summary>
        public readonly string Author;

        ModuleIdentity(ModuleType moduleType, Guid id, string title, string version, string author)
        {
            ModuleType = moduleType;
            Id = id;
            Title = title;
            Version = version;
            Author = author;
        }

        /// <summary>
        /// Returns a display name for this identity in the given culture
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public string ToString(CultureInfo cultureInfo)
        {
            string moduleType;
            switch (ModuleType)
            {
                case ModuleType.Unknown:
                    moduleType = Resources.ResourceManager.GetString(nameof(Resources.ModuleIdentity_ToString_UnknownModule), cultureInfo);
                    break;
                case ModuleType.Loader:
                    moduleType = Resources.ResourceManager.GetString(nameof(Resources.ModuleIdentity_ToString_LoaderModule), cultureInfo);
                    break;
                case ModuleType.Preprocessor:
                    moduleType = Resources.ResourceManager.GetString(nameof(Resources.ModuleIdentity_ToString_PreprocessorModule), cultureInfo);
                    break;
                case ModuleType.Composer:
                    moduleType = Resources.ResourceManager.GetString(nameof(Resources.ModuleIdentity_ToString_ComposerModule), cultureInfo);
                    break;
                case ModuleType.Postprocessor:
                    moduleType = Resources.ResourceManager.GetString(nameof(Resources.ModuleIdentity_ToString_PostprocessorModule), cultureInfo);
                    break;
                case ModuleType.Publisher:
                    moduleType = Resources.ResourceManager.GetString(nameof(Resources.ModuleIdentity_ToString_PublisherModule), cultureInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return $"{moduleType} {Title} {Version}";
        }

        /// <summary>
        /// Returns a display name for this identity in the given culture
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(CultureInfo.CurrentUICulture);
    }
}