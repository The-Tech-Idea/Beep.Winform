using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(255, 214, 229);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(240, 190, 210);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color ButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(240, 150, 150);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
    }
}