using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Calendar Fonts & Colors
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 16f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.White;

        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.LightGray;

        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color CalendarForeColor { get; set; } = Color.White;

        public Color CalendarTodayForeColor { get; set; } = Color.Orange;

        public Color CalendarBorderColor { get; set; } = Color.DimGray;

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(60, 63, 65);
        public Color CalendarHoverForeColor { get; set; } = Color.White;

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.Gray;
        public Font FooterFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
    }
}
