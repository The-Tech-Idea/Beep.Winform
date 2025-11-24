using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle  ScrollListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle  ScrollListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle  ScrollListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color ScrollListBackColor { get; set; } = Color.White;
        public Color ScrollListForeColor { get; set; } = Color.Black;
        public Color ScrollListBorderColor { get; set; } = Color.Gray;
        public Color ScrollListItemForeColor { get; set; } = Color.Black;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.DarkBlue;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.LightBlue;
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.Blue;
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.DarkBlue;
        public Color ScrollListItemBorderColor { get; set; } = Color.LightGray;
        public TypographyStyle  ScrollListIItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle  ScrollListItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
    }
}
