using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDialogForm : BeepiForm
    {
        // This member holds the custom result.
        private BeepDialogResult _dialogResult = BeepDialogResult.None;

        public BeepDialogForm()
        {
            InitializeComponent();
        }

        #region Utility Methods

        private void ShowCustomControl(Control ctl, string title)
        {
            // Remove any previously added custom control.
            if (beepDialogBox1._customControl != null && Controls.Contains(beepDialogBox1._customControl))
            {
                Controls.Remove(beepDialogBox1._customControl);
                beepDialogBox1._customControl.Dispose();
            }

            beepDialogBox1._customControl = ctl;
            beepDialogBox1._customControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Position the custom control.
            int buttonHeight = beepDialogBox1._buttonPanel.Height;
            beepDialogBox1._customControl.Location = new Point(beepDialogBox1.DrawingRect.Left + 10, beepDialogBox1.TitleBottomY + 10);
            beepDialogBox1._customControl.Size = new Size(
                beepDialogBox1.DrawingRect.Width - 20,
                beepDialogBox1.DrawingRect.Height - beepDialogBox1.TitleBottomY - buttonHeight - 20
            );

            beepDialogBox1.Controls.Add(beepDialogBox1._customControl);
            beepDialogBox1._customControl.BringToFront();

            if (!string.IsNullOrEmpty(title))
            {
                beepDialogBox1.TitleText = title;
            }

            BringToFront();
        }

        private void CleanupCustomControl()
        {
            // Remove and dispose any custom control.
            if (beepDialogBox1._customControl != null && Controls.Contains(beepDialogBox1._customControl))
            {
                beepDialogBox1.Controls.Remove(beepDialogBox1._customControl);
                beepDialogBox1._customControl.Dispose();
                beepDialogBox1._customControl = null;
            }
        }

        #endregion

        #region Synchronous Dialog Templates

        // Synchronous ShowDialog with submit and cancel actions.
        // This version wires up the button click events to set _dialogResult,
        // then calls ShowDialog() to display the form modally.
        public BeepDialogResult ShowDialog(Control ctl, Action submit = null, Action cancel = null, string title = null)
        {
            if (ctl == null)
                throw new ArgumentNullException(nameof(ctl));

            // Reset the custom result.
            _dialogResult = BeepDialogResult.None;
            ShowCustomControl(ctl, title);

            // Attach event handlers.
            EventHandler primaryHandler = (s, e) =>
            {
                submit?.Invoke();
                _dialogResult = BeepDialogResult.OK;
                // Set the form's DialogResult and close.
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            EventHandler secondaryHandler = (s, e) =>
            {
                cancel?.Invoke();
                _dialogResult = BeepDialogResult.Cancel;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            beepDialogBox1.PrimaryButtonClicked += primaryHandler;
            beepDialogBox1.SecondaryButtonClicked += secondaryHandler;

            // Show the form modally.
          DialogResult a =  ShowDialog();

            // Remove event handlers.
            beepDialogBox1.PrimaryButtonClicked -= primaryHandler;
            beepDialogBox1.SecondaryButtonClicked -= secondaryHandler;
            CleanupCustomControl();

            return _dialogResult;
        }

        // Overload for dialogs with a single OK action.
        public BeepDialogResult ShowDialog(Control ctl, Action ok, string title)
        {
            _dialogResult = BeepDialogResult.None;
            ShowCustomControl(ctl, title);

            EventHandler primaryHandler = (s, e) =>
            {
                ok?.Invoke();
                _dialogResult = BeepDialogResult.OK;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            beepDialogBox1.PrimaryButtonClicked += primaryHandler;
            beepDialogBox1._secondaryButton.Visible = false;

            ShowDialog();

            beepDialogBox1.PrimaryButtonClicked -= primaryHandler;
            CleanupCustomControl();

            return _dialogResult;
        }

        // Synchronous info dialog.
        public void ShowInfoDialog(string message, Action okAction, string title)
        {
            // Create a label for the message.
            var label = new Label
            {
                Text = message,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            // Create a user control to host the label.
            var control = new UserControl
            {
                ClientSize = new Size(400, 200)
            };
            control.Controls.Add(label);

            // Display the dialog.
            ShowDialog(control, submit: okAction, title: title);
        }

        // Synchronous confirmation dialog.
        public BeepDialogResult ShowConfirmationDialog(string message, Action confirmAction, Action cancelAction)
        {
            _dialogResult = BeepDialogResult.None;

            // Configure dialog appearance.
            beepDialogBox1.ShowTitle = false;
            beepDialogBox1.ShowTitleLine = false;
            beepDialogBox1.PrimaryButtonColor = Color.Red;
            beepDialogBox1.PrimaryButtonText = "Yes, I'm sure";
            beepDialogBox1.SecondaryButtonText = "No, cancel";

            // Configure icon.
            beepDialogBox1._iconImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.alert.svg";
            beepDialogBox1._iconImage.Location = new Point((this.Width - beepDialogBox1._iconImage.Width) / 2, 20);
            beepDialogBox1._iconImage.Visible = true;

            // Create and add a label.
            var beepLabel = new BeepLabel
            {
                Text = message,
                AutoSize = true,
                Location = new Point((this.Width - 200) / 2, beepDialogBox1._iconImage.Bottom + 10),
                Width = 200
            };
            Controls.Add(beepLabel);

            EventHandler primaryHandler = (s, e) =>
            {
                confirmAction?.Invoke();
                _dialogResult = BeepDialogResult.Yes;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            EventHandler secondaryHandler = (s, e) =>
            {
                cancelAction?.Invoke();
                _dialogResult = BeepDialogResult.No;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            beepDialogBox1.PrimaryButtonClicked += primaryHandler;
            beepDialogBox1.SecondaryButtonClicked += secondaryHandler;

            base.ShowDialog();

            beepDialogBox1.PrimaryButtonClicked -= primaryHandler;
            beepDialogBox1.SecondaryButtonClicked -= secondaryHandler;
            CleanupCustomControl();

            return _dialogResult;
        }

        // Synchronous custom button dialog.
        public BeepDialogResult ShowCustomButtonDialog(string title, string message, string primaryButtonText, string secondaryButtonText)
        {
            var label = new Label
            {
                Text = message,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var control = new UserControl
            {
                ClientSize = new Size(400, 200)
            };
            control.Controls.Add(label);

            // Set custom button texts.
            beepDialogBox1.PrimaryButtonText = primaryButtonText;
            beepDialogBox1.SecondaryButtonText = secondaryButtonText;

            return ShowDialog(control, submit: null, cancel: null, title: title);
        }

        #endregion
    }
}
