using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    public enum NumericUpDownDisplayMode
    {
        Standard,
        Percentage,
        Currency,
        CustomUnit,
        ProgressValue
    }

    public enum NumericSpinButtonSize
    {
        Small,
        Standard,
        Large,
        ExtraLarge
    }

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Numeric UpDown")]
    [Description("A custom numeric up-down control with Beep theming, optimized for business applications.")]
    public partial class BeepNumericUpDown : BaseControl
    {
        #region Internal Fields (exposed to partial classes)
        internal TextBox _textBox;
        internal bool _isEditing = false;
        internal Rectangle _upButtonRect;
        internal Rectangle _downButtonRect;
        internal bool _upButtonPressed;
        internal bool _downButtonPressed;
        internal bool _upButtonHovered;
        internal bool _downButtonHovered;
        internal Timer _repeatTimer;
        internal int _repeatCount;
        internal const int INITIAL_DELAY = 500;
        internal const int REPEAT_DELAY = 50;
        internal const int MAX_REPEAT_SPEED = 10;
        #endregion

        #region Private Fields
        private decimal _minimumValue = 0m;
        private decimal _maximumValue = 100m;
        private decimal _incrementValue = 1m;
        private decimal _value = 0m;
        private int _decimalPlaces = 0;
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
        private bool _highlightInvalidInput = true;
        private Color _invalidInputColor = Color.FromArgb(254, 202, 202);
        private bool _showSpinButtons = true;
        private bool _animateValueChanges = false;
        private bool _isPercentageMode = false;
        private bool _autoCalculateFromTotal = false;
        private decimal _totalValue = 0m;
        #endregion

        #region Events
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
                Invalidate();
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
                Invalidate();
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
                    var args = new ValueValidatingEventArgs(oldValue, newValue);
                    ValueValidating?.Invoke(this, args);
                    
                    if (args.Cancel)
                    {
                        ValueValidationFailed?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    _value = newValue;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    
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
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                _decimalPlaces = Math.Max(0, Math.Min(value, 15));
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue(NumericUpDownDisplayMode.Standard)]
        public NumericUpDownDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                _displayMode = value;
                ConfigureForDisplayMode();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue("")]
        public string Prefix
        {
            get => _prefix;
            set { _prefix = value ?? ""; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue("")]
        public string Suffix
        {
            get => _suffix;
            set { _suffix = value ?? ""; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue("")]
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value ?? "";
                if (!string.IsNullOrEmpty(_unit))
                    _suffix = $" {_unit}";
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Format")]
        [DefaultValue(false)]
        public bool ThousandsSeparator
        {
            get => _thousandsSeparator;
            set { _thousandsSeparator = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
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
        public bool InterceptArrowKeys
        {
            get => _interceptArrowKeys;
            set => _interceptArrowKeys = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
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
        [Category("Appearance")]
        [Description("The accent color used for focused state and spin buttons.")]
        public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 212);

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("When true, uses theme colors from the current theme.")]
        public bool UseThemeColors { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool WrapValues
        {
            get => _wrapValues;
            set => _wrapValues = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool SelectAllOnFocus
        {
            get => _selectAllOnFocus;
            set => _selectAllOnFocus = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(NumericSpinButtonSize.Standard)]
        public NumericSpinButtonSize ButtonSize
        {
            get => _buttonSize;
            set
            {
                _buttonSize = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowSpinButtons
        {
            get => _showSpinButtons;
            set
            {
                _showSpinButtons = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [DefaultValue(false)]
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
        public bool AutoCalculateFromTotal
        {
            get => _autoCalculateFromTotal;
            set => _autoCalculateFromTotal = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("When true, displays a subtle shadow effect around the control.")]
        public bool EnableShadow { get; set; } = false;

        [Browsable(true)]
        [Category("Business")]
        [DefaultValue(0)]
        public decimal TotalValue
        {
            get => _totalValue;
            set => _totalValue = value;
        }
        #endregion

        #region Constructor
        public BeepNumericUpDown() : base()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);
            
            Size = new Size(120, 30);
            Margin = new Padding(0);
            
            BorderRadius = 4;
            ShowAllBorders = true;
            IsBorderAffectedByTheme = true;
            CanBeHovered = true;
            CanBeFocused = true;
            
            InitializeComponent();
        }

        partial void InitializeComponent();

        private void ConfigureForDisplayMode()
        {
            switch (_displayMode)
            {
                case NumericUpDownDisplayMode.Percentage:
                    _suffix = "%";
                    _decimalPlaces = 0;
                    if (_maximumValue == 1000m)
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
            }
        }

        private decimal ValidateValueRange(decimal value)
        {
            if (_wrapValues)
            {
                if (value > _maximumValue) return _minimumValue;
                if (value < _minimumValue) return _maximumValue;
                return value;
            }
            return Math.Max(_minimumValue, Math.Min(value, _maximumValue));
        }
        #endregion

        #region Public Methods
        public void PerformStep() => IncrementValueInternal();
        
        public void ConfigureForPercentage()
        {
            DisplayMode = NumericUpDownDisplayMode.ProgressValue;
            IsPercentageMode = true;
        }

        public void ConfigureForCurrency(string currencySymbol = "$")
        {
            DisplayMode = NumericUpDownDisplayMode.Currency;
            Prefix = currencySymbol;
            DecimalPlaces = 2;
            ThousandsSeparator = true;
        }

        public void ConfigureForTaskProgress()
        {
            DisplayMode = NumericUpDownDisplayMode.ProgressValue;
            MinimumValue = 0;
            MaximumValue = 100;
            IncrementValue = 5;
            DecimalPlaces = 0;
            Unit = "";
            IsPercentageMode = true;
        }

        public void SetPercentageValue(decimal part, decimal total)
        {
            if (total > 0)
            {
                _totalValue = total;
                Value = Math.Round(part / total * 100, _decimalPlaces);
            }
        }

        public decimal GetActualValue()
        {
            if (_isPercentageMode && _autoCalculateFromTotal && _totalValue > 0)
                return _value / 100m * _totalValue;
            return _value;
        }
        
        public override void SetValue(object value)
        {
            if (value == null) return;
            if (value is decimal d) Value = d;
            else if (decimal.TryParse(value.ToString(), out decimal parsed))
                Value = parsed;
        }

        public override object GetValue() => Value;
        #endregion

        #region Theme Integration
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            BackColor = _currentTheme.TextBoxBackColor;
            ForeColor = _currentTheme.TextBoxForeColor;
            BorderColor = _currentTheme.TextBoxBorderColor;
            HoverBackColor = _currentTheme.TextBoxHoverBackColor;
            HoverForeColor = _currentTheme.TextBoxHoverForeColor;
            HoverBorderColor = _currentTheme.TextBoxHoverBorderColor;
            
            if (_textBox != null)
            {
                _textBox.BackColor = _currentTheme.TextBoxBackColor;
                _textBox.ForeColor = _currentTheme.TextBoxForeColor;
                _textBox.Font = SystemFonts.DefaultFont;
            }
            
            _invalidInputColor = _currentTheme.ErrorColor;
            Invalidate();
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_textBox != null)
                {
                    _textBox.Dispose();
                    _textBox = null;
                }
                if (_repeatTimer != null)
                {
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

   