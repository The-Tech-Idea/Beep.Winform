namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        public string FontName { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 12f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16f,
            FontWeight = FontWeight.SemiBold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Yellow,
            LineHeight = 1.2f,
            LetterSpacing = 0.4f,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.5f,
            LetterSpacing = 0.25f,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.Gray,
            LineHeight = 1.1f,
            LetterSpacing = 0.2f,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Black,
            LineHeight = 1f,
            LetterSpacing = 0.3f,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Underline,
            TextColor = System.Drawing.Color.Cyan,
            LineHeight = 1f,
            LetterSpacing = 0.2f,
            IsUnderlined = true,
            IsStrikeout = false
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.LightGray,
            LineHeight = 1f,
            LetterSpacing = 0.4f,
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}
