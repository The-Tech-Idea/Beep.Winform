using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Blockquote, InlineCode, CodeBlock

        // Blockquote: neon blue border, deep purple/black background
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(0, 255, 255);           // Neon Cyan

        // Inline code: magenta background (subtle), black border
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(44, 0, 70);         // Deep Magenta Black

        // Code block: deeper black, magenta neon border
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(12, 12, 32);         // Cyberpunk Darkest
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon Magenta
    }
}
