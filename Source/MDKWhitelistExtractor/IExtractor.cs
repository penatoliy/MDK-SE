using System.Threading;
using SpaceEngineers.Game;

namespace Malware.MDKWhitelistExtractor
{
    interface IExtractor
    {
        void Invoke(CommandLine commandLine, SpaceEngineersGame game, SynchronizationContext synchronizationContext);
    }
}