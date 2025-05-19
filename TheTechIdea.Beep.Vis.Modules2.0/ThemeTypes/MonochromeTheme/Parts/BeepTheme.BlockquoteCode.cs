using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Blockquote, InlineCode, CodeBlock with default monochrome colors
        public Color BlockquoteBorderColor { get; set; } = Color.Gray;

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(45, 45, 45);
        public Color CodeBlockBorderColor { get; set; } = Color.DarkGray;
    }
}
