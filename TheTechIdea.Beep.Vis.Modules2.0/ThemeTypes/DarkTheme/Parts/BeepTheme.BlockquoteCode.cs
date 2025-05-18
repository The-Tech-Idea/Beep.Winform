using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(100, 100, 100); // subtle gray border

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(40, 40, 40); // dark background for inline code
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(30, 30, 30); // darker block background
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(80, 80, 80); // muted border for code block
    }
}
