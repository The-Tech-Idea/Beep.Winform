using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(187, 187, 187); // Light Gray

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Very Light Gray
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(230, 230, 230); // Light Gray
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(200, 200, 200); // Gray
    }
}
