using System;
using System.Threading;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using SpaceEngineers.Game;
using VRage.Game;
using VRage.Plugins;
using VRage.Utils;

namespace Malware.MDKWhitelistExtractor
{
    public class ExtractorPlugin : IPlugin
    {
        static ExtractorPlugin()
        {
            SynchronizationContext.SetSynchronizationContext(new GameSynchronizationContext());
        }

        CommandLine _commandLine;
        bool _firstInit = true;

        public SpaceEngineersGame Game { get; private set; }

        public void Dispose()
        { }

        public void Init(object gameInstance)
        {
            _commandLine = new CommandLine(Environment.CommandLine);

            Game = (SpaceEngineersGame)gameInstance;
        }

        public void Update()
        {
            if (_firstInit)
            {
                _firstInit = false;
                var synchronizationContext = SynchronizationContext.Current;

                MyGuiSandbox.AddScreen(new MyGuiScreenProgressAsync(MyStringId.GetOrCompute("MDK is analyzing game data..."), null, () => new Process(Game, _commandLine, synchronizationContext),
                    (r, s) =>
                    {
                        s.CloseScreen();
                        MySandboxGame.ExitThreadSafe();
                    }));
            }
        }

        class GameSynchronizationContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback d, object state)
            {
                MySandboxGame.Static.Invoke(() => d(state), "ExtractorPlugin");
            }
        }

        class Process : IMyAsyncResult
        {
            readonly SpaceEngineersGame _game;
            readonly CommandLine _commandLine;
            readonly SynchronizationContext _synchronizationContext;

            readonly IExtractor[] _extractors =
            {
                new Terminals(),
                new Whitelist(),
                new ImplicitNamespaces(),
                new WrapperTemplate()
            };

            TaskCompletionSource<bool> _taskCompletion;
            Task _task;

            public Process(SpaceEngineersGame game, CommandLine commandLine, SynchronizationContext synchronizationContext)
            {
                _game = game;
                _commandLine = commandLine;
                _synchronizationContext = synchronizationContext;
                _task = Run();
            }

            public bool IsCompleted => _task.IsCompleted;
            ParallelTasks.Task IMyAsyncResult.Task => default(ParallelTasks.Task);

            async Task Run()
            {
                _taskCompletion = new TaskCompletionSource<bool>();
                MySession.AfterLoading += MySession_AfterLoading;
                MySessionLoader.LoadInventoryScene();

                await _taskCompletion.Task;
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            void MySession_AfterLoading()
            {
                foreach (var extractor in _extractors)
                    extractor.Invoke(_commandLine, _game, _synchronizationContext);
                _taskCompletion.SetResult(true);
            }
        }
    }
}