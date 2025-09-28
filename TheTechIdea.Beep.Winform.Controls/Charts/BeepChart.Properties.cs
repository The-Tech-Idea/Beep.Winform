using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        #region Fields
        private IChartPainter _painter;
        private IChartAxisPainter _axisPainter;
        private IChartSeriesPainter _seriesPainter;
        private IChartLegendPainter _legendPainter = new RightSideLegendPainter();
        private ChartType _lastSeriesType;
        private ChartSurfaceStyle _surfaceStyle = ChartSurfaceStyle.Card;
        private Color _accentColor = Color.FromArgb(0, 150, 136);
        private bool _showTitle = true;
        private readonly SeriesRenderOptions _seriesOptions = new SeriesRenderOptions();
        private System.Windows.Forms.Timer _animTimer;
        private ToolTip _dataPointToolTip;
        private ChartDataPoint _hoveredPoint;
        private Point _lastMouseDownPoint;
        private float _zoomFactor = 1.0f;
        private DateTime _lastInvalidateTime;
        private bool _isDrawingRectCalculated = false;
        private Rectangle _chartDrawingRect;
        private Dictionary<string, int> _xAxisCategories = new Dictionary<string, int>();
        private Dictionary<string, int> _yAxisCategories = new Dictionary<string, int>();
        private DateTime _xAxisDateMin = DateTime.MinValue;
        private DateTime _yAxisDateMin = DateTime.MinValue;
        #endregion

        #region Title Properties
        [Category("Appearance")]
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                _showTitle = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public string ChartTitle { get; set; } = "Chart Title";

        [Category("Appearance")]
        public string ChartValue { get; set; } = "$12,000,000";

        [Category("Appearance")]
        public string ChartSubtitle { get; set; } = "Subtitle or Description";

        [Browsable(true)]
        public Font ChartTitleFont { get; set; } = new Font("Arial", 12, FontStyle.Bold);

        [Browsable(true)]
        public Font ChartValueFont { get; set; } = new Font("Arial", 20, FontStyle.Bold);

        [Browsable(true)]
        public Font ChartSubtitleFont { get; set; } = new Font("Arial", 9, FontStyle.Regular);

        [Browsable(true)]
        public Color ChartTitleForeColor { get; set; } = Color.Black;
        #endregion

        #region Data Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private List<ChartDataSeries> _dataSeries = new List<ChartDataSeries>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ChartDataSeries> DataSeries
        {
            get => _dataSeries;
            set
            {
                _dataSeries = value ?? new List<ChartDataSeries>();
                if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                {
                    BeepChartDataHelper.DetectAxisTypes(this);
                    BeepChartViewportHelper.AutoScaleViewport(this);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> LegendLabels { get; set; } = new List<string>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>
        {
            Color.Blue, Color.Red, Color.Green, Color.Purple, Color.Orange,
            Color.Yellow, Color.Cyan, Color.Magenta, Color.Brown, Color.Pink
        };
        #endregion

        #region Axis Properties
        public AxisType BottomAxisType { get; set; } = AxisType.Numeric;
        public AxisType LeftAxisType { get; set; } = AxisType.Numeric;
        public string XAxisTitle { get; set; } = "X Title";
        public string YAxisTitle { get; set; } = "Y Title";
        public float XLabelAngle { get; set; } = 0f;
        public float YLabelAngle { get; set; } = 0f;
        public TimeTickGranularity XTimeGranularity { get; set; } = TimeTickGranularity.Auto;
        public TimeTickGranularity YTimeGranularity { get; set; } = TimeTickGranularity.Auto;
        #endregion

        #region Viewport Properties
        public float ViewportXMin { get; set; }
        public float ViewportXMax { get; set; }
        public float ViewportYMin { get; set; }
        public float ViewportYMax { get; set; }
        #endregion

        #region Chart Appearance Properties
        public Rectangle ChartDrawingRect
        {
            get => _chartDrawingRect;
            private set
            {
                _chartDrawingRect = value;
            }
        }

        public bool CustomDraw { get; set; }
        public bool ShowLegend { get; set; } = true;
        public ChartType ChartType { get; set; } = ChartType.Line;
        public LegendPlacement LegendPlacement { get; set; } = LegendPlacement.Right;

        [Category("Appearance")]
        public ChartSurfaceStyle SurfaceStyle
        {
            get => _surfaceStyle;
            set { _surfaceStyle = value; InitializePainter(); Invalidate(); }
        }

        [Category("Appearance")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public bool Stacked
        {
            get => _seriesOptions.Stacked;
            set { _seriesOptions.Stacked = value; Invalidate(); }
        }

        [Category("Appearance")]
        public bool SmoothLines
        {
            get => _seriesOptions.SmoothLines;
            set { _seriesOptions.SmoothLines = value; Invalidate(); }
        }

        [Category("Appearance")]
        public bool ShowMarkers
        {
            get => _seriesOptions.ShowMarkers;
            set { _seriesOptions.ShowMarkers = value; Invalidate(); }
        }

        [Category("Appearance")]
        public StackedMode StackedMode
        {
            get => _seriesOptions.Mode;
            set { _seriesOptions.Mode = value; Invalidate(); }
        }
        #endregion

        #region Theme Properties
        public Color ChartBackColor { get; set; } = Color.White;
        public Color ChartLineColor { get; set; } = Color.Black;
        public Color ChartFillColor { get; set; } = Color.LightGray;
        public Color ChartAxisColor { get; set; } = Color.Black;
        public Color ChartTitleColor { get; set; } = Color.Black;
        public Color ChartTextColor { get; set; } = Color.Black;
        public Color ChartLegendBackColor { get; set; } = Color.White;
        public Color ChartLegendTextColor { get; set; } = Color.Black;
        public Color ChartLegendShapeColor { get; set; } = Color.Black;
        public Color ChartGridLineColor { get; set; } = Color.LightGray;
        #endregion

        #region Internal Properties for Helpers
        internal Dictionary<string, int> XAxisCategories => _xAxisCategories;
        internal Dictionary<string, int> YAxisCategories => _yAxisCategories;
        internal DateTime XAxisDateMin 
        { 
            get => _xAxisDateMin; 
            set => _xAxisDateMin = value; 
        }
        internal DateTime YAxisDateMin 
        { 
            get => _yAxisDateMin; 
            set => _yAxisDateMin = value; 
        }
        internal SeriesRenderOptions SeriesOptions => _seriesOptions;
        internal ChartDataPoint HoveredPoint 
        { 
            get => _hoveredPoint; 
            set => _hoveredPoint = value; 
        }
        internal Point LastMouseDownPoint 
        { 
            get => _lastMouseDownPoint; 
            set => _lastMouseDownPoint = value; 
        }
        internal float ZoomFactor => _zoomFactor;
        internal DateTime LastInvalidateTime 
        { 
            get => _lastInvalidateTime; 
            set => _lastInvalidateTime = value; 
        }
        internal ToolTip DataPointToolTip => _dataPointToolTip;
        internal System.Windows.Forms.Timer AnimTimer => _animTimer;
        #endregion
    }
}