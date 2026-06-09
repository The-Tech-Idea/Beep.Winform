using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        private int _batchUpdateCount;
        private bool _batchNeedsRefresh;

        /// <summary>
        /// Suppresses repaints during batch data operations.
        /// Must be paired with <see cref="EndUpdate"/>.
        /// Nestable — only the outermost EndUpdate triggers a refresh.
        /// </summary>
        public void BeginUpdate()
        {
            _batchUpdateCount++;
        }

        /// <summary>
        /// Resumes repaints after <see cref="BeginUpdate"/> and
        /// refreshes the chart if data changed.
        /// </summary>
        public void EndUpdate()
        {
            if (_batchUpdateCount > 0) _batchUpdateCount--;
            if (_batchUpdateCount == 0 && _batchNeedsRefresh)
            {
                _batchNeedsRefresh = false;
                RefreshDataState();
                Invalidate();
            }
        }

        private void InvalidateOrDefer()
        {
            if (_batchUpdateCount > 0)
                _batchNeedsRefresh = true;
            else
                Invalidate();
        }

        /// <summary>
        /// Binds a DataTable, DataView, or IList to the chart.
        /// Numeric columns become Y series; the first non-numeric
        /// column becomes the X axis (category axis).  Each numeric
        /// column gets its own <see cref="ChartDataSeries"/> with
        /// points derived from each row.
        /// </summary>
        public void BindDataSource(object source, string? xColumn = null)
        {
            var seriesList = new List<ChartDataSeries>();
            if (source is DataTable dt)
                BindDataTable(dt, xColumn, seriesList);
            else if (source is System.Data.DataView dv)
                BindDataTable(dv.Table!, xColumn, seriesList);
            else if (source is IList list)
                BindIList(list, xColumn, seriesList);
            else
                return;

            DataSeries = seriesList;
        }

        private void BindDataTable(DataTable dt, string? xCol, List<ChartDataSeries> outList)
        {
            if (dt.Rows.Count == 0) return;
            // Find the X-axis column: explicit name, or first string column
            string xColName = xCol;
            if (xColName == null)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.DataType == typeof(string) || dc.DataType == typeof(DateTime))
                    { xColName = dc.ColumnName; break; }
                }
            }

            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.DataType == typeof(string) && dc.ColumnName != xColName) continue;
                if (!IsNumericType(dc.DataType) && dc.DataType != typeof(DateTime)) continue;
                if (dc.ColumnName == xColName) continue;

                var series = new ChartDataSeries { Name = dc.ColumnName };
                foreach (DataRow row in dt.Rows)
                {
                    string x = xColName != null ? row[xColName]?.ToString() ?? "" : "";
                    float y = 0f;
                    try { y = Convert.ToSingle(row[dc]); } catch { }
                    series.Points.Add(new ChartDataPoint(x, y.ToString(), y));
                }
                outList.Add(series);
            }
        }

        private void BindIList(IList list, string? xProp, List<ChartDataSeries> outList)
        {
            if (list.Count == 0) return;
            var itemType = list[0]!.GetType();
            var props = itemType.GetProperties();

            // Find X property
            string xPropName = xProp;
            if (xPropName == null)
            {
                foreach (var p in props)
                {
                    if (p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime))
                    { xPropName = p.Name; break; }
                }
            }

            foreach (var prop in props)
            {
                if (!IsNumericType(prop.PropertyType)) continue;
                if (prop.Name == xPropName) continue;

                var series = new ChartDataSeries { Name = prop.Name };
                foreach (var item in list)
                {
                    if (item == null) continue;
                    string x = xPropName != null
                        ? itemType.GetProperty(xPropName)?.GetValue(item)?.ToString() ?? ""
                        : "";
                    float y = 0f;
                    try { y = Convert.ToSingle(prop.GetValue(item)); } catch { }
                    series.Points.Add(new ChartDataPoint(x, y.ToString(), y));
                }
                outList.Add(series);
            }
        }

        private static bool IsNumericType(Type t)
            => t == typeof(int) || t == typeof(long) || t == typeof(float)
            || t == typeof(double) || t == typeof(decimal)
            || t == typeof(short) || t == typeof(byte);

        public List<ChartDataPoint> GetOptimizedPointsForSeries(int seriesIndex)
        {
            if (seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
                return new List<ChartDataPoint>();

            if (!IsLargeDataset() || !EnableVertexSimplification)
            {
                return _dataSeries[seriesIndex].Points.ToList();
            }

            // Return cached simplified points if available
            if (_simplifiedPointsCache.TryGetValue(seriesIndex, out var cached))
            {
                return cached;
            }

            // Simplify and cache
            var original = _dataSeries[seriesIndex].Points;
            var simplified = SimplifyPointsRDP(original, _simplificationTolerance);
            _simplifiedPointsCache[seriesIndex] = simplified;

            return simplified;
        }

        private List<ChartDataPoint> SimplifyPointsRDP(List<ChartDataPoint> points, float epsilon)
        {
            // Ramer-Douglas-Peucker line simplification
            if (points.Count <= 2)
                return new List<ChartDataPoint>(points);

            var dmax = 0f;
            var index = 0;

            for (int i = 1; i < points.Count - 1; i++)
            {
                var d = PerpendicularDistance(points[i], points[0], points[points.Count - 1]);
                if (d > dmax)
                {
                    index = i;
                    dmax = d;
                }
            }

            var result = new List<ChartDataPoint>();
            if (dmax > epsilon)
            {
                var rec1 = SimplifyPointsRDP(points.Take(index + 1).ToList(), epsilon);
                var rec2 = SimplifyPointsRDP(points.Skip(index).ToList(), epsilon);
                result.AddRange(rec1.Take(rec1.Count - 1));
                result.AddRange(rec2);
            }
            else
            {
                result.Add(points[0]);
                result.Add(points[points.Count - 1]);
            }

            return result;
        }

        private float PerpendicularDistance(ChartDataPoint pt, ChartDataPoint lineStart, ChartDataPoint lineEnd)
        {
            // Distance from point to line segment
            var x1 = lineStart.X is object x1Obj ? Convert.ToSingle(x1Obj) : 0f;
            var y1 = lineStart.Y is object y1Obj ? Convert.ToSingle(y1Obj) : 0f;
            var x2 = lineEnd.X is object x2Obj ? Convert.ToSingle(x2Obj) : 0f;
            var y2 = lineEnd.Y is object y2Obj ? Convert.ToSingle(y2Obj) : 0f;
            var x0 = pt.X is object x0Obj ? Convert.ToSingle(x0Obj) : 0f;
            var y0 = pt.Y is object y0Obj ? Convert.ToSingle(y0Obj) : 0f;

            var numerator = Math.Abs((y2 - y1) * x0 - (x2 - x1) * y0 + x2 * y1 - y2 * x1);
            var denominator = (float)Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));

            return denominator > 0 ? numerator / denominator : 0f;
        }

        public List<ChartDataPoint> GetCulledPointsForViewport(List<ChartDataPoint> points)
        {
            if (!IsLargeDataset() || !EnablePointCulling)
                return points;

            // Cull points outside current viewport
            var culled = new List<ChartDataPoint>();
            foreach (var point in points)
            {
                var x = ConvertXValue(point);
                var y = ConvertYValue(point);

                if (x is float xVal && y is float yVal)
                {
                    // Keep point if within viewport (with small margin)
                    if (xVal >= ViewportXMin - 0.1f && xVal <= ViewportXMax + 0.1f &&
                        yVal >= ViewportYMin - 0.1f && yVal <= ViewportYMax + 0.1f)
                    {
                        culled.Add(point);
                    }
                }
            }

            return culled.Count > 0 ? culled : points.Take(1).ToList(); // Return at least 1 point
        }

        public void ClearSimplificationCache()
        {
            _simplifiedPointsCache.Clear();
        }

        public string GetPerformanceReport()
        {
            var totalPoints = GetTotalPointCount();
            var isLarge = IsLargeDataset();
            var decimationFactor = BeepChartPerformanceHelper.EstimateDecimationFactor(totalPoints, 200);
            var gridDensity = BeepChartPerformanceHelper.OptimizeGridDensity(totalPoints);
            var renderingCost = BeepChartPerformanceHelper.EstimateRenderingCost(totalPoints, Width * Height);

            return $"Total Points: {totalPoints}, Large Dataset: {isLarge}, " +
                   $"Decimation: {decimationFactor}x, Grid Density: {gridDensity}, " +
                   $"Rendering Cost: {renderingCost:P1}, Cache Size: {_simplifiedPointsCache.Count} series";
        }

        public int AppendDataPoint(int seriesIndex, ChartDataPoint point, bool requestRender = true)
        {
            if (point == null || seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
                return 0;

            var series = _dataSeries[seriesIndex];
            if (series.Points == null)
            {
                series.Points = new List<ChartDataPoint>();
            }

            series.Points.Add(point);
            TrimSeriesWindowIfNeeded(series);
            UpdateViewportForStreamingPoint(point);

            _streamRefreshPending = true;
            _pendingStreamPointCount++;

            StreamingDataAppended?.Invoke(this, new ChartStreamingDataAppendedEventArgs(
                seriesIndex,
                series.Name,
                1,
                series.Points.Count,
                GetTotalPointCount()));

            if (requestRender)
            {
                RequestIncrementalRender();
            }

            return 1;
        }

        public int AppendDataPoints(int seriesIndex, IEnumerable<ChartDataPoint> points, bool requestRender = true)
        {
            if (points == null || seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
                return 0;

            var series = _dataSeries[seriesIndex];
            if (series.Points == null)
            {
                series.Points = new List<ChartDataPoint>();
            }

            int added = 0;
            foreach (var point in points)
            {
                if (point == null)
                    continue;

                series.Points.Add(point);
                added++;
                UpdateViewportForStreamingPoint(point);
            }

            if (added <= 0)
                return 0;

            TrimSeriesWindowIfNeeded(series);

            _streamRefreshPending = true;
            _pendingStreamPointCount += added;

            StreamingDataAppended?.Invoke(this, new ChartStreamingDataAppendedEventArgs(
                seriesIndex,
                series.Name,
                added,
                series.Points.Count,
                GetTotalPointCount()));

            if (requestRender)
            {
                RequestIncrementalRender();
            }

            return added;
        }

        public int AppendDataPoint(string seriesName, ChartDataPoint point, bool createSeriesIfMissing = true, bool requestRender = true)
        {
            if (point == null || string.IsNullOrWhiteSpace(seriesName))
                return 0;

            int seriesIndex = _dataSeries.FindIndex(s => string.Equals(s.Name, seriesName, StringComparison.OrdinalIgnoreCase));
            if (seriesIndex < 0 && createSeriesIfMissing)
            {
                var newSeries = new ChartDataSeries
                {
                    Name = seriesName,
                    Visible = true,
                    Points = new List<ChartDataPoint>()
                };
                _dataSeries.Add(newSeries);
                seriesIndex = _dataSeries.Count - 1;
            }

            return AppendDataPoint(seriesIndex, point, requestRender);
        }

        public void FlushStreamingRender()
        {
            RequestIncrementalRender(force: true);
        }

        #region Public Methods
        public void ApplyVisualPreset(ChartVisualPreset preset)
        {
            switch (preset)
            {
                case ChartVisualPreset.Dashboard:
                    ApplyDashboardPreset();
                    break;
                case ChartVisualPreset.Analytical:
                    ApplyAnalyticalPreset();
                    break;
                case ChartVisualPreset.HighContrast:
                    ApplyHighContrastPreset();
                    break;
                case ChartVisualPreset.Print:
                    ApplyPrintPreset();
                    break;
                case ChartVisualPreset.Presentation:
                    ApplyPresentationPreset();
                    break;
            }

            Invalidate();
        }

        private void ApplyDashboardPreset()
        {
            // Dashboard: Light, compact, minimal grid
            ChartBackColor = Color.White;
            ChartGridLineColor = Color.FromArgb(230, 230, 230);
            ShowLegend = true;
            LegendPlacement = LegendPlacement.InsideTopRight;
            ChartTitleFont = new Font("Segoe UI", 14f, FontStyle.Bold);
            ChartValueFont = new Font("Segoe UI", 11f);
            ChartLineColor = Color.FromArgb(200, 200, 200);
            ChartTextColor = Color.FromArgb(80, 80, 80);
        }

        private void ApplyAnalyticalPreset()
        {
            // Analytical: High contrast, detailed grid, precision focus
            ChartBackColor = Color.White;
            ChartGridLineColor = Color.FromArgb(200, 200, 200);
            ShowLegend = true;
            LegendPlacement = LegendPlacement.Right;
            ChartTitleFont = new Font("Segoe UI", 12f, FontStyle.Bold);
            ChartValueFont = new Font("Segoe UI", 10f);
            ChartLineColor = Color.FromArgb(150, 150, 150);
            ChartTextColor = Color.Black;
        }

        private void ApplyHighContrastPreset()
        {
            // High Contrast: Accessibility - strong colors, thick lines
            ChartBackColor = Color.White;
            ChartGridLineColor = Color.Black;
            ShowLegend = true;
            LegendPlacement = LegendPlacement.Bottom;
            ChartTitleFont = new Font("Segoe UI", 14f, FontStyle.Bold);
            ChartValueFont = new Font("Segoe UI", 12f, FontStyle.Bold);
            ChartLineColor = Color.Black;
            ChartTextColor = Color.Black;
            // Update series colors to high-contrast palette
            var hsColorScheme = new List<Color>
            {
                Color.Black,
                Color.FromArgb(0, 0, 255),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(0, 128, 0),
                Color.FromArgb(255, 165, 0)
            };
            ChartDefaultSeriesColors = hsColorScheme;
        }

        private void ApplyPrintPreset()
        {
            // Print: Monochrome-friendly, high detail, B&W output
            ChartBackColor = Color.White;
            ChartGridLineColor = Color.FromArgb(180, 180, 180);
            ShowLegend = true;
            LegendPlacement = LegendPlacement.Bottom;
            ChartTitleFont = new Font("Segoe UI", 13f, FontStyle.Bold);
            ChartValueFont = new Font("Segoe UI", 10f);
            ChartLineColor = Color.FromArgb(100, 100, 100);
            ChartTextColor = Color.Black;
            // Use grayscale for better B&W printing
            var bwColorScheme = new List<Color>
            {
                Color.Black,
                Color.FromArgb(80, 80, 80),
                Color.FromArgb(160, 160, 160),
                Color.FromArgb(40, 40, 40),
                Color.FromArgb(120, 120, 120)
            };
            ChartDefaultSeriesColors = bwColorScheme;
        }

        private void ApplyPresentationPreset()
        {
            // Presentation: Vibrant, large fonts, minimal clutter
            ChartBackColor = Color.White;
            ChartGridLineColor = Color.FromArgb(240, 240, 240);
            ShowLegend = true;
            LegendPlacement = LegendPlacement.Bottom;
            ChartTitleFont = new Font("Segoe UI", 18f, FontStyle.Bold);
            ChartValueFont = new Font("Segoe UI", 14f, FontStyle.Bold);
            ChartLineColor = Color.FromArgb(220, 220, 220);
            ChartTextColor = Color.FromArgb(60, 60, 60);
            // Use vibrant colors for presentation
            var vibrantColorScheme = new List<Color>
            {
                Color.FromArgb(0, 150, 200),
                Color.FromArgb(255, 107, 107),
                Color.FromArgb(52, 211, 153),
                Color.FromArgb(251, 191, 36),
                Color.FromArgb(139, 92, 246)
            };
            ChartDefaultSeriesColors = vibrantColorScheme;
        }

        public void StartAnimation(int durationMs = 600)
        {
            var elapsed = 0;
            void Tick(object sender, EventArgs e)
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

        public object ConvertXValue(ChartDataPoint point)
        {
            return BeepChartDataHelper.ConvertXValue(this, point);
        }

        public object ConvertYValue(ChartDataPoint point)
        {
            return BeepChartDataHelper.ConvertYValue(this, point);
        }

        public bool ResetViewport()
        {
            if (!CanResetViewport)
            {
                return false;
            }

            var oldViewport = GetViewportSnapshot();
            BeepChartViewportHelper.AutoScaleViewport(this);
            RaiseViewportChangedIfNeeded(oldViewport, "Reset");
            return true;
        }

        public bool IsolateSeriesAt(int seriesIndex)
        {
            if (!EnableLegendIsolate || seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
            {
                return false;
            }

            // If already isolated, restore first
            if (_isolatedSeriesIndex >= 0)
            {
                RestoreAllSeries();
            }

            // Save current visibility state
            _seriesVisibilityBeforeIsolation.Clear();
            for (int i = 0; i < _dataSeries.Count; i++)
            {
                _seriesVisibilityBeforeIsolation[i] = _dataSeries[i].Visible;
            }

            // Hide all series except the selected one
            for (int i = 0; i < _dataSeries.Count; i++)
            {
                _dataSeries[i].Visible = (i == seriesIndex);
            }

            _isolatedSeriesIndex = seriesIndex;
            RefreshDataState(invalidate: true);
            SeriesIsolationChanged?.Invoke(this, new ChartSeriesIsolationChangedEventArgs(seriesIndex, true, _dataSeries[seriesIndex].Name));
            return true;
        }

        public bool RestoreAllSeries()
        {
            if (_isolatedSeriesIndex < 0)
            {
                return false;
            }

            // Restore visibility state
            foreach (var kvp in _seriesVisibilityBeforeIsolation)
            {
                if (kvp.Key < _dataSeries.Count)
                {
                    _dataSeries[kvp.Key].Visible = kvp.Value;
                }
            }

            int wasIsolated = _isolatedSeriesIndex;
            _isolatedSeriesIndex = -1;
            _seriesVisibilityBeforeIsolation.Clear();
            RefreshDataState(invalidate: true);
            SeriesIsolationChanged?.Invoke(this, new ChartSeriesIsolationChangedEventArgs(wasIsolated, false, null));
            return true;
        }

        public bool IsSeriesIsolated => _isolatedSeriesIndex >= 0;
        public int IsolatedSeriesIndex => _isolatedSeriesIndex;

        public bool SelectPoint(int seriesIndex, int pointIndex, bool addToSelection = false)
        {
            if (!EnablePointSelection || seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
            {
                return false;
            }

            var series = _dataSeries[seriesIndex];
            if (pointIndex < 0 || pointIndex >= series.Points.Count)
            {
                return false;
            }

            var key = (seriesIndex, pointIndex);

            // Single-select mode: clear previous selection
            if (_selectionMode == ChartSelectionMode.Single && !addToSelection)
            {
                _selectedPoints.Clear();
            }

            if (_selectedPoints.Add(key))
            {
                Invalidate();
                SelectionChanged?.Invoke(this, new ChartSelectionChangedEventArgs(
                    _selectedPoints.ToList(),
                    _selectedSeries.ToList(),
                    seriesIndex,
                    pointIndex));
                return true;
            }

            return false;
        }

        public bool SelectSeries(int seriesIndex, bool addToSelection = false)
        {
            if (!EnablePointSelection || seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
            {
                return false;
            }

            // Single-select mode: clear previous selection
            if (_selectionMode == ChartSelectionMode.Single && !addToSelection)
            {
                _selectedSeries.Clear();
            }

            if (_selectedSeries.Add(seriesIndex))
            {
                Invalidate();
                SelectionChanged?.Invoke(this, new ChartSelectionChangedEventArgs(
                    _selectedPoints.ToList(),
                    _selectedSeries.ToList(),
                    seriesIndex,
                    -1));
                return true;
            }

            return false;
        }

        public bool DeselectPoint(int seriesIndex, int pointIndex)
        {
            var key = (seriesIndex, pointIndex);
            if (_selectedPoints.Remove(key))
            {
                Invalidate();
                SelectionChanged?.Invoke(this, new ChartSelectionChangedEventArgs(
                    _selectedPoints.ToList(),
                    _selectedSeries.ToList(),
                    seriesIndex,
                    pointIndex));
                return true;
            }

            return false;
        }

        public bool DeselectSeries(int seriesIndex)
        {
            if (_selectedSeries.Remove(seriesIndex))
            {
                Invalidate();
                SelectionChanged?.Invoke(this, new ChartSelectionChangedEventArgs(
                    _selectedPoints.ToList(),
                    _selectedSeries.ToList(),
                    seriesIndex,
                    -1));
                return true;
            }

            return false;
        }

        public void ClearSelection()
        {
            if (_selectedPoints.Count > 0 || _selectedSeries.Count > 0)
            {
                _selectedPoints.Clear();
                _selectedSeries.Clear();
                Invalidate();
                SelectionChanged?.Invoke(this, new ChartSelectionChangedEventArgs(
                    new List<(int, int)>(),
                    new List<int>(),
                    -1,
                    -1));
            }
        }

        public int SelectedPointCount => _selectedPoints.Count;
        public int SelectedSeriesCount => _selectedSeries.Count;
        public bool HasSelection => _selectedPoints.Count > 0 || _selectedSeries.Count > 0;

        public string GetSelectionAsCSV()
        {
            if (_selectedPoints.Count == 0)
                return string.Empty;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("SeriesName,SeriesIndex,PointIndex,XValue,YValue,DisplayValue");

            foreach (var (seriesIndex, pointIndex) in _selectedPoints)
            {
                if (seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
                    continue;

                var series = _dataSeries[seriesIndex];
                if (pointIndex < 0 || pointIndex >= series.Points.Count)
                    continue;

                var point = series.Points[pointIndex];
                var xValue = ConvertXValue(point);
                var yValue = ConvertYValue(point);
                sb.AppendLine($"\"{series.Name}\",{seriesIndex},{pointIndex},{xValue},{yValue},{point.Y:G4}");
            }

            return sb.ToString();
        }

        public string GetSelectionAsJSON()
        {
            if (_selectedPoints.Count == 0)
                return "[]";

            var list = new List<Dictionary<string, object>>();

            foreach (var (seriesIndex, pointIndex) in _selectedPoints)
            {
                if (seriesIndex < 0 || seriesIndex >= _dataSeries.Count)
                    continue;

                var series = _dataSeries[seriesIndex];
                if (pointIndex < 0 || pointIndex >= series.Points.Count)
                    continue;

                var point = series.Points[pointIndex];
                list.Add(new Dictionary<string, object>
                {
                    { "SeriesName", series.Name },
                    { "SeriesIndex", seriesIndex },
                    { "PointIndex", pointIndex },
                    { "XValue", ConvertXValue(point) },
                    { "YValue", ConvertYValue(point) },
                    { "DisplayValue", point.Y }
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(list, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }

        public string ExportAllDataAsCSV(bool onlyVisibleSeries = true)
        {
            if (_dataSeries.Count == 0)
                return string.Empty;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("SeriesName,SeriesIndex,PointIndex,XValue,YValue,DisplayValue,Visible");

            for (int seriesIndex = 0; seriesIndex < _dataSeries.Count; seriesIndex++)
            {
                var series = _dataSeries[seriesIndex];
                if (series?.Points == null)
                    continue;

                if (onlyVisibleSeries && !series.Visible)
                    continue;

                for (int pointIndex = 0; pointIndex < series.Points.Count; pointIndex++)
                {
                    var point = series.Points[pointIndex];
                    var xValue = ConvertXValue(point);
                    var yValue = ConvertYValue(point);
                    sb.AppendLine($"\"{series.Name}\",{seriesIndex},{pointIndex},{xValue},{yValue},{point.Y:G4},{series.Visible}");
                }
            }

            return sb.ToString();
        }

        public string ExportAllDataAsJSON(bool onlyVisibleSeries = true, bool indented = true)
        {
            var list = new List<Dictionary<string, object>>();

            for (int seriesIndex = 0; seriesIndex < _dataSeries.Count; seriesIndex++)
            {
                var series = _dataSeries[seriesIndex];
                if (series?.Points == null)
                    continue;

                if (onlyVisibleSeries && !series.Visible)
                    continue;

                for (int pointIndex = 0; pointIndex < series.Points.Count; pointIndex++)
                {
                    var point = series.Points[pointIndex];
                    list.Add(new Dictionary<string, object>
                    {
                        { "SeriesName", series.Name },
                        { "SeriesIndex", seriesIndex },
                        { "PointIndex", pointIndex },
                        { "XValue", ConvertXValue(point) },
                        { "YValue", ConvertYValue(point) },
                        { "DisplayValue", point.Y },
                        { "Visible", series.Visible }
                    });
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(
                list,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = indented });
        }

        public Bitmap CaptureChartBitmap()
        {
            int width = Math.Max(1, ClientSize.Width);
            int height = Math.Max(1, ClientSize.Height);
            var bitmap = new Bitmap(width, height);
            DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));
            return bitmap;
        }

        public bool SaveChartImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                using var bmp = CaptureChartBitmap();
                var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
                var format = ext switch
                {
                    ".jpg" => ImageFormat.Jpeg,
                    ".jpeg" => ImageFormat.Jpeg,
                    ".bmp" => ImageFormat.Bmp,
                    ".gif" => ImageFormat.Gif,
                    ".tif" => ImageFormat.Tiff,
                    ".tiff" => ImageFormat.Tiff,
                    _ => ImageFormat.Png
                };

                bmp.Save(filePath, format);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public PrintDocument CreatePrintDocument(bool fitToPage = true)
        {
            var document = new PrintDocument();
            document.PrintPage += (sender, e) =>
            {
                using var bmp = CaptureChartBitmap();
                if (fitToPage)
                {
                    var target = e.MarginBounds;
                    e.Graphics.DrawImage(bmp, target);
                }
                else
                {
                    e.Graphics.DrawImage(bmp, 0, 0);
                }
                e.HasMorePages = false;
            };
            return document;
        }

        public bool PrintChart(bool showPrintDialog = true, bool fitToPage = true)
        {
            try
            {
                using var document = CreatePrintDocument(fitToPage);
                if (showPrintDialog)
                {
                    using var dialog = new PrintDialog { Document = document };
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return false;
                    }
                }

                document.Print();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ShowPrintPreview(bool fitToPage = true)
        {
            try
            {
                using var document = CreatePrintDocument(fitToPage);
                using var previewDialog = new PrintPreviewDialog
                {
                    Document = document,
                    Width = 1000,
                    Height = 700
                };

                previewDialog.ShowDialog();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ExportChartStateAsJSON(bool indented = true)
        {
            var state = new ChartSerializableState
            {
                ViewportXMin = ViewportXMin,
                ViewportXMax = ViewportXMax,
                ViewportYMin = ViewportYMin,
                ViewportYMax = ViewportYMax,
                SurfaceStyle = SurfaceStyle,
                ChartType = ChartType,
                CurrentVisualPreset = CurrentVisualPreset,
                ShowLegend = ShowLegend,
                EnablePerformanceMode = EnablePerformanceMode,
                EnableRealTimeStreaming = EnableRealTimeStreaming,
                EnableDenseLabelOptimization = EnableDenseLabelOptimization,
                MaxVisibleAxisLabels = MaxVisibleAxisLabels,
                MaxStreamPointsPerSeries = MaxStreamPointsPerSeries,
                StreamRenderThrottleMs = StreamRenderThrottleMs
            };

            return System.Text.Json.JsonSerializer.Serialize(
                state,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = indented });
        }

        public bool ImportChartStateFromJSON(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                var state = System.Text.Json.JsonSerializer.Deserialize<ChartSerializableState>(json);
                if (state == null)
                    return false;

                ViewportXMin = state.ViewportXMin;
                ViewportXMax = state.ViewportXMax;
                ViewportYMin = state.ViewportYMin;
                ViewportYMax = state.ViewportYMax;
                SurfaceStyle = state.SurfaceStyle;
                ChartType = state.ChartType;
                CurrentVisualPreset = state.CurrentVisualPreset;
                ShowLegend = state.ShowLegend;
                EnablePerformanceMode = state.EnablePerformanceMode;
                EnableRealTimeStreaming = state.EnableRealTimeStreaming;
                EnableDenseLabelOptimization = state.EnableDenseLabelOptimization;
                MaxVisibleAxisLabels = state.MaxVisibleAxisLabels;
                MaxStreamPointsPerSeries = state.MaxStreamPointsPerSeries;
                StreamRenderThrottleMs = state.StreamRenderThrottleMs;

                Invalidate();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveChartStateToFile(string filePath, bool indented = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                var json = ExportChartStateAsJSON(indented);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadChartStateFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            try
            {
                var json = File.ReadAllText(filePath);
                return ImportChartStateFromJSON(json);
            }
            catch
            {
                return false;
            }
        }

        private sealed class ChartSerializableState
        {
            public float ViewportXMin { get; set; }
            public float ViewportXMax { get; set; }
            public float ViewportYMin { get; set; }
            public float ViewportYMax { get; set; }
            public ChartSurfaceStyle SurfaceStyle { get; set; }
            public ChartType ChartType { get; set; }
            public ChartVisualPreset CurrentVisualPreset { get; set; }
            public bool ShowLegend { get; set; }
            public bool EnablePerformanceMode { get; set; }
            public bool EnableRealTimeStreaming { get; set; }
            public bool EnableDenseLabelOptimization { get; set; }
            public int MaxVisibleAxisLabels { get; set; }
            public int MaxStreamPointsPerSeries { get; set; }
            public int StreamRenderThrottleMs { get; set; }
        }
        #endregion

        #region Internal Methods
        internal void HandleContextMenuRequest(Point location)
        {
            var args = new ChartContextMenuRequestedEventArgs(
                location,
                _selectedPoints.ToList(),
                _selectedSeries.ToList());

            // Create default menu if host didn't provide one
            if (args.Menu == null && EnableContextMenu)
            {
                args.Menu = CreateDefaultContextMenu();
            }

            // Raise event for host customization
            ContextMenuRequested?.Invoke(this, args);

            // Show menu if it was created/provided
            if (args.Menu != null)
            {
                args.Menu.Show(this, location);
            }
        }

        private ContextMenuStrip CreateDefaultContextMenu()
        {
            var menu = new ContextMenuStrip();

            if (HasSelection)
            {
                var copyCSVItem = new ToolStripMenuItem("Copy as CSV");
                copyCSVItem.Click += (s, e) =>
                {
                    var csv = GetSelectionAsCSV();
                    if (!string.IsNullOrEmpty(csv))
                    {
                        Clipboard.SetText(csv);
                    }
                };
                menu.Items.Add(copyCSVItem);

                var copyJSONItem = new ToolStripMenuItem("Copy as JSON");
                copyJSONItem.Click += (s, e) =>
                {
                    var json = GetSelectionAsJSON();
                    if (!string.IsNullOrEmpty(json))
                    {
                        Clipboard.SetText(json);
                    }
                };
                menu.Items.Add(copyJSONItem);

                menu.Items.Add(new ToolStripSeparator());

                var clearSelectionItem = new ToolStripMenuItem("Clear Selection");
                clearSelectionItem.Click += (s, e) => ClearSelection();
                menu.Items.Add(clearSelectionItem);
            }
            else
            {
                var noSelectionItem = new ToolStripMenuItem("(No selection)") { Enabled = false };
                menu.Items.Add(noSelectionItem);
            }

            menu.Items.Add(new ToolStripSeparator());

            var copyAllCSVItem = new ToolStripMenuItem("Copy All Visible Data as CSV");
            copyAllCSVItem.Click += (s, e) =>
            {
                var csv = ExportAllDataAsCSV(onlyVisibleSeries: true);
                if (!string.IsNullOrEmpty(csv))
                {
                    Clipboard.SetText(csv);
                }
            };
            menu.Items.Add(copyAllCSVItem);

            var copyAllJSONItem = new ToolStripMenuItem("Copy All Visible Data as JSON");
            copyAllJSONItem.Click += (s, e) =>
            {
                var json = ExportAllDataAsJSON(onlyVisibleSeries: true, indented: true);
                if (!string.IsNullOrEmpty(json))
                {
                    Clipboard.SetText(json);
                }
            };
            menu.Items.Add(copyAllJSONItem);

            var copyImageItem = new ToolStripMenuItem("Copy Chart Image");
            copyImageItem.Click += (s, e) =>
            {
                using var bmp = CaptureChartBitmap();
                Clipboard.SetImage(new Bitmap(bmp));
            };
            menu.Items.Add(copyImageItem);

            var printItem = new ToolStripMenuItem("Print Chart...");
            printItem.Click += (s, e) => PrintChart(showPrintDialog: true, fitToPage: true);
            menu.Items.Add(printItem);

            var copyStateItem = new ToolStripMenuItem("Copy Chart State as JSON");
            copyStateItem.Click += (s, e) =>
            {
                var state = ExportChartStateAsJSON(indented: true);
                if (!string.IsNullOrEmpty(state))
                {
                    Clipboard.SetText(state);
                }
            };
            menu.Items.Add(copyStateItem);

            return menu;
        }

        internal void HandleLegendItemClick(int legendItemIndex)
        {
            if (legendItemIndex < 0 || legendItemIndex >= _dataSeries.Count)
            {
                return;
            }

            // Detect double-click: same item within 300ms
            const int doubleClickTimeMs = 300;
            bool isDoubleClick = (_lastLegendItemClickIndex == legendItemIndex &&
                                  (DateTime.Now - _lastLegendItemClickTime).TotalMilliseconds < doubleClickTimeMs);

            _lastLegendItemClickIndex = legendItemIndex;
            _lastLegendItemClickTime = DateTime.Now;

            if (isDoubleClick)
            {
                // Toggle isolation
                if (IsSeriesIsolated && _isolatedSeriesIndex == legendItemIndex)
                {
                    RestoreAllSeries();
                }
                else
                {
                    IsolateSeriesAt(legendItemIndex);
                }
            }
        }

        internal void HandleKeyboardSeriesCycling(bool nextSeries)
        {
            if (_dataSeries.Count == 0)
                return;

            if (nextSeries)
            {
                _keyboardFocusedSeriesIndex = (_keyboardFocusedSeriesIndex + 1) % _dataSeries.Count;
            }
            else
            {
                _keyboardFocusedSeriesIndex = (_keyboardFocusedSeriesIndex - 1 + _dataSeries.Count) % _dataSeries.Count;
            }

            _keyboardFocusedPointIndex = 0;
            RaiseKeyboardFocusChanged();
            Invalidate();
        }

        internal void HandleKeyboardPointNavigation(bool previous)
        {
            if (_dataSeries.Count == 0 || _dataSeries[_keyboardFocusedSeriesIndex].Points.Count == 0)
                return;

            var series = _dataSeries[_keyboardFocusedSeriesIndex];
            int pointCount = series.Points.Count;

            if (previous)
            {
                _keyboardFocusedPointIndex = (_keyboardFocusedPointIndex - 1 + pointCount) % pointCount;
            }
            else
            {
                _keyboardFocusedPointIndex = (_keyboardFocusedPointIndex + 1) % pointCount;
            }

            RaiseKeyboardFocusChanged();
            Invalidate();
        }

        internal void HandleKeyboardPointSelection(bool toggle)
        {
            if (_dataSeries.Count == 0 || _keyboardFocusedSeriesIndex >= _dataSeries.Count)
                return;

            var series = _dataSeries[_keyboardFocusedSeriesIndex];
            if (_keyboardFocusedPointIndex >= series.Points.Count)
                return;

            if (toggle)
            {
                var key = (_keyboardFocusedSeriesIndex, _keyboardFocusedPointIndex);
                if (_selectedPoints.Contains(key))
                {
                    DeselectPoint(_keyboardFocusedSeriesIndex, _keyboardFocusedPointIndex);
                }
                else
                {
                    SelectPoint(_keyboardFocusedSeriesIndex, _keyboardFocusedPointIndex, addToSelection: true);
                }
            }
            else
            {
                SelectPoint(_keyboardFocusedSeriesIndex, _keyboardFocusedPointIndex, addToSelection: false);
            }
        }

        internal void HandleKeyboardSelectAll()
        {
            if (EnablePointSelection && _dataSeries.Count > 0)
            {
                for (int s = 0; s < _dataSeries.Count; s++)
                {
                    var series = _dataSeries[s];
                    for (int p = 0; p < series.Points.Count; p++)
                    {
                        var key = (s, p);
                        if (!_selectedPoints.Contains(key))
                        {
                            _selectedPoints.Add(key);
                        }
                    }
                }

                Invalidate();
                SelectionChanged?.Invoke(this, new ChartSelectionChangedEventArgs(
                    _selectedPoints.ToList(),
                    _selectedSeries.ToList(),
                    -1,
                    -1));
            }
        }

        internal void RaiseKeyboardFocusChanged()
        {
            if (_keyboardFocusedSeriesIndex >= 0 && _keyboardFocusedSeriesIndex < _dataSeries.Count)
            {
                var series = _dataSeries[_keyboardFocusedSeriesIndex];
                if (_keyboardFocusedPointIndex >= 0 && _keyboardFocusedPointIndex < series.Points.Count)
                {
                    var point = series.Points[_keyboardFocusedPointIndex];
                    KeyboardFocusChanged?.Invoke(this, new ChartKeyboardFocusChangedEventArgs(
                        _keyboardFocusedSeriesIndex,
                        _keyboardFocusedPointIndex,
                        series.Name,
                        point));
                }
            }
        }

        // Internal helpers continue below.
        internal void ToggleSeriesByIndex(int index)
        {
            if (index >= 0 && index < _dataSeries.Count)
            {
                _dataSeries[index].Visible = !_dataSeries[index].Visible;
                Invalidate();
            }
        }

        internal void ShowTooltip(ChartDataPoint point)
        {
            if (_dataPointToolTip != null)
            {
                string tooltipText = point.ToolTip ?? $"{point.X}, {point.Y}";
                _dataPointToolTip.Show(tooltipText, this, PointToClient(MousePosition), 3000);
            }
        }

        internal void HideTooltip()
        {
            _dataPointToolTip?.Hide(this);
        }

        internal void RefreshDataState(bool invalidate = true)
        {
            _xAxisCategories.Clear();
            _yAxisCategories.Clear();
            _xAxisDateMin = DateTime.MinValue;
            _yAxisDateMin = DateTime.MinValue;
            ClearSimplificationCache();
            _isDataDirty = true; // force hit-area rebuild

            BeepChartDataHelper.DetectAxisTypes(this);
            BeepChartViewportHelper.AutoScaleViewport(this);

            if (invalidate)
            {
                InvalidateOrDefer();
            }
        }

        internal ViewportSnapshot GetViewportSnapshot()
        {
            return new ViewportSnapshot(ViewportXMin, ViewportXMax, ViewportYMin, ViewportYMax);
        }

        internal void RaiseViewportChangedIfNeeded(ViewportSnapshot oldViewport, string source)
        {
            if (!oldViewport.Equals(GetViewportSnapshot()))
            {
                // Clear simplification cache on viewport change for accuracy
                if (IsLargeDataset() && EnablePointCulling)
                {
                    ClearSimplificationCache();
                }

                ViewportChanged?.Invoke(this, new ChartViewportChangedEventArgs(
                    oldViewport.XMin,
                    oldViewport.XMax,
                    oldViewport.YMin,
                    oldViewport.YMax,
                    ViewportXMin,
                    ViewportXMax,
                    ViewportYMin,
                    ViewportYMax,
                    source));
            }
        }

        internal void CollectTrackballDataAtPosition(Point mousePosition)
        {
            _trackballDataPoints.Clear();
            var plotRect = ChartDrawingRect;
            
            if (!plotRect.Contains(mousePosition))
            {
                _trackballCrosshairX = -1f;
                return;
            }

            // Convert mouse X to data X coordinate
            float pixelX = mousePosition.X - plotRect.Left;
            float normalizedX = pixelX / plotRect.Width;
            float dataX = ViewportXMin + normalizedX * (ViewportXMax - ViewportXMin);
            
            _trackballCrosshairX = mousePosition.X;

            // Collect closest points from each visible series at this X coordinate
            foreach (var series in _dataSeries)
            {
                if (series.Visible && series.Points != null)
                {
                    var closestPoint = ChartInputHelper.FindClosestPointAtX(this, series, dataX);
                    if (closestPoint != null)
                    {
                        _trackballDataPoints.Add(new ChartTrackballDataPoint
                        {
                            SeriesName = series.Name,
                            SeriesColor = series.Color,
                            DataPoint = closestPoint,
                            XValue = ConvertXValue(closestPoint),
                            YValue = ConvertYValue(closestPoint),
                            DisplayValue = $"{closestPoint.Y:G4}"
                        });
                    }
                }
            }
        }

        public void SetSeriesFillPattern(int seriesIndex, ChartSeriesFillPattern pattern)
        {
            if (seriesIndex >= 0 && seriesIndex < _dataSeries.Count)
            {
                _seriesFillPatterns[seriesIndex] = pattern;
                Invalidate();
                NonColorStateCommunicationChanged?.Invoke(this, new ChartNonColorStateCommunicationChangedEventArgs(
                    seriesIndex,
                    pattern,
                    _dataSeries[seriesIndex].Name));
            }
        }

        public ChartSeriesFillPattern GetSeriesFillPattern(int seriesIndex)
        {
            if (_seriesFillPatterns.TryGetValue(seriesIndex, out var pattern))
                return pattern;
            return ChartSeriesFillPattern.Solid;
        }

        public void ApplyAutoPatterns()
        {
            // Assign different patterns to each series for automatic accessibility
            var patterns = new[]
            {
                ChartSeriesFillPattern.Solid,
                ChartSeriesFillPattern.Horizontal,
                ChartSeriesFillPattern.Vertical,
                ChartSeriesFillPattern.Diagonal,
                ChartSeriesFillPattern.BackDiagonal,
                ChartSeriesFillPattern.Cross,
                ChartSeriesFillPattern.DiagonalCross,
                ChartSeriesFillPattern.Dots
            };

            for (int i = 0; i < _dataSeries.Count; i++)
            {
                _seriesFillPatterns[i] = patterns[i % patterns.Length];
            }

            Invalidate();
        }

        public void ClearFillPatterns()
        {
            _seriesFillPatterns.Clear();
            Invalidate();
        }

        internal (int XLabelInterval, int YLabelInterval) GetRecommendedLabelIntervals()
        {
            if (!EnableDenseLabelOptimization || _dataSeries.Count == 0)
            {
                return (1, 1);
            }

            int totalPoints = GetTotalPointCount();
            if (totalPoints <= 0)
            {
                return (1, 1);
            }

            int visibleEstimate = 0;
            foreach (var series in _dataSeries)
            {
                if (!series.Visible || series.Points == null || series.Points.Count == 0)
                    continue;

                visibleEstimate += GetCulledPointsForViewport(series.Points).Count;
            }

            if (visibleEstimate <= 0)
            {
                visibleEstimate = totalPoints;
            }

            float viewportPercent = Math.Min(1f, Math.Max(0.01f, visibleEstimate / (float)totalPoints));
            int baseInterval = BeepChartPerformanceHelper.RecommendLabelInterval(totalPoints, viewportPercent);

            int xInterval = Math.Max(1, baseInterval);
            int yInterval = Math.Max(1, baseInterval / 2);

            // Clamp by target max axis labels budget.
            int maxLabels = Math.Max(3, MaxVisibleAxisLabels);
            xInterval = Math.Max(xInterval, (int)Math.Ceiling(visibleEstimate / (float)maxLabels));
            yInterval = Math.Max(yInterval, (int)Math.Ceiling(visibleEstimate / (float)(maxLabels + 2)));

            if (xInterval != _lastAppliedXLabelInterval || yInterval != _lastAppliedYLabelInterval)
            {
                _lastAppliedXLabelInterval = xInterval;
                _lastAppliedYLabelInterval = yInterval;
                LabelOptimizationApplied?.Invoke(this, new ChartLabelOptimizationAppliedEventArgs(
                    xInterval,
                    yInterval,
                    totalPoints,
                    visibleEstimate));
            }

            return (xInterval, yInterval);
        }

        private void RequestIncrementalRender(bool force = false)
        {
            if (!EnableRealTimeStreaming)
            {
                RefreshDataState(invalidate: true);
                return;
            }

            var now = DateTime.UtcNow;
            var elapsedMs = (now - _lastInvalidateTime).TotalMilliseconds;
            if (!force && elapsedMs < StreamRenderThrottleMs)
            {
                if (_streamRenderTimer != null)
                {
                    int remaining = Math.Max(1, StreamRenderThrottleMs - (int)Math.Floor(elapsedMs));
                    _streamRenderTimer.Interval = remaining;
                    _streamRenderTimer.Stop();
                    _streamRenderTimer.Start();
                }
                return;
            }

            _streamRenderTimer?.Stop();

            if (_streamRefreshPending)
            {
                // Keep streaming viewport stable while still updating axis typing/categorical state.
                BeepChartDataHelper.DetectAxisTypes(this);
                ClearSimplificationCache();
                _streamRefreshPending = false;
            }

            int renderedPending = _pendingStreamPointCount;
            _pendingStreamPointCount = 0;
            _lastInvalidateTime = now;
            Invalidate();

            IncrementalRenderCompleted?.Invoke(this, new ChartIncrementalRenderCompletedEventArgs(
                renderedPending,
                GetTotalPointCount(),
                StreamRenderThrottleMs));
        }

        private void TrimSeriesWindowIfNeeded(ChartDataSeries series)
        {
            if (series?.Points == null)
                return;

            int maxPoints = Math.Max(100, MaxStreamPointsPerSeries);
            int overflow = series.Points.Count - maxPoints;
            if (overflow > 0)
            {
                series.Points.RemoveRange(0, overflow);
            }
        }

        private void UpdateViewportForStreamingPoint(ChartDataPoint point)
        {
            if (!AutoScrollViewportOnStream || point == null)
                return;

            var xObj = ConvertXValue(point);
            var yObj = ConvertYValue(point);
            if (xObj is not float xVal || yObj is not float yVal)
                return;

            float xSpan = ViewportXMax - ViewportXMin;
            float ySpan = ViewportYMax - ViewportYMin;

            if (xSpan <= 0 || ySpan <= 0)
                return;

            if (xVal > ViewportXMax)
            {
                float shift = xVal - ViewportXMax;
                ViewportXMin += shift;
                ViewportXMax += shift;
            }

            if (yVal > ViewportYMax)
            {
                float padding = ySpan * 0.10f;
                ViewportYMax = yVal + padding;
            }
            else if (yVal < ViewportYMin)
            {
                float padding = ySpan * 0.10f;
                ViewportYMin = yVal - padding;
            }
        }
        #endregion

        #region Private Initialization Methods
        private void InitializeDefaultSettings()
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                _dataPointToolTip = new ToolTip();
                _animTimer = new System.Windows.Forms.Timer { Interval = 16 };
                _streamRenderTimer = new System.Windows.Forms.Timer { Interval = Math.Max(1, StreamRenderThrottleMs) };
                _streamRenderTimer.Tick += (s, e) =>
                {
                    _streamRenderTimer.Stop();
                    RequestIncrementalRender(force: true);
                };
            }
            BeepChartViewportHelper.UpdateChartDrawingRectBase(this);
            BeepChartViewportHelper.AutoScaleViewport(this);
            var t = BeepThemesManager.CurrentTheme;
            ChartBackColor         = t.ChartBackColor;
            ChartLineColor         = t.ChartLineColor;
            ChartFillColor         = t.ChartFillColor;
            ChartAxisColor         = t.ChartAxisColor;
            ChartTitleColor        = t.ChartTitleColor;
            ChartTitleForeColor    = t.ChartTitleColor;
            ChartTextColor         = t.ChartTextColor;
            ChartLegendBackColor   = t.ChartLegendBackColor;
            ChartLegendTextColor   = t.ChartLegendTextColor;
            ChartLegendShapeColor  = t.ChartLegendShapeColor;
            ChartGridLineColor     = t.ChartGridLineColor;
            ChartDefaultSeriesColors = new List<Color>(t.ChartDefaultSeriesColors);
            ChartTitleFont    = BeepThemesManager.ToFont(t.ChartTitleFont   ?? t.TitleStyle) ?? SystemFonts.DefaultFont;
            ChartValueFont    = BeepThemesManager.ToFont(t.ChartSubTitleFont ?? t.GetBlockHeaderFont()) ?? SystemFonts.DefaultFont;
            ChartSubtitleFont = BeepThemesManager.ToFont(t.GetBlockTextFont()) ?? SystemFonts.DefaultFont;
        }

        private void InitializeDesignTimeSampleData()
        {
            // DesignMode property is always false inside a constructor,
            // so use LicenseManager.UsageMode which works at construction time.
            bool isDesignTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime
                                || DesignMode
                                || (Site != null && Site.DesignMode);
            if (isDesignTime)
            {
                _dataSeries = new List<ChartDataSeries>
                {
                    new ChartDataSeries
                    {
                        Name = "Sample Series",
                        ChartType = ChartType.Line,
                        ShowLine = true,
                        ShowPoint = true,
                        Visible = true,
                        Points = new List<ChartDataPoint>
                        {
                            new ChartDataPoint("1", "5", 10f, "A", Color.Red),
                            new ChartDataPoint("2", "15", 15f, "B", Color.Green),
                            new ChartDataPoint("3", "8", 20f, "C", Color.Blue)
                        }
                    }
                };
                BeepChartViewportHelper.UpdateChartDrawingRectBase(this);
                RefreshDataState(invalidate: false);
            }
        }

        internal void InitializePainter()
        {
            switch (_surfaceStyle)
            {
                case ChartSurfaceStyle.Card:
                    _chartpainter = new CardChartPainter();
                    break;
                case ChartSurfaceStyle.Outline:
                    _chartpainter = new OutlineChartPainter();
                    break;
                case ChartSurfaceStyle.Glass:
                    _chartpainter = new GlassChartPainter2();
                    break;
                case ChartSurfaceStyle.Classic:
                default:
                    _chartpainter = new CardChartPainter();
                    break;
            }
            _chartpainter?.Initialize(this, _currentTheme);

            _axisPainter = new CartesianAxisPainter();

            // Series painter will be set dynamically based on chart type
            _lastSeriesType = ChartType;
            _seriesPainter = SeriesPainterFactory.GetPainter(_lastSeriesType);
            _seriesPainter.Initialize(this);
        }
        #endregion
    }

    internal readonly struct ViewportSnapshot
    {
        private const float Epsilon = 0.000001f;

        public float XMin { get; }
        public float XMax { get; }
        public float YMin { get; }
        public float YMax { get; }

        public ViewportSnapshot(float xMin, float xMax, float yMin, float yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
        }

        public bool Equals(ViewportSnapshot other)
        {
            return Math.Abs(XMin - other.XMin) < Epsilon &&
                   Math.Abs(XMax - other.XMax) < Epsilon &&
                   Math.Abs(YMin - other.YMin) < Epsilon &&
                   Math.Abs(YMax - other.YMax) < Epsilon;
        }
    }
}