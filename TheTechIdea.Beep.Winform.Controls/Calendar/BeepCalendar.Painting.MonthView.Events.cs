using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private int DrawMonthCellEvents(
            Graphics g,
            Rectangle cellRect,
            IEnumerable<CalendarEvent> dayEvents,
            int eventHeight,
            int eventSpacing,
            int eventStartOffset,
            CalendarPainterContext ctx)
        {
            int eventY = cellRect.Y + eventStartOffset;
            foreach (var evt in dayEvents.Take(CalendarLayoutMetrics.MaxEventsPerCell))
            {
                var eventRect = new Rectangle(cellRect.X + 2, eventY, cellRect.Width - 4, eventHeight);
                bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                bool isHovered = _hoveredEvent?.Id == evt.Id;
                _stylePainter.PaintEventBar(g, eventRect, evt, isSelected, isHovered, ctx);
                eventY += eventHeight + eventSpacing;
            }

            return eventY;
        }
    }
}