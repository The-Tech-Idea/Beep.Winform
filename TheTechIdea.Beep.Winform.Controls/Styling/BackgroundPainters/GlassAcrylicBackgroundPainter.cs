using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Glass Acrylic background painter - 3-layer frosted glass effect
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class GlassAcrylicBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Glass Acrylic: Multi-layer frosted glass
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.GlassAcrylic);
            
            // INLINE STATE HANDLING - GlassAcrylic: Base alpha modulation for frosted glass translucency (pressed 140, hover +20, selected +30, focus +15, disabled 70)
            int baseAlpha = state switch
            {
                ControlState.Pressed => 140,
                ControlState.Hovered => Math.Min(255, baseColor.A + 20),
                ControlState.Selected => Math.Min(255, baseColor.A + 30),
                ControlState.Focused => Math.Min(255, baseColor.A + 15),
                ControlState.Disabled => 70,
                _ => baseColor.A
            };
            
            Color baseGlass = Color.FromArgb(baseAlpha, baseColor);
            using (var brush = new SolidBrush(baseGlass))
            {
                g.FillPath(brush, path);
            }

            // Get bounds for clip region calculations
            RectangleF bounds = path.GetBounds();

            // Layer 2: Top highlight (15% alpha = 38) - Top third region
            Color highlightGlass = Color.FromArgb(38, Color.White);
            using (var brush = new SolidBrush(highlightGlass))
            using (var highlightRegion = new Region(path))
            {
                // Clip to top third
                using (var clipRect = new GraphicsPath())
                {
                    clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height / 3));
                    highlightRegion.Intersect(clipRect);
                    g.SetClip(highlightRegion, CombineMode.Replace);
                    g.FillPath(brush, path);
                    g.ResetClip();
                }
            }

            // Layer 3: Subtle shine (25% alpha = 64) - Top fifth region
            Color shineGlass = Color.FromArgb(64, Color.White);
            using (var brush = new SolidBrush(shineGlass))
            using (var shineRegion = new Region(path))
            {
                // Clip to top fifth
                using (var clipRect = new GraphicsPath())
                {
                    clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height / 5));
                    shineRegion.Intersect(clipRect);
                    g.SetClip(shineRegion, CombineMode.Replace);
                    g.FillPath(brush, path);
                    g.ResetClip();
                }
            }
        }
    }
}
