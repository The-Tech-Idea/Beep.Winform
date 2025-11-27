using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = ForeColor;
            this.CalendarDaysHeaderForColor = ForeColor;
            this.CalendarSelectedDateBackColor = PanelBackColor;
            this.CalendarSelectedDateForColor = ForeColor;
            this.CalendarBackColor = PanelBackColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = ForeColor;
            this.CalendarBorderColor = InactiveBorderColor;
            this.CalendarHoverBackColor = PanelGradiantMiddleColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = PanelBackColor;
        }
    }
}