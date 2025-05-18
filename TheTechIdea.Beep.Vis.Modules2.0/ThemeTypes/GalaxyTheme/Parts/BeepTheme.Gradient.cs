using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(0x1A, 0x1A, 0x2E); // PrimaryColor
        public Color GradientEndColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
