using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        public string FontName { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 8f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.Gray
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Black
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Light,
            TextColor = Color.DarkGray
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.White
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Blue,
            IsUnderlined = true
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.Gray,
            IsUnderlined = false
        };
    }
}
