using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    public class BeepChart : BeepControl
    {
        #region Fields and Properties
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
                    AutoScaleViewport();
                    Invalidate();
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
                System.Diagnostics.Trace.WriteLine("BeepChart OnPaint Called");
                if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
                {
                    System.Diagnostics.Trace.WriteLine("Invalid client dimensions, skipping render");
                    e.Graphics.Clear(Color.White);
                    return;
                }

                 UpdateChartDrawingRect(e.Graphics);
                e.Graphics.Clear(DesignMode ? Color.White : ChartBackColor);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                if (DesignMode && DataSeries != null && DataSeries.Any())
                {
                    System.Diagnostics.Trace.WriteLine("Design-time rendering: Drawing sample data");
                    DrawAxes(e.Graphics);
                    DrawAxisTitles(e.Graphics);
                    // ← Add this
                    DrawAxisTicks(e.Graphics);
                    if (ChartType != ChartType.Pie)
                    {
                        System.Diagnostics.Trace.WriteLine("Drawing data series (design-time)");
                        DrawDataSeries(e.Graphics);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Drawing pie series (design-time)");
                        DrawPieSeries(e.Graphics);
                    }
                    if (ShowLegend && ChartType != ChartType.Pie)
                    {
                        System.Diagnostics.Trace.WriteLine("Drawing legend (design-time)");
                        DrawLegend(e.Graphics);
                    }
                }
                else if (CustomDraw)
                {
                    System.Diagnostics.Trace.WriteLine("Drawing custom series");
                    DrawCustomSeries(e.Graphics);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("Drawing axes");
                    DrawAxes(e.Graphics);
                    System.Diagnostics.Trace.WriteLine("Drawing axis labels");
                    DrawAxisTitles(e.Graphics);
                    DrawAxisTicks(e.Graphics);
                    if (ChartType != ChartType.Pie)
                    {
                        System.Diagnostics.Trace.WriteLine("Drawing data series");
                        DrawDataSeries(e.Graphics);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Drawing pie series");
                        DrawPieSeries(e.Graphics);
                    }
                }

                if ( ShowLegend )
                {
                    System.Diagnostics.Trace.WriteLine("Drawing legend");
                    DrawLegend(e.Graphics);
                }

                if (hoveredPoint != null && !DesignMode && dataPointToolTip != null)
                {
                    System.Diagnostics.Trace.WriteLine("Showing tooltip");
                    ShowTooltip(hoveredPoint);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"OnPaint Error: {ex.Message}\n{ex.StackTrace}");
                e.Graphics.Clear(Color.Red);
                e.Graphics.DrawString("Error rendering chart", new Font("Arial", 10), Brushes.White, 10, 10);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                float delta = e.Delta > 0 ? 0.9f : 1.1f;
                zoomFactor *= delta;
                zoomFactor = Math.Max(0.1f, Math.Min(10.0f, zoomFactor));
                AutoScaleViewport();
                Invalidate();
            }
            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                lastMouseDownPoint = e.Location;
                // Invalidate only if panning or state changes
                if (lastMouseDownPoint != Point.Empty)
                {
                    Invalidate();
                }
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
                    if ((DateTime.Now - lastInvalidateTime).TotalMilliseconds > 50) // Debounce 50ms
                    {
                        Invalidate();
                        lastInvalidateTime = DateTime.Now;
                    }
                }
                else
                {
                    ChartDataPoint newHoveredPoint = GetHoveredDataPoint(e.Location);
                    if (newHoveredPoint != hoveredPoint) // Invalidate only if hovered point changes
                    {
                        hoveredPoint = newHoveredPoint;
                        if ((DateTime.Now - lastInvalidateTime).TotalMilliseconds > 50) // Debounce 50ms
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
                //Invalidate only if state changes
                if ((DateTime.Now - lastInvalidateTime).TotalMilliseconds > 50) // Debounce 50ms
                {
                    Invalidate();
                    lastInvalidateTime = DateTime.Now;
                }
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

        #endregion

        #region Drawing Methods
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

        private void DrawDataSeries(Graphics g)
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

                    return new { DataPoint = p, ScreenX = xScreen, ScreenY = yScreen };
                }).ToList();

                // Draw lines / area if needed
                using (Pen pen = new Pen(series.Color != Color.Empty ? series.Color : ChartLineColor, 2))
                {
                    // etc...
                }

                // Draw points and labels
                if (series.ShowPoint)
                {
                    using (Font labelFont = new Font("Arial", 8))
                    using (Brush labelBrush = new SolidBrush(ChartTextColor))
                    {
                        foreach (var sp in screenPoints)
                        {
                            // Draw the point
                            g.FillEllipse(Brushes.Black, sp.ScreenX - 2, sp.ScreenY - 2, 4, 4);

                            if (series.ShowLabel)
                            {
                                // What text do you want?  
                                // e.g. "X=..., Y=..."
                                string labelText = $"({sp.DataPoint.X}, {sp.DataPoint.Y})";

                                // or use sp.DataPoint.ToolTip or sp.DataPoint.Label if you have that
                                // string labelText = sp.DataPoint.Label ?? $"{sp.DataPoint.X}, {sp.DataPoint.Y}";

                                // draw near the point
                                g.DrawString(labelText, labelFont, labelBrush,
                                             sp.ScreenX + 3, sp.ScreenY - 15);
                            }
                        }
                    }
                }
            }
        }

        private void DrawNumericAxisTicks(Graphics g, int numberOfXTicks, int numberOfYTicks)
        {
            // Only do this if the user set numeric axes:
            if (BottomAxisType != AxisType.Numeric || LeftAxisType != AxisType.Numeric)
                return;

            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;
            if (xRange <= 0 || yRange <= 0)
                return;

            using (Font tickFont = new Font("Arial", 8))
            using (Brush textBrush = new SolidBrush(ChartTextColor))
            using (Pen tickPen = new Pen(ChartGridLineColor, 1))
            {
                // ------------------
                // X Axis Ticks
                // ------------------
                float xStep = xRange / numberOfXTicks;
                for (int i = 0; i <= numberOfXTicks; i++)
                {
                    float val = ViewportXMin + i * xStep;
                    float xPos = ChartDrawingRect.Left
                                 + (val - ViewportXMin) / xRange * ChartDrawingRect.Width;

                    // small line below X axis
                    g.DrawLine(tickPen,
                        xPos, ChartDrawingRect.Bottom,
                        xPos, ChartDrawingRect.Bottom + 4);

                    // label
                    string label = val.ToString("0.##");
                    SizeF size = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, textBrush,
                        xPos - size.Width / 2,
                        ChartDrawingRect.Bottom + 4);
                }

                // ------------------
                // Y Axis Ticks
                // ------------------
                float yStep = yRange / numberOfYTicks;
                for (int i = 0; i <= numberOfYTicks; i++)
                {
                    float val = ViewportYMin + i * yStep;
                    float yPos = ChartDrawingRect.Bottom
                                 - (val - ViewportYMin) / yRange * ChartDrawingRect.Height;

                    // small line left of Y axis
                    g.DrawLine(tickPen,
                        ChartDrawingRect.Left - 4, yPos,
                        ChartDrawingRect.Left, yPos);

                    // label
                    string label = val.ToString("0.##");
                    SizeF size = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, textBrush,
                        ChartDrawingRect.Left - size.Width - 6,
                        yPos - size.Height / 2);
                }
            }
        }

        private void DrawPieSeries(Graphics g)
        {
            try
            {
                if (!DataSeries.Any() || !DataSeries.Any(s => s.Points != null && s.Points.Any())) return;

                var series = DataSeries.First();
                if (!series.Visible) return;

                float totalY = series.Points.Sum(p => Math.Max(ConvertYValue(p) is float yVal ? yVal : 0, 0));
                if (totalY <= 0) return;

                int pieDiameter = Math.Min(ChartDrawingRect.Width, ChartDrawingRect.Height) - 20;
                Rectangle pieRect = new Rectangle(
                    ChartDrawingRect.Left + (ChartDrawingRect.Width - pieDiameter) / 2,
                    ChartDrawingRect.Top + (ChartDrawingRect.Height - pieDiameter) / 2,
                    pieDiameter,
                    pieDiameter
                );

                float startAngle = 0f;
                int colorIndex = 0;

                foreach (var point in series.Points)
                {
                    float y = Math.Max(ConvertYValue(point) is float yVal ? yVal : 0, 0);
                    if (y <= 0) continue;

                    float sweepAngle = (y / totalY) * 360f;
                    Color sliceColor = point.Color != Color.Empty ? point.Color : ChartDefaultSeriesColors[colorIndex % ChartDefaultSeriesColors.Count];

                    using (Brush brush = new SolidBrush(sliceColor))
                    {
                        g.FillPie(brush, pieRect, startAngle, sweepAngle);
                    }

                    using (Pen pen = new Pen(ChartLineColor, 1))
                    {
                        g.DrawPie(pen, pieRect, startAngle, sweepAngle);
                    }

                    startAngle += sweepAngle;
                    colorIndex++;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"DrawPieSeries Error: {ex.Message}");
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<CustomDrawSeriesEventArgs> CustomDrawSeries;

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

                ViewportXMin = DataSeries.Min(s => s.Points.Min(p => ConvertXValue(p) is float x ? x : 0));
                ViewportXMax = DataSeries.Max(s => s.Points.Max(p => ConvertXValue(p) is float x ? x : 0));
                ViewportYMin = DataSeries.Min(s => s.Points.Min(p => ConvertYValue(p) is float y ? y : 0));
                ViewportYMax = DataSeries.Max(s => s.Points.Max(p => ConvertYValue(p) is float y ? y : 0));

                float xPadding = (ViewportXMax - ViewportXMin) * 0.1f;
                float yPadding = (ViewportYMax - ViewportYMin) * 0.1f;
                ViewportXMin -= xPadding; ViewportXMax += xPadding;
                ViewportYMin -= yPadding; ViewportYMax += yPadding;

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