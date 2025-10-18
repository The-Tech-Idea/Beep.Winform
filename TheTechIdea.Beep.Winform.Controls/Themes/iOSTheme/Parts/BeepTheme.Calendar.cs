using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(242,242,247);
            this.CalendarDaysHeaderForColor = Color.FromArgb(242,242,247);
            this.CalendarSelectedDateBackColor = Color.FromArgb(242,242,247);
            this.CalendarSelectedDateForColor = Color.FromArgb(242,242,247);
            this.CalendarBackColor = Color.FromArgb(242,242,247);
            this.CalendarForeColor = Color.FromArgb(28,28,30);
            this.CalendarTodayForeColor = Color.FromArgb(28,28,30);
            this.CalendarBorderColor = Color.FromArgb(198,198,207);
            this.CalendarHoverBackColor = Color.FromArgb(242,242,247);
            this.CalendarHoverForeColor = Color.FromArgb(28,28,30);
            this.CalendarFooterColor = Color.FromArgb(242,242,247);
        }
    }
}