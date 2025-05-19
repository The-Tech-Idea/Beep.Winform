using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Calendar Fonts & Colors
        public TypographyStyle CalendarTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 18,
            LineHeight = 1.3f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 50, 30),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(70, 50, 30);
        public TypographyStyle DaysHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(90, 70, 50);
        public TypographyStyle SelectedDateFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 220),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(180, 160, 140);
        public Color CalendarSelectedDateForColor { get; set; } = Color.FromArgb(255, 245, 220);
        public TypographyStyle CalendarSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 220),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CalendarUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarBackColor { get; set; } = Color.FromArgb(240, 230, 210);
        public Color CalendarForeColor { get; set; } = Color.FromArgb(90, 70, 50);
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(120, 90, 60);
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(150, 130, 110);
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(255, 245, 220);
        public TypographyStyle HeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 16,
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 50, 30),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MonthFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle YearFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DaysFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DaysSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 220),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DateFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(220, 210, 190);
        public TypographyStyle FooterFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(90, 70, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}