using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.RoyalBlue;            // Default link color
        public Color VisitedLinkColor { get; set; } = Color.MediumPurple;  // Visited link
        public Color HoverLinkColor { get; set; } = Color.DeepSkyBlue;     // On hover
        public Color LinkHoverColor { get; set; } = Color.CornflowerBlue;  // Alternate hover (for UI effects)
    }
}
