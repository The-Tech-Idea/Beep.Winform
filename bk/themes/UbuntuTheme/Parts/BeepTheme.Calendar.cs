using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(242,242,245);
            this.CalendarDaysHeaderForColor = Color.FromArgb(242,242,245);
            this.CalendarSelectedDateBackColor = Color.FromArgb(242,242,245);
            this.CalendarSelectedDateForColor = Color.FromArgb(242,242,245);
            this.CalendarBackColor = Color.FromArgb(242,242,245);
            this.CalendarForeColor = Color.FromArgb(44,44,44);
            this.CalendarTodayForeColor = Color.FromArgb(44,44,44);
            this.CalendarBorderColor = Color.FromArgb(218,218,222);
            this.CalendarHoverBackColor = Color.FromArgb(242,242,245);
            this.CalendarHoverForeColor = Color.FromArgb(44,44,44);
            this.CalendarFooterColor = Color.FromArgb(242,242,245);
        }
    }
}