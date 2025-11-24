using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle  CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.Black;
        public TypographyStyle  DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.DarkSlateGray;
        public TypographyStyle  SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.DodgerBlue;
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public TypographyStyle  CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
        public TypographyStyle  CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.Black;
        public Color CalendarTodayForeColor { get; set; } = Color.Red;
        public Color CalendarBorderColor { get; set; } = Color.LightGray;
        public Color CalendarHoverBackColor { get; set; } = Color.LightSteelBlue;
        public Color CalendarHoverForeColor { get; set; } = Color.Black;
        public TypographyStyle  HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11F, FontStyle.Bold);
        public TypographyStyle  MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle  YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle  DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
        public TypographyStyle  DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.LightGray;
        public TypographyStyle  FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8F, FontStyle.Italic);
    }
}
