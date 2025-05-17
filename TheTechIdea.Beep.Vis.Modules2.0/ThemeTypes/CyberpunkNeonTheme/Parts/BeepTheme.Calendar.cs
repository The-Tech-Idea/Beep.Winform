using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Calendar Fonts & Colors

        public Font CalendarTitleFont { get; set; } = new Font("Consolas", 14f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon Cyan

        public Font DaysHeaderFont { get; set; } = new Font("Consolas", 11.5f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(255, 0, 255);       // Neon Magenta

        public Font SelectedDateFont { get; set; } = new Font("Consolas", 12f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(0, 102, 255);    // Neon Blue
        public Color CalendarSelectedDateForColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow

        public Font CalendarSelectedFont { get; set; } = new Font("Consolas", 12f, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Consolas", 11f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(24, 24, 48);                 // Deep Cyberpunk Black
        public Color CalendarForeColor { get; set; } = Color.FromArgb(0, 255, 255);                // Neon Cyan
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(0, 255, 128);           // Neon Green

        public Color CalendarBorderColor { get; set; } = Color.FromArgb(255, 0, 255);              // Neon Magenta

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255);           // Neon Magenta
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);           // Neon Yellow

        public Font HeaderFont { get; set; } = new Font("Consolas", 13f, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Consolas", 13f, FontStyle.Bold | FontStyle.Italic);
        public Font YearFont { get; set; } = new Font("Consolas", 12f, FontStyle.Italic);
        public Font DaysFont { get; set; } = new Font("Consolas", 11f, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Consolas", 11.5f, FontStyle.Bold | FontStyle.Underline);
        public Font DateFont { get; set; } = new Font("Consolas", 10.5f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(54, 162, 235);             // Neon Soft Blue
        public Font FooterFont { get; set; } = new Font("Consolas", 11f, FontStyle.Italic);
    }
}
