using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Tree Fonts & Colors
        public Font TreeTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color TreeBackColor { get; set; } = Color.White;
        public Color TreeForeColor { get; set; } = Color.Black;
        public Color TreeBorderColor { get; set; } = Color.LightGray;
        public Color TreeNodeForeColor { get; set; } = Color.Black;
        public Color TreeNodeHoverForeColor { get; set; } = Color.DodgerBlue;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.DodgerBlue;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.White;
    }
}
