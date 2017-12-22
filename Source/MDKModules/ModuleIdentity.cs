using System;
using System.Globalization;
using System.Reflection;
using Malware.MDKModules.Composer;
using Malware.MDKModules.Publisher;

namespace Malware.MDKModules
{
    /// <summary>
    /// Represents the identity of a module
    /// </summary>
    public struct ModuleIdentity
    {
        static readonly Tuple<Type, ModuleType>[] SupportedModuleTypes = new[]
        {
            new Tuple<Type, ModuleType>( typeof(IComposer), ModuleType.Composer ),
            new Tuple<Type, ModuleType>( typeof(IPublisher), ModuleType.Publisher ),
        };

        /// <summary>
        /// Creates an identity description for the given module type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static ModuleIdentity For(Type type, CultureInfo cultureInfo = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsAbstract)
                throw new ArgumentException("Type cannot be abstract", nameof(type));
            if (!typeof(IModule).IsAssignableFrom(type))
                throw new ArgumentException($"Type must implement {typeof(IModule).FullName}", nameof(type));
            if (type.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("Type must have a public parameterless constructor", nameof(type));
            var attribute = type.GetCustomAttribute<ModuleAttribute>();
            if (type.GetCustomAttribute<ModuleAttribute>() == null)
                throw new ArgumentException($"Type must have a {typeof(ModuleAttribute).FullName}", nameof(type));

            var moduleType = ModuleType.Unknown;
            foreach (var info in SupportedModuleTypes)
            {
                if (info.Item1.IsAssignableFrom(type))
                {
                    if (moduleType == ModuleType.Unknown)
                        moduleType = info.Item2;
                    else
                        throw new ArgumentException("Type can only be a single module type", nameof(type));
                }
            }
            if (moduleType == ModuleType.Unknown)
                throw new ArgumentException("Type is not a recognized module type", nameof(type));

            return new ModuleIdentity(type, moduleType, 
                attribute.Id, 
                attribute.GetTitle(cultureInfo ?? CultureInfo.CurrentUICulture) ?? type.FullName,
                attribute.Version ?? "1.0.0",
                attribute.GetDescription(cultureInfo ?? CultureInfo.CurrentUICulture) ?? "", 
                attribute.Author);
        }

        /// <summary>
        /// Creates an identity description for the given module type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ModuleIdentity For<T>() where T : class, IModule, new()
        {
            return For(typeof(T));
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
        /// The description of this module
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The author of this module
        /// </summary>
        public readonly string Author;

        ModuleIdentity(Type type, ModuleType moduleType, Guid id, string title, string version, string description, string author)
        {
            ModuleType = moduleType;
            Description = description;
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
                case ModuleType.Composer:
                    moduleType = Resources.ResourceManager.GetString(nameof(Resources.ModuleIdentity_ToString_ComposerModule), cultureInfo);
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