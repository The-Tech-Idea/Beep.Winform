using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        public string FontName { get; set; } = "Arial";
        public float FontSize { get; set; } = 8f;
        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle() { FontSize = 20f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120) };
        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 105, 148) };
        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180) };
        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 130, 180) };
        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(0, 150, 200), IsUnderlined = true };
        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(0, 105, 148) };
    }
}
