using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color GradientEndColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}