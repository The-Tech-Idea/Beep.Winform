using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Tailwind Card background painter - utility-first card design
    /// Subtle vertical gradient (darker at bottom) for depth
    /// </summary>
    public static class TailwindCardBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Tailwind: clean white
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.TailwindCard);

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

            // Get state-adjusted color
            Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                baseColor, state, BackgroundPainterHelpers.StateIntensity.Normal);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Tailwind card gradient: 5% darker at bottom for depth - use HSL for more natural results
            Color bottomColor = ColorAccessibilityHelper.DarkenColor(stateColor, 0.05f);
            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, stateColor, bottomColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }
    }
}
