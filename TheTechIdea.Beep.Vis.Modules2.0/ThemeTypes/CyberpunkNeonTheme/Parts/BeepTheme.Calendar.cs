using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Calendar Fonts & Colors

        public TypographyStyle CalendarTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon Cyan

        public TypographyStyle DaysHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(255, 0, 255);       // Neon Magenta

        public TypographyStyle SelectedDateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(0, 102, 255);    // Neon Blue
        public Color CalendarSelectedDateForColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon Yellow

        public TypographyStyle CalendarSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);
        public TypographyStyle CalendarUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.FromArgb(24, 24, 48);                 // Deep Cyberpunk Black
        public Color CalendarForeColor { get; set; } = Color.FromArgb(0, 255, 255);                // Neon Cyan
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(0, 255, 128);           // Neon Green

        public Color CalendarBorderColor { get; set; } = Color.FromArgb(255, 0, 255);              // Neon Magenta

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255);           // Neon Magenta
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);           // Neon Yellow

        public TypographyStyle HeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold);
        public TypographyStyle MonthFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle YearFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Italic);
        public TypographyStyle DaysFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);
        public TypographyStyle DaysSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Bold | FontStyle.Underline);
        public TypographyStyle DateFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(54, 162, 235);             // Neon Soft Blue
        public TypographyStyle FooterFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 8f, FontStyle.Italic);
    }
}
