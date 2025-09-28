using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            
            if (DrawingRect.Width <= 0 || DrawingRect.Height <= 0)
                return;

            // Create layout context
            var ctx = new ChartLayout
            {
                DrawingRect = DrawingRect,
                PlotRect = Rectangle.Empty,
                Radius = BorderRadius,
                AccentColor = _accentColor
            };

            // Initialize and adjust layout
            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;
            _painter?.DrawBackground(g, ctx);

            var bounds = ctx.PlotRect != Rectangle.Empty ? ctx.PlotRect : ctx.DrawingRect;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw title section
            var currentY = DrawTitleSection(g, bounds);

            // Draw axes and get plot area
            var axisCtx = CreateAxisContext(bounds, currentY);
            axisCtx = _axisPainter.AdjustPlotRect(g, axisCtx);
            _axisPainter.DrawAxes(g, axisCtx);
            _axisPainter.DrawTicks(g, axisCtx);

            _chartDrawingRect = axisCtx.PlotRect;

            // Draw series based on chart type
            DrawSeriesByType(g, axisCtx);

            // Draw foreground elements
            DrawHitAreas(axisCtx);
            _painter?.DrawForeground(g, ctx);
        }

        private int DrawTitleSection(Graphics g, Rectangle bounds)
        {
            int textAreaLeft = bounds.Left + 10;
            int currentY = bounds.Top + 10;

            if (!ShowTitle) return currentY;

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

            return currentY;
        }

        private AxisLayout CreateAxisContext(Rectangle bounds, int currentY)
        {
            return new AxisLayout
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
                XCategories = _xAxisCategories,
                YCategories = _yAxisCategories,
                XDateMin = _xAxisDateMin,
                YDateMin = _yAxisDateMin,
                ShowLegend = ShowLegend,
                XLabelAngle = XLabelAngle,
                YLabelAngle = YLabelAngle,
                XTimeGranularity = XTimeGranularity,
                YTimeGranularity = YTimeGranularity
            };
        }

        private void DrawSeriesByType(Graphics g, AxisLayout axisCtx)
        {
            // Get or create appropriate series painter
            _seriesPainter = SeriesPainterFactory.GetPainter(ChartType);
            _seriesPainter.Initialize(this);

            // Draw the series
            _seriesPainter.DrawSeries(g, axisCtx.PlotRect, _dataSeries, 
                p => BeepChartDataHelper.ConvertXValue(this, p), 
                p => BeepChartDataHelper.ConvertYValue(this, p), 
                ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax, 
                ChartDefaultSeriesColors, ChartAxisColor, ChartTextColor, _seriesOptions);

            // Draw legend if enabled
            if (ShowLegend)
            {
                _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, ChartDefaultSeriesColors, 
                    new Font("Arial", 8), ChartLegendTextColor, ChartLegendBackColor, 
                    ChartLegendShapeColor, this, ToggleSeriesByIndex, LegendPlacement);
            }
        }

        private void DrawHitAreas(AxisLayout axisCtx)
        {
            // Add legend hit area
            var legendArea = new Rectangle(ChartDrawingRect.Right + 10, ChartDrawingRect.Top, 120, 
                Math.Max(100, ChartDrawingRect.Height / 3));
            AddHitArea("Legend", legendArea, null, () => { /* future: raise click */ });
        }

        public override void ApplyTheme()
        {
            try
            {
                BackColor = _currentTheme.ChartBackColor;
                ForeColor = _currentTheme.ChartTitleColor;
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
    }
}