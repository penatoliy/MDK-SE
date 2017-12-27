using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using Malware.MDKModules;
using Malware.MDKUI.Properties;

namespace Malware.MDKUI.BlueprintManager
{
    /// <summary>
    /// Represents a single blueprint
    /// </summary>
    public class BlueprintModel : Model, IEditableObject, INotifyDataErrorInfo
    {
        readonly Func<string, string, MessageEventType, bool, bool> _sendMessageCallback;
        bool _isSignificant;
        string _name;
        string _editedName;
        bool _isBeingEdited;
        bool _editedNameIsValid;
        string _renameError;

        BlueprintModel()
        {
            CopyToClipboardCommand = new ModelCommand(CopyToClipboard);
        }

        /// <summary>
        /// Creates a new instance of the blueprint model
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="thumbnail"></param>
        /// <param name="directory"></param>
        /// <param name="isSignificant"></param>
        /// <param name="sendMessageCallback"></param>
        public BlueprintModel([NotNull] BlueprintManagerDialogModel manager, ImageSource thumbnail, [NotNull] DirectoryInfo directory, bool isSignificant, Func<string, string, MessageEventType, bool, bool> sendMessageCallback)
            : this()
        {
            _sendMessageCallback = sendMessageCallback;
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));
            Thumbnail = thumbnail;
            Directory = directory ?? throw new ArgumentNullException(nameof(directory));
            Name = Directory.Name;
            IsSignificant = isSignificant;
        }

        void SendMessage(string title, string description, MessageEventType type, bool defaultResult = true)
        {
            _sendMessageCallback?.Invoke(title, description, type, defaultResult);
        }

        /// <inheritdoc cref="INotifyDataErrorInfo.ErrorsChanged"/>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// The script directory
        /// </summary>
        public DirectoryInfo Directory { get; }

        /// <summary>
        /// Gets the blueprint manager model this blueprint belongs to
        /// </summary>
        public BlueprintManagerDialogModel Manager { get; }

        /// <summary>
        /// An optional thumbnail
        /// </summary>
        public ImageSource Thumbnail { get; }

        /// <summary>
        /// A command to copy the script this model represents to the clipboard.
        /// </summary>
        public ModelCommand CopyToClipboardCommand { get; }

        /// <summary>
        /// The name of this blueprint
        /// </summary>
        public string Name
        {
            get => _name;
            private set
            {
                if (value == _name)
                    return;
                _name = value;
                EditedName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets and sets the name when it's being edited.
        /// </summary>
        public string EditedName
        {
            get => _editedName;
            set
            {
                if (value == _editedName)
                    return;
                _editedName = value;
                _renameError = null;
                EditedNameIsValid = !string.IsNullOrEmpty(value) && value.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
                OnPropertyChanged();
            }
        }

        bool EditedNameIsValid
        {
            get => _editedNameIsValid;
            set
            {
                if (_editedNameIsValid == value)
                    return;
                _editedNameIsValid = value;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(EditedName)));
            }
        }

        /// <summary>
        /// Determines whether this blueprint is currently in edit mode.
        /// </summary>
        public bool IsBeingEdited
        {
            get => _isBeingEdited;
            private set
            {
                if (value == _isBeingEdited)
                    return;
                _isBeingEdited = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Determines whether this particular blueprint is significant in any way. Is used to emphasize newly deployed scripts, for instance.
        /// </summary>
        public bool IsSignificant
        {
            get => _isSignificant;
            set
            {
                if (value == _isSignificant)
                    return;
                _isSignificant = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="INotifyDataErrorInfo.HasErrors"/>
        public bool HasErrors => !EditedNameIsValid;

        /// <inheritdoc cref="IEditableObject.BeginEdit"/>
        public void BeginEdit()
        {
            if (IsBeingEdited)
                return;
            _renameError = null;
            IsBeingEdited = true;
        }

        /// <inheritdoc cref="IEditableObject.EndEdit"/>
        public void EndEdit()
        {
            if (HasErrors)
                return;
            if (Name != EditedName)
            {
                try
                {
                    var newPath = Path.Combine(Directory.Parent?.FullName ?? ".", EditedName);
                    Directory.MoveTo(newPath);
                }
                catch (Exception exception)
                {
                    _renameError = exception.Message;
                    EditedNameIsValid = false;
                }
            }
            IsBeingEdited = false;
            Name = EditedName;
        }

        /// <inheritdoc cref="IEditableObject.CancelEdit"/>
        public void CancelEdit()
        {
            _renameError = null;
            EditedName = Name;
            IsBeingEdited = false;
        }

        /// <inheritdoc cref="INotifyDataErrorInfo.GetErrors"/>
        public IEnumerable GetErrors(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(EditedName):
                    if (!EditedNameIsValid)
                        yield return _renameError ?? string.Format(Resources.BlueprintModel_GetErrors_InvalidName, string.Join(" ", Path.GetInvalidFileNameChars().Where(ch => !char.IsControl(ch))));
                    break;
            }
        }

        /// <summary>
        /// Deletes this blueprint
        /// </summary>
        public void Delete()
        {
            try
            {
                Directory.Delete(true);
            }
            catch (Exception e)
            {
                Manager.SendMessage(Resources.BlueprintModel_Delete_Error, string.Format(Resources.BlueprintModel_Error_Description, Name, e.Message), MessageEventType.Error);
            }
        }

        /// <summary>
        /// Opens the target folder of this blueprint
        /// </summary>
        public void OpenFolder()
        {
            if (IsBeingEdited)
                CancelEdit();
            var process = new Process
            {
                StartInfo =
                {
                    FileName = Directory.FullName
                }
            };
            process.Start();
        }

        /// <summary>
        /// Gets a <see cref="BlueprintInfo"/> for this model
        /// </summary>
        /// <returns></returns>
        public BlueprintInfo GetBlueprintInfo()
        {
            var scriptFileName = Path.Combine(Directory.FullName, "script.cs");
            var thumbFileName = Path.Combine(Directory.FullName, "thumb.png");
            return new BlueprintInfo(Name, scriptFileName, thumbFileName);
        }

        /// <summary>
        /// Copies the blueprint this model represents to the clipboard.
        /// </summary>
        public void CopyToClipboard()
        {
            try
            {
                var item = this;
                var fileInfo = new FileInfo(Path.Combine(item.Directory.FullName, "script.cs"));
                if (fileInfo.Exists)
                {
                    var script = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
                    Clipboard.SetText(script, TextDataFormat.UnicodeText);
                    SendMessage(Resources.BlueprintManagerDialogModel_CopyToClipboard_Copy, Resources.BlueprintManagerDialogModel_CopyToClipboard_Copy_Description, MessageEventType.Info);
                }
            }
            catch (Exception e)
            {
                SendMessage(Resources.BlueprintManagerDialogModel_CopyToClipboard_Copy_Error, string.Format(Resources.BlueprintManagerDialogModel_CopyToClipboard_Copy_Error_Description, e.Message), MessageEventType.Error);
            }
        }
    }
}
