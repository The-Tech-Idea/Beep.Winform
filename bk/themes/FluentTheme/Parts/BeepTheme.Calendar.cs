using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(245,246,248);
            this.CalendarDaysHeaderForColor = Color.FromArgb(245,246,248);
            this.CalendarSelectedDateBackColor = Color.FromArgb(245,246,248);
            this.CalendarSelectedDateForColor = Color.FromArgb(245,246,248);
            this.CalendarBackColor = Color.FromArgb(245,246,248);
            this.CalendarForeColor = Color.FromArgb(32,32,32);
            this.CalendarTodayForeColor = Color.FromArgb(32,32,32);
            this.CalendarBorderColor = Color.FromArgb(218,223,230);
            this.CalendarHoverBackColor = Color.FromArgb(245,246,248);
            this.CalendarHoverForeColor = Color.FromArgb(32,32,32);
            this.CalendarFooterColor = Color.FromArgb(245,246,248);
        }
    }
}