using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

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

            // Slightly adjust for state - use HSL for more natural neon glow
            baseNeon = state switch
            {
                ControlState.Hovered => ColorAccessibilityHelper.LightenColor(baseNeon, 0.1f),
                ControlState.Pressed => ColorAccessibilityHelper.DarkenColor(baseNeon, 0.1f),
                ControlState.Disabled => PathPainterHelpers.WithAlpha(baseNeon, 100),
                _ => baseNeon
            };

            RectangleF bounds = path.GetBounds();
            float size = Math.Min(bounds.Width, bounds.Height);
            if (size <= 0) size = 16f;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw multiple expanding strokes with decreasing alpha to fake glow
            // Draw from widest (faintest) to narrowest (brightest)
            for (int i = 4; i >= 1; i--)
            {
                // Adjusted alpha logic: Outer layers (larger i) should be fainter
                // i=4 (Widest): Alpha ~40
                // i=1 (Narrowest): Alpha ~160
                int alpha = (int)(200 * (1.0f - (i / 5.0f))); 
                
                float width = (size / 20f) + i * 2.0f;

                Color glowColor = Color.FromArgb(
                    Math.Min(255, Math.Max(0, alpha)),
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
