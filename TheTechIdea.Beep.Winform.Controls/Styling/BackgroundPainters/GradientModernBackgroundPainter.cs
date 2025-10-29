using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gradient Modern background painter - Vertical gradient from primary to 30% darker
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class GradientModernBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Gradient Modern: Smooth vertical gradient with state-aware intensity
            Color primaryColor = useThemeColors && theme != null ? theme.PrimaryColor : StyleColors.GetBackground(BeepControlStyle.GradientModern);

            // GradientModern-specific state handling - NO HELPER FUNCTIONS
            // Unique gradient intensity modulation for modern gradient design
            float gradientIntensity = state switch
            {
                ControlState.Hovered =>1.15f,
                ControlState.Pressed =>0.90f,
                ControlState.Selected =>1.25f,
                ControlState.Focused =>1.08f,
                ControlState.Disabled =>0.60f,
                _ =>1.0f,
            };

            Color secondary = ControlPaint.Dark(primaryColor, gradientIntensity);
            var bounds = path.GetBounds();
            var gradientBrush = PaintersFactory.GetLinearGradientBrush(bounds, primaryColor, secondary, LinearGradientMode.Vertical);
            g.FillPath(gradientBrush, path);
        }
    }
}
