using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    public static class NeonShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(BeepControlStyle.Neon)) return path;

            Color cyanGlow = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Neon);
            int glowRadius = StyleShadows.GetShadowBlur(BeepControlStyle.Neon); // 24px - intense

            RectangleF bounds = path.GetBounds();
            
            // Neon: Maximum intensity cyan glow
            for (int i = glowRadius; i > 0; i -= 2)
            {
                float alpha = (float)i / glowRadius * 0.8f; // High intensity
                Rectangle glowBounds = Rectangle.Round(bounds);
                glowBounds.Inflate(i, i);

                using (var glowPath = new GraphicsPath())
                {
                    if (radius == 0)
                    {
                        glowPath.AddRectangle(glowBounds);
                    }
                    else
                    {
                        int diameter = (radius + i) * 2;
                        Size size = new Size(diameter, diameter);
                        Rectangle arc = new Rectangle(glowBounds.Location, size);
                        glowPath.AddArc(arc, 180, 90);
                        arc.X = glowBounds.Right - diameter;
                        glowPath.AddArc(arc, 270, 90);
                        arc.Y = glowBounds.Bottom - diameter;
                        glowPath.AddArc(arc, 0, 90);
                        arc.X = glowBounds.Left;
                        glowPath.AddArc(arc, 90, 90);
                        glowPath.CloseFigure();
                    }

                    using (var pen = new Pen(Color.FromArgb((int)(alpha * 255), cyanGlow), 3f))
                    {
                        g.DrawPath(pen, glowPath);
                    }
                }
            }

            return path;
        }
    }
}
