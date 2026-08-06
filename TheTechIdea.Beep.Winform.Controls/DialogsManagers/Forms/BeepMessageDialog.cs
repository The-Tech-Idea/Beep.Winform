using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
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
        /// Wires behaviour. Layout is declared in the designer file.
        /// </summary>
        /// <remarks>
        /// Composition used to happen here — <c>SetIcon</c>, <c>SetTitle</c>, <c>AddContent</c>,
        /// <c>AddAction</c> — and that is exactly why this form rendered blank in the Visual Studio
        /// designer: the design surface shows what <c>InitializeComponent</c> parents, and it parented
        /// only the shell. The grid and every control in it are now declared in the designer file, and
        /// what is left here is what a designer file cannot express: which button accepts and which
        /// cancels.
        /// </remarks>
        private void ComposeLayout()
        {
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
            set => Helpers.DialogHelpers.SetMessage(this, _messageLabel, value);
        }

        public string DetailsText { get; set; } = string.Empty;

        /// <summary>
        /// The dialog's actions, as the typed model every other form already accepts.
        /// </summary>
        /// <remarks>
        /// This dialog had no such property, so a caller who set <c>DialogConfig.TypedButtons</c> got
        /// them honoured by the custom and question dialogs and silently ignored here — the OK button
        /// kept its designer caption whatever the config said. "DialogButton is the single
        /// representation" is not true of a folder where one form does not read it.
        /// <para>
        /// One action only: this is the message dialog, and a message has one way out. A config asking
        /// for more than one is answered by a question dialog, which is what <c>CreateDialog</c>
        /// selects when the buttons say so.
        /// </para>
        /// </remarks>
        public DialogButton[] TypedButtons
        {
            get => _typedButtons;
            set
            {
                _typedButtons = value ?? Array.Empty<DialogButton>();
                if (_typedButtons.Length == 0) return;

                var primary = _typedButtons[0];
                if (!string.IsNullOrWhiteSpace(primary.Text)) _okButton.Text = primary.Text;
                _okButton.Enabled = primary.Enabled;
                _okButton.Visible = primary.Visible;
            }
        }

        private DialogButton[] _typedButtons = Array.Empty<DialogButton>();

        /// <summary>
        /// Propagates this form's theme to its controls, matching the other five dialogs.
        /// </summary>
        /// <remarks>
        /// This is about the form's own <c>Theme</c> rather than the global one. A change made through
        /// <c>BeepThemesManager</c> needs nothing from here — every control below derives from
        /// <c>BaseControl</c>, which subscribes to <c>ThemeChanged</c> and re-applies itself. This
        /// dialog previously had no override at all, so a form-level theme reached nothing.
        /// </remarks>
        public override void ApplyTheme()
        {
            if (_titleLabel == null) return;

            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme; _messageLabel.Theme = Theme;
            _okButton.Theme = Theme;
        }

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

        protected override void OnShown(EventArgs e) { base.OnShown(e);
            Helpers.DialogHelpers.DescribeActions(this); _okButton.Focus(); }

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
