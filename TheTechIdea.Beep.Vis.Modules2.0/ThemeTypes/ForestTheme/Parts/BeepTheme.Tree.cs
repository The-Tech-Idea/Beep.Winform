using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Tree Fonts & Colors
        public Font TreeTitleFont { get; set; } = new Font("Segoe UI", 12F, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Color TreeBackColor { get; set; } = Color.FromArgb(34, 45, 30); // dark olive green
        public Color TreeForeColor { get; set; } = Color.FromArgb(200, 230, 200); // light greenish
        public Color TreeBorderColor { get; set; } = Color.FromArgb(85, 107, 47); // dark olive
        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(210, 240, 210); // soft green
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(240, 255, 240); // light green
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(50, 80, 40); // darker hover green
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(60, 100, 50); // medium green
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(120, 180, 90); // medium light green
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(30, 50, 20); // dark green background
    }
}
