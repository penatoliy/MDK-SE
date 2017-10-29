using System;

namespace Malware.MDKModules
{
    /// <summary>
    /// Information about a given script blueprint
    /// </summary>
    public struct BlueprintInfo
    {
        /// <summary>
        /// Gets an empty blueprint info
        /// </summary>
        public static readonly BlueprintInfo Empty = new BlueprintInfo();

        /// <summary>
        /// The display name of this blueprint
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The path to the script file
        /// </summary>
        public readonly string ScriptFileName;

        /// <summary>
        /// An optional path to a thumb image (may be <c>null</c>
        /// </summary>
        public readonly string ThumbFileName;

        /// <summary>
        /// Creates a new <see cref="BlueprintInfo"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scriptFileName"></param>
        /// <param name="thumbFileName"></param>
        public BlueprintInfo(string name, string scriptFileName, string thumbFileName = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(scriptFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(scriptFileName));
            Name = name;
            ScriptFileName = scriptFileName;
            ThumbFileName = string.IsNullOrWhiteSpace(thumbFileName) ? null : thumbFileName;
        }

        /// <summary>
        /// Determines whether this structure is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() => Name != null;
    }
}