using System.ComponentModel;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
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
         
            if (UseThemeColors && _currentTheme != null)
            {
                _painter?.DrawBackground(g, ctx);
            }
            else
            {
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }
            var plotBounds = ctx.PlotRect != Rectangle.Empty ? ctx.PlotRect : ctx.DrawingRect;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw title section
            var currentY = DrawTitleSection(g, plotBounds);

            // Draw axes and get plot area
            var axisCtx = CreateAxisContext(plotBounds, currentY);
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
                var titleFont = PaintersFactory.GetFont(ChartTitleFont);
                using (Brush titleBrush = PaintersFactory.GetSolidBrush(ChartTitleForeColor))
                {
                    SizeF titleSize = TextUtils.MeasureText(g,ChartTitle, titleFont);
                    g.DrawString(ChartTitle, titleFont, titleBrush, textAreaLeft, currentY);
                    currentY += (int)titleSize.Height + 5;
                }
            }

            if (!string.IsNullOrEmpty(ChartValue))
            {
                var valueFont = PaintersFactory.GetFont(ChartValueFont);
                using (Brush valueBrush = PaintersFactory.GetSolidBrush(ChartTextColor))
                {
                    SizeF valueSize = TextUtils.MeasureText(g,ChartValue, valueFont);
                    g.DrawString(ChartValue, valueFont, valueBrush, textAreaLeft, currentY);
                    currentY += (int)valueSize.Height + 5;
                }
            }

            if (!string.IsNullOrEmpty(ChartSubtitle))
            {
                var subFont = PaintersFactory.GetFont(ChartSubtitleFont);
                using (Brush subBrush = PaintersFactory.GetSolidBrush(ChartLineColor))
                {
                    SizeF subSize = TextUtils.MeasureText(g,ChartSubtitle, subFont);
                    g.DrawString(ChartSubtitle, subFont, subBrush, textAreaLeft, currentY);
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
                TitleFont = PaintersFactory.GetFont(ChartTitleFont),
                LabelFont = PaintersFactory.GetFont(ChartValueFont),
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
                    PaintersFactory.GetFont(ChartValueFont), ChartLegendTextColor, ChartLegendBackColor, 
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