using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Metric;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum MetricWidgetStyle
    {
        SimpleValue,      // Just number and label
        ValueWithTrend,   // Number + trend indicator
        ProgressMetric,   // Number with progress bar
        GaugeMetric,      // Circular gauge display
        ComparisonMetric, // Two values side-by-side
        CardMetric        // Card-Style with icon
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Metric Widget")]
    [Category("Beep Widgets")]
    [Description("A metric/KPI display widget with multiple Style variations.")]
    public class BeepMetricWidget : BaseControl
    {
        #region Fields
        private MetricWidgetStyle _style = MetricWidgetStyle.SimpleValue;
        private IWidgetPainter _painter;
        private string _title = "Metric Title";
        private string _value = "1,234";
        private string _units = string.Empty;
        private string _trendValue = "+12.5%";
        private string _trendDirection = "up"; // "up", "down", "neutral"
        private double _trendPercentage = 12.5;
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _successColor = Color.FromArgb(76, 175, 80);
        private Color _warningColor = Color.FromArgb(255, 193, 7);
        private Color _errorColor = Color.FromArgb(244, 67, 54);
        private Color _trendColor = Color.Green;
        private bool _showTrend = false;
        private bool _showIcon = false;
        private string _iconPath = string.Empty;

        // Events - using BaseControl's built-in event system like BeepAppBar
        public event EventHandler<BeepEventDataArgs> ValueClicked;
        public event EventHandler<BeepEventDataArgs> TrendClicked;
        public event EventHandler<BeepEventDataArgs> IconClicked;
        #endregion

        #region Constructor
        public BeepMetricWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(200, 120);
            ApplyThemeToChilds = false;
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case MetricWidgetStyle.SimpleValue:
                    _painter = new SimpleValuePainter();
                    break;
                case MetricWidgetStyle.ValueWithTrend:
                    _painter = new TrendMetricPainter();
                    break;
                case MetricWidgetStyle.ProgressMetric:
                    _painter = new ProgressMetricPainter();
                    break;
                case MetricWidgetStyle.GaugeMetric:
                    _painter = new GaugeMetricPainter();
                    break;
                case MetricWidgetStyle.ComparisonMetric:
                    _painter = new ComparisonMetricPainter();
                    break;
                case MetricWidgetStyle.CardMetric:
                    _painter = new CardMetricPainter();
                    break;
                default:
                    _painter = new SimpleValuePainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Widget")]
        [Description("Visual Style of the metric widget.")]
        public MetricWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Widget")]
        [Description("Title/label for the metric.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Widget")]
        [Description("The main metric value to display.")]
        public string Value
        {
            get => _value;
            set { _value = value; Invalidate(); }
        }

        [Category("Widget")]
        [Description("Units for the metric value (e.g., '%', '$', 'users').")]
        public string Units
        {
            get => _units;
            set { _units = value; Invalidate(); }
        }

        [Category("Widget")]
        [Description("Trend value text (e.g., '+12.5%', '-3.2%').")]
        public string TrendValue
        {
            get => _trendValue;
            set { _trendValue = value; Invalidate(); }
        }

        [Category("Widget")]
        [Description("Trend direction: 'up', 'down', or 'neutral'.")]
        public string TrendDirection
        {
            get => _trendDirection;
            set { _trendDirection = value; UpdateTrendColor(); Invalidate(); }
        }

        [Category("Widget")]
        [Description("Trend percentage value for calculations.")]
        public double TrendPercentage
        {
            get => _trendPercentage;
            set { _trendPercentage = value; Invalidate(); }
        }

        [Category("Widget")]
        [Description("Whether to show the trend indicator.")]
        public bool ShowTrend
        {
            get => _showTrend;
            set { _showTrend = value; Invalidate(); }
        }

        [Category("Widget")]
        [Description("Whether to show an icon.")]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; Invalidate(); }
        }

        [Category("Widget")]
        [Description("Path to the icon image.")]
        public string IconPath
        {
            get => _iconPath;
            set { _iconPath = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Accent color for the widget.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for trend indicators.")]
        public Color TrendColor
        {
            get => _trendColor;
            set { _trendColor = value; Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Create widget context with current data
            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _value,
                Units = _units,
                TrendValue = _trendValue,
                TrendDirection = _trendDirection,
                TrendPercentage = _trendPercentage,
                AccentColor = _accentColor,
                TrendColor = _trendColor,
                ShowTrend = _showTrend,
                ShowIcon = _showIcon,
                IsInteractive = true,
                CornerRadius = BorderRadius
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            // Set up hit areas using BaseControl's system (like BeepAppBar)
            RefreshHitAreas(ctx);
            
            // Let painters register custom hit areas
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => 
            {
                // This will trigger the HitDetected event from BaseControl
                // No need for custom event args - use BaseControl's system
            });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            // Clear existing hit areas (like BeepAppBar does)
            ClearHitList();

            // Add hit areas for different parts of the widget
            if (!ctx.ValueRect.IsEmpty)
            {
                AddHitArea("Value", ctx.ValueRect, null, () =>
                {
                    ValueClicked?.Invoke(this, new BeepEventDataArgs("ValueClicked", this));
                });
            }

            if (ctx.ShowTrend && !ctx.TrendRect.IsEmpty)
            {
                AddHitArea("Trend", ctx.TrendRect, null, () =>
                {
                    TrendClicked?.Invoke(this, new BeepEventDataArgs("TrendClicked", this));
                });
            }

            if (ctx.ShowIcon && !ctx.IconRect.IsEmpty)
            {
                AddHitArea("Icon", ctx.IconRect, null, () =>
                {
                    IconClicked?.Invoke(this, new BeepEventDataArgs("IconClicked", this));
                });
            }
        }
        #endregion

        #region Helper Methods
        private void UpdateTrendColor()
        {
            switch (_trendDirection.ToLower())
            {
                case "up":
                    _trendColor = Color.Green;
                    break;
                case "down":
                    _trendColor = Color.Red;
                    break;
                default:
                    _trendColor = Color.Gray;
                    break;
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply card-specific theme colors for metric display
            BackColor = _currentTheme.CardBackColor;
            ForeColor = _currentTheme.CardTextForeColor;
            
            // Update accent color for progress bars and highlights
            _accentColor = _currentTheme.AccentColor;
            
            // Update status colors for different metric states
            _successColor = _currentTheme.SuccessColor;
            _warningColor = _currentTheme.WarningColor;
            _errorColor = _currentTheme.ErrorColor;
            
            InitializePainter();
            Invalidate();
        }
        #endregion
    }
}