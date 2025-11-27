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
            this.CalendarTitleForColor = ForeColor;
            this.CalendarDaysHeaderForColor = ForeColor;
            this.CalendarSelectedDateBackColor = SecondaryColor;
            this.CalendarSelectedDateForColor = ForeColor;
            this.CalendarBackColor = SurfaceColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = AccentColor;
            this.CalendarBorderColor = BorderColor;
            this.CalendarHoverBackColor = SecondaryColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = ForeColor;
        }
    }
}