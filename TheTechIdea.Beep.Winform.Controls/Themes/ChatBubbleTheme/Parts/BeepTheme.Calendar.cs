using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyCalendar()
        {
            this.CalendarTitleForColor = ForeColor;
            this.CalendarDaysHeaderForColor = ForeColor;
            this.CalendarSelectedDateBackColor = PrimaryColor;
            this.CalendarSelectedDateForColor = OnPrimaryColor;
            this.CalendarBackColor = PanelGradiantStartColor;
            this.CalendarForeColor = ForeColor;
            this.CalendarTodayForeColor = ForeColor;
            this.CalendarBorderColor = BorderColor;
            this.CalendarHoverBackColor = PanelGradiantMiddleColor;
            this.CalendarHoverForeColor = ForeColor;
            this.CalendarFooterColor = ForeColor;
        }
    }
}