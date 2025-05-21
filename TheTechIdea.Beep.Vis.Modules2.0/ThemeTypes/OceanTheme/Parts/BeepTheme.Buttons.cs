using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle ButtonHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle ButtonSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.White };

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color ButtonHoverForeColor { get; set; } = Color.White;
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(0, 100, 150);
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(0, 200, 240);
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(0, 110, 160);
        public Color ButtonBackColor { get; set; } = Color.FromArgb(0, 150, 200);
        public Color ButtonForeColor { get; set; } = Color.White;
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 100, 100);
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(200, 80, 80);
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(0, 100, 150);
    }
}