using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// GNOME background painter - Adwaita design system
    /// Warm gray backgrounds with subtle gradients
    /// GNOME's signature warm, welcoming aesthetic
    /// </summary>
    public static class GnomeBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // GNOME Adwaita: warm gray
            Color warmGray = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : StyleColors.GetBackground(BeepControlStyle.Gnome);
            Color blueAccent = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.Gnome);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get state-adjusted color
            Color fillColor = GetGnomeStateFill(state, warmGray, blueAccent);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // GNOME Adwaita signature: subtle vertical gradient for depth - use HSL for more natural results
            if (state == ControlState.Normal || state == ControlState.Hovered)
            {
                Color topColor = ColorAccessibilityHelper.LightenColor(fillColor, 0.02f);
                Color bottomColor = ColorAccessibilityHelper.DarkenColor(fillColor, 0.02f);
                var brush = PaintersFactory.GetLinearGradientBrush(
                    bounds, topColor, bottomColor, LinearGradientMode.Vertical);
                g.FillPath(brush, path);
            }
            else
            {
                var brush = PaintersFactory.GetSolidBrush(fillColor);
                g.FillPath(brush, path);
            }
        }

        private static Color GetGnomeStateFill(ControlState state, Color warmGray, Color blueAccent)
        {
            // Use HSL for more natural results
            return state switch
            {
                ControlState.Hovered => ColorAccessibilityHelper.LightenColor(warmGray, 0.05f),
                ControlState.Pressed => ColorAccessibilityHelper.DarkenColor(warmGray, 0.08f),
                ControlState.Selected => BackgroundPainterHelpers.BlendColors(warmGray, blueAccent, 0.15f),
                ControlState.Focused => ColorAccessibilityHelper.LightenColor(warmGray, 0.03f),
                ControlState.Disabled => BackgroundPainterHelpers.WithAlpha(warmGray, 100),
                _ => warmGray
            };
        }
    }
}
