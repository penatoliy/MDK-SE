using System;

namespace Malware.MDKModules
{
    public interface IMDK
    {
        string ExpandMacros(Build build, string source);

        Version PackageVersion { get; }
    }
}