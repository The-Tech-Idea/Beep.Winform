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
        private BeepButton _decrementButton;
        private BeepButton _incrementButton;
        private BeepTextBox _valueTextBox;

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
                UpdateTextBox();
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
                UpdateTextBox();
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
                    UpdateTextBox();
                    
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
                UpdateTextBox();
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
                UpdateTextBox();
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
                UpdateTextBox();
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
                UpdateTextBox();
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
                UpdateTextBox();
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
                UpdateTextBox();
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
                if (_valueTextBox != null)
                    _valueTextBox.ReadOnly = value;
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
                UpdateLayout(DrawingRect);
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
                if (_decrementButton != null)
                    _decrementButton.Visible = value;
                if (_incrementButton != null)
                    _incrementButton.Visible = value;
                UpdateLayout(DrawingRect);
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

        // Child control access
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTextBox TextBox => _valueTextBox;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepButton UpButton => _incrementButton;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepButton DownButton => _decrementButton;
        #endregion

        #region Constructor
        public BeepNumericUpDown() : base()
        {
            // Enable smooth resizing
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            
            this.Size = DefaultSize;
            Margin = new Padding(0);
        
            // Initialize child controls
            InitializeControls();
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
        #endregion

        #region Initialization
        private void InitializeControls()
        {
            // Decrement Button (Left)
            _decrementButton = new BeepButton
            {
                Text = "−",  // Using minus sign instead of hyphen
                IsFrameless = true,
                AutoSize = false,
                IsChild = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _decrementButton.Click += DecrementButton_Click;
            _decrementButton.MouseDown += SpinButton_MouseDown;
            _decrementButton.MouseUp += SpinButton_MouseUp;

            // Increment Button (Right)
            _incrementButton = new BeepButton
            {
                Text = "+",
                IsFrameless = true,
                AutoSize = false,
                IsChild = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _incrementButton.Click += IncrementButton_Click;
            _incrementButton.MouseDown += SpinButton_MouseDown;
            _incrementButton.MouseUp += SpinButton_MouseUp;

            // Value TextBox (Center)
            _valueTextBox = new BeepTextBox
            {
                TextAlignment = HorizontalAlignment.Center,
                IsFrameless = true,
                OnlyDigits = false, // We'll handle validation ourselves
                IsChild = true,
                BorderStyle = BorderStyle.None
            };
            _valueTextBox.TextChanged += ValueTextBox_TextChanged;
            _valueTextBox.KeyDown += ValueTextBox_KeyDown;
            _valueTextBox.Enter += ValueTextBox_Enter;
            _valueTextBox.Leave += ValueTextBox_Leave;

            // Add controls to the parent
            Controls.Add(_decrementButton);
            Controls.Add(_valueTextBox);
            Controls.Add(_incrementButton);
            
            UpdateDrawingRect();
            UpdateLayout(DrawingRect);
            
            // Set initial display
            UpdateTextBox();
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
        private void UpdateLayout(Rectangle rect)
        {
            if (_valueTextBox == null) return;
            
            _valueTextBox.AutoSize = false;
            
            int padding = 2;
            int buttonWidth = _showSpinButtons ? GetButtonWidth() : 0;
            
            // Calculate available dimensions
            int availableWidth = rect.Width - 2 * padding;
            int availableHeight = rect.Height - 2 * padding;
            int elementHeight = availableHeight;
            int elementY = rect.Top + padding;

            if (_showSpinButtons)
            {
                // TextBox width = total width - 2 buttons - paddings
                int textBoxWidth = Math.Max(40, availableWidth - (2 * buttonWidth) - (2 * padding));
                
                // Layout Decrement Button (left)
                _decrementButton.Location = new Point(rect.Left + padding, elementY);
                _decrementButton.Size = new Size(buttonWidth, elementHeight);
                _decrementButton.Visible = true;

                // Layout TextBox (center)
                _valueTextBox.Location = new Point(rect.Left + padding + buttonWidth + padding, elementY);
                _valueTextBox.Size = new Size(textBoxWidth, elementHeight);

                // Layout Increment Button (right)
                _incrementButton.Location = new Point(_valueTextBox.Right + padding, elementY);
                _incrementButton.Size = new Size(buttonWidth, elementHeight);
                _incrementButton.Visible = true;
            }
            else
            {
                // Full width for text box
                _decrementButton.Visible = false;
                _incrementButton.Visible = false;
                
                _valueTextBox.Location = new Point(rect.Left + padding, elementY);
                _valueTextBox.Size = new Size(availableWidth, elementHeight);
            }
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            UpdateLayout(DrawingRect);
            Invalidate();
        }
        #endregion

        #region Drawing
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            UpdateLayout(rectangle);
            
            // Enable anti-aliasing for smooth rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Draw the text box
            if (_valueTextBox?.Visible == true && _valueTextBox.Width > 0 && _valueTextBox.Height > 0)
            {
                var valueTextBoxRect = new Rectangle(_valueTextBox.Left, _valueTextBox.Top, _valueTextBox.Width, _valueTextBox.Height);
                _valueTextBox.Draw(graphics, valueTextBoxRect);
            }
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

        private void UpdateTextBox()
        {
            if (_valueTextBox == null) return;

            string newText = FormatValue(_value);
            if (_valueTextBox.Text != newText)
            {
                _valueTextBox.TextChanged -= ValueTextBox_TextChanged;
                _valueTextBox.Text = newText;
                _valueTextBox.TextChanged += ValueTextBox_TextChanged;
            }
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
            
            if (!string.IsNullOrEmpty(_prefix))
                cleanText = cleanText.StartsWith(_prefix) ? cleanText.Substring(_prefix.Length) : cleanText;
                
            if (!string.IsNullOrEmpty(_suffix))
                cleanText = cleanText.EndsWith(_suffix) ? cleanText.Substring(0, cleanText.Length - _suffix.Length) : cleanText;

            // Remove thousands separators
            cleanText = cleanText.Replace(",", "").Replace(" ", "");

            if (decimal.TryParse(cleanText, out decimal result))
            {
                return result;
            }

            return _value; // Return current value if parsing fails
        }
        #endregion

        #region Event Handlers
        private void IncrementButton_Click(object sender, EventArgs e)
        {
            PerformIncrement();
        }

        private void DecrementButton_Click(object sender, EventArgs e)
        {
            PerformDecrement();
        }

        private void SpinButton_MouseDown(object sender, MouseEventArgs e)
        {
            // Could implement auto-repeat functionality here
        }

        private void SpinButton_MouseUp(object sender, MouseEventArgs e)
        {
            // Stop auto-repeat if implemented
        }

        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_valueTextBox.ReadOnly) return;

            decimal newValue = ParseDisplayValue(_valueTextBox.Text);
            
            // Validate range
            if (newValue >= _minimumValue && newValue <= _maximumValue)
            {
                _value = newValue;
                
                // Remove any error highlighting
                if (_highlightInvalidInput)
                    _valueTextBox.BackColor = _currentTheme?.TextBoxBackColor ?? SystemColors.Window;
                    
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (_highlightInvalidInput)
            {
                // Highlight invalid input
                _valueTextBox.BackColor = _invalidInputColor;
            }
        }

        private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_interceptArrowKeys) return;

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
            }
        }

        private void ValueTextBox_Enter(object sender, EventArgs e)
        {
            if (_selectAllOnFocus)
            {
                _valueTextBox.SelectAll();
            }
        }

        private void ValueTextBox_Leave(object sender, EventArgs e)
        {
            // Ensure the display is properly formatted when focus is lost
            UpdateTextBox();
        }

        private void PerformIncrement(decimal? customIncrement = null)
        {
            decimal increment = customIncrement ?? _incrementValue;
            Value = _value + increment;
        }

        private void PerformDecrement(decimal? customDecrement = null)
        {
            decimal decrement = customDecrement ?? _incrementValue;
            Value = _value - decrement;
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
        #endregion

        #region Theme Integration
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null) return;

            // Apply theme to child controls
            _valueTextBox?.ApplyTheme();
            _decrementButton?.ApplyTheme();
            _incrementButton?.ApplyTheme();

            // Customize button appearance
            if (_decrementButton != null)
            {
                _decrementButton.BackColor = _currentTheme.ButtonBackColor;
                _decrementButton.ForeColor = _currentTheme.ButtonForeColor;
                _decrementButton.BorderColor = _currentTheme.BorderColor;
            }

            if (_incrementButton != null)
            {
                _incrementButton.BackColor = _currentTheme.ButtonBackColor;
                _incrementButton.ForeColor = _currentTheme.ButtonForeColor;
                _incrementButton.BorderColor = _currentTheme.BorderColor;
            }

            Invalidate();
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
                // Clean up event handlers
                if (_valueTextBox != null)
                {
                    _valueTextBox.TextChanged -= ValueTextBox_TextChanged;
                    _valueTextBox.KeyDown -= ValueTextBox_KeyDown;
                    _valueTextBox.Enter -= ValueTextBox_Enter;
                    _valueTextBox.Leave -= ValueTextBox_Leave;
                }

                if (_decrementButton != null)
                {
                    _decrementButton.Click -= DecrementButton_Click;
                    _decrementButton.MouseDown -= SpinButton_MouseDown;
                    _decrementButton.MouseUp -= SpinButton_MouseUp;
                }

                if (_incrementButton != null)
                {
                    _incrementButton.Click -= IncrementButton_Click;
                    _incrementButton.MouseDown -= SpinButton_MouseDown;
                    _incrementButton.MouseUp -= SpinButton_MouseUp;
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
