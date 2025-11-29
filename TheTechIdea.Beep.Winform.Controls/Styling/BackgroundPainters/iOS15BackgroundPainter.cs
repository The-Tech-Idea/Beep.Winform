using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
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

            var baseBrush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(baseBrush, path);

            // iOS blur effect: translucent white overlay with state-aware intensity
            if (state != ControlState.Disabled)
            {
                int overlayAlpha = Math.Min(255, 30 + translucencyBoost);
                var overlayBrush = PaintersFactory.GetSolidBrush(
                    Color.FromArgb(overlayAlpha, Color.White));
                g.FillPath(overlayBrush, path);
            }
        }
    }
}
