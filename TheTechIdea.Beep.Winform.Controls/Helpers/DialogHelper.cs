using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public static class DialogHelper
    {
        private const int DEFAULT_WIDTH = 400;
        private const int DEFAULT_MARGIN = 10;

        // Changed to object type to avoid needing IDMEEditor definition
        // Replace with your actual editor interface if available
        public static IDMEEditor DMEEditor { get; set; }

        private static (BeepDialogForm Form, UserControl Control) CreateBaseDialog(string title, Size size)
        {
            var form = new BeepDialogForm();
            var control = new UserControl { ClientSize = size };
            return (form, control);
        }

        private static Label CreateLabel(string text, int width, ContentAlignment alignment = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = text,
                Width = width,
                Height = 20,
                TextAlign = alignment
            };
        }

        public static BeepDialogResult InputBoxYesNo(string title, string promptText)
        {
            var (form, control) = CreateBaseDialog(title, new Size(375, 60));
            using (form)
            {
                var label = CreateLabel(promptText, control.Width - (DEFAULT_MARGIN * 2), ContentAlignment.MiddleCenter);
                label.Dock = DockStyle.Fill;
                control.Controls.Add(label);
                return form.ShowDialog(control, null, null, title);
            }
        }

        public static (BeepDialogResult Result, string Value) InputBox(string title, string promptText, string initialValue)
        {
            var (form, control) = CreateBaseDialog(title, new Size(375, 60));
            using (form)
            {
                var label = CreateLabel(promptText, 200);
                label.Location = new Point((control.Width - 200) / 2, DEFAULT_MARGIN);
                var textBox = new TextBox
                {
                    Text = initialValue,
                    Width = 200,
                    Height = 20,
                    Location = new Point((control.Width - 200) / 2, 30)
                };
                control.Controls.AddRange(new Control[] { label, textBox });
                var result = form.ShowDialog(control, null, null, title);
                return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
            }
        }

        public static (BeepDialogResult Result, string Value) InputLargeBox(string title, string promptText, string initialValue)
        {
            var (form, control) = CreateBaseDialog(title, new Size(DEFAULT_WIDTH, 200));
            using (form)
            {
                var label = CreateLabel(promptText, control.Width - (DEFAULT_MARGIN * 2));
                label.Location = new Point(DEFAULT_MARGIN, DEFAULT_MARGIN);
                var textBox = new TextBox
                {
                    Text = initialValue,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = control.Width - (DEFAULT_MARGIN * 2),
                    Height = 130,
                    Location = new Point(DEFAULT_MARGIN, 40)
                };
                control.Controls.AddRange(new Control[] { label, textBox });
                var result = form.ShowDialog(control, null, null, title);
                return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
            }
        }

        public static (BeepDialogResult Result, string Value) InputComboBox(string title, string promptText, List<string> items, string selectedValue)
        {
            var (form, control) = CreateBaseDialog(title, new Size(DEFAULT_WIDTH, 100));
            using (form)
            {
                var label = CreateLabel(promptText, control.Width - (DEFAULT_MARGIN * 2));
                label.Location = new Point(DEFAULT_MARGIN, DEFAULT_MARGIN);
                var comboBox = new ComboBox
                {
                    Text = selectedValue,
                    Width = control.Width - (DEFAULT_MARGIN * 2),
                    Height = 30,
                    Location = new Point(DEFAULT_MARGIN, 40),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                comboBox.Items.AddRange(items.ToArray());
                control.Controls.AddRange(new Control[] { label, comboBox });
                var result = form.ShowDialog(control, null, null, title);
                return (result, result == BeepDialogResult.OK ? comboBox.Text : selectedValue);
            }
        }

        public static void ShowMessageBox(string title, string message)
        {
            var (form, control) = CreateBaseDialog(title, new Size(DEFAULT_WIDTH, 200));
            using (form)
            {
                var label = CreateLabel(message, control.Width - (DEFAULT_MARGIN * 2), ContentAlignment.MiddleCenter);
                label.Dock = DockStyle.Fill;
                label.AutoSize = true;
                control.Controls.Add(label);
                form.ShowDialog(control, null, null, title);

                // Modified to remove Errors dependency - replace with your actual logging mechanism
                if (DMEEditor != null && DMEEditor is IDMEEditor editor)
                {
                    editor.AddLogMessage("Success", "Displayed MsgBox", DateTime.Now, 0, null, 0); // Replace 0 with your success code
                }
            }
        }

        public static (BeepDialogResult Result, string Value) DialogCombo(string text, List<object> comboSource, string displayMember, string valueMember)
        {
            var (form, control) = CreateBaseDialog(text, new Size(500, 200));
            using (form)
            {
                var label = CreateLabel(text, control.Width - (DEFAULT_MARGIN * 2));
                label.Location = new Point(DEFAULT_MARGIN * 2, DEFAULT_MARGIN * 2);
                var comboBox = new ComboBox
                {
                    DataSource = comboSource,
                    DisplayMember = displayMember,
                    ValueMember = valueMember,
                    Width = control.Width - (DEFAULT_MARGIN * 2),
                    Height = 30,
                    Location = new Point(DEFAULT_MARGIN * 2, 50),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                control.Controls.AddRange(new Control[] { label, comboBox });
                var result = form.ShowDialog(control, null, null, text);
                return (result, result == BeepDialogResult.OK ? comboBox.SelectedValue?.ToString() : null);
            }
        }

        public static List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts ?? "txt",
                Filter = filter ?? "All files (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = dir ?? Environment.CurrentDirectory
            })
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames.ToList() : new List<string>();
            }
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
            using (var dialog = new OpenFileDialog
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts ?? "txt",
                Filter = filter ?? "All files (*.*)|*.*",
                InitialDirectory = dir ?? Environment.CurrentDirectory
            })
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        public static string SaveFileDialog(string exts, string dir, string filter)
        {
            using (var dialog = new SaveFileDialog
            {
                Title = "Save File",
                DefaultExt = exts ?? "txt",
                Filter = filter ?? "All files (*.*)|*.*",
                InitialDirectory = dir ?? Environment.CurrentDirectory
            })
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }
    }

    // Added temporary interface definition - replace with your actual IDMEEditor
  
}