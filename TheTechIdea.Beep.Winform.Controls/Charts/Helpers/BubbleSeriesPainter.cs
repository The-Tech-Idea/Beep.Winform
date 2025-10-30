using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class BubbleSeriesPainter : IChartSeriesPainter
    {
        private BaseControl _owner;
        public void Initialize(BaseControl owner) => _owner = owner;

        public void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            List<Color> palette, Color axisColor, Color textColor,
            SeriesRenderOptions options)
        {
            float maxBubbleSize = 50f;
            float minBubbleSize = 5f;
            float maxValue = data.SelectMany(s => s.Points).DefaultIfEmpty(new ChartDataPoint { Value = 1 }).Max(p => p.Value);
            float anim = Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);

            int sIndex = 0;
            foreach (var series in data)
            {
                if (!series.Visible) continue;
                Color color = series.Color != Color.Empty ? series.Color : palette[sIndex % palette.Count];

                foreach (var p in series.Points)
                {
                    float xv = toX(p) is float xf ? xf : 0f;
                    float yv = toY(p) is float yf ? yf : 0f;
                    yv = yMin + (yv - yMin) * anim;
                    float sx = plotRect.Left + (xv - xMin) / (xMax - xMin) * plotRect.Width;
                    float sy = plotRect.Bottom - (yv - yMin) / (yMax - yMin) * plotRect.Height;

                    float bubbleSize = maxValue > 0 ? p.Value / maxValue * maxBubbleSize : minBubbleSize;
                    bubbleSize = Math.Max(bubbleSize * anim, minBubbleSize);

                    var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, color));
                    var pen = PaintersFactory.GetPen(axisColor, 1);
                    g.FillEllipse(brush, sx - bubbleSize / 2, sy - bubbleSize / 2, bubbleSize, bubbleSize);
                    g.DrawEllipse(pen, sx - bubbleSize / 2, sy - bubbleSize / 2, bubbleSize, bubbleSize);
                }
                sIndex++;
            }
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit) { }
    }
}
