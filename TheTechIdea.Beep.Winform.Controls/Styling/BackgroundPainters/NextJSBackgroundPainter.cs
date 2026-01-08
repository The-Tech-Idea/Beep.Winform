using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Next.js App Router background painter - Contemporary web aesthetic
    /// Inspired by Next.js 13+ App Router design patterns
    /// </summary>
    public static class NextJSBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Next.js: modern, clean backgrounds with subtle gradients
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : Color.FromArgb(250, 250, 250);

            // Ensure accessibility compliance for text containers
            if (useThemeColors && theme != null && theme.ForeColor != Color.Empty)
            {
                Color accessibleColor = ColorAccessibilityHelper.EnsureContrastRatio(
                    baseColor, theme.ForeColor, 
                    ColorAccessibilityHelper.WCAG_AA_Normal);
                if (accessibleColor != baseColor)
                {
                    baseColor = accessibleColor;
                }
            }

            // Next.js uses multi-stop gradients for modern feel (smoother transitions)
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                // Create multi-stop gradient for Next.js modern aesthetic
                var stops = new[]
                {
                    (0.0f, baseColor), // Top
                    (0.4f, ColorAccessibilityHelper.LightenColor(baseColor, 0.015f)), // Upper
                    (0.6f, baseColor), // Middle
                    (1.0f, ColorAccessibilityHelper.DarkenColor(baseColor, 0.015f))  // Bottom
                };

                // Use multi-stop gradient helper for smoother transitions
                BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Normal);
            }
            else
            {
                // Fallback to subtle gradient if bounds are invalid
                BackgroundPainterHelpers.PaintSubtleGradientBackground(g, path, baseColor, 
                    0.03f, state, BackgroundPainterHelpers.StateIntensity.Normal);
            }

            // Add very subtle top border for Next.js card style
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                var topBorderRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, 1);
                var borderBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(15, Color.Black));
                g.FillRectangle(borderBrush, topBorderRect);
            }
        }
    }
}
