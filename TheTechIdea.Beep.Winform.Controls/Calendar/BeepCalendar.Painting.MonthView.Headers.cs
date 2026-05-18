using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawMonthHeaders(Graphics g, Rectangle headerBand, CalendarPainterContext ctx)
        {
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                var headerRect = CalendarLayoutGeometry.GetColumnRect(headerBand, i, 7);
                bool isToday = (int)DateTime.Today.DayOfWeek == i;
                _stylePainter.PaintDayHeader(g, headerRect, dayNames[i], isToday, ctx);
            }
        }
    }
}
