using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public enum ChartSurfaceStyle
    {
        Classic,
        Card,
        Outline,
        Glass
    }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepChart), "BeepChart.bmp")]
    [Description("A custom chart control for WinForms.")]
    [DefaultProperty("DataSeries")]
    [DisplayName("BeepChart")]
    [Category("Beep Controls")]
    public class BeepChart : BaseControl
    {
        #region Fields and Properties
        private IChartPainter _painter;
        private IChartAxisPainter _axisPainter;
        private IChartSeriesPainter _seriesPainter;
        private IChartLegendPainter _legendPainter = new RightSideLegendPainter();
        private ChartType _lastSeriesType;
        private ChartSurfaceStyle _surfaceStyle = ChartSurfaceStyle.Card;
        private Color _accentColor = Color.FromArgb(0, 150, 136);
        private bool _showtitle= true;
        private readonly SeriesRenderOptions _seriesOptions = new SeriesRenderOptions();
        private System.Windows.Forms.Timer _animTimer;

        [Category("Appearance")]
        public bool ShowTitle
        {
            get => _showtitle;
            set
            {
                _showtitle = value;
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
            = new Font("Arial", 12, FontStyle.Bold);

        [Browsable(true)]
        public Font ChartValueFont { get; set; }
            = new Font("Arial", 20, FontStyle.Bold);

        [Browsable(true)]
        public Font ChartSubtitleFont { get; set; }
            = new Font("Arial", 9, FontStyle.Regular);

        [Browsable(true)]
        public Color ChartTitleForeColor { get; set; } = Color.Black;

        // If you want separate color for Value or Subtitle, simply add them similarly:
        // public Color ChartValueForeColor { get; set; } = Color.DarkBlue;
        // public Color ChartSubtitleForeColor { get; set; } = Color.Gray;

        // ... and similarly for ChartValueForeColor, ChartSubtitleForeColor, etc.

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
                    // 1) Auto-detect axis types (optional convenience)
                    DetectAxisTypes();

                    AutoScaleViewport();
                   // Invalidate();
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

        private Dictionary<string, int> xAxisCategories = new Dictionary<string, int>();
        private Dictionary<string, int> yAxisCategories = new Dictionary<string, int>();
        private DateTime xAxisDateMin = DateTime.MinValue;
        private DateTime yAxisDateMin = DateTime.MinValue;

        public AxisType BottomAxisType { get; set; } = AxisType.Numeric;
        public AxisType LeftAxisType { get; set; } = AxisType.Numeric;

        public float ViewportXMin { get; set; }
        public float ViewportXMax { get; set; }
        public float ViewportYMin { get; set; }
        public float ViewportYMax { get; set; }

        private Rectangle chartDrawingRect;
        public Rectangle ChartDrawingRect
        {
            get => chartDrawingRect;
            private set
            {
                chartDrawingRect = value;
               // Invalidate();
            }
        }

        private bool customDraw;
        public bool CustomDraw
        {
            get => customDraw;
            set
            {
                customDraw = value;
                Invalidate();
            }
        }

        public bool ShowLegend { get; set; } = true;
        public ChartType ChartType { get; set; } = ChartType.Line;

        public string XAxisTitle { get; set; } = "X Title";
        public string YAxisTitle { get; set; } = "Y Title";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private ToolTip dataPointToolTip;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<CustomDrawSeriesEventArgs> CustomDrawSeries;

        private ChartDataPoint hoveredPoint;
        private Point lastMouseDownPoint;
        private float zoomFactor = 1.0f;
        private DateTime lastInvalidateTime;
        private bool isDrawinRectCalculated=false;

        #region Theme Support
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

        public override void ApplyTheme()
        {
            try
            {
            //    base.ApplyTheme();
                  BackColor= _currentTheme.ChartBackColor;
                  ForeColor= _currentTheme.ChartTitleColor;
                   ChartBackColor = _currentTheme.ChartBackColor;
                    ChartLineColor = _currentTheme.ChartLineColor;
                    ChartFillColor = _currentTheme.ChartFillColor;
                    ChartAxisColor = _currentTheme.ChartAxisColor;
                    ChartTitleColor = _currentTheme.ChartTitleColor;
                   ChartTitleForeColor = _currentTheme.ChartTitleColor;
                   ChartTextColor = _currentTheme.ChartTextColor;
                    ChartLegendBackColor = _currentTheme.ChartLegendBackColor;
                    ChartLegendTextColor = _currentTheme.ChartLegendTextColor;
                    ChartLegendShapeColor = _currentTheme.ChartLegendShapeColor;
                    ChartGridLineColor = _currentTheme.ChartGridLineColor;
                    ChartDefaultSeriesColors = new List<Color>(_currentTheme.ChartDefaultSeriesColors);
                    ChartTitleFont = FontListHelper.CreateFontFromTypography(_currentTheme.TitleStyle);
                    ChartValueFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetBlockHeaderFont());
                    ChartSubtitleFont = FontListHelper.CreateFontFromTypography(_currentTheme.GetBlockTextFont());
                    InitializePainter();
                    Invalidate();
             
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"ApplyTheme Error: {ex.Message}");
            }
        }
        #endregion

        #endregion

        #region Constructor and Initialization
        public BeepChart():base()
        {
            try
            {
               

                InitializeDefaultSettings();
                InitializeDesignTimeSampleData();
                InitializePainter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"BeepChart Constructor Error: {ex.Message}");
            }
        }
        protected override Size DefaultSize => new Size(400, 300);
        private void InitializeDefaultSettings()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                dataPointToolTip = new ToolTip();
                UpdateChartDrawingRectBase();
                AutoScaleViewport();
            }
        }

        private void InitializeDesignTimeSampleData()
        {
            if (DesignMode)
            {
                DataSeries = new List<ChartDataSeries>
                {
                    new ChartDataSeries
                    {
                        Name = "Sample Series",
                        ChartType = ChartType.Line,
                        ShowLine = true,
                        ShowPoint = true,
                        Points = new List<ChartDataPoint>
                        {
                            new ChartDataPoint("1", "5", 10f, "A", Color.Red),
                            new ChartDataPoint("2", "15", 15f, "B", Color.Green),
                            new ChartDataPoint("3", "8", 20f, "C", Color.Blue)
                        }
                    }
                };
                UpdateChartDrawingRectBase();
                AutoScaleViewport();
            }
        }

        private void InitializePainter()
        {
            switch (_surfaceStyle)
            {
                case ChartSurfaceStyle.Card:
                    _painter = new CardChartPainter();
                    break;
                case ChartSurfaceStyle.Outline:
                    _painter = new OutlineChartPainter();
                    break;
                case ChartSurfaceStyle.Glass:
                    _painter = new GlassChartPainter2();
                    break;
                case ChartSurfaceStyle.Classic:
                default:
                    _painter = new CardChartPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);

            _axisPainter = new CartesianAxisPainter();

            // pick a series painter by primary chart type (assume uniform for simplicity)
            _lastSeriesType = ChartType;
            switch (_lastSeriesType)
            {
                case ChartType.Line:
                    _seriesPainter = new LineSeriesPainter();
                    break;
                case ChartType.Bar:
                    _seriesPainter = new BarSeriesPainter();
                    break;
                default:
                    _seriesPainter = new LineSeriesPainter();
                    break;
            }
            _seriesPainter.Initialize(this);
        }
        #endregion

        #region Override Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            isDrawinRectCalculated = false;
          //  UpdateChartDrawingRectBase();
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                AutoScaleViewport();
            }
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                base.OnHandleCreated(e);
                if (ChartDrawingRect.IsEmpty)
                {
                    UpdateChartDrawingRectBase();
                }
                if (DesignMode)
                {
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"OnHandleCreated Error: {ex.Message}");
            }
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            if (DrawingRect.Width <= 0 || DrawingRect.Height <= 0)
                return;

            // painter: adjust inner bounds and draw accents
            var ctx = new ChartLayout
            {
                DrawingRect = DrawingRect,
                PlotRect = Rectangle.Empty,
                Radius = BorderRadius,
                AccentColor = _accentColor
            };
            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;
            _painter?.DrawBackground(g, ctx);

            var bounds = ctx.PlotRect != Rectangle.Empty ? ctx.PlotRect : ctx.DrawingRect;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            int textAreaLeft = bounds.Left + 10;
            int currentY = bounds.Top + 10;
            if (ShowTitle)
            {
                if (!string.IsNullOrEmpty(ChartTitle))
                {
                    using (Brush titleBrush = new SolidBrush(ChartTitleForeColor))
                    {
                        SizeF titleSize = g.MeasureString(ChartTitle, ChartTitleFont);
                        g.DrawString(ChartTitle, ChartTitleFont, titleBrush, textAreaLeft, currentY);
                        currentY += (int)titleSize.Height + 5;
                    }
                }
                if (!string.IsNullOrEmpty(ChartValue))
                {
                    using (Brush valueBrush = new SolidBrush(ChartTextColor))
                    {
                        SizeF valueSize = g.MeasureString(ChartValue, ChartValueFont);
                        g.DrawString(ChartValue, ChartValueFont, valueBrush, textAreaLeft, currentY);
                        currentY += (int)valueSize.Height + 5;
                    }
                }
                if (!string.IsNullOrEmpty(ChartSubtitle))
                {
                    using (Brush subBrush = new SolidBrush(ChartLineColor))
                    {
                        SizeF subSize = g.MeasureString(ChartSubtitle, ChartSubtitleFont);
                        g.DrawString(ChartSubtitle, ChartSubtitleFont, subBrush, textAreaLeft, currentY);
                        currentY += (int)subSize.Height + 10;
                    }
                }
            }

            // axis painter computes plot rect
            var axisCtx = new AxisLayout
            {
                Bounds = new Rectangle(bounds.Left, currentY, bounds.Width, bounds.Bottom - currentY),
                BottomAxisType = BottomAxisType,
                LeftAxisType = LeftAxisType,
                XMin = ViewportXMin,
                XMax = ViewportXMax,
                YMin = ViewportYMin,
                YMax = ViewportYMax,
                XTitle = XAxisTitle,
                YTitle = YAxisTitle,
                TitleFont = new Font("Arial", 10, FontStyle.Bold),
                LabelFont = new Font("Arial", 8),
                TextColor = ChartTextColor,
                AxisColor = ChartAxisColor,
                GridColor = ChartGridLineColor,
                XCategories = xAxisCategories,
                YCategories = yAxisCategories,
                XDateMin = xAxisDateMin,
                YDateMin = yAxisDateMin,
                ShowLegend = ShowLegend,
                XLabelAngle = XLabelAngle,
                YLabelAngle = YLabelAngle,
                XTimeGranularity = XTimeGranularity,
                YTimeGranularity = YTimeGranularity
            };
            axisCtx = _axisPainter.AdjustPlotRect(g, axisCtx);
            _axisPainter.DrawAxes(g, axisCtx);
            _axisPainter.DrawTicks(g, axisCtx);

            ChartDrawingRect = axisCtx.PlotRect;

            if (ChartType == ChartType.Pie)
            {
                _seriesPainter = new PieSeriesPainter();
                _seriesPainter.Initialize(this);
                _seriesPainter.DrawSeries(g, axisCtx.PlotRect, _dataSeries, p => ConvertXValue(p), p => ConvertYValue(p), ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax, ChartDefaultSeriesColors, ChartAxisColor, ChartTextColor, _seriesOptions);
                _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, ChartDefaultSeriesColors, new Font("Arial", 8), ChartLegendTextColor, ChartLegendBackColor, ChartLegendShapeColor, this, ToggleSeriesByIndex, LegendPlacement);
            }
            else if (ChartType == ChartType.Line)
            {
                _seriesPainter = new LineSeriesPainter();
                _seriesPainter.Initialize(this);
                _seriesPainter.DrawSeries(g, axisCtx.PlotRect, _dataSeries, p => ConvertXValue(p), p => ConvertYValue(p), ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax, ChartDefaultSeriesColors, ChartAxisColor, ChartTextColor, _seriesOptions);
                if (ShowLegend) _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, ChartDefaultSeriesColors, new Font("Arial", 8), ChartLegendTextColor, ChartLegendBackColor, ChartLegendShapeColor, this, ToggleSeriesByIndex, LegendPlacement);
            }
            else if (ChartType == ChartType.Bar)
            {
                _seriesPainter = new BarSeriesPainter();
                _seriesPainter.Initialize(this);
                _seriesPainter.DrawSeries(g, axisCtx.PlotRect, _dataSeries, p => ConvertXValue(p), p => ConvertYValue(p), ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax, ChartDefaultSeriesColors, ChartAxisColor, ChartTextColor, _seriesOptions);
                if (ShowLegend) _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, ChartDefaultSeriesColors, new Font("Arial", 8), ChartLegendTextColor, ChartLegendBackColor, ChartLegendShapeColor, this, ToggleSeriesByIndex, LegendPlacement);
            }
            else if (ChartType == ChartType.Area)
            {
                _seriesPainter = new AreaSeriesPainter();
                _seriesPainter.Initialize(this);
                _seriesPainter.DrawSeries(g, axisCtx.PlotRect, _dataSeries, p => ConvertXValue(p), p => ConvertYValue(p), ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax, ChartDefaultSeriesColors, ChartAxisColor, ChartTextColor, _seriesOptions);
                if (ShowLegend) _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, ChartDefaultSeriesColors, new Font("Arial", 8), ChartLegendTextColor, ChartLegendBackColor, ChartLegendShapeColor, this, ToggleSeriesByIndex, LegendPlacement);
            }
            else if (ChartType == ChartType.Bubble)
            {
                _seriesPainter = new BubbleSeriesPainter();
                _seriesPainter.Initialize(this);
                _seriesPainter.DrawSeries(g, axisCtx.PlotRect, _dataSeries, p => ConvertXValue(p), p => ConvertYValue(p), ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax, ChartDefaultSeriesColors, ChartAxisColor, ChartTextColor, _seriesOptions);
                if (ShowLegend) _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, ChartDefaultSeriesColors, new Font("Arial", 8), ChartLegendTextColor, ChartLegendBackColor, ChartLegendShapeColor, this, ToggleSeriesByIndex, LegendPlacement);
            }
            else
            {
                if (ChartType == ChartType.Bubble) DrawBubbleSeries(g);
                if (ChartType == ChartType.Area) DrawAreaSeries(g);
                if (ShowLegend) _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, ChartDefaultSeriesColors, new Font("Arial", 8), ChartLegendTextColor, ChartLegendBackColor, ChartLegendShapeColor, this, ToggleSeriesByIndex, LegendPlacement);
            }

            // Add simple legend/title hit areas (optional)
            var legendArea = new Rectangle(ChartDrawingRect.Right + 10, ChartDrawingRect.Top, 120, Math.Max(100, ChartDrawingRect.Height / 3));
            AddHitArea("Legend", legendArea, null, () => { /* future: raise click */ });

            _painter?.DrawForeground(g, ctx);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

            }
            catch (Exception ex)
            {
                // ...
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                ChartInputHelper.HandleMouseWheel(this, e);
            }
            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                ChartInputHelper.HandleMouseDown(ref lastMouseDownPoint, e);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                var newHoveredPoint = ChartInputHelper.HandleMouseMove(this, e, ref lastMouseDownPoint, ref lastInvalidateTime, zoomFactor);
                if (newHoveredPoint != hoveredPoint)
                {
                    hoveredPoint = newHoveredPoint;
                    if ((DateTime.Now - lastInvalidateTime).TotalMilliseconds > 50)
                    {
                        Invalidate();
                        lastInvalidateTime = DateTime.Now;
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                ChartInputHelper.HandleMouseUp(this, ref lastMouseDownPoint);
            }
            base.OnMouseUp(e);
        }
        #endregion

        #region Conversion Methods
        public object ConvertXValue(ChartDataPoint point)
        {
            return ConvertValue(point.X, BottomAxisType, xAxisCategories, ref xAxisDateMin);
        }

        public object ConvertYValue(ChartDataPoint point)
        {
            return ConvertValue(point.Y, LeftAxisType, yAxisCategories, ref yAxisDateMin);
        }

        private float ConvertValue(string value, AxisType axisType, Dictionary<string, int> categoryMap, ref DateTime dateMin)
        {
            try
            {
                string parsedValue = value ?? string.Empty;
                switch (axisType)
                {
                    case AxisType.Numeric:
                        return float.TryParse(parsedValue, out float numericValue) ? numericValue : 0;
                    case AxisType.Date:
                        if (DateTime.TryParse(parsedValue, out DateTime dateValue))
                        {
                            if (dateMin == DateTime.MinValue) dateMin = dateValue;
                            return (float)(dateValue - dateMin).TotalDays;
                        }
                        return 0;
                    case AxisType.Text:
                        if (!categoryMap.ContainsKey(parsedValue))
                            categoryMap[parsedValue] = categoryMap.Count;
                        return categoryMap[parsedValue];
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Error converting value: {ex.Message}");
                return 0;
            }
        }
        private void DetectAxisTypes()
        {
            var firstSeries = _dataSeries.FirstOrDefault(s => s.Points != null && s.Points.Any());
            if (firstSeries != null)
            {
                string exampleX = firstSeries.Points[0].X;
                string exampleY = firstSeries.Points[0].Y;

                if (float.TryParse(exampleX, out _))
                    BottomAxisType = AxisType.Numeric;
                else if (DateTime.TryParse(exampleX, out _))
                    BottomAxisType = AxisType.Date;
                else
                    BottomAxisType = AxisType.Text;

                if (float.TryParse(exampleY, out _))
                    LeftAxisType = AxisType.Numeric;
                else if (DateTime.TryParse(exampleY, out _))
                    LeftAxisType = AxisType.Date;
                else
                    LeftAxisType = AxisType.Text;
            }
        }
        #endregion

        #region Drawing Methods (legacy helpers still used for some types)
        private ChartDataPoint GetHoveredDataPoint(Point location)
        {
            foreach (var series in DataSeries)
            {
                if (series.Points == null) continue;
                foreach (var point in series.Points)
                {
                    float x = ConvertXValue(point) is float xVal ? xVal : 0;
                    float y = ConvertYValue(point) is float yVal ? yVal : 0;
                    PointF screenPos = new PointF(
                        ChartDrawingRect.Left + (x - ViewportXMin) / (ViewportXMax - ViewportXMin) * ChartDrawingRect.Width,
                        ChartDrawingRect.Bottom - (y - ViewportYMin) / (ViewportYMax - ViewportYMin) * ChartDrawingRect.Height
                    );
                    if (Math.Sqrt(Math.Pow(screenPos.X - location.X, 2) + Math.Pow(screenPos.Y - location.Y, 2)) < 10)
                        return point;
                }
            }
            return null;
        }

        private void ShowTooltip(ChartDataPoint point)
        {
            if (dataPointToolTip != null)
            {
                string tooltipText = point.ToolTip ?? $"{point.X}, {point.Y}";
                dataPointToolTip.Show(tooltipText, this, PointToClient(MousePosition), 3000);
            }
        }

        private void PanViewport(int deltaX, int deltaY)
        {
            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;
            float panX = (float)deltaX / ChartDrawingRect.Width * xRange * zoomFactor;
            float panY = (float)deltaY / ChartDrawingRect.Height * yRange * zoomFactor;

            ViewportXMin -= panX;
            ViewportXMax -= panX;
            ViewportYMin += panY;
            ViewportYMax += panY;

            EnforceViewportLimits();
            Invalidate();
        }

        private void UpdateChartDrawingRectBase()
        {
            try
            {
                int leftPadding = 40;  
                int rightPadding = 30; 
                int topPadding = 40;   
                int bottomPadding = 40; 

                ChartDrawingRect = new Rectangle(
                    DrawingRect.Left + leftPadding,
                    DrawingRect.Top + topPadding,
                    DrawingRect.Width - leftPadding - rightPadding,
                    DrawingRect.Height - topPadding - bottomPadding
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"UpdateChartDrawingRectBase Error: {ex.Message}");
                ChartDrawingRect = ClientRectangle;
            }
        }

        private void DrawBubbleSeries(Graphics g)
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.Points != null && s.Points.Any()))
                    return;

                float maxBubbleSize = 50f; 
                float minBubbleSize = 5f;  

                float maxX = DataSeries.SelectMany(s => s.Points).Max(p => ConvertXValue(p) is float x ? x : 0);
                float minX = DataSeries.SelectMany(s => s.Points).Min(p => ConvertXValue(p) is float x ? x : 0);
                float maxY = DataSeries.SelectMany(s => s.Points).Max(p => ConvertYValue(p) is float y ? y : 0);
                float minY = DataSeries.SelectMany(s => s.Points).Min(p => ConvertYValue(p) is float y ? y : 0);
                float maxValue = DataSeries.SelectMany(s => s.Points).Max(p => p.Value);

                foreach (var series in DataSeries)
                {
                    if (!series.Visible) continue;

                    foreach (var point in series.Points)
                    {
                        float x = ConvertXValue(point) is float xVal ? xVal : 0;
                        float y = ConvertYValue(point) is float yVal ? yVal : 0;
                        float bubbleSize = maxValue > 0 ? point.Value / maxValue * maxBubbleSize : minBubbleSize;
                        bubbleSize = Math.Max(bubbleSize, minBubbleSize);

                        float screenX = ChartDrawingRect.Left + (x - minX) / (maxX - minX) * ChartDrawingRect.Width;
                        float screenY = ChartDrawingRect.Bottom - (y - minY) / (maxY - minY) * ChartDrawingRect.Height;

                        Color bubbleColor = point.Color != Color.Empty
                            ? point.Color
                            : ChartDefaultSeriesColors[DataSeries.IndexOf(series) % ChartDefaultSeriesColors.Count];

                        using (Brush brush = new SolidBrush(Color.FromArgb(150, bubbleColor))) 
                        {
                            g.FillEllipse(brush, screenX - bubbleSize / 2, screenY - bubbleSize / 2, bubbleSize, bubbleSize);
                        }
                        using (Pen pen = new Pen(ChartLineColor, 1))
                        {
                            g.DrawEllipse(pen, screenX - bubbleSize / 2, screenY - bubbleSize / 2, bubbleSize, bubbleSize);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawBubbleSeries Error: {ex.Message}");
            }
        }

        private void DrawAreaSeries(Graphics g)
        {
            if (!DataSeries.Any())
                return;

            foreach (var series in DataSeries)
            {
                if (!series.Visible || series.Points == null || !series.Points.Any())
                    continue;

                var screenPoints = series.Points.Select(p =>
                {
                    float xVal = ConvertXValue(p) is float xv ? xv : 0;
                    float yVal = ConvertYValue(p) is float yv ? yv : 0;

                    float xScreen = ChartDrawingRect.Left +
                        (xVal - ViewportXMin) / (ViewportXMax - ViewportXMin) * ChartDrawingRect.Width;

                    float yScreen = ChartDrawingRect.Bottom - 
                        (yVal - ViewportYMin) / (ViewportYMax - ViewportYMin) * ChartDrawingRect.Height;

                    return new PointF(xScreen, yScreen);
                }).ToList();

                if (screenPoints.Count < 2)
                    continue;

                var areaPoints = new List<PointF>(screenPoints)
                {
                    new PointF(screenPoints.Last().X, ChartDrawingRect.Bottom), 
                    new PointF(screenPoints.First().X, ChartDrawingRect.Bottom) 
                };

                using (Brush areaBrush = new SolidBrush(Color.FromArgb(100, series.Color)))
                {
                    g.FillPolygon(areaBrush, areaPoints.ToArray());
                }

                using (Pen pen = new Pen(series.Color != Color.Empty ? series.Color : ChartLineColor, 2))
                {
                    g.DrawLines(pen, screenPoints.ToArray());
                }
            }
        }

        private void DrawPieLegend(Graphics g)
        {
            var pieSeries = DataSeries.FirstOrDefault(s => s.ChartType == ChartType.Pie && s.Visible);
            if (pieSeries == null || !pieSeries.Points.Any())
                return;

            int legendX = ChartDrawingRect.Right + 10;
            int legendY = ChartDrawingRect.Top;
            int itemHeight = 20;

            using (Font font = new Font("Arial", 8))
            using (Brush textBrush = new SolidBrush(ChartLegendTextColor))
            using (Brush backBrush = new SolidBrush(ChartLegendBackColor))
            {
                int legendHeight = pieSeries.Points.Count * itemHeight + 10;
                int legendWidth = 120; 
                g.FillRectangle(backBrush, legendX, legendY, legendWidth, legendHeight);

                int currentY = legendY + 2;
                int colorIndex = 0;

                foreach (var point in pieSeries.Points)
                {
                    Color sliceColor = point.Color != Color.Empty
                        ? point.Color
                        : ChartDefaultSeriesColors[colorIndex % ChartDefaultSeriesColors.Count];

                    using (Brush swatchBrush = new SolidBrush(sliceColor))
                    {
                        g.FillRectangle(swatchBrush, legendX + 2, currentY, 15, 15);
                    }
                    using (Pen pen = new Pen(ChartLegendShapeColor, 1))
                    {
                        g.DrawRectangle(pen, legendX + 2, currentY, 15, 15);
                    }

                    string labelText = !string.IsNullOrEmpty(point.X)
                        ? point.X
                        : $"Slice {colorIndex + 1}";

                    g.DrawString(labelText, font, textBrush, legendX + 20, currentY);

                    currentY += itemHeight;
                    colorIndex++;
                }
            }
        }

        private void DrawLegend(Graphics g)
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.ShowInLegend)) return;

                int legendX = ChartDrawingRect.Right + 10;
                int legendY = ChartDrawingRect.Top;
                int itemHeight = 20;
                int itemWidth = 50;

                using (Font font = new Font("Arial", 8))
                using (Brush textBrush = new SolidBrush(ChartLegendTextColor))
                using (Brush backBrush = new SolidBrush(ChartLegendBackColor))
                {
                    int currentY = legendY;
                    int colorIndex = 0;

                    g.FillRectangle(backBrush, legendX, legendY, itemWidth + 20, DataSeries.Count * itemHeight + 10);

                    foreach (var series in DataSeries)
                    {
                        if (!series.ShowInLegend || !series.Visible) continue;

                        Color seriesColor = series.Color != Color.Empty ? series.Color : ChartDefaultSeriesColors[colorIndex % ChartDefaultSeriesColors.Count];
                        string seriesName = string.IsNullOrEmpty(series.Name) ? $"Series {colorIndex + 1}" : series.Name;

                        using (Brush swatchBrush = new SolidBrush(seriesColor))
                        {
                            g.FillRectangle(swatchBrush, legendX + 2, currentY + 2, 15, 15);
                        }

                        using (Pen pen = new Pen(ChartLegendShapeColor, 1))
                        {
                            g.DrawRectangle(pen, legendX + 2, currentY + 2, 15, 15);
                        }

                        g.DrawString(seriesName, font, textBrush, legendX + 20, currentY + 2);

                        currentY += itemHeight;
                        colorIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawLegend Error: {ex.Message}");
            }
        }

        #endregion

        #region Auto-Scaling and Viewport Management
        private void AutoScaleViewport()
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.Points != null && s.Points.Any())) return;

                float xMin = float.MaxValue, xMax = float.MinValue;
                float yMin = float.MaxValue, yMax = float.MinValue;

                Dictionary<string, int> xCategories = new Dictionary<string, int>();
                Dictionary<string, int> yCategories = new Dictionary<string, int>();
                int xCategoryIndex = 0, yCategoryIndex = 0;

                foreach (var series in DataSeries)
                {
                    foreach (var point in series.Points)
                    {
                        var xValue = ConvertXValue(point);
                        if (xValue is float xVal)
                        {
                            xMin = Math.Min(xMin, xVal);
                            xMax = Math.Max(xMax, xVal);
                        }
                        else if (xValue is string xStr)
                        {
                            if (!xCategories.ContainsKey(xStr))
                                xCategories[xStr] = xCategoryIndex++;
                            xMin = Math.Min(xMin, xCategories[xStr]);
                            xMax = Math.Max(xMax, xCategories[xStr]);
                        }

                        var yValue = ConvertYValue(point);
                        if (yValue is float yVal)
                        {
                            yMin = Math.Min(yMin, yVal);
                            yMax = Math.Max(yMax, yVal);
                        }
                        else if (yValue is string yStr)
                        {
                            if (!yCategories.ContainsKey(yStr))
                                yCategories[yStr] = yCategoryIndex++;
                            yMin = Math.Min(yMin, yCategories[yStr]);
                            yMax = Math.Max(yMax, yCategories[yStr]);
                        }
                    }
                }

                if (xMin == float.MaxValue || xMax == float.MinValue || yMin == float.MaxValue || yMax == float.MinValue)
                    return;

                ViewportXMin = xMin;
                ViewportXMax = xMax;
                ViewportYMin = yMin;
                ViewportYMax = yMax;

                float xPadding = (ViewportXMax - ViewportXMin) * 0.1f;
                float yPadding = (ViewportYMax - ViewportYMin) * 0.1f;

                ViewportXMin -= xPadding;
                ViewportXMax += xPadding;
                ViewportYMin -= yPadding;
                ViewportYMax += yPadding;

                EnforceViewportLimits();
                Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"AutoScaleViewport Error: {ex.Message}");
            }
        }
        private void EnforceViewportLimits()
        {
            if (ViewportXMin > ViewportXMax)
            {
                float mid = (ViewportXMin + ViewportXMax) / 2;
                ViewportXMin = mid - 1;
                ViewportXMax = mid + 1;
            }
            if (ViewportYMin > ViewportYMax)
            {
                float mid = (ViewportYMin + ViewportYMax) / 2;
                ViewportYMin = mid - 1;
                ViewportYMax = mid + 1;
            }
            ViewportXMin = Math.Max(ViewportXMin, float.MinValue);
            ViewportXMax = Math.Min(ViewportXMax, float.MaxValue);
            ViewportYMin = Math.Max(ViewportYMin, float.MinValue);
            ViewportYMax = Math.Min(ViewportYMax, float.MaxValue);
        }

        #endregion

        #region Cleanup
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dataPointToolTip?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Surface style properties
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
        #endregion

        public void StartAnimation(int durationMs = 600)
        {
            _seriesOptions.AnimationProgress = 0f;
            _animTimer ??= new System.Windows.Forms.Timer();
            _animTimer.Interval = 16;
            int elapsed = 0;
            void Tick(object s, EventArgs e)
            {
                elapsed += _animTimer.Interval;
                _seriesOptions.AnimationProgress = Math.Min(1f, elapsed / (float)durationMs);
                Invalidate();
                if (_seriesOptions.AnimationProgress >= 1f)
                {
                    _animTimer.Stop();
                    _animTimer.Tick -= Tick;
                }
            }
            _animTimer.Tick -= Tick;
            _animTimer.Tick += Tick;
            _animTimer.Start();
        }

        private void ToggleSeriesByIndex(int index)
        {
            if (index >= 0 && index < _dataSeries.Count)
            {
                _dataSeries[index].Visible = !_dataSeries[index].Visible;
                Invalidate();
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

        public LegendPlacement LegendPlacement { get; set; } = LegendPlacement.Right;
        public float XLabelAngle { get; set; } = 0f;
        public float YLabelAngle { get; set; } = 0f;
        public TimeTickGranularity XTimeGranularity { get; set; } = TimeTickGranularity.Auto;
        public TimeTickGranularity YTimeGranularity { get; set; } = TimeTickGranularity.Auto;

        [Category("Appearance")]
        public StackedMode StackedMode
        {
            get => _seriesOptions.Mode;
            set { _seriesOptions.Mode = value; Invalidate(); }
        }
    }

    public class CustomDrawSeriesEventArgs : EventArgs
    {
        public Graphics Graphics { get; }
        public Rectangle ChartArea { get; }
        public List<ChartDataSeries> DataSeries { get; }

        public CustomDrawSeriesEventArgs(Graphics g, Rectangle chartArea, List<ChartDataSeries> dataSeries)
        {
            Graphics = g;
            ChartArea = chartArea;
            DataSeries = dataSeries;
        }
    }

    internal sealed class OutlineChartPainter : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -1, -1);
            ctx.PlotRect = Rectangle.Empty;
            return ctx;
        }
        public override void DrawBackground(Graphics g, ChartLayout ctx)
        {
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var path = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, path);
            using var pen = new Pen(Theme?.StatsCardBorderColor ?? Color.Silver, 1);
            g.DrawPath(pen, path);
        }
        public override void DrawForeground(Graphics g, ChartLayout ctx) { }
    }

    internal sealed class GlassChartPainter2 : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            return ctx;
        }
        public override void DrawBackground(Graphics g, ChartLayout ctx)
        {
            using var glass = new SolidBrush(Color.FromArgb(28, Color.White));
            using var path = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(glass, path);
            using var pen = new Pen(Color.FromArgb(64, Color.White), 1);
            g.DrawPath(pen, path);
        }
        public override void DrawForeground(Graphics g, ChartLayout ctx)
        {
            var top = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, ctx.DrawingRect.Height / 3);
            using var lg = new LinearGradientBrush(top, Color.FromArgb(64, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical);
            g.FillRectangle(lg, top);
        }
    }
}