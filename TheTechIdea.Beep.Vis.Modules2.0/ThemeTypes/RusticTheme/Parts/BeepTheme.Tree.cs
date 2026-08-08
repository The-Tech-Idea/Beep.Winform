using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; }
        public TypographyStyle TreeNodeSelectedFont { get; set; }
        public TypographyStyle TreeNodeUnSelectedFont { get; set; }
        public Color TreeBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color TreeForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TreeBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(160, 82, 45);
    }
}
