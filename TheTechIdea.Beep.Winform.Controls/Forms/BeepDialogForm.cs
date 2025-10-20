using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDialogForm : BeepiFormPro
    {
        private const int DEFAULT_WIDTH = 400;
        private const int DEFAULT_HEIGHT = 200;
        private const int DEFAULT_MARGIN = 10;

        private BeepDialogResult _dialogResult = BeepDialogResult.None;

        public BeepDialogForm()
        {
            InitializeComponent();
        }

        #region Utility Methods

        private void ShowCustomControl(Control ctl, string title = null)
        {
            if (ctl == null)
                throw new ArgumentNullException(nameof(ctl));

            CleanupCustomControl(); // Ensure clean state

            beepDialogBox1._customControl = ctl;
            beepDialogBox1._customControl.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                                                 AnchorStyles.Right | AnchorStyles.Bottom;

            int buttonHeight = beepDialogBox1._buttonPanel.Height;
            beepDialogBox1._customControl.Location = new Point(
                beepDialogBox1.DrawingRect.Left + DEFAULT_MARGIN,
                beepDialogBox1.TitleBottomY + DEFAULT_MARGIN
            );
            beepDialogBox1._customControl.Size = new Size(
                beepDialogBox1.DrawingRect.Width - (DEFAULT_MARGIN * 2),
                beepDialogBox1.DrawingRect.Height - beepDialogBox1.TitleBottomY -
                buttonHeight - (DEFAULT_MARGIN * 2)
            );

            beepDialogBox1.Controls.Add(beepDialogBox1._customControl);
            beepDialogBox1._customControl.BringToFront();

            if (!string.IsNullOrEmpty(title))
                beepDialogBox1.TitleText = title;

            BringToFront();
        }

        private void CleanupCustomControl()
        {
            if (beepDialogBox1?._customControl != null &&
                beepDialogBox1.Controls.Contains(beepDialogBox1._customControl))
            {
                beepDialogBox1.Controls.Remove(beepDialogBox1._customControl);
                beepDialogBox1._customControl.Dispose();
                beepDialogBox1._customControl = null;
            }
        }

       

        #endregion

        #region Synchronous Dialog Templates

        public BeepDialogResult ShowDialog(Control ctl, Action submit = null,
                                         Action cancel = null, string title = null)
        {
            if (ctl == null)
                throw new ArgumentNullException(nameof(ctl));

            _dialogResult = BeepDialogResult.None;
            ShowCustomControl(ctl, title);

            void PrimaryHandler(object s, EventArgs e)
            {
                submit?.Invoke();
                _dialogResult = BeepDialogResult.OK;
                DialogResult = DialogResult.OK;
                Close();
            }

            void SecondaryHandler(object s, EventArgs e)
            {
                cancel?.Invoke();
                _dialogResult = BeepDialogResult.Cancel;
                DialogResult = DialogResult.Cancel;
                Close();
            }

            try
            {
                beepDialogBox1.PrimaryButtonClicked += PrimaryHandler;
                beepDialogBox1.SecondaryButtonClicked += SecondaryHandler;
                base.ShowDialog();
            }
            finally
            {
                beepDialogBox1.PrimaryButtonClicked -= PrimaryHandler;
                beepDialogBox1.SecondaryButtonClicked -= SecondaryHandler;
                CleanupCustomControl();
            }

            return _dialogResult;
        }

        public BeepDialogResult ShowDialog(Control ctl, Action ok, string title)
        {
            if (ctl == null)
                throw new ArgumentNullException(nameof(ctl));

            _dialogResult = BeepDialogResult.None;
            ShowCustomControl(ctl, title);

            void PrimaryHandler(object s, EventArgs e)
            {
                ok?.Invoke();
                _dialogResult = BeepDialogResult.OK;
                DialogResult = DialogResult.OK;
                Close();
            }

            try
            {
                beepDialogBox1._secondaryButton.Visible = false;
                beepDialogBox1.PrimaryButtonClicked += PrimaryHandler;
                base.ShowDialog();
            }
            finally
            {
                beepDialogBox1.PrimaryButtonClicked -= PrimaryHandler;
                beepDialogBox1._secondaryButton.Visible = true;
                CleanupCustomControl();
            }

            return _dialogResult;
        }

        public void ShowInfoDialog(string message, Action okAction, string title)
        {
            using (var control = new UserControl { ClientSize = new Size(DEFAULT_WIDTH, DEFAULT_HEIGHT) })
            {
                var label = new Label
                {
                    Text = message,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                control.Controls.Add(label);
                ShowDialog(control, okAction, title);
            }
        }

        public BeepDialogResult ShowConfirmationDialog(string message, Action confirmAction,
                                                     Action cancelAction)
        {
            _dialogResult = BeepDialogResult.None;

            try
            {
                // Configure dialog appearance
                beepDialogBox1.ShowTitle = false;
                beepDialogBox1.ShowTitleLine = false;
                beepDialogBox1.PrimaryButtonColor = Color.Red;
                beepDialogBox1.PrimaryButtonText = "Yes, I'm sure";
                beepDialogBox1.SecondaryButtonText = "No, cancel";

                // Configure icon
                beepDialogBox1._iconImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.alert.svg";
                beepDialogBox1._iconImage.Location = new Point(
                    (Width - beepDialogBox1._iconImage.Width) / 2,
                    DEFAULT_MARGIN * 2
                );
                beepDialogBox1._iconImage.Visible = true;

                // Create and add label
                using (var beepLabel = new BeepLabel
                {
                    Text = message,
                    AutoSize = true,
                    Width = 200,
                    Location = new Point(
                        (Width - 200) / 2,
                        beepDialogBox1._iconImage.Bottom + DEFAULT_MARGIN
                    )
                })
                {
                    Controls.Add(beepLabel);

                    void PrimaryHandler(object s, EventArgs e)
                    {
                        confirmAction?.Invoke();
                        _dialogResult = BeepDialogResult.Yes;
                        DialogResult = DialogResult.OK;
                        Close();
                    }

                    void SecondaryHandler(object s, EventArgs e)
                    {
                        cancelAction?.Invoke();
                        _dialogResult = BeepDialogResult.No;
                        DialogResult = DialogResult.Cancel;
                        Close();
                    }

                    beepDialogBox1.PrimaryButtonClicked += PrimaryHandler;
                    beepDialogBox1.SecondaryButtonClicked += SecondaryHandler;
                    base.ShowDialog();
                }
            }
            finally
            {
                // Reset appearance
                beepDialogBox1.ShowTitle = true;
                beepDialogBox1.ShowTitleLine = true;
                beepDialogBox1.PrimaryButtonColor = default;
                beepDialogBox1.PrimaryButtonText = "OK";
                beepDialogBox1.SecondaryButtonText = "Cancel";
                beepDialogBox1._iconImage.Visible = false;
                CleanupCustomControl();
            }

            return _dialogResult;
        }

        public BeepDialogResult ShowCustomButtonDialog(string title, string message,
                                                     string primaryButtonText,
                                                     string secondaryButtonText)
        {
            using (var control = new UserControl { ClientSize = new Size(DEFAULT_WIDTH, DEFAULT_HEIGHT) })
            {
                var label = new Label
                {
                    Text = message,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                control.Controls.Add(label);

                try
                {
                    beepDialogBox1.PrimaryButtonText = primaryButtonText ?? "OK";
                    beepDialogBox1.SecondaryButtonText = secondaryButtonText ?? "Cancel";
                    return ShowDialog(control, null, null, title);
                }
                finally
                {
                    beepDialogBox1.PrimaryButtonText = "OK";
                    beepDialogBox1.SecondaryButtonText = "Cancel";
                }
            }
        }

        #endregion
    }
}