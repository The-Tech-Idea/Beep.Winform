using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.White;
        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.Yellow;
        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.Black;
        public Color CalendarSelectedDateForColor { get; set; } = Color.Lime;
        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.Black;
        public Color CalendarForeColor { get; set; } = Color.White;
        public Color CalendarTodayForeColor { get; set; } = Color.Cyan;
        public Color CalendarBorderColor { get; set; } = Color.White;
        public Color CalendarHoverBackColor { get; set; } = Color.DarkGray;
        public Color CalendarHoverForeColor { get; set; } = Color.Black;
        public Font HeaderFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font YearFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.Gray;
        public Font FooterFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
    }
}
