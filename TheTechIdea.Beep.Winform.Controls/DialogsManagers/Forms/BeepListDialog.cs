using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public partial class BeepListDialog : BeepiFormPro
    {
        private DialogButton[] _typedButtons = Array.Empty<DialogButton>();

        public string ReturnValue { get; private set; } = string.Empty;
        public SimpleItem? ReturnItem { get; private set; }
        public List<SimpleItem>? Items { get; set; }
        public DialogPreset PresetIntent { get; set; } = DialogPreset.None;

        public BeepListDialog()
        {
            InitializeComponent();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        public string Title { get => _titleLabel.Text; set => _titleLabel.Text = value ?? string.Empty; }
        public string Message { get => _messageLabel.Text; set => _messageLabel.Text = value ?? string.Empty; }

        public DialogButton[] TypedButtons
        {
            get => _typedButtons;
            set
            {
                _typedButtons = value ?? Array.Empty<DialogButton>();
                if (_typedButtons.Length >= 1) _okButton.Text = _typedButtons[0].Text ?? "OK";
                if (_typedButtons.Length >= 2) _cancelButton.Text = _typedButtons[1].Text ?? "Cancel";
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            ReturnValue = _comboBox.SelectedItem?.Text ?? string.Empty;
            ReturnItem = _comboBox.SelectedItem;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            ReturnValue = string.Empty;
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { _okButton.PerformClick(); return true; }
            if (keyData == Keys.Escape) { _cancelButton.PerformClick(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e) { base.OnShown(e); _comboBox.Focus(); }

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;
            _headerPanel.Theme = Theme; _bodyPanel.Theme = Theme; _buttonPanel.Theme = Theme;
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme; _messageLabel.Theme = Theme;
            _comboBox.Theme = Theme; _okButton.Theme = Theme; _cancelButton.Theme = Theme;
        }
    }
}
