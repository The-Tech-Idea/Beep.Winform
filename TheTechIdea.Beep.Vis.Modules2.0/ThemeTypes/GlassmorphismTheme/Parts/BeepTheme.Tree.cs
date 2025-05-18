using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Tree Fonts & Colors
        public Font TreeTitleFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.White;
        public Color TreeForeColor { get; set; } = Color.Black;
        public Color TreeBorderColor { get; set; } = Color.LightGray;

        public Color TreeNodeForeColor { get; set; } = Color.Black;
        public Color TreeNodeHoverForeColor { get; set; } = Color.DarkBlue;
        public Color TreeNodeHoverBackColor { get; set; } = Color.LightBlue;

        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.DeepSkyBlue;

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.MediumSeaGreen;
    }
}
