using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Windows 11 Mica background painter - desktop wallpaper-tinted material
    /// Vertical gradient with primary color tinting based on state
    /// </summary>
    public static class Windows11MicaBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Mica base color
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Windows11Mica);
            Color primaryColor = useThemeColors && theme != null 
                ? theme.PrimaryColor 
                : StyleColors.GetPrimary(BeepControlStyle.Windows11Mica);

            // Get Mica-tinted color based on state
            Color stateColor = GetMicaStateColor(baseColor, primaryColor, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Mica gradient effect (2% darker at bottom)
            Color topColor = stateColor;
            Color bottomColor = BackgroundPainterHelpers.Darken(stateColor, 0.02f);

            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, topColor, bottomColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }

        private static Color GetMicaStateColor(Color baseColor, Color primaryColor, ControlState state)
        {
            // Mica tint strength varies by state
            float tintStrength = state switch
            {
                ControlState.Hovered => 0.08f,
                ControlState.Pressed => 0.12f,
                ControlState.Selected => 0.10f,
                ControlState.Focused => 0.06f,
                ControlState.Disabled => 0.02f,
                _ => 0.05f // Normal
            };

            // Blend base with primary based on tint strength
            Color blended = BlendColors(baseColor, primaryColor, tintStrength);

            // Additional modifications for specific states
            if (state == ControlState.Pressed)
            {
                blended = BackgroundPainterHelpers.Darken(blended, 0.10f);
            }
            else if (state == ControlState.Disabled)
            {
                blended = BackgroundPainterHelpers.WithAlpha(blended, 75);
            }

            return blended;
        }

        private static Color BlendColors(Color baseColor, Color blendColor, float ratio)
        {
            return Color.FromArgb(
                baseColor.A,
                (int)(baseColor.R * (1 - ratio) + blendColor.R * ratio),
                (int)(baseColor.G * (1 - ratio) + blendColor.G * ratio),
                (int)(baseColor.B * (1 - ratio) + blendColor.B * ratio)
            );
        }
    }
}
