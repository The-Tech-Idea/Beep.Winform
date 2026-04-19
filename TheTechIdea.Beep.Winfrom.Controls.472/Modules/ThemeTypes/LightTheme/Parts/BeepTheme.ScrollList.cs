using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle  ScrollListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  ScrollListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  ScrollListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color ScrollListBackColor { get; set; } = Color.White;
        public Color ScrollListForeColor { get; set; } = Color.Black;
        public Color ScrollListBorderColor { get; set; } = Color.LightGray;
        public Color ScrollListItemForeColor { get; set; } = Color.Black;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.DarkBlue;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.LightBlue;
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color ScrollListItemBorderColor { get; set; } = Color.LightGray;
        public TypographyStyle  ScrollListIItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  ScrollListItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
    }
}
