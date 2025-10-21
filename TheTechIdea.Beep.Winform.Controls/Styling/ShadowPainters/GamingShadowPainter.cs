using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    public static class GamingShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(BeepControlStyle.Gaming)) return path;

            Color neonGreen = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Gaming);
            int glowRadius = StyleShadows.GetShadowBlur(BeepControlStyle.Gaming);

            RectangleF bounds = path.GetBounds();
            
            // Gaming: Intense green RGB glow
            for (int i = glowRadius; i > 0; i -= 2)
            {
                float alpha = (float)i / glowRadius * 0.5f;
                Rectangle glowBounds = Rectangle.Round(bounds);
                glowBounds.Inflate(i, i);

                using (var glowPath = new GraphicsPath())
                {
                    glowPath.AddRectangle(glowBounds); // Angular

                    using (var pen = new Pen(Color.FromArgb((int)(alpha * 255), neonGreen), 2f))
                    {
                        g.DrawPath(pen, glowPath);
                    }
                }
            }

            return path;
        }
    }
}
