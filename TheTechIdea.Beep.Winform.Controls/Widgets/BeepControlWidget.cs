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
        ToggleSwitch,
        Slider,
        DropdownFilter,
        DatePicker,
        SearchBox,
        ButtonGroup,
        CheckboxGroup,
        RangeSelector,
        ColorPicker,
        NumberSpinner
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Control Widget")]
    [Category("Beep Widgets")]
    [Description("Interactive control widget with strongly-typed input models.")]
    public class BeepControlWidget : BaseControl
    {
        #region Fields
        private ControlWidgetStyle _style = ControlWidgetStyle.ToggleSwitch;
        private IWidgetPainter _painter;
        private string _title = "Control Title";
        private readonly ToggleValue _toggleValue = new ToggleValue();
        private readonly SliderValue _sliderValue = new SliderValue();
        private readonly RangeValue _rangeValue = new RangeValue();
        private readonly DateValue _dateValue = new DateValue();
        private readonly ColorValue _colorValue = new ColorValue();
        private readonly NumberValue _numberValue = new NumberValue();
        private string _selectedOption = string.Empty;
        private string _searchText = string.Empty;
        private List<string> _options = new List<string> { "Option 1", "Option 2", "Option 3" };
        private List<CheckboxOption> _checkboxOptions = new List<CheckboxOption>
        {
            new CheckboxOption { Label = "Option 1", IsChecked = false },
            new CheckboxOption { Label = "Option 2", IsChecked = false },
            new CheckboxOption { Label = "Option 3", IsChecked = false }
        };
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
        #endregion

        #region Events
        public event EventHandler<BeepEventDataArgs> ValueChanged;
        public event EventHandler<BeepEventDataArgs> ControlClicked;
        public event EventHandler<BeepEventDataArgs> OptionSelected;
        #endregion

        #region Constructor
        public BeepControlWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            Size = new Size(200, 60);
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
        [Description("Visual style of the control widget.")]
        public ControlWidgetStyle Style
        {
            get => _style;
            set
            {
                _style = value;
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
        [Description("Toggle switch value (for ToggleSwitch style).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToggleValue ToggleValue
        {
            get => _toggleValue;
            set
            {
                if (value == null) return;
                _toggleValue.IsOn = value.IsOn;
                Invalidate();
                OnValueChanged();
            }
        }

        [Category("Control")]
        [Description("Slider value (for Slider style).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SliderValue SliderValue
        {
            get => _sliderValue;
            set
            {
                if (value == null) return;
                _sliderValue.Value = value.Value;
                _sliderValue.MinValue = value.MinValue;
                _sliderValue.MaxValue = value.MaxValue;
                Invalidate();
                OnValueChanged();
            }
        }

        [Category("Control")]
        [Description("Range selector value (for RangeSelector style).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RangeValue RangeValue
        {
            get => _rangeValue;
            set
            {
                if (value == null) return;
                _rangeValue.MinValue = value.MinValue;
                _rangeValue.MaxValue = value.MaxValue;
                _rangeValue.CurrentValue = value.CurrentValue;
                Invalidate();
                OnValueChanged();
            }
        }

        [Category("Control")]
        [Description("Date picker value (for DatePicker style).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DateValue DateValue
        {
            get => _dateValue;
            set
            {
                if (value == null) return;
                _dateValue.Value = value.Value;
                _dateValue.MinDate = value.MinDate;
                _dateValue.MaxDate = value.MaxDate;
                Invalidate();
                OnValueChanged();
            }
        }

        [Category("Control")]
        [Description("Color picker value (for ColorPicker style).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ColorValue ColorValue
        {
            get => _colorValue;
            set
            {
                if (value == null) return;
                _colorValue.Value = value.Value;
                Invalidate();
                OnValueChanged();
            }
        }

        [Category("Control")]
        [Description("Number spinner value (for NumberSpinner style).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public NumberValue NumberValue
        {
            get => _numberValue;
            set
            {
                if (value == null) return;
                _numberValue.Value = value.Value;
                _numberValue.MinValue = value.MinValue;
                _numberValue.MaxValue = value.MaxValue;
                _numberValue.Step = value.Step;
                Invalidate();
                OnValueChanged();
            }
        }

        [Category("Control")]
        [Description("Selected option for dropdown/button styles.")]
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                _selectedOption = value ?? string.Empty;
                Invalidate();
                OptionSelected?.Invoke(this, new BeepEventDataArgs("OptionSelected", this) { EventData = _selectedOption });
            }
        }

        [Category("Control")]
        [Description("Search text for search style.")]
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value ?? string.Empty; Invalidate(); OnValueChanged(); }
        }

        [Category("Control")]
        [Description("Available options for selection controls.")]
        public List<string> Options
        {
            get => _options;
            set { _options = value ?? new List<string>(); Invalidate(); }
        }

        [Category("Control")]
        [Description("Checkbox options for checkbox group style.")]
        public List<CheckboxOption> CheckboxOptions
        {
            get => _checkboxOptions;
            set { _checkboxOptions = value ?? new List<CheckboxOption>(); Invalidate(); }
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
                Value = GetDisplayValue(),
                AccentColor = _accentColor,
                ShowHeader = _showLabel,
                IsInteractive = _isEnabled,
                CornerRadius = BorderRadius,
                MinValue = GetMinValue(),
                MaxValue = GetMaxValue(),
                CurrentValue = GetCurrentValue(),
                SelectedDate = _dateValue.Value,
                SelectedColor = _colorValue.Value
            };

            PopulateCheckedItems(ctx);

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void PopulateCheckedItems(WidgetContext ctx)
        {
            ctx.CheckedItems = new List<bool>();
            for (int i = 0; i < _checkboxOptions.Count; i++)
            {
                ctx.CheckedItems.Add(_checkboxOptions[i].IsChecked);
            }
        }

        private string GetDisplayValue()
        {
            switch (_style)
            {
                case ControlWidgetStyle.ToggleSwitch:
                    return _toggleValue.IsOn ? "On" : "Off";
                case ControlWidgetStyle.Slider:
                    return _sliderValue.Value.ToString("F2");
                case ControlWidgetStyle.DropdownFilter:
                    return _selectedOption;
                case ControlWidgetStyle.DatePicker:
                    return _dateValue.Value.ToString("MM/dd/yyyy");
                case ControlWidgetStyle.SearchBox:
                    return _searchText;
                case ControlWidgetStyle.RangeSelector:
                    return _rangeValue.CurrentValue.ToString("F2");
                case ControlWidgetStyle.ColorPicker:
                    return $"#{_colorValue.Value.R:X2}{_colorValue.Value.G:X2}{_colorValue.Value.B:X2}";
                case ControlWidgetStyle.NumberSpinner:
                    return _numberValue.Value.ToString("F2");
                default:
                    return string.Empty;
            }
        }

        private double GetMinValue()
        {
            switch (_style)
            {
                case ControlWidgetStyle.Slider:
                    return _sliderValue.MinValue;
                case ControlWidgetStyle.RangeSelector:
                    return _rangeValue.MinValue;
                case ControlWidgetStyle.NumberSpinner:
                    return _numberValue.MinValue.HasValue ? (double)_numberValue.MinValue.Value : 0d;
                default:
                    return 0d;
            }
        }

        private double GetMaxValue()
        {
            switch (_style)
            {
                case ControlWidgetStyle.Slider:
                    return _sliderValue.MaxValue;
                case ControlWidgetStyle.RangeSelector:
                    return _rangeValue.MaxValue;
                case ControlWidgetStyle.NumberSpinner:
                    return _numberValue.MaxValue.HasValue ? (double)_numberValue.MaxValue.Value : 100d;
                default:
                    return 100d;
            }
        }

        private decimal? GetCurrentValue()
        {
            switch (_style)
            {
                case ControlWidgetStyle.ToggleSwitch:
                    return _toggleValue.IsOn ? 1m : 0m;
                case ControlWidgetStyle.Slider:
                    return (decimal)_sliderValue.Value;
                case ControlWidgetStyle.RangeSelector:
                    return (decimal)_rangeValue.CurrentValue;
                case ControlWidgetStyle.NumberSpinner:
                    return _numberValue.Value;
                default:
                    return null;
            }
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

            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;

            _buttonBackColor = _currentTheme.ButtonBackColor;
            _buttonForeColor = _currentTheme.ButtonForeColor;
            _buttonBorderColor = _currentTheme.ButtonBorderColor;
            _buttonHoverBackColor = _currentTheme.ButtonHoverBackColor;
            _buttonHoverForeColor = _currentTheme.ButtonHoverForeColor;
            _buttonSelectedBackColor = _currentTheme.ButtonSelectedBackColor;
            _buttonSelectedForeColor = _currentTheme.ButtonSelectedForeColor;
            _buttonPressedBackColor = _currentTheme.ButtonPressedBackColor;

            _checkBoxBackColor = _currentTheme.CheckBoxBackColor;
            _checkBoxForeColor = _currentTheme.CheckBoxForeColor;
            _checkBoxCheckedBackColor = _currentTheme.CheckBoxCheckedBackColor;

            _accentColor = _currentTheme.AccentColor;
            _primaryColor = _currentTheme.PrimaryColor;
            _secondaryColor = _currentTheme.SecondaryColor;

            InitializePainter();
            Invalidate();
        }
    }
}
