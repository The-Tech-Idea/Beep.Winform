using System.ComponentModel;
using System.Drawing.Drawing2D;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
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
            
            // Use passed-in bounds when DrawingRect is not yet calculated
            var effectiveRect = (DrawingRect.Width > 0 && DrawingRect.Height > 0)
                ? DrawingRect
                : bounds;

            if (effectiveRect.Width <= 0 || effectiveRect.Height <= 0)
                return;

            // Create layout context
            var ctx = new ChartLayout
            {
                DrawingRect = effectiveRect,
                PlotRect = Rectangle.Empty,
                Radius = BorderRadius,
                AccentColor = _accentColor
            };

            // Initialize and adjust layout via chart painter
            var activeTheme = _currentTheme ?? BeepThemesManager.CurrentTheme;
            _chartpainter?.Initialize(this, activeTheme);
            ctx = _chartpainter?.AdjustLayout(effectiveRect, ctx) ?? ctx;

            // When no chart painter is set, define a default PlotRect with basic padding
            if (ctx.PlotRect == Rectangle.Empty || ctx.PlotRect.Width <= 0 || ctx.PlotRect.Height <= 0)
            {
                int pad = 8;
                ctx.PlotRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.DrawingRect.Top + pad,
                    Math.Max(1, ctx.DrawingRect.Width - pad * 2),
                    Math.Max(1, ctx.DrawingRect.Height - pad * 2));
            }

            // Draw background
            if (_chartpainter != null)
            {
                _chartpainter.DrawBackground(g, ctx);
            }
            else
            {
                BeepStyling.PaintStyleBackground(g, effectiveRect, ControlStyle);
            }

            var plotBounds = ctx.PlotRect;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw title section
            var currentY = DrawTitleSection(g, plotBounds);
            ctx.TitleBottom = currentY; // let DrawForeground know where title ends

            // Draw axes and get plot area
            if (_axisPainter == null)
                _axisPainter = new CartesianAxisPainter();
            var axisCtx = CreateAxisContext(plotBounds, currentY);
            axisCtx = _axisPainter.AdjustPlotRect(g, axisCtx);
            _axisPainter.DrawAxes(g, axisCtx);
            _axisPainter.DrawTicks(g, axisCtx);

            _chartDrawingRect = axisCtx.PlotRect;

            // Draw series based on chart type
            DrawSeriesByType(g, axisCtx);

            // Draw foreground elements
            DrawHitAreas(axisCtx);
            _chartpainter?.DrawForeground(g, ctx);
        }

        private int DrawTitleSection(Graphics g, Rectangle bounds)
        {
            int textAreaLeft = bounds.Left + 10;
            int currentY = bounds.Top + 10;

            if (!ShowTitle) return currentY;

            if (!string.IsNullOrEmpty(ChartTitle))
            {
                var titleFont = PaintersFactory.GetFont(ChartTitleFont ?? SystemFonts.DefaultFont);
                // Do NOT dispose — PaintersFactory returns cached/shared brushes
                var titleBrush = PaintersFactory.GetSolidBrush(ChartTitleForeColor);
                SizeF titleSize = TextUtils.MeasureText(g, ChartTitle, titleFont);
                g.DrawString(ChartTitle, titleFont, titleBrush, textAreaLeft, currentY);
                currentY += (int)titleSize.Height + 5;
            }

            if (!string.IsNullOrEmpty(ChartValue))
            {
                var valueFont = PaintersFactory.GetFont(ChartValueFont ?? SystemFonts.DefaultFont);
                var valueBrush = PaintersFactory.GetSolidBrush(ChartTextColor);
                SizeF valueSize = TextUtils.MeasureText(g, ChartValue, valueFont);
                g.DrawString(ChartValue, valueFont, valueBrush, textAreaLeft, currentY);
                currentY += (int)valueSize.Height + 5;
            }

            if (!string.IsNullOrEmpty(ChartSubtitle))
            {
                var subFont = PaintersFactory.GetFont(ChartSubtitleFont ?? SystemFonts.DefaultFont);
                var subBrush = PaintersFactory.GetSolidBrush(ChartLineColor);
                SizeF subSize = TextUtils.MeasureText(g, ChartSubtitle, subFont);
                g.DrawString(ChartSubtitle, subFont, subBrush, textAreaLeft, currentY);
                currentY += (int)subSize.Height + 10;
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
                DeriveChartColorsFromTheme();
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
                ChartTitleFont    = BeepThemesManager.ToFont(_currentTheme.ChartTitleFont   ?? _currentTheme.TitleStyle);
                ChartValueFont    = BeepThemesManager.ToFont(_currentTheme.ChartSubTitleFont ?? _currentTheme.GetBlockHeaderFont());
                ChartSubtitleFont = BeepThemesManager.ToFont(_currentTheme.GetBlockTextFont());
                InitializePainter();
                Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"ApplyTheme Error: {ex.Message}");
            }
        }

        // ── Chart color derivation ──────────────────────────────────────────────
        /// <summary>
        /// Derives all Chart* color properties on <see cref="_currentTheme"/> from its
        /// semantic palette (PrimaryColor, AccentColor, SurfaceColor, ForeColor …).
        /// Called at the start of ApplyTheme() so the rest of that method reads
        /// already-computed, theme-consistent values.
        /// </summary>
        private void DeriveChartColorsFromTheme()
        {
            if (_currentTheme == null) return;

            var t = _currentTheme;

            // structural background
            t.ChartBackColor       = t.SurfaceColor;
            t.ChartLegendBackColor = t.SurfaceColor;

            // axis / line: primary when readable on surface, otherwise foreground
            Color axisColor = ChartContrastRatio(t.PrimaryColor, t.SurfaceColor) >= 3.0
                ? t.PrimaryColor
                : t.ForeColor;
            t.ChartAxisColor       = axisColor;
            t.ChartLineColor       = axisColor;

            // fill: semi-transparent tint of primary
            t.ChartFillColor = Color.FromArgb(40, t.PrimaryColor.R, t.PrimaryColor.G, t.PrimaryColor.B);

            // grid: ~12 % foreground blended into surface (subtle)
            t.ChartGridLineColor = ChartBlendColor(t.SurfaceColor, t.ForeColor, 0.12);

            // text / legend labels
            t.ChartTitleColor      = t.ForeColor;
            t.ChartTextColor       = t.ForeColor;
            t.ChartLegendTextColor = t.ForeColor;
            t.ChartLegendShapeColor = t.ForeColor;

            // series palette
            t.ChartDefaultSeriesColors = BuildSeriesColors(t, 8);
        }

        private static List<Color> BuildSeriesColors(IBeepTheme t, int count)
        {
            // start with semantic palette seeds
            var seeds = new[] { t.AccentColor, t.PrimaryColor, t.SecondaryColor,
                                t.SuccessColor, t.ErrorColor, t.WarningColor };

            var result = new List<Color>();

            foreach (var c in seeds)
            {
                if (result.Count >= count) break;
                if (ChartContrastRatio(c, t.SurfaceColor) >= 2.5 && !result.Any(x => ChartColorsClose(x, c)))
                    result.Add(c);
            }

            // extend with hue-shifted variants of AccentColor until count reached
            float[] shifts = { 30f, 60f, 120f, 150f, 210f, 270f, 300f, 330f };
            foreach (float shift in shifts)
            {
                if (result.Count >= count) break;
                var candidate = ChartShiftHue(t.AccentColor, shift);
                if (ChartContrastRatio(candidate, t.SurfaceColor) >= 2.5 && !result.Any(x => ChartColorsClose(x, candidate)))
                    result.Add(candidate);
            }

            // last-resort guarantee: at least one color
            if (result.Count == 0)
                result.Add(t.ForeColor);

            return result;
        }

        private static bool ChartColorsClose(Color a, Color b)
        {
            int dr = a.R - b.R, dg = a.G - b.G, db = a.B - b.B;
            return dr * dr + dg * dg + db * db < 900; // ~30 per channel
        }

        private static Color ChartBlendColor(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0, 1);
            byte Lerp(byte x, byte y) => (byte)Math.Round(x + (y - x) * t);
            return Color.FromArgb(255, Lerp(a.R, b.R), Lerp(a.G, b.G), Lerp(a.B, b.B));
        }

        private static double ChartContrastRatio(Color fg, Color bg)
        {
            static double SRgb(double v) => v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
            static double Lum(Color c) =>
                0.2126 * SRgb(c.R / 255.0) +
                0.7152 * SRgb(c.G / 255.0) +
                0.0722 * SRgb(c.B / 255.0);
            double l1 = Lum(fg), l2 = Lum(bg);
            return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05);
        }

        private static Color ChartShiftHue(Color c, float degrees)
        {
            ChartColorToHsl(c, out float h, out float s, out float l);
            h = ((h + degrees / 360f) % 1f + 1f) % 1f;
            return ChartHslToColor(h, s, l);
        }

        private static void ChartColorToHsl(Color c, out float h, out float s, out float l)
        {
            float r = c.R / 255f, g = c.G / 255f, b = c.B / 255f;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            l = (max + min) / 2f;
            float d = max - min;
            if (d < 0.0001f) { h = s = 0f; return; }
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);
            if (max == r)      h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g) h = (b - r) / d + 2f;
            else               h = (r - g) / d + 4f;
            h /= 6f;
        }

        private static Color ChartHslToColor(float h, float s, float l)
        {
            if (s < 0.0001f) { byte v = (byte)(l * 255); return Color.FromArgb(v, v, v); }
            float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
            float p = 2f * l - q;
            static float Hue(float p, float q, float t)
            {
                t = ((t % 1f) + 1f) % 1f;
                if (t < 1 / 6f) return p + (q - p) * 6f * t;
                if (t < 1 / 2f) return q;
                if (t < 2 / 3f) return p + (q - p) * (2 / 3f - t) * 6f;
                return p;
            }
            return Color.FromArgb(
                (byte)(Hue(p, q, h + 1 / 3f) * 255),
                (byte)(Hue(p, q, h)          * 255),
                (byte)(Hue(p, q, h - 1 / 3f) * 255));
        }
    }
}