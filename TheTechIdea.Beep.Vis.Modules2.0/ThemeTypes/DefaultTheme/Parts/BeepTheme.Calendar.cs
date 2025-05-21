using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray

        public TypographyStyle DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(117, 117, 117); // Medium Gray

        public TypographyStyle SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public TypographyStyle CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray

        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(244, 67, 54); // Red Accent

        public Color CalendarBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Light Gray

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue

        public TypographyStyle HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
        public TypographyStyle MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
        public TypographyStyle DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(117, 117, 117); // Medium Gray
        public TypographyStyle FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Italic);
    }
}
