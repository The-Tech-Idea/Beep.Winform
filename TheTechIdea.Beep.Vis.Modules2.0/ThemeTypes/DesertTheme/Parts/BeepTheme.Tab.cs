using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Tab Fonts & Colors
        public TypographyStyle TabFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public TypographyStyle TabHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle TabSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(245, 222, 179); // Wheat
        public Color TabForeColor { get; set; } = Color.FromArgb(101, 67, 33);   // Dark Brown

        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(255, 228, 181); // Moccasin
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(139, 69, 19);   // Saddle Brown

        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(222, 184, 135); // Burlywood
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(160, 82, 45);   // Sienna

        public Color TabBorderColor { get; set; } = Color.FromArgb(210, 180, 140);       // Tan
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(255, 218, 185);    // Peach Puff
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(160, 82, 45);      // Sienna

        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(244, 164, 96);  // Sandy Brown
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(101, 67, 33);   // Dark Brown
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(205, 133, 63);// Peru
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(210, 180, 140);  // Tan
    }
}
