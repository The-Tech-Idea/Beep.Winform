using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(245,246,247);
            this.CalendarDaysHeaderForColor = Color.FromArgb(245,246,247);
            this.CalendarSelectedDateBackColor = Color.FromArgb(245,246,247);
            this.CalendarSelectedDateForColor = Color.FromArgb(245,246,247);
            this.CalendarBackColor = Color.FromArgb(245,246,247);
            this.CalendarForeColor = Color.FromArgb(43,45,48);
            this.CalendarTodayForeColor = Color.FromArgb(43,45,48);
            this.CalendarBorderColor = Color.FromArgb(220,223,230);
            this.CalendarHoverBackColor = Color.FromArgb(245,246,247);
            this.CalendarHoverForeColor = Color.FromArgb(43,45,48);
            this.CalendarFooterColor = Color.FromArgb(245,246,247);
        }
    }
}