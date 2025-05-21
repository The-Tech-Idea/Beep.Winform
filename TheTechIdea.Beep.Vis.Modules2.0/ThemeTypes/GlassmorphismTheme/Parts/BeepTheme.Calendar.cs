using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.Black;

        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.DarkSlateGray;

        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color CalendarSelectedDateForColor { get; set; } = Color.Black;

        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color CalendarForeColor { get; set; } = Color.Black;
        public Color CalendarTodayForeColor { get; set; } = Color.Blue;
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(160, 180, 200);

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(200, 210, 230);
        public Color CalendarHoverForeColor { get; set; } = Color.Black;

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
        public Font YearFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.DimGray;
        public Font FooterFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Italic);
    }
}
