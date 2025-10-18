using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(26,27,38);
            this.CalendarDaysHeaderForColor = Color.FromArgb(26,27,38);
            this.CalendarSelectedDateBackColor = Color.FromArgb(26,27,38);
            this.CalendarSelectedDateForColor = Color.FromArgb(26,27,38);
            this.CalendarBackColor = Color.FromArgb(26,27,38);
            this.CalendarForeColor = Color.FromArgb(192,202,245);
            this.CalendarTodayForeColor = Color.FromArgb(192,202,245);
            this.CalendarBorderColor = Color.FromArgb(86,95,137);
            this.CalendarHoverBackColor = Color.FromArgb(26,27,38);
            this.CalendarHoverForeColor = Color.FromArgb(192,202,245);
            this.CalendarFooterColor = Color.FromArgb(26,27,38);
        }
    }
}