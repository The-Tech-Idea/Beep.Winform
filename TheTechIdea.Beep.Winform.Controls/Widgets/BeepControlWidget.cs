using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
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
        private List<string> _options = new List<string> { "Option 1", "Option 2", "Option 3" };
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _buttonBackColor = Color.White;
        private Color _buttonForeColor = Color.Black;
        private Color _buttonBorderColor = Color.FromArgb(200, 200, 200);
        private Color _buttonHoverBackColor = Color.FromArgb(245, 245, 245);
        private Color _buttonHoverForeColor = Color.Black;
        private Color _buttonSelectedBackColor = Color.FromArgb(33, 150, 243);
        private Color _buttonSelectedForeColor = Color.White;
        private Color _buttonPressedBackColor = Color.FromArgb(25, 118, 210);
        private Color _checkBoxBackColor = Color.White;
        private Color _checkBoxForeColor = Color.Black;
        private Color _checkBoxCheckedBackColor = Color.FromArgb(33, 150, 243);
        private Color _primaryColor = Color.FromArgb(33, 150, 243);
        private Color _secondaryColor = Color.FromArgb(158, 158, 158);
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
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Control")]
        [Description("Title/label for the control.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Control")]
        [Description("Current value of the control.")]
        public object Value
        {
            get => _value;
            set { _value = value; Invalidate(); OnValueChanged(); }
        }

        [Category("Control")]
        [Description("Minimum value for range controls.")]
        public object MinValue
        {
            get => _minValue;
            set { _minValue = value; Invalidate(); }
        }

        [Category("Control")]
        [Description("Maximum value for range controls.")]
        public object MaxValue
        {
            get => _maxValue;
            set { _maxValue = value; Invalidate(); }
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
                CustomData = new Dictionary<string, object>
                {
                    ["MinValue"] = _minValue,
                    ["MaxValue"] = _maxValue,
                    ["Options"] = _options,
                    ["IsEnabled"] = _isEnabled,
                    ["CurrentValue"] = _value
                }
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