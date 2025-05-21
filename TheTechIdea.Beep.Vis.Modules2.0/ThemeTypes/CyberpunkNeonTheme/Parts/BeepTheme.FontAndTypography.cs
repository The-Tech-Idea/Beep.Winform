namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        public string FontName { get; set; } = "Consolas";
        public float FontSize { get; set; } = 11f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(255, 0, 255),  // Neon Magenta
            LetterSpacing = 1.4f,
            LineHeight = 1.15f
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 13.5f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(0, 255, 128), // Neon Green
            LetterSpacing = 1.2f,
            LineHeight = 1.13f
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 11f,
            FontWeight = FontWeight.Regular,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(0, 255, 255), // Neon Cyan
            LetterSpacing = 1.1f,
            LineHeight = 1.07f
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 10f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(255, 255, 0), // Neon Yellow
            LetterSpacing = 1.0f,
            LineHeight = 1.02f
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 11f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(0, 255, 255), // Neon Cyan
            LetterSpacing = 1.3f
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 11f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Underline,
            TextColor = System.Drawing.Color.FromArgb(255, 255, 0), // Neon Yellow
            LetterSpacing = 1.1f
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Consolas",
            FontSize = 10.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(0, 255, 255), // Neon Cyan
            LetterSpacing = 2.2f
        };
    }
}
