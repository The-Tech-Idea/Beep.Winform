using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(40,44,52);
            this.CalendarDaysHeaderForColor = Color.FromArgb(40,44,52);
            this.CalendarSelectedDateBackColor = Color.FromArgb(40,44,52);
            this.CalendarSelectedDateForColor = Color.FromArgb(40,44,52);
            this.CalendarBackColor = Color.FromArgb(40,44,52);
            this.CalendarForeColor = Color.FromArgb(171,178,191);
            this.CalendarTodayForeColor = Color.FromArgb(171,178,191);
            this.CalendarBorderColor = Color.FromArgb(92,99,112);
            this.CalendarHoverBackColor = Color.FromArgb(40,44,52);
            this.CalendarHoverForeColor = Color.FromArgb(171,178,191);
            this.CalendarFooterColor = Color.FromArgb(40,44,52);
        }
    }
}