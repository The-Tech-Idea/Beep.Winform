using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Link (TextBox Link) colors

        // Standard link: Soft blue (playful and classic)
        public Color LinkColor { get; set; } = Color.FromArgb(54, 162, 235);        // Soft Blue

        // Visited link: Candy pink (makes it clear and fits theme)
        public Color VisitedLinkColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Hover link: Mint (lively feedback)
        public Color HoverLinkColor { get; set; } = Color.FromArgb(127, 255, 212);   // Mint

        // Alt hover (stronger pop, for underlines or outlines)
        public Color LinkHoverColor { get; set; } = Color.FromArgb(255, 182, 193);   // Light Pink
    }
}
