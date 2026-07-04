using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public partial class BeepQuestionDialog : BeepiFormPro
    {
        private bool _detailsExpanded;
        private DialogButton[] _typedButtons = Array.Empty<DialogButton>();

        public string ReturnValue { get; private set; } = string.Empty;
        public DialogPreset PresetIntent { get; set; } = DialogPreset.None;

        public BeepQuestionDialog()
        {
            InitializeComponent();
        }

        public string Title { get => _titleLabel.Text; set => _titleLabel.Text = value ?? string.Empty; }
        public string Message { get => _messageLabel.Text; set => _messageLabel.Text = value ?? string.Empty; }

        public string DetailsText { get => _detailsLabel.Text; set { _detailsLabel.Text = value ?? string.Empty; UpdateDetailsVisibility(); } }
        public bool DetailsExpanded { get => _detailsExpanded; set { _detailsExpanded = value; UpdateDetailsVisibility(); } }

        public DialogButton[] TypedButtons
        {
            get => _typedButtons;
            set
            {
                _typedButtons = value ?? Array.Empty<DialogButton>();
                if (_typedButtons.Length >= 1) { _yesButton.Text = _typedButtons[0].Text ?? "Yes"; _yesButton.Visible = true; }
                if (_typedButtons.Length >= 2) { _noButton.Text = _typedButtons[1].Text ?? "No"; _noButton.Visible = true; }
            }
        }

        public Dictionary<TheTechIdea.Beep.Vis.Modules.BeepDialogButtons, string>? CustomButtonLabels { get; set; }

        private void YesButton_Click(object? sender, EventArgs e) { ReturnValue = _typedButtons.Length > 0 ? (_typedButtons[0].Id ?? "yes") : "yes"; DialogResult = System.Windows.Forms.DialogResult.Yes; Close(); }
        private void NoButton_Click(object? sender, EventArgs e) { ReturnValue = _typedButtons.Length > 1 ? (_typedButtons[1].Id ?? "no") : "no"; DialogResult = System.Windows.Forms.DialogResult.No; Close(); }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { _yesButton.PerformClick(); return true; }
            if (keyData == Keys.Escape) { _noButton.PerformClick(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;
            _headerPanel.Theme = Theme; _bodyPanel.Theme = Theme; _buttonPanel.Theme = Theme;
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme; _messageLabel.Theme = Theme;
            _detailsToggleButton.Theme = Theme; _detailsLabel.Theme = Theme;
            _yesButton.Theme = Theme; _noButton.Theme = Theme;
        }

        private void UpdateDetailsVisibility()
        {
            var hasText = !string.IsNullOrEmpty(_detailsLabel.Text);
            _detailsToggleButton.Visible = hasText;
            _detailsLabel.Visible = hasText && _detailsExpanded;
            _detailsToggleButton.Text = _detailsExpanded ? "Hide details" : "Show details";
        }

        private void DetailsToggleButton_Click(object? sender, EventArgs e) { _detailsExpanded = !_detailsExpanded; UpdateDetailsVisibility(); }
    }
}
