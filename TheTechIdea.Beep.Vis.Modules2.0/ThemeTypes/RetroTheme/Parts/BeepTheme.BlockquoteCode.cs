using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow for blockquote border
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for inline code background
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for code block background
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan for code block border
    }
}