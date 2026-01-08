using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Radix UI background painter - Accessible, modern design system
    /// Inspired by Radix UI primitives
    /// </summary>
    public static class RadixUIBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Radix UI: clean, accessible backgrounds
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : Color.FromArgb(255, 255, 255);

            // Radix UI uses normal state changes with accessibility in mind
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);

            // Radix UI emphasizes accessibility - ensure proper contrast
            if (useThemeColors && theme != null)
            {
                // Ensure background meets contrast requirements
                var accessibleColor = TheTechIdea.Beep.Winform.Controls.Styling.Helpers.ColorAccessibilityHelper
                    .EnsureContrastRatio(baseColor, theme.ForeColor, 
                        TheTechIdea.Beep.Winform.Controls.Styling.Helpers.ColorAccessibilityHelper.WCAG_AA_Normal);
                
                if (accessibleColor != baseColor)
                {
                    baseColor = accessibleColor;
                }
            }

            // Radix UI could add subtle radial highlights for depth
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0 && (state == ControlState.Hovered || state == ControlState.Focused))
            {
                Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.02f);
                Color edgeColor = baseColor;
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
            else
            {
                // Fallback to solid if no radial needed
                var brush = PaintersFactory.GetSolidBrush(baseColor);
                g.FillPath(brush, path);
            }
        }
    }
}
