using System.IO;

namespace Malware.MDKServices.Import
{
    public class ScriptFile
    {
        public ScriptFile(string fileName, string content)
        {
            Content = content ?? "";
            FileName = Path.GetFullPath(fileName);
        }

        public string Content { get; }

        public string FileName { get; }
    }
}