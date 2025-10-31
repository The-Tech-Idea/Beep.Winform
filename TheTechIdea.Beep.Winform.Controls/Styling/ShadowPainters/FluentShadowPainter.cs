using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

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
            if (!StyleShadows.HasShadow(style)) return path;

            // State-specific shadow effects
            switch (state)
            {
                case ControlState.Hovered:
                    return ShadowPainterHelpers.PaintFloatingShadow(g, path, radius, 4);
                case ControlState.Pressed:
                    return ShadowPainterHelpers.PaintCardShadow(g, path, radius, ShadowPainterHelpers.CardShadowStyle.Small);
                case ControlState.Selected:
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 120, 212);
                    return ShadowPainterHelpers.PaintAmbientShadow(g, path, radius, spread: 8, opacity: 0.7f);
                case ControlState.Disabled:
                    return path;
            }
            // Match FluentFormPainter: subtle shadow for acrylic effect
            return ShadowPainterHelpers.PaintDropShadow(
                g, path, radius,
                0, 2, 6, // offsetX, offsetY, blur
                Color.FromArgb(40, 0, 0, 0), // subtle shadow
                0.3f);
        }
    }
}
