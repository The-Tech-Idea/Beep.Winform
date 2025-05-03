using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class DialogHelper
    {
      

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
            // Validate inputs
            if (items == null || !items.Any())
            {
                MiscFunctions.SendLog("InputComboBox called with empty or null items collection");
                return (BeepDialogResult.Cancel, null);
            }

            using (var dialog = new BeepDialogModal())
            {
                // Create the items list first
                dialog.Items = items.Select(item => new SimpleItem
                {
                    Value = item,
                    DisplayField = item,
                    Text = item  // Important for ToString() representation
                }).ToList();

                // Basic dialog setup - DialogType should be set AFTER Items so SetInputListVisible works with items
                dialog.Title = title ?? "Select an Item";
                dialog.Message = message ?? "Please select an item from the list:";
                dialog.DialogButtons = BeepDialogButtons.OkCancel;

                // Set initial selection if provided
                if (!string.IsNullOrEmpty(selectedValue) && dialog.Items.Any(i => i.Value == selectedValue))
                {
                    dialog.ReturnValue = selectedValue;
                }
                else if (dialog.Items.Any())
                {
                    // Default to first item
                    dialog.ReturnValue = dialog.Items[0].Value?.ToString();
                }

                // Set dialog type LAST to trigger setup with populated items
                dialog.DialogType = DialogType.GetInputFromList;

                // Show dialog and return result
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
                    DisplayField = item.GetType().GetProperty(displayMember)?.GetValue(item)?.ToString()
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

            try
            {
                // Find an appropriate owner form
                Form owner = FindAppropriateOwner();

                // Prepare the dialog with good defaults
                dialog.TopMost = true;
                dialog.StartPosition = owner != null ?
                    FormStartPosition.CenterParent : FormStartPosition.CenterScreen;

                if (owner != null)
                {
                    if (owner.InvokeRequired)
                    {
                        // Use Invoke with a proper delegate that returns a value
                        result = (BeepDialogResult)owner.Invoke(new Func<BeepDialogResult>(() =>
                        {
                            var ret = dialog.ShowDialog(owner);
                            return MapDialogResult(ret);
                        }));
                    }
                    else
                    {
                        // Already on UI thread
                        var ret = dialog.ShowDialog(owner);
                        result = MapDialogResult(ret);
                    }
                }
                else
                {
                    // No suitable owner found, show without an owner
                    var ret = dialog.ShowDialog();
                    result = MapDialogResult(ret);
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Error showing dialog: {ex.Message}");
            }

            return result;
        }
        private static Form FindAppropriateOwner()
        {
            // Check if we have any open forms
            if (Application.OpenForms.Count == 0)
                return null;

            // Try to find the active form first
            Form activeForm = Form.ActiveForm;
            if (activeForm != null && !activeForm.IsDisposed && activeForm.Visible)
                return activeForm;

            // If no active form, find the first visible top-level form
            foreach (Form form in Application.OpenForms)
            {
                if (!form.IsDisposed && form.Visible && form.TopLevel)
                    return form;
            }

            // If all else fails, use the first form (with validation)
            Form firstForm = Application.OpenForms[0];
            if (!firstForm.IsDisposed)
                return firstForm;

            // No suitable form found
            return null;
        }

        // Safe mapping helper
        private static BeepDialogResult MapDialogResult(DialogResult ret)
        {
            return _resultMapReverse.TryGetValue(ret, out var mappedResult)
                ? mappedResult
                : BeepDialogResult.None;
        }


        #endregion
        private static readonly Dictionary<BeepDialogResult, DialogResult> _resultMap = new Dictionary<BeepDialogResult, DialogResult>
        {
            { BeepDialogResult.OK, DialogResult.OK },
            { BeepDialogResult.Cancel, DialogResult.Cancel },
            { BeepDialogResult.Yes, DialogResult.Yes },
            { BeepDialogResult.No, DialogResult.No },
            { BeepDialogResult.Abort, DialogResult.Abort },
            { BeepDialogResult.Retry, DialogResult.Retry },
            { BeepDialogResult.Ignore, DialogResult.Ignore },
            { BeepDialogResult.None, DialogResult.None },
            { BeepDialogResult.Continue, DialogResult.Continue }

        };
        private static readonly Dictionary<DialogResult, BeepDialogResult> _resultMapReverse = _resultMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }
}
