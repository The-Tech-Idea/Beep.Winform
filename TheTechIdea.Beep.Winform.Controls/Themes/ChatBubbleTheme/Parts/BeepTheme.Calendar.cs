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
            this.CalendarTitleForColor = Color.FromArgb(245,248,255);
            this.CalendarDaysHeaderForColor = Color.FromArgb(245,248,255);
            this.CalendarSelectedDateBackColor = Color.FromArgb(245,248,255);
            this.CalendarSelectedDateForColor = Color.FromArgb(245,248,255);
            this.CalendarBackColor = Color.FromArgb(245,248,255);
            this.CalendarForeColor = Color.FromArgb(24,28,35);
            this.CalendarTodayForeColor = Color.FromArgb(24,28,35);
            this.CalendarBorderColor = Color.FromArgb(210,220,235);
            this.CalendarHoverBackColor = Color.FromArgb(245,248,255);
            this.CalendarHoverForeColor = Color.FromArgb(24,28,35);
            this.CalendarFooterColor = Color.FromArgb(245,248,255);
        }
    }
}