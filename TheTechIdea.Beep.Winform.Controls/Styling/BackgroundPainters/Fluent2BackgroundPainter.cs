using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Fluent 2 background painter - Windows 11 era Fluent Design
    /// Clean solid backgrounds with refined state transitions
    /// </summary>
    public static class Fluent2BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal,
            ElevationLevel elevation = ElevationLevel.Level2)
        {
            if (g == null || path == null) return;

            // Fluent 2: clean system background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Fluent2);

            // Use ElevationSystem for state-aware elevation transitions
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(elevation, state);

            // Fluent 2 uses strong, noticeable state changes
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);

            // Fluent 2: Add subtle acrylic effect overlay for depth (Fluent 2 uses acrylic materials)
            if (state != ControlState.Disabled)
            {
                var bounds = path.GetBounds();
                if (bounds.Width > 0 && bounds.Height > 0)
                {
                    // Add subtle top highlight for acrylic effect
                    var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height / 4f);
                    var acrylicHighlight = PaintersFactory.GetLinearGradientBrush(
                        topRect,
                        Color.FromArgb(30, Color.White),
                        Color.Transparent,
                        LinearGradientMode.Vertical);
                    
                    using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
                    {
                        g.FillRectangle(acrylicHighlight, topRect);
                    }

                    // Fluent 2 buttons have radial highlights - add radial gradient for depth
                    Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.03f);
                    Color edgeColor = baseColor;
                    BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
                }
            }
        }
    }
}
