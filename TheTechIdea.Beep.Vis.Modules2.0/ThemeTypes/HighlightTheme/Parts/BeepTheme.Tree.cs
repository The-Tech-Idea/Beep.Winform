using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Tree Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  TreeTitleFont { get; set; }
        public TypographyStyle  TreeNodeSelectedFont { get; set; }
        public TypographyStyle  TreeNodeUnSelectedFont { get; set; }
        public Color TreeBackColor { get; set; } = Color.White;
        public Color TreeForeColor { get; set; } = Color.Black;
        public Color TreeBorderColor { get; set; } = Color.LightGray;
        public Color TreeNodeForeColor { get; set; } = Color.Black;
        public Color TreeNodeHoverForeColor { get; set; } = Color.DarkBlue;
        public Color TreeNodeHoverBackColor { get; set; } = Color.LightBlue;
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.Black;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.LightGray;
    }
}
