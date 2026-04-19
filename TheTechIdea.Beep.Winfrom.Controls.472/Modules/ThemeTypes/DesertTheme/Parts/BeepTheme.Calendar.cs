using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Calendar Fonts & Colors - Desert Warmth
        public TypographyStyle CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown

        public TypographyStyle DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public TypographyStyle SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(244, 164, 96); // SandyBrown
        public Color CalendarSelectedDateForColor { get; set; } = Color.FromArgb(60, 30, 10); // Dark Brown

        public TypographyStyle CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(255, 248, 220); // Cornsilk
        public Color CalendarForeColor { get; set; } = Color.FromArgb(101, 67, 33); // Brown
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135); // Burlywood
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(60, 30, 10); // Dark Brown

        public TypographyStyle HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public TypographyStyle FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
    }
}
