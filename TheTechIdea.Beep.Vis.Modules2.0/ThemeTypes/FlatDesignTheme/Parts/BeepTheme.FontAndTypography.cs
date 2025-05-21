using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        public string FontName { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 14f;

        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 24,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(33, 33, 33),
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(66, 66, 66),
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(85, 85, 85),
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            FontWeight = FontWeight.Light,
            TextColor = Color.FromArgb(120, 120, 120),
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };

        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(33, 150, 243), // typical blue link color
            IsUnderlined = true,
            IsStrikeout = false
        };

        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 10,
            FontWeight = FontWeight.Normal,
            TextColor = Color.FromArgb(158, 158, 158),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}
