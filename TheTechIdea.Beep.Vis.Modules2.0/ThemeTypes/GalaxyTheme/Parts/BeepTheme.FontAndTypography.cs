namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        public string FontName { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 10f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = System.Drawing.FontStyle.Regular,
            LineHeight = 1.3f,
            LetterSpacing = 0f,
            TextColor = System.Drawing.Color.White
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Regular,
            FontStyle = System.Drawing.FontStyle.Italic,
            LineHeight = 1.3f,
            LetterSpacing = 0f,
            TextColor = System.Drawing.Color.FromArgb(0xA0, 0xA0, 0xFF)
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            LineHeight = 1.4f,
            LetterSpacing = 0f,
            TextColor = System.Drawing.Color.LightGray
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 9f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Italic,
            LineHeight = 1.2f,
            LetterSpacing = 0f,
            TextColor = System.Drawing.Color.Gray
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            LineHeight = 1.2f,
            LetterSpacing = 0f,
            TextColor = System.Drawing.Color.White
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            FontStyle = System.Drawing.FontStyle.Underline,
            LineHeight = 1.2f,
            LetterSpacing = 0f,
            TextColor = System.Drawing.Color.FromArgb(0x4E, 0xC5, 0xF1)
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Regular,
            LineHeight = 1f,
            LetterSpacing = 1f,
            TextColor = System.Drawing.Color.Gray
        };
    }
}
