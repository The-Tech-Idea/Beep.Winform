using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// WebFramework shadow painter - Modern Web CSS-like shadows
    /// Web UX: Box-shadow Style with spread, blur, and color
    /// </summary>
    public static class WebFrameworkShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            GraphicsPath remainingPath;
            // Web-Style shadows: CSS box-shadow inspired
            switch (state)
            {
                case ControlState.Hovered:
                    // Web hover: Medium shadow with spread
                    remainingPath = ShadowPainterHelpers.PaintColoredShadow(
                        g, path, radius,
                        baseColor: Color.FromArgb(40, 0, 0, 0),
                        offsetX: 0, offsetY: 4,
                        intensity: 0.4f
                    );
                    break;

                case ControlState.Pressed:
                    // Web pressed: Inner shadow (inset effect)
                    remainingPath = ShadowPainterHelpers.PaintInnerShadow(
                        g, path, radius,
                        depth: 3,
                        shadowColor: Color.FromArgb(50, 0, 0, 0)
                    );
                    break;

                case ControlState.Selected:
                    // Web selected: Colored box-shadow
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(59, 130, 246);
                    remainingPath = ShadowPainterHelpers.PaintColoredShadow(
                        g, path, radius,
                        baseColor: Color.FromArgb(80, accentColor),
                        offsetX: 0, offsetY: 0,
                        intensity: 0.8f
                    );
                    break;

                default:
                    remainingPath = path;
                    break;
            }
            return remainingPath;
        }
    }
}
