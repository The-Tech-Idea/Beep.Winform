using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Tree Fonts & Colors

        public Font TreeTitleFont { get; set; } = new Font("Comic Sans MS", 12f, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Comic Sans MS", 11f, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.FromArgb(255, 224, 235);              // Pastel Pink
        public Color TreeForeColor { get; set; } = Color.FromArgb(44, 62, 80);                 // Navy
        public Color TreeBorderColor { get; set; } = Color.FromArgb(127, 255, 212);            // Mint

        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(44, 62, 80);             // Navy
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);     // Candy Pink
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255);     // Baby Blue

        public Color TreeNodeSelectedForeColor { get; set; } = Color.FromArgb(255, 223, 93);   // Lemon Yellow
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240);  // Mint

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(127, 255, 212);    // Mint
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(255, 253, 194);    // Lemon Yellow
    }
}
