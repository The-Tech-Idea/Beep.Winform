using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = PanelBackColor;
            this.CalendarDaysHeaderForColor = PanelBackColor;
            this.CalendarSelectedDateBackColor = PanelBackColor;
            this.CalendarSelectedDateForColor = PanelBackColor;
            this.CalendarBackColor = PanelBackColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = ForeColor;
            this.CalendarBorderColor = BorderColor;
            this.CalendarHoverBackColor = PanelBackColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = PanelBackColor;
        }
    }
}