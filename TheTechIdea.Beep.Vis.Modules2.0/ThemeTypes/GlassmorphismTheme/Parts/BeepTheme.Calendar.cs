using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Calendar Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.Black;

        public TypographyStyle  DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.DarkSlateGray;

        public TypographyStyle  SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color CalendarSelectedDateForColor { get; set; } = Color.Black;

        public TypographyStyle  CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color CalendarForeColor { get; set; } = Color.Black;
        public Color CalendarTodayForeColor { get; set; } = Color.Blue;
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(160, 180, 200);

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(200, 210, 230);
        public Color CalendarHoverForeColor { get; set; } = Color.Black;

        public TypographyStyle  HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Bold);
        public TypographyStyle  YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Regular);
        public TypographyStyle  DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.DimGray;
        public TypographyStyle  FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Italic);
    }
}
