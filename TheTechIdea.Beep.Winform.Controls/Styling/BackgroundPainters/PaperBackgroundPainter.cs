using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Paper background painter - natural paper/parchment texture
    /// Off-white background with subtle noise for paper grain effect
    /// Warm, organic aesthetic reminiscent of physical paper
    /// </summary>
    public static class PaperBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Paper: warm off-white (like real paper)
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xFA, 0xFA, 0xF8);

            // Solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = Rectangle.Round(path.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Paper grain noise effect (deterministic based on bounds)
                int seed = unchecked(bounds.GetHashCode() * 31);
                var rand = new Random(seed);
                var noiseBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(6, 180, 180, 175));
                int noiseCount = Math.Max(15, (bounds.Width * bounds.Height) / 2000);
                
                for (int i = 0; i < noiseCount; i++)
                {
                    int x = rand.Next(bounds.Left, bounds.Right);
                    int y = rand.Next(bounds.Top, bounds.Bottom);
                    g.FillRectangle(noiseBrush, x, y, 1, 1);
                }

                // Subtle top edge highlight (paper fold effect)
                var pen = PaintersFactory.GetPen(Color.FromArgb(30, Color.White), 1f);
                g.DrawLine(pen, bounds.Left, bounds.Top + 0.5f, bounds.Right, bounds.Top + 0.5f);
            }
        }
    }
}
