using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(160, 180, 200); // Subtle blue-gray

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(220, 225, 235); // Light bluish-gray
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(240, 245, 250); // Very light blue
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Soft border
    }
}
