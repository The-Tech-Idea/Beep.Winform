using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Tree Fonts & Colors
<<<<<<< HEAD
        public Font TreeTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font TreeNodeSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font TreeNodeUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.FromArgb(24, 24, 30);
        public Color TreeForeColor { get; set; } = Color.LightGray;
        public Color TreeBorderColor { get; set; } = Color.DimGray;

        public Color TreeNodeForeColor { get; set; } = Color.LightGray;
        public Color TreeNodeHoverForeColor { get; set; } = Color.CornflowerBlue;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(45, 45, 48);

        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.MediumSlateBlue;

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.CornflowerBlue;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(24, 24, 30);
=======
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
