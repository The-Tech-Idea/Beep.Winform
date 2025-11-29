using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Ubuntu shadow painter - authentic Yaru/GNOME theme shadow
    /// Clean, subtle drop shadow matching GTK4/libadwaita design
    /// Single-layer soft shadow for professional appearance
    /// </summary>
    public static class UbuntuShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get bounds and shadow parameters
            RectangleF bounds = path.GetBounds();
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            
            // Yaru uses a clean neutral shadow (not warm-tinted)
            // Similar to GNOME Adwaita - professional and subtle
            Color baseShadowColor = Color.FromArgb(0, 0, 0);

            // State-based opacity - Yaru has minimal but present interactive feedback
            int alpha = state switch
            {
                ControlState.Hovered => 55,    // Slightly more visible on hover
                ControlState.Pressed => 30,    // Very subtle when pressed (appears closer)
                ControlState.Focused => 50,    // Moderate focus indication
                ControlState.Disabled => 15,   // Almost invisible when disabled
                _ => 40                        // Default: subtle but visible
            };

            // Calculate shadow bounds - offset down slightly (Y offset) and expand for blur
            int spread = blur / 3;  // Subtle spread
            Rectangle shadowBounds = Rectangle.Round(bounds);
            shadowBounds.Offset(0, offsetY);
            shadowBounds.Inflate(spread, spread);

            // Draw single clean shadow layer (authentic Ubuntu/GNOME style)
            using (var shadowPath = new GraphicsPath())
            {
                if (radius == 0)
                {
                    shadowPath.AddRectangle(shadowBounds);
                }
                else
                {
                    // Create rounded rectangle for shadow
                    int effectiveRadius = radius + spread;
                    int diameter = effectiveRadius * 2;
                    
                    if (diameter > 0 && shadowBounds.Width > diameter && shadowBounds.Height > diameter)
                    {
                        Size cornerSize = new Size(diameter, diameter);
                        Rectangle arc = new Rectangle(shadowBounds.Location, cornerSize);

                        // Top-left
                        shadowPath.AddArc(arc, 180, 90);
                        // Top-right
                        arc.X = shadowBounds.Right - diameter;
                        shadowPath.AddArc(arc, 270, 90);
                        // Bottom-right
                        arc.Y = shadowBounds.Bottom - diameter;
                        shadowPath.AddArc(arc, 0, 90);
                        // Bottom-left
                        arc.X = shadowBounds.Left;
                        shadowPath.AddArc(arc, 90, 90);
                        shadowPath.CloseFigure();
                    }
                    else
                    {
                        // Fallback to rectangle if too small for rounded corners
                        shadowPath.AddRectangle(shadowBounds);
                    }
                }

                // Fill with subtle shadow color
                using (var brush = new SolidBrush(Color.FromArgb(alpha, baseShadowColor)))
                {
                    g.FillPath(brush, shadowPath);
                }
            }

            return path;
        }
    }
}
