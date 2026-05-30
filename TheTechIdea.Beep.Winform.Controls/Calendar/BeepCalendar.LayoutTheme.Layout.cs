using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public new void UpdateLayout()
        {
            base.UpdateLayout();

            if (!_controlsInitialized || _layout == null)
                return;

            Rectangle contentRect = GetCalendarLayoutRect();
            if (contentRect.Width <= 0 || contentRect.Height <= 0)
                return;

            ApplyResponsiveButtonLabels(contentRect.Width);

            int toolbarHeight = ScaleMetric(32);
            int sidebarWidth = GetResponsiveSidebarWidth(contentRect.Width);
            _layout.UpdateLayout(contentRect, toolbarHeight, sidebarWidth, ScaleMetric(Math.Max(0, GridLeftGutter)), GetMetricScale());

            LayoutToolbar(_rects.HeaderRect, _rects.ViewSelectorRect);
            Invalidate();
        }

        private Rectangle GetCalendarLayoutRect()
        {
            Rectangle rect = GetContentRectForDrawing();
            if (rect.Width <= 0 || rect.Height <= 0)
                rect = ClientRectangle;

            int margin = ScaleMetric(CalendarLayoutMetrics.OuterMargin);
            if (rect.Width > margin * 2 && rect.Height > margin * 2)
                rect.Inflate(-margin, -margin);

            return rect;
        }

        private int GetResponsiveSidebarWidth(int availableWidth)
        {
            if (!_state.ShowSidebar) return 0;

            int preferred = ScaleMetric(CalendarLayoutMetrics.SidebarWidth);
            int minimum = ScaleMetric(220);
            int width = Math.Min(preferred, Math.Max(0, availableWidth / 3));
            return width >= minimum ? width : 0;
        }

        private void UpdateViewButtonStates()
        {
            // Painted toolbar handles active states via PaintToolbar() - no-op for backward compat
        }
    }
}
