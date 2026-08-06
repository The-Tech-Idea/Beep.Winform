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
            ComposeLayout();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        /// <summary>
        /// Places the controls into the shell's header / body / footer regions.
        /// </summary>
        /// <remarks>
        /// Kept out of <c>InitializeComponent</c>: arranging a variable number of actions wants a
        /// loop, and control flow in a designer file breaks the VS designer's parser.
        /// </remarks>
        private void ComposeLayout()
        {



            // The framework's dialog contract, available now that BeepButton implements
            // IButtonControl. Yes accepts, No cancels — so Enter and Escape are routed by the form
            // itself, and both buttons additionally carry the semantics assistive technology reads.
            _yesButton.DialogResult = DialogResult.Yes;
            _noButton.DialogResult = DialogResult.No;
            AcceptButton = _yesButton;
            CancelButton = _noButton;

        }

        public string Title { get => _titleLabel.Text; set => Helpers.DialogHelpers.SetTitle(this, _titleLabel, value); }
        public string Message { get => _messageLabel.Text; set => Helpers.DialogHelpers.SetMessage(this, _messageLabel, value); }

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

        private void YesButton_Click(object? sender, EventArgs e) { ReturnValue = _typedButtons.Length > 0 ? (_typedButtons[0].Id ?? "yes") : "yes"; DialogResult = DialogResult.Yes; Close(); }
        private void NoButton_Click(object? sender, EventArgs e) { ReturnValue = _typedButtons.Length > 1 ? (_typedButtons[1].Id ?? "no") : "no"; DialogResult = DialogResult.No; Close(); }

        // ProcessCmdKey override removed: AcceptButton and CancelButton, set in ComposeLayout,
        // route Enter and Escape through the framework.

        public override void ApplyTheme()
        {
            if (_titleLabel == null) return;
            // _buttonPanel is gone: the shell's footer row owns action placement now.
            _dialogIcon.Theme = Theme;
            _titleLabel.Theme = Theme; _messageLabel.Theme = Theme;
            _detailsToggleButton.Theme = Theme; _detailsLabel.Theme = Theme;
            _yesButton.Theme = Theme; _noButton.Theme = Theme;

        }

        /// <summary>
        /// Opens with focus on the safe action, never the one that commits.
        /// </summary>
        /// <remarks>
        /// This dialog was one of two that decided nothing about initial focus, and it is the one that
        /// matters: it is the confirm dialog, so it is the one most likely to carry a destructive
        /// action. A confirm dialog that opens with the committing button focused turns a stray Enter
        /// into data loss — which is the risk `Example_images/dialog2.png` is drawn to slow down, with
        /// "No, keep it." sitting left of "Yes, Delete!".
        /// <para>
        /// No is the safe answer here, and it is already this dialog's <c>CancelButton</c>.
        /// </para>
        /// </remarks>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Helpers.DialogHelpers.DescribeActions(this);

            if (_noButton.Visible && _noButton.Enabled) _noButton.Focus();
            else if (_yesButton.Visible) _yesButton.Focus();
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
