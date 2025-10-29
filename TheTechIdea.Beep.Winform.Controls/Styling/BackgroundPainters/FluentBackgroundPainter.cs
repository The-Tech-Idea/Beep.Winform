using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Fluent (legacy) background painter - Microsoft Fluent Design System
    /// Fluent UX: Acrylic materials, subtle transparency, depth through layering
    /// </summary>
    public static class FluentBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Fluent Style: Semi-transparent with subtle lighting
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Fluent);

            // Fluent-specific state handling - subtle reveals
            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R +26), Math.Min(255, baseColor.G +26), Math.Min(255, baseColor.B +26)),
                ControlState.Pressed => Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - (int)(baseColor.R *0.08f)), Math.Max(0, baseColor.G - (int)(baseColor.G *0.08f)), Math.Max(0, baseColor.B - (int)(baseColor.B *0.08f))),
                ControlState.Selected => Color.FromArgb(200, (useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0,120,212)).R, (useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0,120,212)).G, (useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0,120,212)).B),
                ControlState.Disabled => Color.FromArgb(80, baseColor.R, baseColor.G, baseColor.B),
                ControlState.Focused => Color.FromArgb(baseColor.A, Math.Min(255, baseColor.R +31), Math.Min(255, baseColor.G +31), Math.Min(255, baseColor.B +31)),
                _ => baseColor,
            };

            // Fluent Style: Subtle acrylic-like gradient
            var bounds = path.GetBounds();
            // Create the secondary color for the gradient
            Color secondary = Color.FromArgb(stateColor.A, Math.Max(0, stateColor.R -8), Math.Max(0, stateColor.G -8), Math.Max(0, stateColor.B -8));

            // Use PaintersFactory for the base gradient brush (single color-to-color fallback)
            var brush = PaintersFactory.GetLinearGradientBrush(bounds, stateColor, secondary, LinearGradientMode.ForwardDiagonal);

            // Build local ColorBlend for interpolation if we want custom stops
            var blend = new ColorBlend();
            blend.Colors = new Color[] { stateColor, Color.FromArgb(stateColor.A, Math.Min(255, stateColor.R +5), Math.Min(255, stateColor.G +5), Math.Min(255, stateColor.B +5)), stateColor };
            blend.Positions = new float[] {0f,0.5f,1f };
            try
            {
                brush.InterpolationColors = blend;
            }
            catch { }

            g.FillPath(brush, path);
        }
    }
}
