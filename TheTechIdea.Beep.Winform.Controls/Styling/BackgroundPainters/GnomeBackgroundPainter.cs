using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gnome background painter - Adwaita design system
    /// Warm gray backgrounds with subtle gradients
    /// GNOME's signature warm, welcoming aesthetic
    /// 6px radius for friendly rounded corners
    /// </summary>
    public static class GnomeBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            // Gnome Adwaita colors: Warm gray with subtle warmth
            Color warmGray = useThemeColors && theme != null ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Gnome);
            Color blueAccent = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Gnome);

            RectangleF bounds = path.GetBounds();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // State-based color adjustments
            Color fillColor = GetGnomeStateFill(state, warmGray, blueAccent);

            // Gnome: Subtle vertical gradient for depth (Adwaita signature)
            if (state == ControlState.Normal || state == ControlState.Hovered)
            {
                Color topColor = ColorUtils.Lighten(fillColor,0.02f);
                Color bottomColor = ColorUtils.Darken(fillColor,0.02f);
                var brush = PaintersFactory.GetLinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical);
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
            return state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(warmGray,0.05f),
                ControlState.Pressed => ColorUtils.Darken(warmGray,0.08f),
                ControlState.Selected => Color.FromArgb(30, blueAccent),
                ControlState.Disabled => ColorUtils.Desaturate(warmGray,0.5f),
                _ => warmGray
            };
        }
    }
}
