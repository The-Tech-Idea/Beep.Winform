using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    /// <summary>
    /// Static dialog manager for displaying various types of dialogs
    /// </summary>
    public static partial class DialogManager
    {
        private static BeepPopupForm? _hostForm;
        private static readonly Dictionary<int, Form> _progressDialogs = new Dictionary<int, Form>();
        private static int _progressTokenCounter = 0;

        /// <summary>
        /// Gets the host form for dialogs
        /// </summary>
        public static BeepPopupForm? HostForm
        {
            get => _hostForm;
            private set => _hostForm = value;
        }

        /// <summary>
        /// Sets the host form for dialogs
        /// </summary>
        public static void SetHostForm(BeepPopupForm popupForm)
        {
            HostForm = popupForm;
        }

        #region Basic Message Dialogs

        /// <summary>
        /// Display a simple message box
        /// </summary>
        public static void MsgBox(string title, string promptText)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = promptText;
                dialog.DialogType = DialogType.Information;
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Show a message with custom icon
        /// </summary>
        public static void ShowMessege(string title, string message, string icon)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = message;
                dialog.DialogType = DialogType.Information;
                // TODO: Set custom icon path if needed
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Show an alert dialog
        /// </summary>
        public static DialogReturn ShowAlert(string title, string message, string icon)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = message;
                dialog.DialogType = DialogType.Warning;
                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Cancel = result == DialogResult.Cancel
                };
            }
        }

        /// <summary>
        /// Show exception details
        /// </summary>
        public static void ShowException(string title, Exception ex)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title ?? "Error";
                dialog.Message = ex.Message;
                dialog.DialogType = DialogType.Error;
                // TODO: Add details section with stack trace
                dialog.ShowDialog();
            }
        }

        #endregion

        #region Input Dialogs

        /// <summary>
        /// Display input box for single line text
        /// </summary>
        public static DialogReturn InputBox(string title, string promptText)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = promptText;
                dialog.DialogType = DialogType.GetInputString;
                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Value = dialog.ReturnValue ?? string.Empty,
                    Cancel = result == DialogResult.Cancel,
                    Submit = result == DialogResult.OK
                };
            }
        }

        /// <summary>
        /// Display input box for multi-line text
        /// </summary>
        public static DialogReturn InputLargeBox(string title, string promptText)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = promptText;
                dialog.DialogType = DialogType.GetInputString;
                // TODO: Configure for multi-line input
                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Value = dialog.ReturnValue ?? string.Empty,
                    Cancel = result == DialogResult.Cancel,
                    Submit = result == DialogResult.OK
                };
            }
        }

        /// <summary>
        /// Display input box with Yes/No buttons
        /// </summary>
        public static DialogReturn InputBoxYesNo(string title, string promptText)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = promptText;
                dialog.DialogType = DialogType.Question;
                dialog.DialogButtons = BeepDialogButtons.YesNo;
                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Value = dialog.ReturnValue ?? string.Empty,
                    Cancel = result == DialogResult.No || result == DialogResult.Cancel,
                    Submit = result == DialogResult.Yes
                };
            }
        }

        /// <summary>
        /// Display password input box
        /// </summary>
        public static DialogReturn InputPassword(string title, string promptText, bool masked = true)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = promptText;
                dialog.DialogType = DialogType.GetInputString;
                // TODO: Set password masking
                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Value = dialog.ReturnValue ?? string.Empty,
                    Cancel = result == DialogResult.Cancel,
                    Submit = result == DialogResult.OK
                };
            }
        }

        #endregion

        #region List Selection Dialogs

        /// <summary>
        /// Display combo box selection dialog
        /// </summary>
        public static DialogReturn InputComboBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = promptText;
                dialog.DialogType = DialogType.GetInputFromList;
                dialog.Items = itvalues;
                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Value = dialog.ReturnValue ?? string.Empty,
                    Tag = dialog.ReturnItem,
                    Cancel = result == DialogResult.Cancel,
                    Submit = result == DialogResult.OK
                };
            }
        }

        /// <summary>
        /// Display combo box with string list
        /// </summary>
        public static DialogReturn InputComboBox(string title, string promptText, List<string> values)
        {
            var items = values.Select(v => new SimpleItem { Text = v, Value = v }).ToList();
            return InputComboBox(title, promptText, items);
        }

        /// <summary>
        /// Display list box selection dialog
        /// </summary>
        public static DialogReturn InputListBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            // Similar to combo box but with list box UI
            return InputComboBox(title, promptText, itvalues);
        }

        /// <summary>
        /// Display radio button group selection
        /// </summary>
        public static DialogReturn InputRadioGroupBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            // Similar to combo box but with radio buttons
            return InputComboBox(title, promptText, itvalues);
        }

        /// <summary>
        /// Display multi-select list
        /// </summary>
        public static DialogReturn MultiSelect(string title, string promptText, List<SimpleItem> items)
        {
            // TODO: Implement multi-select dialog
            return new DialogReturn
            {
                Result = BeepDialogResult.Cancel,
                Items = new List<SimpleItem>(),
                Cancel = true
            };
        }

        /// <summary>
        /// Display checklist dialog
        /// </summary>
        public static DialogReturn InputCheckList(string title, string promptText, List<SimpleItem> items)
        {
            // TODO: Implement checklist dialog
            return new DialogReturn
            {
                Result = BeepDialogResult.Cancel,
                Items = new List<SimpleItem>(),
                Cancel = true
            };
        }

        /// <summary>
        /// Custom combo dialog
        /// </summary>
        public static DialogReturn DialogCombo(string text, List<SimpleItem> comboSource, string DisplyMember, string ValueMember)
        {
            return InputComboBox("Select", text, comboSource);
        }

        #endregion

        #region Numeric and Date/Time Input

        /// <summary>
        /// Input integer with validation
        /// </summary>
        public static DialogReturn InputInt(string title, string promptText, int? min = null, int? max = null, int? @default = null)
        {
            var result = InputBox(title, promptText);
            if (result.Submit && int.TryParse(result.Value, out int value))
            {
                if ((min.HasValue && value < min.Value) || (max.HasValue && value > max.Value))
                {
                    MsgBox("Invalid Input", $"Value must be between {min ?? int.MinValue} and {max ?? int.MaxValue}");
                    result.Cancel = true;
                    result.Submit = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Input double with validation
        /// </summary>
        public static DialogReturn InputDouble(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null)
        {
            var result = InputBox(title, promptText);
            if (result.Submit && double.TryParse(result.Value, out double value))
            {
                if ((min.HasValue && value < min.Value) || (max.HasValue && value > max.Value))
                {
                    MsgBox("Invalid Input", $"Value must be between {min ?? double.MinValue} and {max ?? double.MaxValue}");
                    result.Cancel = true;
                    result.Submit = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Input DateTime with validation
        /// </summary>
        public static DialogReturn InputDateTime(string title, string promptText, DateTime? min = null, DateTime? max = null, DateTime? @default = null)
        {
            // TODO: Implement DateTimePicker dialog
            return InputBox(title, promptText);
        }

        /// <summary>
        /// Input TimeSpan with validation
        /// </summary>
        public static DialogReturn InputTimeSpan(string title, string promptText, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? @default = null)
        {
            // TODO: Implement TimeSpan picker dialog
            return InputBox(title, promptText);
        }

        #endregion

        #region Confirmation Dialogs

        /// <summary>
        /// Show confirmation dialog with custom buttons
        /// </summary>
        public static DialogReturn Confirm(string title, string message, IEnumerable<Vis.Modules.BeepDialogButtons> buttons, BeepDialogIcon icon, Vis.Modules.BeepDialogButtons? defaultButton = null)
        {
            // Map to BeepDialogModal enum
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = message;
                dialog.DialogType = MapIconToDialogType(icon);
                
                // Map buttons
                var buttonList = buttons.ToList();
                if (buttonList.Count == 2)
                {
                    if (buttonList.Contains(Vis.Modules.BeepDialogButtons.Yes) && buttonList.Contains(Vis.Modules.BeepDialogButtons.No))
                        dialog.DialogButtons = BeepDialogButtons.YesNo;
                    else if (buttonList.Contains(Vis.Modules.BeepDialogButtons.Ok) && buttonList.Contains(Vis.Modules.BeepDialogButtons.Cancel))
                        dialog.DialogButtons = BeepDialogButtons.OkCancel;
                }

                var result = dialog.ShowDialog();

                return new DialogReturn
                {
                    Result = ConvertDialogResult(result),
                    Cancel = result == DialogResult.Cancel || result == DialogResult.No,
                    Submit = result == DialogResult.OK || result == DialogResult.Yes
                };
            }
        }

        /// <summary>
        /// Confirm file overwrite
        /// </summary>
        public static DialogReturn ConfirmOverwrite(string filePath)
        {
            return Confirm("Confirm Overwrite",
                $"The file '{System.IO.Path.GetFileName(filePath)}' already exists. Do you want to overwrite it?",
                new[] { Vis.Modules.BeepDialogButtons.Yes, Vis.Modules.BeepDialogButtons.No },
                BeepDialogIcon.Question);
        }

        #endregion

        #region File and Folder Dialogs

        /// <summary>
        /// Open file dialog
        /// </summary>
        public static DialogReturn LoadFileDialog(string exts, string dir, string filter)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = filter;
                ofd.InitialDirectory = dir;
                ofd.DefaultExt = exts;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = ofd.FileName,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        /// <summary>
        /// Open file dialog with initial filename
        /// </summary>
        public static DialogReturn LoadFileDialog(string exts, string dir, string filter, string initialFileName)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = filter;
                ofd.InitialDirectory = dir;
                ofd.DefaultExt = exts;
                ofd.FileName = initialFileName;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = ofd.FileName,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        /// <summary>
        /// Open multiple files dialog
        /// </summary>
        public static List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = filter;
                ofd.InitialDirectory = dir;
                ofd.DefaultExt = exts;
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileNames.ToList();
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Save file dialog
        /// </summary>
        public static DialogReturn SaveFileDialog(string exts, string dir, string filter)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = filter;
                sfd.InitialDirectory = dir;
                sfd.DefaultExt = exts;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = sfd.FileName,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        /// <summary>
        /// Save file dialog with default filename
        /// </summary>
        public static DialogReturn SaveFileDialog(string exts, string dir, string filter, string defaultFileName)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = filter;
                sfd.InitialDirectory = dir;
                sfd.DefaultExt = exts;
                sfd.FileName = defaultFileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = sfd.FileName,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        /// <summary>
        /// Select folder dialog
        /// </summary>
        public static DialogReturn SelectFolderDialog()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = fbd.SelectedPath,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        /// <summary>
        /// Select folder dialog with options
        /// </summary>
        public static DialogReturn SelectFolderDialog(string title, string initialDir, bool allowCreate)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = title;
                fbd.SelectedPath = initialDir;
                fbd.ShowNewFolderButton = allowCreate;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = fbd.SelectedPath,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        /// <summary>
        /// Select file from list
        /// </summary>
        public static DialogReturn SelectFile(List<SimpleItem> files, string filter)
        {
            return InputComboBox("Select File", "Choose a file:", files);
        }

        #endregion

        #region Special Directories

        /// <summary>
        /// Select special directory using combo box
        /// </summary>
        public static DialogReturn SelectSpecialDirectoriesComboBox()
        {
            var dirs = GetSpecialDirectories();
            return InputComboBox("Select Directory", "Choose a special directory:", dirs);
        }

        /// <summary>
        /// Select special directory using list box
        /// </summary>
        public static DialogReturn SelectSpecialDirectoriesListBox()
        {
            var dirs = GetSpecialDirectories();
            return InputListBox("Select Directory", "Choose a special directory:", dirs);
        }

        /// <summary>
        /// Select special directory using radio buttons
        /// </summary>
        public static DialogReturn SelectSpecialDirectoriesRadioGroupBox()
        {
            var dirs = GetSpecialDirectories();
            return InputRadioGroupBox("Select Directory", "Choose a special directory:", dirs);
        }

        private static List<SimpleItem> GetSpecialDirectories()
        {
            return new List<SimpleItem>
            {
                new SimpleItem { Text = "Desktop", Value = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
                new SimpleItem { Text = "My Documents", Value = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
                new SimpleItem { Text = "Program Files", Value = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) },
                new SimpleItem { Text = "AppData", Value = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) },
                new SimpleItem { Text = "Temp", Value = System.IO.Path.GetTempPath() }
            };
        }

        #endregion

        #region Color and Font Pickers

        /// <summary>
        /// Select color
        /// </summary>
        public static DialogReturn SelectColor(string? title = null, string? initialColor = null)
        {
            using (var cd = new ColorDialog())
            {
                if (!string.IsNullOrEmpty(initialColor))
                {
                    try
                    {
                        cd.Color = System.Drawing.ColorTranslator.FromHtml(initialColor);
                    }
                    catch { }
                }

                if (cd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = System.Drawing.ColorTranslator.ToHtml(cd.Color),
                        Tag = cd.Color,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        /// <summary>
        /// Select font
        /// </summary>
        public static DialogReturn SelectFont(string? title = null, string? initialFont = null)
        {
            using (var fd = new FontDialog())
            {
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    return new DialogReturn
                    {
                        Result = BeepDialogResult.OK,
                        Value = fd.Font.Name,
                        Tag = fd.Font,
                        Submit = true
                    };
                }
            }

            return new DialogReturn { Result = BeepDialogResult.Cancel, Cancel = true };
        }

        #endregion

        #region Progress and Toast

        /// <summary>
        /// Show progress dialog
        /// </summary>
        public static int ShowProgress(string title, string? message = null)
        {
            int token = ++_progressTokenCounter;
            // TODO: Create and show progress form
            // For now, return token for tracking
            return token;
        }

        /// <summary>
        /// Update progress
        /// </summary>
        public static void UpdateProgress(int token, int percent, string? status = null)
        {
            // TODO: Update progress form
            if (_progressDialogs.ContainsKey(token))
            {
                // Update progress
            }
        }

        /// <summary>
        /// Close progress dialog
        /// </summary>
        public static void CloseProgress(int token)
        {
            if (_progressDialogs.ContainsKey(token))
            {
                _progressDialogs[token]?.Close();
                _progressDialogs.Remove(token);
            }
        }

        /// <summary>
        /// Show toast notification
        /// </summary>
        public static void ShowToast(string message, int durationMs = 3000, string? icon = null)
        {
            // TODO: Implement toast notification
            // For now, use a simple message box
            MsgBox("Notification", message);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert WinForms DialogResult to BeepDialogResult
        /// </summary>
        private static BeepDialogResult ConvertDialogResult(DialogResult result)
        {
            return result switch
            {
                DialogResult.OK => BeepDialogResult.OK,
                DialogResult.Cancel => BeepDialogResult.Cancel,
                DialogResult.Yes => BeepDialogResult.Yes,
                DialogResult.No => BeepDialogResult.No,
                DialogResult.Abort => BeepDialogResult.Abort,
                DialogResult.Retry => BeepDialogResult.Retry,
                DialogResult.Ignore => BeepDialogResult.Ignore,
                _ => BeepDialogResult.None
            };
        }

        /// <summary>
        /// Map BeepDialogIcon to DialogType
        /// </summary>
        private static DialogType MapIconToDialogType(BeepDialogIcon icon)
        {
            return icon switch
            {
                BeepDialogIcon.Information => DialogType.Information,
                BeepDialogIcon.Warning => DialogType.Warning,
                BeepDialogIcon.Error => DialogType.Error,
                BeepDialogIcon.Question => DialogType.Question,
                BeepDialogIcon.Success => DialogType.Information,
                _ => DialogType.None
            };
        }

        #endregion
    }
}
