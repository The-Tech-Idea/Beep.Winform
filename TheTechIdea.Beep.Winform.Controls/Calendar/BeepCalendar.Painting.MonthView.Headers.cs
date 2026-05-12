using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawMonthHeaders(Graphics g, Rectangle grid, int cellWidth, int dayHeaderHeight, CalendarPainterContext ctx)
        {
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                var headerRect = new Rectangle(grid.X + i * cellWidth, grid.Y, cellWidth, dayHeaderHeight);
                bool isToday = (int)DateTime.Today.DayOfWeek == i;
                _stylePainter.PaintDayHeader(g, headerRect, dayNames[i], isToday, ctx);
            }
        }
    }
}