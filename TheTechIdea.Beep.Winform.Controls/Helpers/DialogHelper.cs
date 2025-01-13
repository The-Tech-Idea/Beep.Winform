using System.ComponentModel;
using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using DialogResult = TheTechIdea.Beep.Vis.Modules.DialogResult;


namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class DialogHelper
    {
        public static IDMEEditor DMEEditor { get; set; }
        public static DialogResult InputBoxYesNo(string title, string promptText)
        {
            // Create an instance of the BeepDialogBox
            BeepDialogBox dialog = new BeepDialogBox();

            // Create a user control to hold the content
            UserControl control = new UserControl
            {
                ClientSize = new Size(375, 60) // Set the size of the user control
            };

            // Create a label for the prompt text
            Label label = new Label
            {
                Text = promptText,
                Dock = DockStyle.Fill, // Fill the entire user control
                TextAlign = ContentAlignment.MiddleCenter, // Center the text
                AutoSize = false // Prevent the label from resizing
            };

            // Add the label to the user control
            control.Controls.Add(label);

            // Set up the dialog
            dialog.TitleText = title; // Set the title
            dialog.ShowDialog(control,
                submit: () => dialog.DialogResult = DialogResult.Yes,
                cancel: () => dialog.DialogResult = DialogResult.No,
                Title: title);

            // Show the dialog and return the result
            return dialog.DialogResult;
        }
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            // Create the label and textbox
            Label label = new Label
            {
                Text = promptText,
                Width = 200,
                Height = 14,
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.None
            };

            TextBox textBox = new TextBox
            {
                Text = value,
                Width = 200,
                Height = 20,
                Anchor = AnchorStyles.None
            };

            // Create a container control
            UserControl control = new UserControl
            {
                ClientSize = new Size(375, 60) // Set control size
            };

            // Calculate center positions
            int labelX = (control.ClientSize.Width - label.Width) / 2;
            int labelY = (control.ClientSize.Height / 2) - label.Height - 5; // Above the textbox
            int textBoxX = (control.ClientSize.Width - textBox.Width) / 2;
            int textBoxY = (control.ClientSize.Height / 2);

            // Position the label and textbox
            label.SetBounds(labelX, labelY, label.Width, label.Height);
            textBox.SetBounds(textBoxX, textBoxY, textBox.Width, textBox.Height);

            // Add controls to the container
            control.Controls.Add(label);
            control.Controls.Add(textBox);

            // Create and configure the BeepDialog
            BeepDialogBox dialog = new BeepDialogBox
            {
                TitleText = title // Set dialog title
            };

            // Add the user control to the dialog and show it
            dialog.ShowDialog(control,
                submit: () => dialog.DialogResult = DialogResult.OK,
                cancel: () => dialog.DialogResult = DialogResult.Cancel,
                Title: title);

            // Update the value with the user's input
            if (dialog.DialogResult == DialogResult.OK)
            {
                value = textBox.Text;
            }

            return dialog.DialogResult;
        }
        public static DialogResult InputLargeBox(string title, string promptText, ref string value)
        {
            // Create the label and textbox
            Label label = new Label
            {
                Text = promptText,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = 380, // Slightly smaller than the control width
                Height = 20 // Fixed height for the label
            };

            TextBox textBox = new TextBox
            {
                Text = value,
                Multiline = true, // Enable multiline input
                ScrollBars = ScrollBars.Vertical, // Add a vertical scrollbar
                Width = 380, // Slightly smaller than the control width
                Height = 130 // Reserve space for larger input
            };

            // Create a user control to host the label and textbox
            UserControl control = new UserControl
            {
                ClientSize = new Size(400, 200) // Larger client size for multiline input
            };

            // Position the label and textbox with padding
            int labelX = 10; // Padding from the left
            int labelY = 10; // Padding from the top
            int textBoxX = 10; // Same padding as label
            int textBoxY = labelY + label.Height + 10; // Below the label with spacing

            // Set bounds for the label and textbox
            label.SetBounds(labelX, labelY, label.Width, label.Height);
            textBox.SetBounds(textBoxX, textBoxY, textBox.Width, textBox.Height);

            // Add controls to the container
            control.Controls.Add(label);
            control.Controls.Add(textBox);

            // Create and configure the BeepDialog
            BeepDialogBox dialog = new BeepDialogBox
            {
                TitleText = title // Set dialog title
            };

            // Add the user control to the dialog and show it
            dialog.ShowDialog(control,
                submit: () => dialog.DialogResult = DialogResult.OK,
                cancel: () => dialog.DialogResult = DialogResult.Cancel,
                Title: title);

            // Update the value if the user clicks OK
            if (dialog.DialogResult == DialogResult.OK)
            {
                value = textBox.Text;
            }

            return dialog.DialogResult;
        }
        public static void MsgBox(string title, string promptText)
        {
            try
            {
                // Create a new instance of BeepDialogBox
                BeepDialogBox dialog = new BeepDialogBox();

                // Use ShowInfoDialog from BeepDialogBox
                dialog.ShowInfoDialog(
                    message: promptText,
                    okAction: null, // No action needed for OK button
                    Title: title
                );

                // Log success
                DMEEditor.AddLogMessage("Success", "Displayed MsgBox", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string message = "Could not display MsgBox";
                DMEEditor.AddLogMessage(ex.Message, message, DateTime.Now, -1, message, Errors.Failed);
            }
        }
        public static DialogResult InputComboBox(string title, string promptText, List<string> itvalues, ref string value)
        {
            try
            {
                // Create a BeepDialogBox instance
                BeepDialogBox dialog = new BeepDialogBox();

                // Create a user control to host the label and ComboBox
                UserControl control = new UserControl
                {
                    ClientSize = new Size(400, 100) // Set the size of the control
                };

                // Create and configure the label
                Label label = new Label
                {
                    Text = promptText,
                    Width = control.ClientSize.Width - 20,
                    Height = 20,
                    Location = new Point(10, 10), // Padding from the top and left
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Create and configure the ComboBox
                ComboBox comboBox = new ComboBox
                {
                    Text = value, // Set the initial value
                    Width = control.ClientSize.Width - 20,
                    Height = 30,
                    Location = new Point(10, label.Bottom + 10) // Below the label
                };

                // Populate the ComboBox with items
                comboBox.Items.AddRange(itvalues.ToArray());

                // Add the label and ComboBox to the user control
                control.Controls.Add(label);
                control.Controls.Add(comboBox);

                // Show the dialog with the user control
                dialog.ShowDialog(control,
                    submit: () => dialog.DialogResult = DialogResult.OK,
                    cancel: () => dialog.DialogResult = DialogResult.Cancel,
                    Title: title
                );

                // Retrieve the selected value if the dialog result is OK
                if (dialog.DialogResult == DialogResult.OK)
                {
                    value = comboBox.Text;
                }

                return dialog.DialogResult;
            }
            catch (Exception ex)
            {
                string message = "Could not display InputComboBox";
                DMEEditor.AddLogMessage(ex.Message, message, DateTime.Now, -1, message, Errors.Failed);
                return DialogResult.Cancel;
            }
        }
        public static string DialogCombo(string text, List<object> comboSource, string displayMember, string valueMember)
        {
            // Create the label and ComboBox
            Label label = new Label
            {
                Text = text,
                Width = 460,
                Height = 20,
                Location = new Point(20, 20),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            ComboBox comboBox = new ComboBox
            {
                DataSource = comboSource,
                DisplayMember = displayMember,
                ValueMember = valueMember,
                Width = 460,
                Height = 30,
                Location = new Point(20, label.Bottom + 10)
            };

            // Create a user control to host the label and ComboBox
            UserControl control = new UserControl
            {
                ClientSize = new Size(500, 200)
            };

            // Add the label and ComboBox to the user control
            control.Controls.Add(label);
            control.Controls.Add(comboBox);

            // Create the dialog instance
            BeepDialogBox dialog = new BeepDialogBox
            {
                TitleText = text // Set the dialog title
            };

            // Show the dialog with the custom control
            dialog.ShowDialog(control,
                submit: () => dialog.DialogResult = DialogResult.OK,
                cancel: () => dialog.DialogResult = DialogResult.Cancel,
                Title: text
            );

            // Return the selected value or null if the dialog is canceled
            return dialog.DialogResult == DialogResult.OK ? comboBox.SelectedValue?.ToString() : null;
        }
        public static List<string> LoadFilesDialog(string exts, string dir, string filter)
        {
            OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog()
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = dir


                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };

            openFileDialog1.Multiselect = true;
            DialogResult result = MapDialogResult(openFileDialog1.ShowDialog());

            return openFileDialog1.FileNames.ToList();
        }
        public static string SelectFolderDialog()
        {
            string folderPath = string.Empty;
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            string specialFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            folderBrowser.SelectedPath = specialFolderPath;
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = folderBrowser.SelectedPath;
                // ...
            }
            return folderPath;
        }
        public static string LoadFileDialog(string exts, string dir, string filter)
        {
            OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog()
            {
                Title = "Browse Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = dir


                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };
            openFileDialog1.InitialDirectory = dir;

            DialogResult result = MapDialogResult(openFileDialog1.ShowDialog());

            return openFileDialog1.FileName;
        }
        public static string SaveFileDialog(string exts, string dir, string filter)
        {
            SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog()
            {
                Title = "Save File",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = exts,
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = dir


                //ReadOnlyChecked = true,
                //ShowReadOnly = true
            };


            DialogResult result = MapDialogResult(saveFileDialog1.ShowDialog());

            return saveFileDialog1.FileName;
        }
        public static string ShowSpecialDirectoriesComboBox()
        {
            // Create the ComboBox
            ComboBox comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList, // Ensure user can only select items
                Width = 300,
                Height = 30
            };

            // Get all special directories and add them to the ComboBox
            foreach (var directory in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                comboBox.Items.Add(directory);
            }

            // Set the default selected index
            comboBox.SelectedIndex = 0;

            // Create a label for the dialog
            Label label = new Label
            {
                Text = "Select a special directory:",
                AutoSize = false,
                Width = 300,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(10, 10)
            };

            // Position the ComboBox below the label
            comboBox.Location = new Point(10, label.Bottom + 10);

            // Create a user control to host the label and ComboBox
            UserControl control = new UserControl
            {
                ClientSize = new Size(350, 100)
            };

            // Add the label and ComboBox to the user control
            control.Controls.Add(label);
            control.Controls.Add(comboBox);

            // Create and configure the dialog
            BeepDialogBox dialog = new BeepDialogBox
            {
                TitleText = "Special Directories"
            };

            // Show the dialog with the custom control
            dialog.ShowDialog(control,
                submit: () => dialog.DialogResult = DialogResult.OK,
                cancel: () => dialog.DialogResult = DialogResult.Cancel,
                Title: "Special Directories"
            );

            // Return the selected directory path if confirmed
            if (dialog.DialogResult == DialogResult.OK)
            {
                var selectedDirectory = (Environment.SpecialFolder)comboBox.SelectedItem;
                return Environment.GetFolderPath(selectedDirectory);
            }

            // If canceled, return null
            return null;
        }
        public static string SelectFile(string filter)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set the filter for the file extension and default file extension 
                openFileDialog.Filter = filter;

                // Display the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the path of the selected file
                    string selectedFilePath = openFileDialog.FileName;
                    return selectedFilePath;
                }
            }

            return null; // or return an empty string, depending on how you want to handle the cancellation
        }
        private static DialogResult MapDialogResult(System.Windows.Forms.DialogResult dialogResult)
        {
            DialogResult retval = DialogResult.None;
            switch (dialogResult)
            {

                case System.Windows.Forms.DialogResult.None:
                    retval = DialogResult.None;
                    break;
                case System.Windows.Forms.DialogResult.OK:
                    retval = DialogResult.OK;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                    retval = DialogResult.Cancel;
                    break;
                case System.Windows.Forms.DialogResult.Abort:
                    retval = DialogResult.Abort;
                    break;
                case System.Windows.Forms.DialogResult.Retry:
                    retval = DialogResult.Retry;
                    break;
                case System.Windows.Forms.DialogResult.Ignore:
                    retval = DialogResult.Ignore;
                    break;
                case System.Windows.Forms.DialogResult.Yes:
                    retval = DialogResult.Yes;
                    break;
                case System.Windows.Forms.DialogResult.No:
                    retval = DialogResult.No;
                    break;
                default:
                    retval = DialogResult.None;
                    break;
            }
            return retval;
        }

    }
}
