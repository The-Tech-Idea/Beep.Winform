using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum NumericUpDownDisplayMode
    {
        Standard,        // Standard number display
        Percentage,      // Shows % symbol after value
        Currency,        // Shows currency symbol before value
        CustomUnit,      // Shows custom unit (prefix/suffix)
        ProgressValue    // Optimized for progress values (0-100)
    }

    public enum NumericSpinButtonSize
    {
        Small,    // 12px buttons
        Standard, // 16px buttons  
        Large,    // 20px buttons
        ExtraLarge // 24px buttons
    }

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Numeric UpDown")]
    [Description("A custom numeric up-down control with Beep theming, optimized for business applications.")]
    public class BeepNumericUpDown : BeepControl
    {
        #region Private Fields
        // Text box is only shown when editing
        private TextBox _textBox;
        private bool _isEditing = false;
        
        // Button areas and states
        private Rectangle _upButtonRect;
        private Rectangle _downButtonRect;
        private bool _upButtonPressed;
        private bool _downButtonPressed;
        private bool _upButtonHovered;
        private bool _downButtonHovered;
        private System.Windows.Forms.Timer _repeatTimer;
        private int _repeatCount;
        private const int INITIAL_DELAY = 500;
        private const int REPEAT_DELAY = 50;
        private const int MAX_REPEAT_SPEED = 10;

        private decimal _minimumValue = 0m;
        private decimal _maximumValue = 100m;
        private decimal _incrementValue = 1m;
        private decimal _value = 0m;
        private int _decimalPlaces = 0;
        
        // Enhanced features for business applications
        private NumericUpDownDisplayMode _displayMode = NumericUpDownDisplayMode.Standard;
        private NumericSpinButtonSize _buttonSize = NumericSpinButtonSize.Standard;
        private string _prefix = "";
        private string _suffix = "";
        private string _unit = "";
        private bool _thousandsSeparator = false;
        private bool _readOnly = false;
        private bool _interceptArrowKeys = true;
        private bool _allowNegative = true;
        private bool _wrapValues = false;
        private bool _selectAllOnFocus = true;
        
        // Visual enhancements
        private bool _highlightInvalidInput = true;
        private Color _invalidInputColor = Color.FromArgb(254, 202, 202);
        private bool _showSpinButtons = true;
        private bool _animateValueChanges = false;
        
        // Business-specific features
        private bool _isPercentageMode = false;
        private bool _autoCalculateFromTotal = false;
        private decimal _totalValue = 0m;
        
        // Events
        public event EventHandler ValueChanged;
        public event EventHandler<ValueValidatingEventArgs> ValueValidating;
        public event EventHandler MinimumReached;
        public event EventHandler MaximumReached;
        public event EventHandler UpButtonClicked;
        public event EventHandler DownButtonClicked;
        public event EventHandler ValueValidationFailed;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the minimum numeric value.")]
        public decimal MinimumValue
        {
            get => _minimumValue;
            set
            {
                _minimumValue = value;
                if (_value < _minimumValue)
                    Value = _minimumValue;
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the maximum numeric value.")]
        public decimal MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;
                if (_value > _maximumValue)
                    Value = _maximumValue;
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the amount by which the value changes.")]
        public decimal IncrementValue
        {
            get => _incrementValue;
            set => _incrementValue = value;
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the current numeric value.")]
        public decimal Value
        {
            get => _value;
            set
            {
                var newValue = ValidateValueRange(value);
                if (_value != newValue)
                {
                    var oldValue = _value;
                    
                    // Fire validation event
                    var args = new ValueValidatingEventArgs(oldValue, newValue);
                    ValueValidating?.Invoke(this, args);
                    
                    if (args.Cancel)
                    {
                        ValueValidationFailed?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    _value = newValue;
                    UpdateDisplayText();
                    
                    // Fire change events
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    
                    // Fire limit events
                    if (_value == _minimumValue)
                        MinimumReached?.Invoke(this, EventArgs.Empty);
                    if (_value == _maximumValue)
                        MaximumReached?.Invoke(this, EventArgs.Empty);
                    
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue(0)]
        [Description("Number of decimal places to display.")]
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                _decimalPlaces = Math.Max(0, Math.Min(value, 15));
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue(NumericUpDownDisplayMode.Standard)]
        [Description("Display mode for the numeric value.")]
        public NumericUpDownDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                _displayMode = value;
                ConfigureForDisplayMode();
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue("")]
        [Description("Text to display before the value.")]
        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value ?? "";
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue("")]
        [Description("Text to display after the value.")]
        public string Suffix
        {
            get => _suffix;
            set
            {
                _suffix = value ?? "";
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue("")]
        [Description("Unit of measurement to display.")]
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value ?? "";
                if (!string.IsNullOrEmpty(_unit))
                    _suffix = $" {_unit}";
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue(false)]
        [Description("Whether to display thousands separator.")]
        public bool ThousandsSeparator
        {
            get => _thousandsSeparator;
            set
            {
                _thousandsSeparator = value;
                UpdateDisplayText();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Whether the text box is read-only (buttons still work).")]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                if (_textBox != null)
                    _textBox.ReadOnly = value;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether arrow keys increment/decrement values.")]
        public bool InterceptArrowKeys
        {
            get => _interceptArrowKeys;
            set => _interceptArrowKeys = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether negative values are allowed.")]
        public bool AllowNegative
        {
            get => _allowNegative;
            set
            {
                _allowNegative = value;
                if (!_allowNegative && _minimumValue < 0)
                    MinimumValue = 0;
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Whether values wrap around from max to min and vice versa.")]
        public bool WrapValues
        {
            get => _wrapValues;
            set => _wrapValues = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether to select all text when the control gains focus.")]
        public bool SelectAllOnFocus
        {
            get => _selectAllOnFocus;
            set => _selectAllOnFocus = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(NumericSpinButtonSize.Standard)]
        [Description("Size of the spin buttons.")]
        public NumericSpinButtonSize ButtonSize
        {
            get => _buttonSize;
            set
            {
                _buttonSize = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Whether to show the spin buttons.")]
        public bool ShowSpinButtons
        {
            get => _showSpinButtons;
            set
            {
                _showSpinButtons = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [DefaultValue(false)]
        [Description("Optimized for percentage values (0-100).")]
        public bool IsPercentageMode
        {
            get => _isPercentageMode;
            set
            {
                _isPercentageMode = value;
                if (value)
                {
                    DisplayMode = NumericUpDownDisplayMode.Percentage;
                    MinimumValue = 0;
                    MaximumValue = 100;
                    DecimalPlaces = 0;
                }
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [DefaultValue(false)]
        [Description("Whether to auto-calculate percentage from total value.")]
        public bool AutoCalculateFromTotal
        {
            get => _autoCalculateFromTotal;
            set => _autoCalculateFromTotal = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [DefaultValue(0)]
        [Description("Total value for percentage calculations.")]
        public decimal TotalValue
        {
            get => _totalValue;
            set => _totalValue = value;
        }
        #endregion

        #region Constructor
        public BeepNumericUpDown() : base()
        {
            // Enable smooth resizing
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.Selectable, true);
            
            this.Size = DefaultSize;
            Margin = new Padding(0);
            
            // Set default visual properties
            BorderRadius = 4;
            ShowAllBorders = true;
            IsBorderAffectedByTheme = true;
            CanBeHovered = true;
            CanBeFocused = true;
        
            // Initialize the TextBox but don't add it to Controls yet
            InitializeTextBox();
            
            // Initialize button repeat timer
            _repeatTimer = new System.Windows.Forms.Timer();
            _repeatTimer.Tick += RepeatTimer_Tick;
            
            // Set initial layout
            UpdateLayout();
        }

        protected override Size DefaultSize => new Size(120, GetHeightForButtonSize(_buttonSize));

        private int GetHeightForButtonSize(NumericSpinButtonSize size)
        {
            return size switch
            {
                NumericSpinButtonSize.Small => 24,
                NumericSpinButtonSize.Standard => 30,
                NumericSpinButtonSize.Large => 36,
                NumericSpinButtonSize.ExtraLarge => 42,
                _ => 30
            };
        }
        
        private int GetButtonWidth()
        {
            return _buttonSize switch
            {
                NumericSpinButtonSize.Small => 20,
                NumericSpinButtonSize.Standard => 24,
                NumericSpinButtonSize.Large => 28,
                NumericSpinButtonSize.ExtraLarge => 32,
                _ => 24
            };
        }
        #endregion

        #region Initialization
        private void InitializeTextBox()
        {
            // Create text box for direct input
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Center,
                BackColor = Color.White,
                Visible = false  // Initially hidden
            };
            
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.KeyDown += TextBox_KeyDown;
            _textBox.LostFocus += TextBox_LostFocus;
            
            // Don't add to Controls yet - will be added when editing starts
        }

        private void ConfigureForDisplayMode()
        {
            switch (_displayMode)
            {
                case NumericUpDownDisplayMode.Percentage:
                    _suffix = "%";
                    _decimalPlaces = 0;
                    if (_maximumValue == 1000m) // Default max, change to 100 for percentage
                    {
                        _maximumValue = 100m;
                        _minimumValue = 0m;
                    }
                    break;
                    
                case NumericUpDownDisplayMode.Currency:
                    _prefix = "$";
                    _decimalPlaces = 2;
                    _thousandsSeparator = true;
                    break;
                    
                case NumericUpDownDisplayMode.ProgressValue:
                    _suffix = "%";
                    _minimumValue = 0m;
                    _maximumValue = 100m;
                    _decimalPlaces = 0;
                    _incrementValue = 1m;
                    break;
                    
                case NumericUpDownDisplayMode.Standard:
                default:
                    _prefix = "";
                    _suffix = "";
                    break;
            }
        }
        #endregion

        #region Layout
        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;
            
            int padding = 2;
            int buttonWidth = _showSpinButtons ? GetButtonWidth() : 0;
            
            // Calculate dimensions
            int clientWidth = Width - 2 * padding;
            int clientHeight = Height - 2 * padding;
            int textBoxWidth = clientWidth;
            
            // Calculate button areas for drawing and hit testing
            if (_showSpinButtons)
            {
                textBoxWidth = clientWidth - (2 * buttonWidth);
                
                // Button rectangles (used for painting and hit testing)
                _downButtonRect = new Rectangle(
                    padding, 
                    padding, 
                    buttonWidth, 
                    clientHeight);
                
                _upButtonRect = new Rectangle(
                    Width - padding - buttonWidth,
                    padding,
                    buttonWidth,
                    clientHeight);
            }
            else
            {
                // Clear button rectangles when buttons are hidden
                _downButtonRect = Rectangle.Empty;
                _upButtonRect = Rectangle.Empty;
            }
            
            // Update text box dimensions for when it becomes visible
            if (_textBox != null)
            {
                int textBoxLeft = _showSpinButtons ? padding + buttonWidth : padding;
                _textBox.Bounds = new Rectangle(
                    textBoxLeft, 
                    padding, 
                    textBoxWidth, 
                    clientHeight);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLayout();
        }
        #endregion

        #region Drawing
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawControl(e.Graphics);
        }
        
        private void DrawControl(Graphics g)
        {
            // Set up for high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Get theme colors
            Color backColor = _currentTheme?.TextBoxBackColor ?? SystemColors.Window;
            Color foreColor = _currentTheme?.TextBoxForeColor ?? SystemColors.ControlText;
            Color borderColor = _currentTheme?.TextBoxBorderColor ?? SystemColors.ControlDark;
            Color buttonBackColor = _currentTheme?.ButtonBackColor ?? SystemColors.Control;
            Color buttonForeColor = _currentTheme?.ButtonForeColor ?? SystemColors.ControlText;
            Color buttonHoverBackColor = _currentTheme?.ButtonHoverBackColor ?? SystemColors.ControlLight;
            Color buttonPressedBackColor = _currentTheme?.ButtonPressedBackColor ?? SystemColors.ControlDark;
            
            // Apply hover/focus state
            if (IsHovered)
                borderColor = _currentTheme?.TextBoxHoverBorderColor ?? SystemColors.Highlight;
            else if (IsFocused)
                borderColor = _currentTheme?.TextBoxBorderColor ?? SystemColors.Highlight;
            
            // Draw the background with rounded corners if specified
            using (var bgBrush = new SolidBrush(backColor))
            {
                var clientRect = new Rectangle(0, 0, Width, Height);
                
                if (IsRounded)
                {
                    using (var path = GetRoundedRectPath(clientRect, BorderRadius))
                    {
                        g.FillPath(bgBrush, path);
                        
                        // Draw border
                        using (var borderPen = new Pen(borderColor, 1))
                        {
                            g.DrawPath(borderPen, path);
                        }
                    }
                }
                else
                {
                    // Draw rectangle background and border
                    g.FillRectangle(bgBrush, clientRect);
                    using (var borderPen = new Pen(borderColor, 1))
                    {
                        g.DrawRectangle(borderPen, new Rectangle(0, 0, Width - 1, Height - 1));
                    }
                }
            }
            
            // Only draw the value text if we're not in edit mode (textbox visible)
            if (!_isEditing)
            {
                // Calculate text area
                int padding = 2;
                int buttonWidth = _showSpinButtons ? GetButtonWidth() : 0;
                int textBoxLeft = _showSpinButtons ? padding + buttonWidth : padding;
                int textWidth = Width - (2 * padding);
                
                if (_showSpinButtons)
                {
                    textWidth -= (2 * buttonWidth);
                }
                
                Rectangle textRect = new Rectangle(
                    textBoxLeft,
                    padding,
                    textWidth,
                    Height - (2 * padding));
                
                // Draw formatted value
                using (var textBrush = new SolidBrush(foreColor))
                using (var font = SystemFonts.DefaultFont)
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    
                    string formattedText = FormatValue(_value);
                    g.DrawString(formattedText, font, textBrush, textRect, format);
                }
            }
            
            // Draw buttons if enabled
            if (_showSpinButtons)
            {
                // Draw down button
                DrawButton(g, _downButtonRect, "−", 
                    _downButtonPressed, _downButtonHovered, 
                    buttonBackColor, buttonForeColor,
                    buttonHoverBackColor, buttonPressedBackColor);
                
                // Draw up button
                DrawButton(g, _upButtonRect, "+", 
                    _upButtonPressed, _upButtonHovered,
                    buttonBackColor, buttonForeColor,
                    buttonHoverBackColor, buttonPressedBackColor);
            }
        }
        
        private void DrawButton(Graphics g, Rectangle rect, string text, 
            bool pressed, bool hovered, 
            Color backColor, Color foreColor, 
            Color hoverBackColor, Color pressedBackColor)
        {
            // Select background color based on state
            Color bgColor = backColor;
            if (pressed)
                bgColor = pressedBackColor;
            else if (hovered)
                bgColor = hoverBackColor;
            
            // Draw button background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, rect);
            }
            
            // Draw button text
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe UI", 10f))
            {
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                
                g.DrawString(text, font, textBrush, rect, stringFormat);
            }
        }
        
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Main control drawing implementation
            DrawControl(graphics);
        }
        #endregion

        #region Value Management
        private decimal ValidateValueRange(decimal value)
        {
            if (_wrapValues)
            {
                if (value > _maximumValue)
                    return _minimumValue;
                if (value < _minimumValue)
                    return _maximumValue;
                return value;
            }
            
            return Math.Max(_minimumValue, Math.Min(value, _maximumValue));
        }

        private void UpdateDisplayText()
        {
            // If currently editing, update the textbox text
            if (_isEditing && _textBox != null)
            {
                string newText = FormatValue(_value);
                if (_textBox.Text != newText)
                {
                    _textBox.TextChanged -= TextBox_TextChanged;
                    _textBox.Text = newText;
                    _textBox.TextChanged += TextBox_TextChanged;
                }
            }
            
            // Always invalidate to redraw
            Invalidate();
        }

        private string FormatValue(decimal value)
        {
            string format = _decimalPlaces > 0 ? $"F{_decimalPlaces}" : "F0";
            
            if (_thousandsSeparator)
                format = _decimalPlaces > 0 ? $"N{_decimalPlaces}" : "N0";
            
            string formattedValue = value.ToString(format);
            
            return $"{_prefix}{formattedValue}{_suffix}";
        }

        private decimal ParseDisplayValue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return _minimumValue;

            // Remove prefix and suffix
            string cleanText = text.Trim();
            
            if (!string.IsNullOrEmpty(_prefix) && cleanText.StartsWith(_prefix))
                cleanText = cleanText.Substring(_prefix.Length);
                
            if (!string.IsNullOrEmpty(_suffix) && cleanText.EndsWith(_suffix))
                cleanText = cleanText.Substring(0, cleanText.Length - _suffix.Length);

            // Remove thousands separators
            cleanText = cleanText.Replace(",", "").Replace(" ", "");

            if (decimal.TryParse(cleanText, out decimal result))
            {
                return result;
            }

            return _value; // Return current value if parsing fails
        }
        #endregion

        #region Editing Mode
        private void StartEditing()
        {
            if (_readOnly || _isEditing)
                return;
            
            _isEditing = true;
            
            // Set up the text box
            _textBox.Text = FormatValue(_value);
            
            // Add it to the control now that we need it
            if (!Controls.Contains(_textBox))
            {
                Controls.Add(_textBox);
            }
            
            _textBox.Visible = true;
            _textBox.BringToFront();
            _textBox.Focus();
            
            if (_selectAllOnFocus)
            {
                _textBox.SelectAll();
            }
            
            Invalidate();
        }
        
        private void StopEditing(bool applyChanges)
        {
            if (!_isEditing)
                return;
            
            _isEditing = false;
            
            if (applyChanges)
            {
                // Apply the change if valid
                decimal newValue = ParseDisplayValue(_textBox.Text);
                if (newValue >= _minimumValue && newValue <= _maximumValue)
                {
                    Value = newValue;
                }
            }
            
            // Hide the text box
            _textBox.Visible = false;
            
            // Focus back to the control
            this.Focus();
            
            Invalidate();
        }
        #endregion

        #region Event Handlers
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (_textBox.ReadOnly) return;

            decimal newValue = ParseDisplayValue(_textBox.Text);
            
            // Validate range
            if (newValue >= _minimumValue && newValue <= _maximumValue)
            {
                _value = newValue;
                
                // Remove any error highlighting
                if (_highlightInvalidInput)
                    _textBox.BackColor = _currentTheme?.TextBoxBackColor ?? SystemColors.Window;
                    
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (_highlightInvalidInput)
            {
                // Highlight invalid input
                _textBox.BackColor = _invalidInputColor;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    StopEditing(true);
                    e.Handled = true;
                    e.SuppressKeyPress = true; // Prevents the beep
                    break;
                    
                case Keys.Escape:
                    StopEditing(false);
                    e.Handled = true;
                    break;
                    
                case Keys.Up:
                    if (_interceptArrowKeys)
                    {
                        PerformIncrement();
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.Down:
                    if (_interceptArrowKeys)
                    {
                        PerformDecrement();
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.PageUp:
                    if (_interceptArrowKeys)
                    {
                        PerformIncrement(_incrementValue * 10);
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.PageDown:
                    if (_interceptArrowKeys)
                    {
                        PerformDecrement(_incrementValue * 10);
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.Home:
                    if (_interceptArrowKeys)
                    {
                        Value = _minimumValue;
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.End:
                    if (_interceptArrowKeys)
                    {
                        Value = _maximumValue;
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            // When the textbox loses focus, stop editing and apply changes
            StopEditing(true);
        }
        
        // Mouse handling for buttons and clicks
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // Handle button clicks
            if (_showSpinButtons)
            {
                if (_upButtonRect.Contains(e.Location))
                {
                    _upButtonPressed = true;
                    PerformIncrement();
                    
                    // Start repeat timer for continuous press
                    _repeatCount = 0;
                    _repeatTimer.Interval = INITIAL_DELAY;
                    _repeatTimer.Start();
                    
                    Invalidate(_upButtonRect);
                    return; // Don't start editing if button was clicked
                }
                else if (_downButtonRect.Contains(e.Location))
                {
                    _downButtonPressed = true;
                    PerformDecrement();
                    
                    // Start repeat timer for continuous press
                    _repeatCount = 0;
                    _repeatTimer.Interval = INITIAL_DELAY;
                    _repeatTimer.Start();
                    
                    Invalidate(_downButtonRect);
                    return; // Don't start editing if button was clicked
                }
            }
            
            // If click wasn't on a button, start editing
            if (!_readOnly && !_isEditing)
            {
                StartEditing();
            }
        }
        
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            
            // Stop the repeat timer
            _repeatTimer.Stop();
            
            // Reset button states
            bool invalidateNeeded = _upButtonPressed || _downButtonPressed;
            _upButtonPressed = false;
            _downButtonPressed = false;
            
            if (invalidateNeeded)
            {
                Invalidate();
            }
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (_showSpinButtons)
            {
                bool oldUpHovered = _upButtonHovered;
                bool oldDownHovered = _downButtonHovered;
                
                _upButtonHovered = _upButtonRect.Contains(e.Location);
                _downButtonHovered = _downButtonRect.Contains(e.Location);
                
                if (oldUpHovered != _upButtonHovered || oldDownHovered != _downButtonHovered)
                {
                    Invalidate();
                }
            }
        }
        
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            bool invalidateNeeded = _upButtonHovered || _downButtonHovered;
            _upButtonHovered = false;
            _downButtonHovered = false;
            
            if (invalidateNeeded)
            {
                Invalidate();
            }
        }
        
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            
            // If not already editing and not read-only, start editing on double-click
            if (!_readOnly && !_isEditing && !_showSpinButtons)
            {
                StartEditing();
            }
        }
        
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            IsFocused = true;
            Invalidate();
        }
        
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            
            // Only set IsFocused to false if the textbox doesn't have focus
            if (!(_textBox?.Focused ?? false))
            {
                IsFocused = false;
                
                // If we're losing focus and not to the textbox, stop editing
                if (_isEditing && !(_textBox?.Focused ?? false))
                {
                    StopEditing(true);
                }
            }
            
            Invalidate();
        }
        
        // Timer for button repeat
        private void RepeatTimer_Tick(object sender, EventArgs e)
        {
            _repeatCount++;
            
            // Gradually speed up the repeat rate
            if (_repeatCount > 10 && _repeatTimer.Interval > REPEAT_DELAY)
            {
                _repeatTimer.Interval = Math.Max(REPEAT_DELAY, _repeatTimer.Interval - 50);
            }
            
            // Determine speed factor based on how long button has been pressed
            int speedFactor = Math.Min(MAX_REPEAT_SPEED, _repeatCount / 20 + 1);
            
            // Perform the appropriate action
            if (_upButtonPressed)
            {
                PerformIncrement(_incrementValue * speedFactor);
            }
            else if (_downButtonPressed)
            {
                PerformDecrement(_incrementValue * speedFactor);
            }
            else
            {
                _repeatTimer.Stop();
            }
        }

        private void PerformIncrement(decimal? customIncrement = null)
        {
            decimal increment = customIncrement ?? _incrementValue;
            Value = _value + increment;
            UpButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void PerformDecrement(decimal? customDecrement = null)
        {
            decimal decrement = customDecrement ?? _incrementValue;
            Value = _value - decrement;
            DownButtonClicked?.Invoke(this, EventArgs.Empty);
        }
        
        // Handle key events directly on the control
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (!_isEditing && _interceptArrowKeys)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        PerformIncrement();
                        e.Handled = true;
                        break;
                        
                    case Keys.Down:
                        PerformDecrement();
                        e.Handled = true;
                        break;
                        
                    case Keys.PageUp:
                        PerformIncrement(_incrementValue * 10);
                        e.Handled = true;
                        break;
                        
                    case Keys.PageDown:
                        PerformDecrement(_incrementValue * 10);
                        e.Handled = true;
                        break;
                        
                    case Keys.Home:
                        Value = _minimumValue;
                        e.Handled = true;
                        break;
                        
                    case Keys.End:
                        Value = _maximumValue;
                        e.Handled = true;
                        break;
                        
                    case Keys.Enter:
                    case Keys.Space:
                    case Keys.F2:
                        if (!_readOnly)
                        {
                            StartEditing();
                            e.Handled = true;
                        }
                        break;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs a single step increment.
        /// </summary>
        public void PerformStep()
        {
            PerformIncrement();
        }

        /// <summary>
        /// Sets up the control for percentage input (0-100).
        /// </summary>
        public void ConfigureForPercentage()
        {
            DisplayMode = NumericUpDownDisplayMode.ProgressValue;
            IsPercentageMode = true;
        }

        /// <summary>
        /// Sets up the control for currency input.
        /// </summary>
        public void ConfigureForCurrency(string currencySymbol = "$")
        {
            DisplayMode = NumericUpDownDisplayMode.Currency;
            Prefix = currencySymbol;
            DecimalPlaces = 2;
            ThousandsSeparator = true;
        }

        /// <summary>
        /// Sets up the control for task progress (perfect for TasksView integration).
        /// </summary>
        public void ConfigureForTaskProgress()
        {
            DisplayMode = NumericUpDownDisplayMode.ProgressValue;
            MinimumValue = 0;
            MaximumValue = 100;
            IncrementValue = 5; // 5% increments
            DecimalPlaces = 0;
            Unit = "";
            IsPercentageMode = true;
        }

        /// <summary>
        /// Sets value as percentage of total.
        /// </summary>
        public void SetPercentageValue(decimal part, decimal total)
        {
            if (total > 0)
            {
                _totalValue = total;
                Value = Math.Round((part / total) * 100, _decimalPlaces);
            }
        }

        /// <summary>
        /// Gets the actual value (useful for percentage calculations).
        /// </summary>
        public decimal GetActualValue()
        {
            if (_isPercentageMode && _autoCalculateFromTotal && _totalValue > 0)
            {
                return (_value / 100m) * _totalValue;
            }
            return _value;
        }
        
        /// <summary>
        /// Begins editing the value with the text box.
        /// </summary>
        public void BeginEdit()
        {
            if (!_readOnly && !_isEditing)
            {
                StartEditing();
            }
        }
        
        /// <summary>
        /// Ends editing and optionally applies the changes.
        /// </summary>
        public void EndEdit(bool applyChanges = true)
        {
            if (_isEditing)
            {
                StopEditing(applyChanges);
            }
        }
        
        /// <summary>
        /// Select all text in the control.
        /// </summary>
        public void SelectAll()
        {
            if (_isEditing && _textBox != null)
            {
                _textBox.SelectAll();
            }
            else if (!_readOnly)
            {
                StartEditing();
                _textBox?.SelectAll();
            }
        }
        #endregion

        #region Theme Integration
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null) return;

            // Set control background and border colors
            BackColor = _currentTheme.TextBoxBackColor;
            ForeColor = _currentTheme.TextBoxForeColor;
            BorderColor = _currentTheme.TextBoxBorderColor;
            
            // Set hover and focus states
            HoverBackColor = _currentTheme.TextBoxHoverBackColor;
            HoverForeColor = _currentTheme.TextBoxHoverForeColor;
            HoverBorderColor = _currentTheme.TextBoxHoverBorderColor;
            
            // Apply theme to text box
            if (_textBox != null)
            {
                _textBox.BackColor = _currentTheme.TextBoxBackColor;
                _textBox.ForeColor = _currentTheme.TextBoxForeColor;
                // Use system font instead of theme font
                _textBox.Font = SystemFonts.DefaultFont;
            }
            
            // Set the invalid input color based on theme
            _invalidInputColor = _currentTheme.ErrorColor;
            
            // Apply display mode specific styling
            ApplyDisplayModeTheming();

            Invalidate();
        }
        
        private void ApplyDisplayModeTheming()
        {
            // Apply specific theming for different display modes
            if (_textBox != null)
            {
                switch (_displayMode)
                {
                    case NumericUpDownDisplayMode.Currency:
                        // Add slightly different styling for currency display
                        _textBox.ForeColor = _currentTheme.TextBoxForeColor;
                        break;
                        
                    case NumericUpDownDisplayMode.Percentage:
                    case NumericUpDownDisplayMode.ProgressValue:
                        // Add slightly more accent-colored styling for percentage display
                        _textBox.TextAlign = HorizontalAlignment.Center;
                        break;
                }
            }
        }
        #endregion

        #region Value Setting and Getting
        public override void SetValue(object value)
        {
            if (value == null) return;

            decimal parsedValue = 0m;

            if (value is int intValue)
                parsedValue = Convert.ToDecimal(intValue);
            else if (value is long longValue)
                parsedValue = Convert.ToDecimal(longValue);
            else if (value is float floatValue)
                parsedValue = Convert.ToDecimal(floatValue);
            else if (value is double doubleValue)
                parsedValue = Convert.ToDecimal(doubleValue);
            else if (value is decimal decimalValue)
                parsedValue = decimalValue;
            else if (decimal.TryParse(value.ToString(), out decimal tempParsed))
                parsedValue = tempParsed;

            Value = parsedValue;
        }

        public override object GetValue()
        {
            return Value;
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up event handlers and resources
                if (_textBox != null)
                {
                    _textBox.TextChanged -= TextBox_TextChanged;
                    _textBox.KeyDown -= TextBox_KeyDown;
                    _textBox.LostFocus -= TextBox_LostFocus;
                    _textBox.Dispose();
                    _textBox = null;
                }
                
                if (_repeatTimer != null)
                {
                    _repeatTimer.Tick -= RepeatTimer_Tick;
                    _repeatTimer.Stop();
                    _repeatTimer.Dispose();
                    _repeatTimer = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    #region Event Args
    public class ValueValidatingEventArgs : EventArgs
    {
        public decimal OldValue { get; }
        public decimal NewValue { get; }
        public bool Cancel { get; set; }

        public ValueValidatingEventArgs(decimal oldValue, decimal newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Cancel = false;
        }
    }
    #endregion
}
