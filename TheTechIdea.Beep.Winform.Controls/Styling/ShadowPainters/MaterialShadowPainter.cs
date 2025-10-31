using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material (legacy) shadow painter - Google Material Design (v2)
    /// Material UX: Elevation-based shadows with penumbra + umbra
    /// </summary>
    public static class MaterialShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            GraphicsPath remainingPath = (GraphicsPath)path.Clone();

            // Material Design shadows: Elevation system
            switch (state)
            {
                case ControlState.Hovered:
                    remainingPath = ShadowPainterHelpers.PaintFloatingShadow(g, path, radius, elevation: 4);
                    break;

                case ControlState.Pressed:
                    remainingPath = ShadowPainterHelpers.PaintFloatingShadow(g, path, radius, elevation: 8);
                    break;

                case ControlState.Selected:
                    // Material selected: Elevated with accent shadow
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(33, 150, 243);
                    remainingPath = ShadowPainterHelpers.PaintDoubleShadow(
                        g, path, radius,
                        color1: Color.FromArgb(70, accentColor),
                        color2: Color.FromArgb(40, 0, 0, 0),
                        offset1X: 0, offset1Y: 0,
                        offset2X: 0, offset2Y: 0
                    );
                    break;

                case ControlState.Disabled:
                    // Material disabled: Very subtle shadow (1dp)
                    remainingPath = ShadowPainterHelpers.PaintMaterialShadow(g, path, radius, MaterialElevation.Level0);
                    break;

                case ControlState.Focused:
                    // Material focus: Ripple-like glow
                    Color focusColor = useThemeColors ? theme.AccentColor : Color.FromArgb(33, 150, 243);
                    remainingPath = ShadowPainterHelpers.PaintNeonGlow(
                        g, path, radius,
                        glowColor: Color.FromArgb(100, focusColor),
                        glowRadius: 8,
                        intensity: 0.8f
                    );
                    break;

                default: // Normal
                    // Material default: 2dp elevation (matches MaterialFormPainter)
                    remainingPath = ShadowPainterHelpers.PaintMaterialShadow(g, path, radius, MaterialElevation.Level2);
                    break;
            }

            return remainingPath;
        }
    }
}
