using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum AxisType
    {
        Numeric,
        Text,
        Date
    }

    public enum TextAlignment
    {
        Horizontal,
        Vertical,
        Diagonal45
    }

    public enum ChartType
    {
        Line,
        Bar,
        Pie,
        Bubble
    }
    public enum ChartStyle
    {
        Light,
        Dark
    }
    public enum ChartLegendPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public enum ChartLegendAlignment
    {
        Center,
        Start,
        End
    }
    public enum ChartLegendOrientation
    {
        Horizontal,
        Vertical
    }
    public enum ChartDataPointStyle
    {
        Circle,
        Square,
        Diamond,
        Triangle
    }
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepChartBase))]
    [DefaultProperty("ChartDataSeries")]
    public class BeepChartBase : BeepControl
    {

        // Chart Properties
        public ChartType ChartType { get; set; } = ChartType.Line;
        public ChartStyle ChartStyle { get; set; } = ChartStyle.Light;
        // Legend labels for each data series
        public List<string> LegendLabels { get; set; } = new List<string>();

        // Legend Properties
        private int legendPadding = 20; // Space reserved for legends
        public bool ShowLegend { get; set; } = true;
        public ChartLegendPosition LegendPosition { get; set; } = ChartLegendPosition.Right;
        public ChartLegendAlignment LegendAlignment { get; set; } = ChartLegendAlignment.Center;
        public ChartLegendOrientation LegendOrientation { get; set; } = ChartLegendOrientation.Vertical;
        public ChartDataPointStyle LegendStyle { get; set; } = ChartDataPointStyle.Circle;
        // Data series
        public List<ChartDataSeries> DataSeries { get; set; } = new List<ChartDataSeries>();
        private ToolTip dataPointToolTip = new ToolTip();
        private ChartDataPoint lastHoveredDataPoint = null; // Track the last data point hovered over
        public bool ShowLeftAxisTitle { get; set; } = true;
        public bool ShowRightAxisTitle { get; set; } = false;
        public bool ShowBottomAxisTitle { get; set; } = true;
        public bool ShowTopAxisTitle { get; set; } = false;

        // Axis Titles
        public string LeftAxisTitle { get; set; } = "Left Axis";
        public string RightAxisTitle { get; set; } = "Right Axis";
        public string BottomAxisTitle { get; set; } = "Bottom Axis";
        public string TopAxisTitle { get; set; } = "Top Axis";

        // Axis Types and Alignment
        public AxisType LeftAxisType { get; set; } = AxisType.Numeric;
        public AxisType RightAxisType { get; set; } = AxisType.Numeric;
        public AxisType BottomAxisType { get; set; } = AxisType.Numeric;
        public AxisType TopAxisType { get; set; } = AxisType.Numeric;

        public TextAlignment LeftTitleAlignment { get; set; } = TextAlignment.Vertical;
        public TextAlignment RightTitleAlignment { get; set; } = TextAlignment.Vertical;
        public TextAlignment BottomTitleAlignment { get; set; } = TextAlignment.Horizontal;
        public TextAlignment TopTitleAlignment { get; set; } = TextAlignment.Horizontal;

        // Options for displaying axes
        public bool ShowTopAxis { get; set; } = false;
        public bool ShowRightAxis { get; set; } = false;

        // Zoom and Scroll properties
        public float ViewportXMin { get; private set; } = 0;
        public float ViewportXMax { get; private set; } = 10;
        public float ViewportYMin { get; private set; } = 0;
        public float ViewportYMax { get; private set; } = 10;

        // Padding for the chart area
        public Padding ChartPadding { get; set; } = new Padding(40, 20, 40, 40);
        protected Rectangle ChartDrawingRect;

        private Point lastMousePosition;
        private bool isDragging = false;
        public bool ShowGridLines { get; set; } = true;
        private bool _customDraw = false;
        public bool CustomDraw
        {
            get { return _customDraw; }
            set { _customDraw = value;Invalidate(); }
        }
        public BeepChartBase()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 200;
            }
            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            if (!DesignMode)
            {
                InitializeSampleData();
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Only initialize sample data if in design mode
            if (DesignMode)
            {
                InitializeSampleData();
                Invalidate(); // Trigger redraw to display sample data
            }
        }

        private void InitializeSampleData()
        {
            // Define sample data for the first series
            var sampleDataSeries1 = new List<ChartDataPoint>
    {
        new ChartDataPoint { X = 1, Y = 5, Size = 10, Label = "A", ToolTip = "Test 1", Color = Color.Red },
        new ChartDataPoint { X = 2, Y = 15, Size = 15, Label = "B", ToolTip = "Test 2", Color = Color.Green },
        new ChartDataPoint { X = 3, Y = 8, Size = 20, Label = "C", ToolTip = "Test 3", Color = Color.Blue },
        new ChartDataPoint { X = 4, Y = 12, Size = 25, Label = "D", ToolTip = "Test 4", Color = Color.Purple },
        new ChartDataPoint { X = 5, Y = 20, Size = 18, Label = "E", ToolTip = "Test 5", Color = Color.Orange }
    };

            // Define sample data for a second series, to demonstrate multiple series in the legend
            var sampleDataSeries2 = new List<ChartDataPoint>
    {
        new ChartDataPoint { X = 1, Y = 7, Size = 10, Label = "F", ToolTip = "Test 6", Color = Color.Cyan },
        new ChartDataPoint { X = 2, Y = 17, Size = 12, Label = "G", ToolTip = "Test 7", Color = Color.Magenta },
        new ChartDataPoint { X = 3, Y = 10, Size = 22, Label = "H", ToolTip = "Test 8", Color = Color.Yellow },
        new ChartDataPoint { X = 4, Y = 14, Size = 20, Label = "I", ToolTip = "Test 9", Color = Color.Brown },
        new ChartDataPoint { X = 5, Y = 25, Size = 16, Label = "J", ToolTip = "Test 10", Color = Color.Gray }
    };

            // Clear any existing data
            DataSeries.Clear();
            ChartDataSeries chartDataPoints = new ChartDataSeries();
            chartDataPoints.AddRange(sampleDataSeries1);
            chartDataPoints.ChartType = ChartType.Line;
            chartDataPoints.ShowLine = true;
            chartDataPoints.ShowPoint = true;
            chartDataPoints.ShowArea = true;
            DataSeries.Add(chartDataPoints);
            ChartDataSeries chartDataPoints2 = new ChartDataSeries();
            chartDataPoints2.AddRange(sampleDataSeries2);
            chartDataPoints2.ChartType = ChartType.Line;
            chartDataPoints2.ShowLine = true;
            chartDataPoints2.ShowPoint = true;
            chartDataPoints2.ShowArea = true;
            DataSeries.Add(chartDataPoints2);
           
            // Optionally, assign legend labels for each series
            //   LegendLabels = new List<string> { "Series 1", "Series 2" };
        }
        // Mouse events for zooming and panning
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            float zoomFactor = 0.1f;
            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;

            if (e.Delta > 0) // Zoom in
            {
                ViewportXMin += xRange * zoomFactor;
                ViewportXMax -= xRange * zoomFactor;
                ViewportYMin += yRange * zoomFactor;
                ViewportYMax -= yRange * zoomFactor;
            }
            else if (e.Delta < 0) // Zoom out
            {
                ViewportXMin -= xRange * zoomFactor;
                ViewportXMax += xRange * zoomFactor;
                ViewportYMin -= yRange * zoomFactor;
                ViewportYMax += yRange * zoomFactor;
            }

            EnforceViewportLimits();
            Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePosition = e.Location;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                float xRange = ViewportXMax - ViewportXMin;
                float yRange = ViewportYMax - ViewportYMin;

                float xOffset = (lastMousePosition.X - e.X) * xRange / Width;
                float yOffset = (e.Y - lastMousePosition.Y) * yRange / Height;

                ViewportXMin += xOffset;
                ViewportXMax += xOffset;
                ViewportYMin += yOffset;
                ViewportYMax += yOffset;

                EnforceViewportLimits();
                lastMousePosition = e.Location;
                Invalidate();
            }
            // Convert mouse position to chart coordinates
            Point mousePosition = e.Location;

            // Check if we're hovering over any data point
            ChartDataPoint hoveredDataPoint = GetHoveredDataPoint(mousePosition);

            if (hoveredDataPoint != null && hoveredDataPoint != lastHoveredDataPoint)
            {
                // Show tooltip for the hovered data point
                dataPointToolTip.Show(hoveredDataPoint.ToolTip, this, e.Location.X + 15, e.Location.Y + 15);
                lastHoveredDataPoint = hoveredDataPoint;
            }
            else if (hoveredDataPoint == null)
            {
                // Hide tooltip if not hovering over any data point
                dataPointToolTip.Hide(this);
                lastHoveredDataPoint = null;
            }
        }
        private ChartDataPoint GetHoveredDataPoint(Point mousePosition)
        {
            // Loop through all data series to find if any point is near the mouse position
            foreach (var series in DataSeries)
            {
                foreach (var dataPoint in series)
                {
                    var pointScreenPosition = GetDataPointScreenPosition(dataPoint);
                    float size = dataPoint.Size;

                    // Check if the mouse is within the data point's bounds
                    RectangleF dataPointBounds = new RectangleF(
                        pointScreenPosition.X - size / 2,
                        pointScreenPosition.Y - size / 2,
                        size, size);

                    if (dataPointBounds.Contains(mousePosition))
                    {
                        return dataPoint;
                    }
                }
            }
            return null;
        }
        private PointF GetDataPointScreenPosition(ChartDataPoint dataPoint)
        {
            float? x = dataPoint.GetXAsFloat();
            float? y = dataPoint.GetYAsFloat();

            if (x.HasValue && y.HasValue)
            {
                // Transform data point coordinates to screen coordinates within the chart area
                float xPos = ChartDrawingRect.Left + (x.Value - ViewportXMin) / (ViewportXMax - ViewportXMin) * ChartDrawingRect.Width;
                float yPos = ChartDrawingRect.Bottom - (y.Value - ViewportYMin) / (ViewportYMax - ViewportYMin) * ChartDrawingRect.Height;

                return new PointF(xPos, yPos);
            }

            return PointF.Empty;
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void EnforceViewportLimits()
        {
            float minRange = 1f;
            if ((ViewportXMax - ViewportXMin) < minRange)
            {
                ViewportXMin = Math.Max(0, ViewportXMin);
                ViewportXMax = ViewportXMin + minRange;
            }

            if ((ViewportYMax - ViewportYMin) < minRange)
            {
                ViewportYMin = Math.Max(0, ViewportYMin);
                ViewportYMax = ViewportYMin + minRange;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            UpdateChartDrawingRect();

            if (CustomDraw) DrawDataSeries(e.Graphics, ChartDrawingRect);
            else
            {
                DrawAxisTitles(e.Graphics);
                DrawAxes(e.Graphics);
                DrawAxisLabels(e.Graphics);
             
                if(ChartType != ChartType.Pie)
                    DrawDataSeries(e.Graphics, ChartDrawingRect);
                else
                    DrawPieDataSeries(e.Graphics, ChartDrawingRect);
            }
          
            
            if (ShowLegend && ChartType!=ChartType.Pie)
            {
                DrawLegend(e.Graphics);
            }
           
        }

        protected void UpdateChartDrawingRect()
        {
            int leftPadding = ChartPadding.Left;
            int rightPadding = ChartPadding.Right;
            int topPadding = ChartPadding.Top;
            int bottomPadding = ChartPadding.Bottom;

            // Adjust padding for each axis title if it is shown
            using (Font titleFont = new Font("Arial", 10, FontStyle.Bold))
            {
                if (ShowLeftAxisTitle && !string.IsNullOrEmpty(LeftAxisTitle))
                {
                    SizeF titleSize = TextRenderer.MeasureText(LeftAxisTitle, titleFont);
                    leftPadding += (int)titleSize.Height + 5; // Add padding for left title
                }

                if (ShowRightAxisTitle && !string.IsNullOrEmpty(RightAxisTitle))
                {
                    SizeF titleSize = TextRenderer.MeasureText(RightAxisTitle, titleFont);
                    rightPadding += (int)titleSize.Height + 5; // Add padding for right title
                }

                if (ShowBottomAxisTitle && !string.IsNullOrEmpty(BottomAxisTitle))
                {
                    SizeF titleSize = TextRenderer.MeasureText(BottomAxisTitle, titleFont);
                    bottomPadding += (int)titleSize.Height + 5; // Add padding for bottom title
                }

                if (ShowTopAxisTitle && !string.IsNullOrEmpty(TopAxisTitle))
                {
                    SizeF titleSize = TextRenderer.MeasureText(TopAxisTitle, titleFont);
                    topPadding += (int)titleSize.Height + 5; // Add padding for top title
                }
            }
            // Adjust padding for legend based on position
            if (ShowLegend)
            {
                switch (LegendPosition)
                {
                    case ChartLegendPosition.Right:
                        rightPadding += legendPadding;
                        break;
                    case ChartLegendPosition.Left:
                        leftPadding += legendPadding;
                        break;
                    case ChartLegendPosition.Top:
                        topPadding += legendPadding;
                        break;
                    case ChartLegendPosition.Bottom:
                        bottomPadding += legendPadding;
                        break;
                }
            }
            // Calculate ChartDrawingRect with adjusted padding
            ChartDrawingRect = new Rectangle(
                DrawingRect.Left + leftPadding,
                DrawingRect.Top + topPadding,
                DrawingRect.Width - leftPadding - rightPadding,
                DrawingRect.Height - topPadding - bottomPadding
            );
        }


        protected void DrawAxisTitles(Graphics g)
        {
            using Font titleFont = new Font("Arial", 10, FontStyle.Bold);
            using Brush titleBrush = new SolidBrush(ForeColor);

            // Draw Left Axis Title (Vertical, aligned to left of the control)
            if (!string.IsNullOrEmpty(LeftAxisTitle) && ShowLeftAxisTitle)
            {
                SizeF textSize = g.MeasureString(LeftAxisTitle, titleFont);
                g.TranslateTransform(Padding.Left / 2, (Height - textSize.Width) / 2); // Position title at the center-left of the control border
                g.RotateTransform(-90); // Rotate text for vertical alignment
                g.DrawString(LeftAxisTitle, titleFont, titleBrush, 0, 0);
                g.ResetTransform();
            }

            // Draw Right Axis Title (Vertical, aligned to right of the control)
            if (!string.IsNullOrEmpty(RightAxisTitle) && ShowRightAxisTitle && ShowRightAxis)
            {
                SizeF textSize = g.MeasureString(RightAxisTitle, titleFont);
                g.TranslateTransform(Width - Padding.Right / 2, (Height - textSize.Width) / 2);
                g.RotateTransform(90); // Rotate text for vertical alignment
                g.DrawString(RightAxisTitle, titleFont, titleBrush, 0, 0);
                g.ResetTransform();
            }

            // Draw Bottom Axis Title (Horizontal, aligned to bottom of the control)
            if (!string.IsNullOrEmpty(BottomAxisTitle) && ShowBottomAxisTitle)
            {
                SizeF textSize = g.MeasureString(BottomAxisTitle, titleFont);
                g.DrawString(BottomAxisTitle, titleFont, titleBrush,
                    (Width - textSize.Width) / 2, Height - Padding.Bottom / 2 - textSize.Height);
            }

            // Draw Top Axis Title (Horizontal, aligned to top of the control)
            if (!string.IsNullOrEmpty(TopAxisTitle) && ShowTopAxisTitle && ShowTopAxis)
            {
                SizeF textSize = g.MeasureString(TopAxisTitle, titleFont);
                g.DrawString(TopAxisTitle, titleFont, titleBrush,
                    (Width - textSize.Width) / 2, Padding.Top / 2);
            }
        }

        private void DrawAxes(Graphics g)
        {
            using Pen axisPen = new Pen(Color.Black);
            g.DrawLine(axisPen, ChartDrawingRect.Left, ChartDrawingRect.Top, ChartDrawingRect.Left, ChartDrawingRect.Bottom); // Left axis
            g.DrawLine(axisPen, ChartDrawingRect.Left, ChartDrawingRect.Bottom, ChartDrawingRect.Right, ChartDrawingRect.Bottom); // Bottom axis

            if (ShowTopAxis)
                g.DrawLine(axisPen, ChartDrawingRect.Left, ChartDrawingRect.Top, ChartDrawingRect.Right, ChartDrawingRect.Top); // Top axis

            if (ShowRightAxis)
                g.DrawLine(axisPen, ChartDrawingRect.Right, ChartDrawingRect.Top, ChartDrawingRect.Right, ChartDrawingRect.Bottom); // Right axis
        }

        private void DrawAxisLabels(Graphics g)
        {
            using Font labelFont = new Font("Arial", 8);
            using Brush labelBrush = new SolidBrush(ForeColor);

            DrawAxisLabelsForSide(g, labelFont, labelBrush, LeftAxisType, LeftTitleAlignment, "left");
            if (ShowRightAxis)
                DrawAxisLabelsForSide(g, labelFont, labelBrush, RightAxisType, RightTitleAlignment, "right");
            DrawAxisLabelsForSide(g, labelFont, labelBrush, BottomAxisType, BottomTitleAlignment, "bottom");
            if (ShowTopAxis)
                DrawAxisLabelsForSide(g, labelFont, labelBrush, TopAxisType, TopTitleAlignment, "top");
        }

        private void DrawAxisLabelsForSide(Graphics g, Font font, Brush brush, AxisType axisType, TextAlignment alignment, string axisPosition)
        {
            int labelCount = 10; // Example count, adjust or make configurable
            float labelSpacing;
            Rectangle labelArea = ChartDrawingRect;

            for (int i = 0; i < labelCount; i++)
            {
                string labelText = GetAxisLabelText(i, labelCount, axisType);

                SizeF labelSize = g.MeasureString(labelText, font);
                PointF labelPosition = axisPosition switch
                {
                    "left" => new PointF(
                        labelArea.Left - labelSize.Width - ChartPadding.Left / 2,
                        labelArea.Bottom - i * labelArea.Height / labelCount - labelSize.Height / 2),

                    "right" => new PointF(
                        labelArea.Right + ChartPadding.Right / 2,
                        labelArea.Bottom - i * labelArea.Height / labelCount - labelSize.Height / 2),

                    "bottom" => new PointF(
                        labelArea.Left + i * labelArea.Width / labelCount - labelSize.Width / 2,
                        labelArea.Bottom + ChartPadding.Bottom / 4),

                    "top" => new PointF(
                        labelArea.Left + i * labelArea.Width / labelCount - labelSize.Width / 2,
                        labelArea.Top - ChartPadding.Top / 4 - labelSize.Height),

                    _ => PointF.Empty
                };

                // Translate and rotate based on alignment
                g.TranslateTransform(labelPosition.X, labelPosition.Y);
                float angle = alignment switch
                {
                    TextAlignment.Horizontal => 0,
                    TextAlignment.Vertical => 90,
                    TextAlignment.Diagonal45 => 45,
                    _ => 0
                };
                g.RotateTransform(angle);

                // Draw the label text
                g.DrawString(labelText, font, brush, 0, 0);
                g.ResetTransform();
            }
        }

        private string GetAxisLabelText(int index, int totalLabels, AxisType axisType)
        {
            return axisType switch
            {
                AxisType.Numeric => ((ViewportYMax - ViewportYMin) / totalLabels * index + ViewportYMin).ToString("0.0"),
                AxisType.Text => $"Label {index + 1}", // Placeholder for text labels
                AxisType.Date => DateTime.Now.AddDays(index).ToShortDateString(), // Example: dates increasing by days
                _ => string.Empty
            };
        }


       
        #region Draw Legends
        private void DrawLegend(Graphics g)
        {
            using Font legendFont = new Font("Arial", 8);
            using Brush legendBrush = new SolidBrush(ForeColor);

            List<string> legendLabels = GetLegendLabels();
            SizeF legendSize = CalculateLegendSize(g, legendFont, legendLabels);

            RectangleF legendRect = GetLegendRectangle(legendSize);
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, BackColor)), legendRect);

            float xOffset = legendRect.Left + 10;
            float yOffset = legendRect.Top + 10;

            foreach (var label in legendLabels)
            {
                DrawLegendItem(g, label, new PointF(xOffset, yOffset), legendFont, legendBrush);

                if (LegendOrientation == ChartLegendOrientation.Horizontal)
                {
                    xOffset += legendSize.Width / legendLabels.Count;
                }
                else
                {
                    yOffset += legendSize.Height / legendLabels.Count;
                }
            }
        }

        private void DrawLegendItem(Graphics g, string label, PointF position, Font font, Brush brush)
        {
            float shapeSize = 10;
            RectangleF shapeRect = new RectangleF(position.X, position.Y, shapeSize, shapeSize);

            // Determine color based on series index
            Color seriesColor = GetSeriesColor(label); // You can implement GetSeriesColor to retrieve the appropriate color.

            using Brush shapeBrush = new SolidBrush(seriesColor);

            switch (LegendStyle)
            {
                case ChartDataPointStyle.Circle:
                    g.FillEllipse(shapeBrush, shapeRect);
                    break;
                case ChartDataPointStyle.Square:
                    g.FillRectangle(shapeBrush, shapeRect);
                    break;
                case ChartDataPointStyle.Diamond:
                    using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        path.AddPolygon(new[]
                        {
                    new PointF(position.X + shapeSize / 2, position.Y),
                    new PointF(position.X + shapeSize, position.Y + shapeSize / 2),
                    new PointF(position.X + shapeSize / 2, position.Y + shapeSize),
                    new PointF(position.X, position.Y + shapeSize / 2)
                });
                        g.FillPath(shapeBrush, path);
                    }
                    break;
                case ChartDataPointStyle.Triangle:
                    using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        path.AddPolygon(new[]
                        {
                    new PointF(position.X + shapeSize / 2, position.Y),
                    new PointF(position.X + shapeSize, position.Y + shapeSize),
                    new PointF(position.X, position.Y + shapeSize)
                });
                        g.FillPath(shapeBrush, path);
                    }
                    break;
            }

            g.DrawString(label, font, brush, position.X + shapeSize + 5, position.Y);
        }

        private SizeF CalculateLegendSize(Graphics g, Font font, List<string> legendLabels)
        {
            float width = 0;
            float height = 0;

            foreach (var label in legendLabels)
            {
                SizeF size = g.MeasureString(label, font);
                if (LegendOrientation == ChartLegendOrientation.Horizontal)
                {
                    width += size.Width + 20;
                    height = Math.Max(height, size.Height);
                }
                else
                {
                    height += size.Height + 10;
                    width = Math.Max(width, size.Width);
                }
            }

            return new SizeF(width, height);
        }

        private RectangleF GetLegendRectangle(SizeF legendSize)
        {
            float x = 0, y = 0;

            switch (LegendPosition)
            {
                case ChartLegendPosition.Top:
                    x = (Width - legendSize.Width) / 2;
                    y = ChartPadding.Top / 2;
                    break;
                case ChartLegendPosition.Bottom:
                    x = (Width - legendSize.Width) / 2;
                    y = Height - legendSize.Height - ChartPadding.Bottom / 2;
                    break;
                case ChartLegendPosition.Left:
                    x = ChartPadding.Left / 2;
                    y = (Height - legendSize.Height) / 2;
                    break;
                case ChartLegendPosition.Right:
                    x = Width - legendSize.Width - ChartPadding.Right / 2;
                    y = (Height - legendSize.Height) / 2;
                    break;
            }

            return new RectangleF(x, y, legendSize.Width, legendSize.Height);
        }

        private List<string> GetLegendLabels()
        {
            List<string> labels = new List<string>();
            for (int i = 0; i < DataSeries.Count; i++)
            {
                labels.Add($"Series {i + 1}");
            }
            return labels;
        }
        #region Draw Legends
        private void DrawLegendForSeries(Graphics g, List<ChartDataPoint> series, int seriesIndex)
        {
            using Font legendFont = new Font("Arial", 8);
            using Brush legendBrush = new SolidBrush(ForeColor);

            // Generate labels for each point in the series
            List<string> legendLabels = series.Select(point => point.Label).ToList();
            SizeF legendSize = CalculateLegendSize(g, legendFont, legendLabels);

            // Adjust legend position based on the LegendPosition and series index (for stacking)
            RectangleF legendRect = GetLegendRectangleForSeries(legendSize, seriesIndex);
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, BackColor)), legendRect);

            float xOffset = legendRect.Left + 10;
            float yOffset = legendRect.Top + 10;

            // Draw each label in the series legend, now using your `DrawLegendItem` format
            foreach (var point in series)
            {
                DrawLegendItem(g, point.Label, new PointF(xOffset, yOffset), legendFont, legendBrush); // Use the color from `GetSeriesColor` as per your original setup

                if (LegendOrientation == ChartLegendOrientation.Horizontal)
                {
                    xOffset += legendSize.Width / legendLabels.Count;
                }
                else
                {
                    yOffset += legendSize.Height / legendLabels.Count;
                }
            }
        }

        private RectangleF GetLegendRectangleForSeries(SizeF legendSize, int seriesIndex)
        {
            float x = 0, y = 0;

            // Offset each series legend based on its index to avoid overlap
            float seriesOffset = 10 * seriesIndex;

            switch (LegendPosition)
            {
                case ChartLegendPosition.Top:
                    x = (Width - legendSize.Width) / 2;
                    y = ChartPadding.Top / 2 + seriesOffset;
                    break;
                case ChartLegendPosition.Bottom:
                    x = (Width - legendSize.Width) / 2;
                    y = Height - legendSize.Height - ChartPadding.Bottom / 2 - seriesOffset;
                    break;
                case ChartLegendPosition.Left:
                    x = ChartPadding.Left / 2;
                    y = (Height - legendSize.Height) / 2 + seriesOffset;
                    break;
                case ChartLegendPosition.Right:
                    x = Width - legendSize.Width - ChartPadding.Right / 2;
                    y = (Height - legendSize.Height) / 2 + seriesOffset;
                    break;
            }

            return new RectangleF(x, y, legendSize.Width, legendSize.Height);
        }
        #endregion

        // Optional: Get series color by label
        private Color GetSeriesColor(string label)
        {
            // You can retrieve the color of the series based on the label or index.
            // For example, return ChartDataSeries[i][0].Color based on the series index.
            return Color.Blue; // Default color or implement logic based on label
        }


        #endregion Draw Legends
        #region Data Type Handling
        protected Dictionary<string, int> xAxisCategories = new Dictionary<string, int>();
        protected Dictionary<string, int> yAxisCategories = new Dictionary<string, int>();
        protected DateTime xAxisDateMin = DateTime.MinValue;
        protected DateTime yAxisDateMin = DateTime.MinValue;
        public object ConvertXValue(ChartDataPoint point)
        {
            return ConvertValue(point.X, BottomAxisType, xAxisCategories, ref xAxisDateMin);
        }

        public object ConvertYValue(ChartDataPoint point)
        {
            return ConvertValue(point.Y, LeftAxisType, yAxisCategories, ref yAxisDateMin);
        }

        private float ConvertValue(object value, AxisType axisType, Dictionary<string, int> categoryMap, ref DateTime dateMin)
        {
            switch (axisType)
            {
                case AxisType.Numeric:
                    return Convert.ToSingle(value);

                case AxisType.Date:
                    var dateValue = Convert.ToDateTime(value);
                    if (dateMin == DateTime.MinValue) dateMin = dateValue;
                    return (float)(dateValue - dateMin).TotalDays;

                case AxisType.Text:
                    string textValue = value.ToString();
                    if (!categoryMap.ContainsKey(textValue))
                        categoryMap[textValue] = categoryMap.Count;
                    return categoryMap[textValue];
            }
            return 0;
        }
        #endregion Data Type Handling
        #region Chart Types Drawing
        protected virtual void DrawDataSeries(Graphics g, Rectangle chartArea)
        {
            foreach (var series in DataSeries)
            {
                if (!series.Visible) continue;

                switch (series.ChartType)
                {
                    case ChartType.Line:
                        if (series.ShowLine)
                            DrawLineSeries(g, series, chartArea);
                       
                        break;

                    case ChartType.Bubble:
                        DrawBubbleSeries(g, series, chartArea);
                        break;
                    case ChartType.Bar:
                        DrawBarSeries(g, series, chartArea);
                        break;
                }
                if (series.ShowArea)
                    DrawAreaUnderLine(g, series, chartArea);
                
            }
        }

        protected void DrawBubbleSeries(Graphics g,ChartDataSeries series, Rectangle chartArea)
        {
          
                foreach (var point in series)
                {
                    // Convert X and Y values based on axis types
                    float x = Convert.ToSingle(ConvertXValue(point));
                    float y = Convert.ToSingle(ConvertYValue(point));
                    float size = point.Size;

                    // Calculate position in chart area, accounting for viewport scaling
                    float scaledX = chartArea.Left + (x - ViewportXMin) * chartArea.Width / (ViewportXMax - ViewportXMin);
                    float scaledY = chartArea.Bottom - (y - ViewportYMin) * chartArea.Height / (ViewportYMax - ViewportYMin);

                    // Adjust bubble size for scaling, keeping it within the chart's visual range
                    float bubbleSize = Math.Min(size, 20); // For example, max size can be limited to 20

                    // Draw the bubble
                    using (Brush brush = new SolidBrush(point.Color))
                    {
                        g.FillEllipse(brush, scaledX - bubbleSize / 2, scaledY - bubbleSize / 2, bubbleSize, bubbleSize);
                    }
                    if(series.ShowPoint) DrawPoint(g, point, series, chartArea);
                 // Draw optional labels, if specified
                    if (!string.IsNullOrEmpty(point.Label))
                    {
                        using (Font labelFont = new Font("Arial", 8))
                        using (Brush textBrush = new SolidBrush(ForeColor))
                        {
                            g.DrawString(point.Label, labelFont, textBrush, scaledX, scaledY);
                        }
                    }
                }
           
        }
        private void DrawPoint(Graphics g,ChartDataPoint point, ChartDataSeries series, Rectangle chartArea)
        {
             // Convert X and Y values based on axis types
                float x = Convert.ToSingle(ConvertXValue(point));
                float y = Convert.ToSingle(ConvertYValue(point));

                // Calculate position in chart area, accounting for viewport scaling
                float scaledX = chartArea.Left + (x - ViewportXMin) * chartArea.Width / (ViewportXMax - ViewportXMin);
                float scaledY = chartArea.Bottom - (y - ViewportYMin) * chartArea.Height / (ViewportYMax - ViewportYMin);
                switch (series.DataPointStyle)
                {
                case ChartDataPointStyle.Circle:
                    DrawCircle(g, point, scaledX, scaledY);
                    break;
                case ChartDataPointStyle.Square:
                    DrawSquare(g, point, scaledX, scaledY);
                    break;
                case ChartDataPointStyle.Diamond:
                    DrawDiamond(g, point, scaledX, scaledY);
                    break;
                case ChartDataPointStyle.Triangle:
                    DrawTriangle(g, point, scaledX, scaledY);
                    break;
                }
         
        }
        private void DrawCircle(Graphics g, ChartDataPoint point, float x, float y)
        {
            float size = point.Size;
            using (Brush brush = new SolidBrush(point.Color))
            {
                g.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
            }
        }
        private void DrawSquare(Graphics g, ChartDataPoint point, float x, float y)
        {
            float size = point.Size;
            using (Brush brush = new SolidBrush(point.Color))
            {
                g.FillRectangle(brush, x - size / 2, y - size / 2, size, size);
            }
        }
        private void DrawDiamond(Graphics g, ChartDataPoint point, float x, float y)
        {
            float size = point.Size;
            using (Brush brush = new SolidBrush(point.Color))
            {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddPolygon(new[]
                    {
                        new PointF(x, y - size / 2),
                        new PointF(x + size / 2, y),
                        new PointF(x, y + size / 2),
                        new PointF(x - size / 2, y)
                    });
                    g.FillPath(brush, path);
                }
            }
        }
        private void DrawTriangle(Graphics g, ChartDataPoint point, float x, float y)
        {
            float size = point.Size;
            using (Brush brush = new SolidBrush(point.Color))
            {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddPolygon(new[]
                    {
                        new PointF(x, y - size / 2),
                        new PointF(x + size / 2, y + size / 2),
                        new PointF(x - size / 2, y + size / 2)
                    });
                    g.FillPath(brush, path);
                }
            }
        }
        private void DrawAreaUnderLine(Graphics g, ChartDataSeries series, Rectangle chartArea)
        {
            if (!series.ShowArea) return;

            var areaPoints = new List<PointF>();
            for (int i = 0; i < series.Count; i++)
            {
                var point = series[i];
                float x = Convert.ToSingle(ConvertXValue(point));
                float y = Convert.ToSingle(ConvertYValue(point));

                float scaledX = chartArea.Left + (x - ViewportXMin) * chartArea.Width / (ViewportXMax - ViewportXMin);
                float scaledY = chartArea.Bottom - (y - ViewportYMin) * chartArea.Height / (ViewportYMax - ViewportYMin);

                areaPoints.Add(new PointF(scaledX, scaledY));
            }

            Color areaColor = Color.FromArgb(50, series.Color); // Adjust transparency for the area
            using Brush areaBrush = new SolidBrush(areaColor);
            g.FillPolygon(areaBrush, areaPoints.ToArray());
        }
        private void DrawLineSeries(Graphics g, ChartDataSeries series, Rectangle chartArea)
        {
            using Pen linePen = new Pen(Color.Black, 2); // Customize color and width per series

            for (int i = 0; i < series.Count - 1; i++)
            {
                var start = series[i];
                var end = series[i + 1];
                var startPoint = TransformDataPoint(start, chartArea);
                var endPoint = TransformDataPoint(end, chartArea);

                g.DrawLine(linePen, startPoint, endPoint);
            }
        }
        // Transforms data points based on viewport and chart area
        private Point TransformDataPoint(ChartDataPoint point, Rectangle chartArea)
        {
            float x = Convert.ToSingle(point.GetXAsFloat() ?? 0);
            float y = Convert.ToSingle(point.GetYAsFloat() ?? 0);

            float scaledX = chartArea.Left + (x - ViewportXMin) * chartArea.Width / (ViewportXMax - ViewportXMin);
            float scaledY = chartArea.Bottom - (y - ViewportYMin) * chartArea.Height / (ViewportYMax - ViewportYMin);

            return new Point((int)scaledX, (int)scaledY);
        }
        protected void DrawBarSeries(Graphics g, ChartDataSeries series, Rectangle chartArea)
        {
              float barWidth = chartArea.Width / series.Count;
                float barPadding = 5; // Padding between bars

                for (int i = 0; i < series.Count; i++)
                {
                    var point = series[i];

                    // Convert X and Y values based on axis types
                    float x = Convert.ToSingle(ConvertXValue(point));
                    float y = Convert.ToSingle(ConvertYValue(point));

                    // Calculate position in chart area, accounting for viewport scaling
                    float scaledX = chartArea.Left + (x - ViewportXMin) * chartArea.Width / (ViewportXMax - ViewportXMin);
                    float scaledY = chartArea.Bottom - (y - ViewportYMin) * chartArea.Height / (ViewportYMax - ViewportYMin);

                    // Calculate bar position and size
                    float barX = scaledX - barWidth / 2 + i * barWidth + barPadding;
                    float barHeight = chartArea.Bottom - scaledY;

                    // Draw the bar
                    using (Brush brush = new SolidBrush(point.Color))
                    {
                        g.FillRectangle(brush, barX, scaledY, barWidth - barPadding, barHeight);
                    }

                    // Draw optional labels, if specified
                    if (!string.IsNullOrEmpty(point.Label))
                    {
                        using (Font labelFont = new Font("Arial", 8))
                        using (Brush textBrush = new SolidBrush(ForeColor))
                        {
                            g.DrawString(point.Label, labelFont, textBrush, scaledX, scaledY);
                        }
                    }
                }
           
        }

        protected void DrawPieDataSeries(Graphics g,  Rectangle chartArea)
        {
            int maxSeriesCount = 3; // Limit to 3 series for readability in a pie chart
            float radiusStep = chartArea.Width / (2 * maxSeriesCount); // Adjust radius for each inner pie

            // Calculate the total sum of values for each series to determine slice angles
            List<float> seriesTotals = new List<float>();
            foreach (var series in DataSeries)
            {
                
                float seriesTotal = series.Take(6).Sum(point => Convert.ToSingle(ConvertYValue(point))); // Limiting to 6 points per series for readability
                seriesTotals.Add(seriesTotal);
            }

            int seriesIndex = 0;
            float baseRadius = chartArea.Width / 2f; // Starting radius for the outermost pie
            foreach (var series in DataSeries.Take(maxSeriesCount))
            {
                float startAngle = 0;
                float total = seriesTotals[seriesIndex];
                float seriesRadius = baseRadius - seriesIndex * radiusStep; // Adjust radius for each series

                // Define a new rectangle for each inner pie chart
                RectangleF seriesArea = new RectangleF(
                    chartArea.X + (chartArea.Width - seriesRadius * 2) / 2,
                    chartArea.Y + (chartArea.Height - seriesRadius * 2) / 2,
                    seriesRadius * 2,
                    seriesRadius * 2
                );

                int dataPointIndex = 0;
                foreach (var point in series.Take(6))
                {
                    float value = Convert.ToSingle(ConvertYValue(point));
                    float sweepAngle = value / total * 360;

                    // Draw the pie slice for this data point
                    Color seriesColor = GetSeriesColor($"Series {seriesIndex + 1}");
                    using (Brush brush = new SolidBrush(seriesColor))
                    {
                        g.FillPie(brush, seriesArea, startAngle, sweepAngle);
                    }

                    startAngle += sweepAngle;
                    dataPointIndex++;
                }

                // Draw the legend for this specific series
                DrawLegendForSeries(g, series, seriesIndex);

                seriesIndex++;
            }
        }


        #endregion Chart Types Drawing
    }
}
