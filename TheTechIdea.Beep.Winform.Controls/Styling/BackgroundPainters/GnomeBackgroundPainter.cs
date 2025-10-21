using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

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
            Color warmGray = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Gnome);
            Color blueAccent = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Gnome);

            RectangleF bounds = path.GetBounds();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // State-based color adjustments
            Color fillColor = GetGnomeStateFill(state, warmGray, blueAccent);

            // Gnome: Subtle vertical gradient for depth (Adwaita signature)
            if (state == ControlState.Normal || state == ControlState.Hovered)
            {
                // Very subtle gradient (lighter top to slightly darker bottom)
                Color topColor = ColorUtils.Lighten(fillColor, 0.02f);
                Color bottomColor = ColorUtils.Darken(fillColor, 0.02f);

                using (var brush = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }
            }
            else
            {
                // Selected/pressed/disabled: Flat fill
                using (var brush = new SolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private static Color GetGnomeStateFill(ControlState state, Color warmGray, Color blueAccent)
        {
            // Gnome Adwaita state handling
            return state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(warmGray, 0.05f), // Subtle lighten
                ControlState.Pressed => ColorUtils.Darken(warmGray, 0.08f), // Noticeable press
                ControlState.Selected => Color.FromArgb(30, blueAccent), // Translucent blue overlay
                ControlState.Disabled => ColorUtils.Desaturate(warmGray, 0.5f), // Grayed out
                _ => warmGray // Normal: Warm gray
            };
        }
    }
}
