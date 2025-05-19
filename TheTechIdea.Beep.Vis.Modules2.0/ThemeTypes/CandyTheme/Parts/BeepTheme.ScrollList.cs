using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // ScrollList Fonts & Colors

        public TypographyStyle ScrollListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle ScrollListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        public Color ScrollListBackColor { get; set; } = Color.FromArgb(255, 224, 235);        // Pastel Pink
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(44, 62, 80);           // Navy
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(127, 255, 212);      // Mint

        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(44, 62, 80);       // Navy
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue

        public Color ScrollListItemSelectedForeColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(54, 162, 235); // Pastel Blue

        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(206, 183, 255);      // Pastel Lavender

        public TypographyStyle ScrollListIItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
    }
}
