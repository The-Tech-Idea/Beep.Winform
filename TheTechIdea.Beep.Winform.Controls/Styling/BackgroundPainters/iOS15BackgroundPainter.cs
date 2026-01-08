using System;
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
    /// iOS 15 background painter - translucent blur-inspired design
    /// Clean surface with white translucent overlay (blur simulation)
    /// State changes affect overlay intensity, not base color
    /// </summary>
    public static class iOS15BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // iOS 15: clean white/light surface
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.iOS15);

            // iOS state handling affects translucency, not base color
            int translucencyBoost = state switch
            {
                ControlState.Hovered => 25,   // Subtle blur increase
                ControlState.Pressed => 40,   // Noticeable blur increase
                ControlState.Selected => 35,  // Selection blur
                ControlState.Focused => 15,   // Gentle focus blur
                _ => 0
            };

            // Handle disabled state specially
            Color fillColor = state == ControlState.Disabled 
                ? BackgroundPainterHelpers.WithAlpha(baseColor, 60)
                : baseColor;

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // iOS 15 uses multi-stop gradients for smoother iOS-style transitions
            // Create multi-stop gradient for authentic iOS look
            var stops = new[]
            {
                (0.0f, fillColor), // Top
                (0.3f, ColorAccessibilityHelper.LightenColor(fillColor, 0.02f)), // Upper
                (0.7f, fillColor), // Middle
                (1.0f, ColorAccessibilityHelper.DarkenColor(fillColor, 0.01f))  // Bottom
            };

            // Use multi-stop gradient helper for smoother iOS-style transitions
            BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Subtle);

            // iOS blur effect: translucent white overlay with state-aware intensity
            if (state != ControlState.Disabled)
            {
                int overlayAlpha = Math.Min(255, 30 + translucencyBoost);
                var overlayBrush = PaintersFactory.GetSolidBrush(
                    Color.FromArgb(overlayAlpha, Color.White));
                g.FillPath(overlayBrush, path);

                // iOS buttons often have radial highlights - add for button states
                if (state == ControlState.Hovered || state == ControlState.Focused)
                {
                    Color centerColor = ColorAccessibilityHelper.LightenColor(fillColor, 0.04f);
                    Color edgeColor = fillColor;
                    BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
                }
            }
        }
    }
}
