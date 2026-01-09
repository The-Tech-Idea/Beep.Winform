using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
 
using BeepDialogResult=TheTechIdea.Beep.Vis.Modules.BeepDialogResult;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dialog")]
    [Category("Beep Controls")]
    [Description("A dialog box control that displays a message and buttons.")]
    public class BeepDialogBox : BeepPanel
    {
        public BeepImage _iconImage = new BeepImage();
        public BeepPanel _buttonPanel = new BeepPanel();
        public BeepButton _primaryButton = new BeepButton();
        public BeepButton _secondaryButton = new BeepButton();
        public BeepButton _thirdButton = new BeepButton();
        public BeepButton _closeButton = new BeepButton();
        public Control _customControl = new BeepButton();
        public BeepDialogResult dialogResult ;
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

     

        private void InitializeDialog()
        {
            // Initialize controls as before
            //MessageBox.Config("Initializing Dialog");
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
                IsFrameless = true,
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
        //    base.ApplyTheme();
            if (_currentTheme == null) return;

            // MessageBox.Config("Applying MenuStyle");

            // Check if each button is not null before applying theme
            if (_primaryButton != null)
            {
                //     MessageBox.Config("Applying MenuStyle to button 1");
             //   _primaryButton.MenuStyle = MenuStyle;
               _primaryButton.BackColor = _currentTheme.ButtonBackColor;
                _primaryButton.ForeColor = _currentTheme.PrimaryTextColor;
            }
            // MessageBox.Config("Applying MenuStyle 1");
            if (_secondaryButton != null)
            {
                //      MessageBox.Config("Applying MenuStyle to button 2");
                _secondaryButton.Theme = Theme;
                _secondaryButton.BackColor = _currentTheme.ButtonBackColor;
                _secondaryButton.ForeColor = _currentTheme.SecondaryTextColor;
            }
            // MessageBox.Config("Applying MenuStyle 2");
            if (_closeButton != null)
            {
                //   MessageBox.Config("Applying MenuStyle to button 3");
                _closeButton.Theme = Theme;
            //    _closeButton.BackColor = Color.Red;
                _closeButton.ForeColor = _currentTheme.DialogCloseButtonForeColor ;
            }
            // MessageBox.Config("Applying MenuStyle 3");
            // Additional background colors, title lines, etc.
             BackColor = _currentTheme.PanelBackColor;
            _buttonPanel.Theme = Theme;
            Arrange();

            //  MessageBox.Config("Applying MenuStyle 4");
            ////base.ApplyTheme();
            // Invalidate(); // Redraw to apply theme changes
        }
      

    }
}

