using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // ScrollList Fonts & Colors
        public TypographyStyle ScrollListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle ScrollListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);
        public Color ScrollListBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color ScrollListForeColor { get; set; } = Color.WhiteSmoke;
        public Color ScrollListBorderColor { get; set; } = Color.Gray;
        public Color ScrollListItemForeColor { get; set; } = Color.LightGray;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.White;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.DimGray;
        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.DarkGray;
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.LightGray;
        public Color ScrollListItemBorderColor { get; set; } = Color.DimGray;
        public TypographyStyle ScrollListIItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Regular);
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 11f, FontStyle.Bold);
    }
}
