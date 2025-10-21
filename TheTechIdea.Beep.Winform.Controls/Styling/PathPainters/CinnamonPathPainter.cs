using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    public static class CinnamonPathPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, ControlState state = ControlState.Normal)
        {
            Color mintGreen = StyleColors.GetPrimary(BeepControlStyle.Cinnamon);
            PathPainterHelpers.PaintSolidPath(g, path, mintGreen);
        }
    }
}
