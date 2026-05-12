using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        #region Events
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<CustomDrawSeriesEventArgs> CustomDrawSeries;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartInteractiveAreaEventArgs> InteractiveAreaHit;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartPointHitEventArgs> PointHit;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartAxisHitEventArgs> AxisHit;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartLegendHitEventArgs> LegendHit;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartPointHoverChangedEventArgs> PointHoverChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartViewportChangedEventArgs> ViewportChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartTrackballDataCollectedEventArgs> TrackballDataCollected;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartSeriesIsolationChangedEventArgs> SeriesIsolationChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartSelectionChangedEventArgs> SelectionChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartContextMenuRequestedEventArgs> ContextMenuRequested;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartKeyboardFocusChangedEventArgs> KeyboardFocusChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartPerformanceOptimizedEventArgs> PerformanceOptimized;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartNonColorStateCommunicationChangedEventArgs> NonColorStateCommunicationChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartLabelOptimizationAppliedEventArgs> LabelOptimizationApplied;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartStreamingDataAppendedEventArgs> StreamingDataAppended;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<ChartIncrementalRenderCompletedEventArgs> IncrementalRenderCompleted;
        #endregion

        #region Override Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _isDrawingRectCalculated = false;
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                BeepChartViewportHelper.AutoScaleViewport(this);
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
                    BeepChartViewportHelper.UpdateChartDrawingRectBase(this);
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

  

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && EnableMouseWheelZoom)
            {
                var oldViewport = GetViewportSnapshot();
                float zoomStep = Math.Clamp(MouseWheelZoomStepPercent, 0.01f, 0.90f);
                ChartInputHelper.HandleMouseWheel(this, e, zoomStep);
                RaiseViewportChangedIfNeeded(oldViewport, "MouseWheel");
            }
            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                ChartInputHelper.HandleMouseDown(ref _lastMouseDownPoint, e);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                var oldViewport = GetViewportSnapshot();
                float panFactor = Math.Clamp(MouseDragPanFactor, 0.10f, 5.0f);
                var newHoveredPoint = ChartInputHelper.HandleMouseMove(
                    this,
                    e,
                    ref _lastMouseDownPoint,
                    ref _lastInvalidateTime,
                    panFactor,
                    EnableMouseDragPan);
                RaiseViewportChangedIfNeeded(oldViewport, "MouseDragPan");

                // Trackball mode: collect all series values at current X position
                if (EnableTrackball && TrackballShowMultiSeriesValues)
                {
                    CollectTrackballDataAtPosition(e.Location);
                    TrackballDataCollected?.Invoke(this, new ChartTrackballDataCollectedEventArgs(_trackballDataPoints, _trackballCrosshairX));
                }

                if (newHoveredPoint != _hoveredPoint)
                {
                    _hoveredPoint = newHoveredPoint;
                    PointHoverChanged?.Invoke(this, new ChartPointHoverChangedEventArgs(_hoveredPoint));
                    if (_hoveredPoint != null)
                    {
                        ShowTooltip(_hoveredPoint);
                    }
                    else
                    {
                        HideTooltip();
                    }

                    if ((DateTime.Now - _lastInvalidateTime).TotalMilliseconds > 50)
                    {
                        Invalidate();
                        _lastInvalidateTime = DateTime.Now;
                    }
                }
                else if (EnableTrackball && (DateTime.Now - _lastInvalidateTime).TotalMilliseconds > 50)
                {
                    Invalidate();
                    _lastInvalidateTime = DateTime.Now;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                // Handle right-click context menu
                if (e.Button == MouseButtons.Right && EnableContextMenu)
                {
                    _lastRightClickLocation = e.Location;
                    HandleContextMenuRequest(e.Location);
                }
                else
                {
                    ChartInputHelper.HandleMouseUp(this, ref _lastMouseDownPoint);
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && e.Button == MouseButtons.Left)
            {
                ResetViewport();
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                _hoveredPoint = null;
                PointHoverChanged?.Invoke(this, new ChartPointHoverChangedEventArgs(null));
                HideTooltip();
                
                // Clear trackball state
                _trackballCrosshairX = -1f;
                _trackballDataPoints.Clear();
                Invalidate();
            }

            base.OnMouseLeave(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // Handle keyboard point selection/navigation
            if (EnableKeyboardNavigation)
            {
                var keyCode = keyData & Keys.KeyCode;
                bool isCtrlHeld = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool isShiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

                switch (keyCode)
                {
                    case Keys.Tab:
                        HandleKeyboardSeriesCycling(!isShiftHeld);
                        return true;

                    case Keys.Left:
                    case Keys.Up:
                        if (isCtrlHeld || isShiftHeld)
                            break; // Fall through to viewport navigation
                        HandleKeyboardPointNavigation(previous: true);
                        return true;

                    case Keys.Right:
                    case Keys.Down:
                        if (isCtrlHeld || isShiftHeld)
                            break; // Fall through to viewport navigation
                        HandleKeyboardPointNavigation(previous: false);
                        return true;

                    case Keys.Return:
                    case Keys.Space:
                        HandleKeyboardPointSelection(isCtrlHeld);
                        return true;

                    case Keys.A:
                        if (isCtrlHeld)
                        {
                            HandleKeyboardSelectAll();
                            return true;
                        }
                        break;
                }
            }

            // Handle keyboard viewport navigation
            if (!EnableKeyboardViewportNavigation)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            float panStep = Math.Clamp(KeyboardPanStepPercent, 0.01f, 0.50f);
            float zoomStep = Math.Clamp(KeyboardZoomStepPercent, 0.01f, 0.90f);

            var keyCode2 = keyData & Keys.KeyCode;
            switch (keyCode2)
            {
                case Keys.R:
                    ResetViewport();
                    return true;

                case Keys.Escape:
                    if (HasSelection)
                    {
                        ClearSelection();
                        return true;
                    }
                    break;

                case Keys.Add:
                case Keys.Oemplus:
                {
                    var oldViewport = GetViewportSnapshot();
                    ChartInputHelper.HandleKeyboardZoom(this, zoomIn: true, stepPercent: zoomStep);
                    RaiseViewportChangedIfNeeded(oldViewport, "KeyboardZoomIn");
                    return true;
                }

                case Keys.Subtract:
                case Keys.OemMinus:
                {
                    var oldViewport = GetViewportSnapshot();
                    ChartInputHelper.HandleKeyboardZoom(this, zoomIn: false, stepPercent: zoomStep);
                    RaiseViewportChangedIfNeeded(oldViewport, "KeyboardZoomOut");
                    return true;
                }

                case Keys.Left:
                {
                    var oldViewport = GetViewportSnapshot();
                    ChartInputHelper.HandleKeyboardPan(this, horizontalPercent: -panStep, verticalPercent: 0f);
                    RaiseViewportChangedIfNeeded(oldViewport, "KeyboardPanLeft");
                    return true;
                }

                case Keys.Right:
                {
                    var oldViewport = GetViewportSnapshot();
                    ChartInputHelper.HandleKeyboardPan(this, horizontalPercent: panStep, verticalPercent: 0f);
                    RaiseViewportChangedIfNeeded(oldViewport, "KeyboardPanRight");
                    return true;
                }

                case Keys.Up:
                {
                    var oldViewport = GetViewportSnapshot();
                    ChartInputHelper.HandleKeyboardPan(this, horizontalPercent: 0f, verticalPercent: -panStep);
                    RaiseViewportChangedIfNeeded(oldViewport, "KeyboardPanUp");
                    return true;
                }

                case Keys.Down:
                {
                    var oldViewport = GetViewportSnapshot();
                    ChartInputHelper.HandleKeyboardPan(this, horizontalPercent: 0f, verticalPercent: panStep);
                    RaiseViewportChangedIfNeeded(oldViewport, "KeyboardPanDown");
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
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

    public sealed class ChartInteractiveAreaEventArgs : EventArgs
    {
        public string AreaName { get; }
        public Rectangle Bounds { get; }
        public string AreaType { get; }
        public int? SeriesIndex { get; }
        public int? PointIndex { get; }
        public int? LegendItemIndex { get; }
        public ChartDataPoint DataPoint { get; }

        public ChartInteractiveAreaEventArgs(string areaName, Rectangle bounds)
            : this(areaName, bounds, areaName, null, null, null, null)
        {
        }

        public ChartInteractiveAreaEventArgs(
            string areaName,
            Rectangle bounds,
            string areaType,
            int? seriesIndex,
            int? pointIndex,
            int? legendItemIndex,
            ChartDataPoint dataPoint)
        {
            AreaName = areaName;
            Bounds = bounds;
            AreaType = areaType;
            SeriesIndex = seriesIndex;
            PointIndex = pointIndex;
            LegendItemIndex = legendItemIndex;
            DataPoint = dataPoint;
        }
    }

    public sealed class ChartPointHitEventArgs : EventArgs
    {
        public string AreaName { get; }
        public string AreaType { get; }
        public int SeriesIndex { get; }
        public int PointIndex { get; }
        public Rectangle Bounds { get; }
        public ChartDataPoint DataPoint { get; }

        public ChartPointHitEventArgs(
            string areaName,
            string areaType,
            int seriesIndex,
            int pointIndex,
            Rectangle bounds,
            ChartDataPoint dataPoint)
        {
            AreaName = areaName;
            AreaType = areaType;
            SeriesIndex = seriesIndex;
            PointIndex = pointIndex;
            Bounds = bounds;
            DataPoint = dataPoint;
        }
    }

    public sealed class ChartAxisHitEventArgs : EventArgs
    {
        public string AreaName { get; }
        public string AxisType { get; }
        public Rectangle Bounds { get; }

        public ChartAxisHitEventArgs(string areaName, string axisType, Rectangle bounds)
        {
            AreaName = areaName;
            AxisType = axisType;
            Bounds = bounds;
        }
    }

    public sealed class ChartLegendHitEventArgs : EventArgs
    {
        public string AreaName { get; }
        public bool IsLegendItem { get; }
        public int? LegendItemIndex { get; }
        public Rectangle Bounds { get; }

        public ChartLegendHitEventArgs(string areaName, bool isLegendItem, int? legendItemIndex, Rectangle bounds)
        {
            AreaName = areaName;
            IsLegendItem = isLegendItem;
            LegendItemIndex = legendItemIndex;
            Bounds = bounds;
        }
    }

    public sealed class ChartPointHoverChangedEventArgs : EventArgs
    {
        public ChartDataPoint DataPoint { get; }
        public bool HasPoint => DataPoint != null;

        public ChartPointHoverChangedEventArgs(ChartDataPoint dataPoint)
        {
            DataPoint = dataPoint;
        }
    }

    public sealed class ChartViewportChangedEventArgs : EventArgs
    {
        public float OldXMin { get; }
        public float OldXMax { get; }
        public float OldYMin { get; }
        public float OldYMax { get; }
        public float NewXMin { get; }
        public float NewXMax { get; }
        public float NewYMin { get; }
        public float NewYMax { get; }
        public string Source { get; }

        public ChartViewportChangedEventArgs(
            float oldXMin,
            float oldXMax,
            float oldYMin,
            float oldYMax,
            float newXMin,
            float newXMax,
            float newYMin,
            float newYMax,
            string source)
        {
            OldXMin = oldXMin;
            OldXMax = oldXMax;
            OldYMin = oldYMin;
            OldYMax = oldYMax;
            NewXMin = newXMin;
            NewXMax = newXMax;
            NewYMin = newYMin;
            NewYMax = newYMax;
            Source = source;
        }
    }

    public sealed class ChartTrackballDataCollectedEventArgs : EventArgs
    {
        public List<ChartTrackballDataPoint> DataPoints { get; }
        public float CrosshairX { get; }
        public bool IsActive => CrosshairX >= 0;

        public ChartTrackballDataCollectedEventArgs(List<ChartTrackballDataPoint> dataPoints, float crosshairX)
        {
            DataPoints = dataPoints ?? new List<ChartTrackballDataPoint>();
            CrosshairX = crosshairX;
        }
    }

    public sealed class ChartTrackballDataPoint
    {
        public string SeriesName { get; set; }
        public Color SeriesColor { get; set; }
        public ChartDataPoint DataPoint { get; set; }
        public object XValue { get; set; }
        public object YValue { get; set; }
        public string DisplayValue { get; set; }
    }

    public sealed class ChartSeriesIsolationChangedEventArgs : EventArgs
    {
        public int SeriesIndex { get; }
        public bool IsIsolated { get; }
        public string SeriesName { get; }

        public ChartSeriesIsolationChangedEventArgs(int seriesIndex, bool isIsolated, string seriesName)
        {
            SeriesIndex = seriesIndex;
            IsIsolated = isIsolated;
            SeriesName = seriesName;
        }
    }

    public sealed class ChartSelectionChangedEventArgs : EventArgs
    {
        public List<(int, int)> SelectedPoints { get; }
        public List<int> SelectedSeries { get; }
        public int LastSelectedSeriesIndex { get; }
        public int LastSelectedPointIndex { get; }

        public ChartSelectionChangedEventArgs(List<(int, int)> selectedPoints, List<int> selectedSeries, int lastSeriesIndex, int lastPointIndex)
        {
            SelectedPoints = selectedPoints ?? new List<(int, int)>();
            SelectedSeries = selectedSeries ?? new List<int>();
            LastSelectedSeriesIndex = lastSeriesIndex;
            LastSelectedPointIndex = lastPointIndex;
        }
    }

    public sealed class ChartContextMenuRequestedEventArgs : EventArgs
    {
        public Point Location { get; }
        public List<(int SeriesIndex, int PointIndex)> SelectedPoints { get; }
        public List<int> SelectedSeries { get; }
        public ContextMenuStrip Menu { get; set; }

        public ChartContextMenuRequestedEventArgs(Point location, List<(int, int)> selectedPoints, List<int> selectedSeries)
        {
            Location = location;
            SelectedPoints = selectedPoints ?? new List<(int, int)>();
            SelectedSeries = selectedSeries ?? new List<int>();
            Menu = null;
        }
    }

    public sealed class ChartKeyboardFocusChangedEventArgs : EventArgs
    {
        public int SeriesIndex { get; }
        public int PointIndex { get; }
        public string SeriesName { get; }
        public ChartDataPoint FocusedPoint { get; }

        public ChartKeyboardFocusChangedEventArgs(int seriesIndex, int pointIndex, string seriesName, ChartDataPoint focusedPoint)
        {
            SeriesIndex = seriesIndex;
            PointIndex = pointIndex;
            SeriesName = seriesName;
            FocusedPoint = focusedPoint;
        }
    }

    public sealed class ChartPerformanceOptimizedEventArgs : EventArgs
    {
        public int TotalPointCount { get; }
        public int DecimationFactor { get; }
        public float RenderingCost { get; }
        public int CacheSize { get; }

        public ChartPerformanceOptimizedEventArgs(int totalPoints, int decimationFactor, float renderingCost, int cacheSize)
        {
            TotalPointCount = totalPoints;
            DecimationFactor = decimationFactor;
            RenderingCost = renderingCost;
            CacheSize = cacheSize;
        }
    }

    public sealed class ChartNonColorStateCommunicationChangedEventArgs : EventArgs
    {
        public int SeriesIndex { get; }
        public ChartSeriesFillPattern Pattern { get; }
        public string SeriesName { get; }

        public ChartNonColorStateCommunicationChangedEventArgs(int seriesIndex, ChartSeriesFillPattern pattern, string seriesName)
        {
            SeriesIndex = seriesIndex;
            Pattern = pattern;
            SeriesName = seriesName;
        }
    }

    public sealed class ChartLabelOptimizationAppliedEventArgs : EventArgs
    {
        public int XLabelInterval { get; }
        public int YLabelInterval { get; }
        public int TotalPointCount { get; }
        public int VisiblePointEstimate { get; }

        public ChartLabelOptimizationAppliedEventArgs(int xLabelInterval, int yLabelInterval, int totalPointCount, int visiblePointEstimate)
        {
            XLabelInterval = xLabelInterval;
            YLabelInterval = yLabelInterval;
            TotalPointCount = totalPointCount;
            VisiblePointEstimate = visiblePointEstimate;
        }
    }

    public sealed class ChartStreamingDataAppendedEventArgs : EventArgs
    {
        public int SeriesIndex { get; }
        public string SeriesName { get; }
        public int AddedPointCount { get; }
        public int SeriesPointCount { get; }
        public int TotalPointCount { get; }

        public ChartStreamingDataAppendedEventArgs(int seriesIndex, string seriesName, int addedPointCount, int seriesPointCount, int totalPointCount)
        {
            SeriesIndex = seriesIndex;
            SeriesName = seriesName;
            AddedPointCount = addedPointCount;
            SeriesPointCount = seriesPointCount;
            TotalPointCount = totalPointCount;
        }
    }

    public sealed class ChartIncrementalRenderCompletedEventArgs : EventArgs
    {
        public int RenderedPendingPointCount { get; }
        public int TotalPointCount { get; }
        public int ThrottleMs { get; }

        public ChartIncrementalRenderCompletedEventArgs(int renderedPendingPointCount, int totalPointCount, int throttleMs)
        {
            RenderedPendingPointCount = renderedPendingPointCount;
            TotalPointCount = totalPointCount;
            ThrottleMs = throttleMs;
        }
    }
}