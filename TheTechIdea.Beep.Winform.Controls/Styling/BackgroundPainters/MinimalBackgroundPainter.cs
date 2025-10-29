using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Minimal background painter - Simple solid background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MinimalBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Minimal: Ultra-subtle changes for true minimalism
            Color backgroundColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Minimal);

            // Minimal-specific state handling - NO HELPER FUNCTIONS
            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.02f)), Math.Min(255, backgroundColor.G + (int)(255 *0.02f)), Math.Min(255, backgroundColor.B + (int)(255 *0.02f))),
                ControlState.Pressed => Color.FromArgb(backgroundColor.A, Math.Max(0, backgroundColor.R - (int)(backgroundColor.R *0.03f)), Math.Max(0, backgroundColor.G - (int)(backgroundColor.G *0.03f)), Math.Max(0, backgroundColor.B - (int)(backgroundColor.B *0.03f))),
                ControlState.Selected => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.04f)), Math.Min(255, backgroundColor.G + (int)(255 *0.04f)), Math.Min(255, backgroundColor.B + (int)(255 *0.04f))),
                ControlState.Focused => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.015f)), Math.Min(255, backgroundColor.G + (int)(255 *0.015f)), Math.Min(255, backgroundColor.B + (int)(255 *0.015f))),
                ControlState.Disabled => Color.FromArgb(120, backgroundColor),
                _ => backgroundColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);

            // Minimal Style: whisper-light vertical highlight for understated depth
            var bounds = path.GetBounds();
            var gradient = PaintersFactory.GetLinearGradientBrush(bounds, Color.FromArgb(12,255,255,255), Color.FromArgb(0,255,255,255), LinearGradientMode.Vertical);
            g.FillPath(gradient, path);
        }
    }
}
