using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 14F, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.Black;
        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.DarkSlateGray;
        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.DodgerBlue;
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.Black;
        public Color CalendarTodayForeColor { get; set; } = Color.Red;
        public Color CalendarBorderColor { get; set; } = Color.LightGray;
        public Color CalendarHoverBackColor { get; set; } = Color.LightSteelBlue;
        public Color CalendarHoverForeColor { get; set; } = Color.Black;
        public Font HeaderFont { get; set; } = new Font("Segoe UI", 11F, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.LightGray;
        public Font FooterFont { get; set; } = new Font("Segoe UI", 8F, FontStyle.Italic);
    }
}
