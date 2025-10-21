using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// KDE shadow painter - Breeze design system
    /// Blue glow effect on interaction (signature KDE Breeze)
    /// 12px blur radius, Breeze blue (#3DAEE9) color
    /// 0.6f intensity for noticeable but not overwhelming glow
    /// </summary>
    public static class KdeShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(BeepControlStyle.Kde)) return path;

            // KDE: Blue glow only on interaction (Breeze signature)
            if (state != ControlState.Hovered && state != ControlState.Focused && state != ControlState.Selected)
            {
                return path; // No glow in normal/pressed/disabled states
            }

            Color breezeBlue = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Kde);
            int glowRadius = StyleShadows.GetShadowBlur(BeepControlStyle.Kde); // 12px
            float intensity = 0.6f;

            RectangleF bounds = path.GetBounds();
            
            // KDE Breeze: Multi-layer blue glow effect
            for (int i = glowRadius; i > 0; i -= 2)
            {
                float alpha = (intensity * (float)i / glowRadius) * 0.3f; // Fade out
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

                    using (var pen = new Pen(Color.FromArgb((int)(alpha * 255), breezeBlue), 2f))
                    {
                        g.DrawPath(pen, glowPath);
                    }
                }
            }

            return path;
        }
    }
}
