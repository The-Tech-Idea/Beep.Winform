using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// KDE background painter - Breeze design system
    /// Clean light backgrounds with signature blue glow effects
    /// Breeze's characteristic #3DAEE9 blue accent
    /// </summary>
    public static class KdeBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // KDE Breeze colors
            Color lightBackground = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : StyleColors.GetBackground(BeepControlStyle.Kde);
            Color breezeBlue = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.Kde);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get state-adjusted fill
            Color fillColor = GetKdeStateFill(state, lightBackground, breezeBlue);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            // KDE Breeze signature: blue glow overlay on hover/focused
            if (state == ControlState.Hovered || state == ControlState.Focused)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(18, breezeBlue));
                g.FillPath(glowBrush, path);
            }
        }

        private static Color GetKdeStateFill(ControlState state, Color lightBackground, Color breezeBlue)
        {
            // Use HSL for more natural results
            return state switch
            {
                ControlState.Hovered => lightBackground, // Keep base (glow added separately)
                ControlState.Pressed => ColorAccessibilityHelper.DarkenColor(lightBackground, 0.08f),
                ControlState.Selected => BackgroundPainterHelpers.BlendColors(lightBackground, breezeBlue, 0.12f),
                ControlState.Focused => lightBackground, // Keep base (glow added separately)
                ControlState.Disabled => BackgroundPainterHelpers.WithAlpha(lightBackground, 100),
                _ => lightBackground
            };
        }
    }
}
