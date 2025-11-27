using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    public static class NeonPathPainter
    {
        /// <summary>
        /// Neon path painter with glow effect.
        /// Paints a bright neon fill and multiple soft strokes around the path to simulate outer glow.
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            // Base neon color from style
            Color baseNeon = StyleColors.GetPrimary(BeepControlStyle.Neon);

            // Slightly adjust for state
            baseNeon = state switch
            {
                ControlState.Hovered => PathPainterHelpers.Lighten(baseNeon, 0.1f),
                ControlState.Pressed => PathPainterHelpers.Darken(baseNeon, 0.1f),
                ControlState.Disabled => PathPainterHelpers.WithAlpha(baseNeon, 100),
                _ => baseNeon
            };

            RectangleF bounds = path.GetBounds();
            float size = Math.Min(bounds.Width, bounds.Height);
            if (size <= 0) size = 16f;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw multiple expanding strokes with decreasing alpha to fake glow
            for (int i = 4; i >= 1; i--)
            {
                int alpha = 50 + (i * 30); // 170..50
                float width = (size / 20f) + i * 1.5f;

                Color glowColor = Color.FromArgb(
                    Math.Min(255, alpha),
                    baseNeon.R,
                    baseNeon.G,
                    baseNeon.B);

                using (var pen = new Pen(glowColor, width))
                {
                    pen.LineJoin = LineJoin.Round;
                    // Center strokes on the path; thicker pens will extend both inwards and outwards.
                    g.DrawPath(pen, path);
                }
            }

            // Solid inner fill
            PathPainterHelpers.PaintSolidPath(g, path, baseNeon, state);
        }
    }
}
