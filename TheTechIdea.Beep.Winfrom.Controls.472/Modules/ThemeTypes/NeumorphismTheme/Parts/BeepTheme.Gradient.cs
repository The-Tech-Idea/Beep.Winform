using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(240, 240, 245); // Light gray for gradient start
        public Color GradientEndColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray for gradient end
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical; // Vertical for soft neumorphic effect
    }
}