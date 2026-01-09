using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum ControlWidgetStyle
    {
        ToggleSwitch,     // On/off toggle switches
        Slider,           // Range sliders
        DropdownFilter,   // Filter dropdowns
        DatePicker,       // Date/time selection
        SearchBox,        // Search input with suggestions
        ButtonGroup,      // Radio button groups
        CheckboxGroup,    // Multiple checkboxes
        RangeSelector,    // Min/max range selection
        ColorPicker,      // Color selection
        NumberSpinner     // Number input with up/down
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Control Widget")]
    [Category("Beep Widgets")]
    [Description("Interactive control widget with multiple input types.")]
    public class BeepControlWidget : BaseControl
    {
        #region Fields
        private ControlWidgetStyle _style = ControlWidgetStyle.ToggleSwitch;
        private IWidgetPainter _painter;
        private string _title = "Control Title";
        private object _value = false;
        private object _minValue = 0;
        private object _maxValue = 100;
        // Strongly-typed value properties
        private ToggleValue? _toggleValue;
        private SliderValue? _sliderValue;
        private RangeValue? _rangeValue;
        private DateValue? _dateValue;
        private ColorValue? _colorValue;
        private NumberValue? _numberValue;
        private List<string> _options = new List<string> { "Option 1", "Option 2", "Option 3" };
        private Color _accentColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Empty;
        private Color _buttonBackColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonBackColor ?? Color.Empty;
        private Color _buttonForeColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonForeColor ?? Color.Empty;
        private Color _buttonBorderColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonBorderColor ?? Color.Empty;
        private Color _buttonHoverBackColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonHoverBackColor ?? Color.Empty;
        private Color _buttonHoverForeColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonHoverForeColor ?? Color.Empty;
        private Color _buttonSelectedBackColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonSelectedBackColor ?? Color.Empty;
        private Color _buttonSelectedForeColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonSelectedForeColor ?? Color.Empty;
        private Color _buttonPressedBackColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ButtonPressedBackColor ?? Color.Empty;
        private Color _checkBoxBackColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.CheckBoxBackColor ?? Color.Empty;
        private Color _checkBoxForeColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.CheckBoxForeColor ?? Color.Empty;
        private Color _checkBoxCheckedBackColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.CheckBoxCheckedBackColor ?? Color.Empty;
        private Color _primaryColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.Empty;
        private Color _secondaryColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.SecondaryColor ?? Color.Empty;
        private bool _isEnabled = true;
        private bool _showLabel = true;

        // Events
        public event EventHandler<BeepEventDataArgs> ValueChanged;
        public event EventHandler<BeepEventDataArgs> ControlClicked;
        public event EventHandler<BeepEventDataArgs> OptionSelected;
        #endregion

        #region Constructor
        public BeepControlWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(200, 60);
            ApplyThemeToChilds = false;
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case ControlWidgetStyle.ToggleSwitch:
                    _painter = new ToggleSwitchPainter();
                    break;
                case ControlWidgetStyle.Slider:
                    _painter = new SliderPainter();
                    break;
                case ControlWidgetStyle.DropdownFilter:
                    _painter = new DropdownFilterPainter();
                    break;
                case ControlWidgetStyle.DatePicker:
                    _painter = new DatePickerPainter();
                    break;
                case ControlWidgetStyle.SearchBox:
                    _painter = new SearchBoxPainter();
                    break;
                case ControlWidgetStyle.ButtonGroup:
                    _painter = new ButtonGroupPainter();
                    break;
                case ControlWidgetStyle.CheckboxGroup:
                    _painter = new CheckboxGroupPainter();
                    break;
                case ControlWidgetStyle.RangeSelector:
                    _painter = new RangeSelectorPainter();
                    break;
                case ControlWidgetStyle.ColorPicker:
                    _painter = new ColorPickerPainter();
                    break;
                case ControlWidgetStyle.NumberSpinner:
                    _painter = new NumberSpinnerPainter();
                    break;
                default:
                    _painter = new ToggleSwitchPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Control")]
        [Description("Visual Style of the control widget.")]
        public ControlWidgetStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                // Clear typed values when style changes
                _toggleValue = null;
                _sliderValue = null;
                _rangeValue = null;
                _dateValue = null;
                _colorValue = null;
                _numberValue = null;
                InitializePainter();
                Invalidate();
            }
        }

        [Category("Control")]
        [Description("Title/label for the control.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Control")]
        [Description("Current value of the control (legacy object type).")]
        [Obsolete("Use typed value properties (ToggleValue, SliderValue, etc.) based on Style. This property will be removed in a future version.")]
        public object Value
        {
            get => _value;
            set { _value = value; SyncTypedValues(); Invalidate(); OnValueChanged(); }
        }

        [Category("Control")]
        [Description("Minimum value for range controls (legacy object type).")]
        [Obsolete("Use typed value properties (SliderValue, RangeValue, etc.) based on Style. This property will be removed in a future version.")]
        public object MinValue
        {
            get => _minValue;
            set { _minValue = value; SyncTypedValues(); Invalidate(); }
        }

        [Category("Control")]
        [Description("Maximum value for range controls (legacy object type).")]
        [Obsolete("Use typed value properties (SliderValue, RangeValue, etc.) based on Style. This property will be removed in a future version.")]
        public object MaxValue
        {
            get => _maxValue;
            set { _maxValue = value; SyncTypedValues(); Invalidate(); }
        }

        // Strongly-typed value properties
        [Category("Control")]
        [Description("Toggle switch value (for ToggleSwitch style).")]
        public ToggleValue? ToggleValue
        {
            get
            {
                if (_style != ControlWidgetStyle.ToggleSwitch) return null;
                if (_toggleValue == null) _toggleValue = new ToggleValue { IsOn = Convert.ToBoolean(_value) };
                return _toggleValue;
            }
            set
            {
                _toggleValue = value;
                if (value != null) { _value = value.IsOn; Invalidate(); OnValueChanged(); }
            }
        }

        [Category("Control")]
        [Description("Slider value (for Slider style).")]
        public SliderValue? SliderValue
        {
            get
            {
                if (_style != ControlWidgetStyle.Slider) return null;
                if (_sliderValue == null)
                {
                    _sliderValue = new SliderValue
                    {
                        Value = Convert.ToDouble(_value),
                        MinValue = Convert.ToDouble(_minValue),
                        MaxValue = Convert.ToDouble(_maxValue)
                    };
                }
                return _sliderValue;
            }
            set
            {
                _sliderValue = value;
                if (value != null)
                {
                    _value = value.Value;
                    _minValue = value.MinValue;
                    _maxValue = value.MaxValue;
                    Invalidate();
                    OnValueChanged();
                }
            }
        }

        [Category("Control")]
        [Description("Range selector value (for RangeSelector style).")]
        public RangeValue? RangeValue
        {
            get
            {
                if (_style != ControlWidgetStyle.RangeSelector) return null;
                if (_rangeValue == null)
                {
                    _rangeValue = new RangeValue
                    {
                        MinValue = Convert.ToDouble(_minValue),
                        MaxValue = Convert.ToDouble(_maxValue),
                        CurrentValue = Convert.ToDouble(_value)
                    };
                }
                return _rangeValue;
            }
            set
            {
                _rangeValue = value;
                if (value != null)
                {
                    _minValue = value.MinValue;
                    _maxValue = value.MaxValue;
                    _value = value.CurrentValue;
                    Invalidate();
                    OnValueChanged();
                }
            }
        }

        [Category("Control")]
        [Description("Date picker value (for DatePicker style).")]
        public DateValue? DateValue
        {
            get
            {
                if (_style != ControlWidgetStyle.DatePicker) return null;
                if (_dateValue == null) _dateValue = new DateValue { Value = _value is DateTime dt ? dt : DateTime.Now };
                return _dateValue;
            }
            set
            {
                _dateValue = value;
                if (value != null) { _value = value.Value; Invalidate(); OnValueChanged(); }
            }
        }

        [Category("Control")]
        [Description("Color picker value (for ColorPicker style).")]
        public ColorValue? ColorValue
        {
            get
            {
                if (_style != ControlWidgetStyle.ColorPicker) return null;
                if (_colorValue == null) _colorValue = new ColorValue { Value = _value is Color c ? c : Color.Black };
                return _colorValue;
            }
            set
            {
                _colorValue = value;
                if (value != null) { _value = value.Value; Invalidate(); OnValueChanged(); }
            }
        }

        [Category("Control")]
        [Description("Number spinner value (for NumberSpinner style).")]
        public NumberValue? NumberValue
        {
            get
            {
                if (_style != ControlWidgetStyle.NumberSpinner) return null;
                if (_numberValue == null)
                {
                    _numberValue = new NumberValue
                    {
                        Value = Convert.ToDecimal(_value),
                        MinValue = _minValue != null ? Convert.ToDecimal(_minValue) : null,
                        MaxValue = _maxValue != null ? Convert.ToDecimal(_maxValue) : null
                    };
                }
                return _numberValue;
            }
            set
            {
                _numberValue = value;
                if (value != null)
                {
                    _value = value.Value;
                    _minValue = value.MinValue;
                    _maxValue = value.MaxValue;
                    Invalidate();
                    OnValueChanged();
                }
            }
        }

        private void SyncTypedValues()
        {
            // Sync typed values when object values change
            switch (_style)
            {
                case ControlWidgetStyle.ToggleSwitch:
                    if (_toggleValue != null) _toggleValue.IsOn = Convert.ToBoolean(_value);
                    break;
                case ControlWidgetStyle.Slider:
                    if (_sliderValue != null)
                    {
                        _sliderValue.Value = Convert.ToDouble(_value);
                        _sliderValue.MinValue = Convert.ToDouble(_minValue);
                        _sliderValue.MaxValue = Convert.ToDouble(_maxValue);
                    }
                    break;
                case ControlWidgetStyle.RangeSelector:
                    if (_rangeValue != null)
                    {
                        _rangeValue.CurrentValue = Convert.ToDouble(_value);
                        _rangeValue.MinValue = Convert.ToDouble(_minValue);
                        _rangeValue.MaxValue = Convert.ToDouble(_maxValue);
                    }
                    break;
                case ControlWidgetStyle.DatePicker:
                    if (_dateValue != null) _dateValue.Value = _value is DateTime dt ? dt : DateTime.Now;
                    break;
                case ControlWidgetStyle.ColorPicker:
                    if (_colorValue != null) _colorValue.Value = _value is Color c ? c : Color.Black;
                    break;
                case ControlWidgetStyle.NumberSpinner:
                    if (_numberValue != null)
                    {
                        _numberValue.Value = Convert.ToDecimal(_value);
                        _numberValue.MinValue = _minValue != null ? Convert.ToDecimal(_minValue) : null;
                        _numberValue.MaxValue = _maxValue != null ? Convert.ToDecimal(_maxValue) : null;
                    }
                    break;
            }
        }

        [Category("Control")]
        [Description("Available options for selection controls.")]
        public List<string> Options
        {
            get => _options;
            set { _options = value ?? new List<string>(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the control.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Control")]
        [Description("Whether the control is enabled for interaction.")]
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; Invalidate(); }
        }

        [Category("Control")]
        [Description("Whether to show the label/title.")]
        public bool ShowLabel
        {
            get => _showLabel;
            set { _showLabel = value; Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _value?.ToString() ?? "",
                AccentColor = _accentColor,
                ShowHeader = _showLabel,
                IsInteractive = _isEnabled,
                CornerRadius = BorderRadius,
                
                // Control-specific typed properties with type conversions
                MinValue = Convert.ToDouble(_minValue),
                MaxValue = Convert.ToDouble(_maxValue),
                CurrentValue = _value != null ? Convert.ToDecimal(_value) : (decimal?)null
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Control", ctx.ContentRect, null, () =>
                {
                    ControlClicked?.Invoke(this, new BeepEventDataArgs("ControlClicked", this));
                });
            }
        }

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new BeepEventDataArgs("ValueChanged", this));
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply control-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update button colors
            _buttonBackColor = _currentTheme.ButtonBackColor;
            _buttonForeColor = _currentTheme.ButtonForeColor;
            _buttonBorderColor = _currentTheme.ButtonBorderColor;
            _buttonHoverBackColor = _currentTheme.ButtonHoverBackColor;
            _buttonHoverForeColor = _currentTheme.ButtonHoverForeColor;
            _buttonSelectedBackColor = _currentTheme.ButtonSelectedBackColor;
            _buttonSelectedForeColor = _currentTheme.ButtonSelectedForeColor;
            _buttonPressedBackColor = _currentTheme.ButtonPressedBackColor;
            
            // Update checkbox colors if applicable
            _checkBoxBackColor = _currentTheme.CheckBoxBackColor;
            _checkBoxForeColor = _currentTheme.CheckBoxForeColor;
            _checkBoxCheckedBackColor = _currentTheme.CheckBoxCheckedBackColor;
            
            // Update accent colors
            _accentColor = _currentTheme.AccentColor;
            _primaryColor = _currentTheme.PrimaryColor;
            _secondaryColor = _currentTheme.SecondaryColor;
            
            InitializePainter();
            Invalidate();
        }
    }
}