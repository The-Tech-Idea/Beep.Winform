using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Blockquote styling
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for border

        // Inline code styling
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(40, 40, 70); // Dark indigo-gray for contrast
        public TypographyStyle InlineCodeTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto Mono", // Changed to monospaced Roboto variant
            FontSize = 11f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(46, 204, 113), // Neon green for text
            LineHeight = 1.2f,
            LetterSpacing = 0.1f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Code block styling
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for depth
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for border
        public TypographyStyle CodeBlockTextStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto Mono", // Changed to monospaced Roboto variant
            FontSize = 11f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(241, 196, 15), // Neon yellow for code text
            LineHeight = 1.4f,
            LetterSpacing = 0.1f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };

        // Optional glow effects
        public int CodeBlockGlowSize { get; set; } = 4; // Slightly larger for neon glow
        public Color CodeBlockGlowColor { get; set; } = Color.FromArgb(120, 26, 188, 156); // Semi-transparent neon turquoise
    }
}