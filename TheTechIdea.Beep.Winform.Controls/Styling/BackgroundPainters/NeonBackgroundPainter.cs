using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Neon background painter - cyberpunk neon aesthetic
    /// Very dark background with intense dual-color glow effects
    /// Pink glow from top, cyan glow from left
    /// </summary>
    public static class NeonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Neon: very dark purple-black
            Color veryDark = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : StyleColors.GetBackground(BeepControlStyle.Neon);
            Color cyanGlow = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.Neon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // State-adjusted fill with strong feedback
            Color fillColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                veryDark, state, BackgroundPainterHelpers.StateIntensity.Strong);

            var fillBrush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(fillBrush, path);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Pink glow from top
                float topGlowHeight = Math.Min(80f, bounds.Height * 0.4f);
                if (topGlowHeight > 5)
                {
                    var topGlowRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, topGlowHeight);
                    var pinkGlow = PaintersFactory.GetLinearGradientBrush(
                        topGlowRect, 
                        Color.FromArgb(35, 255, 0, 150), 
                        Color.FromArgb(0, 255, 0, 150), 
                        LinearGradientMode.Vertical);
                    g.FillRectangle(pinkGlow, topGlowRect);
                }

                // Cyan glow from left
                float leftGlowWidth = Math.Min(100f, bounds.Width * 0.4f);
                if (leftGlowWidth > 5)
                {
                    var leftGlowRect = new RectangleF(bounds.Left, bounds.Top, leftGlowWidth, bounds.Height);
                    var cyanGlow1 = PaintersFactory.GetLinearGradientBrush(
                        leftGlowRect, 
                        Color.FromArgb(25, 0, 255, 255), 
                        Color.FromArgb(0, 0, 255, 255), 
                        LinearGradientMode.Horizontal);
                    g.FillRectangle(cyanGlow1, leftGlowRect);
                }

                // Neon accent lines (top pink, left cyan)
                var neonPinkPen = PaintersFactory.GetPen(Color.FromArgb(120, 255, 0, 200), 1.5f);
                g.DrawLine(neonPinkPen, bounds.Left, bounds.Top + 0.5f, bounds.Right, bounds.Top + 0.5f);

                var neonCyanPen = PaintersFactory.GetPen(Color.FromArgb(120, 0, 255, 255), 1.5f);
                g.DrawLine(neonCyanPen, bounds.Left + 0.5f, bounds.Top, bounds.Left + 0.5f, bounds.Bottom);
            }

            // Intensified glow overlay on hover/selected
            if (state == ControlState.Hovered || state == ControlState.Selected)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, cyanGlow));
                g.FillPath(glowBrush, path);
            }
        }
    }
}
