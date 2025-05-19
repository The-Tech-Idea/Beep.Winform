using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Tree Fonts & Colors
        public TypographyStyle TreeTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public TypographyStyle TreeNodeSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle TreeNodeUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Regular);

        public Color TreeBackColor { get; set; } = Color.FromArgb(18, 18, 32);                  // Dark background
        public Color TreeForeColor { get; set; } = Color.FromArgb(0, 255, 255);                  // Neon cyan text
        public Color TreeBorderColor { get; set; } = Color.FromArgb(255, 0, 255);                // Neon magenta border

        public Color TreeNodeForeColor { get; set; } = Color.FromArgb(0, 255, 128);              // Neon green node text
        public Color TreeNodeHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);         // Neon yellow hover text
        public Color TreeNodeHoverBackColor { get; set; } = Color.FromArgb(34, 34, 68);          // Slightly lighter background on hover

        public Color TreeNodeSelectedForeColor { get; set; } = Color.White;                      // White selected text
        public Color TreeNodeSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);      // Neon magenta selected background

        public Color TreeNodeCheckedBoxForeColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon cyan checkbox
        public Color TreeNodeCheckedBoxBackColor { get; set; } = Color.FromArgb(18, 18, 32);     // Dark background for checkbox
    }
}
