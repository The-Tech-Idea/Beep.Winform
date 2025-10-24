using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(250,250,250);
            this.CalendarDaysHeaderForColor = Color.FromArgb(250,250,250);
            this.CalendarSelectedDateBackColor = Color.FromArgb(250,250,250);
            this.CalendarSelectedDateForColor = Color.FromArgb(250,250,250);
            this.CalendarBackColor = Color.FromArgb(250,250,250);
            this.CalendarForeColor = Color.FromArgb(20,20,20);
            this.CalendarTodayForeColor = Color.FromArgb(20,20,20);
            this.CalendarBorderColor = Color.FromArgb(0,0,0);
            this.CalendarHoverBackColor = Color.FromArgb(250,250,250);
            this.CalendarHoverForeColor = Color.FromArgb(20,20,20);
            this.CalendarFooterColor = Color.FromArgb(250,250,250);
        }
    }
}