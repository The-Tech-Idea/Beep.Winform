using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Calendar Fonts & Colors
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.White;

        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.LightGray;

        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color CalendarForeColor { get; set; } = Color.LightGray;

        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(70, 70, 70);

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public Color CalendarHoverForeColor { get; set; } = Color.White;

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.Gray;
        public Font FooterFont { get; set; } = new Font("Segoe UI", 8, FontStyle.Italic);
    }
}
