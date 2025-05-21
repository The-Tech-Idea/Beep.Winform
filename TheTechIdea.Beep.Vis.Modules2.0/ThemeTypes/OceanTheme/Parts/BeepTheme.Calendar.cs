using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle CalendarTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color CalendarTitleForColor { get; set; } = Color.White;
        public TypographyStyle DaysHeaderFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(200, 255, 255) };
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(200, 255, 255);
        public TypographyStyle SelectedDateFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public TypographyStyle CalendarSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle CalendarUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color CalendarBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color CalendarForeColor { get; set; } = Color.White;
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(0, 200, 240);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color CalendarHoverForeColor { get; set; } = Color.White;
        public TypographyStyle HeaderFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle MonthFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public TypographyStyle YearFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public TypographyStyle DaysFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public TypographyStyle DaysSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle DateFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.White };
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(0, 130, 180);
        public TypographyStyle FooterFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.White };
    }
}