using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
<<<<<<< HEAD
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
=======
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; }
        public TypographyStyle TreeNodeSelectedFont { get; set; }
        public TypographyStyle TreeNodeUnSelectedFont { get; set; }
        public Color TreeBackColor { get; set; }
        public Color TreeForeColor { get; set; }
        public Color TreeBorderColor { get; set; }
        public Color TreeNodeForeColor { get; set; }
        public Color TreeNodeHoverForeColor { get; set; }
        public Color TreeNodeHoverBackColor { get; set; }
        public Color TreeNodeSelectedForeColor { get; set; }
        public Color TreeNodeSelectedBackColor { get; set; }
        public Color TreeNodeCheckedBoxForeColor { get; set; }
        public Color TreeNodeCheckedBoxBackColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
