using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        public string FontName { get; set; } = "Roboto";
        public float FontSize { get; set; } = 8f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(33, 33, 33) // Dark gray
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(117, 117, 117) // Medium gray
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(66, 66, 66) // Standard body text gray
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117) // Caption gray
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 8f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.White,
            IsUnderlined = false
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 150, 243), // Material Blue 500
            IsUnderlined = true
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 8f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(117, 117, 117), // Gray
            IsUnderlined = false
        };
    }
}
