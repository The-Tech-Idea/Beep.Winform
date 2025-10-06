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
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
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
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }

            // Layer 2: Top highlight (15% alpha = 38) - INLINE WithAlpha
            Rectangle highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 3);
            Color highlightGlass = Color.FromArgb(38, Color.White);
            using (var brush = new SolidBrush(highlightGlass))
            {
                if (path != null)
                {
                    // INLINE CreateRoundedRectangle for highlight layer
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

            // Layer 3: Subtle shine (25% alpha = 64) - INLINE WithAlpha
            Rectangle shineRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 5);
            Color shineGlass = Color.FromArgb(64, Color.White);
            using (var brush = new SolidBrush(shineGlass))
            {
                if (path != null)
                {
                    // INLINE CreateRoundedRectangle for shine layer
                    using (var shinePath = new GraphicsPath())
                    {
                        int radius = 6;
                        int diameter = radius * 2;
                        if (shineRect.Width > 0 && shineRect.Height > 0)
                        {
                            shinePath.AddArc(shineRect.X, shineRect.Y, diameter, diameter, 180, 90);
                            shinePath.AddArc(shineRect.Right - diameter, shineRect.Y, diameter, diameter, 270, 90);
                            shinePath.AddArc(shineRect.Right - diameter, shineRect.Bottom - diameter, diameter, diameter, 0, 90);
                            shinePath.AddArc(shineRect.X, shineRect.Bottom - diameter, diameter, diameter, 90, 90);
                            shinePath.CloseFigure();
                            g.FillPath(brush, shinePath);
                        }
                    }
                }
                else
                {
                    g.FillRectangle(brush, shineRect);
                }
            }
        }
    }
}
