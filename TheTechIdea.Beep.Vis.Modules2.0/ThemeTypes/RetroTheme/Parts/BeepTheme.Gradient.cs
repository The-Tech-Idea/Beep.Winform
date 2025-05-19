using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal for gradient start
        public Color GradientEndColor { get; set; } = Color.FromArgb(0, 43, 43); // Darker teal for gradient end
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for retro CRT effect
    }
}