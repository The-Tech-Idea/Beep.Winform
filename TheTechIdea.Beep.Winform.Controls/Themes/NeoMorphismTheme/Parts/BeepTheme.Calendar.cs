using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(236,240,243);
            this.CalendarDaysHeaderForColor = Color.FromArgb(236,240,243);
            this.CalendarSelectedDateBackColor = Color.FromArgb(236,240,243);
            this.CalendarSelectedDateForColor = Color.FromArgb(236,240,243);
            this.CalendarBackColor = Color.FromArgb(236,240,243);
            this.CalendarForeColor = Color.FromArgb(58,66,86);
            this.CalendarTodayForeColor = Color.FromArgb(58,66,86);
            this.CalendarBorderColor = Color.FromArgb(221,228,235);
            this.CalendarHoverBackColor = Color.FromArgb(236,240,243);
            this.CalendarHoverForeColor = Color.FromArgb(58,66,86);
            this.CalendarFooterColor = Color.FromArgb(236,240,243);
        }
    }
}