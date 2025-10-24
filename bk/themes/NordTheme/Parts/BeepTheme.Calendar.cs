using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(46,52,64);
            this.CalendarDaysHeaderForColor = Color.FromArgb(46,52,64);
            this.CalendarSelectedDateBackColor = Color.FromArgb(46,52,64);
            this.CalendarSelectedDateForColor = Color.FromArgb(46,52,64);
            this.CalendarBackColor = Color.FromArgb(46,52,64);
            this.CalendarForeColor = Color.FromArgb(216,222,233);
            this.CalendarTodayForeColor = Color.FromArgb(216,222,233);
            this.CalendarBorderColor = Color.FromArgb(76,86,106);
            this.CalendarHoverBackColor = Color.FromArgb(46,52,64);
            this.CalendarHoverForeColor = Color.FromArgb(216,222,233);
            this.CalendarFooterColor = Color.FromArgb(46,52,64);
        }
    }
}