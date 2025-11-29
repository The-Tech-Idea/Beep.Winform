using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Terminal shadow painter - No shadow (flat console aesthetic)
    /// Terminal/console UIs are traditionally flat
    /// Returns path unchanged intentionally
    /// </summary>
    public static class TerminalShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Terminal: Intentionally no shadow for authentic console look
            return path;
        }
    }
}
