using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Link (TextBox Link) colors
        public Color LinkColor { get; set; } = Color.Blue;
        public Color VisitedLinkColor { get; set; } = Color.Purple;
        public Color HoverLinkColor { get; set; } = Color.DarkBlue;
        public Color LinkHoverColor { get; set; } = Color.DarkBlue;  // Typically same as HoverLinkColor
    }
}
