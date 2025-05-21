using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(33, 33, 33);

        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(66, 66, 66);

        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(30, 136, 229);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(189, 189, 189);

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(232, 240, 253);
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210);

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(158, 158, 158);
        public Font FooterFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Italic);
    }
}
