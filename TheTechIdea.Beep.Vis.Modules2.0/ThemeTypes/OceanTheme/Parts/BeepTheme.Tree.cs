using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle() { FontSize = 14f, FontWeight = FontWeight.Bold, TextColor = Color.White };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 8f, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color TreeBackColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color TreeForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color TreeBorderColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(200, 255, 255);
        public Color TreeNodeHoverForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(0, 160, 210);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(0, 180, 230);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(0, 180, 230);
    }
}
