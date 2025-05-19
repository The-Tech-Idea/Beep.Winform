using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for blockquote border
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for inline code background
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for code block background
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for code block border
    }
}