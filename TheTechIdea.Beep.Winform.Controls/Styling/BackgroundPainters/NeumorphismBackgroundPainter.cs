using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Neumorphism background painter - Soft embossed with inner highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// Pressed state inverts the shadow effect
    /// </summary>
    public static class NeumorphismBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Neumorphism: Soft 3D embossed effect
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Neumorphism);
            
            // INLINE STATE HANDLING - Neumorphism: Subtle soft embossed tinting (3% hover, 6% press darkened for depth, 5% selected, 2% focus, 110 alpha disabled)
            baseColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(baseColor.A,
                    Math.Min(255, baseColor.R + (int)(baseColor.R * 0.03)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G * 0.03)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B * 0.03))),
                ControlState.Pressed => Color.FromArgb(baseColor.A,
                    Math.Max(0, baseColor.R - (int)(baseColor.R * 0.06)),
                    Math.Max(0, baseColor.G - (int)(baseColor.G * 0.06)),
                    Math.Max(0, baseColor.B - (int)(baseColor.B * 0.06))),
                ControlState.Selected => Color.FromArgb(baseColor.A,
                    Math.Min(255, baseColor.R + (int)(baseColor.R * 0.05)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G * 0.05)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B * 0.05))),
                ControlState.Disabled => Color.FromArgb(110, baseColor),
                ControlState.Focused => Color.FromArgb(baseColor.A,
                    Math.Min(255, baseColor.R + (int)(baseColor.R * 0.02)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G * 0.02)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B * 0.02))),
                _ => baseColor
            };

            using (var brush = new SolidBrush(baseColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // INLINE INNER HIGHLIGHT - Neumorphism: Top half highlight, INVERTED when pressed (10% darken vs 10% lighten)
            Rectangle highlightRect = new Rectangle(
                bounds.X + 2,
                bounds.Y + 2,
                bounds.Width - 4,
                bounds.Height / 2
            );

            // INLINE LIGHTEN/DARKEN CALCULATION - Pressed inverts to darken (-10%), otherwise lighten (+10%)
            Color highlightColor;
            if (state == ControlState.Pressed)
            {
                // INLINE Darken 10%
                highlightColor = Color.FromArgb(
                    Math.Max(0, baseColor.R - (int)(baseColor.R * 0.1)),
                    Math.Max(0, baseColor.G - (int)(baseColor.G * 0.1)),
                    Math.Max(0, baseColor.B - (int)(baseColor.B * 0.1))
                );
            }
            else
            {
                // INLINE Lighten 10%
                highlightColor = Color.FromArgb(
                    Math.Min(255, baseColor.R + (int)(baseColor.R * 0.1)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G * 0.1)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B * 0.1))
                );
            }
            
            // INLINE WithAlpha 60
            Color highlightColorWithAlpha = Color.FromArgb(60, highlightColor);
            
            using (var brush = new SolidBrush(highlightColorWithAlpha))
            {
                if (path != null)
                {
                    // INLINE CreateRoundedRectangle for highlight
                    using (var highlightPath = new GraphicsPath())
                    {
                        int radius = 6;
                        int diameter = radius * 2;
                        if (highlightRect.Width > 0 && highlightRect.Height > 0)
                        {
                            highlightPath.AddArc(highlightRect.X, highlightRect.Y, diameter, diameter, 180, 90);
                            highlightPath.AddArc(highlightRect.Right - diameter, highlightRect.Y, diameter, diameter, 270, 90);
                            highlightPath.AddArc(highlightRect.Right - diameter, highlightRect.Bottom - diameter, diameter, diameter, 0, 90);
                            highlightPath.AddArc(highlightRect.X, highlightRect.Bottom - diameter, diameter, diameter, 90, 90);
                            highlightPath.CloseFigure();
                            g.FillPath(brush, highlightPath);
                        }
                    }
                }
                else
                {
                    g.FillRectangle(brush, highlightRect);
                }
            }
        }
    }
}
