using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    public static class NeonPathPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, ControlState state = ControlState.Normal)
        {
            Color cyanGlow = StyleColors.GetPrimary(BeepControlStyle.Neon);
            PathPainterHelpers.PaintSolidPath(g, path, cyanGlow);
        }
    }
}
