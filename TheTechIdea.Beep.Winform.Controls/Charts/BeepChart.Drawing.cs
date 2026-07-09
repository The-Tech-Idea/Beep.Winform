using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using ControlsHelpers = TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        // Standard 8-colour palette used as a fallback when
        // ChartDefaultSeriesColors is null or empty — prevents
        // DivideByZeroException in series painters that do
        // palette[i % palette.Count].
        internal static readonly List<Color> s_defaultSeriesPalette = new()
        {
            Color.FromArgb(0, 120, 215),  // Blue
            Color.FromArgb(220, 80, 80),  // Red
            Color.FromArgb(80, 180, 80),  // Green
            Color.FromArgb(255, 185, 0),  // Amber
            Color.FromArgb(140, 80, 200), // Purple
            Color.FromArgb(0, 180, 200),  // Cyan
            Color.FromArgb(240, 140, 60), // Orange
            Color.FromArgb(100, 120, 160),// Slate
        };

        // Per-paint cache for string→float conversion steps.
        // Avoids parsing the same X/Y strings every time a point
        // is visited (labels + position = 2 parses per point minimum).
        // Cleared at the start of each Paint() call.
        private readonly Dictionary<ChartDataPoint, (float x, float y)> _convertCache = new();

        private float ConvertXCached(ChartDataPoint p)
        {
            if (_convertCache.TryGetValue(p, out var v)) return v.x;
            float x = BeepChartDataHelper.ConvertXValue(this, p) is float xf ? xf : 0f;
            _convertCache[p] = (x, 0);
            return x;
        }

        private float ConvertYCached(ChartDataPoint p)
        {
            if (_convertCache.TryGetValue(p, out var v)) { _convertCache[p] = (v.x, v.y); return v.y; }
            float y = BeepChartDataHelper.ConvertYValue(this, p) is float yf ? yf : 0f;
            float x = BeepChartDataHelper.ConvertXValue(this, p) is float xf ? xf : 0f;
            _convertCache[p] = (x, y);
            return y;
        }

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
            // Clear the per-paint conversion cache so stale parses
            // from a previous paint cycle don't survive.  Each
            // point is parsed at most once per paint even when
            // visited by multiple draw passes (series + labels).
            _convertCache.Clear();
            
            // Use passed-in bounds when DrawingRect is not yet calculated
            var effectiveRect = (DrawingRect.Width > 0 && DrawingRect.Height > 0)
                ? DrawingRect
                : bounds;

            if (effectiveRect.Width <= 0 || effectiveRect.Height <= 0)
                return;

            // Build chart-owned hit areas later in RegisterInteractiveAreas.
            // Do NOT call ClearHitList() here — the hit-area skip optimisation
            // (RegisterInteractiveAreas) may decide to keep the previous
            // frame's hit areas.  Clearing now would wipe them before the
            // skip check runs, breaking tooltips/clicks on static charts.

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

            // Enforce a minimum size derived from the actual margins
            // so the legend, axis labels, and plot never clip.
            float dpi = DeviceDpi / 96f;
            int minW = (axisCtx.PlotRect.Left - axisCtx.Bounds.Left)
                     + (axisCtx.Bounds.Right - axisCtx.PlotRect.Right)
                     + ControlsHelpers.DpiScalingHelper.ScaleValue(80, dpi);
            int minH = (axisCtx.PlotRect.Top - axisCtx.Bounds.Top)
                     + (axisCtx.Bounds.Bottom - axisCtx.PlotRect.Bottom)
                     + ControlsHelpers.DpiScalingHelper.ScaleValue(40, dpi);
            if (MinimumSize.Width < minW || MinimumSize.Height < minH)
                MinimumSize = new System.Drawing.Size(minW, minH);

            // Draw series based on chart type
            DrawSeriesByType(g, axisCtx);

            // Draw foreground elements
            RegisterInteractiveAreas(axisCtx);

            // Draw legend AFTER the hit-list clear so legend hit
            // areas survive.  Previously the legend was drawn inside
            // DrawSeriesByType (before ClearHitList), and every
            // paint wiped its hit rects.
            if (ShowLegend)
            {
                var legendPalette = ChartDefaultSeriesColors;
                if (legendPalette == null || legendPalette.Count == 0)
                    legendPalette = s_defaultSeriesPalette;
                _legendPainter.DrawLegend(g, axisCtx.PlotRect, _dataSeries, legendPalette,
                    PaintersFactory.GetFont(ChartValueFont), ChartLegendTextColor, ChartLegendBackColor,
                    ChartLegendShapeColor, this, ToggleSeriesByIndex, OnInteractiveAreaHit, LegendPlacement);
            }

            _chartpainter?.DrawForeground(g, ctx);

            // Draw trackball crosshair and tooltip if enabled
            if (EnableTrackball && _trackballCrosshairX >= 0)
            {
                DrawTrackballCrosshair(g, axisCtx.PlotRect);
                if (TrackballShowMultiSeriesValues && _trackballDataPoints.Count > 0)
                {
                    DrawTrackballTooltip(g, axisCtx.PlotRect);
                }
            }
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
                TextRenderer.DrawText(g, ChartTitle, titleFont, new Point(textAreaLeft, currentY), titleBrush.Color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                currentY += (int)titleSize.Height + 5;
            }

            if (!string.IsNullOrEmpty(ChartValue))
            {
                var valueFont = PaintersFactory.GetFont(ChartValueFont ?? SystemFonts.DefaultFont);
                var valueBrush = PaintersFactory.GetSolidBrush(ChartTextColor);
                SizeF valueSize = TextUtils.MeasureText(g, ChartValue, valueFont);
                TextRenderer.DrawText(g, ChartValue, valueFont, new Point(textAreaLeft, currentY), valueBrush.Color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                currentY += (int)valueSize.Height + 5;
            }

            if (!string.IsNullOrEmpty(ChartSubtitle))
            {
                var subFont = PaintersFactory.GetFont(ChartSubtitleFont ?? SystemFonts.DefaultFont);
                var subBrush = PaintersFactory.GetSolidBrush(ChartLineColor);
                SizeF subSize = TextUtils.MeasureText(g, ChartSubtitle, subFont);
                TextRenderer.DrawText(g, ChartSubtitle, subFont, new Point(textAreaLeft, currentY), subBrush.Color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                currentY += (int)subSize.Height + 10;
            }

            return currentY;
        }

        private AxisLayout CreateAxisContext(Rectangle bounds, int currentY)
        {
            var labelIntervals = GetRecommendedLabelIntervals();

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
                YTimeGranularity = YTimeGranularity,
                XLabelInterval = labelIntervals.XLabelInterval,
                YLabelInterval = labelIntervals.YLabelInterval,
                LegendPlacement = LegendPlacement,
                LegendItemCount = _dataSeries?.Count ?? 0,
                DpiScale = DeviceDpi / 96f,
            };
        }

        private void DrawSeriesByType(Graphics g, AxisLayout axisCtx)
        {
            // Get or create appropriate series painter
            _seriesPainter = SeriesPainterFactory.GetPainter(ChartType);
            _seriesPainter.Initialize(this);

            // Guard against empty palette — each painter does
            // palette[i % palette.Count] which throws
            // DivideByZeroException when Count == 0.
            // Fall back to a standard 8-colour palette.
            var palette = ChartDefaultSeriesColors;
            if (palette == null || palette.Count == 0)
                palette = s_defaultSeriesPalette;

            // Performance: when vertex simplification is active and the
            // dataset is large, swap in the RDP-simplified points so the
            // painter draws ~50 points instead of 100k.  This single
            // replacement eliminates the per-paint string-parse loop
            // for 99% of the invisible points.
            List<ChartDataSeries> renderData = _dataSeries;
            if (EnableVertexSimplification && IsLargeDataset())
            {
                renderData = new List<ChartDataSeries>(_dataSeries.Count);
                foreach (var s in _dataSeries)
                {
                    var copy = new ChartDataSeries
                    {
                        Name = s.Name, Color = s.Color, Visible = s.Visible,
                        ShowLabel = s.ShowLabel, ShowLine = s.ShowLine,
                        ShowPoint = s.ShowPoint,
                        Points = GetOptimizedPointsForSeries(_dataSeries.IndexOf(s))
                    };
                    renderData.Add(copy);
                }
            }

            // Draw the series
            // Clip to the plot rect so series lines/bars cannot bleed
            // into the axis labels or title area when zoomed/panned.
            var oldClip = g.Clip;
            g.SetClip(axisCtx.PlotRect, CombineMode.Intersect);
            try
            {
                _seriesPainter.DrawSeries(g, axisCtx.PlotRect, renderData, 
                    p => ConvertXCached(p), 
                    p => ConvertYCached(p), 
                    ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax, 
                    palette, ChartAxisColor, ChartTextColor, _seriesOptions);
            }
            finally
            {
                g.Clip = oldClip;
            }

            // Draw selection highlights
            if (EnablePointSelection && HasSelection)
            {
                DrawSelectionHighlights(g, axisCtx.PlotRect);
            }

            // Draw keyboard focus indicator
            if (EnableKeyboardNavigation && _keyboardFocusedSeriesIndex >= 0 && _keyboardFocusedSeriesIndex < _dataSeries.Count)
            {
                DrawKeyboardFocusIndicator(g, axisCtx.PlotRect);
            }

            // Draw fill patterns for accessibility/non-color state communication
            if (EnableFillPatterns)
            {
                DrawSeriesFillPatterns(g, axisCtx.PlotRect);
            }

            // Draw trend line (linear regression) for Line/Area/Scatter charts
            if (_seriesOptions.ShowTrendLine && _dataSeries.Count > 0)
            {
                DrawTrendLine(g, axisCtx.PlotRect);
            }

            if (CustomDraw)
            {
                CustomDrawSeries?.Invoke(this, new CustomDrawSeriesEventArgs(g, axisCtx.PlotRect, _dataSeries));
            }
            // Legend is drawn AFTER DrawSeriesByType returns, in Paint().
        }

        // Hit-area rebuild skip: re-adding thousands of hit rects
        // per paint is O(n) per frame.  When the data count and
        // viewport haven't changed, the hit areas are still correct,
        // so we skip the rebuild.
        private int _lastHitDataCount;
        private (float xMin, float xMax, float yMin, float yMax) _lastHitViewport;
        // Set to true when DataSeries or viewport changes — forces a
        // full hit-area rebuild even when point count and viewport
        // ranges haven't changed (e.g. values changed in-place).
        internal bool _isDataDirty = true;

        private void RegisterInteractiveAreas(AxisLayout axisCtx)
        {
            // Quick skip: if the data size and viewport are unchanged,
            // the hit areas from the previous frame are still valid.
            int totalPoints = 0;
            foreach (var s in _dataSeries)
                totalPoints += s.Points?.Count ?? 0;
            var vp = (ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax);
            if (!_isDataDirty && totalPoints == _lastHitDataCount && vp == _lastHitViewport)
                return;
            _isDataDirty = false;
            _lastHitDataCount = totalPoints;
            _lastHitDataCount = totalPoints;
            _lastHitViewport = vp;
            // Only clear when we're about to rebuild — never before
            // the skip check (the old frame's hit areas must survive
            // when data/viewport haven't changed).
            ClearHitList();
            _axisPainter?.UpdateHitAreas(this, axisCtx, OnInteractiveAreaHit);
            _seriesPainter?.UpdateHitAreas(this, axisCtx.PlotRect, _dataSeries, GetScreenPoint, OnInteractiveAreaHit);
        }

        private void OnInteractiveAreaHit(string areaName, Rectangle bounds)
        {
            var args = BuildInteractiveAreaArgs(areaName, bounds);
            InteractiveAreaHit?.Invoke(this, args);

            if (args.DataPoint != null && args.SeriesIndex.HasValue && args.PointIndex.HasValue)
            {
                PointHit?.Invoke(this, new ChartPointHitEventArgs(
                    args.AreaName,
                    args.AreaType,
                    args.SeriesIndex.Value,
                    args.PointIndex.Value,
                    args.Bounds,
                    args.DataPoint));

                // Handle point selection on click
                if (EnablePointSelection)
                {
                    bool isCtrlHeld = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                    bool isShiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

                    var key = (args.SeriesIndex.Value, args.PointIndex.Value);
                    bool isSelected = _selectedPoints.Contains(key);

                    if (isCtrlHeld || (_selectionMode == ChartSelectionMode.Multiple && isShiftHeld))
                    {
                        // Toggle selection
                        if (isSelected)
                        {
                            DeselectPoint(args.SeriesIndex.Value, args.PointIndex.Value);
                        }
                        else
                        {
                            SelectPoint(args.SeriesIndex.Value, args.PointIndex.Value, addToSelection: true);
                        }
                    }
                    else
                    {
                        // Single select
                        if (!isSelected)
                        {
                            SelectPoint(args.SeriesIndex.Value, args.PointIndex.Value, addToSelection: false);
                        }
                        else if (_selectionMode == ChartSelectionMode.Single)
                        {
                            DeselectPoint(args.SeriesIndex.Value, args.PointIndex.Value);
                        }
                    }
                }
            }

            if (string.Equals(args.AreaType, "XAxis", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(args.AreaType, "YAxis", StringComparison.OrdinalIgnoreCase))
            {
                AxisHit?.Invoke(this, new ChartAxisHitEventArgs(args.AreaName, args.AreaType, args.Bounds));
            }

            if (string.Equals(args.AreaType, "Legend", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(args.AreaType, "LegendItem", StringComparison.OrdinalIgnoreCase))
            {
                bool isLegendItem = string.Equals(args.AreaType, "LegendItem", StringComparison.OrdinalIgnoreCase);
                LegendHit?.Invoke(this, new ChartLegendHitEventArgs(args.AreaName, isLegendItem, args.LegendItemIndex, args.Bounds));

                // Handle legend item isolation on double-click
                if (isLegendItem && args.LegendItemIndex.HasValue)
                {
                    HandleLegendItemClick(args.LegendItemIndex.Value);
                }
            }
        }

        private ChartInteractiveAreaEventArgs BuildInteractiveAreaArgs(string areaName, Rectangle bounds)
        {
            string areaType = areaName;
            int? seriesIndex = null;
            int? pointIndex = null;
            int? legendItemIndex = null;

            if (!string.IsNullOrWhiteSpace(areaName))
            {
                var parts = areaName.Split('_');
                if (parts.Length > 0)
                    areaType = parts[0];

                // Area naming: "Type_seriesIdx_pointIdx" for 3-part
                // names (e.g. "Point_0_5"), "Type_index" for 2-part
                // names (e.g. "Legend_2").  The old code parsed
                // parts[1] as pointIndex first, then overrode both
                // from parts[1..2] — the override made triples work
                // by accident but left pairs incorrectly interpreted.
                if (parts.Length >= 3
                    && int.TryParse(parts[1], out int si)
                    && int.TryParse(parts[2], out int pi))
                {
                    seriesIndex = si;
                    pointIndex = pi;
                }
                else if (parts.Length >= 2
                         && int.TryParse(parts[1], out int idx))
                {
                    if (areaType == "Point")            pointIndex = idx;
                    else if (areaType == "Series")       seriesIndex = idx;
                    else if (areaType == "LegendItem")   legendItemIndex = idx;
                    else                                 seriesIndex = idx;
                }
            }

            if (string.Equals(areaType, "PieSlice", StringComparison.OrdinalIgnoreCase) && pointIndex.HasValue)
            {
                seriesIndex = 0;
            }

            var dataPoint = ResolveInteractivePoint(seriesIndex, pointIndex);
            return new ChartInteractiveAreaEventArgs(areaName, bounds, areaType, seriesIndex, pointIndex, legendItemIndex, dataPoint);
        }

        private ChartDataPoint ResolveInteractivePoint(int? seriesIndex, int? pointIndex)
        {
            if (!seriesIndex.HasValue || !pointIndex.HasValue)
            {
                return null;
            }

            if (seriesIndex.Value < 0 || seriesIndex.Value >= _dataSeries.Count)
            {
                return null;
            }

            var series = _dataSeries[seriesIndex.Value];
            if (series?.Points == null || pointIndex.Value < 0 || pointIndex.Value >= series.Points.Count)
            {
                return null;
            }

            return series.Points[pointIndex.Value];
        }

        private PointF GetScreenPoint(ChartDataPoint point)
        {
            if (point == null || ChartDrawingRect.Width <= 0 || ChartDrawingRect.Height <= 0)
                return PointF.Empty;

            float x = ConvertXCached(point);
            float y = ConvertYCached(point);

            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;

            if (xRange <= 0 || yRange <= 0)
            {
                return PointF.Empty;
            }

            return new PointF(
                ChartDrawingRect.Left + (x - ViewportXMin) / xRange * ChartDrawingRect.Width,
                ChartDrawingRect.Bottom - (y - ViewportYMin) / yRange * ChartDrawingRect.Height);
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

        private void DrawTrackballCrosshair(Graphics g, Rectangle plotRect)
        {
            if (_trackballCrosshairX < plotRect.Left || _trackballCrosshairX > plotRect.Right)
                return;

            var crosshairPen = PaintersFactory.GetPen(TrackballCrosshairColor, TrackballCrosshairWidth);
            g.DrawLine(crosshairPen, _trackballCrosshairX, plotRect.Top, _trackballCrosshairX, plotRect.Bottom);
        }

        private void DrawTrackballTooltip(Graphics g, Rectangle plotRect)
        {
            if (_trackballDataPoints.Count == 0)
                return;

            // Prepare text content
            var lines = new List<string>();
            foreach (var dp in _trackballDataPoints)
            {
                lines.Add($"{dp.SeriesName}: {dp.DisplayValue}");
            }

            var font = PaintersFactory.GetFont(ChartValueFont ?? SystemFonts.DefaultFont);
            float maxWidth = 0, totalHeight = 0;

            // Measure all lines
            foreach (var line in lines)
            {
                var size = TextRenderer.MeasureText(line, font);
                maxWidth = Math.Max(maxWidth, size.Width);
                totalHeight += size.Height + 2;
            }

            int padding = 8;
            int tooltipWidth = (int)maxWidth + padding * 2;
            int tooltipHeight = (int)totalHeight + padding * 2;

            // Position tooltip near crosshair
            int tooltipX = Math.Min((int)_trackballCrosshairX + 10, plotRect.Right - tooltipWidth - 5);
            tooltipX = Math.Max(tooltipX, plotRect.Left + 5);
            int tooltipY = plotRect.Top + 10;

            // Use factory-cached brushes/pens (no per-paint GDI allocation)
            var backBrush = PaintersFactory.GetSolidBrush(TrackballTooltipBackColor);
            var borderPen = PaintersFactory.GetPen(TrackballTooltipBorderColor, 1f);
            g.FillRectangle(backBrush, tooltipX, tooltipY, tooltipWidth, tooltipHeight);
            g.DrawRectangle(borderPen, tooltipX, tooltipY, tooltipWidth - 1, tooltipHeight - 1);

            // Draw text
            var textBrush = PaintersFactory.GetSolidBrush(ChartTextColor);
            float currentY = tooltipY + padding;
            foreach (var dp in _trackballDataPoints)
            {
                var line = $"{dp.SeriesName}: {dp.DisplayValue}";
                TextRenderer.DrawText(g, line, font, new Point(tooltipX + padding, (int)currentY), textBrush.Color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
                var lineHeight = TextRenderer.MeasureText(line, font).Height;
                currentY += lineHeight + 2;
            }
        }

        private void DrawSelectionHighlights(Graphics g, Rectangle plotRect)
        {
            if (!HasSelection)
                return;

            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;

            if (xRange <= 0 || yRange <= 0 || plotRect.Width <= 0 || plotRect.Height <= 0)
                return;

            var selectionBrush = PaintersFactory.GetSolidBrush(SelectionColor);
            var selectionPen = PaintersFactory.GetPen(SelectionBorderColor, 1.5f);
            float markerSize = SelectionMarkerSize * 2; // diameter

            // Draw selected points
            foreach (var (seriesIndex, pointIndex) in _selectedPoints)
            {
                if (seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
                    continue;

                var series = _dataSeries[seriesIndex];
                if (pointIndex < 0 || pointIndex >= series.Points.Count)
                    continue;

                var point = series.Points[pointIndex];
                float x = BeepChartDataHelper.ConvertXValue(this, point) is float xVal ? xVal : 0f;
                float y = BeepChartDataHelper.ConvertYValue(this, point) is float yVal ? yVal : 0f;

                float screenX = plotRect.Left + (x - ViewportXMin) / xRange * plotRect.Width;
                float screenY = plotRect.Bottom - (y - ViewportYMin) / yRange * plotRect.Height;

                // Draw highlight circle
                g.FillEllipse(selectionBrush, screenX - markerSize / 2, screenY - markerSize / 2, markerSize, markerSize);
                g.DrawEllipse(selectionPen, screenX - markerSize / 2, screenY - markerSize / 2, markerSize, markerSize);
            }
        }

        /// <summary>
        /// Draws a linear-regression trend line across the plot.
        /// Uses the first visible series' data points to compute
        /// the line, and draws it as a dashed stroke.
        /// </summary>
        private void DrawTrendLine(Graphics g, Rectangle plotRect)
        {
            // Pick the first visible series with at least 2 points
            var series = _dataSeries.FirstOrDefault(
                s => s.Visible && s.Points != null && s.Points.Count >= 2);
            if (series == null) return;

            var xs = new List<float>();
            var ys = new List<float>();
            foreach (var p in series.Points)
            {
                float x = BeepChartDataHelper.ConvertXValue(this, p) is float xf ? xf : 0f;
                float y = BeepChartDataHelper.ConvertYValue(this, p) is float yf ? yf : 0f;
                xs.Add(x); ys.Add(y);
            }
            if (xs.Count < 2) return;

            var (slope, intercept, _) = TrendLineHelper.Compute(xs, ys);

            // Colour: use the series colour at reduced alpha so it
            // reads as an overlay, not a data line.
            Color lineColor = series.Color != Color.Empty
                ? Color.FromArgb(160, series.Color)
                : Color.FromArgb(160, ChartLineColor);

            TrendLineHelper.DrawTrendLine(g, plotRect,
                slope, intercept,
                ViewportXMin, ViewportXMax,
                ViewportYMin, ViewportYMax,
                lineColor);
        }

        private void DrawKeyboardFocusIndicator(Graphics g, Rectangle plotRect)
        {
            if (_keyboardFocusedSeriesIndex < 0 || _keyboardFocusedSeriesIndex >= _dataSeries.Count)
                return;

            var series = _dataSeries[_keyboardFocusedSeriesIndex];
            if (_keyboardFocusedPointIndex < 0 || _keyboardFocusedPointIndex >= series.Points.Count)
                return;

            var point = series.Points[_keyboardFocusedPointIndex];
            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;

            if (xRange <= 0 || yRange <= 0 || plotRect.Width <= 0 || plotRect.Height <= 0)
                return;

            float x = BeepChartDataHelper.ConvertXValue(this, point) is float xVal ? xVal : 0f;
            float y = BeepChartDataHelper.ConvertYValue(this, point) is float yVal ? yVal : 0f;

            float screenX = plotRect.Left + (x - ViewportXMin) / xRange * plotRect.Width;
            float screenY = plotRect.Bottom - (y - ViewportYMin) / yRange * plotRect.Height;

            // Draw focus indicator as a dashed rectangle around the point
            float focusSize = SelectionMarkerSize * 3;
            using var focusPen = new Pen(Color.FromArgb(200, Color.Orange), 2f)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };
            g.DrawRectangle(focusPen, screenX - focusSize / 2, screenY - focusSize / 2, focusSize, focusSize);
        }

        private void DrawSeriesFillPatterns(Graphics g, Rectangle plotRect)
        {
            if (!EnableFillPatterns || _seriesFillPatterns.Count == 0)
                return;

            float xRange = ViewportXMax - ViewportXMin;
            float yRange = ViewportYMax - ViewportYMin;

            if (xRange <= 0 || yRange <= 0 || plotRect.Width <= 0 || plotRect.Height <= 0)
                return;

            // Draw pattern overlays on series fill areas
            foreach (var kvp in _seriesFillPatterns)
            {
                int seriesIndex = kvp.Key;
                var pattern = kvp.Value;

                if (seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
                    continue;

                var series = _dataSeries[seriesIndex];
                if (!series.Visible || series.Points.Count == 0)
                    continue;

                DrawPatternOverlay(g, plotRect, pattern, series.Color);
            }
        }

        private void DrawPatternOverlay(Graphics g, Rectangle plotRect, ChartSeriesFillPattern pattern, Color baseColor)
        {
            if (pattern == ChartSeriesFillPattern.Solid)
                return;

            var hatchBrush = pattern switch
            {
                ChartSeriesFillPattern.Horizontal => new HatchBrush(HatchStyle.Horizontal, Color.FromArgb(100, baseColor), Color.Transparent),
                ChartSeriesFillPattern.Vertical => new HatchBrush(HatchStyle.Vertical, Color.FromArgb(100, baseColor), Color.Transparent),
                ChartSeriesFillPattern.Diagonal => new HatchBrush(HatchStyle.ForwardDiagonal, Color.FromArgb(100, baseColor), Color.Transparent),
                ChartSeriesFillPattern.BackDiagonal => new HatchBrush(HatchStyle.BackwardDiagonal, Color.FromArgb(100, baseColor), Color.Transparent),
                ChartSeriesFillPattern.Cross => new HatchBrush(HatchStyle.Cross, Color.FromArgb(100, baseColor), Color.Transparent),
                ChartSeriesFillPattern.DiagonalCross => new HatchBrush(HatchStyle.DiagonalCross, Color.FromArgb(100, baseColor), Color.Transparent),
                ChartSeriesFillPattern.Dots => new HatchBrush(HatchStyle.SmallConfetti, Color.FromArgb(100, baseColor), Color.Transparent),
                _ => null
            };

            if (hatchBrush != null)
            {
                g.FillRectangle(hatchBrush, plotRect);
                hatchBrush.Dispose();
            }
        }
    }
}