using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Shadcn/ui background painter - Clean, minimal, modern design system
    /// Inspired by shadcn/ui component library
    /// </summary>
    public static class ShadcnBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Shadcn: clean white/light background with subtle state changes
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : Color.White;

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

            // Shadcn uses subtle state changes
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // Add very subtle border highlight for depth when no border pass will follow
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                if (BackgroundPainterHelpers.ShouldPaintDecorativeEdgeStroke(style))
                {
                    var borderBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(10, Color.Black));
                    using (var pen = new Pen(borderBrush, 1f))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawPath(pen, path);
                    }
                }

                // Shadcn could add subtle radial highlights for depth
                if (state == ControlState.Hovered || state == ControlState.Focused)
                {
                    Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.02f);
                    Color edgeColor = baseColor;
                    BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
                }
            }
        }
    }
}
