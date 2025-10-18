using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(15,16,32);
            this.CalendarDaysHeaderForColor = Color.FromArgb(15,16,32);
            this.CalendarSelectedDateBackColor = Color.FromArgb(15,16,32);
            this.CalendarSelectedDateForColor = Color.FromArgb(15,16,32);
            this.CalendarBackColor = Color.FromArgb(15,16,32);
            this.CalendarForeColor = Color.FromArgb(245,247,255);
            this.CalendarTodayForeColor = Color.FromArgb(245,247,255);
            this.CalendarBorderColor = Color.FromArgb(74,79,123);
            this.CalendarHoverBackColor = Color.FromArgb(15,16,32);
            this.CalendarHoverForeColor = Color.FromArgb(245,247,255);
            this.CalendarFooterColor = Color.FromArgb(15,16,32);
        }
    }
}