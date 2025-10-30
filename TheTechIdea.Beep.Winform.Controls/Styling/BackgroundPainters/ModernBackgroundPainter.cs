using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class ModernBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            // Modern style - clean, sophisticated look with subtle gradient
            // Use panel gradient colors from theme for modern feel
            Color startColor = useThemeColors && theme != null ? (theme.PanelGradiantStartColor != Color.Empty ? theme.PanelGradiantStartColor : theme.BackgroundColor) : Color.FromArgb(255, 255, 255);
            Color endColor = useThemeColors && theme != null ? (theme.PanelGradiantEndColor != Color.Empty ? theme.PanelGradiantEndColor : startColor) : Color.FromArgb(250, 250, 252);
            
            // Apply state variations if needed
            if (state != ControlState.Normal)
            {
                startColor = BackgroundPainterHelpers.ApplyState(startColor, state);
                endColor = BackgroundPainterHelpers.ApplyState(endColor, state);
            }

            var bounds = path.GetBounds();
            
            // Modern look: subtle vertical gradient with very faint white highlight
            var gradient = PaintersFactory.GetLinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Vertical);
            g.FillPath(gradient, path);
            
            // Add a subtle modern highlight at the top for depth
            var highlight = PaintersFactory.GetLinearGradientBrush(bounds, Color.FromArgb(20, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical);
            g.FillPath(highlight, path);
        }
    }
}
