namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        public string FontName { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 14f; // Increased from 12f for better readability

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Black,
            LineHeight = 1.2f
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Italic,
            TextColor = System.Drawing.Color.Gray,
            LineHeight = 1.15f
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14, // Increased from 12 for better readability
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Black,
            LineHeight = 1.1f
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12, // Increased from 10 for better readability
            FontWeight = FontWeight.Light,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Gray,
            LineHeight = 1.0f
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 15, // Increased from 14 for better readability
            FontWeight = FontWeight.SemiBold,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.White,
            LineHeight = 1.1f
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 13, // Increased from 12 for better readability
            FontWeight = FontWeight.Medium,
            FontStyle = System.Drawing.FontStyle.Underline,
            TextColor = System.Drawing.Color.FromArgb(0, 120, 215), // blue link color
            LineHeight = 1.1f
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 9,
            FontWeight = FontWeight.Normal,
            FontStyle = System.Drawing.FontStyle.Regular,
            TextColor = System.Drawing.Color.Gray,
            LineHeight = 1.0f,
            IsUnderlined = true
        };
    }
}
