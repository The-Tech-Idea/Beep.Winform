using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(240, 240, 245); // Light silver
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // Dark goldenrod
    }
}