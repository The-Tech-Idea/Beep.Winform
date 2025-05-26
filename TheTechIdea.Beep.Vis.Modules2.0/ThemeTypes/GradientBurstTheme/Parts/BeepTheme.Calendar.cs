using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(33, 33, 33);

        public TypographyStyle  DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(66, 66, 66);

        public TypographyStyle  SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(30, 136, 229);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public TypographyStyle  CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);
        public TypographyStyle  CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(189, 189, 189);

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(232, 240, 253);
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210);

        public TypographyStyle  HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Bold);
        public TypographyStyle  MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public TypographyStyle  DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);
        public TypographyStyle  DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(158, 158, 158);
        public TypographyStyle  FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Italic);
    }
}
