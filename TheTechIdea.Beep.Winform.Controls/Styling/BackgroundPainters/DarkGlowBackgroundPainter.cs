using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Dark Glow background painter - Solid dark with 3-ring neon glow
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// Glow intensity modulated by state
    /// </summary>
    public static class DarkGlowBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Dark Glow: Dark background with neon glow rings
            Color darkColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.DarkGlow);
            
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
            
            Color glowColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.DarkGlow);

            // Fill dark background
            using (var brush = new SolidBrush(darkColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

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

            // INLINE GEOMETRY + GLOW RING 1 (80% alpha base, 1px inset)
            Rectangle glow1 = new Rectangle(
                bounds.X + 1,
                bounds.Y + 1,
                bounds.Width - 2,
                bounds.Height - 2
            );
            int alpha1 = Math.Min(255, (int)(80 * glowMultiplier));
            Color glowColor1 = Color.FromArgb(alpha1, glowColor);
            using (var pen = new Pen(glowColor1, 1f))
            {
                if (path != null)
                {
                    // INLINE CreateRoundedRectangle for glow ring 1
                    using (var glowPath = new GraphicsPath())
                    {
                        int radius = 4;
                        int diameter = radius * 2;
                        glowPath.AddArc(glow1.X, glow1.Y, diameter, diameter, 180, 90);
                        glowPath.AddArc(glow1.Right - diameter, glow1.Y, diameter, diameter, 270, 90);
                        glowPath.AddArc(glow1.Right - diameter, glow1.Bottom - diameter, diameter, diameter, 0, 90);
                        glowPath.AddArc(glow1.X, glow1.Bottom - diameter, diameter, diameter, 90, 90);
                        glowPath.CloseFigure();
                        g.DrawPath(pen, glowPath);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, glow1);
                }
            }

            // INLINE GEOMETRY + GLOW RING 2 (40% alpha base, 3px inset)
            Rectangle glow2 = new Rectangle(
                bounds.X + 3,
                bounds.Y + 3,
                bounds.Width - 6,
                bounds.Height - 6
            );
            int alpha2 = Math.Min(255, (int)(40 * glowMultiplier));
            Color glowColor2 = Color.FromArgb(alpha2, glowColor);
            using (var pen = new Pen(glowColor2, 1f))
            {
                if (path != null)
                {
                    // INLINE CreateRoundedRectangle for glow ring 2
                    using (var glowPath = new GraphicsPath())
                    {
                        int radius = 4;
                        int diameter = radius * 2;
                        glowPath.AddArc(glow2.X, glow2.Y, diameter, diameter, 180, 90);
                        glowPath.AddArc(glow2.Right - diameter, glow2.Y, diameter, diameter, 270, 90);
                        glowPath.AddArc(glow2.Right - diameter, glow2.Bottom - diameter, diameter, diameter, 0, 90);
                        glowPath.AddArc(glow2.X, glow2.Bottom - diameter, diameter, diameter, 90, 90);
                        glowPath.CloseFigure();
                        g.DrawPath(pen, glowPath);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, glow2);
                }
            }

            // INLINE GEOMETRY + GLOW RING 3 (20% alpha base, 6px inset)
            Rectangle glow3 = new Rectangle(
                bounds.X + 6,
                bounds.Y + 6,
                bounds.Width - 12,
                bounds.Height - 12
            );
            int alpha3 = Math.Min(255, (int)(20 * glowMultiplier));
            Color glowColor3 = Color.FromArgb(alpha3, glowColor);
            using (var pen = new Pen(glowColor3, 1f))
            {
                if (path != null)
                {
                    // INLINE CreateRoundedRectangle for glow ring 3
                    using (var glowPath = new GraphicsPath())
                    {
                        int radius = 4;
                        int diameter = radius * 2;
                        glowPath.AddArc(glow3.X, glow3.Y, diameter, diameter, 180, 90);
                        glowPath.AddArc(glow3.Right - diameter, glow3.Y, diameter, diameter, 270, 90);
                        glowPath.AddArc(glow3.Right - diameter, glow3.Bottom - diameter, diameter, diameter, 0, 90);
                        glowPath.AddArc(glow3.X, glow3.Bottom - diameter, diameter, diameter, 90, 90);
                        glowPath.CloseFigure();
                        g.DrawPath(pen, glowPath);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, glow3);
                }
            }
        }
    }
}
