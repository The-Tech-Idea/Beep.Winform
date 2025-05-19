using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // ScrollList Fonts & Colors

        public TypographyStyle ScrollListTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Bold);
        public TypographyStyle ScrollListSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public TypographyStyle ScrollListUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);

        public Color ScrollListBackColor { get; set; } = Color.FromArgb(18, 18, 32);                  // Cyberpunk Black
        public Color ScrollListForeColor { get; set; } = Color.FromArgb(0, 255, 255);                // Neon Cyan
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(255, 0, 255);              // Neon Magenta

        public Color ScrollListItemForeColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon Cyan
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);       // Neon Yellow
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(34, 34, 68);        // Cyberpunk Panel

        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);    // Neon Magenta
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128);  // Neon Green

        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon Cyan

        public TypographyStyle ScrollListIItemFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);
        public TypographyStyle ScrollListItemSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
    }
}
