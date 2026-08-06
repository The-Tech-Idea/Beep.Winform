using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public partial class BeepInputDialog : BeepiFormPro
    {
        private bool _inputValidationPassed = true;
        private DialogButton[] _typedButtons = Array.Empty<DialogButton>();

        public string ReturnValue { get; private set; } = string.Empty;
        public DialogPreset PresetIntent { get; set; } = DialogPreset.None;

        public BeepInputDialog()
        {
            InitializeComponent();
            ComposeLayout();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        public string Title { get => _titleLabel.Text; set => Helpers.DialogHelpers.SetTitle(this, _titleLabel, value); }
        public string Message { get => _messageLabel.Text; set => Helpers.DialogHelpers.SetMessage(this, _messageLabel, value); }

        /// <summary>Places the controls into the shell's header / body / footer regions.</summary>
        private void ComposeLayout()
        {



            _okButton.DialogResult = DialogResult.OK;
            _cancelButton.DialogResult = DialogResult.Cancel;
            AcceptButton = _okButton;
            CancelButton = _cancelButton;

        }

        public Func<string, string?>? InputValidator { get; set; }
        // Skill § 1: multiline input height = single-line * 2 + spacing (skill § default-size composition).
        public bool InputBoxMultiline
        {
            get => _inputBox.Multiline;
            set
            {
                _inputBox.Multiline = value;
                if (value)
                {
                    int single = BeepLayoutMetrics.FieldStandard.Height.ScaleValue(this);
                    _inputBox.Height = single * 2 + BeepLayoutMetrics.SmallGap.ScaleValue(this);
                }
            }
        }
        public bool InputBoxUsePasswordChar { get => _inputBox.UseSystemPasswordChar; set => _inputBox.UseSystemPasswordChar = value; }
        public string InputDefaultValue { set => _inputBox.Text = value; }

        public DialogButton[] TypedButtons
        {
            get => _typedButtons;
            set
            {
                _typedButtons = value ?? Array.Empty<DialogButton>();
                if (_typedButtons.Length >= 1) { _okButton.Text = _typedButtons[0].Text ?? "OK"; }
                if (_typedButtons.Length >= 2) { _cancelButton.Text = _typedButtons[1].Text ?? "Cancel"; }
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            ReturnValue = _inputBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            ReturnValue = string.Empty;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void InputBox_TextChanged(object? sender, EventArgs e)
        {
            if (InputValidator == null) { _inputValidationPassed = true; _validationLabel.Visible = false; _okButton.Enabled = true; return; }
            var err = InputValidator(_inputBox.Text ?? "");
            _inputValidationPassed = string.IsNullOrEmpty(err);
            _validationLabel.Text = err ?? "";
            _validationLabel.Visible = !string.IsNullOrEmpty(err);
            _okButton.Enabled = _inputValidationPassed;
        }

        /// <summary>
        /// Routes Enter and Escape through the form's button properties, not the fields behind them.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>CancelButton</c> is the single lever a caller has for turning Escape off —
        /// <c>BeepDialogManager</c> clears it when a config sets <c>CloseOnEscape = false</c>.
        /// Pressing <c>_cancelButton</c> directly read straight past that decision.
        /// </para>
        /// <para>
        /// Enter is left alone while a multi-line field has focus, so it inserts a newline instead of
        /// submitting the dialog. Committing on Enter in a box the caller asked to be multi-line makes
        /// the second line unreachable from the keyboard.
        /// </para>
        /// </remarks>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool typingInMultiline = _inputBox.Multiline && ReferenceEquals(ActiveControl, _inputBox);

            if (keyData == Keys.Enter && !typingInMultiline && _okButton.Enabled)
            {
                _okButton.PerformClick();
                return true;
            }

            if (keyData == Keys.Escape)
            {
                CancelButton?.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Helpers.DialogHelpers.DescribeActions(this); _inputBox.Focus(); _inputBox.SelectAll();
        }

        public override void ApplyTheme()
        {
            if (_titleLabel == null) return;
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme; _messageLabel.Theme = Theme;
            _inputBox.Theme = Theme; _validationLabel.Theme = Theme;
            _okButton.Theme = Theme; _cancelButton.Theme = Theme;

        }

        private void _dialogIcon_Click(object sender, EventArgs e)
        {

        }
    }
}
