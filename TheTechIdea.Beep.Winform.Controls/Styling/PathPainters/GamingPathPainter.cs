using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    public static class GamingPathPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, ControlState state = ControlState.Normal)
        {
            Color neonGreen = StyleColors.GetPrimary(BeepControlStyle.Gaming);
            g.SmoothingMode = SmoothingMode.None; // Angular
            PathPainterHelpers.PaintSolidPath(g, path, neonGreen);
        }
    }
}
