using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class ConflictPolicyDialog : Form
    {
        private readonly ComboBox _policyComboBox;
        private readonly CheckBox _emptyOnlyCheckBox;
        private readonly TextBox _descriptionTextBox;

        public ConnectionConflictPolicy SelectedPolicy { get; private set; } = ConnectionConflictPolicy.Replace;
        public bool ImportWhenEmptyOnly => _emptyOnlyCheckBox.Checked;

        public ConflictPolicyDialog(string title, bool showImportMode)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Size = new Size(520, showImportMode ? 320 : 270);

            var label = new Label
            {
                Text = "Conflict policy:",
                AutoSize = true,
                Location = new Point(14, 16)
            };

            _policyComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(14, 40),
                Width = 475
            };
            _policyComboBox.Items.AddRange(new object[]
            {
                ConnectionConflictPolicy.Replace,
                ConnectionConflictPolicy.Rename,
                ConnectionConflictPolicy.Skip,
                ConnectionConflictPolicy.MergeByGuid
            });
            _policyComboBox.SelectedIndex = 0;
            _policyComboBox.SelectedIndexChanged += (_, _) => UpdateDescription();

            _descriptionTextBox = new TextBox
            {
                Location = new Point(14, 74),
                Width = 475,
                Height = 112,
                ReadOnly = true,
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            _emptyOnlyCheckBox = new CheckBox
            {
                Text = "Import only when target profile is empty",
                AutoSize = true,
                Visible = showImportMode,
                Location = new Point(14, 194)
            };

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(333, showImportMode ? 235 : 202),
                Width = 75
            };
            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(414, showImportMode ? 235 : 202),
                Width = 75
            };

            Controls.Add(label);
            Controls.Add(_policyComboBox);
            Controls.Add(_descriptionTextBox);
            Controls.Add(_emptyOnlyCheckBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
            UpdateDescription();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK && _policyComboBox.SelectedItem is ConnectionConflictPolicy selected)
            {
                SelectedPolicy = selected;
            }

            base.OnFormClosing(e);
        }

        private void UpdateDescription()
        {
            var selected = _policyComboBox.SelectedItem is ConnectionConflictPolicy policy
                ? policy
                : ConnectionConflictPolicy.Replace;

            _descriptionTextBox.Text = selected switch
            {
                ConnectionConflictPolicy.Replace => "Replace: overwrite target item when a name or identity conflict is found.",
                ConnectionConflictPolicy.Rename => "Rename: keep both items by importing conflicting records with a generated name.",
                ConnectionConflictPolicy.Skip => "Skip: ignore conflicting records and only import/promote non-conflicting items.",
                ConnectionConflictPolicy.MergeByGuid => "MergeByGuid: update target only when GuidID matches; otherwise skip conflicting records.",
                _ => string.Empty
            };
        }
    }
}
