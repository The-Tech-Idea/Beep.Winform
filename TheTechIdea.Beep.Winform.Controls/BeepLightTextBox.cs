using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms.Design;
using System.Xml;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Description("A lightweight text box control that only shows editing UI when needed.")]
    [DisplayName("Beep Light TextBox")]
    [Category("Beep Controls")]
    public class BeepLightTextBox : BeepControl
    {
        #region Properties
        private TextBox _editTextBox;
        private bool _isEditing = false;
        private BeepButton _imageButton;
        private BeepImage _image;
        private BeepLabel _label;
   
     //   private string _displayText = string.Empty;
        private bool _passwordMode = false;
        private Color _textColor;
        private Size _maxImageSize = new Size(16, 16);
        private int _padding = 4;
        private string _resourcespath= "TheTechIdea.Beep.Winform.Controls.GFX.SVG.INFO";
        private string _infoicon = "info.svg";
        private string _erroricon = "error.svg";
        private string _warningicon = "warning.svg";
        private string _successicon = "success.svg";
        private string _alerticon = "alert.svg";
        private string _likelyicon = "like.svg";
        private string _importanticon = "important.svg";
        private string _hearticon = "heart.svg";
        private string _helpicon = "help.svg";
        private string _questionicon = "question.svg";
        private string _ignoreicon = "ignore.svg";
        private string _coolicon = "cool.svg";

    

        public new event EventHandler TextChanged;
        private int _lines = 3;
        private ScrollBars _scrollBars = ScrollBars.None;

        private ValidationTypeBasedonIcon _validationType = ValidationTypeBasedonIcon.Info;
        [Browsable(true)]
        [Category("Validation")]
        [Description("Type of validation to show when the field is invalid.")]
        public ValidationTypeBasedonIcon ValidationType
        {
            get => _validationType;
            set
            {
                _validationType = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines which scroll bars should be displayed in multiline mode.")]
        public ScrollBars ScrollBars
        {
            get => _scrollBars;
            set
            {
                _scrollBars = value;
                if (_editTextBox != null)
                {
                    _editTextBox.ScrollBars = value;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Number of visible lines when multiline is enabled.")]
        public int Lines
        {
            get => _lines;
            set
            {
                if (value < 1) value = 1;
                _lines = value;
                if (_multiline)
                {
                    int lineHeight = TextRenderer.MeasureText("X", _textFont).Height;
                    Height = (lineHeight * _lines) + (_padding * 2) + 2; // +2 for borders
                }
                Invalidate();
            }
        }

        private bool _multiline = false;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Determines whether the text box supports multiple lines.")]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                if (_editTextBox != null)
                {
                    _editTextBox.Multiline = value;
                    _editTextBox.WordWrap = value && _wordWrap;  // Only apply WordWrap in multiline mode
                    _editTextBox.ScrollBars = value ? _scrollBars : ScrollBars.None;

                    if (value)
                    {
                        int lineHeight = TextRenderer.MeasureText("X", _textFont).Height;
                        Height = (lineHeight * _lines) + (_padding * 2) + 2; // +2 for borders
                    }

                    AdjustEditTextBoxSize();
                }
                Invalidate();
            }
        }



        // Configuration for when to activate editing
        private EditActivation _editActivation = EditActivation.Click;
        public enum EditActivation { Click, DoubleClick, Manual }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Determines how editing is activated")]
        public EditActivation ActivationMode
        {
            get => _editActivation;
            set => _editActivation = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config if text should be displayed as password")]
        public bool PasswordMode
        {
            get => _passwordMode;
            set
            {
                _passwordMode = value;
                if (_editTextBox != null)
                    _editTextBox.UseSystemPasswordChar = value;
                Invalidate();
            }
        }

      

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the text")]
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Invalidate();
            }
        }

        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the text")]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                UseThemeFont = false;
                Font = _textFont;
                if (_editTextBox != null)
                    _editTextBox.Font = _textFont;
                Invalidate();
            }
        }

        private string _placeholderText = "";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text shown when the field is empty")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Specify the image file to load (SVG, PNG, JPG, etc.).")]
        public string ImagePath
        {
            get => _imageButton?.ImagePath;
            set
            {
                if (_imageButton == null)
                {
                    _imageButton = new BeepButton();
                }
                _imageButton.ImagePath = value;
                _imageButton.Size = _maxImageSize;
                _imageButton.MaxImageSize = new Size(_maxImageSize.Width - 1, _maxImageSize.Height - 1);
                _imageButton.IsChild = true;
                _imageButton.IsFrameless = true;
                _imageButton.ShowAllBorders = false;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum size of the image")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                if (_imageButton != null)
                {
                    _imageButton.Size = value;
                    _imageButton.MaxImageSize = new Size(value.Width - 1, value.Height - 1);
                }
                Invalidate();
            }
        }

        private HorizontalAlignment _textAlignment = HorizontalAlignment.Left;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Alignment of the text within the control")]
        public HorizontalAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                if (_editTextBox != null)
                    _editTextBox.TextAlign = value;
                Invalidate();
            }
        }

        private bool _isReadOnly = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Determines if the text can be edited")]
        public bool ReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                if (_editTextBox != null)
                    _editTextBox.ReadOnly = value;
            }
        }
        private bool _isRequired = false;
        [Browsable(true)]
        [Category("Validation")]
        [Description("Indicates whether a value is required (cannot be empty)")]
        public bool IsRequired
        {
            get => _isRequired;
            set => _isRequired = value;
        }

        private string _requiredErrorMessage = "This field is required.";
        [Browsable(true)]
        [Category("Validation")]
        [Description("Error message shown when a required field is empty")]
        public string RequiredErrorMessage
        {
            get => _requiredErrorMessage;
            set => _requiredErrorMessage = value;
        }

        private bool _validateOnLostFocus = true;
        [Browsable(true)]
        [Category("Validation")]
        [Description("Whether to validate when focus is lost")]
        public bool ValidateOnLostFocus
        {
            get => _validateOnLostFocus;
            set => _validateOnLostFocus = value;
        }
        private bool _wordWrap = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Determines whether text should wrap at the end of the line in multiline mode.")]
        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                if (_editTextBox != null)
                {
                    _editTextBox.WordWrap = value;
                }
                Invalidate();
            }
        }

        #endregion

        #region Constructor and Initialization
        public BeepLightTextBox() : base()
        {
            BoundProperty = "Text";
            BorderRadius = 3;
            Padding = new Padding(1);
            ShowAllBorders = true;
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            CanBeHovered = true;
            _textColor = ForeColor;

            // Add a click handler to start editing when clicked
            this.MouseClick += BeepLightTextBox_MouseClick;
            this.MouseDoubleClick += BeepLightTextBox_MouseDoubleClick;

            _image = new BeepImage
            {
                ImagePath = _resourcespath + "." + _infoicon,
                Size = new Size(16, 16),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false
            };
            _label = new BeepLabel
            {
                Text = "Info",
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false
            };
            // Create but don't add the editing textbox yet
            InitializeEditTextBox();
            InitializeImageButton();
            InitializeDefaultErrorMessages();
            InitializeDropdown();

        }
        protected override void OnParentChanged(EventArgs e)
        {
            // First unsubscribe from old parent
            if (Tag is Control oldParent && oldParent is BeepControl oldBeepParent)
                oldBeepParent.ClearChildExternalDrawing(this);

            base.OnParentChanged(e);

            // Remember this new parent for next time
            Tag = Parent;

            // Only register if validation indicator is visible and there's an error
            if (Parent is BeepControl newBeepParent && _showValidationIndicator)
            {
                newBeepParent.AddChildExternalDrawing(
                    this,
                    DrawInfoIconExternally,
                    DrawingLayer.AfterContent
                );
            }
        }
        private void InitializeDefaultErrorMessages()
        {
            // Set default error messages that will be used if no custom ones are specified
            _customErrorMessages[TextBoxMaskFormat.Email] = "Please enter a valid email address.";
            _customErrorMessages[TextBoxMaskFormat.Date] = "Please enter a valid date.";
            _customErrorMessages[TextBoxMaskFormat.Time] = "Please enter a valid time.";
            _customErrorMessages[TextBoxMaskFormat.DateTime] = "Please enter a valid date and time.";
            _customErrorMessages[TextBoxMaskFormat.Decimal] = "Please enter a valid number.";
            _customErrorMessages[TextBoxMaskFormat.Currency] = "Please enter a valid currency amount.";
            _customErrorMessages[TextBoxMaskFormat.Percentage] = "Please enter a valid percentage.";
            _customErrorMessages[TextBoxMaskFormat.PhoneNumber] = "Please enter a valid phone number.";
            _customErrorMessages[TextBoxMaskFormat.SocialSecurityNumber] = "Please enter a valid social security number.";
            _customErrorMessages[TextBoxMaskFormat.ZipCode] = "Please enter a valid ZIP code.";
            _customErrorMessages[TextBoxMaskFormat.IPAddress] = "Please enter a valid IP address.";
            _customErrorMessages[TextBoxMaskFormat.URL] = "Please enter a valid URL.";
            _customErrorMessages[TextBoxMaskFormat.CreditCard] = "Please enter a valid credit card number.";
            _customErrorMessages[TextBoxMaskFormat.Custom] = "The entered value does not match the required format.";
        }
        private void InitializeEditTextBox()
        {
            _editTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Visible = false,
                WordWrap = _wordWrap,  // Apply the WordWrap property
                Multiline = _multiline, // Use the Multiline property
                TextAlign = _textAlignment,
                UseSystemPasswordChar = _passwordMode,
                Font = _textFont,
                ScrollBars = _scrollBars
            };

            _editTextBox.LostFocus += (s, e) => EndEditing(true);
            // Consolidate KeyDown and KeyPress handling to prevent conflicts
            _editTextBox.KeyDown += (s, e) =>
            {
                // Handle navigation keys in KeyDown
                if (!_multiline && e.KeyCode == Keys.Enter)
                {
                    EndEditing(true);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    EndEditing(false);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
                else if (_isDropdownOpen)
                {
                    // Handle dropdown navigation
                    if (e.KeyCode == Keys.Escape || (e.KeyCode == Keys.Tab && !e.Shift))
                    {
                        HideDropdown();
                        e.Handled = true;
                    }
                }
                else if (_items.Count > 0 && e.KeyCode == Keys.Down)
                {
                    ShowDropdown();
                    e.Handled = true;
                }
            };

            _editTextBox.GotFocus += (s, e) =>
            {
                if (_isEditing)
                {
                    // Only select all text when initially receiving focus, not during typing
                    if (_editTextBox.Text.Length > 0 && _editTextBox.SelectionLength == 0)
                    {
                        _editTextBox.SelectAll();
                    }
                }
            };
         
            // Only use KeyPress for character filtering, not for navigation
            _editTextBox.KeyPress += (s, e) =>
            {
                // We'll keep the filtering logic in a separate method
                HandleKeyPress(e);
            };

            _editTextBox.TextChanged += (s, e) =>
            {
                // Store caret position before making changes
                int selectionStart = _editTextBox.SelectionStart;
                int selectionLength = _editTextBox.SelectionLength;

                // Update underlying text
                Text = _editTextBox.Text;

                // Apply mask formatting if needed
                if (!_isApplyingMask)
                {
                    _isApplyingMask = true;
                    ApplyMaskFormat();
                    _isApplyingMask = false;
                }

                // Handle dropdown if needed
                if (_items.Count > 0)
                {
                    if (!_isDropdownOpen)
                    {
                        ShowDropdown();
                    }
                    else if (_dropdownListForm != null)
                    {
                        _dropdownListForm.Filter(_editTextBox.Text);
                    }
                }

                // Invoke custom TextChanged event
                TextChanged?.Invoke(this, EventArgs.Empty);

                // Make sure we still have focus
                if (!_editTextBox.Focused)
                {
                    _editTextBox.Focus();
                }
            };
            _editTextBox.Text = string.Empty;

        }
        private void InitializeImageButton()
        {
            if (_imageButton == null)
            {
                _imageButton = new BeepButton
                {
                    Size = _maxImageSize,
                    IsChild = true,
                    IsFrameless = true,
                    ShowAllBorders = false,
                    IsBorderAffectedByTheme = false,
                    MaxImageSize = new Size(_maxImageSize.Width - 1, _maxImageSize.Height - 1),
                    ImagePath = "dropdown_arrow.svg" // Set a default dropdown arrow icon
                };

                _imageButton.Click += (s, e) =>
                {
                    // If dropdown mode, show dropdown, otherwise start editing
                    if (_items.Count > 0)
                    {
                        if (_isDropdownOpen)
                            HideDropdown();
                        else
                            ShowDropdown();
                    }
                    else if (!_isEditing && !ReadOnly)
                    {
                        StartEditing();
                    }
                };

                // Add the button to the Controls collection
                Controls.Add(_imageButton);
            }
        }
        private void InitializeDropdown()
        {
            if (_dropdownListForm == null)
            {
                _dropdownListForm = new BeepPopupListForm
                {
                    ListItems = _items,
                    ShowTitle = false,
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = _currentTheme?.ListBackColor ?? Color.White,
                    ForeColor = _currentTheme?.ListForeColor ?? Color.Black,
                    MaximumSize = new Size(
                        _maxDropdownWidth > 0 ? _maxDropdownWidth : Screen.PrimaryScreen.WorkingArea.Width,
                        _maxDropdownHeight
                    )
                };
                _dropdownListForm.AutoClose = false;
                // Correctly subscribe to the SelectedItemChanged event with proper event args type
                _dropdownListForm.SelectedItemChanged += (s, e) =>
                {
                    // e is SelectedItemChangedEventArgs which has a SelectedItem property
                    if (e.SelectedItem != null)
                    {
                        SelectedItem = e.SelectedItem;
                        Text = e.SelectedItem.Text;
                        _editTextBox.Text = Text;
                        HideDropdown();
                    }
                };
            }
        }

        #endregion

        #region Editing Behavior
        private void BeepLightTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (_editActivation == EditActivation.Click && !_isEditing && !ReadOnly)
            {
                StartEditing();
            }
        }
        private void BeepLightTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_editActivation == EditActivation.DoubleClick && !_isEditing && !ReadOnly)
            {
                StartEditing();
            }
        }
        private void AdjustEditTextBoxSize()
        {
            if (_editTextBox == null)
                return;

            // Ensure WordWrap reflects multiline mode
            _editTextBox.WordWrap = _multiline && _wordWrap;

            // Calculate padding values
            int leftPad = Padding.Left + 2; // Add a little extra for better appearance
            int topPad = Padding.Top + 2;
            int rightPad = Padding.Right + 2;
            int bottomPad = Padding.Bottom + 2;

            // Button width adjustment
            int buttonWidth = 0;
            if (_imageButton != null && _imageButton.Visible && !string.IsNullOrEmpty(_imageButton.ImagePath))
            {
                buttonWidth = _imageButton.Width + 2; // Add small gap between text and button
            }

            if (_multiline)
            {
                // For multiline mode, fill the entire client area with appropriate padding
                _editTextBox.Location = new Point(leftPad, topPad);
                _editTextBox.Size = new Size(
                    Width - (leftPad + rightPad + buttonWidth),
                    Height - (topPad + bottomPad)
                );
            }
            else
            {
                // For single line mode, calculate vertical centering
                int availableHeight = Height - (topPad + bottomPad);
                int textHeight = _editTextBox.PreferredHeight;
                int verticalPosition = topPad + Math.Max(0, (availableHeight - textHeight) / 2);

                _editTextBox.Location = new Point(leftPad, verticalPosition);
                _editTextBox.Size = new Size(
                    Width - (leftPad + rightPad + buttonWidth),
                    textHeight
                );
            }

            // If the textbox is visible but misaligned, force a refresh
            if (_editTextBox.Visible)
            {
                _editTextBox.Refresh();
            }
        }
        public void StartEditing()
        {
            if (_isEditing || ReadOnly)
                return;

            _isEditing = true;
            IsSelected = true;

            // Position and configure the edit box
            AdjustEditTextBoxSize();

            // Apply appropriate theme colors to the edit textbox
            _editTextBox.BackColor = IsChild && Parent != null ? Parent.BackColor : _currentTheme.TextBoxBackColor;
            _editTextBox.ForeColor = _currentTheme.TextBoxForeColor;

            // Make sure edit textbox is added to Controls if not already
            if (!Controls.Contains(_editTextBox))
            {
                Controls.Add(_editTextBox);
            }

            // Set text after adding to controls to avoid text being cleared
            _editTextBox.Text = Text;

            _editTextBox.Visible = true;
            _editTextBox.BringToFront();

            // Force a layout update before focusing to ensure control is properly positioned
            Application.DoEvents();

            // Set focus and select text as a separate operation
            _editTextBox.Focus();
            _editTextBox.SelectAll();

            Invalidate();
        }

        public void EndEditing(bool saveChanges)
        {
            if (_isEditing)
                return;

            _isEditing = false;

            if (saveChanges)
            {
                string newValue = _editTextBox.Text;
                Text = newValue;
                // If ApplyMask is on, perform validation right away
                if (_isApplyingMask && _maskFormatEnum != TextBoxMaskFormat.None)
                {
                    if (!ValidateData(out string validationMessage))
                    {
                        // If validation failed, show tooltip and keep editing if indicator enabled
                        if (_showValidationIndicator)
                        {
                            _isEditing = true;
                            _editTextBox.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    // Original validation logic for non-masked input
                    bool isValid = true;
                    string message = string.Empty;

                    // Check if field is required
                    if (_isRequired && string.IsNullOrWhiteSpace(newValue))
                    {
                        isValid = false;
                        message = _requiredErrorMessage;
                    }
                    else
                    {
                        // Remaining validation logic
                        // ...
                    }

                    // Update validation state
                    IsValid = isValid;
                    ValidationMessage = message;

                    // Etc...
                }

                // Set the text value if we get here
                Text = newValue;
                OnTextChanged(EventArgs.Empty);
            }

            // Hide the textbox
            _editTextBox.Visible = false;
            IsSelected = Focused;

            Invalidate();
            Focus();
            if (_dropdownListForm != null && _dropdownListForm.Visible)
            {
                // If the dropdown is open, don't hide it
                HideDropdown();
            }
        }

        #endregion

        #region Drawing and Layout
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            PositionImageButton();
            // Remember this new parent for next time
            

        }

        private void PositionImageButton()
        {
            if (_imageButton != null && !string.IsNullOrEmpty(_imageButton.ImagePath))
            {
                // Position the image button on the right side of the control
                _imageButton.Location = new Point(
                    Width - _imageButton.Width - _padding,
                    (Height - _imageButton.Height) / 2
                );

                //if (!Controls.Contains(_imageButton))
                //{
                //    Controls.Add(_imageButton);
                //}
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionImageButton();

            if (_isEditing)
            {
                AdjustEditTextBoxSize();
            }
        }


        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            DrawControl(g, DrawingRect);
        }

        private void DrawControl(Graphics g,Rectangle rectangle)
        {
            // Always draw the border regardless of content
            Color borderColor = BorderColor;
            rectangle = new Rectangle(rectangle.X+1 , rectangle.Y+1, rectangle.Width - 1, rectangle.Height - 1);
            // Determine border color based on state
            if (!Enabled)
            {
                borderColor = DisabledBackColor;
            }
            else if (IsSelected || Focused)
            {
                borderColor = _selectedBorderColor;
            }
            else if (_isHovered)
            {
                borderColor = HoverBorderColor;
            }

            if (ShowAllBorders == false)
            {
                using (Pen borderPen = new Pen(borderColor, 1))
                {
                    // Always draw a bottom line  border (whether we have text or not)
                    // Draw line at the bottom of the control
                    g.DrawLine(borderPen, 0, rectangle.Height - 1, rectangle.Width, rectangle.Height - 1);


                }
            }


            // If we're in edit mode, don't draw text content as the TextBox will show it
            if (_isEditing)
            {
                // Make sure the edit textbox is visible and added to the control
                if (!Controls.Contains(_editTextBox))
                {
                    Controls.Add(_editTextBox);
                }

                if (!_editTextBox.Visible)
                {
                    _editTextBox.Visible = true;
                }

                return;
            }

            // Determine text color based on state
            Color textColor;

            if (!Enabled)
            {
                textColor = DisabledForeColor;
            }
            else if (IsSelected)
            {
                textColor = SelectedForeColor;
            }
            else if (_isHovered)
            {
                textColor = HoverForeColor;
            }
            else
            {
                // If text is empty, use placeholder color, otherwise use text color
                textColor = string.IsNullOrEmpty(Text) ?
                    _currentTheme?.TextBoxPlaceholderColor ?? Color.Gray :
                    (Enabled ? _textColor : DisabledForeColor);
            }

            // If validation failed and we're showing indicators, draw with error styling
            if (!_isValid && _showValidationIndicator)
            {
                // Use error colors from theme if available
                textColor = _currentTheme?.TextBoxErrorForeColor ?? Color.Red;
                borderColor = _currentTheme?.TextBoxErrorBorderColor ?? Color.Red;

                // Draw error border
                using (Pen errorPen = new Pen(borderColor, 1))
                {
                    if (IsRounded)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(rectangle, BorderRadius))
                        {
                            g.DrawPath(errorPen, path);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(errorPen, rectangle);
                    }
                }
            }

            // Calculate text layout
            Rectangle textRect = rectangle;
            textRect.X += _padding;
            textRect.Width -= _padding * 2;
            textRect.Y += _padding;
            textRect.Height -= _padding * 2;

            // Adjust for the image button if present
            if (_imageButton != null && !string.IsNullOrEmpty(_imageButton.ImagePath))
            {
                textRect.Width -= _imageButton.Width + _padding;
            }

            // Inside the DrawContent method, where text formatting flags are set
            TextFormatFlags flags = TextFormatFlags.EndEllipsis;

            if (_multiline)
            {
                // Only add word break if word wrap is enabled
                if (_wordWrap)
                    flags |= TextFormatFlags.WordBreak;
            }
            else
            {
                flags |= TextFormatFlags.VerticalCenter;
            }

            switch (_textAlignment)
            {
                case HorizontalAlignment.Left:
                    flags |= TextFormatFlags.Left;
                    break;
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Right;
                    break;
            }

            // Determine text to draw - either placeholder or actual text
            string textToDraw = string.IsNullOrEmpty(Text) ? _placeholderText :     Text;
            if (string.IsNullOrEmpty(Text))
            {
                if (string.IsNullOrEmpty(_placeholderText))
                {
                    if (_isApplyingMask)
                    { 
                        switch (MaskFormat)
                        {
                            case TextBoxMaskFormat.Currency:
                                _placeholderText = "0.00";
                                break;
                            case TextBoxMaskFormat.Decimal:
                                _placeholderText = "0.00";
                                break;
                            case TextBoxMaskFormat.Percentage:
                                _placeholderText = "0%";
                                break;
                            case TextBoxMaskFormat.PhoneNumber:
                                _placeholderText = "(000) 000-0000";
                                break;
                            case TextBoxMaskFormat.SocialSecurityNumber:
                                _placeholderText = "000-00-0000";
                                break;
                            case TextBoxMaskFormat.ZipCode:
                                _placeholderText = "00000";
                                break;
                            case TextBoxMaskFormat.IPAddress:
                                _placeholderText = "000.000.000.000";
                                break;
                            case TextBoxMaskFormat.URL:
                                _placeholderText = "http://www.example.com";
                                break;
                            case TextBoxMaskFormat.CreditCard:
                                _placeholderText = "0000-0000-0000-0000";
                                break;
                            case TextBoxMaskFormat.Email:
                                _placeholderText = "aaa@aaa.com";
                                break;
                            case TextBoxMaskFormat.Date:
                                _placeholderText = DateTime.Now.ToString("MM/dd/yyyy");
                                break;
                            case TextBoxMaskFormat.Time:
                                _placeholderText = DateTime.Now.ToString("hh:mm tt");
                                break;
                            case TextBoxMaskFormat.DateTime:
                                _placeholderText = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                                break;
                            case TextBoxMaskFormat.Custom:
                                _placeholderText = "Enter value";
                                break;
                            default:
                                _placeholderText = "Enter value";
                                break;



                        }
                    }
                }
                textToDraw = _placeholderText;
                // Use placeholder text color from theme if available
                textColor = _currentTheme?.TextBoxPlaceholderColor ?? Color.Gray;
            }
            else
            {
                textToDraw = Text;

                // If password mode, draw masked text
                if (_passwordMode && !string.IsNullOrEmpty(Text))
                {
                    textToDraw = new string('•', Text.Length);
                }
            }
            // Always draw text (either the actual text or placeholder)
            // Only draw if there's something to draw
            if (!string.IsNullOrEmpty(textToDraw))
            {
                TextRenderer.DrawText(g, textToDraw, _textFont, textRect, textColor, flags);
            }

            // If we have items, draw a dropdown indicator
            if (_items.Count > 0 )
            {
                int arrowSize = 8;
                int padding = 4;
                int arrowX = Width - arrowSize - padding;
                int arrowY = (Height - arrowSize) / 2;

                // Draw dropdown arrow
                using (Brush brush = new SolidBrush(Enabled ? ForeColor : DisabledForeColor))
                using (GraphicsPath path = new GraphicsPath())
                {
                    Point[] points = new Point[]
                    {
                new Point(arrowX, arrowY),
                new Point(arrowX + arrowSize, arrowY),
                new Point(arrowX + arrowSize / 2, arrowY + arrowSize)
                    };

                    path.AddPolygon(points);
                    g.FillPath(brush, path);
                }
            }
        }
        private void DrawInfoIconExternally(Graphics g, Rectangle childBounds)
        {
            // only draw when invalid & not editing
            if (!_isValid && _showValidationIndicator )
            {
                 int iconSize = childBounds.Height-1;
                // compute icon location relative to parent
                int iconX = childBounds.Right +  _padding;
                int iconY = childBounds.Y + (childBounds.Height - iconSize) / 2;

                // choose image based on error type
                switch (ValidationType)
                {
                    case ValidationTypeBasedonIcon.Info:
                        _image.ImagePath = $"{_resourcespath}.{_infoicon}";
                        break;
                    case ValidationTypeBasedonIcon.Warning:
                        _image.ImagePath = $"{_resourcespath}.{_warningicon}";
                        break;
                    case ValidationTypeBasedonIcon.Error:
                        _image.ImagePath = $"{_resourcespath}.{_erroricon}";
                        break;
                    case ValidationTypeBasedonIcon.Success:
                        _image.ImagePath = $"{_resourcespath}.{_successicon}";
                        break;
                    case ValidationTypeBasedonIcon.Alert:
                        _image.ImagePath = $"{_resourcespath}.{_alerticon}";
                        break;
                    case ValidationTypeBasedonIcon.Important:
                        _image.ImagePath = $"{_resourcespath}.{_importanticon}";
                        break;
                    case ValidationTypeBasedonIcon.Likely:
                        _image.ImagePath = $"{_resourcespath}.{_likelyicon}";
                        break;
                    case ValidationTypeBasedonIcon.Heart:
                        _image.ImagePath = $"{_resourcespath}.{_hearticon}";
                        break;
                    case ValidationTypeBasedonIcon.Help:
                        _image.ImagePath = $"{_resourcespath}.{_helpicon}";
                        break;
                    case ValidationTypeBasedonIcon.Question:
                        _image.ImagePath = $"{_resourcespath}.{_questionicon}";
                        break;
                    case ValidationTypeBasedonIcon.Ignore:
                        _image.ImagePath = $"{_resourcespath}.{_ignoreicon}";
                        break;
                    case ValidationTypeBasedonIcon.Cool:
                        _image.ImagePath = $"{_resourcespath}.{_coolicon}";
                        break;
                }
                // build your resource path
               

                // Draw it into the parent's graphics at exactly that rectangle
                var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
                _image.Draw(g, iconRect);
                // draw the standard error icon so we know for sure something should appear
                //using (var bmp = SystemIcons.Error.ToBitmap())
                //{
                //    g.DrawImage(bmp, iconRect);
                //}

                //Debug.WriteLine($"[BeepLightTextBox] Drew error icon at {iconRect}");
            }
        }
        private void DrawInfoMessageExternally(Graphics g, Rectangle childBounds)
        {
            // only draw when invalid & not editing
            if (_isValid && _showValidationIndicator)
            {
                // draw the standard error text so we know for sure something should appear
                // below control
                List<string> messages = new List<string>();
                messages=GetErrorMessagess();
                string message = string.Join(Environment.NewLine, messages);
                var textSize = g.MeasureString(message, _textFont);
                int textX = childBounds.X + _padding;
                int textY = childBounds.Bottom + _padding;
                var textRect = new Rectangle(textX, textY, (int)textSize.Width, (int)textSize.Height);
                // Draw the text
                TextRenderer.DrawText(g, message, _textFont, textRect, Color.Red, TextFormatFlags.Left);

            }
        }

        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Handle parent background color inheritance for child controls
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                parentbackcolor = Parent.BackColor;
            }
            else
            {
                BackColor = _currentTheme.TextBoxBackColor;
                
            }

            // Apply text colors
            ForeColor = _currentTheme.TextBoxForeColor;
            TextColor = _currentTheme.TextBoxForeColor;

            // Border colors
            BorderColor = _currentTheme.TextBoxBorderColor;

            // State-specific colors (to be used in drawing)
            HoverBackColor = _currentTheme.TextBoxHoverBackColor;
            HoverForeColor = _currentTheme.TextBoxHoverForeColor;
            HoverBorderColor = _currentTheme.TextBoxHoverBorderColor;

            SelectedBackColor = _currentTheme.TextBoxSelectedBackColor;
            SelectedForeColor = _currentTheme.TextBoxSelectedForeColor;
            _selectedBorderColor = _currentTheme.TextBoxSelectedBorderColor;

            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;

            // Apply colors to the edit textbox
            if (_editTextBox != null)
            {
                _editTextBox.BackColor = BackColor;
                _editTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            }

            // Apply colors to the image button if exists
            if (_imageButton != null)
            {
              //  _imageButton.Theme = Theme;
                _imageButton.IsChild = true;
                _imageButton.BackColor = BackColor;
                _imageButton.ParentBackColor = BackColor;
                _imageButton.ForeColor = _currentTheme.TextBoxForeColor;
            }
            if (_label != null)
            {
                //_label.Theme = Theme;
                _label.IsChild = true;
                _label.BackColor = BackColor;
                _label.ParentBackColor = BackColor;
                _label.ForeColor = _currentTheme.TextBoxForeColor;
            }
            if (_image != null)
            {
              //  _image.Theme = Theme;
                _image.IsChild = true;
                _image.BackColor = BackColor;
                _image.ParentBackColor = BackColor;
                _image.ForeColor = _currentTheme.TextBoxForeColor;
            }
            // Apply font from theme if needed
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager_v2.ToFont(_currentTheme.LabelSmall);
                Font = _textFont;
                if (_editTextBox != null)
                {
                    _editTextBox.Font = _textFont;
                }
            }

            // Force redraw
            Invalidate();
        }

        #endregion
        #region Validation Properties
        private TextBoxMaskFormat _maskFormatEnum = TextBoxMaskFormat.None;
        private string _customMask = string.Empty;
        private string _dateFormat = "MM/dd/yyyy";
        private string _timeFormat = "HH:mm:ss";
        private string _dateTimeFormat = "MM/dd/yyyy HH:mm:ss";
        private bool _onlyDigits = false;
        private bool _onlyCharacters = false;
        
        private bool _showValidationIndicator = true;
        private bool _isValid = true;
        private string _validationMessage = string.Empty;

        private bool _isApplyingMask = false;
        [Browsable(true)]
        [Category("Validation")]
        [Description("Restrict input to a specific format (e.g., email, date, etc.).")]
       public bool ApplyMask
        {
            get => _isApplyingMask;
            set
            {
                _isApplyingMask = value;
                if (_isEditing)
                    ApplyMaskFormat();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Validation")]
        [Description("Restrict input to digits only.")]
        public bool OnlyDigits
        {
            get => _onlyDigits;
            set => _onlyDigits = value;
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Restrict input to alphabetic characters only.")]
        public bool OnlyCharacters
        {
            get => _onlyCharacters;
            set => _onlyCharacters = value;
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Specify the mask or format for text display.")]
        public TextBoxMaskFormat MaskFormat
        {
            get => _maskFormatEnum;
            set
            {
                _maskFormatEnum = value;
                if (_isEditing)
                    ApplyMaskFormat();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Specify a custom mask format.")]
        public string CustomMask
        {
            get => _customMask;
            set
            {
                _customMask = value;
                if (MaskFormat == TextBoxMaskFormat.Custom && _isEditing)
                    ApplyMaskFormat();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Specify the custom date format for display.")]
        public string DateFormat
        {
            get => _dateFormat;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _dateFormat = "MM/dd/yyyy"; // Default format
                }
                else
                {
                    // Validate the format string
                    try
                    {
                        DateTime.Now.ToString(value, CultureInfo.CurrentCulture);
                        _dateFormat = value;
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"The date format '{value}' is invalid.");
                    }
                }
                if (MaskFormat == TextBoxMaskFormat.Date && _isEditing)
                    ApplyMaskFormat();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Specify the custom time format for display.")]
        public string TimeFormat
        {
            get => _timeFormat;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _timeFormat = "HH:mm:ss"; // Default time format (24-hour)
                }
                else
                {
                    // Validate the format string
                    try
                    {
                        DateTime.Now.ToString(value, CultureInfo.CurrentCulture);
                        _timeFormat = value;
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"The time format '{value}' is invalid.");
                    }
                }
                if (MaskFormat == TextBoxMaskFormat.Time && _isEditing)
                    ApplyMaskFormat();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Specify the custom date and time format for display.")]
        public string DateTimeFormat
        {
            get => _dateTimeFormat;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _dateTimeFormat = $"{_dateFormat} {_timeFormat}"; // Default combined format
                }
                else
                {
                    // Validate the format string
                    try
                    {
                        DateTime.Now.ToString(value, CultureInfo.CurrentCulture);
                        _dateTimeFormat = value;
                    }
                    catch (FormatException)
                    {
                        throw new FormatException($"The date and time format '{value}' is invalid.");
                    }
                }
                if (MaskFormat == TextBoxMaskFormat.DateTime && _isEditing)
                    ApplyMaskFormat();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Shows a validation indicator when the value is invalid")]
        public bool ShowValidationIndicator
        {
            get => _showValidationIndicator;
            set
            {
                _showValidationIndicator = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get => _isValid;
            private set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnValidationStateChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public string ValidationMessage
        {
            get => _validationMessage;
            private set
            {
                _validationMessage = value;
                ToolTipText = _isValid ? string.Empty : value;
            }
        }
        public event EventHandler ValidationStateChanged;
        public event EventHandler<ValidationEventArgs> Validating;

       

        // Add a method to update the external drawing registration when validation state changes
        protected virtual void OnValidationStateChanged(EventArgs e)
        {
            ValidationStateChanged?.Invoke(this, e);

            // Update external drawing registration based on validation state
            if (Parent is BeepControl parentBeepControl)
            {
               
                // First clear existing registration to prevent duplicates
                parentBeepControl.ClearChildExternalDrawing(this);

                // Only register if we have an error and validation indicator is shown
                if (!_isValid && _showValidationIndicator)
                {
                    parentBeepControl.AddChildExternalDrawing(
                        this,
                        DrawInfoIconExternally,
                        DrawingLayer.AfterContent
                    );
                }
            }
        }


        /// <summary>
        /// Clears the validation state and hides any validation tooltips
        /// </summary>
        public void ClearValidation()
        {
            IsValid = true;
            ValidationMessage = string.Empty;

            // Hide tooltip
            if (_toolTip != null)
            {
                _toolTip.Hide(this);
            }

            Invalidate();
        }

        /// <summary>
        /// Validates the current text value according to the control's validation settings.
        /// </summary>
        /// <param name="message">Error message if validation fails</param>
        /// <returns>True if the value is valid, false otherwise</returns>
        /// <summary>
        /// Validates the current text value according to the control's validation settings,
        /// with special handling for masked input.
        /// </summary>
        /// <param name="message">Error message if validation fails</param>
        /// <returns>True if the value is valid, false otherwise</returns>
        public bool ValidateData(out string message)
        {
            // Reset validation icon
            ValidationType = ValidationTypeBasedonIcon.None;
         
            // First check if the field is required
            if (_isRequired && string.IsNullOrWhiteSpace(Text))
            {
                message = _requiredErrorMessage;
                IsValid = false;
                ValidationMessage = message;
                ValidationType = ValidationTypeBasedonIcon.Error;

                if (_showTooltipOnValidationError)
                {
                    ShowValidationTooltip(message);
                }
                return false;
            }

            // Skip validation if empty and not required
            if (string.IsNullOrWhiteSpace(Text) && !_isRequired)
            {
                message = string.Empty;
                IsValid = true;
                ValidationMessage = string.Empty;
                return true;
            }

            // Check if there's any custom validation
            var args = new ValidationEventArgs { Value = Text, IsValid = true };
            Validating?.Invoke(this, args);

            if (args.Cancel)
            {
                message = args.Message;
                IsValid = false;
                ValidationMessage = message;
                ValidationType = ValidationTypeBasedonIcon.Alert;

                if (_showTooltipOnValidationError)
                {
                    ShowValidationTooltip(message);
                }
                return false;
            }

            // Now check mask-related validation
            bool isValid;

            // If we have a mask format and ApplyMask is true, validate according to format
            if (_maskFormatEnum != TextBoxMaskFormat.None && _isApplyingMask)
            {
                isValid = ValidateByFormat(Text, out message);

                // Set the specific validation icon type based on the mask format
                if (!isValid)
                {
                    switch (_maskFormatEnum)
                    {
                        case TextBoxMaskFormat.Email:
                        case TextBoxMaskFormat.URL:
                        case TextBoxMaskFormat.IPAddress:
                            ValidationType = ValidationTypeBasedonIcon.Warning;
                            break;

                        case TextBoxMaskFormat.Currency:
                        case TextBoxMaskFormat.Decimal:
                        case TextBoxMaskFormat.Percentage:
                            ValidationType = ValidationTypeBasedonIcon.Error;
                            break;

                        case TextBoxMaskFormat.Date:
                        case TextBoxMaskFormat.Time:
                        case TextBoxMaskFormat.DateTime:
                            ValidationType = ValidationTypeBasedonIcon.Important;
                            break;

                        default:
                            ValidationType = ValidationTypeBasedonIcon.Error;
                            break;
                    }
                }
            }
            else
            {
                // No mask or mask not applied, do regular validation
                isValid = ValidateByFormat(Text, out message);
                ValidationType = !isValid ? ValidationTypeBasedonIcon.Error : ValidationTypeBasedonIcon.None;
            }

            // Update validation state
            IsValid = isValid;
            ValidationMessage = message;

            // Show or hide tooltip based on validation result
            if (!isValid && _showTooltipOnValidationError)
            {
                ShowValidationTooltip(message);
            }
            else if (isValid && _toolTip != null)
            {
                _toolTip.Hide(this);
            }

            return isValid;
        }


        private void HandleKeyPress(KeyPressEventArgs e)
        {
            // Retrieve the current culture's decimal and group separators
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            // Don't handle special keys in KeyPress - we do that in KeyDown
            if (e.KeyChar == (char)Keys.Escape || e.KeyChar == (char)Keys.Tab || e.KeyChar == (char)Keys.Enter)
            {
                return; // Let KeyDown handle these
            }
            // Handle input restrictions based on OnlyDigits and OnlyCharacters
            if (OnlyDigits)
            {
                // Allow digits, control characters, decimal separator, and group separator
                if (!char.IsDigit(e.KeyChar) &&
                    !char.IsControl(e.KeyChar) &&
                    e.KeyChar.ToString() != decimalSeparator &&
                    e.KeyChar.ToString() != groupSeparator)
                {
                    e.Handled = true;
                    return;
                }

                // If decimal separator is pressed, allow only one occurrence
                if (e.KeyChar.ToString() == decimalSeparator && _editTextBox.Text.Contains(decimalSeparator))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (OnlyCharacters)
            {
                // Allow only letters and control characters
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }
            }

            // Additional restrictions based on MaskFormat
            switch (MaskFormat)
            {
                case TextBoxMaskFormat.Currency:
                case TextBoxMaskFormat.Percentage:
                case TextBoxMaskFormat.Decimal:
                    // Allow only one decimal separator based on current culture
                    if (e.KeyChar.ToString() == decimalSeparator && _editTextBox.Text.Contains(decimalSeparator))
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.PhoneNumber:
                    // Allow digits, control characters, and specific phone number symbols
                    if (!char.IsDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar) &&
                        e.KeyChar != '(' &&
                        e.KeyChar != ')' &&
                        e.KeyChar != '-' &&
                        e.KeyChar != ' ' &&
                        e.KeyChar != '+')
                    {
                        e.Handled = true;
                    }
                    break;

                case TextBoxMaskFormat.Email:
                    // Allow standard email characters: letters, digits, and specific symbols
                    if (!char.IsLetterOrDigit(e.KeyChar) &&
                        !char.IsControl(e.KeyChar) &&
                        e.KeyChar != '@' &&
                        e.KeyChar != '.' &&
                        e.KeyChar != '_' &&
                        e.KeyChar != '-' &&
                        e.KeyChar != '+')
                    {
                        e.Handled = true;
                    }
                    break;

                    // Add additional cases for other mask formats
                    // ... (other format validations as in BeepTextBox)
            }
        }
        private void ApplyMaskFormat()
        {
            if (!_isEditing || _editTextBox.Text == string.Empty)
                return;
            int originalSelection = _editTextBox.SelectionStart;
            string originalText = _editTextBox.Text;
            bool formatApplied = false;
            switch (MaskFormat)
            {
                case TextBoxMaskFormat.Currency:
                    ApplyCurrencyFormat();
                    break;
                case TextBoxMaskFormat.Percentage:
                    ApplyPercentageFormat();
                    break;
                case TextBoxMaskFormat.Date:
                    ApplyDateFormat();
                    break;
                case TextBoxMaskFormat.Time:
                    ApplyTimeFormat();
                    break;
                case TextBoxMaskFormat.PhoneNumber:
                    ApplyPhoneNumberFormat();
                    break;
                case TextBoxMaskFormat.Email:
                    // Email format doesn't alter display but can trigger validation
                    break;
                case TextBoxMaskFormat.SocialSecurityNumber:
                    ApplySSNFormat();
                    break;
                case TextBoxMaskFormat.ZipCode:
                    ApplyZipCodeFormat();
                    break;
                case TextBoxMaskFormat.IPAddress:
                    ApplyIPAddressFormat();
                    break;
                case TextBoxMaskFormat.CreditCard:
                    ApplyCreditCardFormat();
                    break;
                case TextBoxMaskFormat.Hexadecimal:
                    // Hexadecimal input is handled via KeyPress restrictions
                    break;
                case TextBoxMaskFormat.URL:
                    // URL format doesn't alter display but can trigger validation
                    break;
                case TextBoxMaskFormat.TimeSpan:
                    ApplyTimeSpanFormat();
                    break;
                case TextBoxMaskFormat.Decimal:
                    ApplyDecimalFormat();
                    break;
                case TextBoxMaskFormat.DateTime:
                    ApplyDateTimeFormat();
                    break;
                case TextBoxMaskFormat.Custom:
                    ApplyCustomFormat();
                    break;
            }
            // After applying the mask, validate the result if needed
            if (formatApplied && ValidateOnLostFocus)
            {
                string message;
                if (!ValidateByFormat(_editTextBox.Text, out message))
                {
                    // If validation fails, set the validation state but don't show tooltip yet
                    // (it will be shown when editing ends or on ValidateData call)
                    IsValid = false;
                    ValidationMessage = message;

                    // Choose an appropriate icon based on format
                    switch (MaskFormat)
                    {
                        case TextBoxMaskFormat.Date:
                        case TextBoxMaskFormat.Time:
                        case TextBoxMaskFormat.DateTime:
                            ValidationType = ValidationTypeBasedonIcon.Important;
                            break;
                        default:
                            ValidationType = ValidationTypeBasedonIcon.Warning;
                            break;
                    }
                }
                else
                {
                    // Format is valid
                    IsValid = true;
                    ValidationMessage = string.Empty;
                    ValidationType = ValidationTypeBasedonIcon.None;
                }
            }
        }

        // Format-specific methods (similar to BeepTextBox)
        private bool ApplyCurrencyFormat()
        {
            if (decimal.TryParse(_editTextBox.Text, out decimal value))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = value.ToString("C2");
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
                return true;
            }
            return false;
        }


        private bool ApplyPercentageFormat()
        {
            if (decimal.TryParse(_editTextBox.Text, out decimal value))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = value.ToString("P2");
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
                return true;
            }
            return false;
        }

        // Add implementations for all other format types similar to BeepTextBox
        // ...

        // Example of date formatting
        private bool ApplyDateFormat()
        {
            if (DateTime.TryParse(_editTextBox.Text, out DateTime date))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = date.ToString(_dateFormat);
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
                return true;
            }
            return false;
        }
        private bool ValidateByFormat(string value, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
                return true; // Allow empty values (unless you want to make fields required)

            switch (MaskFormat)
            {
                case TextBoxMaskFormat.Email:
                    return ValidateEmail(value, out message);

                case TextBoxMaskFormat.Date:
                    return ValidateDate(value, out message);

                case TextBoxMaskFormat.Time:
                    return ValidateTime(value, out message);

                case TextBoxMaskFormat.DateTime:
                    return ValidateDateTime(value, out message);

                case TextBoxMaskFormat.Currency:
                case TextBoxMaskFormat.Decimal:
                case TextBoxMaskFormat.Percentage:
                    return ValidateDecimal(value, out message);

                case TextBoxMaskFormat.PhoneNumber:
                    return ValidatePhoneNumber(value, out message);

                case TextBoxMaskFormat.IPAddress:
                    return ValidateIPAddress(value, out message);

                case TextBoxMaskFormat.URL:
                    return ValidateUrl(value, out message);

                case TextBoxMaskFormat.SocialSecurityNumber:
                    return ValidateSSN(value, out message);

                case TextBoxMaskFormat.ZipCode:
                    return ValidateZipCode(value, out message);

                // Add other format validations

                case TextBoxMaskFormat.Custom:
                    // Custom validation could use regex based on CustomMask property
                    if (!string.IsNullOrEmpty(CustomMask))
                    {
                        try
                        {
                            if (!Regex.IsMatch(value, CustomMask))
                            {
                                message = "Value does not match the required format.";
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            message = $"Invalid custom format: {ex.Message}";
                            return false;
                        }
                    }
                    return true;

                default:
                    return true; // No specific validation for other formats
            }
        }

        // Format-specific validation methods
        /// <summary>
        /// Validates if a string represents a valid email address format.
        /// </summary>
        /// <param name="value">The email address to validate.</param>
        /// <param name="message">Error message if validation fails.</param>
        /// <returns>True if the email is valid, false otherwise.</returns>
        private bool ValidateEmail(string value, out string message)
        {
            message = string.Empty;

            // Handle null or empty email
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_isRequired)
                {
                    message = _requiredErrorMessage;
                    return false;
                }
                return true; // Empty is valid if not required
            }

            try
            {
                // Trim the email to remove any leading/trailing whitespace
                value = value.Trim();

                // Check length constraints
                if (value.Length > 254) // Maximum practical length for an email
                {
                    message = "Email address is too long.";
                    return false;
                }

                // Split the email into local part and domain
                int atIndex = value.LastIndexOf('@');
                if (atIndex <= 0 || atIndex == value.Length - 1)
                {
                    message = "Please enter a valid email address.";
                    return false;
                }

                string localPart = value.Substring(0, atIndex);
                string domain = value.Substring(atIndex + 1);

                // Local part validation
                if (localPart.Length > 64) // Max length for local part
                {
                    message = "The username part of the email is too long.";
                    return false;
                }

                // Domain validation
                if (!domain.Contains('.'))
                {
                    message = "The domain name appears to be incomplete.";
                    return false;
                }

                // Comprehensive regex pattern for email validation
                // This handles most valid email formats per RFC 5322
                string pattern = @"^(?:[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-zA-Z0-9-]*[a-zA-Z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";

                // For better performance with a slightly less strict pattern:
                // string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                if (!Regex.IsMatch(value, pattern))
                {
                    message = "Please enter a valid email address.";
                    return false;
                }

                // Additional domain validation - check TLD has at least 2 characters
                string[] domainParts = domain.Split('.');
                string tld = domainParts[domainParts.Length - 1];
                if (tld.Length < 2)
                {
                    message = "The domain extension appears to be invalid.";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                message = "Invalid email format: " + ex.Message;
                return false;
            }
        }


        private bool ValidateDate(string value, out string message)
        {
            message = string.Empty;
            if (!DateTime.TryParse(value, out _))
            {
                message = "Please enter a valid date.";
                return false;
            }
            return true;
        }
        private bool ValidateTime(string value, out string message)
        {
            message = string.Empty;
            if (!TimeSpan.TryParse(value, out _) && !DateTime.TryParse(value, out _))
            {
                message = "Please enter a valid time.";
                return false;
            }
            return true;
        }

        private bool ValidateDateTime(string value, out string message)
        {
            message = string.Empty;
            if (!DateTime.TryParse(value, out _))
            {
                message = "Please enter a valid date and time.";
                return false;
            }
            return true;
        }

        private bool ValidateDecimal(string value, out string message)
        {
            message = string.Empty;
            if (!decimal.TryParse(value, out _))
            {
                message = "Please enter a valid number.";
                return false;
            }
            return true;
        }

        private bool ValidatePhoneNumber(string value, out string message)
        {
            message = string.Empty;

            // Strip all non-digit characters
            string digits = Regex.Replace(value, @"\D", "");

            // Check if we have a valid phone number (minimum 10 digits for US phone)
            if (digits.Length < 10)
            {
                message = "Please enter a valid phone number.";
                return false;
            }

            return true;
        }

        private bool ValidateIPAddress(string value, out string message)
        {
            message = string.Empty;

            // Try to parse as an IP address
            if (!System.Net.IPAddress.TryParse(value, out _))
            {
                message = "Please enter a valid IP address.";
                return false;
            }

            return true;
        }

        private bool ValidateUrl(string value, out string message)
        {
            message = string.Empty;

            // Use Uri class to validate URL format
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                message = "Please enter a valid URL.";
                return false;
            }

            return true;
        }

        private bool ValidateSSN(string value, out string message)
        {
            message = string.Empty;

            // Strip all non-digit characters
            string digits = Regex.Replace(value, @"\D", "");

            // Check if we have exactly 9 digits
            if (digits.Length != 9)
            {
                message = "Please enter a valid social security number.";
                return false;
            }

            // Validate that it's not all zeros in a group (000-xx-xxxx, xxx-00-xxxx, xxx-xx-0000)
            if (digits.Substring(0, 3) == "000" || digits.Substring(3, 2) == "00" || digits.Substring(5, 4) == "0000")
            {
                message = "Please enter a valid social security number.";
                return false;
            }

            return true;
        }

        private bool ValidateZipCode(string value, out string message)
        {
            message = string.Empty;

            // Strip all non-digit characters
            string digits = Regex.Replace(value, @"\D", "");

            // Check if we have either 5 or 9 digits
            if (digits.Length != 5 && digits.Length != 9)
            {
                message = "Please enter a valid ZIP code (5 or 9 digits).";
                return false;
            }

            return true;
        }
        // Add these methods to the BeepLightTextBox class

        private void ApplyTimeFormat()
        {
            if (DateTime.TryParse(_editTextBox.Text, out DateTime time))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = time.ToString(_timeFormat);
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplyDateTimeFormat()
        {
            if (DateTime.TryParse(_editTextBox.Text, out DateTime dateTime))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = dateTime.ToString(_dateTimeFormat);
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplyPhoneNumberFormat()
        {
            // Simple formatting for U.S. phone numbers
            string digits = Regex.Replace(_editTextBox.Text, @"\D", "");
            if (digits.Length >= 10)
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = Regex.Replace(digits, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplySSNFormat()
        {
            // Formats SSN as XXX-XX-XXXX
            string digits = Regex.Replace(_editTextBox.Text, @"\D", "");
            if (digits.Length >= 9)
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = Regex.Replace(digits, @"(\d{3})(\d{2})(\d{4})", "$1-$2-$3");
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplyZipCodeFormat()
        {
            // Formats ZIP code as XXXXX or XXXXX-XXXX
            string digits = Regex.Replace(_editTextBox.Text, @"\D", "");
            int selectionStart = _editTextBox.SelectionStart;

            if (digits.Length == 5)
            {
                _editTextBox.Text = digits;
            }
            else if (digits.Length > 5)
            {
                _editTextBox.Text = Regex.Replace(digits.Substring(0, Math.Min(9, digits.Length)),
                    @"(\d{5})(\d{0,4})", m =>
                    m.Groups[2].Value.Length > 0 ? $"{m.Groups[1].Value}-{m.Groups[2].Value}" : m.Groups[1].Value);
            }

            _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
        }

        private void ApplyIPAddressFormat()
        {
            string input = _editTextBox.Text.Trim();
            int selectionStart = _editTextBox.SelectionStart;

            // Only format if it looks like an IP address
            if (Regex.IsMatch(input, @"^[\d\.]+$"))
            {
                string[] octets = input.Split('.');
                if (octets.Length <= 4)
                {
                    for (int i = 0; i < octets.Length; i++)
                    {
                        if (octets[i].Length > 0 && int.TryParse(octets[i], out int value))
                        {
                            if (value > 255)
                                octets[i] = "255";
                        }
                    }
                    _editTextBox.Text = string.Join(".", octets);
                }
            }

            _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
        }

        private void ApplyCreditCardFormat()
        {
            // Formats credit card numbers as XXXX XXXX XXXX XXXX
            string digits = Regex.Replace(_editTextBox.Text, @"\D", "");
            if (digits.Length > 0)
            {
                int selectionStart = _editTextBox.SelectionStart;
                digits = digits.Length > 16 ? digits.Substring(0, 16) : digits;
                _editTextBox.Text = Regex.Replace(digits, @"(\d{4})(?=\d)", "$1 ").Trim();
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplyTimeSpanFormat()
        {
            if (TimeSpan.TryParse(_editTextBox.Text, out TimeSpan ts))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = ts.ToString(@"hh\:mm\:ss");
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplyDecimalFormat()
        {
            if (decimal.TryParse(_editTextBox.Text, out decimal value))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = value.ToString("N2"); // Numeric format with 2 decimal places
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplyCustomFormat()
        {
            if (!string.IsNullOrEmpty(_customMask))
            {
                try
                {
                    // For custom mask where digits are represented by '9' and letters by 'a'
                    string formatted = _customMask;
                    string content = _editTextBox.Text;
                    int digitIndex = 0;
                    int letterIndex = 0;

                    for (int i = 0; i < _customMask.Length; i++)
                    {
                        if (_customMask[i] == '9') // Digit placeholder
                        {
                            char replacement = digitIndex < content.Length && char.IsDigit(content[digitIndex])
                                ? content[digitIndex++]
                                : '_';
                            formatted = formatted.Remove(i, 1).Insert(i, replacement.ToString());
                        }
                        else if (_customMask[i] == 'a') // Letter placeholder
                        {
                            char replacement = letterIndex < content.Length && char.IsLetter(content[letterIndex])
                                ? content[letterIndex++]
                                : '_';
                            formatted = formatted.Remove(i, 1).Insert(i, replacement.ToString());
                        }
                        // Non-placeholder characters are kept as-is
                    }

                    int selectionStart = _editTextBox.SelectionStart;
                    _editTextBox.Text = formatted;
                    _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
                }
                catch
                {
                    // If formatting fails, leave text as-is
                }
            }
        }

        // Implement other validation methods like ValidateTime, ValidateDecimal, etc.

        #endregion
        #region Validation Error Messages
        private List<string> GetErrorMessagess()
        {
            List<string> messages = new List<string>();
            foreach (var format in Enum.GetValues(typeof(TextBoxMaskFormat)))
            {
                if (_customErrorMessages.TryGetValue((TextBoxMaskFormat)format, out string message))
                {
                    messages.Add(message);
                }
            }
            return messages;
        }
        private Dictionary<TextBoxMaskFormat, string> _customErrorMessages = new Dictionary<TextBoxMaskFormat, string>();

        [Browsable(false)]
        public Dictionary<TextBoxMaskFormat, string> CustomErrorMessages => _customErrorMessages;

        [Browsable(true)]
        [Category("Validation")]
        [Description("Custom error message for email validation")]
        public string EmailValidationMessage
        {
            get => GetErrorMessage(TextBoxMaskFormat.Email);
            set => SetErrorMessage(TextBoxMaskFormat.Email, value);
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Custom error message for date validation")]
        public string DateValidationMessage
        {
            get => GetErrorMessage(TextBoxMaskFormat.Date);
            set => SetErrorMessage(TextBoxMaskFormat.Date, value);
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Custom error message for time validation")]
        public string TimeValidationMessage
        {
            get => GetErrorMessage(TextBoxMaskFormat.Time);
            set => SetErrorMessage(TextBoxMaskFormat.Time, value);
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Custom error message for decimal number validation")]
        public string DecimalValidationMessage
        {
            get => GetErrorMessage(TextBoxMaskFormat.Decimal);
            set => SetErrorMessage(TextBoxMaskFormat.Decimal, value);
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("Custom error message for phone number validation")]
        public string PhoneValidationMessage
        {
            get => GetErrorMessage(TextBoxMaskFormat.PhoneNumber);
            set => SetErrorMessage(TextBoxMaskFormat.PhoneNumber, value);
        }

        private string GetErrorMessage(TextBoxMaskFormat format)
        {
            if (_customErrorMessages.TryGetValue(format, out string message))
                return message;
            return string.Empty;
        }

        private void SetErrorMessage(TextBoxMaskFormat format, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                if (_customErrorMessages.ContainsKey(format))
                    _customErrorMessages.Remove(format);
            }
            else
            {
                _customErrorMessages[format] = message;
            }
        }
        #endregion
        #region ToolTip
        #region Tooltip Configuration
        private bool _showTooltipOnValidationError = true;
        private ToolTipIcon _validationToolTipIcon = ToolTipIcon.Error;
        private string _validationToolTipTitle = "Validation Error";

        [Browsable(true)]
        [Category("Validation")]
        [Description("Whether to show a tooltip when validation fails")]
        public bool ShowTooltipOnValidationError
        {
            get => _showTooltipOnValidationError;
            set => _showTooltipOnValidationError = value;
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("The icon to show in the validation tooltip")]
        public ToolTipIcon ValidationToolTipIcon
        {
            get => _validationToolTipIcon;
            set => _validationToolTipIcon = value;
        }

        [Browsable(true)]
        [Category("Validation")]
        [Description("The title to show in the validation tooltip")]
        public string ValidationToolTipTitle
        {
            get => _validationToolTipTitle;
            set => _validationToolTipTitle = value;
        }
        #endregion
        protected virtual  void ShowToolTipIfExists()
        {
            // If we're in an error state, prioritize the validation tooltip
            if (!_isValid && _showTooltipOnValidationError)
            {
                ShowValidationTooltip(ValidationMessage);
            }
            // Otherwise use the regular tooltip if there is one
            else if (!string.IsNullOrEmpty(ToolTipText))
            {
                base.ShowToolTipIfExists();
            }
        }

        /// <summary>
        /// Shows a validation error tooltip with appropriate styling
        /// </summary>
        /// <param name="message">The validation error message to display</param>
        private void ShowValidationTooltip(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // Set the tooltip text
            ToolTipText = message;

            // Get the tooltip from the base class
            if (_toolTip == null)
            {
                // If the base class tooltip is not initialized, create our own
                _toolTip = new ToolTip
                {
                    IsBalloon = true,
                    AutoPopDelay = 5000,
                    InitialDelay = 500,
                    ReshowDelay = 500,
                    ShowAlways = true,
                    ToolTipIcon = ToolTipIcon.Error,
                    ToolTipTitle = "Validation Error"
                };
            }

            // Show the tooltip near the control
            Point location = new Point(Width / 2, Height + 5);
            _toolTip.Show(message, this, location, 5000); // Show for 5 seconds
        }
        #endregion
        #region Mouse Events
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            // If the control is in an invalid state, show the validation tooltip again
            if (!_isValid && _showTooltipOnValidationError && !_isEditing)
            {
                ShowValidationTooltip(ValidationMessage);
            }
            else
            {
                // Otherwise, show the regular tooltip if there is one
                ShowToolTipIfExists();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            IsSelected = true;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (Enabled && CanBeHovered)
            {
                IsHovered = true;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsHovered = false;
          
            // Hide any tooltips when the mouse leaves
            if (_toolTip != null)
            {
                _toolTip.Hide(this);
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            //// Hide dropdown if it's open and focus is not on the dropdown
            //if (_dropdownListForm != null && _dropdownListForm.Visible)
            //{
            //    // If the dropdown is open, don't hide it
            //    HideDropdown();
            //}
        }

        #endregion
        #region Dropdown Properties
        private BeepPopupListForm _dropdownListForm;
        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private bool _isDropdownOpen = false;
        private int _maxDropdownHeight = 200;
        private int _maxDropdownWidth = 0; // 0 means match textbox width
        private BeepPopupFormPosition _dropdownPosition = BeepPopupFormPosition.Bottom;
        private bool _allowSearch = true;

        [Browsable(true)]
        [Category("Dropdown")]
        [Description("Items to display in the dropdown list")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                if (_dropdownListForm != null)
                {
                    _dropdownListForm.ListItems = _items;
                }
            }
        }

        [Browsable(true)]
        [Category("Dropdown")]
        [Description("The currently selected item from the dropdown")]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    if (_selectedItem != null)
                    {
                        Text = _selectedItem.Text;
                    }
                    OnSelectedItemChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Dropdown")]
        [Description("Maximum height of the dropdown list")]
        public int MaxDropdownHeight
        {
            get => _maxDropdownHeight;
            set => _maxDropdownHeight = value;
        }

        [Browsable(true)]
        [Category("Dropdown")]
        [Description("Maximum width of the dropdown list (0 = auto)")]
        public int MaxDropdownWidth
        {
            get => _maxDropdownWidth;
            set => _maxDropdownWidth = value;
        }

        [Browsable(true)]
        [Category("Dropdown")]
        [Description("Position of the dropdown relative to the control")]
        public BeepPopupFormPosition DropdownPosition
        {
            get => _dropdownPosition;
            set => _dropdownPosition = value;
        }

        [Browsable(true)]
        [Category("Dropdown")]
        [Description("Allow searching in the dropdown")]
        public bool AllowSearch
        {
            get => _allowSearch;
            set => _allowSearch = value;
        }

        // Events
        public event EventHandler DropdownOpened;
        public event EventHandler DropdownClosed;
        public event EventHandler SelectedItemChanged;

        protected virtual void OnDropdownOpened(EventArgs e)
        {
            DropdownOpened?.Invoke(this, e);
        }

        protected virtual void OnDropdownClosed(EventArgs e)
        {
            DropdownClosed?.Invoke(this, e);
        }

        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
        }
        /// <summary>
        /// Shows the dropdown list
        /// </summary>
        public void ShowDropdown()
        {
            if (_isDropdownOpen || _items.Count == 0)
                return;

            InitializeDropdown();

            // Store active control to restore focus after dropdown appears
            Control activeControl =_editTextBox;
            bool wasEditing = _isEditing;

            // Set size and position
            int width = _maxDropdownWidth > 0 ? _maxDropdownWidth : Width;

            _dropdownListForm.MaximumSize = new Size(
                width,
                _maxDropdownHeight
            );

            // Configure dropdown form to maintain focus in textbox
            _dropdownListForm.Deactivate += DropdownForm_Deactivate;
            _dropdownListForm.Shown += DropdownForm_Shown;

            // Show the dropdown
            SimpleItem selectedItem = _dropdownListForm.ShowPopup(
                "Select an item",
                this,
                _dropdownPosition,
                false  // Don't show title
            );

            _isDropdownOpen = true;
            OnDropdownOpened(EventArgs.Empty);

            // Restore focus to edit textbox immediately
            if (wasEditing && _editTextBox != null)
            {
                // Use BeginInvoke to ensure UI updates before focusing
                BeginInvoke(new Action(() => {
                    if (_editTextBox != null && _isEditing)
                    {
                        _editTextBox.Focus();
                        // Preserve original caret position
                        int selStart = _editTextBox.SelectionStart;
                        _editTextBox.SelectionStart = selStart;
                    }
                }));
            }
        }

        private void DropdownForm_Shown(object sender, EventArgs e)
        {
            // When dropdown is shown, immediately set focus back to textbox
            if (_editTextBox != null && _isEditing)
            {
                BeginInvoke(new Action(() => {
                    _editTextBox.Focus();
                }));
            }
        }

        private void DropdownForm_Deactivate(object sender, EventArgs e)
        {
            // When dropdown loses focus, check if it's because we're focusing back on the textbox
            if (!_editTextBox.Focused && _isEditing)
            {
                BeginInvoke(new Action(() => {
                    if (_editTextBox != null && _isEditing)
                    {
                        _editTextBox.Focus();
                    }
                }));
            }
        }

        /// <summary>
        /// Hides the dropdown list
        /// </summary>
        public void HideDropdown()
        {
            if (!_isDropdownOpen || _dropdownListForm == null)
                return;

            // Unsubscribe from dropdown events
            _dropdownListForm.Deactivate -= DropdownForm_Deactivate;
            _dropdownListForm.Shown -= DropdownForm_Shown;

            _dropdownListForm.Hide();
            _isDropdownOpen = false;
            OnDropdownClosed(EventArgs.Empty);

            // Return focus to the textbox
            BeginInvoke(new Action(() => {
                if (_editTextBox != null && _isEditing)
                {
                    _editTextBox.Focus();
                    // Preserve the caret position
                    int currentPos = _editTextBox.SelectionStart;
                    _editTextBox.SelectionStart = currentPos;
                }
            }));
        }

        /// <summary>
        /// Sets the dropdown items from a list of strings
        /// </summary>
        public void SetItems(IEnumerable<string> itemTexts)
        {
            _items.Clear();
            foreach (string text in itemTexts)
            {
                _items.Add(new SimpleItem { Text = text });
            }
        }

        /// <summary>
        /// Sets the dropdown items from a DataTable
        /// </summary>
        /// <param name="dataTable">The data source</param>
        /// <param name="displayMember">Column name to display</param>
        /// <param name="valueMember">Column name to use as value (optional)</param>
        public void SetItems(DataTable dataTable, string displayMember, string valueMember = null)
        {
            _items.Clear();

            foreach (DataRow row in dataTable.Rows)
            {
                string displayText = row[displayMember]?.ToString() ?? string.Empty;

                SimpleItem item = new SimpleItem { Text = displayText };

                // If valueMember is specified, set it as SubText (or another property)
                if (!string.IsNullOrEmpty(valueMember) && dataTable.Columns.Contains(valueMember))
                {
                    item.SubText = row[valueMember]?.ToString() ?? string.Empty;
                }

                _items.Add(item);
            }
        }

        #endregion
        #region Key Events
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.KeyValue == (int)Keys.Enter)
            {
                // Handle Enter key to select the current item
                if (_isDropdownOpen)
                {
                    HideDropdown();
                    e.Handled = true;
                }
            }
            if (e.KeyValue == (int)Keys.Escape)
            {
                // Handle Escape key to close the dropdown
                if (_isDropdownOpen)
                {
                    HideDropdown();
                    e.Handled = true;
                }
            }
            // hide dropdown if the user presses Tab and leave the control
            if (e.KeyCode == Keys.Tab && _isDropdownOpen)
            {
                HideDropdown();
                e.Handled = true;
            }
            // Handle keyboard navigation for the dropdown
            if (_items.Count > 0)
            {
                if (e.KeyCode == Keys.Down && !_isDropdownOpen)
                {
                    ShowDropdown();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape && _isDropdownOpen)
                {
                    HideDropdown();
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region IBeepComponent Implementation
        public override void SetValue(object value)
        {
          
            if (value is SimpleItem item)
            {
                this.SelectedItem = item;
                this.Text = item.Text;
            }
            else
            {
                this.Text = value?.ToString();
            }
            _editTextBox.Text = this.Text;
        }

        public override object GetValue()
        {
            return Text;
        }

        public override void ClearValue()
        {
            Text = string.Empty;
        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            DrawControl(graphics, rectangle);
        }
        #endregion
    }
   

}
