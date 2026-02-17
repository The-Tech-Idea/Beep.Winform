using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle TreeNodeSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color TreeForeColor { get; set; } = Color.LightGray;
        public Color TreeBorderColor { get; set; } = Color.DimGray;
        public Color TreeNodeForeColor { get; set; } = Color.Gainsboro;
        public Color TreeNodeHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(70, 120, 180);
        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.CornflowerBlue;
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(40, 40, 40);
    }
}
