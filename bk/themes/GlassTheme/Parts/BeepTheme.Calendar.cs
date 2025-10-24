using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(236,244,255);
            this.CalendarDaysHeaderForColor = Color.FromArgb(236,244,255);
            this.CalendarSelectedDateBackColor = Color.FromArgb(236,244,255);
            this.CalendarSelectedDateForColor = Color.FromArgb(236,244,255);
            this.CalendarBackColor = Color.FromArgb(236,244,255);
            this.CalendarForeColor = Color.FromArgb(17,24,39);
            this.CalendarTodayForeColor = Color.FromArgb(17,24,39);
            this.CalendarBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.CalendarHoverBackColor = Color.FromArgb(236,244,255);
            this.CalendarHoverForeColor = Color.FromArgb(17,24,39);
            this.CalendarFooterColor = Color.FromArgb(236,244,255);
        }
    }
}