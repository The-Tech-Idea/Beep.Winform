using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color GradientEndColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for sleek flow
    }
}