using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(250,250,252);
            this.CalendarDaysHeaderForColor = Color.FromArgb(250,250,252);
            this.CalendarSelectedDateBackColor = Color.FromArgb(250,250,252);
            this.CalendarSelectedDateForColor = Color.FromArgb(250,250,252);
            this.CalendarBackColor = Color.FromArgb(250,250,252);
            this.CalendarForeColor = Color.FromArgb(28,28,30);
            this.CalendarTodayForeColor = Color.FromArgb(28,28,30);
            this.CalendarBorderColor = Color.FromArgb(229,229,234);
            this.CalendarHoverBackColor = Color.FromArgb(250,250,252);
            this.CalendarHoverForeColor = Color.FromArgb(28,28,30);
            this.CalendarFooterColor = Color.FromArgb(250,250,252);
        }
    }
}