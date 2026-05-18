using System;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawListViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            var monthEvents = _eventService.GetEventsForMonth(_state.CurrentDate)
                .OrderBy(e => e.StartTime)
                .ToList();
            int rowHeight = ScaleMetric(CalendarLayoutMetrics.ListRowHeight);
            int rowSpacing = ScaleMetric(CalendarLayoutMetrics.ListRowSpacing);
            int padding = ScaleMetric(CalendarLayoutMetrics.SidebarPadding);

            int yPos = grid.Y + padding;
            foreach (var evt in monthEvents)
            {
                if (yPos + rowHeight > grid.Bottom) break;

                var eventRect = new Rectangle(grid.X + padding, yPos, Math.Max(1, grid.Width - (padding * 2)), rowHeight);
                bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                bool isHovered = _hoveredEvent?.Id == evt.Id;
                _stylePainter.PaintListViewEvent(g, eventRect, evt, isSelected, isHovered, ctx);
                yPos += rowHeight + rowSpacing;
            }
        }
    }
}
