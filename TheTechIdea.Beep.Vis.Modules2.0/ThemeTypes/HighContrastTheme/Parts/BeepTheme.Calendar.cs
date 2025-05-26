using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.White;
        public TypographyStyle  DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.Yellow;
        public TypographyStyle  SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.Black;
        public Color CalendarSelectedDateForColor { get; set; } = Color.Lime;
        public TypographyStyle  CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Bold);
        public TypographyStyle  CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.Black;
        public Color CalendarForeColor { get; set; } = Color.White;
        public Color CalendarTodayForeColor { get; set; } = Color.Cyan;
        public Color CalendarBorderColor { get; set; } = Color.White;
        public Color CalendarHoverBackColor { get; set; } = Color.DarkGray;
        public Color CalendarHoverForeColor { get; set; } = Color.Black;
        public TypographyStyle  HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public TypographyStyle  DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Bold);
        public TypographyStyle  DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.Gray;
        public TypographyStyle  FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
    }
}
