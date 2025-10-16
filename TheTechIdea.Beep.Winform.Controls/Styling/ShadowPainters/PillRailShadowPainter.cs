using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Pill rail shadow painter - no shadow (placeholder for consistency)
    /// Pill controls typically don't use shadows for a clean look
    /// </summary>
    public static class PillRailShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal,
            MaterialElevation elevation = MaterialElevation.Level0)
        {
            // Pill rail style has no shadows for clean flat appearance
            if (!StyleShadows.HasShadow(style))
                return path;

            GraphicsPath remainingPath = (GraphicsPath)path.Clone();

            return remainingPath;
        }
    }
}
