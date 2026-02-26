using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Linear style background painter - Clean, fast, modern SaaS look
    /// Inspired by Linear.app design system
    /// </summary>
    public static class LinearBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Linear: ultra-clean, minimal backgrounds
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : Color.FromArgb(255, 255, 255);

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

            // Linear uses very subtle state changes
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // Linear style: very subtle inner border for definition when border pass is suppressed
            var bounds = path.GetBounds();
            if (BackgroundPainterHelpers.ShouldPaintDecorativeEdgeStroke(style) && bounds.Width > 2 && bounds.Height > 2)
            {
                using (var insetPath = path.CreateInsetPath(1))
                {
                    if (insetPath != null && insetPath.PointCount > 0)
                    {
                        try
                        {
                            var borderPen = PaintersFactory.GetPen(Color.FromArgb(8, Color.Black), 1f);
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.DrawPath(borderPen, insetPath);
                        }
                        finally
                        {
                            insetPath?.Dispose();
                        }
                    }
                }
            }
        }
    }
}
