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
            ComposeLayout();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        /// <summary>
        /// Places the controls into the shell's header / body / footer regions.
        /// </summary>
        /// <remarks>
        /// Kept out of <c>InitializeComponent</c> deliberately. Composition is the part that would
        /// otherwise want loops — one per action button — and control flow in a designer file breaks
        /// the VS designer's parser. Creation stays in the designer file; arrangement lives here.
        /// </remarks>
        private void ComposeLayout()
        {
            _headerPanel.Controls.Add(_dialogIcon, 0, 0);
            _headerPanel.Controls.Add(_titleLabel, 1, 0);

            _shell.SetHeader(_headerPanel);
            _shell.SetBody(_messageLabel);
            _shell.AddAction(_okButton);

            // The framework's default-action contract, now that BeepButton implements
            // IButtonControl. This replaces the hand-rolled ProcessCmdKey override every dialog in
            // this directory carried: Enter activates the accept button and Escape cancels, and the
            // button additionally gets its default-action cue and the semantics assistive technology
            // announces — none of which a key-routing override provides.
            _okButton.DialogResult = DialogResult.OK;
            AcceptButton = _okButton;
            CancelButton = _okButton;
        }

        public string Title
        {
            get => _titleLabel.Text;
            set => Helpers.DialogHelpers.SetTitle(this, _titleLabel, value);
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
            DialogResult = DialogResult.OK;
            Close();
        }

        // ProcessCmdKey override removed: AcceptButton handles Enter, and CancelButton handles
        // Escape — set in ComposeLayout now that BeepButton implements IButtonControl. A dialog
        // with a single OK action uses that button for both, which is the standard contract for a
        // message box on every platform.

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
