using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Cyberpunk background painter - futuristic neon aesthetic
    /// Very dark background with pink gradient glow and cyan accent outline
    /// </summary>
    public static class CyberpunkBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Cyberpunk: almost black
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(10, 10, 12);
            Color neon = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0, 255, 200);

            // Solid background with strong state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Pink gradient glow from left edge
                float glowWidth = Math.Min(100f, bounds.Width * 0.4f);
                if (glowWidth > 5)
                {
                    var leftRect = new RectangleF(bounds.Left, bounds.Top, glowWidth, bounds.Height);
                    var leftGrad = PaintersFactory.GetLinearGradientBrush(
                        leftRect,
                        Color.FromArgb(50, 255, 0, 150),
                        Color.Transparent,
                        LinearGradientMode.Horizontal);
                    g.FillRectangle(leftGrad, leftRect);
                }

                if (BackgroundPainterHelpers.ShouldPaintDecorativeEdgeStroke(style))
                {
                    // Neon accent outline
                    var pen = PaintersFactory.GetPen(Color.FromArgb(100, neon), 1.5f);
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}
