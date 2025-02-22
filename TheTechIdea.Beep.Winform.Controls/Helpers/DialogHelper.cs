
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

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
            public static Size DefaultInputSize { get; set; } = new Size(375, 60);
            public static Size DefaultLargeSize { get; set; } = new Size(400, 200);
            public static Size DefaultComboSize { get; set; } = new Size(500, 200);
        }

        public class DialogStyle
        {
            public Color BackgroundColor { get; set; } = SystemColors.Control;
            public Font Font { get; set; } = SystemFonts.DefaultFont;
            public Color TextColor { get; set; } = SystemColors.ControlText;
        }

        private static void ApplyStyle(Control control, DialogStyle style)
        {
            if (style == null) return;
            control.BackColor = style.BackgroundColor;
            control.Font = style.Font;
            control.ForeColor = style.TextColor;
            foreach (Control child in control.Controls)
                ApplyStyle(child, style);
        }
        #endregion

        #region Base Methods
        private static (BeepDialogForm Form, UserControl Control) CreateBaseDialog(string title, Size size, DialogStyle style = null)
        {
            ArgumentNullException.ThrowIfNull(title, nameof(title));
            var form = new BeepDialogForm();
            var control = new UserControl { ClientSize = size };
            ApplyStyle(control, style);
            return (form, control);
        }

        private static Label CreateLabel(string text, int width, ContentAlignment alignment = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = text ?? string.Empty,
                Width = Math.Max(width, 0),
                Height = 20,
                TextAlign = alignment
            };
        }
        #endregion

        #region Existing Dialogs (Enhanced)
        public static BeepDialogResult InputBoxYesNo(string title, string promptText, DialogStyle style = null)
        {
            var (form, control) = CreateBaseDialog(title, DialogConfig.DefaultInputSize, style);
            using (form)
            using (control)
            {
                var label = CreateLabel(promptText, control.Width - (DialogConfig.DefaultMargin * 2), ContentAlignment.MiddleCenter);
                label.Dock = DockStyle.Fill;
                control.Controls.Add(label);
                return form.ShowDialog(control, null, null, title);
            }
        }

        public static (BeepDialogResult Result, string Value) InputBox(string title, string promptText,
            string initialValue = "", Func<string, bool> validator = null, DialogStyle style = null)
        {
            var (form, control) = CreateBaseDialog(title, DialogConfig.DefaultInputSize, style);
            using (form)
            using (control)
            {
                var label = CreateLabel(promptText, 200);
                label.Location = new Point((control.Width - 200) / 2, DialogConfig.DefaultMargin);
                var textBox = new TextBox
                {
                    Text = initialValue ?? string.Empty,
                    Width = 200,
                    Height = 20,
                    Location = new Point((control.Width - 200) / 2, 30)
                };
                control.Controls.AddRange([label, textBox]);
                var result = form.ShowDialog(control, null, null, title);
                if (validator != null && result == BeepDialogResult.OK && !validator(textBox.Text))
                    return (BeepDialogResult.Cancel, initialValue);
                return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
            }
        }

        public static (BeepDialogResult Result, string Value) InputLargeBox(string title, string promptText,
            string initialValue = "", DialogStyle style = null)
        {
            var (form, control) = CreateBaseDialog(title, DialogConfig.DefaultLargeSize, style);
            using (form)
            using (control)
            {
                var label = CreateLabel(promptText, control.Width - (DialogConfig.DefaultMargin * 2));
                label.Location = new Point(DialogConfig.DefaultMargin, DialogConfig.DefaultMargin);
                var textBox = new TextBox
                {
                    Text = initialValue ?? string.Empty,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Width = control.Width - (DialogConfig.DefaultMargin * 2),
                    Height = 130,
                    Location = new Point(DialogConfig.DefaultMargin, 40)
                };
                control.Controls.AddRange([label, textBox]);
                var result = form.ShowDialog(control, null, null, title);
                return (result, result == BeepDialogResult.OK ? textBox.Text : initialValue);
            }
        }

        public static (BeepDialogResult Result, string Value) InputComboBox(string title, string promptText,
            IEnumerable<string> items, string selectedValue = "", DialogStyle style = null)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            var (form, control) = CreateBaseDialog(title, new Size(DialogConfig.DefaultWidth, 100), style);
            using (form)
            using (control)
            {
                var label = CreateLabel(promptText, control.Width - (DialogConfig.DefaultMargin * 2));
                label.Location = new Point(DialogConfig.DefaultMargin, DialogConfig.DefaultMargin);
                var comboBox = new ComboBox
                {
                    Text = selectedValue ?? string.Empty,
                    Width = control.Width - (DialogConfig.DefaultMargin * 2),
                    Height = 30,
                    Location = new Point(DialogConfig.DefaultMargin, 40),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                comboBox.Items.AddRange(items.ToArray());
                control.Controls.AddRange([label, comboBox]);
                var result = form.ShowDialog(control, null, null, title);
                return (result, result == BeepDialogResult.OK ? comboBox.Text : selectedValue);
            }
        }

        public static void ShowMessageBox(string title, string message, DialogStyle style = null)
        {
            var (form, control) = CreateBaseDialog(title, DialogConfig.DefaultLargeSize, style);
            using (form)
            using (control)
            {
                var label = CreateLabel(message, control.Width - (DialogConfig.DefaultMargin * 2), ContentAlignment.MiddleCenter);
                label.Dock = DockStyle.Fill;
                label.AutoSize = true;
                control.Controls.Add(label);
                form.ShowDialog(control, null, null, title);
                DMEEditor?.AddLogMessage("Success", "Displayed MsgBox", DateTime.Now, 0, null, 0);
            }
        }

        public static (BeepDialogResult Result, string Value) DialogCombo<T>(string title, IEnumerable<T> comboSource,
            string displayMember, string valueMember, DialogStyle style = null)
        {
            ArgumentNullException.ThrowIfNull(comboSource, nameof(comboSource));
            var (form, control) = CreateBaseDialog(title, DialogConfig.DefaultComboSize, style);
            using (form)
            using (control)
            {
                var label = CreateLabel(title, control.Width - (DialogConfig.DefaultMargin * 2));
                label.Location = new Point(DialogConfig.DefaultMargin * 2, DialogConfig.DefaultMargin * 2);
                var comboBox = new ComboBox
                {
                    DataSource = comboSource.ToList(),
                    DisplayMember = displayMember,
                    ValueMember = valueMember,
                    Width = control.Width - (DialogConfig.DefaultMargin * 2),
                    Height = 30,
                    Location = new Point(DialogConfig.DefaultMargin * 2, 50),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                control.Controls.AddRange([label, comboBox]);
                var result = form.ShowDialog(control, null, null, title);
                return (result, result == BeepDialogResult.OK ? comboBox.SelectedValue?.ToString() : null);
            }
        }

        public static IReadOnlyList<string> LoadFilesDialog(string exts = "txt", string dir = null,
            string filter = "All files (*.*)|*.*")
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
            return dialog.ShowDialog() == DialogResult.OK
                ? dialog.FileNames.ToList().AsReadOnly()
                : Array.Empty<string>();
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

        public static string LoadFileDialog(string exts = "txt", string dir = null,
            string filter = "All files (*.*)|*.*")
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

        public static string SaveFileDialog(string exts = "txt", string dir = null,
            string filter = "All files (*.*)|*.*")
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
        #endregion

        #region New Dialogs
        public static async Task ShowProgressDialog(string title, string initialMessage,
            Func<IProgress<(int Progress, string Message)>, Task> operation, DialogStyle style = null)
        {
            ArgumentNullException.ThrowIfNull(title, nameof(title));
            ArgumentNullException.ThrowIfNull(initialMessage, nameof(initialMessage));
            ArgumentNullException.ThrowIfNull(operation, nameof(operation));

            var (form, control) = CreateBaseDialog(title, DialogConfig.DefaultLargeSize, style);
            try
            {
                using (form)
                using (control)
                {
                    var label = CreateLabel(initialMessage, control.Width - (DialogConfig.DefaultMargin * 2));
                    label.Location = new Point(DialogConfig.DefaultMargin, DialogConfig.DefaultMargin);

                    var progressBar = new ProgressBar
                    {
                        Width = control.Width - (DialogConfig.DefaultMargin * 2),
                        Location = new Point(DialogConfig.DefaultMargin, 40),
                        Maximum = 100,
                        Minimum = 0
                    };

                    control.Controls.AddRange([label, progressBar]);
                    form.FormClosing += (s, e) => e.Cancel = true;

                    var progress = new Progress<(int Progress, string Message)>(update =>
                    {
                        if (control.IsDisposed) return;
                        control.Invoke((Action)(() =>
                        {
                            progressBar.Value = Math.Clamp(update.Progress, 0, 100);
                            label.Text = update.Message;
                        }));
                    });

                    var tcs = new TaskCompletionSource<bool>();
                    form.Shown += async (s, e) =>
                    {
                        try
                        {
                            await operation(progress);
                            tcs.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            DMEEditor?.AddLogMessage("Error", $"Progress operation failed: {ex.Message}",
                                DateTime.Now, -1, ex.StackTrace, ConfigUtil.Errors.Failed);
                            tcs.SetException(ex);
                        }
                        finally
                        {
                            form.Close();
                        }
                    };

                    form.ShowDialog(control, null, null, title);
                    await tcs.Task;
                }
            }
            catch (Exception ex)
            {
                DMEEditor?.AddLogMessage("Error", $"Progress dialog failed: {ex.Message}",
                    DateTime.Now, -1, ex.StackTrace, ConfigUtil.Errors.Failed);
                throw;
            }
        }

        public static (BeepDialogResult Result, Dictionary<string, string> Values) MultiInputDialog(
             string title,
             Dictionary<string, (string DefaultValue, Func<string, bool> Validator)> fields,
             DialogStyle style = null)
        {
            ArgumentNullException.ThrowIfNull(title, nameof(title));
            ArgumentNullException.ThrowIfNull(fields, nameof(fields));
            if (!fields.Any())
                throw new ArgumentException("Fields collection cannot be empty", nameof(fields));

            int height = 60 + (fields.Count * 30) + 40; // Extra space for OK button
            var (form, control) = CreateBaseDialog(title, new Size(DialogConfig.DefaultWidth, height), style);

            try
            {
                using (form)
                using (control)
                {
                    var textBoxes = new Dictionary<string, TextBox>();
                    int y = DialogConfig.DefaultMargin;

                    foreach (var field in fields)
                    {
                        var label = CreateLabel(field.Key, 150);
                        label.Location = new Point(DialogConfig.DefaultMargin, y);

                        var textBox = new TextBox
                        {
                            Text = field.Value.DefaultValue ?? string.Empty,
                            Width = 200,
                            Location = new Point(160, y)
                        };
                        textBoxes[field.Key] = textBox;

                        control.Controls.AddRange([label, textBox]);
                        y += 30;
                    }

                    // Add explicit OK button instead of relying on AcceptButton
                    var okButton = new Button
                    {
                        Text = "OK",
                        Width = 80,
                        Location = new Point(control.Width / 2 - 40, y + DialogConfig.DefaultMargin)
                    };

                    BeepDialogResult result = BeepDialogResult.Cancel;
                    okButton.Click += (s, e) =>
                    {
                        if (fields.All(f => f.Value.Validator == null || f.Value.Validator(textBoxes[f.Key].Text)))
                        {
                            result = BeepDialogResult.OK;
                            form.Close();
                        }
                        else
                        {
                            MessageBox.Show("Please correct invalid inputs", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };
                    control.Controls.Add(okButton);

                    // Set the form's AcceptButton to trigger OK button click with Enter key
                    form.AcceptButton = okButton;

                    result = form.ShowDialog(control, null, null, title);
                    var values = result == BeepDialogResult.OK
                        ? textBoxes.ToDictionary(k => k.Key, v => v.Value.Text)
                        : fields.ToDictionary(k => k.Key, v => v.Value.DefaultValue);

                    return (result, values);
                }
            }
            catch (Exception ex)
            {
                DMEEditor?.AddLogMessage("Error", $"Multi-input dialog failed: {ex.Message}",
                    DateTime.Now, -1, ex.StackTrace, ConfigUtil.Errors.Failed);
                throw;
            }
        }
        #endregion
    }

   
}