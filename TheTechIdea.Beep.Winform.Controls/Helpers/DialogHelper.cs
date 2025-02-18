using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;

namespace TheTechIdea.Beep.Winform.Controls
{
    public static class DialogHelper
    {
        public static IDMEEditor DMEEditor { get; set; }

        // Synchronous InputBoxYesNo using BeepDialogForm.
        public static BeepDialogResult InputBoxYesNo(string title, string promptText)
        {
            using (var form = new BeepDialogForm())
            {
                var label = new Label
                {
                    Text = promptText,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false
                };
                var control = new UserControl
                {
                    ClientSize = new Size(375, 60)
                };
                control.Controls.Add(label);
                return form.ShowDialog(control, submit: null, cancel: null, title: title);
            }
        }

        // Synchronous InputBox using BeepDialogForm.
        public static (BeepDialogResult Result, string Value) InputBox(string title, string promptText, string initialValue)
        {
            using (var form = new BeepDialogForm())
            {
                var label = new Label
                {
                    Text = promptText,
                    Width = 200,
                    Height = 14,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                var textBox = new TextBox
                {
                    Text = initialValue,
                    Width = 200,
                    Height = 20
                };
                var control = new UserControl
                {
                    ClientSize = new Size(375, 60)
                };
                label.Location = new Point((control.Width - label.Width) / 2, 10);
                textBox.Location = new Point((control.Width - textBox.Width) / 2, 30);
                control.Controls.Add(label);
                control.Controls.Add(textBox);
                var result = form.ShowDialog(control, submit: null, cancel: null, title: title);
                return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
            }
        }

        // Synchronous InputLargeBox using BeepDialogForm.
        public static (BeepDialogResult Result, string Value) InputLargeBox(string title, string promptText, string initialValue)
        {
            using (var form = new BeepDialogForm())
            {
                var label = new Label
                {
                    Text = promptText,
                    Width = 380,
                    Height = 20,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                var textBox = new TextBox
                {
                    Text = initialValue,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = 380,
                    Height = 130
                };
                var control = new UserControl
                {
                    ClientSize = new Size(400, 200)
                };
                label.Location = new Point(10, 10);
                textBox.Location = new Point(10, 40);
                control.Controls.Add(label);
                control.Controls.Add(textBox);
                var result = form.ShowDialog(control, submit: null, cancel: null, title: title);
                return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
            }
        }

        // Synchronous InputComboBox using BeepDialogForm.
        public static (BeepDialogResult Result, string Value) InputComboBox(string title, string promptText, List<string> items, string selectedValue)
        {
            using (var form = new BeepDialogForm())
            {
                var label = new Label
                {
                    Text = promptText,
                    Width = 380,
                    Height = 20,
                    Location = new Point(10, 10)
                };
                var comboBox = new ComboBox
                {
                    Text = selectedValue,
                    Width = 380,
                    Height = 30,
                    Location = new Point(10, 40)
                };
                comboBox.Items.AddRange(items.ToArray());
                var control = new UserControl
                {
                    ClientSize = new Size(400, 100)
                };
                control.Controls.Add(label);
                control.Controls.Add(comboBox);
                var result = form.ShowDialog(control, submit: null, cancel: null, title: title);
                return (result, result == BeepDialogResult.OK ? comboBox.Text : selectedValue);
            }
        }

        // Synchronous ShowMessageBox using BeepDialogForm.
        public static void ShowMessageBox(string title, string message)
        {
            using (var form = new BeepDialogForm())
            {
                var label = new Label
                {
                    Text = message,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                var control = new UserControl
                {
                    ClientSize = new Size(400, 200)
                };
                control.Controls.Add(label);
                form.ShowDialog(control, submit: null, cancel: null, title: title);
                DMEEditor?.AddLogMessage("Success", "Displayed MsgBox", DateTime.Now, 0, null, Errors.Ok);
            }
        }

        // Synchronous DialogCombo using BeepDialogForm.
        public static (BeepDialogResult Result, string Value) DialogCombo(string text, List<object> comboSource, string displayMember, string valueMember)
        {
            using (var form = new BeepDialogForm())
            {
                var label = new Label
                {
                    Text = text,
                    Width = 460,
                    Height = 20,
                    Location = new Point(20, 20)
                };
                var comboBox = new ComboBox
                {
                    DataSource = comboSource,
                    DisplayMember = displayMember,
                    ValueMember = valueMember,
                    Width = 460,
                    Height = 30,
                    Location = new Point(20, 50)
                };
                var control = new UserControl
                {
                    ClientSize = new Size(500, 200)
                };
                control.Controls.Add(label);
                control.Controls.Add(comboBox);
                var result = form.ShowDialog(control, submit: null, cancel: null, title: text);
                return (result, result == BeepDialogResult.OK ? comboBox.SelectedValue?.ToString() : null);
            }
        }

        // The file and folder dialogs remain synchronous.
        public static List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                Multiselect = true,
                InitialDirectory = dir
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames.ToList() : new List<string>();
        }

        public static string SelectFolderDialog()
        {
            using (var folderBrowser = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            })
            {
                return folderBrowser.ShowDialog() == DialogResult.OK ? folderBrowser.SelectedPath : null;
            }
        }

        public static string LoadFileDialog(string exts, string dir, string filter)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                InitialDirectory = dir
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }

        public static string SaveFileDialog(string exts, string dir, string filter)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save File",
                DefaultExt = exts,
                Filter = filter,
                InitialDirectory = dir
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }
    }
}
