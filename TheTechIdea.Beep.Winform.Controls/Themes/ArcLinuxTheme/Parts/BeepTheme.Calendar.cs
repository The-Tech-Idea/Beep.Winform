using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = ForeColor;
            this.CalendarDaysHeaderForColor = ForeColor;
            this.CalendarSelectedDateBackColor = PrimaryColor;
            this.CalendarSelectedDateForColor = OnPrimaryColor;
            this.CalendarBackColor = BackgroundColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = AccentColor;
            this.CalendarBorderColor = ActiveBorderColor;
            this.CalendarHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.08);
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = ForeColor;
        }
    }
}
