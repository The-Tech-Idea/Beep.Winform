using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private DateTime? GetTimedViewDateFromLocation(Point location)
        {
            if (_viewPainter == null) return null;
            return _viewPainter.GetDateTimeFromLocation(location, BuildViewPaintArgsForInteraction());
        }
    }
}
