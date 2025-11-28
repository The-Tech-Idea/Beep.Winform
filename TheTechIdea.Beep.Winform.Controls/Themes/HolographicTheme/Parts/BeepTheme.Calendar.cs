using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = ForeColor;
            this.CalendarDaysHeaderForColor = ForeColor;
            this.CalendarSelectedDateBackColor = PrimaryColor;
            this.CalendarSelectedDateForColor = OnPrimaryColor;
            this.CalendarBackColor = SurfaceColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = AccentColor;
            this.CalendarBorderColor = InactiveBorderColor;
            this.CalendarHoverBackColor = PanelGradiantStartColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = SurfaceColor;
        }
    }
}