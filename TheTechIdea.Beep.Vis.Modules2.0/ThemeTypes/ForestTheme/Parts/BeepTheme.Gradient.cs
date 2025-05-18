using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(34, 83, 39); // Dark green start
        public Color GradientEndColor { get; set; } = Color.FromArgb(76, 125, 79); // Medium green end
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
