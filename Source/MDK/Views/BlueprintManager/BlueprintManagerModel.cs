using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Creventive.SteamAPI;
using MDK.Resources;

namespace MDK.Views.BlueprintManager
{
    /// <summary>
    /// The view model for <see cref="BlueprintManagerDialog"/>
    /// </summary>
    public class BlueprintManagerDialogModel : DialogViewModel
    {
        string _blueprintPath;
        BlueprintModel _selectedBlueprint;
        HashSet<string> _significantBlueprints;
        string _customDescription;
        Task _remoteBlueprintsTask;
        CancellationTokenSource _remoteBlueprintsLoadCancellation;
        string _closeText = Text.BlueprintManagerDialogModel_CloseText_Close;
        bool _isCancelable;

        /// <summary>
        /// Creates an instance of <see cref="BlueprintManagerDialogModel"/>
        /// </summary>
        public BlueprintManagerDialogModel()
        {
            DeleteCommand = new ModelCommand(Delete, false);
            RenameCommand = new ModelCommand(Rename, false);
            OpenFolderCommand = new ModelCommand(OpenFolder, false);
        }

        /// <summary>
        /// Creates an instance of <see cref="BlueprintManagerDialogModel"/>
        /// </summary>
        /// <param name="customDescription"></param>
        /// <param name="blueprintPath"></param>
        /// <param name="significantBlueprints"></param>
        public BlueprintManagerDialogModel(string customDescription, string blueprintPath, IEnumerable<string> significantBlueprints)
            : this()
        {
            _significantBlueprints = new HashSet<string>(significantBlueprints, StringComparer.CurrentCultureIgnoreCase);

            CustomDescription = customDescription;
            BlueprintPath = blueprintPath;
        }

        /// <summary>
        /// Occurs when a message has been sent which a user should respond to
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageRequested;

        /// <summary>
        /// Called before deleting a blueprint to allow UI confirmation
        /// </summary>
        public event EventHandler<DeleteBlueprintEventArgs> DeletingBlueprint;

        /// <summary>
        /// Called when all blueprints have been loaded
        /// </summary>
        public event EventHandler BlueprintsLoaded;

        /// <summary>
        /// A list of all available blueprints
        /// </summary>
        public ObservableCollection<BlueprintModel> Blueprints { get; } = new ObservableCollection<BlueprintModel>();

        /// <summary>
        /// Indicates the currently selected blueprint
        /// </summary>
        public BlueprintModel SelectedBlueprint
        {
            get => _selectedBlueprint;
            set
            {
                if (Equals(value, _selectedBlueprint))
                    return;
                _selectedBlueprint = value;
                RenameCommand.IsEnabled = _selectedBlueprint?.CanBeEdited ?? false;
                DeleteCommand.IsEnabled = _selectedBlueprint?.CanBeDeleted ?? false;
                OpenFolderCommand.IsEnabled = _selectedBlueprint != null;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The text to display on the close button
        /// </summary>
        public string CloseText
        {
            get => _closeText;
            set
            {
                if (value == _closeText)
                    return;
                _closeText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Determines whether this dialog can be canceled.
        /// </summary>
        public bool IsCancelable
        {
            get => _isCancelable;
            set
            {
                if (value == _isCancelable)
                    return;
                _isCancelable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A command to rename the currently selected blueprint
        /// </summary>
        public ModelCommand RenameCommand { get; }

        /// <summary>
        /// A command to delete the currently selected blueprint
        /// </summary>
        public ModelCommand DeleteCommand { get; }

        /// <summary>
        /// A command to open the target folder of a selected blueprint
        /// </summary>
        public ModelCommand OpenFolderCommand { get; set; }

        /// <summary>
        /// A custom description to show at the top of the dialog.
        /// </summary>
        public string CustomDescription
        {
            get => _customDescription;
            set
            {
                if (value == _customDescription)
                    return;
                _customDescription = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The blueprint path where the scripts are stored (equivalent to the output path of the script generator)
        /// </summary>
        public string BlueprintPath
        {
            get => _blueprintPath;
            set
            {
                if (value == _blueprintPath)
                    return;
                _blueprintPath = value;
                LoadBlueprints();
                OnPropertyChanged();
            }
        }

        void OpenFolder()
        {
            var item = SelectedBlueprint;
            item?.OpenFolder();
        }

        void Rename()
        {
            var item = SelectedBlueprint;
            item?.BeginEdit();
        }

        void Delete()
        {
            var item = SelectedBlueprint;
            if (item == null)
                return;
            if (item.IsBeingEdited)
                item.CancelEdit();

            var args = new DeleteBlueprintEventArgs(item);
            DeletingBlueprint?.Invoke(this, args);
            if (args.Cancel)
                return;

            try
            {
                item.Delete();
                Blueprints.Remove(item);
            }
            catch (Exception e)
            {
                SendMessage(Text.BlueprintManagerDialogModel_Delete_Error, string.Format(Text.BlueprintManagerDialogModel_Delete_Error_Description, e.Message), MessageEventType.Error);
            }
        }

        void LoadBlueprints()
        {
            Blueprints.Clear();
            LoadLocalBlueprints();
            _remoteBlueprintsLoadCancellation = new CancellationTokenSource();
            _remoteBlueprintsTask = LoadWorkshopBlueprints(_remoteBlueprintsLoadCancellation.Token);
            OnBlueprintsLoaded();
        }

        async Task LoadWorkshopBlueprints(CancellationToken token)
        {
            var blueprintDirectory = new DirectoryInfo(BlueprintPath).Parent?.EnumerateDirectories("workshop").FirstOrDefault();
            if (blueprintDirectory?.Exists ?? false)
            {
                var files = blueprintDirectory.EnumerateFiles("*.sbs")
                    .Select(f =>
                    {
                        if (!long.TryParse(Path.GetFileNameWithoutExtension(f.Name), out var id))
                            return null;
                        return new
                        {
                            Id = id,
                            FileInfo = f
                        };
                    })
                    .Where(f => f != null)
                    .ToArray();
                var ids = files.Select(f => f.Id).ToArray();
                Dictionary<long, SteamPublishedFileDetails> results;
                using (var steamApi = new SteamApi())
                {
                    try
                    {
                        results = (await steamApi.GetPublishedFileDetailsAsync(token, ids))
                            .ToDictionary(file => file.PublishedFileId);
                    }
                    catch (Exception)
                    {
                        results = new Dictionary<long, SteamPublishedFileDetails>();
                        // Not important
                        // TODO: Logging
                    }
                }

                foreach (var file in files)
                {
                    bool found;
                    string name;
                    if (results.TryGetValue(file.Id, out var details))
                    {
                        found = true;
                        name = details.Title;
                    }
                    else
                    {
                        found = false;
                        name = file.Id.ToString();
                    }

                    var model = new BlueprintModel(this, "Steam Workshop", null, file.FileInfo, name, found, _significantBlueprints?.Contains(file.FileInfo.Name) ?? false);
                    Blueprints.Add(model);
                }
            }
        }

        void LoadLocalBlueprints()
        {
            var blueprintDirectory = new DirectoryInfo(BlueprintPath);
            if (blueprintDirectory.Exists)
            {
                foreach (var folder in blueprintDirectory.EnumerateDirectories())
                {
                    if (!File.Exists(Path.Combine(folder.FullName, "script.cs")))
                        continue;

                    BitmapImage icon = null;
                    var thumbFileName = Path.Combine(folder.FullName, "thumb.png");
                    if (File.Exists(thumbFileName))
                    {
                        icon = new BitmapImage();
                        icon.BeginInit();
                        icon.DecodePixelWidth = 32;
                        icon.UriSource = new Uri(thumbFileName);
                        icon.CacheOption = BitmapCacheOption.OnLoad;
                        icon.CreateOptions = BitmapCreateOptions.DelayCreation;
                        icon.EndInit();
                    }

                    var model = new BlueprintModel(this, "Local Workshop", icon, folder, null, true, _significantBlueprints?.Contains(folder.Name) ?? false);
                    Blueprints.Add(model);
                }
            }
        }

        /// <inheritdoc />
        /// <returns></returns>
        protected override bool OnSave()
        {
            return true;
        }

        /// <summary>
        /// Sends a message through the user interface to the end-user.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="defaultResult"></param>
        /// <returns></returns>
        public bool SendMessage(string title, string description, MessageEventType type, bool defaultResult = true)
        {
            var args = new MessageEventArgs(title, description, type, !defaultResult);
            MessageRequested?.Invoke(this, args);
            return !args.Cancel;
        }

        /// <summary>
        /// Fires the <see cref="BlueprintsLoaded"/> event.
        /// </summary>
        protected virtual void OnBlueprintsLoaded()
        {
            BlueprintsLoaded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tells the model that the connected window is closing.
        /// </summary>
        public void OnWindowClosing()
        {
            try
            {
                _remoteBlueprintsLoadCancellation?.Cancel();
                _remoteBlueprintsTask?.Wait();
            }
            catch (Exception)
            {
                // Ignore, it's not important
            }
        }
    }
}
