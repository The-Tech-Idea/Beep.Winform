using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Glow background painter - dark base with neon inner glow
    /// Multi-ring inset glow effect for dramatic appearance
    /// </summary>
    public static class GlowBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, 
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Glow: dark background
            Color bgColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            Color glowColor = BackgroundPainterHelpers.GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);

            // Solid dark background with state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, bgColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // State-modulated glow intensity
            float glowMultiplier = state switch
            {
                ControlState.Hovered => 1.3f,
                ControlState.Pressed => 0.7f,
                ControlState.Selected => 1.2f,
                ControlState.Focused => 1.1f,
                ControlState.Disabled => 0.4f,
                _ => 1.0f
            };

            // Multi-ring inset glow effect
            int glowSize = 3;
            for (int i = glowSize; i > 0; i--)
            {
                int baseAlpha = (int)(30 * ((float)(glowSize - i + 1) / glowSize));
                int alpha = Math.Min(255, (int)(baseAlpha * glowMultiplier));
                Color glowStep = Color.FromArgb(alpha, glowColor);

                using (var innerPath = GraphicsExtensions.CreateInsetPath(path, i))
                {
                    if (innerPath.PointCount > 0)
                    {
                        var glowPen = PaintersFactory.GetPen(glowStep, 2f);
                        g.DrawPath(glowPen, innerPath);
                    }
                }
            }
        }
    }
}
