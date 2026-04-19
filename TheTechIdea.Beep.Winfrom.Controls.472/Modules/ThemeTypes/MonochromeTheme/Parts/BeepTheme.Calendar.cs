using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.WhiteSmoke;

        public TypographyStyle DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.LightGray;

        public TypographyStyle SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.DimGray;
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public TypographyStyle CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.Black;
        public Color CalendarForeColor { get; set; } = Color.WhiteSmoke;

        public Color CalendarTodayForeColor { get; set; } = Color.LightGray;
        public Color CalendarBorderColor { get; set; } = Color.Gray;

        public Color CalendarHoverBackColor { get; set; } = Color.DimGray;
        public Color CalendarHoverForeColor { get; set; } = Color.White;

        public TypographyStyle HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.Gray;
        public TypographyStyle FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8F, FontStyle.Italic);
    }
}
