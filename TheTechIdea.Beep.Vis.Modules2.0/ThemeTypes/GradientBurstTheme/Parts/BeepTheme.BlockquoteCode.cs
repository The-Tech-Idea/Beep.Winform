using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(76, 175, 80);   // Green

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(232, 234, 246); // Light Indigo
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245); // Light Gray
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(120, 144, 156); // Blue Gray
    }
}
