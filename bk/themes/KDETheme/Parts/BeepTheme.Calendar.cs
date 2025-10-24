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
            this.CalendarTitleForColor = Color.FromArgb(248,249,250);
            this.CalendarDaysHeaderForColor = Color.FromArgb(248,249,250);
            this.CalendarSelectedDateBackColor = Color.FromArgb(248,249,250);
            this.CalendarSelectedDateForColor = Color.FromArgb(248,249,250);
            this.CalendarBackColor = Color.FromArgb(248,249,250);
            this.CalendarForeColor = Color.FromArgb(33,37,41);
            this.CalendarTodayForeColor = Color.FromArgb(33,37,41);
            this.CalendarBorderColor = Color.FromArgb(222,226,230);
            this.CalendarHoverBackColor = Color.FromArgb(248,249,250);
            this.CalendarHoverForeColor = Color.FromArgb(33,37,41);
            this.CalendarFooterColor = Color.FromArgb(248,249,250);
        }
    }
}