using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = Color.FromArgb(250,250,251);
            this.CalendarDaysHeaderForColor = Color.FromArgb(250,250,251);
            this.CalendarSelectedDateBackColor = Color.FromArgb(250,250,251);
            this.CalendarSelectedDateForColor = Color.FromArgb(250,250,251);
            this.CalendarBackColor = Color.FromArgb(250,250,251);
            this.CalendarForeColor = Color.FromArgb(31,41,55);
            this.CalendarTodayForeColor = Color.FromArgb(31,41,55);
            this.CalendarBorderColor = Color.FromArgb(229,231,235);
            this.CalendarHoverBackColor = Color.FromArgb(250,250,251);
            this.CalendarHoverForeColor = Color.FromArgb(31,41,55);
            this.CalendarFooterColor = Color.FromArgb(250,250,251);
        }
    }
}