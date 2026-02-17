using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle  CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900

        public TypographyStyle  DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(97, 97, 97); // Grey 600

        public TypographyStyle  SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public TypographyStyle  CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public TypographyStyle  CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900

        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(244, 67, 54); // Red 500
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue 50
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 800

        public TypographyStyle  HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public TypographyStyle  DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public TypographyStyle  FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Italic);
    }
}
