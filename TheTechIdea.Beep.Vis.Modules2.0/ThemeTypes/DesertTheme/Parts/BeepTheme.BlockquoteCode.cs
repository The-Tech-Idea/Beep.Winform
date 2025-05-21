using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Blockquote, InlineCode, CodeBlock colors with warm desert tones
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(210, 180, 140); // Tan

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(245, 222, 179); // Wheat

        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(244, 164, 96); // Sandy Brown
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(205, 133, 63); // Peru
    }
}
