using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = PanelGradiantMiddleColor;
            this.CalendarDaysHeaderForColor = PanelGradiantMiddleColor;
            this.CalendarSelectedDateBackColor = PanelGradiantMiddleColor;
            this.CalendarSelectedDateForColor = PanelGradiantMiddleColor;
            this.CalendarBackColor = PanelGradiantMiddleColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = ForeColor;
            this.CalendarBorderColor = InactiveBorderColor;
            this.CalendarHoverBackColor = PanelGradiantMiddleColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = PanelGradiantMiddleColor;
        }
    }
}