using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// High Contrast shadow painter - No shadow (accessibility)
    /// High contrast modes rely on borders, not shadows
    /// Returns path unchanged for accessibility compliance
    /// </summary>
    public static class HighContrastShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // High Contrast: No shadows for accessibility
            // Borders provide visual separation instead
            return path;
        }
    }
}
