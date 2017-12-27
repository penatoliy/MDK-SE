using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;
using JetBrains.Annotations;
using Malware.MDKModules;
using Malware.MDKServices;
using Malware.MDKUI.BlueprintManager;
using Malware.MDKUI.BugReports;
using Malware.MDKUI.ProjectIntegrity;
using Malware.MDKUI.UpdateDetection;
using MDK.Commands;
using MDK.Options;
using MDK.Options.Versioning;
using MDK.Resources;
using MDK.VisualStudio;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MDK
{
    /// <summary>
    /// The MDK Visual Studio Extension
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(Options.GeneralPage), "MDK/SE", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(Options.PluginsPage), "MDK/SE", "Plugins", 0, 0, true)]
    //    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
    public sealed partial class MDKPackage : ExtendedPackage, IMDK
    {
        /// <summary>
        /// RunMDKToolCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "7b9c2d3e-b001-4a3e-86a8-00dc6f2af032";

        bool _hasCheckedForUpdates;
        bool _isEnabled;
        SolutionManager _solutionManager;

        UpgraderRef[] _upgraders =
        {
            new UpgraderRef(1, typeof(UpgradeTo1))
        };

        /// <summary>
        /// Creates a new instance of <see cref="MDKPackage" />
        /// </summary>
        public MDKPackage()
        {
            ScriptUpgrades = new ScriptUpgrades();
            OutputPane = new OutputPane(this);
            Dialogs = new MDKDialogs(this);
        }

        /// <summary>
        /// Fired when the MDK features are enabled
        /// </summary>
        public event EventHandler Enabled;

        /// <summary>
        /// Fired when the MDK features are disabled
        /// </summary>
        public event EventHandler Disabled;

        /// <summary>
        /// Determines whether the package is currently busy deploying scripts.
        /// </summary>
        public bool IsDeploying { get; private set; }

        /// <summary>
        /// Determines whether the MDK features are currently enabled
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            private set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                if (_isEnabled)
                    Enabled?.Invoke(this, EventArgs.Empty);
                else
                    Disabled?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the MDK options
        /// </summary>
        public IMDKOptions Options { get; private set; }

        /// <summary>Utilities to show common dialogs</summary>
        public IMDKDialogs Dialogs { get; }

        /// <summary>
        /// Gets the output pane
        /// </summary>
        public IOutputPane OutputPane { get; }

        /// <summary>
        /// The service provider
        /// </summary>
        public IServiceProvider ServiceProvider => this;

        /// <summary>
        /// The <see cref="ScriptUpgrades"/> service
        /// </summary>
        public ScriptUpgrades ScriptUpgrades { get; }

        /// <summary>
        /// Gets the installation path for the current MDK package
        /// </summary>
        public DirectoryInfo InstallPath { get; } = new FileInfo(new Uri(typeof(MDKPackage).Assembly.CodeBase).LocalPath).Directory;

        /// <summary>
        /// Gets the manager for deployment modules
        /// </summary>
        public ModuleManager ModuleManager { get; } = new ModuleManager();

        /// <inheritdoc />
        public string WrapScript(string script)
        {
            throw new NotImplementedException();
            //MyScriptCompiler throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            _solutionManager?.Dispose();
            _solutionManager = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Options = new MDKOptions(ServiceProvider);
            UpgradeOptions((MDKOptions)Options);
            ((OutputPane)OutputPane).Initialize();

            AddCommand(
                new QuickDeploySolutionCommand(this),
                new DeployProjectCommand(this),
                new RefreshWhitelistCacheCommand(this),
                new CheckForUpdatesCommand(this),
                new ProjectOptionsCommand(this),
                new BlueprintManagerCommand(this),
                new GlobalBlueprintManagerCommand(this)
            );

            KnownUIContexts.ShellInitializedContext.WhenActivated(OnShellActivated);

            base.Initialize();
        }

        void UpgradeOptions(MDKOptions options)
        {
            var wasUpgraded = false;
            foreach (var upgrader in _upgraders)
            {
                if (upgrader.Version <= options.OptionsVersion)
                    continue;
                try
                {
                    upgrader.Upgrade(this, options);
                    wasUpgraded = true;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Error upgrading options from {options.OptionsVersion} to {upgrader.Version}", e);
                }
            }
            if (wasUpgraded)
                options.Save();
        }

        async void CheckForUpdates()
        {
            if (!Options.NotifyUpdates)
                return;
            // ReSharper disable once RedundantLogicalConditionalExpressionOperand
            var version = await CheckForUpdates(Options.NotifyPrereleaseUpdates || IsPrerelease);
            if (version != null)
                OnUpdateDetected(version);
        }

        /// <summary>
        /// Checks the GitHub sites for any updated releases.
        /// </summary>
        /// <returns>The newest release version on GitHub, or <c>null</c> if the current version is the latest</returns>
        public async Task<Version> CheckForUpdates(bool includePrerelease)
        {
            try
            {
                var client = new GitHub("malware-dev", "mdk-se", "mdk-se");
                var latestRelease = (await client.ReleasesAsync())
                    .Where(release => !string.IsNullOrWhiteSpace(release.TagName) && (!release.Prerelease || includePrerelease))
                    .OrderByDescending(r => r.PublishedAt)
                    .FirstOrDefault();
                if (latestRelease == null)
                    return null;

                var match = Regex.Match(latestRelease.TagName, @"\d+\.\d+(\.\d+)?");
                if (match.Success)
                {
                    var detectedVersion = new Version(match.Value);
                    if (detectedVersion > Version)
                        return detectedVersion;
                }
                return null;
            }
            catch (Exception e)
            {
                LogPackageError("CheckForUpdates", e);
                // We don't want to make a fuss about this.
                return null;
            }
        }

        void OnUpdateDetected(Version detectedVersion)
        {
            if (detectedVersion == null)
                return;
            UpdateDetectedDialog.ShowDialog(new UpdateDetectedDialogModel(detectedVersion, HelpPageUrl, ReleasePageUrl));
        }

        void OnShellActivated()
        {
            _solutionManager = new SolutionManager(this);
            _solutionManager.ProjectLoaded += OnProjectLoaded;
            _solutionManager.SolutionLoaded += OnSolutionLoaded;
            _solutionManager.SolutionClosed += OnSolutionClosed;
        }

        void OnSolutionClosed(object sender, EventArgs e)
        {
            IsEnabled = false;
        }

        void OnSolutionLoaded(object sender, EventArgs e)
        {
            OnSolutionLoaded(DTE.Solution);
        }

        void OnProjectLoaded(object sender, ProjectLoadedEventArgs e)
        {
            if (e.IsStandalone)
                OnProjectLoaded(e.Project);
        }

        async void OnProjectLoaded(Project project)
        {
            ScriptSolutionAnalysisResult result;
            try
            {
                result = await ScriptUpgrades.AnalyzeAsync(project, new ScriptUpgradeAnalysisOptions
                {
                    DefaultGameBinPath = Options.GetActualGameBinPath(),
                    InstallPath = InstallPath.FullName,
                    TargetVersion = Version,
                    GameAssemblyNames = GameAssemblyNames,
                    GameFiles = GameFiles,
                    UtilityAssemblyNames = UtilityAssemblyNames,
                    UtilityFiles = UtilityFiles
                });
            }
            catch (Exception e)
            {
                Dialogs.ShowError(Text.MDKPackage_OnProjectLoaded_ErrorAnalyzingProject, string.Format(Text.MDKPackage_OnProjectLoaded_ErrorAnalyzingProject_Description, project?.Name), e);
                IsEnabled = false;
                return;
            }
            if (!result.HasScriptProjects)
                return;
            IsEnabled = true;
            if (result.IsValid)
                return;

            QueryUpgrade(this, result);
        }

        void QueryUpgrade([NotNull] MDKPackage package, ScriptSolutionAnalysisResult result)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            var model = new RequestUpgradeDialogModel(package, result, HelpPageUrl)
            {
                SaveCallback = () => { package.ScriptUpgrades.Upgrade(result); }
            };
            RequestUpgradeDialog.ShowDialog(model);
        }

        async void OnSolutionLoaded(Solution solution)
        {
            ScriptSolutionAnalysisResult result;
            try
            {
                result = await ScriptUpgrades.AnalyzeAsync(solution, new ScriptUpgradeAnalysisOptions
                {
                    DefaultGameBinPath = Options.GetActualGameBinPath(),
                    InstallPath = InstallPath.FullName,
                    TargetVersion = Version,
                    GameAssemblyNames = GameAssemblyNames,
                    GameFiles = GameFiles,
                    UtilityAssemblyNames = UtilityAssemblyNames,
                    UtilityFiles = UtilityFiles
                });
            }
            catch (Exception e)
            {
                Dialogs.ShowError(Text.MDKPackage_OnSolutionLoaded_ErrorAnalyzingSolution, Text.MDKPackage_OnSolutionLoaded_ErrorAnalyzingSolution_Description, e);
                IsEnabled = false;
                return;
            }
            if (!result.HasScriptProjects)
            {
                IsEnabled = false;
                return;
            }
            IsEnabled = true;

            if (!result.IsValid)
                QueryUpgrade(this, result);

            if (!_hasCheckedForUpdates)
            {
                _hasCheckedForUpdates = true;
                CheckForUpdates();
            }
        }

        /// <summary>
        /// Deploys the all scripts in the solution or a single script project.
        /// </summary>
        /// <param name="project">The specific project to build</param>
        /// <param name="nonBlocking"><c>true</c> if there should be no blocking dialogs shown during deployment. Instead, an <see cref="InvalidOperationException"/> will be thrown for the more grievous errors, while other stoppers merely return false.</param>
        /// <returns></returns>
        public async Task<bool> Deploy(Project project = null, bool nonBlocking = false)
        {
            var dte = DTE;

            if (IsDeploying)
            {
                if (!nonBlocking)
                    Dialogs.ShowMessage(Text.MDKPackage_Deploy_DeploymentRejected, Text.MDKPackage_Deploy_Rejected_DeploymentInProgress, MessageType.Error);
                return false;
            }

            if (!dte.Solution.IsOpen)
            {
                if (!nonBlocking)
                    Dialogs.ShowMessage(Text.MDKPackage_Deploy_DeploymentRejected, Text.MDKPackage_Deploy_NoSolutionOpen, MessageType.Error);
                return false;
            }

            if (dte.Solution.SolutionBuild.BuildState == vsBuildState.vsBuildStateInProgress)
            {
                if (!nonBlocking)
                    Dialogs.ShowMessage(Text.MDKPackage_Deploy_DeploymentRejected, Text.MDKPackage_Deploy_Rejected_BuildInProgress, MessageType.Error);
                return false;
            }

            IsDeploying = true;
            try
            {
                int failedProjects;
                using (new StatusBarAnimation(ServiceProvider, Animation.Build))
                {
                    if (project != null)
                    {
                        dte.Solution.SolutionBuild.BuildProject(dte.Solution.SolutionBuild.ActiveConfiguration.Name, project.FullName, true);
                    }
                    else
                    {
                        dte.Solution.SolutionBuild.Build(true);
                    }
                    failedProjects = dte.Solution.SolutionBuild.LastBuildInfo;
                }

                if (failedProjects > 0)
                {
                    if (!nonBlocking)
                        Dialogs.ShowMessage(Text.MDKPackage_Deploy_DeploymentRejected, Text.MDKPackage_Deploy_BuildFailed, MessageType.Error);
                    return false;
                }

                string title;
                if (project != null)
                    title = string.Format(Text.MDKPackage_Deploy_DeployingSingleScript, Path.GetFileName(project.FullName));
                else
                    title = Text.MDKPackage_Deploy_DeployingAllScripts;
                ImmutableArray<Build> deployedScripts;
                using (var statusBar = new StatusBarProgressBar(ServiceProvider, title, 100))
                using (new StatusBarAnimation(ServiceProvider, Animation.Deploy))
                {
                    var build = new Builder(this);
                    deployedScripts = await build.Build(dte.Solution.FileName, project?.FullName, statusBar);
                }

                if (deployedScripts.Length > 0)
                {
                    if (!nonBlocking && Options.ShowBlueprintManagerOnDeploy)
                    {
                        var distinctPaths = deployedScripts.Select(script => FormattedPath(script.Options.OutputPath)).Distinct().ToArray();
                        if (distinctPaths.Length == 1)
                        {
                            var model = new BlueprintManagerDialogModel(
                                (IMDKWriteableOptions)Options,
                                HelpPageUrl,
                                Text.MDKPackage_Deploy_Description,
                                distinctPaths[0],
                                deployedScripts.Select(s => s.Options.Name));
                            BlueprintManagerDialog.ShowDialog(model);
                        }
                        else
                            Dialogs.ShowMessage(Text.MDKPackage_Deploy_DeploymentComplete, Text.MDKPackage_Deploy_DeploymentCompleteDescription, MessageType.Info);
                    }
                }
                else
                {
                    if (!nonBlocking)
                        Dialogs.ShowMessage(Text.MDKPackage_Deploy_DeploymentCancelled, Text.MDKPackage_Deploy_NoMDKProjects, MessageType.Info);
                    return false;
                }

                return true;
            }
            catch (UnauthorizedAccessException e)
            {
                if (!nonBlocking)
                    Dialogs.ShowMessage(Text.MDKPackage_Deploy_DeploymentCancelled, e.Message, MessageType.Error);
                else
                    throw;
                return false;
            }
            catch (Exception e)
            {
                if (!nonBlocking)
                    Dialogs.ShowError(Text.MDKPackage_Deploy_DeploymentFailed, Text.MDKPackage_Deploy_UnexpectedError, e);
                else
                    throw new InvalidOperationException("An unexpected error occurred during deployment.", e);
                return false;
            }
            finally
            {
                IsDeploying = false;
            }
        }

        string FormattedPath(string scriptOutputPath)
        {
            return Path.GetFullPath(scriptOutputPath).TrimEnd('\\').ToUpper();
        }

        /// <summary>
        /// Expands string macros associated with the given project.
        /// </summary>
        /// <param name="build"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public string ExpandMacros([NotNull] Build build, string source)
        {
            if (build == null)
                throw new ArgumentNullException(nameof(build));
            if (source == null)
                return null;
            var project = build.Project;
            return Regex.Replace(source, @"\$\(ProjectName\)", match =>
            {
                switch (match.Value.ToUpper())
                {
                    case "$(PROJECTNAME)":
                        return project.Name;
                    default:
                        return match.Value;
                }
            });
        }

        class MDKDialogs : IMDKDialogs
        {
            readonly MDKPackage _package;

            public MDKDialogs(MDKPackage package)
            {
                _package = package;
            }

            /// <inheritdoc />
            public BlueprintInfo ShowBlueprintDialog(MDKProjectOptions projectOptions, string customDescription = null)
            {
                var blueprintPath = projectOptions != null ? projectOptions.OutputPath : _package.Options.GetActualOutputPath();
                var model = new BlueprintManagerDialogModel((IMDKWriteableOptions)_package.Options, HelpPageUrl)
                {
                    BlueprintPath = blueprintPath,
                    CustomDescription = customDescription
                };
                if (BlueprintManagerDialog.ShowDialog(model) == true)
                    return model.SelectedBlueprint.GetBlueprintInfo();
                return BlueprintInfo.Empty;
            }

            /// <summary>
            /// Displays an error dialog
            /// </summary>
            /// <param name="title"></param>
            /// <param name="description"></param>
            /// <param name="exception"></param>
            public void ShowError(string title, string description, Exception exception)
            {
                var errorDialogModel = new ErrorDialogModel(HelpPageUrl)
                {
                    Title = title,
                    Description = description,
                    Log = exception.ToString()
                };
                ErrorDialog.ShowDialog(errorDialogModel);
            }

            /// <inheritdoc />
            public MessageResponse ShowMessage(string title, string description, MessageType type)
            {
                OLEMSGICON image;
                OLEMSGBUTTON buttons;
                OLEMSGDEFBUTTON defButton;
                switch (type)
                {
                    case MessageType.Confirm:
                        image = OLEMSGICON.OLEMSGICON_QUERY;
                        buttons = OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
                        defButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                        break;
                    case MessageType.Warning:
                        image = OLEMSGICON.OLEMSGICON_WARNING;
                        buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                        defButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                        break;
                    case MessageType.Error:
                        image = OLEMSGICON.OLEMSGICON_CRITICAL;
                        buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                        defButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                        break;
                    case MessageType.ConfirmOrCancel:
                        image = OLEMSGICON.OLEMSGICON_QUERY;
                        buttons = OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL;
                        defButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                        break;
                    default:
                        image = OLEMSGICON.OLEMSGICON_INFO;
                        buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                        defButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                        break;
                }
                var response = VsShellUtilities.ShowMessageBox(_package.ServiceProvider, description, title, image, buttons, defButton);
                switch (response)
                {
                    case 1:
                    case 6:
                        return MessageResponse.Accept;
                    case 2:
                        return MessageResponse.Cancel;
                    default:
                        return MessageResponse.Reject;
                }
            }
        }

        class UpgraderRef
        {
            Upgrader _upgrader;
            readonly Type _type;

            public UpgraderRef(int version, Type type)
            {
                Version = version;
                _type = type;
            }

            public int Version { get; }

            public void Upgrade(MDKPackage package, MDKOptions options)
            {
                if (_upgrader == null)
                    _upgrader = (Upgrader)Activator.CreateInstance(_type);
                _upgrader.Upgrade(package, options);
            }
        }
    }
}
