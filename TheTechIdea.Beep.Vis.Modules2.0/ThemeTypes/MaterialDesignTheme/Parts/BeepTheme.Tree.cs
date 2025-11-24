using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Tree Fonts & Colors with Material Design defaults
        public TypographyStyle  TreeTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14F, FontStyle.Bold);
        public TypographyStyle  TreeNodeSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 12F, FontStyle.Bold);
        public TypographyStyle  TreeNodeUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 12F, FontStyle.Regular);

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
