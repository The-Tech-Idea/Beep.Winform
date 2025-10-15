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
        public static void Paint(Graphics g, GraphicsPath path, 
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
                g.FillPath(brush, path);
            }

            // Get bounds for clipping calculations
            RectangleF bounds = path.GetBounds();

            // INLINE INNER HIGHLIGHT - Neumorphism: Top half highlight, INVERTED when pressed (10% darken vs 10% lighten)
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
            using (var highlightRegion = new Region(path))
            {
                // Create inset path (2px inset) and clip to top half
                using (var insetPath = GraphicsExtensions.CreateInsetPath(path, 2))
                {
                    RectangleF insetBounds = insetPath.GetBounds();
                    using (var clipRect = new GraphicsPath())
                    {
                        clipRect.AddRectangle(new RectangleF(insetBounds.X, insetBounds.Y, insetBounds.Width, insetBounds.Height / 2));
                        highlightRegion.MakeEmpty();
                        highlightRegion.Union(insetPath);
                        highlightRegion.Intersect(clipRect);
                        g.SetClip(highlightRegion, CombineMode.Replace);
                        g.FillPath(brush, insetPath);
                        g.ResetClip();
                    }
                }
            }
        }
    }
}
