using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for blockquote border
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for inline code background
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for code block background
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for code block border
    }
}