using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(255,251,235);
            this.CalendarDaysHeaderForColor = Color.FromArgb(255,251,235);
            this.CalendarSelectedDateBackColor = Color.FromArgb(255,251,235);
            this.CalendarSelectedDateForColor = Color.FromArgb(255,251,235);
            this.CalendarBackColor = Color.FromArgb(255,251,235);
            this.CalendarForeColor = Color.FromArgb(33,37,41);
            this.CalendarTodayForeColor = Color.FromArgb(33,37,41);
            this.CalendarBorderColor = Color.FromArgb(247,208,136);
            this.CalendarHoverBackColor = Color.FromArgb(255,251,235);
            this.CalendarHoverForeColor = Color.FromArgb(33,37,41);
            this.CalendarFooterColor = Color.FromArgb(255,251,235);
        }
    }
}