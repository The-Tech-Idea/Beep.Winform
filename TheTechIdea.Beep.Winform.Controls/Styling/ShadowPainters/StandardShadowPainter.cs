using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Standard shadow painter - Default fallback shadow
    /// Used when no specific style shadow painter is available
    /// Clean, neutral, state-aware
    /// </summary>
    public static class StandardShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Standard: Good default shadow
            int alpha = state switch
            {
                ControlState.Hovered => 50,
                ControlState.Pressed => 25,
                ControlState.Focused => 45,
                ControlState.Disabled => 15,
                _ => 35
            };

            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius, 0, offsetY, shadowColor, alpha, 2);
        }
    }
}
