using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = ForeColor;
            this.CalendarDaysHeaderForColor = ForeColor;
            this.CalendarSelectedDateBackColor = PanelBackColor;
            this.CalendarSelectedDateForColor = ForeColor;
            this.CalendarBackColor = BackgroundColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = ForeColor;
            this.CalendarBorderColor = SecondaryColor;
            this.CalendarHoverBackColor = PanelGradiantMiddleColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = BackgroundColor;
        }
    }
}