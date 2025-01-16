using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class DialogHelper
    {
        public static IDMEEditor DMEEditor { get; set; }

        // InputBoxYesNo
        // InputBoxYesNo
        public static async Task<BeepDialogResult> InputBoxYesNoAsync(string title, string promptText)
        {
            var dialog = new BeepDialogBox();
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

            return await dialog.ShowDialogAsync(control, null, null, title);
        }

        // InputBox
        public static async Task<(BeepDialogResult Result, string Value)> InputBoxAsync(string title, string promptText, string initialValue)
        {
            var dialog = new BeepDialogBox();

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

            var result = await dialog.ShowDialogAsync(control, null, null, title);

            return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
        }

        // InputLargeBox
        public static async Task<(BeepDialogResult Result, string Value)> InputLargeBoxAsync(string title, string promptText, string initialValue)
        {
            var dialog = new BeepDialogBox();

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

            var result = await dialog.ShowDialogAsync(control, null, null, title);

            return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
        }

        // InputComboBox
        public static async Task<(BeepDialogResult Result, string Value)> InputComboBoxAsync(string title, string promptText, List<string> items, string selectedValue)
        {
            var dialog = new BeepDialogBox();

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

            var result = await dialog.ShowDialogAsync(control, null, null, title);

            return (result, result == BeepDialogResult.OK ? comboBox.Text : selectedValue);
        }

        // MessageBox
        public static async Task ShowMessageBoxAsync(string title, string message)
        {
            try
            {
                var dialog = new BeepDialogBox();
                await dialog.ShowInfoDialogAsync(message, null, title);
                DMEEditor?.AddLogMessage("Success", "Displayed MsgBox", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor?.AddLogMessage(ex.Message, "Failed to display MsgBox", DateTime.Now, -1, "Error", Errors.Failed);
            }
        }

        // DialogCombo
        public static async Task<(BeepDialogResult Result, string Value)> DialogComboAsync(string text, List<object> comboSource, string displayMember, string valueMember)
        {
            var dialog = new BeepDialogBox();

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

            var result = await dialog.ShowDialogAsync(control, null, null, text);

            return (result, result == BeepDialogResult.OK ? comboBox.SelectedValue?.ToString() : null);
        }
        // LoadFilesDialog
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

            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dialog.FileNames.ToList() : new List<string>();
        }

        // SelectFolderDialog
        public static string SelectFolderDialog()
        {
            using var folderBrowser = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            return folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK ? folderBrowser.SelectedPath : null;
        }

        // LoadFileDialog
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

            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dialog.FileName : null;
        }

        // SaveFileDialog
        public static string SaveFileDialog(string exts, string dir, string filter)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save File",
                DefaultExt = exts,
                Filter = filter,
                InitialDirectory = dir
            };

            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dialog.FileName : null;
        }
    }
}
