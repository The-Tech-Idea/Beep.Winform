using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Microsoft Fluent 2 shadow painter
    /// Modern soft shadows with subtle state feedback
    /// Refined elevation for contemporary Windows design
    /// </summary>
    public static class Fluent2ShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fluent 2 shadow - neutral, modern
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Fluent 2 state-based shadow intensity
            int alpha = state switch
            {
                ControlState.Hovered => 55,    // More prominent
                ControlState.Pressed => 25,    // Reduced
                ControlState.Focused => 50,    // Moderate
                ControlState.Selected => 60,   // Most visible
                ControlState.Disabled => 12,   // Minimal
                _ => 40                        // Default modern subtle
            };

            int spread = 2;

            // Use clean drop shadow (Fluent 2 modern style)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread);
        }
    }
}
