using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawWeekSlotEvents(
            Graphics g,
            Rectangle slotRect,
            int yPos,
            int slotHeight,
            IEnumerable<CalendarEvent> dayEvents,
            int eventInsetX,
            int eventInsetY,
            CalendarPainterContext ctx)
        {
            foreach (var evt in dayEvents)
            {
                var eventRect = new Rectangle(
                    slotRect.X + eventInsetX,
                    yPos + eventInsetY,
                    Math.Max(20, slotRect.Width - (eventInsetX * 2)),
                    Math.Max(CalendarLayoutMetrics.MinEventHitHeight, (int)(evt.Duration.TotalHours * slotHeight) - (eventInsetY * 2)));
                bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                bool isHovered = _hoveredEvent?.Id == evt.Id;
                _stylePainter.PaintEventBlock(g, eventRect, evt, isSelected, isHovered, ctx);
            }
        }
    }
}