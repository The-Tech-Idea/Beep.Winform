using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Calendar Container
        public Color CalendarBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for depth
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border

        // Title Section (Month/Year)
        public TypographyStyle CalendarTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public Color CalendarTitleBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray

        // Days Header (Mon-Sun)
        public TypographyStyle DaysHeaderStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(155, 89, 182), // Neon purple
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Normal Dates
        public TypographyStyle DateTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 11f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray for readability
            LineHeight = 1.1f,
            LetterSpacing = 0.1f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Hover State
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green

        // Selected Date
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color CalendarSelectedDateForColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for contrast
        public TypographyStyle SelectedDateTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 11f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
            LineHeight = 1.1f,
            LetterSpacing = 0.1f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Today's Date
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color CalendarTodayBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow

        // Footer (Time/Buttons)
        public Color CalendarTeslaFooterColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray
        public TypographyStyle FooterTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.0f,
            LetterSpacing = 0.1f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Special States
        public Color CalendarWeekendForeColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public Color CalendarOtherMonthForeColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue

        // Glow Effects
        public int CalendarSelectedGlowSize { get; set; } = 5; // Moderate glow for selection
        public Color CalendarSelectedGlowColor { get; set; } = Color.FromArgb(100, 46, 204, 113); // Semi-transparent neon green
    }
}