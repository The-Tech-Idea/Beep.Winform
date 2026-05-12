using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarEventResizeEdge ResolveResizeEdge(Point location, Rectangle eventRect)
        {
            if (eventRect.Height <= 0)
            {
                return CalendarEventResizeEdge.None;
            }

            if (location.Y <= eventRect.Top + ResizeHandleHitSizePx)
            {
                return CalendarEventResizeEdge.Start;
            }

            if (location.Y >= eventRect.Bottom - ResizeHandleHitSizePx)
            {
                return CalendarEventResizeEdge.End;
            }

            return CalendarEventResizeEdge.None;
        }

    }
}