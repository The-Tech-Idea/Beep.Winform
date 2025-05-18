using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Calendar Fonts & Colors
        public Font CalendarTitleFont { get; set; } = new Font(new FontFamily("Segoe UI"), 14, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Font DaysHeaderFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(66, 66, 66);
        public Font SelectedDateFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public Font CalendarSelectedFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(255, 87, 34); // Deep Orange Accent
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254);
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Font HeaderFont { get; set; } = new Font(new FontFamily("Segoe UI"), 12, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font(new FontFamily("Segoe UI"), 9, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font(new FontFamily("Segoe UI"), 9, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font(new FontFamily("Segoe UI"), 9, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Font FooterFont { get; set; } = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular);
    }
}
