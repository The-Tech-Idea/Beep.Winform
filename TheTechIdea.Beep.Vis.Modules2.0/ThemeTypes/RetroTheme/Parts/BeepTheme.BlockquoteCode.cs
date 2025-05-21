using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(32, 32, 32);
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(96, 96, 96);
    }
}