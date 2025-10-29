using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

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
            // Neumorphism: Soft3D embossed effect
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Neumorphism);

            // INLINE STATE HANDLING - Neumorphism: Subtle soft embossed tinting (3% hover,6% press darkened for depth,5% selected,2% focus,110 alpha disabled)
            baseColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(baseColor.A,
                    Math.Min(255, baseColor.R + (int)(baseColor.R *0.03)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G *0.03)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B *0.03))),
                ControlState.Pressed => Color.FromArgb(baseColor.A,
                    Math.Max(0, baseColor.R - (int)(baseColor.R *0.06)),
                    Math.Max(0, baseColor.G - (int)(baseColor.G *0.06)),
                    Math.Max(0, baseColor.B - (int)(baseColor.B *0.06))),
                ControlState.Selected => Color.FromArgb(baseColor.A,
                    Math.Min(255, baseColor.R + (int)(baseColor.R *0.05)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G *0.05)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B *0.05))),
                ControlState.Disabled => Color.FromArgb(110, baseColor),
                ControlState.Focused => Color.FromArgb(baseColor.A,
                    Math.Min(255, baseColor.R + (int)(baseColor.R *0.02)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G *0.02)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B *0.02))),
                _ => baseColor
            };

            var fillBrush = PaintersFactory.GetSolidBrush(baseColor);
            g.FillPath(fillBrush, path);

            // Get bounds for clipping calculations
            RectangleF bounds = path.GetBounds();

            // INLINE INNER HIGHLIGHT - Neumorphism: Top half highlight, INVERTED when pressed (10% darken vs10% lighten)
            Color highlightColor;
            if (state == ControlState.Pressed)
            {
                // INLINE Darken10%
                highlightColor = Color.FromArgb(
                    Math.Max(0, baseColor.R - (int)(baseColor.R *0.1)),
                    Math.Max(0, baseColor.G - (int)(baseColor.G *0.1)),
                    Math.Max(0, baseColor.B - (int)(baseColor.B *0.1))
                );
            }
            else
            {
                // INLINE Lighten10%
                highlightColor = Color.FromArgb(
                    Math.Min(255, baseColor.R + (int)(baseColor.R *0.1)),
                    Math.Min(255, baseColor.G + (int)(baseColor.G *0.1)),
                    Math.Min(255, baseColor.B + (int)(baseColor.B *0.1))
                );
            }

            // INLINE WithAlpha60
            Color highlightColorWithAlpha = Color.FromArgb(60, highlightColor);
            var highlightBrush = PaintersFactory.GetSolidBrush(highlightColorWithAlpha);

            // Create inset path (2px inset) by using GraphicsExtensions.CreateInsetPath which accepts a path
            using (var insetPath = GraphicsExtensions.CreateInsetPath(path,2))
            using (var highlightRegion = new Region(insetPath))
            {
                RectangleF insetBounds = insetPath.GetBounds();
                using (var clipRect = new GraphicsPath())
                {
                    clipRect.AddRectangle(new RectangleF(insetBounds.X, insetBounds.Y, insetBounds.Width, insetBounds.Height /2));
                    highlightRegion.MakeEmpty();
                    highlightRegion.Union(insetPath);
                    highlightRegion.Intersect(clipRect);
                    g.SetClip(highlightRegion, CombineMode.Replace);
                    g.FillPath(highlightBrush, insetPath);
                    g.ResetClip();
                }
            }
        }
    }
}
