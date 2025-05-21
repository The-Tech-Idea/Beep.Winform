using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(100, 149, 237); // CornflowerBlue

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(25, 25, 112); // MidnightBlue
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(0, 0, 64); // Dark Navy
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
    }
}
