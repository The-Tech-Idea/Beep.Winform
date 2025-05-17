using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Calendar Fonts & Colors

        public Font CalendarTitleFont { get; set; } = new Font("Comic Sans MS", 13f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        public Font SelectedDateFont { get; set; } = new Font("Comic Sans MS", 11.5f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color CalendarSelectedDateForColor { get; set; } = Color.FromArgb(44, 62, 80); // Navy for contrast

        public Font CalendarSelectedFont { get; set; } = new Font("Comic Sans MS", 11f, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon yellow
        public Color CalendarForeColor { get; set; } = Color.FromArgb(85, 85, 85); // Gray for dates
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(54, 162, 235); // Soft Blue

        public Color CalendarBorderColor { get; set; } = Color.FromArgb(206, 183, 255); // Pastel Lavender

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public Font HeaderFont { get; set; } = new Font("Comic Sans MS", 12f, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Comic Sans MS", 11f, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Font FooterFont { get; set; } = new Font("Segoe UI", 9.5f, FontStyle.Italic);
    }
}
