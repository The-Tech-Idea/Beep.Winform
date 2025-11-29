using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// iOS background painter - classic iOS design language
    /// Clean surface with subtle translucent overlay and bottom shadow
    /// </summary>
    public static class iOSBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // iOS: clean white surface
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.iOS15);

            // Solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // iOS translucent overlay effect
            var overlayBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(8, 255, 255, 255));
            g.FillPath(overlayBrush, path);

            // Subtle bottom shadow for depth
            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 1) return;

            var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(15, 0, 0, 0));
            using (var bottomRegion = new Region(path))
            using (var clipRect = new GraphicsPath())
            {
                clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Bottom - 1, bounds.Width, 1));
                bottomRegion.Intersect(clipRect);
                g.SetClip(bottomRegion, CombineMode.Replace);
                g.FillPath(shadowBrush, path);
                g.ResetClip();
            }
        }

        /// <summary>
        /// Legacy overload without state
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, path, style, theme, useThemeColors, ControlState.Normal);
        }
    }
}
