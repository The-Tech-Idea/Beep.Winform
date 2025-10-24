using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(243,242,241);
            this.CalendarDaysHeaderForColor = Color.FromArgb(243,242,241);
            this.CalendarSelectedDateBackColor = Color.FromArgb(243,242,241);
            this.CalendarSelectedDateForColor = Color.FromArgb(243,242,241);
            this.CalendarBackColor = Color.FromArgb(243,242,241);
            this.CalendarForeColor = Color.FromArgb(32,31,30);
            this.CalendarTodayForeColor = Color.FromArgb(32,31,30);
            this.CalendarBorderColor = Color.FromArgb(225,225,225);
            this.CalendarHoverBackColor = Color.FromArgb(243,242,241);
            this.CalendarHoverForeColor = Color.FromArgb(32,31,30);
            this.CalendarFooterColor = Color.FromArgb(243,242,241);
        }
    }
}