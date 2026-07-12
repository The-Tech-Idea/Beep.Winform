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
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        public string Title { get => _titleLabel.Text; set => _titleLabel.Text = value ?? string.Empty; }

        public void SetCustomControl(Control control)
        {
            if (_hostedControl != null && !_hostedControl.IsDisposed) { _bodyPanel.Controls.Remove(_hostedControl); _hostedControl.Dispose(); }
            _hostedControl = control;
            if (control != null)
            {
                control.Location = new System.Drawing.Point(0, 0);
                control.Size = _bodyPanel.ClientSize;
                _bodyPanel.Controls.Add(control);
                _bodyPanel.SizeChanged += (s, e) => { if (_hostedControl != null && !_hostedControl.IsDisposed) _hostedControl.Size = _bodyPanel.ClientSize; };
            }
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

        private void OkButton_Click(object? sender, EventArgs e) { ReturnValue = _typedButtons.Length > 0 ? (_typedButtons[0].Id ?? "ok") : "ok"; DialogResult = System.Windows.Forms.DialogResult.OK; Close(); }
        private void CancelButton_Click(object? sender, EventArgs e) { ReturnValue = string.Empty; DialogResult = System.Windows.Forms.DialogResult.Cancel; Close(); }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { _okButton.PerformClick(); return true; }
            if (keyData == Keys.Escape) { _cancelButton.PerformClick(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;
            _headerPanel.Theme = Theme; _bodyPanel.Theme = Theme; _buttonPanel.Theme = Theme;
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme;
            _okButton.Theme = Theme; _cancelButton.Theme = Theme;
        }
    }
}
