using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(10,12,20);
            this.CalendarDaysHeaderForColor = Color.FromArgb(10,12,20);
            this.CalendarSelectedDateBackColor = Color.FromArgb(10,12,20);
            this.CalendarSelectedDateForColor = Color.FromArgb(10,12,20);
            this.CalendarBackColor = Color.FromArgb(10,12,20);
            this.CalendarForeColor = Color.FromArgb(235,245,255);
            this.CalendarTodayForeColor = Color.FromArgb(235,245,255);
            this.CalendarBorderColor = Color.FromArgb(60,70,100);
            this.CalendarHoverBackColor = Color.FromArgb(10,12,20);
            this.CalendarHoverForeColor = Color.FromArgb(235,245,255);
            this.CalendarFooterColor = Color.FromArgb(10,12,20);
        }
    }
}