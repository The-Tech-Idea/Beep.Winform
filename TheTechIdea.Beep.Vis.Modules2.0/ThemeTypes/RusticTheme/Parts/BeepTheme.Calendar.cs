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
            LineHeight = 1.2f,
            LetterSpacing = 0.4f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarTitleForColor { get; set; } = Color.White;
        public TypographyStyle DaysHeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(245, 245, 220), // Beige
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public TypographyStyle SelectedDateFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public TypographyStyle CalendarSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CalendarUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CalendarForeColor { get; set; } = Color.FromArgb(51, 51, 51); // Dark Gray
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan
        public Color CalendarHoverForeColor { get; set; } = Color.White;
        public TypographyStyle HeaderFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle MonthFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle YearFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DaysFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DaysSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DateFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 51, 51), // Dark Gray
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public TypographyStyle FooterFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Georgia",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}