using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material You shadow painter
    /// Uses Material elevation with dynamic color adaptation
    /// </summary>
    public static class MaterialYouShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            // Material You UX: Dynamic color adaptation and state elevation
            if (!StyleShadows.HasShadow(style)) return;

            // Adjust elevation based on state
            var actualElevation = elevation;
            if (state == ControlState.Hovered)
                actualElevation = (MaterialElevation)Math.Min(5, (int)elevation + 1);
            else if (state == ControlState.Pressed)
                actualElevation = (MaterialElevation)Math.Max(0, (int)elevation - 1);
            else if (state == ControlState.Focused)
                actualElevation = (MaterialElevation)Math.Min(5, (int)elevation + 1);

            // Material You dynamic shadow color adaptation
            Color shadowColor = StyleShadows.GetShadowColor(style);

            // Adapt shadow color based on theme if available
            if (useThemeColors && theme != null)
            {
                var themeShadow = BeepStyling.GetThemeColor("Shadow");
                if (themeShadow != Color.Empty)
                    shadowColor = Color.FromArgb(shadowColor.A, themeShadow);
            }

            // Use StyleShadows for consistent Material You shadows
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, 0.6f, blur / 2);
        }
    }
}
