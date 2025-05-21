using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.White;

        public TypographyStyle DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.LightGray;

        public TypographyStyle SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public TypographyStyle CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color CalendarForeColor { get; set; } = Color.LightGray;

        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(70, 70, 70);

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(45, 45, 48);
        public Color CalendarHoverForeColor { get; set; } = Color.White;

        public TypographyStyle HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public TypographyStyle YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11, FontStyle.Regular);
        public TypographyStyle DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Regular);
        public TypographyStyle DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9, FontStyle.Bold);
        public TypographyStyle DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.Gray;
        public TypographyStyle FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8, FontStyle.Italic);
    }
}
