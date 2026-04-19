using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color GradientEndColor { get; set; } = Color.FromArgb(247, 221, 229);
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}