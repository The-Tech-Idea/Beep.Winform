using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {

        public TypographyStyle  CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(230, 230, 250); // Soft white
        public TypographyStyle  DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(180, 180, 255); // Light blue-purple
        public TypographyStyle  SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(70, 20, 120); // Deep purple
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public TypographyStyle  CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9.5f, FontStyle.Bold);
        public TypographyStyle  CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.FromArgb(20, 20, 40); // Dark space blue
        public Color CalendarForeColor { get; set; } = Color.FromArgb(200, 200, 230); // Light lavender
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(100, 180, 255); // Bright blue
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(72, 61, 139); // Dark slate blue
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Medium space blue
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(240, 240, 255); // Bright white
        public TypographyStyle  HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Bold);
        public TypographyStyle  YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public TypographyStyle  DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);
        public TypographyStyle  DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(30, 30, 60); // Deep space blue
        public TypographyStyle  FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);

    }
}