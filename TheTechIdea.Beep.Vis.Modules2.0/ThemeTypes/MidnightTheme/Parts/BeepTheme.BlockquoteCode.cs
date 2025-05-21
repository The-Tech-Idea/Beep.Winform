using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Blockquote, InlineCode, CodeBlock
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue

        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(30, 30, 30); // very dark gray
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(45, 45, 45); // dark gray
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
    }
}
