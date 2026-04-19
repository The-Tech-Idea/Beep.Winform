using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle  TreeTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  TreeNodeSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  TreeNodeUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color TreeBackColor { get; set; } = Color.White;
        public Color TreeForeColor { get; set; } = Color.Black;
        public Color TreeBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TreeNodeForeColor { get; set; } = Color.Black;
        public Color TreeNodeHoverForeColor { get; set; } = Color.White;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.White;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(0, 120, 212);
    }
}
