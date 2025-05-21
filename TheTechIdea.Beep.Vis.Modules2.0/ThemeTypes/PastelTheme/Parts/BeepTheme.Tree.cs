using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = new TypographyStyle() { FontSize = 14, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle TreeNodeSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color TreeBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color TreeForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color TreeBorderColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(245, 183, 203);
    }
}