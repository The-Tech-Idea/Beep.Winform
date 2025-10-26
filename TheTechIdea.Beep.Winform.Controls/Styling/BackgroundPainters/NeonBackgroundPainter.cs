using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Neon background painter - Cyberpunk neon aesthetic
    /// Very dark background (#0A0A14) with intense cyan glow (#00FFFF)
    /// 24px blur, Rajdhani font, maximum neon intensity
    /// </summary>
    public static class NeonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color veryDark = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Neon);
            Color cyanGlow = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Neon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(veryDark, 0.15f),
                ControlState.Pressed => ColorUtils.Lighten(veryDark, 0.2f),
                ControlState.Selected => Color.FromArgb(60, cyanGlow),
                ControlState.Disabled => veryDark,
                _ => veryDark
            };

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = path.GetBounds();
            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Pink glow from top
                var topGlowRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, Math.Min(80f, bounds.Height));
                using (var pinkGlow = new LinearGradientBrush(
                           topGlowRect,
                           Color.FromArgb(40, 255, 0, 150),
                           Color.FromArgb(0, 255, 0, 150),
                           LinearGradientMode.Vertical))
                {
                    g.FillRectangle(pinkGlow, topGlowRect);
                }

                // Cyan glow from left
                var leftGlowRect = new RectangleF(bounds.Left, bounds.Top, Math.Min(100f, bounds.Width), bounds.Height);
                using (var cyanGlow1 = new LinearGradientBrush(
                           leftGlowRect,
                           Color.FromArgb(30, 0, 255, 255),
                           Color.FromArgb(0, 0, 255, 255),
                           LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(cyanGlow1, leftGlowRect);
                }

                // Neon outline lines (top and left)
                using (var neonPinkPen = new Pen(Color.FromArgb(150, 255, 0, 200), 2))
                {
                    g.DrawLine(neonPinkPen, bounds.Left, bounds.Top + 0.5f, bounds.Right, bounds.Top + 0.5f);
                }

                using (var neonCyanPen = new Pen(Color.FromArgb(150, 0, 255, 255), 2))
                {
                    g.DrawLine(neonCyanPen, bounds.Left + 0.5f, bounds.Top, bounds.Left + 0.5f, bounds.Bottom);
                }
            }

            // Neon: Intense cyan glow overlay
            if (state == ControlState.Hovered || state == ControlState.Selected)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(50, cyanGlow)))
                {
                    g.FillPath(glowBrush, path);
                }
            }
        }
    }
}
