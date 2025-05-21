using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
//<<<<<<< HEAD
        // Tree Fonts & Colors with Material Design defaults
        public Font TreeTitleFont { get; set; } = new Font("Roboto", 14F, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Roboto", 12F, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Roboto", 12F, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.White;
        public Color TreeForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TreeBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254); // Light Blue 50

        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.White;
    }
}
