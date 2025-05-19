using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink for gradient start
        public Color GradientEndColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for gradient end
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft pastel effect
    }
}