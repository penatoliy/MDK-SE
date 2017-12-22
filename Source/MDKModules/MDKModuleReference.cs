using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Malware.MDKModules
{
    /// <summary>
    /// Represents a reference to an MDK module
    /// </summary>
    public sealed class MDKModuleReference
    {
        /// <summary>
        /// Creates a module reference from an XML element
        /// </summary>
        /// <param name="moduleElement"></param>
        /// <returns></returns>
        public static MDKModuleReference FromXElement(XElement moduleElement)
        {
            var id = moduleElement.Attribute("id")?.AsGuid();
            if (id == null)
                return null;
            var reference = new MDKModuleReference(id.Value);

            foreach (var element in moduleElement.Elements().Where(e => string.Equals(moduleElement.Name.NamespaceName, e.Name.NamespaceName, StringComparison.InvariantCultureIgnoreCase)))
                reference.Options[element.Name.LocalName] = element.Value;

            return reference;
        }

        /// <summary>
        /// Creates a new module reference
        /// </summary>
        /// <param name="id"></param>
        public MDKModuleReference(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// The ID of the module
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// A list of simple options
        /// </summary>
        public Dictionary<string, string> Options { get; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Creates an XML element from this reference
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XElement ToXElement(string name)
        {
            var moduleElement = new XElement(name, new XAttribute("id", Id.ToString("D")));
            foreach (var pair in Options)
                moduleElement.Add(new XElement(pair.Key, pair.Value));
            return moduleElement;
        }
    }
}