using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Pill Rail shadow painter - Toggle/pill button shadow
    /// Very subtle shadow for pill-shaped controls
    /// Minimal depth indication
    /// </summary>
    public static class PillRailShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Pill/toggle: Minimal shadow
            int alpha = state switch
            {
                ControlState.Hovered => 30,
                ControlState.Pressed => 15,
                ControlState.Focused => 28,
                ControlState.Disabled => 8,
                _ => 20
            };

            return ShadowPainterHelpers.PaintSubtleShadow(
                g, path, radius, 1, alpha);
        }
    }
}
