using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        public string FontName { get; set; } = "Arial";
        public float FontSize { get; set; } = 8f;
        public TypographyStyle TitleStyle { get; set; } = new TypographyStyle() { FontSize = 20f, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle SubtitleStyle { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle BodyStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle CaptionStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(150, 150, 150) };
        public TypographyStyle ButtonStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle LinkStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(245, 183, 203), IsUnderlined = true };
        public TypographyStyle OverlineStyle { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(120, 120, 120) };
    }
}
