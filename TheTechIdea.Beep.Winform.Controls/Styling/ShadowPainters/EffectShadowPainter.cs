using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Effect shadow painter - Dynamic visual effects Style
    /// Effect UX: Dramatic shadows, glows, long shadows, perspective
    /// </summary>
    public static class EffectShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            GraphicsPath remainingPath = (GraphicsPath)path.Clone();

            // State-specific shadow effects
            switch (state)
            {
                case ControlState.Hovered:
                    // Effect hover: Neon glow with vibrant color
                    Color hoverGlow = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(100, 150, 255);
                    remainingPath = ShadowPainterHelpers.PaintNeonGlow(
                        g, path, radius,
                        glowColor: Color.FromArgb(120, hoverGlow),
                        glowRadius: 12,
                        intensity: 0.9f
                    );
                    break;

                case ControlState.Pressed:
                    // Effect pressed: Long shadow
                    Color pressColor = useThemeColors && theme != null ? theme.BackColor : Color.FromArgb(60, 80, 120);
                    remainingPath = ShadowPainterHelpers.PaintLongShadow(
                        g, path, radius,
                        angle: 135, // Bottom-right
                        length: 20,
                        shadowColor: Color.FromArgb(60, pressColor)
                    );
                    break;

                case ControlState.Selected:
                    // Effect selected: Double colored glow
                    Color accentColor = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(120, 100, 255);
                    remainingPath = ShadowPainterHelpers.PaintDoubleShadow(
                        g, path, radius,
                        color1: Color.FromArgb(80, accentColor),
                        color2: Color.FromArgb(60, accentColor.R, accentColor.G, Math.Min(255, accentColor.B + 50)),
                        offset1X: 0, offset1Y: 0,
                        offset2X: 0, offset2Y: 0
                    );
                    break;

                case ControlState.Disabled:
                    remainingPath = ShadowPainterHelpers.PaintCardShadow(g, path, radius, ShadowPainterHelpers.CardShadowStyle.Small);
                    break;

                case ControlState.Focused:
                    // Effect focused: Perspective shadow with glow
                    Color focusColor = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(80, 150, 255);
                    remainingPath = ShadowPainterHelpers.PaintPerspectiveShadow(
                        g, path, radius,
                        direction: ShadowPainterHelpers.PerspectiveDirection.BottomRight,
                        intensity: 0.8f
                    );
                    break;

                default: // Normal
                    // Effect default: Floating shadow
                    remainingPath = ShadowPainterHelpers.PaintFloatingShadow(g, path, radius, elevation: 6);
                    break;
            }

            return remainingPath;
        }
    }
}
