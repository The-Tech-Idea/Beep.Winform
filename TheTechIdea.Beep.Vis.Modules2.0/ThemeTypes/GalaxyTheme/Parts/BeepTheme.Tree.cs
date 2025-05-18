using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Tree Fonts & Colors
        public Font TreeTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color TreeForeColor { get; set; } = Color.White;
        public Color TreeBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border

        public Color TreeNodeForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover background

        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // SuccessColor
    }
}
