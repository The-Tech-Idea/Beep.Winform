using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(0,43,54);
            this.CalendarDaysHeaderForColor = Color.FromArgb(0,43,54);
            this.CalendarSelectedDateBackColor = Color.FromArgb(0,43,54);
            this.CalendarSelectedDateForColor = Color.FromArgb(0,43,54);
            this.CalendarBackColor = Color.FromArgb(0,43,54);
            this.CalendarForeColor = Color.FromArgb(147,161,161);
            this.CalendarTodayForeColor = Color.FromArgb(147,161,161);
            this.CalendarBorderColor = Color.FromArgb(88,110,117);
            this.CalendarHoverBackColor = Color.FromArgb(0,43,54);
            this.CalendarHoverForeColor = Color.FromArgb(147,161,161);
            this.CalendarFooterColor = Color.FromArgb(0,43,54);
        }
    }
}