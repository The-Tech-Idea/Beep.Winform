using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for blockquote border
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(220, 220, 225); // Light gray for inline code background
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(220, 220, 225); // Light gray for code block background
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for code block border
    }
}