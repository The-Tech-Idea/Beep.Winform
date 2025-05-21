using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle TreeNodeSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.White;
        public Color TreeForeColor { get; set; } = Color.Black;
        public Color TreeBorderColor { get; set; } = Color.DarkGray;

        public Color TreeNodeForeColor { get; set; } = Color.Black;
        public Color TreeNodeHoverForeColor { get; set; } = Color.DimGray;
        public Color TreeNodeHoverBackColor { get; set; } = Color.LightGray;

        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.Black;

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.Black;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.White;
    }
}
