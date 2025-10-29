using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Dark Glow background painter - Solid dark with 3-ring neon glow
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// Glow intensity modulated by state
    /// </summary>
    public static class DarkGlowBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Dark Glow: Dark background with neon glow rings
            Color darkColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.DarkGlow);

            // INLINE STATE HANDLING - DarkGlow: Subtle background tinting (5% hover, 3% press, 7% selected, 4% focus, 130 alpha disabled)
            darkColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(darkColor.A,
                    Math.Min(255, darkColor.R + (int)(darkColor.R * 0.05)),
                    Math.Min(255, darkColor.G + (int)(darkColor.G * 0.05)),
                    Math.Min(255, darkColor.B + (int)(darkColor.B * 0.05))),
                ControlState.Pressed => Color.FromArgb(darkColor.A,
                    Math.Min(255, darkColor.R + (int)(darkColor.R * 0.03)),
                    Math.Min(255, darkColor.G + (int)(darkColor.G * 0.03)),
                    Math.Min(255, darkColor.B + (int)(darkColor.B * 0.03))),
                ControlState.Selected => Color.FromArgb(darkColor.A,
                    Math.Min(255, darkColor.R + (int)(darkColor.R * 0.07)),
                    Math.Min(255, darkColor.G + (int)(darkColor.G * 0.07)),
                    Math.Min(255, darkColor.B + (int)(darkColor.B * 0.07))),
                ControlState.Disabled => Color.FromArgb(130, darkColor),
                ControlState.Focused => Color.FromArgb(darkColor.A,
                    Math.Min(255, darkColor.R + (int)(darkColor.R * 0.04)),
                    Math.Min(255, darkColor.G + (int)(darkColor.G * 0.04)),
                    Math.Min(255, darkColor.B + (int)(darkColor.B * 0.04))),
                _ => darkColor
            };
            
            Color glowColor = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.DarkGlow);

            // Fill dark background
            var bgBrush = PaintersFactory.GetSolidBrush(darkColor);
            g.FillPath(bgBrush, path);

            // INLINE GLOW INTENSITY MODULATION - DarkGlow neon philosophy: Glow intensifies on hover (30%), dims on press (-30%), moderate on selected (20%), slight on focus (10%), fades on disabled (-60%)
            float glowMultiplier = state switch
            {
                ControlState.Hovered => 1.3f,
                ControlState.Pressed => 0.7f,
                ControlState.Selected => 1.2f,
                ControlState.Disabled => 0.4f,
                ControlState.Focused => 1.1f,
                _ => 1.0f
            };

            // GLOW RING 1 (80% alpha base, 1px inset)
            using (var glowPath1 = GraphicsExtensions.CreateInsetPath(path, 1))
            {
                int alpha1 = Math.Min(255, (int)(80 * glowMultiplier));
                Color glowColor1 = Color.FromArgb(alpha1, glowColor);
                var pen1 = PaintersFactory.GetPen(glowColor1, 1f);
                g.DrawPath(pen1, glowPath1);
            }

            // GLOW RING 2 (40% alpha base, 3px inset)
            using (var glowPath2 = GraphicsExtensions.CreateInsetPath(path, 3))
            {
                int alpha2 = Math.Min(255, (int)(40 * glowMultiplier));
                Color glowColor2 = Color.FromArgb(alpha2, glowColor);
                var pen2 = PaintersFactory.GetPen(glowColor2, 1f);
                g.DrawPath(pen2, glowPath2);
            }

            // GLOW RING 3 (20% alpha base, 6px inset)
            using (var glowPath3 = GraphicsExtensions.CreateInsetPath(path, 6))
            {
                int alpha3 = Math.Min(255, (int)(20 * glowMultiplier));
                Color glowColor3 = Color.FromArgb(alpha3, glowColor);
                var pen3 = PaintersFactory.GetPen(glowColor3, 1f);
                g.DrawPath(pen3, glowPath3);
            }
        }
    }
}
