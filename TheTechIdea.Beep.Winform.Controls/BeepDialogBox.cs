﻿using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dialog")]
    [Category("Beep Controls")]
    public class BeepDialogBox : BeepPanel
    {
        private BeepImage _iconImage = new BeepImage();
        private BeepPanel _buttonPanel = new BeepPanel();
        private BeepButton _primaryButton = new BeepButton();
        private BeepButton _secondaryButton = new BeepButton();
        private BeepButton _closeButton = new BeepButton();
        private Control _customControl = new BeepButton();

        public event EventHandler PrimaryButtonClicked;
        public event EventHandler SecondaryButtonClicked;
        public event EventHandler CloseButtonClicked;
        public BeepDialogBox()
        {
            //InitializeDialog();

        }
        protected override void InitLayout()
        {
            base.InitLayout();
            InitializeDialog();
            TitleText = "Dialog Title";
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
            int padding = BorderThickness + 10;
            if (_closeButton != null)
            {
                _closeButton.Location = new Point(Width - _closeButton.Width - 10, padding);
            }

            // Reposition _buttonPanel within DrawingRect on resize
            if (_buttonPanel != null)
            {

                _buttonPanel.Location = new Point(padding, DrawingRect.Bottom - _buttonPanel.Height - padding);
                _buttonPanel.Width = Width - padding * 2; ;// DrawingRect.Width - BorderThickness * 3;

            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //  DrawButtonSeparatorLine(e.Graphics);
        }

        private void DrawButtonSeparatorLine(Graphics graphics)
        {
            int lineY = _buttonPanel.Top - 5;
            using (var pen = new Pen(_currentTheme?.BorderColor ?? Color.Gray, TitleLineThickness))
            {
                graphics.DrawLine(pen, DrawingRect.Left + BorderThickness, lineY, DrawingRect.Right - BorderThickness, lineY);
            }
        }

        public void ShowDialog(Control ctl, Action submit, Action cancel, string Title)
        {
            ShowCustomControl(ctl, Title);
            PrimaryButtonClicked += (s, e) => submit?.Invoke();
            SecondaryButtonClicked += (s, e) => cancel?.Invoke();
        }

        public void ShowDialog(Control ctl, Action ok, string Title)
        {
            ShowCustomControl(ctl, Title);
            PrimaryButtonClicked += (s, e) => ok?.Invoke();
            _secondaryButton.Visible = false;
        }

        private void ShowCustomControl(Control ctl, string Title)
        {
            _customControl = ctl;
            _customControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            int buttonHeight = _buttonPanel.Height;
            _customControl.Location = new Point(DrawingRect.Left + 10, TitleBottomY + 10);
            _customControl.Size = new Size(
                DrawingRect.Width - 20,
                DrawingRect.Height - TitleBottomY - buttonHeight - 20);

            Controls.Add(_customControl);
            _customControl.BringToFront();
            TitleText = Title;
            BringToFront();
        }

        #region Dialog Templates

        public void ShowConfirmationDialog(string message, Action confirmAction, Action cancelAction)
        {
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

            PrimaryButtonClicked += (s, e) => confirmAction?.Invoke();
            SecondaryButtonClicked += (s, e) => cancelAction?.Invoke();

            Show();
        }

        public void ShowInfoDialog(string message, Action okAction, string Title)
        {
            ShowDialog(new Label
            {
                Text = message,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            }, okAction, Title);
        }

        public void ShowWarningDialog(string message, Action proceedAction, string Title)
        {
            _iconImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.alert.svg";
            PrimaryButtonColor = Color.Orange;
            ShowDialog(new Label
            {
                Text = message,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            }, proceedAction, Title);
        }

        public void ShowErrorDialog(string message, Action okAction, string Title)
        {
            _iconImage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.delete.svg";
            PrimaryButtonColor = Color.Red;
            ShowDialog(new Label
            {
                Text = message,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            }, okAction, Title);
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
                Padding = new Padding(10),
            };
            _buttonPanel.Location = new Point(DrawingRect.X + BorderThickness, DrawingRect.Bottom - _buttonPanel.Height - BorderThickness);
            _buttonPanel.Width = DrawingRect.Width - BorderThickness * 2;
            Controls.Add(_buttonPanel);

            // Initialize and add primary and secondary buttons
            _primaryButton = new BeepButton
            {
                Text = "OK",
                BackColor = Color.Blue,
                Dock = DockStyle.Right,
                Width = 80,
                Margin = new Padding(10)
            };
            _primaryButton.Click += (s, e) => PrimaryButtonClicked?.Invoke(this, EventArgs.Empty);

            _secondaryButton = new BeepButton
            {
                Text = "Cancel",
                BackColor = Color.Gray,
                Dock = DockStyle.Left,
                Width = 80,
                Margin = new Padding(10)
            };
            _secondaryButton.Click += (s, e) => SecondaryButtonClicked?.Invoke(this, EventArgs.Empty);

            _buttonPanel.Controls.Add(_primaryButton);
            _buttonPanel.Controls.Add(_secondaryButton);

            // Initialize close button
            _closeButton = new BeepButton
            {
                Text = "X",
                Size = new Size(14, 14),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(Width - 35, 10),
                ForeColor = _currentTheme?.TitleForColor ?? Color.Black
            };
            _closeButton.Click += (s, e) => Hide();
            Controls.Add(_closeButton);

            // Apply theme after initializing all components
            ApplyTheme();
        }

        public override void ApplyTheme()
        {
            //base.ApplyTheme();
            if (_currentTheme == null) return;

            // MessageBox.Show("Applying Theme");

            // Check if each button is not null before applying theme
            if (_primaryButton != null)
            {
                //     MessageBox.Show("Applying Theme to button 1");
                _primaryButton.Theme = Theme;
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
                _closeButton.BackColor = Color.Red;
                _closeButton.ForeColor = _currentTheme.CloseButtonColor != Color.Empty ? _currentTheme.CloseButtonColor : _closeButton.ForeColor;
            }
            // MessageBox.Show("Applying Theme 3");
            // Additional background colors, title lines, etc.
            BackColor = _currentTheme.BackgroundColor != Color.Empty ? _currentTheme.BackgroundColor : BackColor;
            try
            {
                if (_buttonPanel != null)
                {
                    //       MessageBox.Show("Applying Theme to button panel");
                    _buttonPanel.Theme = Theme;
                    _buttonPanel.ApplyTheme();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //  MessageBox.Show("Applying Theme 4");
            ////base.ApplyTheme();
            // Invalidate(); // Redraw to apply theme changes
        }



    }
}
