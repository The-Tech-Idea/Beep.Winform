using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Calendar Fonts & Colors
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 14F, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray

        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(117, 117, 117); // Medium Gray

        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray

        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(244, 67, 54); // Red Accent

        public Color CalendarBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Light Gray

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue

        public Font HeaderFont { get; set; } = new Font("Segoe UI", 12F, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(117, 117, 117); // Medium Gray
        public Font FooterFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Italic);
    }
}
