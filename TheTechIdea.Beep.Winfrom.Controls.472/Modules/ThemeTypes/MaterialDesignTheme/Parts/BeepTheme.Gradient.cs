using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Color GradientEndColor { get; set; } = Color.FromArgb(30, 136, 229);   // Material Blue 600
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
