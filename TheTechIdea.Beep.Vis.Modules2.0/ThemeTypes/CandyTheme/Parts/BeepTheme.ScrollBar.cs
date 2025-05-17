using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // ScrollBar Colors

        // Base background: pastel pink for a sweet look
        public Color ScrollBarBackColor { get; set; } = Color.FromArgb(255, 224, 235);        // Pastel Pink

        // Thumb (draggable handle): candy pink, for high visibility
        public Color ScrollBarThumbColor { get; set; } = Color.FromArgb(240, 100, 180);       // Candy Pink

        // Track (area thumb moves along): mint, for a subtle contrast
        public Color ScrollBarTrackColor { get; set; } = Color.FromArgb(204, 255, 240);       // Mint

        // Hover thumb: lemon yellow, for playful feedback
        public Color ScrollBarHoverThumbColor { get; set; } = Color.FromArgb(255, 223, 93);   // Lemon Yellow

        // Hover track: baby blue, gentle pop under thumb
        public Color ScrollBarHoverTrackColor { get; set; } = Color.FromArgb(210, 235, 255);  // Baby Blue

        // Active thumb (while dragging): soft blue for clear feedback
        public Color ScrollBarActiveThumbColor { get; set; } = Color.FromArgb(54, 162, 235);  // Soft Blue
    }
}
