using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Gnome shadow painter - Adwaita design system
    /// Subtle ambient shadows for depth (4px spread, 0.3f opacity)
    /// Soft, natural shadows matching GNOME's welcoming aesthetic
    /// </summary>
    public static class GnomeShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(BeepControlStyle.Gnome)) return path;

            // Gnome: Subtle ambient shadow (Adwaita signature)
            int spread = StyleShadows.GetShadowSpread(BeepControlStyle.Gnome); // 4
            float opacity = 0.3f; // Subtle
            Color shadowColor = StyleShadows.GetShadowColor(BeepControlStyle.Gnome);

            // Ambient shadow: Slightly larger path with soft edges
            RectangleF bounds = path.GetBounds();
            Rectangle shadowBounds = Rectangle.Round(bounds);
            shadowBounds.Inflate(spread, spread);

            using (var shadowPath = new GraphicsPath())
            {
                if (radius == 0)
                {
                    shadowPath.AddRectangle(shadowBounds);
                }
                else
                {
                    int diameter = (radius + spread) * 2;
                    Size size = new Size(diameter, diameter);
                    Rectangle arc = new Rectangle(shadowBounds.Location, size);

                    shadowPath.AddArc(arc, 180, 90);
                    arc.X = shadowBounds.Right - diameter;
                    shadowPath.AddArc(arc, 270, 90);
                    arc.Y = shadowBounds.Bottom - diameter;
                    shadowPath.AddArc(arc, 0, 90);
                    arc.X = shadowBounds.Left;
                    shadowPath.AddArc(arc, 90, 90);
                    shadowPath.CloseFigure();
                }

                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb((int)(opacity * 255), shadowColor));
                g.FillPath(brush, shadowPath);
            }

            return path;
        }
    }
}
