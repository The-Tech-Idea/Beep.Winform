using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material You background painter - personalized dynamic color system
    /// Tonal surface with primary color tinting for personalized feel
    /// </summary>
    public static class MaterialYouBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Material You: dynamic surface color
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.MaterialYou);
            Color primaryColor = useThemeColors && theme != null 
                ? theme.PrimaryColor 
                : StyleColors.GetPrimary(BeepControlStyle.MaterialYou);

            // Material You state handling: blend with primary color
            Color stateColor = GetMaterialYouStateColor(baseColor, primaryColor, state);
            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);

            // Tonal primary overlay (Material You's signature personal tinting)
            var tonalBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(15, primaryColor));
            g.FillPath(tonalBrush, path);
        }

        private static Color GetMaterialYouStateColor(Color baseColor, Color primaryColor, ControlState state)
        {
            return state switch
            {
                // Hover: blend 10% primary
                ControlState.Hovered => BlendColors(baseColor, primaryColor, 0.10f),
                // Pressed: darken 8%
                ControlState.Pressed => BackgroundPainterHelpers.Darken(baseColor, 0.08f),
                // Selected: blend 15% primary
                ControlState.Selected => BlendColors(baseColor, primaryColor, 0.15f),
                // Focused: blend 7% primary
                ControlState.Focused => BlendColors(baseColor, primaryColor, 0.07f),
                // Disabled: reduced alpha
                ControlState.Disabled => BackgroundPainterHelpers.WithAlpha(baseColor, 90),
                _ => baseColor
            };
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
