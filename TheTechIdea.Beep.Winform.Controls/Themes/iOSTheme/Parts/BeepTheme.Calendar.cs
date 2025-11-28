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
            this.CalendarTitleForColor = BackgroundColor;
            this.CalendarDaysHeaderForColor = BackgroundColor;
            this.CalendarSelectedDateBackColor = BackgroundColor;
            this.CalendarSelectedDateForColor = BackgroundColor;
            this.CalendarBackColor = BackgroundColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = ForeColor;
            this.CalendarBorderColor = BorderColor;
            this.CalendarHoverBackColor = BackgroundColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = BackgroundColor;
        }
    }
}