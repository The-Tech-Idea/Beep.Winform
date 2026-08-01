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
            ComposeLayout();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        /// <summary>Places the controls into the shell's header / body / footer regions.</summary>
        private void ComposeLayout()
        {
            _headerPanel.Controls.Add(_dialogIcon, 0, 0);
            _headerPanel.Controls.Add(_titleLabel, 1, 0);

            _bodyPanel.Controls.Add(_messageLabel, 0, 0);
            _bodyPanel.Controls.Add(_comboBox, 0, 1);

            _shell.SetHeader(_headerPanel);
            _shell.SetBody(_bodyPanel);
            _shell.AddAction(_cancelButton);
            _shell.AddAction(_okButton);

            _okButton.DialogResult = DialogResult.OK;
            _cancelButton.DialogResult = DialogResult.Cancel;
            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        public string Title { get => _titleLabel.Text; set => Helpers.DialogHelpers.SetTitle(this, _titleLabel, value); }
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
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            ReturnValue = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ProcessCmdKey override removed: AcceptButton and CancelButton route Enter and Escape.

        protected override void OnShown(EventArgs e) { base.OnShown(e); _comboBox.Focus(); }

        public override void ApplyTheme()
        {
            if (_titleLabel == null) return;
            // The header/body/footer panels are layout, not themed surfaces — the shell's rows carry
            // no chrome of their own, so only the Beep controls inside them take the theme.
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme; _messageLabel.Theme = Theme;
            _comboBox.Theme = Theme; _okButton.Theme = Theme; _cancelButton.Theme = Theme;
        }
    }
}
