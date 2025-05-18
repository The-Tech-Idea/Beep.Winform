using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(0, 123, 255); // Bright blue for accent

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(240, 240, 240); // Light gray background for inline code

        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(30, 30, 30); // Dark background for code block (modern dark mode style)

        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(70, 70, 70); // Subtle border for code block
    }
}
