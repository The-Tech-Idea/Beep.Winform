using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Tree Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  TreeTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  TreeNodeSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  TreeNodeUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

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
    }
}
