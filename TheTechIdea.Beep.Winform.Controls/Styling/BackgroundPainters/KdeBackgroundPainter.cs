using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// KDE background painter - Breeze design system
    /// Clean white/light backgrounds with blue glow effects
    /// 4px radius for modern look
    /// Signature KDE blue (#3DAEE9) for accents
    /// </summary>
    public static class KdeBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            // KDE Breeze colors
            Color lightBackground = useThemeColors && theme != null ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Kde);
            Color breezeBlue = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Kde);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // State-based fill
            Color fillColor = GetKdeStateFill(state, lightBackground, breezeBlue);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            // KDE signature: Blue glow on hover/focused (Breeze effect)
            if (state == ControlState.Hovered || state == ControlState.Focused)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, breezeBlue));
                g.FillPath(glowBrush, path);
            }
        }

        private static Color GetKdeStateFill(ControlState state, Color lightBackground, Color breezeBlue)
        {
            // KDE Breeze state handling
            return state switch
            {
                ControlState.Hovered => lightBackground, // Keep base (glow added separately)
                ControlState.Pressed => ColorUtils.Darken(lightBackground, 0.08f),
                ControlState.Selected => Color.FromArgb(25, breezeBlue), // Translucent blue
                ControlState.Disabled => ColorUtils.Lighten(lightBackground, 0.05f),
                _ => lightBackground // Normal: Clean light background
            };
        }
    }
}
