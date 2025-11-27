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
            this.CalendarTitleForColor = ForeColor;
            this.CalendarDaysHeaderForColor = ForeColor;
            this.CalendarSelectedDateBackColor = SurfaceColor;
            this.CalendarSelectedDateForColor = ForeColor;
            this.CalendarBackColor = SurfaceColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = ForeColor;
            this.CalendarBorderColor = BorderColor;
            this.CalendarHoverBackColor = SurfaceColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = SurfaceColor;
        }
    }
}