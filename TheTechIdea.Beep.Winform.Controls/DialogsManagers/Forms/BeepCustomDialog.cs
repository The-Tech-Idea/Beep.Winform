using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public partial class BeepCustomDialog : BeepiFormPro
    {
        private Control? _hostedControl;
        private DialogButton[] _typedButtons = Array.Empty<DialogButton>();

        public string ReturnValue { get; private set; } = string.Empty;
        public Control? CustomControl => _hostedControl;
        public DialogPreset PresetIntent { get; set; } = DialogPreset.None;

        public BeepCustomDialog()
        {
            InitializeComponent();
            ComposeLayout();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        /// <summary>Places the controls into the shell's header and footer regions.</summary>
        private void ComposeLayout()
        {
            _headerPanel.Controls.Add(_dialogIcon, 0, 0);
            _headerPanel.Controls.Add(_titleLabel, 1, 0);

            _shell.SetHeader(_headerPanel);
            _shell.AddAction(_cancelButton);
            _shell.AddAction(_okButton);

            _okButton.DialogResult = DialogResult.OK;
            _cancelButton.DialogResult = DialogResult.Cancel;
            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        public string Title { get => _titleLabel.Text; set => Helpers.DialogHelpers.SetTitle(this, _titleLabel, value); }

        public void SetCustomControl(Control control)
        {
            if (_hostedControl != null && !_hostedControl.IsDisposed) _hostedControl.Dispose();
            _hostedControl = control;

            // The hosted control *is* the body. SetBody docks it to fill the shell's middle row, so
            // the manual Location/Size assignment and the SizeChanged handler that re-applied Size
            // on every resize — a hand-rolled Dock.Fill, and one that leaked a handler subscription
            // on every call — are both gone.
            if (control != null) _shell.SetBody(control);
        }

        public DialogButton[] TypedButtons
        {
            get => _typedButtons;
            set
            {
                _typedButtons = value ?? Array.Empty<DialogButton>();
                _okButton.Visible = _typedButtons.Length >= 1;
                _cancelButton.Visible = _typedButtons.Length >= 2;
                if (_typedButtons.Length >= 1) _okButton.Text = _typedButtons[0].Text ?? "OK";
                if (_typedButtons.Length >= 2) _cancelButton.Text = _typedButtons[1].Text ?? "Cancel";
            }
        }

        private void OkButton_Click(object? sender, EventArgs e) { ReturnValue = _typedButtons.Length > 0 ? (_typedButtons[0].Id ?? "ok") : "ok"; DialogResult = DialogResult.OK; Close(); }
        private void CancelButton_Click(object? sender, EventArgs e) { ReturnValue = string.Empty; DialogResult = DialogResult.Cancel; Close(); }

        // Enter and Escape are routed by AcceptButton / CancelButton, set in ComposeLayout.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { _okButton.PerformClick(); return true; }
            if (keyData == Keys.Escape) { _cancelButton.PerformClick(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme;
            _okButton.Theme = Theme; _cancelButton.Theme = Theme;
        }
    }
}
