using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Calendar Fonts & Colors - Desert Warmth
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown

        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(244, 164, 96); // SandyBrown
        public Color CalendarSelectedDateForColor { get; set; } = Color.FromArgb(60, 30, 10); // Dark Brown

        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(255, 248, 220); // Cornsilk
        public Color CalendarForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Brown
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135); // Burlywood
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(60, 30, 10); // Dark Brown

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Font FooterFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Italic);
    }
}
