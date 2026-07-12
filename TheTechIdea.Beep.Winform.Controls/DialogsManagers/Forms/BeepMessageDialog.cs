using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public partial class BeepMessageDialog : BeepiFormPro
    {
        private System.Windows.Forms.Timer? _autoCloseTimer;
        private int _autoCloseRemainingMs;
        private string _autoCloseBaseLabel = string.Empty;

        public string ReturnValue { get; private set; } = "ok";

        public BeepMessageDialog()
        {
            InitializeComponent();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        public string Title
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value ?? string.Empty;
        }

        public string Message
        {
            get => _messageLabel.Text;
            set => _messageLabel.Text = value ?? string.Empty;
        }

        public string DetailsText { get; set; } = string.Empty;

        private void OkButton_Click(object? sender, EventArgs e)
        {
            ReturnValue = "ok";
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { _okButton.PerformClick(); return true; }
            if (keyData == Keys.Escape) { DialogResult = System.Windows.Forms.DialogResult.Cancel; Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e) { base.OnShown(e); _okButton.Focus(); }

        public void StartAutoClose(int timeoutMs)
        {
            if (timeoutMs <= 0) return;
            _autoCloseRemainingMs = timeoutMs; _autoCloseBaseLabel = _okButton.Text;
            _autoCloseTimer ??= new System.Windows.Forms.Timer { Interval = 1000 };
            _autoCloseTimer.Tick -= OnAutoCloseTick; _autoCloseTimer.Tick += OnAutoCloseTick;
            _autoCloseTimer.Start(); OnAutoCloseTick(this, EventArgs.Empty);
        }

        private void OnAutoCloseTick(object? sender, EventArgs e)
        {
            if (IsDisposed) { _autoCloseTimer?.Stop(); return; }
            _autoCloseRemainingMs -= 1000;
            if (_autoCloseRemainingMs <= 0) { _autoCloseTimer.Stop(); _okButton.PerformClick(); return; }
            _okButton.Text = string.IsNullOrEmpty(_autoCloseBaseLabel) ? $"OK ({Math.Max(1, _autoCloseRemainingMs / 1000)})" : $"{_autoCloseBaseLabel} ({Math.Max(1, _autoCloseRemainingMs / 1000)})";
        }
    }
}
