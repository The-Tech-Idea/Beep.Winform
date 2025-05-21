using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle CalendarTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(80, 80, 80);
        public TypographyStyle DaysHeaderFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(120, 120, 120);
        public TypographyStyle SelectedDateFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public TypographyStyle CalendarSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle CalendarUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color CalendarBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color CalendarForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public TypographyStyle HeaderFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle MonthFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle YearFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DaysFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle DaysSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle DateFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(237, 181, 201);
        public TypographyStyle FooterFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
    }
}