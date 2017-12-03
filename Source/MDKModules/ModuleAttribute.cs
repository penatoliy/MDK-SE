using System;
using System.Globalization;
using System.Resources;

namespace Malware.MDKModules
{
    /// <summary>
    /// An attribute to tag a type as a module
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleAttribute : Attribute
    {
        string _title;
        string _description;
        ResourceManager _resourceManager;

        /// <summary>
        /// Creates a new <see cref="ModuleAttribute"/>
        /// </summary>
        /// <param name="guid">The unique GUID of this module. Do _not_ reuse an id for multiple modules.</param>
        public ModuleAttribute(string guid)
        {
            Id = new Guid(guid);
        }

        /// <summary>
        /// A resource manager from which to retrieve localized texts
        /// </summary>
        public Type ResourceManagerType { get; set; }

        /// <summary>
        /// The unique ID of this module
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The display title of this module. The type name will be used if omitted.
        /// </summary>
        public string Title
        {
            get
            {
                if (_title == null)
                    return GetTitle(CultureInfo.CurrentUICulture);
                return _title;
            }
            set => _title = value;
        }

        /// <summary>
        /// The resource key from which to retrieve the <see cref="Title"/>
        /// </summary>
        public string TitleResourceKey { get; set; }

        /// <summary>
        /// A description of this module, explaining its function
        /// </summary>
        public string Description
        {
            get
            {
                if (_description == null)
                    return GetTitle(CultureInfo.CurrentUICulture);
                return _description;
            }
            set => _description = value;
        }

        /// <summary>
        /// The resource key from which to retrieve the <see cref="Description"/>
        /// </summary>
        public string DescriptionResourceKey { get; set; }

        /// <summary>
        /// The author of this module.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The version of this module. Defaults to 1.0.0
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Gets the title of the module. If the <see cref="Title"/> property is set, it's returned verbatim. Otherwise,
        /// if there's a valid <see cref="ResourceManagerType"/> and <see cref="TitleResourceKey"/>, those are used
        /// to retrieve the string.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetTitle(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (TitleResourceKey != null)
            {
                if (_resourceManager == null)
                    _resourceManager = ResourceUtilities.Get(ResourceManagerType);
                return _resourceManager.GetString(TitleResourceKey, culture);
            }
            return _title;
        }

        /// <summary>
        /// Gets the description of the module. If the <see cref="Description"/> property is set, it's returned verbatim.
        /// Otherwise, if there's a valid <see cref="ResourceManagerType"/> and <see cref="DescriptionResourceKey"/>, those
        /// are used to retrieve the string.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetDescription(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (DescriptionResourceKey != null)
            {
                if (_resourceManager == null)
                    _resourceManager = ResourceUtilities.Get(ResourceManagerType);
                return _resourceManager.GetString(DescriptionResourceKey, culture);
            }
            return _description;
        }
    }
}