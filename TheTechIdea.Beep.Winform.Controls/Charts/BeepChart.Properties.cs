using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        #region Fields
        private IChartPainter _chartpainter;
        private IChartAxisPainter _axisPainter;
        private IChartSeriesPainter _seriesPainter;
        private IChartLegendPainter _legendPainter = new RightSideLegendPainter();
        private ChartType _lastSeriesType;
        private ChartSurfaceStyle _surfaceStyle = ChartSurfaceStyle.Card;
        private ChartVisualPreset _currentVisualPreset = ChartVisualPreset.Dashboard;
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
        private List<ChartTrackballDataPoint> _trackballDataPoints = new List<ChartTrackballDataPoint>();
        private float _trackballCrosshairX = -1f;
        private int _isolatedSeriesIndex = -1;
        private Dictionary<int, bool> _seriesVisibilityBeforeIsolation = new Dictionary<int, bool>();
        private int _lastLegendItemClickIndex = -1;
        private DateTime _lastLegendItemClickTime = DateTime.MinValue;
        private HashSet<(int SeriesIndex, int PointIndex)> _selectedPoints = new HashSet<(int, int)>();
        private HashSet<int> _selectedSeries = new HashSet<int>();
        private ChartSelectionMode _selectionMode = ChartSelectionMode.Multiple;
        private Point _lastRightClickLocation = Point.Empty;
        private int _keyboardFocusedSeriesIndex = 0;
        private int _keyboardFocusedPointIndex = 0;
        private bool _enablePerformanceMode = true;
        private int _largeDatasetThreshold = 500;
        private bool _enablePointCulling = true;
        private bool _enableVertexSimplification = true;
        private float _simplificationTolerance = 1.0f;
        private Dictionary<int, List<ChartDataPoint>> _simplifiedPointsCache = new Dictionary<int, List<ChartDataPoint>>();
        private Dictionary<int, ChartSeriesFillPattern> _seriesFillPatterns = new Dictionary<int, ChartSeriesFillPattern>();
        private bool _enableFillPatterns = false;
        private float _patternDensity = 3.0f;
        private bool _enableDenseLabelOptimization = true;
        private int _maxVisibleAxisLabels = 10;
        private int _lastAppliedXLabelInterval = 1;
        private int _lastAppliedYLabelInterval = 1;
        private bool _enableRealTimeStreaming = true;
        private int _streamRenderThrottleMs = 33;
        private int _maxStreamPointsPerSeries = 5000;
        private bool _autoScrollViewportOnStream = true;
        private bool _streamRefreshPending = false;
        private int _pendingStreamPointCount = 0;
        private System.Windows.Forms.Timer _streamRenderTimer;
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
        public Font ChartTitleFont { get; set; }

        [Browsable(true)]
        public Font ChartValueFont { get; set; }

        [Browsable(true)]
        public Font ChartSubtitleFont { get; set; }

        [Browsable(true)]
        public Color ChartTitleForeColor { get; set; }
        #endregion

        #region Data Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private List<ChartDataSeries> _dataSeries = new List<ChartDataSeries>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ChartDataSeries> DataSeries
        {
            // Return a defensive copy so callers cannot mutate the
            // backing list and bypass the RefreshDataState setter.
            get => new List<ChartDataSeries>(_dataSeries);
            set
            {
                _dataSeries = value ?? new List<ChartDataSeries>();
                RefreshDataState();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> LegendLabels { get; set; } = new List<string>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Color> ChartDefaultSeriesColors { get; set; } = new List<Color>();
        #endregion

        #region Axis Properties
        public AxisType BottomAxisType { get; set; } = AxisType.Numeric;
        public AxisType LeftAxisType { get; set; } = AxisType.Numeric;
        public string XAxisTitle { get; set; } = "";
        public string YAxisTitle { get; set; } = "";
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

        [Browsable(false)]
        public bool CanResetViewport => _dataSeries != null && _dataSeries.Any(s => s.Points != null && s.Points.Any());

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnableKeyboardViewportNavigation { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(0.08f)]
        public float KeyboardPanStepPercent { get; set; } = 0.08f;

        [Category("Interaction")]
        [DefaultValue(0.10f)]
        public float KeyboardZoomStepPercent { get; set; } = 0.10f;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnableMouseWheelZoom { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(0.10f)]
        public float MouseWheelZoomStepPercent { get; set; } = 0.10f;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnableMouseDragPan { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(1.0f)]
        public float MouseDragPanFactor { get; set; } = 1.0f;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnableTrackball { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool TrackballShowCrosshair { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool TrackballShowMultiSeriesValues { get; set; } = true;

        [Category("Appearance")]
        public Color TrackballCrosshairColor { get; set; } = Color.FromArgb(180, 200, 200, 200);

        [Category("Appearance")]
        [DefaultValue(1.5f)]
        public float TrackballCrosshairWidth { get; set; } = 1.5f;

        [Category("Appearance")]
        public Color TrackballTooltipBackColor { get; set; } = Color.FromArgb(240, 240, 240);

        [Category("Appearance")]
        public Color TrackballTooltipBorderColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnableLegendIsolate { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnablePointSelection { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(typeof(ChartSelectionMode), "Multiple")]
        public ChartSelectionMode SelectionMode
        {
            get => _selectionMode;
            set => _selectionMode = value;
        }

        [Category("Appearance")]
        public Color SelectionColor { get; set; } = Color.FromArgb(255, 255, 200, 50);

        [Category("Appearance")]
        public Color SelectionBorderColor { get; set; } = Color.FromArgb(255, 200, 100, 0);

        [Category("Appearance")]
        [DefaultValue(2.5f)]
        public float SelectionMarkerSize { get; set; } = 2.5f;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnableContextMenu { get; set; } = true;

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool EnableKeyboardNavigation { get; set; } = true;

        [Category("Performance")]
        [DefaultValue(true)]
        public bool EnablePerformanceMode { get; set; } = true;

        [Category("Performance")]
        [DefaultValue(500)]
        public int LargeDatasetThreshold { get; set; } = 500;

        [Category("Performance")]
        [DefaultValue(true)]
        public bool EnablePointCulling { get; set; } = true;

        [Category("Performance")]
        [DefaultValue(true)]
        public bool EnableVertexSimplification { get; set; } = true;

        [Category("Performance")]
        [DefaultValue(1.0f)]
        public float SimplificationTolerance
        {
            get => _simplificationTolerance;
            set => _simplificationTolerance = Math.Max(0.1f, value);
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool EnableFillPatterns
        {
            get => _enableFillPatterns;
            set { _enableFillPatterns = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(3.0f)]
        public float PatternDensity
        {
            get => _patternDensity;
            set { _patternDensity = Math.Max(0.5f, Math.Min(10.0f, value)); Invalidate(); }
        }

        [Category("Performance")]
        [DefaultValue(true)]
        public bool EnableDenseLabelOptimization
        {
            get => _enableDenseLabelOptimization;
            set { _enableDenseLabelOptimization = value; Invalidate(); }
        }

        [Category("Performance")]
        [DefaultValue(10)]
        public int MaxVisibleAxisLabels
        {
            get => _maxVisibleAxisLabels;
            set { _maxVisibleAxisLabels = Math.Max(3, Math.Min(30, value)); Invalidate(); }
        }

        [Category("Performance")]
        [DefaultValue(true)]
        public bool EnableRealTimeStreaming
        {
            get => _enableRealTimeStreaming;
            set => _enableRealTimeStreaming = value;
        }

        [Category("Performance")]
        [DefaultValue(33)]
        public int StreamRenderThrottleMs
        {
            get => _streamRenderThrottleMs;
            set => _streamRenderThrottleMs = Math.Max(0, Math.Min(1000, value));
        }

        [Category("Data")]
        [DefaultValue(5000)]
        public int MaxStreamPointsPerSeries
        {
            get => _maxStreamPointsPerSeries;
            set => _maxStreamPointsPerSeries = Math.Max(100, value);
        }

        [Category("Interaction")]
        [DefaultValue(true)]
        public bool AutoScrollViewportOnStream
        {
            get => _autoScrollViewportOnStream;
            set => _autoScrollViewportOnStream = value;
        }

        public int GetTotalPointCount() => _dataSeries.Sum(s => s.Points.Count);

        public bool IsLargeDataset() => EnablePerformanceMode && GetTotalPointCount() > LargeDatasetThreshold;
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
        public LegendPlacement LegendPlacement { get; set; } = LegendPlacement.InsideTopRight;

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
        public ChartVisualPreset CurrentVisualPreset
        {
            get => _currentVisualPreset;
            set
            {
                if (_currentVisualPreset != value)
                {
                    _currentVisualPreset = value;
                    ApplyVisualPreset(value);
                }
            }
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

        [Category("Appearance")]
        [Description("Show value labels on data points (bars, lines, pies).")]
        [DefaultValue(false)]
        public bool ShowDataLabels
        {
            get => _seriesOptions.ShowDataLabels;
            set { _seriesOptions.ShowDataLabels = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Show a linear-regression trend line on line/area charts.")]
        [DefaultValue(false)]
        public bool ShowTrendLine
        {
            get => _seriesOptions.ShowTrendLine;
            set { _seriesOptions.ShowTrendLine = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Center-hole radius ratio for Doughnut charts (0 = Pie, 0.4 = classic doughnut).")]
        [DefaultValue(0.4f)]
        public float DoughnutHoleRatio
        {
            get => _seriesOptions.DoughnutHoleRatio;
            set { _seriesOptions.DoughnutHoleRatio = Math.Clamp(value, 0f, 0.8f); Invalidate(); }
        }
        #endregion

        #region Theme Properties
        public Color ChartBackColor { get; set; }
        public Color ChartLineColor { get; set; }
        public Color ChartFillColor { get; set; }
        public Color ChartAxisColor { get; set; }
        public Color ChartTitleColor { get; set; }
        public Color ChartTextColor { get; set; }
        public Color ChartLegendBackColor { get; set; }
        public Color ChartLegendTextColor { get; set; }
        public Color ChartLegendShapeColor { get; set; }
        public Color ChartGridLineColor { get; set; }
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
        internal System.Windows.Forms.Timer StreamRenderTimer => _streamRenderTimer;
        internal List<ChartTrackballDataPoint> TrackballDataPoints 
        { 
            get => _trackballDataPoints; 
            set => _trackballDataPoints = value ?? new List<ChartTrackballDataPoint>(); 
        }
        internal float TrackballCrosshairX 
        { 
            get => _trackballCrosshairX; 
            set => _trackballCrosshairX = value; 
        }
        internal HashSet<(int, int)> SelectedPoints => _selectedPoints;
        internal HashSet<int> SelectedSeries => _selectedSeries;
        #endregion
    }
}