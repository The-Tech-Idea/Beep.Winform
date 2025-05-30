using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // ScrollList Fonts & Colors
//<<<<<<< HEAD
        public TypographyStyle  ScrollListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  ScrollListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  ScrollListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color ScrollListBackColor { get; set; } = Color.White;
        public Color ScrollListForeColor { get; set; } = Color.Black;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color ScrollListItemForeColor { get; set; } = Color.Black;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 64, 128);
        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(220, 220, 220);
        public TypographyStyle  ScrollListIItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle  ScrollListItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
    }
}
