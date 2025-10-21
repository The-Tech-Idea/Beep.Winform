using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Gnome path painter - Adwaita design system
    /// 6px radius (friendly rounded corners)
    /// Blue accent fills with subtle gradients
    /// Warm, welcoming GNOME aesthetic
    /// </summary>
    public static class GnomePathPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style, ControlState state = ControlState.Normal)
        {
            // Gnome Adwaita colors
            Color blueAccent = StyleColors.GetPrimary(BeepControlStyle.Gnome); // Blue (#3584E4)
            Color warmGray = StyleColors.GetBackground(BeepControlStyle.Gnome);

            RectangleF bounds = path.GetBounds();
            
            // Determine fill color based on state
            Color fillColor = state switch
            {
                ControlState.Selected => blueAccent,
                ControlState.Focused => blueAccent,
                ControlState.Hovered => ColorUtils.Lighten(blueAccent, 0.1f),
                _ => blueAccent
            };

            // Gnome: Subtle vertical gradient (Adwaita signature)
            Color topColor = ColorUtils.Lighten(fillColor, 0.02f);
            Color bottomColor = ColorUtils.Darken(fillColor, 0.02f);

            using (var brush = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
