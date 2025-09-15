using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal interface ICalendarViewRenderer
    {
        void Draw(Graphics g, CalendarRenderContext ctx);
        void HandleClick(Point location, CalendarRenderContext ctx);
    }
}
