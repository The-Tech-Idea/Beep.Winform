using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum ChartWidgetStyle
    {
        BarChart,         // Vertical/horizontal bars
        LineChart,        // Line/area charts
        PieChart,         // Pie/donut charts
        GaugeChart,       // Gauge/speedometer
        Sparkline,        // Mini trend line
        HeatmapChart,     // Calendar/grid heatmap
        CombinationChart  // Multiple chart types
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Chart Widget")]
    [Category("Beep Widgets")]
    [Description("A chart/visualization widget with multiple chart types.")]
    public class BeepChartWidget : BaseControl
    {
        #region Fields
        private ChartWidgetStyle _style = ChartWidgetStyle.BarChart;
        private IWidgetPainter _painter;
        private string _title = "Chart Title";
        private List<double> _values = new List<double> { 10, 25, 30, 45, 20, 35 };
        private List<string> _labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
        private List<Color> _colors = new List<Color>(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ChartDefaultSeriesColors ?? new List<Color>());
        private Color _accentColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.Empty;
        private bool _showLegend = true;
        private bool _showGrid = true;
        private double _minValue = 0;
        private double _maxValue = 100;

        // Events
        public event EventHandler<BeepEventDataArgs> ChartClicked;
        public event EventHandler<BeepEventDataArgs> DataPointClicked;
        public event EventHandler<BeepEventDataArgs> LegendClicked;
        #endregion

        #region Constructor
        public BeepChartWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(300, 200);
            ApplyThemeToChilds = false;
            InitializeDefaultColors();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeDefaultColors()
        {
            // Prefer theme-provided chart colors; otherwise provide a sensible default palette
            if (_colors == null || _colors.Count == 0)
            {
                var themeColors = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ChartDefaultSeriesColors;
                if (themeColors != null && themeColors.Count > 0)
                {
                    _colors.AddRange(themeColors);
                }
                else
                {
                    _colors.AddRange(new[]
                    {
                        Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.Empty,
                        Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.SuccessColor ?? Color.Empty,
                        Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.WarningColor ?? Color.Empty,
                        Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ErrorColor ?? Color.Empty,
                        Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Empty,
                        Color.Empty,
                    });
                }
            }
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case ChartWidgetStyle.BarChart:
                    _painter = new BarChartPainter();
                    break;
                case ChartWidgetStyle.LineChart:
                    _painter = new LineChartPainter();
                    break;
                case ChartWidgetStyle.PieChart:
                    _painter = new PieChartPainter();
                    break;
                case ChartWidgetStyle.GaugeChart:
                    _painter = new GaugeChartPainter();
                    break;
                case ChartWidgetStyle.Sparkline:
                    _painter = new SparklinePainter();
                    break;
                case ChartWidgetStyle.HeatmapChart:
                    _painter = new HeatmapPainter();
                    break;
                case ChartWidgetStyle.CombinationChart:
                    _painter = new CombinationChartPainter();
                    break;
                default:
                    _painter = new BarChartPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Chart")]
        [Description("Visual Style of the chart widget.")]
        public ChartWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Chart")]
        [Description("Title of the chart.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Chart")]
        [Description("Data values for the chart.")]
        public List<double> Values
        {
            get => _values;
            set { _values = value ?? new List<double>(); Invalidate(); }
        }

        [Category("Chart")]
        [Description("Labels for data points.")]
        public List<string> Labels
        {
            get => _labels;
            set { _labels = value ?? new List<string>(); Invalidate(); }
        }

        [Category("Chart")]
        [Description("Colors for data series.")]
        public List<Color> Colors
        {
            get => _colors;
            set { _colors = value ?? new List<Color>(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the chart.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Chart")]
        [Description("Whether to show the legend.")]
        public bool ShowLegend
        {
            get => _showLegend;
            set { _showLegend = value; Invalidate(); }
        }

        [Category("Chart")]
        [Description("Whether to show grid lines.")]
        public bool ShowGrid
        {
            get => _showGrid;
            set { _showGrid = value; Invalidate(); }
        }

        [Category("Chart")]
        [Description("Minimum value for the chart scale.")]
        public double MinValue
        {
            get => _minValue;
            set { _minValue = value; Invalidate(); }
        }

        [Category("Chart")]
        [Description("Maximum value for the chart scale.")]
        public double MaxValue
        {
            get => _maxValue;
            set { _maxValue = value; Invalidate(); }
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
                Values = _values,
                Labels = _labels,
                Colors = _colors,
                AccentColor = _accentColor,
                ShowLegend = _showLegend,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                
                // Chart-specific typed properties
                MinValue = _minValue,
                MaxValue = _maxValue
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

            if (!ctx.ChartRect.IsEmpty)
            {
                AddHitArea("Chart", ctx.ChartRect, null, () =>
                {
                    ChartClicked?.Invoke(this, new BeepEventDataArgs("ChartClicked", this));
                });
            }

            if (ctx.ShowLegend && !ctx.LegendRect.IsEmpty)
            {
                AddHitArea("Legend", ctx.LegendRect, null, () =>
                {
                    LegendClicked?.Invoke(this, new BeepEventDataArgs("LegendClicked", this));
                });
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply chart-specific theme colors
            BackColor = _currentTheme.ChartBackColor;
            ForeColor = _currentTheme.ChartTextColor;
            
            // Update accent color from theme
            _accentColor = _currentTheme.AccentColor;
            
            // Update series colors from theme
            if (_currentTheme.ChartDefaultSeriesColors != null && _currentTheme.ChartDefaultSeriesColors.Count > 0)
            {
                _colors.Clear();
                _colors.AddRange(_currentTheme.ChartDefaultSeriesColors);
            }
            
            InitializePainter();
            Invalidate();
        }
    }
}