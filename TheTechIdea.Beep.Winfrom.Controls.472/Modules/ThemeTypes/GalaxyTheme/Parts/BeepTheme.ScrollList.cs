using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle  ScrollListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  ScrollListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  ScrollListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color ScrollListBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color ScrollListForeColor { get; set; } = Color.White;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(0x33, 0x33, 0x33); // Subtle border

        public Color ScrollListItemForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(0x23, 0x23, 0x4E); // Hover background

        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.White;

        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(0x44, 0x44, 0x44); // Divider tone

        public TypographyStyle  ScrollListIItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  ScrollListItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
    }
}
