using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Tokyo Night theme background painter - neon-accented dark theme
    /// Deep dark background with blue/purple neon glow effects and scanlines
    /// </summary>
    public static class TokyoBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Tokyo Night's signature deep blue-black
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x1A, 0x1B, 0x27);
            Color accent = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0x7A, 0xA2, 0xF7); // Tokyo Night blue

            // Paint solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Top glow effect (neon ambient light)
            int glowHeight = Math.Min(80, (int)(bounds.Height * 0.4f));
            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                if (glowHeight > 5)
                {
                    var glowRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, glowHeight);
                    var glow = PaintersFactory.GetLinearGradientBrush(
                        glowRect,
                        Color.FromArgb(25, accent),
                        Color.Transparent,
                        LinearGradientMode.Vertical);
                    g.FillRectangle(glow, glowRect);
                }

                // Neon accent line at top
                var neonPen = PaintersFactory.GetPen(Color.FromArgb(60, accent), 1.5f);
                g.DrawLine(neonPen, bounds.Left, bounds.Top + 0.5f, bounds.Right, bounds.Top + 0.5f);
            }

            // Subtle scanlines (cyberpunk feel)
            var rectBounds = Rectangle.Round(bounds);
            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                BackgroundPainterHelpers.PaintScanlineOverlay(g, rectBounds, 
                    Color.FromArgb(8, Color.White), 4);
            }
        }
    }
}
