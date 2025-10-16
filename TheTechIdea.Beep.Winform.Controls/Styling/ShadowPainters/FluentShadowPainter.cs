using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Fluent (legacy) shadow painter - Microsoft Fluent Design System
    /// Fluent UX: Depth through layered shadows, reveal lighting
    /// </summary>
    public static class FluentShadowPainter
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
                    remainingPath = ShadowPainterHelpers.PaintFloatingShadow(g, path, radius, 4);
                    break;
                case ControlState.Pressed:
                    remainingPath = ShadowPainterHelpers.PaintCardShadow(g, path, radius, CardShadowStyle.Small);
                    break;
                case ControlState.Selected:
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 120, 212);
                    remainingPath = ShadowPainterHelpers.PaintAmbientShadow(g, path, radius, Color.FromArgb(60, accentColor), 8, 0.7f);
                    break;
                case ControlState.Disabled:
                    return path;
            }
            return remainingPath;
        }
    }
}
