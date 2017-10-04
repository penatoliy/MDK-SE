using System;
using System.IO;

namespace Malware.MDKServices.Import
{
    /// <summary>
    /// An importer which imports a script blueprint from the local Space Engineers workshop folder
    /// </summary>
    public class LocalBlueprintImporter : BasicBlueprintImporter
    {
        public static void TestMe()
        {
            var serviceProvider = new ServiceProvider();
            serviceProvider.RegisterService<IScriptService>(new ScriptService());

            var importer = new LocalBlueprintImporter();
            var result = importer.Import(@"E:\sesaves\IngameScripts\local\MMasters LCDs 2", serviceProvider);

        }

        /// <inheritdoc />
        protected override string LoadScript(string sourcePath, IServiceProvider serviceProvider)
        {
            var scriptFileName = Path.Combine(sourcePath, "script.cs");
            return File.ReadAllText(scriptFileName);
        }

        /// <inheritdoc />
        protected override void ValidateSourcePath(string sourcePath, IServiceProvider serviceProvider)
        {
            if (!Directory.Exists(sourcePath))
                throw new ArgumentException("Expected a directory path", nameof(sourcePath));

            var scriptFileName = Path.Combine(sourcePath, "script.cs");
            if (!File.Exists(scriptFileName))
                throw new ArgumentException($"No script found at {sourcePath}", nameof(sourcePath));
        }
    }
}
