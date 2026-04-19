using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle  TreeTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  TreeNodeSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  TreeNodeUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.Black;
        public Color TreeForeColor { get; set; } = Color.White;
        public Color TreeBorderColor { get; set; } = Color.White;

        public Color TreeNodeForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverForeColor { get; set; } = Color.Yellow;
        public Color TreeNodeHoverBackColor { get; set; } = Color.DarkSlateGray;

        public Color TreeNodeSelectedForeColor { get; set; } = Color.Black;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.Yellow;

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.Black;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.Lime;
    }
}
