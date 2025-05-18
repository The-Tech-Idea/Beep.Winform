using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Calendar Fonts & Colors
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(255, 196, 0);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(255, 153, 0);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(204, 204, 204);
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(255, 230, 153);
        public Color CalendarHoverForeColor { get; set; } = Color.Black;
        public Font HeaderFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Font FooterFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Italic);
    }
}
