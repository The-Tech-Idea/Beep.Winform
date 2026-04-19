namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        public string FontName { get; set; } = "Comic Sans MS"; // Playful default, swap for "Nunito"/"Montserrat" if you prefer
        public float FontSize { get; set; } = 8f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LetterSpacing = 0.2f,
            LineHeight = 1.15f
        };
        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(127, 255, 212), // Mint
            LetterSpacing = 0.1f,
            LineHeight = 1.12f
        };
        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(44, 62, 80), // Navy
            LetterSpacing = 0f,
            LineHeight = 1.2f
        };
        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.FromArgb(206, 183, 255), // Pastel Lavender
            LetterSpacing = 0.05f,
            LineHeight = 1.1f
        };
        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 8f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(240, 100, 180), // Candy Pink
            LetterSpacing = 0.08f,
            LineHeight = 1.1f
        };
        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Underline,
            TextColor = System.Drawing.Color.FromArgb(54, 162, 235), // Soft Blue
            LetterSpacing = 0.04f,
            LineHeight = 1.05f
        };
        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.FromArgb(255, 223, 93), // Lemon Yellow
            LetterSpacing = 0.2f,
            LineHeight = 1.0f
        };
    }
}
