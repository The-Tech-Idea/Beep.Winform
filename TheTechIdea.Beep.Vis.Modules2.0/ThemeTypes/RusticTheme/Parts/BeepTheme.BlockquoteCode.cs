using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // SaddleBrown
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(240, 235, 210); // Light Beige
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
    }
}