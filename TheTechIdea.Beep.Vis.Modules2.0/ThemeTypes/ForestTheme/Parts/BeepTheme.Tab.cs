using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public Color TabBackColor { get; set; } = Color.FromArgb(34, 49, 34); // Dark forest green
        public Color TabForeColor { get; set; } = Color.LightGreen;
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(46, 139, 87); // SeaGreen
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(24, 39, 24); // Very dark green
        public Color InactiveTabForeColor { get; set; } = Color.Gray;
        public Color TabBorderColor { get; set; } = Color.DarkGreen;
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(60, 179, 113); // MediumSeaGreen
        public Color TabHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(46, 139, 87);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.LimeGreen;
        public Color TabHoverBorderColor { get; set; } = Color.LightGreen;
    }
}
