using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Minimal shadow painter - Very subtle, almost invisible shadow
    /// For minimal/flat designs that need just a hint of depth
    /// State-aware but always extremely subtle
    /// </summary>
    public static class MinimalShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            // Minimal: Only show shadow on interaction
            if (state == ControlState.Normal || state == ControlState.Disabled)
                return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int alpha = state switch
            {
                ControlState.Hovered => 18,
                ControlState.Pressed => 8,
                ControlState.Focused => 15,
                _ => 0
            };

            if (alpha == 0) return path;

            return ShadowPainterHelpers.PaintSubtleShadow(
                g, path, radius, 1, alpha);
        }
    }
}
