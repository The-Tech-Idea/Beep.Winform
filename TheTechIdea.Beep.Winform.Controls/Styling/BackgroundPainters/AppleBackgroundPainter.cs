using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Apple background painter - Clean, subtle gradients with premium feel
    /// Apple UX: Minimal, refined aesthetics with smooth state transitions
    /// </summary>
    public static class AppleBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Apple Style: Clean, subtle backgrounds with premium feel
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Apple);

            // Apple-specific state handling - subtle and elegant
            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 * 0.08f)), Math.Min(255, baseColor.G + (int)(255 * 0.08f)), Math.Min(255, baseColor.B + (int)(255 * 0.08f))),
                ControlState.Pressed => Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - (int)(baseColor.R * 0.12f)), Math.Max(0, baseColor.G - (int)(baseColor.G * 0.12f)), Math.Max(0, baseColor.B - (int)(baseColor.B * 0.12f))),
                ControlState.Selected => Color.FromArgb(baseColor.A, (int)(baseColor.R * 0.85f + (useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0, 122, 255)).R * 0.15f), (int)(baseColor.G * 0.85f + (useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0, 122, 255)).G * 0.15f), (int)(baseColor.B * 0.85f + (useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0, 122, 255)).B * 0.15f)),
                ControlState.Disabled => Color.FromArgb(100, (baseColor.R + baseColor.G + baseColor.B) / 3, (baseColor.R + baseColor.G + baseColor.B) / 3, (baseColor.R + baseColor.G + baseColor.B) / 3),
                ControlState.Focused => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R + (int)(255 * 0.10f)), Math.Min(255, baseColor.G + (int)(255 * 0.10f)), Math.Min(255, baseColor.B + (int)(255 * 0.10f))),
                _ => baseColor,
            };

            var bounds = path.GetBounds();
            var secondary = Color.FromArgb(stateColor.A, Math.Max(0, stateColor.R - 5), Math.Max(0, stateColor.G - 5), Math.Max(0, stateColor.B - 5));
            var brush = PaintersFactory.GetLinearGradientBrush(bounds, stateColor, secondary, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }
    }
}
