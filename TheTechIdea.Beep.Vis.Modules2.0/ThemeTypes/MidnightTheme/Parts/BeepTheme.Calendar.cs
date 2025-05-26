using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 16f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.White;

        public TypographyStyle  DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.LightGray;

        public TypographyStyle  SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public TypographyStyle  CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color CalendarForeColor { get; set; } = Color.White;

        public Color CalendarTodayForeColor { get; set; } = Color.Orange;

        public Color CalendarBorderColor { get; set; } = Color.DimGray;

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(60, 63, 65);
        public Color CalendarHoverForeColor { get; set; } = Color.White;

        public TypographyStyle  HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public TypographyStyle  DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);
        public TypographyStyle  DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.Gray;
        public TypographyStyle  FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
    }
}
