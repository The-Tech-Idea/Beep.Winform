using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum DashboardWidgetStyle
    {
        MultiMetric,      // Multiple KPIs in one widget
        ChartGrid,        // Multiple small charts
        TimelineView,     // Chronological display
        ComparisonGrid,   // Side-by-side comparisons
        StatusOverview,   // System status dashboard
        AnalyticsPanel    // Complex analytics layout
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Dashboard Widget")]
    [Category("Beep Widgets")]
    [Description("A dashboard widget combining multiple data visualizations.")]
    public class BeepDashboardWidget : BaseControl
    {
        #region Fields
        private DashboardWidgetStyle _style = DashboardWidgetStyle.MultiMetric;
        private IWidgetPainter _painter;
        private string _title = "Dashboard";
        private List<Dictionary<string, object>> _metrics = new List<Dictionary<string, object>>();
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _cardBackColor = Color.White;
        private Color _cardHoverBackColor = Color.FromArgb(248, 248, 248);
        private Color _panelBackColor = Color.FromArgb(250, 250, 250);
        private Color _gradientStartColor = Color.FromArgb(240, 240, 240);
        private Color _gradientEndColor = Color.White;
        private Color _gradientMiddleColor = Color.FromArgb(245, 245, 245);
        private LinearGradientMode _gradientDirection = LinearGradientMode.Vertical;
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private bool _showTitle = true;
        private int _columns = 2;
        private int _rows = 2;

        // Events
        public event EventHandler<BeepEventDataArgs> MetricClicked;
        public event EventHandler<BeepEventDataArgs> PanelClicked;
        #endregion

        #region Constructor
        public BeepDashboardWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(400, 300);
            ApplyThemeToChilds = false;
            InitializeSampleMetrics();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleMetrics()
        {
            _metrics.AddRange(new[]
            {
                new Dictionary<string, object> { ["Title"] = "Revenue", ["Value"] = "$127K", ["Trend"] = "+12%", ["Color"] = Color.Green },
                new Dictionary<string, object> { ["Title"] = "Users", ["Value"] = "23,456", ["Trend"] = "+8%", ["Color"] = Color.Blue },  
                new Dictionary<string, object> { ["Title"] = "Orders", ["Value"] = "1,234", ["Trend"] = "-2%", ["Color"] = Color.Red },
                new Dictionary<string, object> { ["Title"] = "Growth", ["Value"] = "15.7%", ["Trend"] = "+5%", ["Color"] = Color.Orange }
            });
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case DashboardWidgetStyle.MultiMetric:
                    _painter = new MultiMetricPainter();
                    break;
                case DashboardWidgetStyle.ChartGrid:
                    _painter = new ChartGridPainter();
                    break;
                case DashboardWidgetStyle.TimelineView:
                    _painter = new TimelineViewPainter();
                    break;
                case DashboardWidgetStyle.ComparisonGrid:
                    _painter = new ComparisonGridPainter();
                    break;
                case DashboardWidgetStyle.StatusOverview:
                    _painter = new StatusOverviewPainter();
                    break;
                case DashboardWidgetStyle.AnalyticsPanel:
                    _painter = new AnalyticsPanelPainter();
                    break;
                default:
                    _painter = new MultiMetricPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Dashboard")]
        [Description("Visual Style of the dashboard widget.")]
        public DashboardWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Dashboard")]
        [Description("Title of the dashboard.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Dashboard")]
        [Description("Metrics data for the dashboard.")]
        public List<Dictionary<string, object>> Metrics
        {
            get => _metrics;
            set { _metrics = value ?? new List<Dictionary<string, object>>(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the dashboard.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Dashboard")]
        [Description("Whether to show the title.")]
        public bool ShowTitle
        {
            get => _showTitle;
            set { _showTitle = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Number of columns in grid layout.")]
        public int Columns
        {
            get => _columns;
            set { _columns = Math.Max(1, value); Invalidate(); }
        }

        [Category("Layout")]
        [Description("Number of rows in grid layout.")]
        public int Rows
        {
            get => _rows;
            set { _rows = Math.Max(1, value); Invalidate(); }
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
                AccentColor = _accentColor,
                ShowHeader = _showTitle,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                
                // Dashboard-specific typed properties
                Metrics = _metrics.Cast<object>().ToList(),
                Columns = _columns,
                Rows = _rows
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
                AddHitArea("Dashboard", ctx.ContentRect, null, () =>
                {
                    PanelClicked?.Invoke(this, new BeepEventDataArgs("PanelClicked", this));
                });
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply dashboard-specific theme colors
            BackColor = _currentTheme.DashboardBackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update card and panel colors
            _cardBackColor = _currentTheme.DashboardCardBackColor;
            _cardHoverBackColor = _currentTheme.DashboardCardHoverBackColor;
            _panelBackColor = _currentTheme.PanelBackColor;
            
            // Update gradient properties if used
            _gradientStartColor = _currentTheme.DashboardGradiantStartColor;
            _gradientEndColor = _currentTheme.DashboardGradiantEndColor;
            _gradientMiddleColor = _currentTheme.DashboardGradiantMiddleColor;
            _gradientDirection = _currentTheme.DashboardGradiantDirection;
            
            // Update accent and border colors
            _accentColor = _currentTheme.AccentColor;
            _borderColor = _currentTheme.BorderColor;
            
            InitializePainter();
            Invalidate();
        }
    }
}