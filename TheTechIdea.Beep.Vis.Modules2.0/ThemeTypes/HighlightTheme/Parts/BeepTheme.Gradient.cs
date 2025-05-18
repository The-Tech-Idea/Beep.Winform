using System.Drawing.Drawing2D;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Gradient Properties
        public Color GradientStartColor { get; set; } = Color.FromArgb(255, 250, 210); // LightGoldenRodYellow
        public Color GradientEndColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
