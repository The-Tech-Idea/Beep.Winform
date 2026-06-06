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

            int toolbarHeight = ScaleMetric(32);
            int sidebarWidth = GetResponsiveSidebarWidth(contentRect.Width);
            _layout.UpdateLayout(contentRect, toolbarHeight, sidebarWidth, ScaleMetric(Math.Max(0, GridLeftGutter)), GetMetricScale(), _viewPainter);

            // Build the immutable surface model for this layout. Painters and
            // hit-test helpers consume this snapshot rather than recomputing
            // geometry on every call.
            var metrics = CalendarStyleMetrics.For(_calendarStyle);
            _surfaceModel = CalendarSurfaceModel.Build(
                _state, _rects,
                ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight),
                ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth),
                ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight),
                ScaleMetric(CalendarLayoutMetrics.EventInsetX),
                ScaleMetric(CalendarLayoutMetrics.EventInsetY),
                ScaleMetric(CalendarLayoutMetrics.MinEventHitHeight),
                ScaleMetric(16),                     // event bar height
                ScaleMetric(2),                      // event spacing
                ScaleMetric(CalendarLayoutMetrics.SidebarPadding),
                ScaleMetric(CalendarLayoutMetrics.SidebarCardHeight),
                ScaleMetric(CalendarLayoutMetrics.SidebarCardGap),
                ScaleMetric(CalendarLayoutMetrics.ListRowHeight),
                ScaleMetric(CalendarLayoutMetrics.ListRowSpacing),
                ScaleMetric(metrics.CornerRadius),
                ScaleMetric(metrics.CellPadding),
                CalendarLayoutMetrics.MaxEventsPerCell,
                Math.Max(0, HeaderLeftPadding),
                CalendarLayoutMetrics.HeaderRightPadding,
                _viewPainter);

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
