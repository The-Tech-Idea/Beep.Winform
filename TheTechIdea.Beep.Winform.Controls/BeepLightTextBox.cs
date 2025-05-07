using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Data;

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
        private string _displayText = string.Empty;
        private bool _passwordMode = false;
        private Color _textColor;
        private Size _maxImageSize = new Size(16, 16);
        private int _padding = 4;

        public new event EventHandler TextChanged;
        private int _lines = 3;
        private ScrollBars _scrollBars = ScrollBars.None;

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
        private EditActivation _editActivation = EditActivation.DoubleClick;
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
        [Description("The text shown in the control.")]
        public override string Text
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnTextChanged(EventArgs.Empty);
                    Invalidate();
                }
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
            Padding = new Padding(4);
            ShowAllBorders = true;
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            CanBeHovered = true;
            _textColor = ForeColor;

            // Add a click handler to start editing when clicked
            this.MouseClick += BeepLightTextBox_MouseClick;
            this.MouseDoubleClick += BeepLightTextBox_MouseDoubleClick;

            // Create but don't add the editing textbox yet
            InitializeEditTextBox();
            InitializeImageButton();
            InitializeDefaultErrorMessages();
            InitializeDropdown();
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
            _editTextBox.KeyDown += (s, e) =>
            {
                if (!_multiline && e.KeyCode == Keys.Enter) // Handle Enter key for single-line mode
                {
                    EndEditing(true);
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    EndEditing(false);
                    e.SuppressKeyPress = true;
                }
            };

            _editTextBox.KeyPress += (s, e) => HandleKeyPress(e);
            _editTextBox.TextChanged += (s, e) =>
            {
                if (!_isApplyingMask)
                {
                    _isApplyingMask = true;
                    ApplyMaskFormat();
                    _isApplyingMask = false;
                }

                TextChanged?.Invoke(this, EventArgs.Empty);
            };
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

                // Correctly subscribe to the SelectedItemChanged event with proper event args type
                _dropdownListForm.SelectedItemChanged += (s, e) =>
                {
                    // e is SelectedItemChangedEventArgs which has a SelectedItem property
                    if (e.SelectedItem != null)
                    {
                        SelectedItem = e.SelectedItem;
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

        /// <summary>
        /// Programmatically start editing the text box
        /// </summary>
        public void StartEditing()
        {
            if (_isEditing || ReadOnly)
                return;

            _isEditing = true;

            // Position and configure the edit box
            AdjustEditTextBoxSize();
            _editTextBox.Text = _displayText;

            // Add to controls and make visible
            Controls.Add(_editTextBox);
            _editTextBox.Visible = true;
            _editTextBox.Focus();
            _editTextBox.SelectAll();

            Invalidate();
        }
        private void AdjustEditTextBoxSize()
        {
            if (_editTextBox == null)
                return;

            // Always ensure WordWrap is correctly set based on multiline mode
            _editTextBox.WordWrap = _multiline && _wordWrap;

            if (_multiline)
            {
                _editTextBox.Location = new Point(_padding, _padding);
                _editTextBox.Size = new Size(Width - (_padding * 2), Height - (_padding * 2));
            }
            else
            {
                _editTextBox.Location = new Point(_padding, (Height - _editTextBox.Height) / 2);
                _editTextBox.Size = new Size(Width - (_padding * 2) - (_imageButton?.Width ?? 0), _editTextBox.PreferredHeight);
            }
        }



        /// <summary>
        /// End the editing process, optionally saving the changes
        /// </summary>
        /// <param name="saveChanges">Whether to save the changes or revert to original text</param>
        public void EndEditing(bool saveChanges)
        {
            if (!_isEditing)
                return;

            _isEditing = false;

            if (saveChanges)
            {
                string newValue = _editTextBox.Text;
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
                    // Raise the Validating event to allow custom validation
                    var args = new ValidationEventArgs { Value = newValue, IsValid = true };
                    Validating?.Invoke(this, args);

                    if (args.Cancel)
                    {
                        isValid = false;
                        message = args.Message;
                    }
                    else if (!ValidateByFormat(newValue, out string formatMessage))
                    {
                        isValid = false;
                        message = formatMessage;
                    }
                }

                // Update validation state
                IsValid = isValid;
                ValidationMessage = message;

                // If validation failed, show tooltip and keep editing if indicator enabled
                if (!isValid)
                {
                    ShowValidationTooltip(message);

                    if (_showValidationIndicator)
                    {
                        _isEditing = true;
                        _editTextBox.Focus();
                        return;
                    }
                }
                else
                {
                    // Hide tooltip if validation passed
                    if (_toolTip != null)
                        _toolTip.Hide(this);

                    Text = newValue;
                }
            }

            Controls.Remove(_editTextBox);
            _editTextBox.Visible = false;

            Invalidate();
            Focus();
        }


        #endregion

        #region Drawing and Layout
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            PositionImageButton();
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

            // Don't draw text content when editing as the TextBox will show it
            if (_isEditing)
                return;

            // Draw text
            string textToDraw = string.IsNullOrEmpty(_displayText) ? _placeholderText : _displayText;
            Color textColor = string.IsNullOrEmpty(_displayText) ? Color.Gray : (Enabled ? _textColor : DisabledForeColor);

            // If validation failed and we're showing indicators, draw with error styling
            if (!_isValid && _showValidationIndicator)
            {
                // Use error colors from theme if available
                textColor = _currentTheme?.TextBoxErrorForeColor ?? Color.Red;

                // Draw error border
                using (Pen errorPen = new Pen(_currentTheme?.TextBoxErrorBorderColor ?? Color.Red, 1))
                {
                    if (IsRounded)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(DrawingRect, BorderRadius))
                        {
                            g.DrawPath(errorPen, path);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(errorPen, DrawingRect);
                    }
                }
            }

            // Calculate text layout
            Rectangle textRect = DrawingRect;
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

            // If password mode, draw masked text
            if (_passwordMode && !string.IsNullOrEmpty(_displayText))
            {
                textToDraw = new string('•', _displayText.Length);
            }

            // Draw the actual text
            TextRenderer.DrawText(g, textToDraw, _textFont, textRect, textColor, flags);

            // Draw validation icon if needed
            if (!_isValid && _showValidationIndicator && !_isEditing)
            {
                // Draw an error icon (e.g., small red X or exclamation mark)
                int iconSize = 16;
                int iconX = Width - iconSize - _padding;
                int iconY = (Height - iconSize) / 2;

                // Draw a simple error indicator
                using (Brush errorBrush = new SolidBrush(Color.Red))
                {
                    g.FillEllipse(errorBrush, iconX, iconY, iconSize, iconSize);
                }
                using (Pen whitePen = new Pen(Color.White, 2))
                {
                    // Draw an X or !
                    g.DrawLine(whitePen, iconX + 5, iconY + 5, iconX + iconSize - 5, iconY + iconSize - 5);
                    g.DrawLine(whitePen, iconX + iconSize - 5, iconY + 5, iconX + 5, iconY + iconSize - 5);
                }
            }

            // If we have items, draw a dropdown indicator
            if (_items.Count > 0 && _imageButton == null)
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


        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            BackColor = _currentTheme.TextBoxBackColor;
            ForeColor = _currentTheme.TextBoxForeColor;
            TextColor = _currentTheme.TextBoxForeColor;
            BorderColor = _currentTheme.TextBoxBorderColor;

            if (_editTextBox != null)
            {
                _editTextBox.BackColor = _currentTheme.TextBoxBackColor;
                _editTextBox.ForeColor = _currentTheme.TextBoxForeColor;
            }

            if (_imageButton != null)
            {
                _imageButton.Theme = Theme;
                _imageButton.BackColor = _currentTheme.TextBoxBackColor;
                _imageButton.ForeColor = _currentTheme.TextBoxForeColor;
                _imageButton.IsChild = true;
            }

            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                Font = _textFont;
                if (_editTextBox != null)
                {
                    _editTextBox.Font = _textFont;
                }
            }

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
        private bool _isApplyingMask = false;
        private bool _showValidationIndicator = true;
        private bool _isValid = true;
        private string _validationMessage = string.Empty;

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

        protected virtual void OnValidationStateChanged(EventArgs e)
        {
            ValidationStateChanged?.Invoke(this, e);
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
        public bool ValidateData(out string message)
        {
            // First check if the field is required
            if (_isRequired && string.IsNullOrWhiteSpace(Text))
            {
                message = _requiredErrorMessage;
                IsValid = false;
                ValidationMessage = message;

                if (_showTooltipOnValidationError)
                {
                    ShowValidationTooltip(message);
                }

                return false;
            }

            // Check if there's any custom validation
            var args = new ValidationEventArgs { Value = Text, IsValid = true };
            Validating?.Invoke(this, args);

            if (args.Cancel)
            {
                message = args.Message;
                IsValid = false;
                ValidationMessage = message;

                if (_showTooltipOnValidationError)
                {
                    ShowValidationTooltip(message);
                }

                return false;
            }

            // Then perform built-in validation
            bool isValid = ValidateByFormat(Text, out message);
            IsValid = isValid;

            if (!isValid)
            {
                ValidationMessage = message;

                if (_showTooltipOnValidationError)
                {
                    ShowValidationTooltip(message);
                }
            }
            else
            {
                // Hide tooltip if validation passed
                if (_toolTip != null)
                {
                    _toolTip.Hide(this);
                }
            }

            return isValid;
        }



        private void HandleKeyPress(KeyPressEventArgs e)
        {
            // Retrieve the current culture's decimal and group separators
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

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
        }

        // Format-specific methods (similar to BeepTextBox)
        private void ApplyCurrencyFormat()
        {
            if (decimal.TryParse(_editTextBox.Text, out decimal value))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = value.ToString("C2");
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        private void ApplyPercentageFormat()
        {
            if (decimal.TryParse(_editTextBox.Text, out decimal value))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = value.ToString("P2");
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
        }

        // Add implementations for all other format types similar to BeepTextBox
        // ...

        // Example of date formatting
        private void ApplyDateFormat()
        {
            if (DateTime.TryParse(_editTextBox.Text, out DateTime date))
            {
                int selectionStart = _editTextBox.SelectionStart;
                _editTextBox.Text = date.ToString(_dateFormat);
                _editTextBox.SelectionStart = Math.Min(selectionStart, _editTextBox.Text.Length);
            }
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
        private bool ValidateEmail(string value, out string message)
        {
            message = string.Empty;
            try
            {
                // Use a more comprehensive regex for email validation
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(value, pattern))
                {
                    message = "Please enter a valid email address.";
                    return false;
                }
                return true;
            }
            catch
            {
                message = "Invalid email format.";
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

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Hide any tooltips when the mouse leaves
            if (_toolTip != null)
            {
                _toolTip.Hide(this);
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            // Hide dropdown if it's open and focus is not on the dropdown
            if (_isDropdownOpen && !_dropdownListForm.ContainsFocus && !_dropdownListForm.Focused)
            {
                HideDropdown();
            }
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

            // Set size and position
            int width = _maxDropdownWidth > 0 ? _maxDropdownWidth : Width;

            _dropdownListForm.MaximumSize = new Size(
                width,
                _maxDropdownHeight
            );

            // Show the dropdown
            SimpleItem selectedItem = _dropdownListForm.ShowPopup(
                "Select an item",
                this,
                _dropdownPosition,
                false  // Don't show title
            );

            _isDropdownOpen = true;
            OnDropdownOpened(EventArgs.Empty);

            // If item was selected (not closed without selection)
            if (selectedItem != null)
            {
                SelectedItem = selectedItem;
            }

            _isDropdownOpen = false;
            OnDropdownClosed(EventArgs.Empty);
        }

        /// <summary>
        /// Hides the dropdown list
        /// </summary>
        public void HideDropdown()
        {
            if (!_isDropdownOpen || _dropdownListForm == null)
                return;

            _dropdownListForm.Close();
            _isDropdownOpen = false;
            OnDropdownClosed(EventArgs.Empty);
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
            this.Text = value?.ToString();
        }

        public override object GetValue()
        {
            return Text;
        }

        public override void ClearValue()
        {
            Text = string.Empty;
        }
        #endregion
    }
}
