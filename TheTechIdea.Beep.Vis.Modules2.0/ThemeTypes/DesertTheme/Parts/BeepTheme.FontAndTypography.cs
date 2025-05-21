using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        public string FontName { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 14f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 22,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(101, 67, 33), // Rich Brown
            LineHeight = 1.2f
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(160, 82, 45), // Saddle Brown
            LineHeight = 1.15f
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(92, 64, 51), // Medium Brown
            LineHeight = 1.1f
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Light,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(160, 82, 45), // Saddle Brown
            LineHeight = 1.0f
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 248, 220), // Cornsilk
            LineHeight = 1.1f
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Underline,
            TextColor = Color.FromArgb(184, 134, 11), // Goldenrod
            LineHeight = 1.1f
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Light,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(210, 180, 140), // Tan
            LineHeight = 1.0f
        };
    }
}
