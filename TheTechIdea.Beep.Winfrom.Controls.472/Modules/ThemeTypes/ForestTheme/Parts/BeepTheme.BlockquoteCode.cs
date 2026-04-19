using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(46, 139, 87); // SeaGreen
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(224, 255, 255); // LightCyan
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(240, 255, 240); // Honeydew
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(34, 139, 34); // ForestGreen
    }
}
