using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(255,255,255);
            this.CalendarDaysHeaderForColor = Color.FromArgb(255,255,255);
            this.CalendarSelectedDateBackColor = Color.FromArgb(255,255,255);
            this.CalendarSelectedDateForColor = Color.FromArgb(255,255,255);
            this.CalendarBackColor = Color.FromArgb(255,255,255);
            this.CalendarForeColor = Color.FromArgb(31,41,55);
            this.CalendarTodayForeColor = Color.FromArgb(31,41,55);
            this.CalendarBorderColor = Color.FromArgb(209,213,219);
            this.CalendarHoverBackColor = Color.FromArgb(255,255,255);
            this.CalendarHoverForeColor = Color.FromArgb(31,41,55);
            this.CalendarFooterColor = Color.FromArgb(255,255,255);
        }
    }
}