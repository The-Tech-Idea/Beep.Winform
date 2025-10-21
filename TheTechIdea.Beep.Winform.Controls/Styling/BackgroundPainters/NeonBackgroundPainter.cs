using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Neon background painter - Cyberpunk neon aesthetic
    /// Very dark background (#0A0A14) with intense cyan glow (#00FFFF)
    /// 24px blur, Rajdhani font, maximum neon intensity
    /// </summary>
    public static class NeonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color veryDark = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Neon);
            Color cyanGlow = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Neon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(veryDark, 0.15f),
                ControlState.Pressed => ColorUtils.Lighten(veryDark, 0.2f),
                ControlState.Selected => Color.FromArgb(60, cyanGlow),
                ControlState.Disabled => veryDark,
                _ => veryDark
            };

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            // Neon: Intense cyan glow overlay
            if (state == ControlState.Hovered || state == ControlState.Selected)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(50, cyanGlow)))
                {
                    g.FillPath(glowBrush, path);
                }
            }
        }
    }
}
