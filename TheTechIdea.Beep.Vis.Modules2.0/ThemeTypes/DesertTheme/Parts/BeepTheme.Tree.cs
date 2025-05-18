using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Tree Fonts & Colors
        public Font TreeTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.FromArgb(255, 244, 214, 165);   // Light sand beige
        public Color TreeForeColor { get; set; } = Color.FromArgb(102, 51, 0);            // Dark brown

        public Color TreeBorderColor { get; set; } = Color.FromArgb(194, 178, 128);       // Soft tan

        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(102, 51, 0);        // Dark brown
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(139, 69, 19);  // Saddle brown
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(255, 250, 240);// Very light sand

        public Color TreeNodeSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 255);  // White
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(210, 180, 140);// Tan

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(160, 82, 45);// Sienna
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(244, 164, 96);// Sandy brown
    }
}
