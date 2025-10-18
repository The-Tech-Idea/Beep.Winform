using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(40,42,54);
            this.CalendarDaysHeaderForColor = Color.FromArgb(40,42,54);
            this.CalendarSelectedDateBackColor = Color.FromArgb(40,42,54);
            this.CalendarSelectedDateForColor = Color.FromArgb(40,42,54);
            this.CalendarBackColor = Color.FromArgb(40,42,54);
            this.CalendarForeColor = Color.FromArgb(248,248,242);
            this.CalendarTodayForeColor = Color.FromArgb(248,248,242);
            this.CalendarBorderColor = Color.FromArgb(98,114,164);
            this.CalendarHoverBackColor = Color.FromArgb(40,42,54);
            this.CalendarHoverForeColor = Color.FromArgb(248,248,242);
            this.CalendarFooterColor = Color.FromArgb(40,42,54);
        }
    }
}