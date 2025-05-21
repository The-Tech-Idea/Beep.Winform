namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        public string FontName { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 10f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Black,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.DimGray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Black,
            LineHeight = 1.4f,
            LetterSpacing = 0f
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 9f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Gray,
            LineHeight = 1.2f,
            LetterSpacing = 0f
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Black,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Regular,
            FontStyle = System.Drawing.FontStyle.Underline,
            TextColor = System.Drawing.Color.SteelBlue,
            LineHeight = 1.1f,
            LetterSpacing = 0f
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.DarkGray,
            LineHeight = 1.1f,
            LetterSpacing = 1f
        };
    }
}
