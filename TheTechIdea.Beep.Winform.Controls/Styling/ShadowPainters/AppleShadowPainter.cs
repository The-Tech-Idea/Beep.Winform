using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Apple shadow painter - Apple Design System
    /// Apple UX: Subtle, clean shadows with minimal noise
    /// </summary>
    public static class AppleShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            GraphicsPath remainingPath = (GraphicsPath)path.Clone();

            // State-specific shadow intensity
            switch (state)
            {
                case ControlState.Hovered:
                case ControlState.Pressed:
                    remainingPath = ShadowPainterHelpers.PaintCardShadow(g, path, radius, ShadowPainterHelpers.CardShadowStyle.Small);
                    break;
                case ControlState.Selected:
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 122, 255);
                    remainingPath = ShadowPainterHelpers.PaintColoredShadow(g, path, radius, accentColor, 0, 0, 0.8f);
                    break;
                case ControlState.Disabled:
                    return path;
            }
            return remainingPath;
        }
    }
}
