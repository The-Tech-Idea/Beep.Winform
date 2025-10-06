using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Bootstrap shadow painter
    /// Uses Bootstrap's standard box-shadow
    /// </summary>
    public static class BootstrapShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            MaterialElevation elevation = MaterialElevation.Level1)
        {
            if (!StyleShadows.HasShadow(style))
                return;

            // Bootstrap shadow utilities with elevation levels
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            Color shadowColor = StyleShadows.GetShadowColor(style);

            // Base opacity for Bootstrap shadows
            float opacity = 0.16f;

            // Adjust for elevation levels (Bootstrap shadow-sm, shadow, shadow-lg, shadow-xl)
            switch (elevation)
            {
                case MaterialElevation.Level0:
                    opacity *= 0.5f; // shadow-sm
                    break;
                case MaterialElevation.Level1:
                    // shadow (default)
                    break;
                case MaterialElevation.Level2:
                    opacity *= 1.2f; // shadow-lg
                    break;
                case MaterialElevation.Level3:
                    opacity *= 1.5f; // shadow-xl
                    break;
                case MaterialElevation.Level4:
                    opacity *= 2.0f; // shadow-xl enhanced
                    break;
                case MaterialElevation.Level5:
                    opacity *= 2.5f; // shadow-xl max
                    break;
            }

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, opacity, blur);
        }

        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal,
            MaterialElevation elevation = MaterialElevation.Level1)
        {
            if (!StyleShadows.HasShadow(style))
                return;

            // Bootstrap shadow utilities with state awareness
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            Color shadowColor = StyleShadows.GetShadowColor(style);

            // Base opacity for Bootstrap shadows
            float opacity = 0.16f;

            // Adjust opacity based on control state
            switch (state)
            {
                case ControlState.Normal:
                    // Default opacity
                    break;
                case ControlState.Hovered:
                    opacity *= 1.3f; // Enhanced shadow on hover
                    break;
                case ControlState.Pressed:
                    opacity *= 0.7f; // Reduced shadow when pressed
                    offsetY = Math.Max(1, offsetY - 1); // Closer shadow
                    break;
                case ControlState.Focused:
                    opacity *= 1.2f; // Slightly enhanced shadow when focused
                    break;
                case ControlState.Disabled:
                    opacity *= 0.5f; // Much reduced shadow when disabled
                    break;
            }

            // Adjust for elevation levels (Bootstrap shadow-sm, shadow, shadow-lg, shadow-xl)
            switch (elevation)
            {
                case MaterialElevation.Level0:
                    opacity *= 0.5f; // shadow-sm
                    break;
                case MaterialElevation.Level1:
                    // shadow (default)
                    break;
                case MaterialElevation.Level2:
                    opacity *= 1.2f; // shadow-lg
                    break;
                case MaterialElevation.Level3:
                    opacity *= 1.5f; // shadow-xl
                    break;
                case MaterialElevation.Level4:
                    opacity *= 2.0f; // shadow-xl enhanced
                    break;
                case MaterialElevation.Level5:
                    opacity *= 2.5f; // shadow-xl max
                    break;
            }

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, opacity, blur);
        }
    }
}
