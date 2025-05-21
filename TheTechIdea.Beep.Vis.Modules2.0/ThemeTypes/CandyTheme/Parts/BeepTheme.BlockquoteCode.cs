using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Blockquote, InlineCode, CodeBlock

        // Blockquote: Light lavender border for gentle accent
        public Color BlockquoteBorderColor { get; set; } = Color.FromArgb(206, 183, 255); // Pastel Lavender

        // Inline code: Mint green background
        public Color InlineCodeBackgroundColor { get; set; } = Color.FromArgb(204, 255, 240); // Pastel Mint

        // Code block: Baby blue background, with a bubblegum-pink border
        public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb(210, 235, 255); // Pastel Blue
        public Color CodeBlockBorderColor { get; set; } = Color.FromArgb(255, 182, 193); // Candy Pink
    }
}
