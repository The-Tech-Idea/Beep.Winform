using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepChart), "BeepChart.bmp")]
    [Description("A custom chart control for WinForms.")]
    [DefaultProperty("DataSeries")]
    [DisplayName("BeepChart")]
    [Category("Beep Controls")]
    public class BeepChart : BeepControl
    {
        #region Fields and Properties
        private bool _showtitle= true;
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
                base.ApplyTheme();
                if (BeepThemesManager.ThemeChartColors.TryGetValue(Theme, out var chartColors))
                {
                    ChartBackColor = chartColors.ChartBackColor;
                    ChartLineColor = chartColors.ChartLineColor;
                    ChartFillColor = chartColors.ChartFillColor;
                    ChartAxisColor = chartColors.ChartAxisColor;
                    ChartTitleColor = chartColors.ChartTitleColor;
                    ChartTextColor = chartColors.ChartTextColor;
                    ChartLegendBackColor = chartColors.ChartLegendBackColor;
                    ChartLegendTextColor = chartColors.ChartLegendTextColor;
                    ChartLegendShapeColor = chartColors.ChartLegendShapeColor;
                    ChartGridLineColor = chartColors.ChartGridLineColor;
                    ChartDefaultSeriesColors = new List<Color>(chartColors.ChartDefaultSeriesColors);
                    ChartTitleFont = BeepThemesManager.ToFont(_currentTheme.TitleStyle);
                    ChartValueFont = _currentTheme.GetBlockHeaderFont();
                    ChartSubtitleFont = _currentTheme.GetBlockTextFont();
                    Invalidate();
                }
                else
                {
                    throw new ArgumentException($"Theme {Theme.ToString()} not found in ThemeChartColors.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"ApplyTheme Error: {ex.Message}");
            }
        }
        #endregion

        #endregion

        #region Constructor and Initialization
        public BeepChart()
        {
            try
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
                UpdateStyles();

                InitializeDefaultSettings();
                InitializeDesignTimeSampleData();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                UpdateDrawingRect();
                if (DrawingRect.Width <= 0 || DrawingRect.Height <= 0)
                    return;
                var g = e.Graphics;
                UpdateChartDrawingRect(e.Graphics);
                e.Graphics.Clear(DesignMode ? Color.White : ChartBackColor);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // Start drawing any Title/Value/Subtitle at top-left
                int textAreaLeft = DrawingRect.Left + 10;
                int currentY = DrawingRect.Top + 10;
                // Only draw if user wants to see these (i.e., ShowTitle == true)
                if (ShowTitle)
                {
                    // 1) Draw main title (e.g., "Revenue")
                    if (!string.IsNullOrEmpty(ChartTitle))
                    {
                        using (Brush titleBrush = new SolidBrush(ChartTitleForeColor))
                        {
                            SizeF titleSize = g.MeasureString(ChartTitle, ChartTitleFont);
                            g.DrawString(ChartTitle, ChartTitleFont, titleBrush, textAreaLeft, currentY);
                            currentY += (int)titleSize.Height + 5;
                        }
                    }

                    // 2) Draw big value (e.g., "$5,010.68")
                    if (!string.IsNullOrEmpty(ChartValue))
                    {
                        // If you want a separate color for the Value:
                        // using (Brush valueBrush = new SolidBrush(ChartValueForeColor))
                        // else just reuse the same color:
                        using (Brush valueBrush = new SolidBrush(ChartTitleForeColor))
                        {
                            SizeF valueSize = g.MeasureString(ChartValue, ChartValueFont);
                            g.DrawString(ChartValue, ChartValueFont, valueBrush, textAreaLeft, currentY);
                            currentY += (int)valueSize.Height + 5;
                        }
                    }

                    // 3) Draw subtitle (e.g., "from $4,430.41")
                    if (!string.IsNullOrEmpty(ChartSubtitle))
                    {
                        // If you want a separate color for the Subtitle:
                        // using (Brush subBrush = new SolidBrush(ChartSubtitleForeColor))
                        // else reuse the same color:
                        using (Brush subBrush = new SolidBrush(ChartTitleForeColor))
                        {
                            SizeF subSize = g.MeasureString(ChartSubtitle, ChartSubtitleFont);
                            g.DrawString(ChartSubtitle, ChartSubtitleFont, subBrush, textAreaLeft, currentY);
                            currentY += (int)subSize.Height + 10;
                        }
                    }
                }
                // Now that we used some space at the top for text, recalc the chart area
                int topPadding = currentY; // chart starts below the last text drawn
                int leftPadding = 40;
                int rightPadding = 30;
                int bottomPadding = 40;

                ChartDrawingRect = new Rectangle(
                    DrawingRect.Left + leftPadding,
                    topPadding,
                    DrawingRect.Width - leftPadding - rightPadding,
                    DrawingRect.Height - topPadding - bottomPadding
                );

                // Only draw axes if NOT pie
                if (ChartType != ChartType.Pie)
                {
                    DrawAxes(e.Graphics);         // the axis lines
                   
                    DrawAxisTitles(e.Graphics);   // axis titles
                    DrawAxisTicks(e.Graphics);    // numeric/text ticks (if you have them)
                }

                // Then draw data
                if (ChartType == ChartType.Pie)
                {
                    DrawPieSeries(e.Graphics);
                    DrawPieLegend(e.Graphics);
                }
                else
                {
                    if(ChartType== ChartType.Line)    DrawLineSeries(e.Graphics); //Line series
                    if (ChartType == ChartType.Bar)   DrawBarSeries(e.Graphics);    // bar series
                    if (ChartType == ChartType.Bubble) DrawBubbleSeries(e.Graphics); // bubble series
                    if (ChartType == ChartType.Area) DrawAreaSeries(e.Graphics);
                }

                // Draw legend if needed
                if (!DesignMode && ShowLegend  && ChartType!= ChartType.Pie)
                {
                    DrawLegend(e.Graphics);
                }

                if (hoveredPoint != null && dataPointToolTip != null)
                {
                    ShowTooltip(hoveredPoint);
                }
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
                float zoomFactorChange = e.Delta > 0 ? 0.9f : 1.1f;

                float mouseXRatio = (e.X - ChartDrawingRect.Left) / (float)ChartDrawingRect.Width;
                float mouseYRatio = 1.0f - (e.Y - ChartDrawingRect.Top) / (float)ChartDrawingRect.Height;

                float newXMin = ViewportXMin + (ViewportXMax - ViewportXMin) * mouseXRatio * (1 - zoomFactorChange);
                float newXMax = ViewportXMax - (ViewportXMax - ViewportXMin) * (1 - mouseXRatio) * (1 - zoomFactorChange);
                float newYMin = ViewportYMin + (ViewportYMax - ViewportYMin) * mouseYRatio * (1 - zoomFactorChange);
                float newYMax = ViewportYMax - (ViewportYMax - ViewportYMin) * (1 - mouseYRatio) * (1 - zoomFactorChange);

                ViewportXMin = newXMin;
                ViewportXMax = newXMax;
                ViewportYMin = newYMin;
                ViewportYMax = newYMax;

                zoomFactor *= zoomFactorChange;
                zoomFactor = Math.Max(0.1f, Math.Min(10.0f, zoomFactor));

                Invalidate();
            }
            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                lastMouseDownPoint = e.Location;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                if (e.Button == MouseButtons.Left && lastMouseDownPoint != Point.Empty)
                {
                    int deltaX = lastMouseDownPoint.X - e.X;
                    int deltaY = lastMouseDownPoint.Y - e.Y;
                    PanViewport(deltaX, deltaY);
                    lastMouseDownPoint = e.Location;

                    if ((DateTime.Now - lastInvalidateTime).TotalMilliseconds > 50)
                    {
                        Invalidate();
                        lastInvalidateTime = DateTime.Now;
                    }
                }
                else
                {
                    ChartDataPoint newHoveredPoint = GetHoveredDataPoint(e.Location);
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
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                lastMouseDownPoint = Point.Empty;
                Invalidate();
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
            // You might check the first non-empty series to guess X or Y axis types
            var firstSeries = _dataSeries.FirstOrDefault(s => s.Points != null && s.Points.Any());
            if (firstSeries != null)
            {
                // Check the first point’s X and Y, or a sampling
                string exampleX = firstSeries.Points[0].X;
                string exampleY = firstSeries.Points[0].Y;

                // Try numeric
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

        #region Drawing Methods
        private void DrawBubbleSeries(Graphics g)
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.Points != null && s.Points.Any()))
                    return;

                float maxBubbleSize = 50f; // Max bubble diameter in pixels
                float minBubbleSize = 5f;  // Min bubble diameter in pixels

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
                        float bubbleSize = (point.Value / maxValue) * maxBubbleSize;
                        bubbleSize = Math.Max(bubbleSize, minBubbleSize);

                        float screenX = ChartDrawingRect.Left + ((x - minX) / (maxX - minX)) * ChartDrawingRect.Width;
                        float screenY = ChartDrawingRect.Bottom - ((y - minY) / (maxY - minY)) * ChartDrawingRect.Height;

                        Color bubbleColor = point.Color != Color.Empty
                            ? point.Color
                            : ChartDefaultSeriesColors[DataSeries.IndexOf(series) % ChartDefaultSeriesColors.Count];

                        using (Brush brush = new SolidBrush(Color.FromArgb(150, bubbleColor))) // Semi-transparent
                        {
                            g.FillEllipse(brush, screenX - bubbleSize / 2, screenY - bubbleSize / 2, bubbleSize, bubbleSize);
                        }
                        using (Pen pen = new Pen(ChartLineColor, 1))
                        {
                            g.DrawEllipse(pen, screenX - bubbleSize / 2, screenY - bubbleSize / 2, bubbleSize, bubbleSize);
                        }

                        // Draw value inside the bubble
                        using (Font labelFont = new Font("Arial", 8))
                        using (Brush textBrush = new SolidBrush(ChartTextColor))
                        {
                            string bubbleLabel = point.Value.ToString();
                            SizeF textSize = g.MeasureString(bubbleLabel, labelFont);
                            g.DrawString(bubbleLabel, labelFont, textBrush,
                                         screenX - textSize.Width / 2,
                                         screenY - textSize.Height / 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawBubbleSeries Error: {ex.Message}");
            }
        }
        private void UpdateChartDrawingRectBase()
        {
            try
            {
                int leftPadding = 40;  // Increased base padding for Y-axis title and labels
                int rightPadding = 30; // Increased base padding for legend or buffer
                int topPadding = 40;   // Increased base padding for X-axis title
                int bottomPadding = 40; // Increased base padding for X-axis labels

                ChartDrawingRect = new Rectangle(
                    ClientRectangle.Left + leftPadding,
                    ClientRectangle.Top + topPadding,
                    ClientRectangle.Width - leftPadding - rightPadding,
                    ClientRectangle.Height - topPadding - bottomPadding
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"UpdateChartDrawingRectBase Error: {ex.Message}");
                ChartDrawingRect = ClientRectangle;
            }
        }
        private void UpdateChartDrawingRect(Graphics g)
        {
            try
            {
                if (isDrawinRectCalculated) return;
                isDrawinRectCalculated = true;
                int leftPadding = 40;  // base padding for Y-axis title and labels
                int rightPadding = 30; // base padding for the legend area
                int topPadding = 40;   // base padding for X-axis title
                int bottomPadding = 40; // base padding for X-axis labels

                if (g != null)
                {
                    using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
                    using (Font labelFont = new Font("Arial", 8))
                    {
                        // 1) Adjust padding for axis titles
                        if (!string.IsNullOrEmpty(YAxisTitle))
                            leftPadding += (int)g.MeasureString(YAxisTitle, titleFont).Height + 15; // rotated text
                        if (!string.IsNullOrEmpty(XAxisTitle))
                            topPadding += (int)g.MeasureString(XAxisTitle, titleFont).Height + 10;

                        // 2) Adjust bottom padding for X-axis labels
                        bottomPadding += (int)g.MeasureString("0", labelFont).Height + 10;

                        // 3) If we are showing a legend, measure the widest series name
                        if (ShowLegend && DataSeries.Any(s => s.ShowInLegend && s.Visible))
                        {
                            using (Font legendFont = new Font("Arial", 8))
                            {
                                int maxLegendWidth = 0;
                                foreach (var series in DataSeries)
                                {
                                    if (!series.ShowInLegend || !series.Visible)
                                        continue;

                                    string seriesName = string.IsNullOrEmpty(series.Name)
                                                        ? "Series"
                                                        : series.Name;

                                    // measure series text
                                    int textWidth = (int)g.MeasureString(seriesName, legendFont).Width;
                                    // add a bit for the color swatch + padding
                                    textWidth += 20;
                                    if (textWidth > maxLegendWidth)
                                        maxLegendWidth = textWidth;
                                }

                                // Pad the right side so the legend text doesn’t get cut off
                                rightPadding += maxLegendWidth + 10;
                            }
                        }
                    }
                }

                // Finally, define the chart area
                ChartDrawingRect = new Rectangle(
                    ClientRectangle.Left + leftPadding,
                    ClientRectangle.Top + topPadding,
                    ClientRectangle.Width - leftPadding - rightPadding,
                    ClientRectangle.Height - topPadding - bottomPadding
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"UpdateChartDrawingRect Error: {ex.Message}");
                ChartDrawingRect = ClientRectangle;
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

                // Convert all data points to screen coordinates first
                var screenPoints = series.Points.Select(p =>
                {
                    float xVal = (ConvertXValue(p) is float xv) ? xv : 0;
                    float yVal = (ConvertYValue(p) is float yv) ? yv : 0;

                    float xScreen = ChartDrawingRect.Left +
                        (xVal - ViewportXMin) / (ViewportXMax - ViewportXMin) * ChartDrawingRect.Width;

                    float yScreen = ChartDrawingRect.Bottom -
                        (yVal - ViewportYMin) / (ViewportYMax - ViewportYMin) * ChartDrawingRect.Height;

                    return new PointF(xScreen, yScreen);
                }).ToList();

                if (screenPoints.Count < 2)
                    continue;

                // Create the area polygon points (including baseline)
                var areaPoints = new List<PointF>(screenPoints)
        {
            new PointF(screenPoints.Last().X, ChartDrawingRect.Bottom), // Bottom right corner
            new PointF(screenPoints.First().X, ChartDrawingRect.Bottom) // Bottom left corner
        };

                // Fill the area under the curve
                using (Brush areaBrush = new SolidBrush(Color.FromArgb(100, series.Color)))
                {
                    g.FillPolygon(areaBrush, areaPoints.ToArray());
                }

                // Draw the boundary line
                using (Pen pen = new Pen(series.Color != Color.Empty ? series.Color : ChartLineColor, 2))
                {
                    g.DrawLines(pen, screenPoints.ToArray());
                }

                // Draw points if needed
                if (series.ShowPoint)
                {
                    using (Brush pointBrush = new SolidBrush(series.Color))
                    {
                        foreach (var point in screenPoints)
                        {
                            g.FillEllipse(pointBrush, point.X - 3, point.Y - 3, 6, 6);
                        }
                    }
                }
            }
        }
        private void DrawAxes(Graphics g)
        {
            using (Pen pen = new Pen(ChartAxisColor, 1))
            {
                g.DrawLine(pen, ChartDrawingRect.Left, ChartDrawingRect.Bottom, ChartDrawingRect.Right, ChartDrawingRect.Bottom);
                g.DrawLine(pen, ChartDrawingRect.Left, ChartDrawingRect.Bottom, ChartDrawingRect.Left, ChartDrawingRect.Top);
            }
        }
        private void DrawAxisTitles(Graphics g)
        {
            using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
            using (Font labelFont = new Font("Arial", 8))
            using (Brush brush = new SolidBrush(ChartTextColor))
            {
                // Draw X-axis title centered along the bottom
                if (!string.IsNullOrEmpty(XAxisTitle))
                {
                    float titleWidth = g.MeasureString(XAxisTitle, titleFont).Width;
                    float xPosition = ChartDrawingRect.Left + (ChartDrawingRect.Width - titleWidth) / 2; // Center horizontally
                    float yPosition = ChartDrawingRect.Bottom + 10; // Below the axis
                    g.DrawString(XAxisTitle, titleFont, brush, xPosition, yPosition);
                }

                // Draw Y-axis title centered along the left, rotated 90 degrees
                if (!string.IsNullOrEmpty(YAxisTitle))
                {
                    float titleWidth = g.MeasureString(YAxisTitle, titleFont).Width;
                    float titleHeight = g.MeasureString(YAxisTitle, titleFont).Height;
                    float xPosition = ChartDrawingRect.Left - 30; // Left of the axis
                    float yPosition = ChartDrawingRect.Top + (ChartDrawingRect.Height + titleWidth) / 2; // Center vertically, adjusted for rotation
                    g.TranslateTransform(xPosition, yPosition);
                    g.RotateTransform(-90);
                    g.DrawString(YAxisTitle, titleFont, brush, 0, 0);
                    g.ResetTransform();
                }

                // Draw axis labels at the ends
                //g.DrawString(XAxisTitle, labelFont, brush, ChartDrawingRect.Right - 15, ChartDrawingRect.Bottom + 5);
                //g.DrawString(YAxisTitle, labelFont, brush, ChartDrawingRect.Left - 15, ChartDrawingRect.Top + 5);
            }
        }
        private void DrawLineSeries(Graphics g)
        {
            if (!DataSeries.Any())
                return;

            foreach (var series in DataSeries)
            {
                if (!series.Visible || series.Points == null || !series.Points.Any())
                    continue;

                // Convert all data points to screen coordinates first
                var screenPoints = series.Points.Select(p =>
                {
                    float xVal = (ConvertXValue(p) is float xv) ? xv : 0;
                    float yVal = (ConvertYValue(p) is float yv) ? yv : 0;

                    float xScreen = ChartDrawingRect.Left +
                        (xVal - ViewportXMin) / (ViewportXMax - ViewportXMin) * ChartDrawingRect.Width;

                    float yScreen = ChartDrawingRect.Bottom -
                        (yVal - ViewportYMin) / (ViewportYMax - ViewportYMin) * ChartDrawingRect.Height;

                    return new PointF(xScreen, yScreen);
                }).ToList();

                // Draw lines if needed
                if (series.ShowLine && screenPoints.Count > 1)
                {
                    using (Pen pen = new Pen(series.Color != Color.Empty ? series.Color : ChartLineColor, 2))
                    {
                        g.DrawLines(pen, screenPoints.ToArray());
                    }
                }

                // Draw points and labels
                if (series.ShowPoint)
                {
                    using (Font labelFont = new Font("Arial", 8))
                    using (Brush labelBrush = new SolidBrush(ChartTextColor))
                    using (Brush pointBrush = new SolidBrush(series.Color != Color.Empty ? series.Color : Color.Black))
                    {
                        foreach (var sp in screenPoints)
                        {
                            // Draw the point
                            g.FillEllipse(pointBrush, sp.X - 2, sp.Y - 2, 4, 4);

                            if (series.ShowLabel)
                            {
                                // Define label text
                                string labelText = $"({sp.X:0.##}, {sp.Y:0.##})";

                                // Draw label near the point
                                g.DrawString(labelText, labelFont, labelBrush,
                                             sp.X + 3, sp.Y - 15);
                            }
                        }
                    }
                }
            }
        }
        private void DrawBarSeries(Graphics g)
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.Points != null && s.Points.Any()))
                    return;

                Dictionary<string, int> yCategories = new Dictionary<string, int>();
                int yCategoryIndex = 0;

                foreach (var series in DataSeries)
                {
                    foreach (var point in series.Points)
                    {
                        if (ConvertYValue(point) is string yStr)
                        {
                            if (!yCategories.ContainsKey(yStr))
                                yCategories[yStr] = yCategoryIndex++;
                        }
                    }
                }

                int barWidth = ChartDrawingRect.Width / DataSeries.First().Points.Count / DataSeries.Count;
                int seriesIndex = 0;
                float maxValue = DataSeries.SelectMany(s => s.Points).Max(p => p.Value);

                foreach (var series in DataSeries)
                {
                    if (!series.Visible) continue;

                    int pointIndex = 0;
                    foreach (var point in series.Points)
                    {
                        float barX = ChartDrawingRect.Left + (pointIndex * barWidth * DataSeries.Count) + (seriesIndex * barWidth);
                        float barHeight = (point.Value / maxValue) * ChartDrawingRect.Height;

                        float barY;
                        if (ConvertYValue(point) is string yStr && yCategories.ContainsKey(yStr))
                        {
                            barY = ChartDrawingRect.Bottom - ((yCategories[yStr] + 1) / (float)yCategories.Count * ChartDrawingRect.Height);
                        }
                        else
                        {
                            barY = ChartDrawingRect.Bottom - barHeight;
                        }

                        Color barColor = point.Color != Color.Empty
                            ? series.Color
                            : ChartDefaultSeriesColors[seriesIndex % ChartDefaultSeriesColors.Count];

                        using (Brush brush = new SolidBrush(barColor))
                        {
                            g.FillRectangle(brush, barX, barY, barWidth - 2, barHeight);
                        }

                        using (Pen pen = new Pen(ChartLineColor, 1))
                        {
                            g.DrawRectangle(pen, barX, barY, barWidth - 2, barHeight);
                        }

                        // Draw Y-label near the bar
                        using (Font labelFont = new Font("Arial", 8))
                        using (Brush textBrush = new SolidBrush(ChartTextColor))
                        {
                            string barLabel = point.Y.ToString();
                            SizeF textSize = g.MeasureString(barLabel, labelFont);
                            g.DrawString(barLabel, labelFont, textBrush,
                                         barX + (barWidth - textSize.Width) / 2,
                                         barY - textSize.Height - 2);
                        }

                        pointIndex++;
                    }
                    seriesIndex++;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawBarSeries Error: {ex.Message}");
            }
        }
        private void DrawBarSeriesUsingValue(Graphics g)
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.Points != null && s.Points.Any()))
                    return;

                int barWidth = ChartDrawingRect.Width / DataSeries.First().Points.Count / DataSeries.Count;
                int seriesIndex = 0;
                float maxValue = DataSeries.SelectMany(s => s.Points).Max(p => p.Value);

                foreach (var series in DataSeries)
                {
                    if (!series.Visible) continue;

                    int pointIndex = 0;
                    foreach (var point in series.Points)
                    {
                        float barHeight = (point.Value / maxValue) * ChartDrawingRect.Height;
                        int barX = ChartDrawingRect.Left + (pointIndex * barWidth * DataSeries.Count) + (seriesIndex * barWidth);
                        int barY = ChartDrawingRect.Bottom - (int)barHeight;

                        Color barColor = point.Color != Color.Empty
                            ? point.Color
                            : ChartDefaultSeriesColors[seriesIndex % ChartDefaultSeriesColors.Count];

                        using (Brush brush = new SolidBrush(barColor))
                        {
                            g.FillRectangle(brush, barX, barY, barWidth - 2, barHeight);
                        }

                        using (Pen pen = new Pen(ChartLineColor, 1))
                        {
                            g.DrawRectangle(pen, barX, barY, barWidth - 2, barHeight);
                        }

                        // Draw value on top of the bar
                        using (Font labelFont = new Font("Arial", 8))
                        using (Brush textBrush = new SolidBrush(ChartTextColor))
                        {
                            string barLabel = point.Value.ToString();
                            SizeF textSize = g.MeasureString(barLabel, labelFont);
                            g.DrawString(barLabel, labelFont, textBrush,
                                         barX + (barWidth - textSize.Width) / 2,
                                         barY - textSize.Height - 2);
                        }

                        pointIndex++;
                    }
                    seriesIndex++;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawBarSeries Error: {ex.Message}");
            }
        }
        private void DrawPieSeries(Graphics g)
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.Points != null && s.Points.Any()))
                    return;

                var series = DataSeries.First();
                if (!series.Visible)
                    return;

                float totalValue = series.Points.Sum(p => p.Value);
                if (totalValue <= 0)
                    return;

                int pieDiameter = Math.Min(ChartDrawingRect.Width, ChartDrawingRect.Height) - 20;
                Rectangle pieRect = new Rectangle(
                    ChartDrawingRect.Left + (ChartDrawingRect.Width - pieDiameter) / 2,
                    ChartDrawingRect.Top + (ChartDrawingRect.Height - pieDiameter) / 2,
                    pieDiameter,
                    pieDiameter
                );

                float centerX = pieRect.Left + pieDiameter / 2f;
                float centerY = pieRect.Top + pieDiameter / 2f;
                float radius = pieDiameter / 2f;

                float startAngle = 0f;
                int colorIndex = 0;

                using (Font labelFont = new Font("Arial", 8))
                using (Brush labelBrush = new SolidBrush(ChartTextColor))
                {
                    foreach (var point in series.Points)
                    {
                        float sliceValue = point.Value;
                        if (sliceValue <= 0) continue;

                        float sweepAngle = (sliceValue / totalValue) * 360f;
                        Color sliceColor = point.Color != Color.Empty
                            ? point.Color
                            : ChartDefaultSeriesColors[colorIndex % ChartDefaultSeriesColors.Count];

                        using (Brush brush = new SolidBrush(sliceColor))
                        {
                            g.FillPie(brush, pieRect, startAngle, sweepAngle);
                        }

                        using (Pen pen = new Pen(ChartLineColor, 1))
                        {
                            g.DrawPie(pen, pieRect, startAngle, sweepAngle);
                        }

                        // Draw labels inside slices
                        float midAngle = startAngle + sweepAngle / 2f;
                        double midAngleRad = midAngle * (Math.PI / 180f);

                        // Position label inside pie slice
                        float labelRadius = radius * 0.6f;
                        float labelX = centerX + (float)Math.Cos(midAngleRad) * labelRadius;
                        float labelY = centerY + (float)Math.Sin(midAngleRad) * labelRadius;

                        string sliceLabel = $"{sliceValue}"; // or customize as desired

                        SizeF textSize = g.MeasureString(sliceLabel, labelFont);
                        using (Brush textBrush = new SolidBrush(ChartTextColor))
                        {
                            g.DrawString(sliceLabel, labelFont, textBrush,
                                         labelX - textSize.Width / 2,
                                         labelY - textSize.Height / 2);
                        }

                        startAngle += sweepAngle;
                        colorIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawPieSeries Error: {ex.Message}");
            }
        }
        private void DrawPieLegend(Graphics g)
        {
            // Typically you only support one 'pie' series in the chart:
            var pieSeries = DataSeries.FirstOrDefault(s => s.ChartType == ChartType.Pie && s.Visible);
            if (pieSeries == null || !pieSeries.Points.Any())
                return;

            // Decide where to place your legend – for example, to the right of the chart:
            int legendX = ChartDrawingRect.Right + 10;
            int legendY = ChartDrawingRect.Top;
            int itemHeight = 20;

            using (Font font = new Font("Arial", 8))
            using (Brush textBrush = new SolidBrush(ChartLegendTextColor))
            using (Brush backBrush = new SolidBrush(ChartLegendBackColor))
            {
                // We’ll figure out how tall to make the background box:
                int legendHeight = pieSeries.Points.Count * itemHeight + 10;
                int legendWidth = 120; // Or measure the largest text

                // Draw a background rectangle behind the legend items
                g.FillRectangle(backBrush, legendX, legendY, legendWidth, legendHeight);

                int currentY = legendY + 2;
                int colorIndex = 0;

                foreach (var point in pieSeries.Points)
                {
                    // Pick color for this slice
                    Color sliceColor = point.Color != Color.Empty
                        ? point.Color
                        : ChartDefaultSeriesColors[colorIndex % ChartDefaultSeriesColors.Count];

                    // Draw a small color swatch
                    using (Brush swatchBrush = new SolidBrush(sliceColor))
                    {
                        g.FillRectangle(swatchBrush, legendX + 2, currentY, 15, 15);
                    }
                    // Outline it
                    using (Pen pen = new Pen(ChartLegendShapeColor, 1))
                    {
                        g.DrawRectangle(pen, legendX + 2, currentY, 15, 15);
                    }

                    // Decide what text to show for this slice
                    // If you’re using the .X property for the slice name:
                    string labelText = !string.IsNullOrEmpty(point.X)
                        ? point.X
                        : $"Slice {colorIndex + 1}";

                    // Draw text to the right of the color swatch
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
        private void DrawCustomSeries(Graphics g)
        {
            try
            {
                if (CustomDrawSeries != null && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                {
                    var args = new CustomDrawSeriesEventArgs(g, ChartDrawingRect, DataSeries);
                    CustomDrawSeries?.Invoke(this, args);
                }
                else
                {
                    using (Font font = new Font("Arial", 10))
                    using (Brush brush = new SolidBrush(ChartTextColor))
                    {
                        g.DrawString("Custom drawing not implemented", font, brush, ChartDrawingRect.Left + 10, ChartDrawingRect.Top + 10);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawCustomSeries Error: {ex.Message}");
            }
        }
        private void DrawAxisTicks(Graphics g)
        {
            // We'll draw X-axis ticks, then Y-axis ticks.
            // That way, we keep them separate but in one method.

            if (ChartDrawingRect.Width <= 0 || ChartDrawingRect.Height <= 0)
                return;

            // 1) Draw ticks/labels for the BOTTOM (X) axis.
            DrawTicksForAxis(
                g,
                isXAxis: true,
                axisType: BottomAxisType,
                minValue: ViewportXMin,
                maxValue: ViewportXMax,
                categoryMap: xAxisCategories,
                dateMin: xAxisDateMin,
                numberOfTicks: 5 // tweak as needed
            );

            // 2) Draw ticks/labels for the LEFT (Y) axis.
            DrawTicksForAxis(
                g,
                isXAxis: false,
                axisType: LeftAxisType,
                minValue: ViewportYMin,
                maxValue: ViewportYMax,
                categoryMap: yAxisCategories,
                dateMin: yAxisDateMin,
                numberOfTicks: 5 // tweak as needed
            );
        }
        private void DrawTicksForAxis(Graphics g,
                              bool isXAxis,
                              AxisType axisType,
                              float minValue,
                              float maxValue,
                              Dictionary<string, int> categoryMap,
                              DateTime dateMin,
                              int numberOfTicks)
        {
            // We'll draw 'numberOfTicks' labels for numeric/date axes,
            // but for text axes, we'll show one label per category.
            using (Font tickFont = new Font("Arial", 8))
            using (Brush textBrush = new SolidBrush(ChartTextColor))
            using (Pen tickPen = new Pen(ChartGridLineColor, 1))
            {
                // For convenience:
                float axisLength = isXAxis
                    ? ChartDrawingRect.Width
                    : ChartDrawingRect.Height;

                // For numeric or date, we do an even spacing.
                // For text, we read directly from the category map.

                switch (axisType)
                {
                    case AxisType.Numeric:
                        DrawNumericTicks(g, isXAxis, minValue, maxValue,
                                         numberOfTicks, tickFont, textBrush, tickPen);
                        break;

                    case AxisType.Date:
                        DrawDateTicks(g, isXAxis, minValue, maxValue, dateMin,
                                      numberOfTicks, tickFont, textBrush, tickPen);
                        break;

                    case AxisType.Text:
                        DrawTextCategoryTicks(g, isXAxis, categoryMap,
                                              tickFont, textBrush, tickPen);
                        break;
                }
            }
        }

        private void DrawNumericTicks(Graphics g, bool isXAxis,
                              float minVal, float maxVal,
                              int numberOfTicks,
                              Font tickFont, Brush textBrush, Pen tickPen)
        {
            if (maxVal <= minVal) return;

            float range = maxVal - minVal;
            float step = range / numberOfTicks;

            for (int i = 0; i <= numberOfTicks; i++)
            {
                float val = minVal + i * step;
                // Convert that data value to screen
                PointF pos = isXAxis
          ? ValueToScreen(val, 0, true)
          : ValueToScreen(0, val, false);

                // Draw a small tick line
                if (isXAxis)
                {
                    // bottom of chart
                    g.DrawLine(tickPen, pos.X, ChartDrawingRect.Bottom,
                                         pos.X, ChartDrawingRect.Bottom + 4);

                    // numeric label
                    string label = val.ToString("0.##");
                    SizeF size = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, textBrush,
                                 pos.X - size.Width / 2,
                                 ChartDrawingRect.Bottom + 4);
                }
                else
                {
                    // left of chart
                    g.DrawLine(tickPen, ChartDrawingRect.Left - 4, pos.Y,
                                         ChartDrawingRect.Left, pos.Y);

                    string label = val.ToString("0.##");
                    SizeF size = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, textBrush,
                                 ChartDrawingRect.Left - size.Width - 6,
                                 pos.Y - size.Height / 2);
                }
            }
        }
        private PointF ValueToScreen(float xValue, float yValue, bool isXAxis)
        {
            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;

            // If we are plotting a tick on the X axis, 
            // we only care about xValue -> horizontal position:
            float xPos = ChartDrawingRect.Left +
                (xValue - ViewportXMin) / xRange * ChartDrawingRect.Width;
            float yPos = ChartDrawingRect.Bottom -
                (yValue - ViewportYMin) / yRange * ChartDrawingRect.Height;

            return new PointF(xPos, yPos);
        }
        private void DrawDateTicks(Graphics g, bool isXAxis,
                           float minVal, float maxVal,
                           DateTime dateMin,
                           int numberOfTicks,
                           Font tickFont, Brush textBrush, Pen tickPen)
        {
            if (maxVal <= minVal) return;

            float range = maxVal - minVal; // in "days" if that's how you do the offset
            float step = range / numberOfTicks;

            for (int i = 0; i <= numberOfTicks; i++)
            {
                float val = minVal + i * step;
                // Convert float offset back to a DateTime
                // This means: dateMin + val "days"
                DateTime tickDate = dateMin.AddDays(val);

                // Convert to screen
                PointF pos = ValueToScreen(val, 0, isXAxis);

                // Draw the tick line
                if (isXAxis)
                {
                    g.DrawLine(tickPen, pos.X, ChartDrawingRect.Bottom,
                                         pos.X, ChartDrawingRect.Bottom + 4);
                    // Format the date label as needed
                    string label = tickDate.ToString("MM/dd"); // or "yyyy-MM-dd", etc.
                    SizeF size = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, textBrush,
                                 pos.X - size.Width / 2,
                                 ChartDrawingRect.Bottom + 4);
                }
                else
                {
                    g.DrawLine(tickPen, ChartDrawingRect.Left - 4, pos.Y,
                                         ChartDrawingRect.Left, pos.Y);
                    string label = tickDate.ToString("MM/dd");
                    SizeF size = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, textBrush,
                                 ChartDrawingRect.Left - size.Width - 6,
                                 pos.Y - size.Height / 2);
                }
            }
        }
        private void DrawTextCategoryTicks(Graphics g, bool isXAxis,
                                   Dictionary<string, int> categoryMap,
                                   Font tickFont, Brush textBrush, Pen tickPen)
        {
            if (categoryMap == null || categoryMap.Count == 0)
                return;

            // Sort categories by their assigned index
            var sortedCategories = categoryMap
                .OrderBy(kvp => kvp.Value)
                .ToList();

            foreach (var kvp in sortedCategories)
            {
                string categoryName = kvp.Key;
                float categoryIndex = kvp.Value; // e.g., 0, 1, 2, etc.

                PointF pos;
                if (isXAxis)
                    pos = ValueToScreen(categoryIndex, 0, isXAxis: true);
                else
                    pos = ValueToScreen(0, categoryIndex, isXAxis: false);

                // Draw the tick line
                if (isXAxis)
                {
                    g.DrawLine(tickPen, pos.X, ChartDrawingRect.Bottom,
                                         pos.X, ChartDrawingRect.Bottom + 4);

                    SizeF size = g.MeasureString(categoryName, tickFont);
                    g.DrawString(categoryName, tickFont, textBrush,
                                 pos.X - size.Width / 2,
                                 ChartDrawingRect.Bottom + 4);
                }
                else
                {
                    g.DrawLine(tickPen, ChartDrawingRect.Left - 4, pos.Y,
                                         ChartDrawingRect.Left, pos.Y);

                    SizeF size = g.MeasureString(categoryName, tickFont);
                    g.DrawString(categoryName, tickFont, textBrush,
                                 ChartDrawingRect.Left - size.Width - 6,
                                 pos.Y - size.Height / 2);
                }
            }
        }

        #endregion

        #region Interaction Methods
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
                dataPointToolTip.Show(tooltipText, this, PointToClient(Control.MousePosition), 3000);
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

   
}