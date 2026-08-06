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

            _okButton.DialogResult = DialogResult.OK;
            _cancelButton.DialogResult = DialogResult.Cancel;
            AcceptButton = _okButton;
            CancelButton = _cancelButton;

        }

        public string Title { get => _titleLabel.Text; set => Helpers.DialogHelpers.SetTitle(this, _titleLabel, value); }

        public void SetCustomControl(Control control)
        {
            _hostedControl = control;

            // The hosted control *is* the content. SetContent docks it to fill the shell's content
            // row, so the manual Location/Size assignment and the SizeChanged handler that re-applied
            // Size on every resize — a hand-rolled Dock.Fill, and one that leaked a handler
            // subscription on every call — are both gone.
            //
            // It takes fill: a hosted control is the reason this dialog exists, so it is the row that
            // should absorb whatever height is left over. Disposal of a previously hosted control is
            // SetContent's, which is why this no longer disposes it first — doing both meant the
            // shell was handed an already-disposed control to remove.
            if (control == null) return;

            // Into the content cell the designer declared — row 1 of the text column, the row given
            // Percent(100) so a hosted control absorbs whatever height is left over.
            var existing = _shell.GetControlFromPosition(1, 1);
            if (existing != null && existing != control)
            {
                _shell.Controls.Remove(existing);
                existing.Dispose();
            }

            control.Dock = DockStyle.Fill;
            control.Margin = Padding.Empty;
            _shell.Controls.Add(control, 1, 1);
        }

        /// <summary>
        /// Opens with focus inside the hosted content when it can take it.
        /// </summary>
        /// <remarks>
        /// The other five dialogs each decide where focus lands; this one decided nothing, so focus
        /// went wherever the tab order happened to start. The hosted control is the reason this dialog
        /// exists, so it gets first refusal — falling back to the primary action when it holds nothing
        /// focusable, which is what a dialog showing a read-only panel wants.
        /// </remarks>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Helpers.DialogHelpers.DescribeActions(this);

            if (_hostedControl != null && !_hostedControl.IsDisposed && _hostedControl.CanSelect
                && _hostedControl.SelectNextControl(null, true, true, true, false))
            {
                return;
            }

            if (_hostedControl != null && !_hostedControl.IsDisposed && _hostedControl.CanSelect)
            {
                _hostedControl.Focus();
                return;
            }

            if (_okButton.Visible && _okButton.Enabled) _okButton.Focus();
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

        /// <summary>
        /// Routes Enter and Escape through the form's button properties, not the fields behind them.
        /// </summary>
        /// <remarks>
        /// The difference matters: <c>CancelButton</c> is the single lever a caller has for turning
        /// Escape off — <c>BeepDialogManager</c> clears it when a config sets
        /// <c>CloseOnEscape = false</c>. Pressing <c>_cancelButton</c> directly read straight past
        /// that decision, so a dialog that asked not to be dismissed by a stray keypress was
        /// dismissed by one anyway.
        /// <para>
        /// With no cancel route the key is swallowed rather than passed on: leaving
        /// <c>CancelButton</c> null stops the default handling, but this override would otherwise
        /// forward Escape to a base that may still act on it depending on how the form is hosted.
        /// </para>
        /// </remarks>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && AcceptButton != null) { AcceptButton.PerformClick(); return true; }

            if (keyData == Keys.Escape)
            {
                CancelButton?.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void ApplyTheme()
        {
            if (_titleLabel == null) return;
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme;
            _okButton.Theme = Theme; _cancelButton.Theme = Theme;

        }
    }
}
