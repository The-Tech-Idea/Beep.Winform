using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        public string FontName { get; set; } = "Courier New";
        public float FontSize { get; set; } = 12;
        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 14,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.4f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(160, 160, 160),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 165, 0),
            IsUnderlined = true,
            IsStrikeout = false
        };
        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 10,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(160, 160, 160),
            IsUnderlined = false,
            IsStrikeout = false
        };
    }
}