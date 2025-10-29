using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material You background painter - Solid with 8% tonal primary highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MaterialYouBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Material You: Dynamic color with personalized primary tinting
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.MaterialYou);
            Color primaryColor = useThemeColors && theme != null ? theme.PrimaryColor : StyleColors.GetPrimary(BeepControlStyle.MaterialYou);

            // MaterialYou-specific state handling - NO HELPER FUNCTIONS
            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(baseColor.A, (int)(baseColor.R * 0.90f + primaryColor.R * 0.10f), (int)(baseColor.G * 0.90f + primaryColor.G * 0.10f), (int)(baseColor.B * 0.90f + primaryColor.B * 0.10f)),
                ControlState.Pressed => Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - (int)(baseColor.R * 0.08f)), Math.Max(0, baseColor.G - (int)(baseColor.G * 0.08f)), Math.Max(0, baseColor.B - (int)(baseColor.B * 0.08f))),
                ControlState.Selected => Color.FromArgb(baseColor.A, (int)(baseColor.R * 0.85f + primaryColor.R * 0.15f), (int)(baseColor.G * 0.85f + primaryColor.G * 0.15f), (int)(baseColor.B * 0.85f + primaryColor.B * 0.15f)),
                ControlState.Focused => Color.FromArgb(baseColor.A, (int)(baseColor.R * 0.93f + primaryColor.R * 0.07f), (int)(baseColor.G * 0.93f + primaryColor.G * 0.07f), (int)(baseColor.B * 0.93f + primaryColor.B * 0.07f)),
                ControlState.Disabled => Color.FromArgb(90, baseColor),
                _ => baseColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);

            // Add tonal primary highlight (8% alpha) - consistent across states
            var tonalBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, primaryColor));
            g.FillPath(tonalBrush, path);
        }
    }
}
