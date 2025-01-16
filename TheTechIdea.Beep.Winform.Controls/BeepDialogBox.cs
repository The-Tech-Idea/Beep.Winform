using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using BeepDialogResult=TheTechIdea.Beep.Vis.Modules.BeepDialogResult;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dialog")]
    [Category("Beep Controls")]
    [Description("A dialog box control that displays a message and buttons.")]
    public class BeepDialogBox : BeepPanel
    {
        private BeepImage _iconImage = new BeepImage();
        private BeepPanel _buttonPanel = new BeepPanel();
        private BeepButton _primaryButton = new BeepButton();
        private BeepButton _secondaryButton = new BeepButton();
        private BeepButton _thirdButton = new BeepButton();
        private BeepButton _closeButton = new BeepButton();
        private Control _customControl = new BeepButton();
        private BeepDialogResult dialogResult ;
        public event EventHandler PrimaryButtonClicked;
        public event EventHandler SecondaryButtonClicked;
        public event EventHandler ThirdButtonClicked;
        public event EventHandler CloseButtonClicked;
        public BeepDialogBox()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 100;
                Height = 100;
            }
            //InitializeDialog();

        }
        protected override void InitLayout()
        {
            base.InitLayout();
            InitializeDialog();
            TitleText = "Dialog Title";
        }
        public BeepDialogResult DialogResult
        {
            get => dialogResult;
            set
            {
                dialogResult = value;
              
            }

        }

        public string PrimaryButtonText
        {
            get => _primaryButton.Text;
            set => _primaryButton.Text = value;
        }
        public string SecondaryButtonText
        {
            get => _secondaryButton.Text;
            set => _secondaryButton.Text = value;
        }
        public Color PrimaryButtonColor
        {
            get => _primaryButton.BackColor;
            set => _primaryButton.BackColor = value;
        }
        public Color SecondaryButtonColor
        {
            get => _secondaryButton.BackColor;
            set => _secondaryButton.BackColor = value;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Arrange();
        }
        private void Arrange()
        {
            int offset = 10; // Padding offset inside the DrawingRect
            Rectangle rect = DrawingRect;

            // Position the close button inside DrawingRect
            if (_closeButton != null)
            {
                int centery = TitleBottomY + (_closeButton.Height / 2);
                _closeButton.Location = new Point(DrawingRect.Width - 30, centery - TitleBottomY);
            }

            // Position the button panel at the bottom of the DrawingRect
            if (_buttonPanel != null)
            {
                _buttonPanel.Location = new Point(
                    rect.Left + offset,
                    rect.Bottom - offset - _buttonPanel.Height
                );
                _buttonPanel.Width = rect.Width - offset * 2;
            }

            // Arrange buttons inside the button panel
            int buttonSpacing = 10; // Spacing between buttons
            int totalButtonWidth = 0;

            // Calculate the total width of visible buttons
            if (_primaryButton.Visible) totalButtonWidth += _primaryButton.Width;
            if (_secondaryButton.Visible) totalButtonWidth += _secondaryButton.Width;
            if (_thirdButton.Visible) totalButtonWidth += _thirdButton.Width;

            totalButtonWidth += buttonSpacing * (VisibleButtonCount() - 1);

            // Center the buttons within the button panel
            int startX = (_buttonPanel.Width - totalButtonWidth) / 2;
            int currentX = startX;

            // Position primary button
            if (_primaryButton.Visible)
            {
                _primaryButton.Location = new Point(currentX, (_buttonPanel.Height - _primaryButton.Height) / 2);
                currentX += _primaryButton.Width + buttonSpacing;
            }

            // Position third button (if visible)
            if (_thirdButton.Visible)
            {
                _thirdButton.Location = new Point(currentX, (_buttonPanel.Height - _thirdButton.Height) / 2);
                currentX += _thirdButton.Width + buttonSpacing;
            }

            // Position secondary button
            if (_secondaryButton.Visible)
            {
                _secondaryButton.Location = new Point(currentX, (_buttonPanel.Height - _secondaryButton.Height) / 2);
            }
        }

        // Helper method to count visible buttons
        private int VisibleButtonCount()
        {
            int count = 0;
            if (_primaryButton.Visible) count++;
            if (_secondaryButton.Visible) count++;
            if (_thirdButton.Visible) count++;
            return count;
        }

        #region Async Dialog Templates
        public async Task<BeepDialogResult> ShowDialogAsync(Control ctl, Func<Task> submit = null, Func<Task> cancel = null, string title = null)
        {
            // Ensure proper initialization
            if (ctl == null) throw new ArgumentNullException(nameof(ctl));

            var tcs = new TaskCompletionSource<BeepDialogResult>();

            ShowCustomControl(ctl, title);

            // Attach handlers to buttons
            PrimaryButtonClicked += async (s, e) =>
            {
                try
                {
                    if (submit != null) await submit();
                    tcs.TrySetResult(BeepDialogResult.OK);
                }
                finally
                {
                    HideDialog();
                }
            };

            SecondaryButtonClicked += async (s, e) =>
            {
                try
                {
                    if (cancel != null) await cancel();
                    tcs.TrySetResult(BeepDialogResult.Cancel);
                }
                finally
                {
                    HideDialog();
                }
            };

            return await tcs.Task;
        }
        public async Task<BeepDialogResult> ShowDialogAsync(Control ctl, Func<Task> ok, string title)
        {
            var tcs = new TaskCompletionSource<BeepDialogResult>();

            ShowCustomControl(ctl, title);

            PrimaryButtonClicked += async (s, e) =>
            {
                if (ok != null) await ok();
                tcs.SetResult(BeepDialogResult.OK);
                HideDialog(); // Replace Close with HideDialog
            };

            _secondaryButton.Visible = false;

            return await tcs.Task;
        }
        public async Task ShowInfoDialogAsync(string message, Func<Task> okAction, string title)
        {
            // Create a label for the message
            var label = new Label
            {
                Text = message,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            // Create a user control to host the label
            var control = new UserControl
            {
                ClientSize = new Size(400, 200)
            };
            control.Controls.Add(label);

            // Configure and display the dialog
            await ShowDialogAsync(
                control,
                submit: async () =>
                {
                    if (okAction != null) await okAction();
                },
                title: title
            );
        }
        public async Task<BeepDialogResult> ShowConfirmationDialogAsync(string message, Func<Task> confirmAction, Func<Task> cancelAction)
        {
            var tcs = new TaskCompletionSource<BeepDialogResult>();

            ShowTitle = false;
            ShowTitleLine = false;

            PrimaryButtonColor = Color.Red;
            PrimaryButtonText = "Yes, I'm sure";
            SecondaryButtonText = "No, cancel";

            _iconImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.alert.svg";
            _iconImage.Location = new Point((Width - _iconImage.Width) / 2, 20);
            _iconImage.Visible = true;

            var beepLabel = new BeepLabel
            {
                Text = message,
                AutoSize = true,
                Location = new Point((Width - 200) / 2, _iconImage.Bottom + 10),
                Width = 200
            };
            Controls.Add(beepLabel);

            PrimaryButtonClicked += async (s, e) =>
            {
                if (confirmAction != null) await confirmAction();
                tcs.SetResult(BeepDialogResult.Yes);
                HideDialog(); // Replace Close with HideDialog
            };

            SecondaryButtonClicked += async (s, e) =>
            {
                if (cancelAction != null) await cancelAction();
                tcs.SetResult(BeepDialogResult.No);
                HideDialog(); // Replace Close with HideDialog
            };

            return await tcs.Task;
        }
        public async Task<BeepDialogResult> ShowCustomButtonDialogAsync(string title, string message, string primaryButtonText, string secondaryButtonText)
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

            // Configure buttons
            //ConfigureButtons(DialogButtons.Custom, customPrimaryButtonText: primaryButtonText, customSecondaryButtonText: secondaryButtonText);

            return await ShowDialogAsync(
                control,
                submit: () => Task.CompletedTask,
                cancel: () => Task.CompletedTask,
                title: title
            );
        }

        #endregion

        #region Utility Methods
        //public void ConfigureButtons(DialogButtons dialogButtons, string customPrimaryButtonText = null, string customSecondaryButtonText = null)
        //{
        //    // Reset buttons
        //    _primaryButton.Visible = false;
        //    _secondaryButton.Visible = false;

        //    switch (dialogButtons)
        //    {
        //        case DialogButtons.None:
        //            // No buttons
        //            break;

        //        case DialogButtons.Ok:
        //            _primaryButton.Text = "OK";
        //            _primaryButton.Visible = true;
        //            _secondaryButton.Visible = false;
        //            break;

        //        case DialogButtons.OkCancel:
        //            _primaryButton.Text = "OK";
        //            _secondaryButton.Text = "Cancel";
        //            _primaryButton.Visible = true;
        //            _secondaryButton.Visible = true;
        //            break;

        //        case DialogButtons.YesNo:
        //            _primaryButton.Text = "Yes";
        //            _secondaryButton.Text = "No";
        //            _primaryButton.Visible = true;
        //            _secondaryButton.Visible = true;
        //            break;

        //        case DialogButtons.YesNoCancel:
        //            _primaryButton.Text = "Yes";
        //            _secondaryButton.Text = "No";
        //           ;
        //            _primaryButton.Visible = true;
        //            _secondaryButton.Visible = true;
        //            break;

        //        case DialogButtons.Custom:
        //            if (!string.IsNullOrEmpty(customPrimaryButtonText))
        //            {
        //                _primaryButton.Text = customPrimaryButtonText;
        //                _primaryButton.Visible = true;
        //            }

        //            if (!string.IsNullOrEmpty(customSecondaryButtonText))
        //            {
        //                _secondaryButton.Text = customSecondaryButtonText;
        //                _secondaryButton.Visible = true;
        //            }
        //            break;

        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(dialogButtons), dialogButtons, "Unsupported DialogButtons type");
        //    }

        //    Arrange(); // Re-arrange the buttons
        //}

        private void ShowCustomControl(Control ctl, string title)
        {
            // Remove any previously added custom controls
            if (_customControl != null && Controls.Contains(_customControl))
            {
                Controls.Remove(_customControl);
                _customControl.Dispose();
            }

            _customControl = ctl;
            _customControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Position the custom control
            int buttonHeight = _buttonPanel.Height;
            _customControl.Location = new Point(DrawingRect.Left + 10, TitleBottomY + 10);
            _customControl.Size = new Size(
                DrawingRect.Width - 20,
                DrawingRect.Height - TitleBottomY - buttonHeight - 20
            );

            Controls.Add(_customControl);
            _customControl.BringToFront();

            if (!string.IsNullOrEmpty(title))
            {
                TitleText = title;
            }

            BringToFront();
        }


        private void HideDialog()
        {
            // Detach event handlers to avoid memory leaks
            PrimaryButtonClicked = null;
            SecondaryButtonClicked = null;
            CloseButtonClicked = null;

            // Hide the control
            Visible = false;

            // Remove custom control if any
            if (_customControl != null && Controls.Contains(_customControl))
            {
                Controls.Remove(_customControl);
                _customControl.Dispose();
                _customControl = null;
            }
        }

        #endregion

        private void InitializeDialog()
        {
            // Initialize controls as before
            //MessageBox.Show("Initializing Dialog");
            _iconImage = new BeepImage { Size = new Size(48, 48), Visible = false };
            Controls.Add(_iconImage);

            _buttonPanel = new BeepPanel
            {
                ShowAllBorders = false,
                ShowTitle = false,
                ShowTitleLine = false,
                ShowShadow = false,
                Height = 60,
                Padding = new Padding(5),
            };
            _buttonPanel.Location = new Point(DrawingRect.X + BorderThickness, DrawingRect.Bottom - _buttonPanel.Height - BorderThickness);
            _buttonPanel.Width = DrawingRect.Width - BorderThickness * 2;
            Controls.Add(_buttonPanel);

            // Initialize and add primary and secondary buttons
            _primaryButton = new BeepButton
            {
                Text = "OK",
              
                Dock = DockStyle.Right,
                Width = 80,
             
                Margin = new Padding(10)
            };
            _primaryButton.Click += (s, e) => PrimaryButtonClicked?.Invoke(this, EventArgs.Empty);

            _secondaryButton = new BeepButton
            {
                Text = "Cancel",
              
                Dock = DockStyle.Left,
                Width = 80,
               
                Margin = new Padding(10)
            };
            _secondaryButton.Click += (s, e) => SecondaryButtonClicked?.Invoke(this, EventArgs.Empty);

            _thirdButton = new BeepButton
            {
                Text = "None",

                Dock = DockStyle.Top,
                Width = 80,
                Visible = false,
                Margin = new Padding(10)
            };
            _thirdButton.Click += (s, e) => ThirdButtonClicked?.Invoke(this, EventArgs.Empty);
            _buttonPanel.Controls.Add(_thirdButton);
            _buttonPanel.Controls.Add(_primaryButton);
            _buttonPanel.Controls.Add(_secondaryButton);

            // Initialize close button
            _closeButton = new BeepButton
            {
                Text = "X",
                IsChild=true,
                Size = new Size(20, 20),
                IsFramless = true,
                UseScaledFont = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(Width - 25, 5),
            };
            _closeButton.Click += (s, e) => Hide();
            Controls.Add(_closeButton);
            Arrange();
            // Apply theme after initializing all components
            ApplyTheme();
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // MessageBox.Show("Applying Theme");

            // Check if each button is not null before applying theme
            if (_primaryButton != null)
            {
                //     MessageBox.Show("Applying Theme to button 1");
             //   _primaryButton.Theme = Theme;
               _primaryButton.BackColor = _currentTheme.ButtonBackColor;
                _primaryButton.ForeColor = _currentTheme.PrimaryTextColor;
            }
            // MessageBox.Show("Applying Theme 1");
            if (_secondaryButton != null)
            {
                //      MessageBox.Show("Applying Theme to button 2");
                _secondaryButton.Theme = Theme;
                _secondaryButton.BackColor = _currentTheme.ButtonBackColor;
                _secondaryButton.ForeColor = _currentTheme.SecondaryTextColor;
            }
            // MessageBox.Show("Applying Theme 2");
            if (_closeButton != null)
            {
                //   MessageBox.Show("Applying Theme to button 3");
                _closeButton.Theme = Theme;
            //    _closeButton.BackColor = Color.Red;
                _closeButton.ForeColor = _currentTheme.CloseButtonColor ;
            }
            // MessageBox.Show("Applying Theme 3");
            // Additional background colors, title lines, etc.
             BackColor = _currentTheme.PanelBackColor;
            _buttonPanel.Theme = Theme;
            Arrange();

            //  MessageBox.Show("Applying Theme 4");
            ////base.ApplyTheme();
            // Invalidate(); // Redraw to apply theme changes
        }
      

    }
}

