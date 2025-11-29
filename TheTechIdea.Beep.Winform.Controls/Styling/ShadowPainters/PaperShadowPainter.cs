using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Paper shadow painter - Soft paper-lift shadow
    /// Creates subtle "paper lifted from surface" effect
    /// Light, clean aesthetic
    /// </summary>
    public static class PaperShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color shadowColor = StyleShadows.GetShadowColor(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Paper: Soft, light shadow
            int alpha = state switch
            {
                ControlState.Hovered => 45,
                ControlState.Pressed => 22,
                ControlState.Focused => 40,
                ControlState.Disabled => 12,
                _ => 32
            };

            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius, 0, offsetY, shadowColor, alpha, 2);
        }
    }
}
