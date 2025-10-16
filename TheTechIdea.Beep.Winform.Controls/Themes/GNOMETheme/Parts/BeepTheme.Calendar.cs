using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(246,245,244);
            this.CalendarDaysHeaderForColor = Color.FromArgb(246,245,244);
            this.CalendarSelectedDateBackColor = Color.FromArgb(246,245,244);
            this.CalendarSelectedDateForColor = Color.FromArgb(246,245,244);
            this.CalendarBackColor = Color.FromArgb(246,245,244);
            this.CalendarForeColor = Color.FromArgb(46,52,54);
            this.CalendarTodayForeColor = Color.FromArgb(46,52,54);
            this.CalendarBorderColor = Color.FromArgb(205,207,212);
            this.CalendarHoverBackColor = Color.FromArgb(246,245,244);
            this.CalendarHoverForeColor = Color.FromArgb(46,52,54);
            this.CalendarFooterColor = Color.FromArgb(246,245,244);
        }
    }
}