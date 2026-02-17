using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Base Button Style
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 11f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);

        // Normal State
        public Color ButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray
        public Color ButtonForeColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise

        // Hover State (brighter glow)
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public TypographyStyle ButtonHoverTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 11f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Pressed State (inverted colors)
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color ButtonPressedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark background for contrast
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green

        // Selected State (purple accent)
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(50, 40, 70); // Dark purple-gray
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public TypographyStyle ButtonSelectedTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 11f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(155, 89, 182), // Neon purple
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Selected+Hover State
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(60, 50, 90); // Lighter purple-gray
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow

        // Error State (red accent)
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(70, 30, 30); // Dark red-gray
        public Color ButtonErrorForeColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red

        // Glow Effects
        public int ButtonGlowSize { get; set; } = 5; // Moderate glow for buttons
        public Color ButtonGlowColor { get; set; } = Color.FromArgb(100, 26, 188, 156); // Semi-transparent neon turquoise
        public int ButtonSelectedGlowSize { get; set; } = 8; // Larger glow for selected state
        public Color ButtonSelectedGlowColor { get; set; } = Color.FromArgb(120, 155, 89, 182); // Semi-transparent neon purple
    }
}
