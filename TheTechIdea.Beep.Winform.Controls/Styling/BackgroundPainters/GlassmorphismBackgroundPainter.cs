using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Glassmorphism background painter - frosted glass UI trend
    /// Semi-transparent background with blur simulation and top highlight
    /// </summary>
    public static class GlassmorphismBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Glassmorphism: semi-transparent dark base
            Color baseColor = useThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.FromArgb(30, 40, 60);

            // Use enhanced blur for better glassmorphism effect
            int blurRadius = 16; // Medium blur for glassmorphism
            BackgroundPainterHelpers.PaintAdvancedBlurBackground(g, path, baseColor, blurRadius, state);
            
            // Also apply frosted glass effect for additional realism
            int baseAlpha = state == ControlState.Disabled ? 120 : 200;
            BackgroundPainterHelpers.PaintFrostedGlassBackground(g, path, baseColor, baseAlpha, state);

            // Radial gradients enhance glass effect - add subtle radial highlight
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.06f);
                Color edgeColor = baseColor;
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
        }
    }
}
