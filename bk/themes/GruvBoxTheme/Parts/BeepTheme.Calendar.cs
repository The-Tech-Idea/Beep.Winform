using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(40,40,40);
            this.CalendarDaysHeaderForColor = Color.FromArgb(40,40,40);
            this.CalendarSelectedDateBackColor = Color.FromArgb(40,40,40);
            this.CalendarSelectedDateForColor = Color.FromArgb(40,40,40);
            this.CalendarBackColor = Color.FromArgb(40,40,40);
            this.CalendarForeColor = Color.FromArgb(235,219,178);
            this.CalendarTodayForeColor = Color.FromArgb(235,219,178);
            this.CalendarBorderColor = Color.FromArgb(168,153,132);
            this.CalendarHoverBackColor = Color.FromArgb(40,40,40);
            this.CalendarHoverForeColor = Color.FromArgb(235,219,178);
            this.CalendarFooterColor = Color.FromArgb(40,40,40);
        }
    }
}