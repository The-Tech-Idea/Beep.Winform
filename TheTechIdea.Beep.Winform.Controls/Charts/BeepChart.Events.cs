using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    public partial class BeepChart
    {
        #region Events
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler<CustomDrawSeriesEventArgs> CustomDrawSeries;
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
                ChartInputHelper.HandleMouseDown(ref _lastMouseDownPoint, e);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                var newHoveredPoint = ChartInputHelper.HandleMouseMove(this, e, ref _lastMouseDownPoint, ref _lastInvalidateTime, _zoomFactor);
                if (newHoveredPoint != _hoveredPoint)
                {
                    _hoveredPoint = newHoveredPoint;
                    if ((DateTime.Now - _lastInvalidateTime).TotalMilliseconds > 50)
                    {
                        Invalidate();
                        _lastInvalidateTime = DateTime.Now;
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                ChartInputHelper.HandleMouseUp(this, ref _lastMouseDownPoint);
            }
            base.OnMouseUp(e);
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