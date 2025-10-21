using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    public static class ElementaryShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(BeepControlStyle.Elementary)) return path;

            Color shadowColor = StyleShadows.GetShadowColor(BeepControlStyle.Elementary);
            int blur = StyleShadows.GetShadowBlur(BeepControlStyle.Elementary);

            RectangleF bounds = path.GetBounds();
            Rectangle shadowBounds = Rectangle.Round(bounds);
            shadowBounds.Inflate(blur / 2, blur / 2);

            using (var shadowPath = new GraphicsPath())
            {
                if (radius == 0)
                {
                    shadowPath.AddRectangle(shadowBounds);
                }
                else
                {
                    int diameter = (radius + blur / 2) * 2;
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

                using (var brush = new SolidBrush(Color.FromArgb(40, shadowColor)))
                {
                    g.FillPath(brush, shadowPath);
                }
            }

            return path;
        }
    }
}
