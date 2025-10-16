using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// WebFramework shadow painter - Modern Web CSS-like shadows
    /// Web UX: Box-shadow style with spread, blur, and color
    /// </summary>
    public static class WebFrameworkShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            GraphicsPath remainingPath;
            // Web-style shadows: CSS box-shadow inspired
            switch (state)
            {
                case ControlState.Hovered:
                    // Web hover: Medium shadow with spread
                    remainingPath = ShadowPainterHelpers.PaintColoredShadow(
                        g, path, radius,
                        offsetX: 0, offsetY: 4,
                        blur: 12,
                        shadowColor: Color.FromArgb(40, 0, 0, 0),
                        spreadRadius: 2
                    );
                    break;

                case ControlState.Pressed:
                    // Web pressed: Inner shadow (inset effect)
                    remainingPath = ShadowPainterHelpers.PaintInnerShadow(
                        g, path, radius,
                        offsetX: 0, offsetY: 2,
                        blur: 4,
                        shadowColor: Color.FromArgb(50, 0, 0, 0),
                        inset: 3
                    );
                    break;

                case ControlState.Selected:
                    // Web selected: Colored box-shadow
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(59, 130, 246);
                    remainingPath = ShadowPainterHelpers.PaintColoredShadow(
                        g, path, radius,
                        offsetX: 0, offsetY: 0,
                        blur: 10,
                        shadowColor: Color.FromArgb(80, accentColor),
                        spreadRadius: 3
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
