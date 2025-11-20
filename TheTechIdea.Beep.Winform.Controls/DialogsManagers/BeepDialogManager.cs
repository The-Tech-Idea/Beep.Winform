using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    public class BeepDialogManager : IDialogManager
    {
        private readonly Dictionary<int, Form> _progressDialogs = new Dictionary<int, Form>();
        private int _progressTokenCounter = 0;

        public void CloseProgress(int token)
        {
            if (_progressDialogs.ContainsKey(token))
            {
                _progressDialogs[token]?.Close();
                _progressDialogs.Remove(token);
            }
        }

        public DialogReturn Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon)
        {
            return Confirm(title, message, buttons, icon, null);
        }

        public DialogReturn Confirm(string title, string message, BeepDialogButtons[] buttons, BeepDialogIcon icon, BeepDialogButtons? defaultButton)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = message;
                dialog.DialogType = MapIconToDialogType(icon);
                
                // Map buttons
                var buttonList = buttons.ToList();
                if (buttonList.Count == 2)
                {
                    if (buttonList.Contains(BeepDialogButtons.Yes) && buttonList.Contains(BeepDialogButtons.No))
                        dialog.DialogButtons = BeepDialogButtons.YesNo;
                    else if (buttonList.Contains(BeepDialogButtons.Ok) && buttonList.Contains(BeepDialogButtons.Cancel))
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

        public DialogReturn ConfirmOverwrite(string filePath)
        {
            return Confirm("Confirm Overwrite",
                $"The file '{System.IO.Path.GetFileName(filePath)}' already exists. Do you want to overwrite it?",
                new[] { BeepDialogButtons.Yes, BeepDialogButtons.No },
                BeepDialogIcon.Question);
        }

        public DialogReturn DialogCombo(string text, List<SimpleItem> comboSource, string DisplyMember, string ValueMember)
        {
            return InputComboBox("Select", text, comboSource);
        }

        public DialogReturn InputBox(string title, string promptText)
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

        public DialogReturn InputBoxYesNo(string title, string promptText)
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

        public DialogReturn InputCheckList(string title, string promptText, List<SimpleItem> items)
        {
            return new DialogReturn
            {
                Result = BeepDialogResult.Cancel,
                Items = new List<SimpleItem>(),
                Cancel = true
            };
        }

        public DialogReturn InputComboBox(string title, string promptText, List<SimpleItem> itvalues)
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

        public DialogReturn InputComboBox(string title, string promptText, List<string> values)
        {
            var items = values.Select(v => new SimpleItem { Text = v, Value = v }).ToList();
            return InputComboBox(title, promptText, items);
        }

        public DialogReturn InputDateTime(string title, string promptText, DateTime? min = null, DateTime? max = null, DateTime? @default = null)
        {
            return InputBox(title, promptText);
        }

        public DialogReturn InputDouble(string title, string promptText, double? min = null, double? max = null, double? @default = null, int? decimals = null)
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

        public DialogReturn InputInt(string title, string promptText, int? min = null, int? max = null, int? @default = null)
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

        public DialogReturn InputLargeBox(string title, string promptText)
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

        public DialogReturn InputListBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            return InputComboBox(title, promptText, itvalues);
        }

        public DialogReturn InputPassword(string title, string promptText, bool masked = true)
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

        public DialogReturn InputRadioGroupBox(string title, string promptText, List<SimpleItem> itvalues)
        {
            return InputComboBox(title, promptText, itvalues);
        }

        public DialogReturn InputTimeSpan(string title, string promptText, TimeSpan? min = null, TimeSpan? max = null, TimeSpan? @default = null)
        {
            return InputBox(title, promptText);
        }

        public DialogReturn LoadFileDialog(string exts, string dir, string filter)
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

        public DialogReturn LoadFileDialog(string exts, string dir, string filter, string initialFileName)
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

        public List<string> LoadFilesDialog(string exts, string dir, string filter)
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

        public void MsgBox(string title, string promptText)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = promptText;
                dialog.DialogType = DialogType.Information;
                dialog.ShowDialog();
            }
        }

        public DialogReturn MultiSelect(string title, string promptText, List<SimpleItem> items)
        {
            return new DialogReturn
            {
                Result = BeepDialogResult.Cancel,
                Items = new List<SimpleItem>(),
                Cancel = true
            };
        }

        public DialogReturn SaveFileDialog(string exts, string dir, string filter)
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

        public DialogReturn SaveFileDialog(string exts, string dir, string filter, string defaultFileName)
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

        public DialogReturn SelectColor(string? title = null, string? initialColor = null)
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

        public DialogReturn SelectFile(List<SimpleItem> files, string filter)
        {
            return InputComboBox("Select File", "Choose a file:", files);
        }

        public DialogReturn SelectFolderDialog()
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

        public DialogReturn SelectFolderDialog(string title, string initialDir, bool allowCreate)
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

        public DialogReturn SelectFont(string? title = null, string? initialFont = null)
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

        public DialogReturn SelectSpecialDirectoriesComboBox()
        {
            var dirs = GetSpecialDirectories();
            return InputComboBox("Select Directory", "Choose a special directory:", dirs);
        }

        public DialogReturn SelectSpecialDirectoriesListBox()
        {
            var dirs = GetSpecialDirectories();
            return InputComboBox("Select Directory", "Choose a special directory:", dirs);
        }

        public DialogReturn SelectSpecialDirectoriesRadioGroupBox()
        {
            var dirs = GetSpecialDirectories();
            return InputComboBox("Select Directory", "Choose a special directory:", dirs);
        }

        public DialogReturn ShowAlert(string title, string message, string icon)
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

        public void ShowException(string title, Exception ex)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title ?? "Error";
                dialog.Message = ex.Message;
                dialog.DialogType = DialogType.Error;
                dialog.ShowDialog();
            }
        }

        public void ShowMessege(string title, string message, string icon)
        {
            using (var dialog = new BeepDialogModal())
            {
                dialog.Title = title;
                dialog.Message = message;
                dialog.DialogType = DialogType.Information;
                dialog.ShowDialog();
            }
        }

        public int ShowProgress(string title, string? message = null)
        {
            int token = ++_progressTokenCounter;
            var dlg = new BeepProgressDialog();
            dlg.Text = title;
            dlg.SetMessage(message ?? title);
            dlg.Cancellable = false;
            _progressDialogs[token] = dlg;

            // If a host form is configured, show owned to host, otherwise show as modal
            if (DialogManager.HostForm != null)
            {
                dlg.Show(DialogManager.HostForm);
                DialogManager.ApplyDialogAnimation(dlg, TheTechIdea.Beep.Vis.Modules.DialogShowAnimation.FadeIn);
            }
            else
            {
                dlg.Show();
            }

            return token;
        }

        public void ShowToast(string message, int durationMs = 3000, string? icon = null)
        {
            MsgBox("Notification", message);
        }

        public void UpdateProgress(int token, int percent, string? status = null)
        {
            if (_progressDialogs.ContainsKey(token))
            {
                if (_progressDialogs[token] is BeepProgressDialog p)
                {
                    p.SetProgress(percent, status);
                }
            }
        }

        private List<SimpleItem> GetSpecialDirectories()
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

        private BeepDialogResult ConvertDialogResult(DialogResult result)
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

        private DialogType MapIconToDialogType(BeepDialogIcon icon)
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
    }
}
