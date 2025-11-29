using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Gradient Modern shadow painter - Deep shadows for gradient backgrounds
    /// Rich, prominent shadows that complement gradient surfaces
    /// State and elevation aware
    /// </summary>
    public static class GradientModernShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Gradient Modern uses more prominent shadows
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Base alpha higher for gradient backgrounds
            int baseAlpha = (int)elevation switch
            {
                0 => 35,
                1 => 45,
                2 => 55,
                3 => 65,
                4 => 75,
                _ => 85
            };

            // State-based adjustments
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.4f),
                ControlState.Pressed => (int)(baseAlpha * 0.6f),
                ControlState.Focused => (int)(baseAlpha * 1.3f),
                ControlState.Disabled => (int)(baseAlpha * 0.4f),
                _ => baseAlpha
            };

            int spread = Math.Max(3, (int)elevation + 2);

            // Use clean drop shadow (more prominent for gradients)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread);
        }
    }
}
