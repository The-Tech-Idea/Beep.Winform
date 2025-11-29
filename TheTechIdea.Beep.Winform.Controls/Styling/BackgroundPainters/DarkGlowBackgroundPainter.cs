using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Dark Glow background painter - dark base with neon glow rings
    /// 3-ring inset glow effect with state-modulated intensity
    /// </summary>
    public static class DarkGlowBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Dark Glow: very dark base
            Color darkColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.DarkGlow);
            Color glowColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.DarkGlow);

            // Solid dark background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, darkColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // Glow intensity modulation based on state
            float glowMultiplier = state switch
            {
                ControlState.Hovered => 1.3f,   // Intensify on hover
                ControlState.Pressed => 0.7f,   // Dim on press
                ControlState.Selected => 1.2f,  // Moderate boost on select
                ControlState.Focused => 1.1f,   // Slight boost on focus
                ControlState.Disabled => 0.4f,  // Fade on disabled
                _ => 1.0f
            };

            // 3-ring neon glow effect (inset rings)
            DrawGlowRing(g, path, glowColor, 1, 80, glowMultiplier);
            DrawGlowRing(g, path, glowColor, 3, 40, glowMultiplier);
            DrawGlowRing(g, path, glowColor, 6, 20, glowMultiplier);
        }

        private static void DrawGlowRing(Graphics g, GraphicsPath path, Color glowColor, 
            int inset, int baseAlpha, float multiplier)
        {
            using (var glowPath = GraphicsExtensions.CreateInsetPath(path, inset))
            {
                if (glowPath.PointCount > 0)
                {
                    int alpha = Math.Min(255, (int)(baseAlpha * multiplier));
                    var pen = PaintersFactory.GetPen(Color.FromArgb(alpha, glowColor), 1f);
                    g.DrawPath(pen, glowPath);
                }
            }
        }
    }
}
