using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    /// <summary>
    /// Shared data-label rendering for Cartesian chart painters
    /// (Line, Bar, Area, Scatter).  Called after the main draw
    /// when <see cref="SeriesRenderOptions.ShowDataLabels"/> is true
    /// and the series has <c>ShowLabel</c> enabled.
    /// </summary>
    internal static class ChartDataLabelHelper
    {
        public static void DrawDataLabels(Graphics g,
            List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX,
            Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            Rectangle plotRect,
            SeriesRenderOptions options,
            Color textColor)
        {
            if (options?.ShowDataLabels != true) return;
            if (data == null) return;

            float anim = Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);
            var labelFont = BeepThemesManager.ToFont("Segoe UI", 7.5f, FontWeight.Normal, FontStyle.Regular);

            for (int sIdx = 0; sIdx < data.Count; sIdx++)
            {
                var series = data[sIdx];
                if (series == null || !series.ShowLabel || !series.Visible
                    || series.Points == null) continue;

                foreach (var p in series.Points)
                {
                    float x = toX(p) is float xf ? xf : 0f;
                    float y = toY(p) is float yf ? yf : 0f;
                    y = yMin + (y - yMin) * anim;

                    float xRange = xMax - xMin;
                    float yRange = yMax - yMin;
                    float sx = xRange > 0
                        ? plotRect.Left + (x - xMin) / xRange * plotRect.Width
                        : plotRect.Left + plotRect.Width * 0.5f;
                    float sy = yRange > 0
                        ? plotRect.Bottom - (y - yMin) / yRange * plotRect.Height
                        : plotRect.Top + plotRect.Height * 0.5f;

                    // Use the point's Label if set, otherwise format Value.
                    string label = !string.IsNullOrEmpty(p.Label)
                        ? p.Label
                        : p.Value.ToString("F1");

                    var sz = TextRenderer.MeasureText(g, label, labelFont);
                    float lx = sx - sz.Width / 2f;
                    float ly = sy - sz.Height - 5;
                    if (ly < plotRect.Top) ly = sy + 3;
                    TextRenderer.DrawText(g, label, labelFont,
                        new Point((int)lx, (int)ly), textColor);
                }
            }
        }
    }
}

