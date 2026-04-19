using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(255, 204, 102); // warm orange
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(255, 249, 196); // light yellow
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(255, 243, 212); // soft yellow
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(255, 204, 102); // warm orange
    }
}
