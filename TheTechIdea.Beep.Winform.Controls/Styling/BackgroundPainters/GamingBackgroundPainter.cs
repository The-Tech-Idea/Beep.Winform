using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gaming background painter - RGB gaming aesthetic
    /// Dark background with angular 0px radius, green glow (#00FF7F)
    /// Orbitron font aesthetic, aggressive cyberpunk Style
    /// </summary>
    public static class GamingBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color darkBg = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Gaming);
            Color neonGreen = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Gaming);

            g.SmoothingMode = SmoothingMode.None; // Angular for gaming aesthetic

            Color fillColor = state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(darkBg, 0.1f),
                ControlState.Pressed => ColorUtils.Lighten(darkBg, 0.15f),
                ControlState.Selected => Color.FromArgb(40, neonGreen),
                ControlState.Disabled => ColorUtils.Darken(darkBg, 0.1f),
                _ => darkBg
            };

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            // Gaming: Green glow overlay on hover
            if (state == ControlState.Hovered)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(30, neonGreen)))
                {
                    g.FillPath(glowBrush, path);
                }
            }
        }
    }
}
