using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public static class DialogHelper
    {
        public static IDMEEditor DMEEditor { get; set; }

        #region Configuration and Styling
        public static class DialogConfig
        {
            public static int DefaultWidth { get; set; } = 400;
            public static int DefaultMargin { get; set; } = 10;
            public static Size DefaultInputSize { get; set; } = new Size(375, 100);
            public static Size DefaultLargeSize { get; set; } = new Size(400, 240);
            public static Size DefaultComboSize { get; set; } = new Size(500, 140);
        }
        #endregion

        #region Standard Dialogs

        public static BeepDialogResult InputBoxYesNo(string title, string message)
        {
            using (var dialog = new BeepDialogModal
            {
                Title = title,
                Message = message,
                DialogButtons = BeepDialogButtons.YesNo,
                DialogType = DialogType.Question
            })
            {
                return ShowDialogSafely(dialog);
            }
        }

        public static (BeepDialogResult Result, string Value) InputBox(string title, string message, string initialValue = "")
        {
            using (var dialog = new BeepDialogModal
            {
                Title = title,
                Message = message,
                DialogButtons = BeepDialogButtons.OkCancel,
                DialogType = DialogType.GetInputString,
                ReturnValue = initialValue
            })
            {
                var result = ShowDialogSafely(dialog);
                return (result, dialog.ReturnValue);
            }
        }

        public static (BeepDialogResult Result, string Value) InputLargeBox(string title, string message, string initialValue = "")
        {
            using (var dialog = new BeepDialogModal
            {
                Title = title,
                Message = message,
                DialogButtons = BeepDialogButtons.OkCancel,
                DialogType = DialogType.GetInputString,
                ReturnValue = initialValue,
                Size = DialogConfig.DefaultLargeSize
            })
            {
                var result = ShowDialogSafely(dialog);
                return (result, dialog.ReturnValue);
            }
        }

        public static (BeepDialogResult Result, string Value) InputComboBox(string title, string message, IEnumerable<string> items, string selectedValue = "")
        {
            using (var dialog = new BeepDialogModal
            {
                Title = title,
                Message = message,
                DialogButtons = BeepDialogButtons.OkCancel,
                DialogType = DialogType.GetInputFromList,
                Items = items.Select(item => new SimpleItem { Value = item, Display = item }).ToList(),
                ReturnValue = selectedValue
            })
            {
                var result = ShowDialogSafely(dialog);
                return (result, dialog.ReturnValue);
            }
        }

        public static void ShowMessageBox(string title, string message)
        {
            using (var dialog = new BeepDialogModal
            {
                Title = title,
                Message = message,
                DialogButtons = BeepDialogButtons.OkCancel,
                DialogType = DialogType.Information
            })
            {
                ShowDialogSafely(dialog);
            }
        }

        #endregion

        #region ComboBox Dialog

        public static (BeepDialogResult Result, string Value) DialogCombo<T>(string title, IEnumerable<T> comboSource,
            string displayMember, string valueMember)
        {
            ArgumentNullException.ThrowIfNull(comboSource, nameof(comboSource));

            using (var dialog = new BeepDialogModal
            {
                Title = title,
                DialogButtons = BeepDialogButtons.OkCancel,
                DialogType = DialogType.GetInputFromList,
                Items = comboSource.Select(item => new SimpleItem
                {
                    Value = item.GetType().GetProperty(valueMember)?.GetValue(item)?.ToString(),
                    Display = item.GetType().GetProperty(displayMember)?.GetValue(item)?.ToString()
                }).ToList()
            })
            {
                var result = ShowDialogSafely(dialog);
                return (result, dialog.ReturnValue);
            }
        }

        #endregion

        #region File Dialogs (Fixed)

        public static IReadOnlyList<string> LoadFilesDialog(string exts = "txt", string dir = null, string filter = "All files (*.*)|*.*")
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                Multiselect = true,
                InitialDirectory = dir ?? Environment.CurrentDirectory
            };
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames.ToList().AsReadOnly() : Array.Empty<string>();
        }

        public static string LoadFileDialog(string exts = "txt", string dir = null, string filter = "All files (*.*)|*.*")
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                InitialDirectory = dir ?? Environment.CurrentDirectory
            };
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }

        public static string SaveFileDialog(string exts = "txt", string dir = null, string filter = "All files (*.*)|*.*")
        {
            using var dialog = new SaveFileDialog
            {
                Title = "Save File",
                DefaultExt = exts,
                Filter = filter,
                InitialDirectory = dir ?? Environment.CurrentDirectory
            };
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }

        public static string SelectFolderDialog()
        {
            using var folderBrowser = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            return folderBrowser.ShowDialog() == DialogResult.OK ? folderBrowser.SelectedPath : null;
        }

        #endregion

        #region Helper Method to Ensure ShowDialog() Works Properly

        private static BeepDialogResult ShowDialogSafely(BeepDialogModal dialog)
        {
            BeepDialogResult result = BeepDialogResult.Cancel;

            if (Application.OpenForms.Count > 0)
            {
                var owner = Application.OpenForms[0]; // Get the first open form as owner
                owner.Invoke((MethodInvoker)(() =>
                {
                    result = dialog.ShowDialog(owner) == DialogResult.OK ? BeepDialogResult.OK : BeepDialogResult.Cancel;
                }));
            }
            else
            {
                result = dialog.ShowDialog() == DialogResult.OK ? BeepDialogResult.OK : BeepDialogResult.Cancel;
            }

            return result;
        }

        #endregion
    }
}
