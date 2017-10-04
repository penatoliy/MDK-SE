using System;

namespace Malware.MDKServices.Import
{
    public class ScriptService : IScriptService
    {
        static readonly string[] ImplicitNamespaces =
        {
            "Sandbox.Game.EntityComponents",
            "Sandbox.ModAPI.Ingame",
            "Sandbox.ModAPI.Interfaces",
            "SpaceEngineers.Game.ModAPI.Ingame",
            "System.Collections.Generic",
            "System.Collections",
            "System.Linq",
            "System.Text",
            "System",
            "VRage.Collections",
            "VRage.Game.Components",
            "VRage.Game.ModAPI.Ingame",
            "VRage.Game.ObjectBuilders.Definitions",
            "VRage.Game",
            "VRageMath"
        };

        static readonly TextTemplate WrapperWithNamespaceTemplate = new TextTemplate(@"<% Namespaces %>

namespace <% Namespace %>
{
    partial class Program: MyGridProgram
    {
        <% Content %>
    }
}
");

        static readonly TextTemplate WrapperWithoutNamespaceTemplate = new TextTemplate(@"<% Namespaces %>

partial class Program: MyGridProgram
{
    <% Content %>
}
");

        /// <summary>
        /// Wraps a script in 
        /// </summary>
        /// <param name="script"></param>
        /// <param name="namespaceName">An optional namespace to wrap the script in</param>
        /// <returns></returns>
        public string WrapScript(string script, string namespaceName = null)
        {
            var template = namespaceName == null ? WrapperWithoutNamespaceTemplate : WrapperWithNamespaceTemplate; 
            var namespaces = $"using {string.Join("\r\nusing ", ImplicitNamespaces)}";
            return template.Apply(key =>
            {
                switch (key.ToUpperInvariant())
                {
                    case "NAMESPACES":
                        return namespaces;
                    case "CONTENT":
                        return script;
                    case "NAMESPACE":
                        return "IngameScript";
                    default:
                        return string.Empty;
                }
            });
        }

        public string UnwrapScript(string script)
        {
            throw new NotImplementedException();
        }
    }
}