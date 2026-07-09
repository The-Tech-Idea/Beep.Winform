using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    /// <summary>
    /// Draws Pie and Doughnut charts.  The <see cref="ChartType.Doughnut"/>
    /// variant uses the same painter with an inner ellipse cutout
    /// (hole ratio controlled by
    /// <see cref="SeriesRenderOptions.DoughnutHoleRatio"/>).
    /// </summary>
    internal sealed class PieSeriesPainter : IChartSeriesPainter
    {
        private BaseControl _owner;
        public void Initialize(BaseControl owner) => _owner = owner;

        public void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            List<Color> palette, Color axisColor, Color textColor,
            SeriesRenderOptions options)
        {
            var series = data.FirstOrDefault();
            if (series == null || !series.Visible || series.Points == null || !series.Points.Any()) return;

            float totalValue = series.Points.Sum(p => p.Value);
            if (totalValue <= 0) return;

            int pieDiameter = Math.Min(plotRect.Width, plotRect.Height) - 20;
            Rectangle pieRect = new Rectangle(
                plotRect.Left + (plotRect.Width - pieDiameter) / 2,
                plotRect.Top + (plotRect.Height - pieDiameter) / 2,
                pieDiameter,
                pieDiameter);

            // Doughnut hole: carve out the centre using a clip region.
            // FillPie draws full slices; the clip omits the inner disc.
            float holeRatio = options?.DoughnutHoleRatio ?? 0f;
            var isDoughnut = holeRatio > 0f;
            Region originalClip = null;
            if (isDoughnut)
            {
                originalClip = g.Clip;
                float cx = pieRect.Left + pieRect.Width / 2f;
                float cy = pieRect.Top + pieRect.Height / 2f;
                float holeRadius = pieRect.Width / 2f * holeRatio;
                var outerPath = new GraphicsPath();
                outerPath.AddEllipse(pieRect);
                var region = new Region(outerPath);
                var innerPath = new GraphicsPath();
                innerPath.AddEllipse(cx - holeRadius, cy - holeRadius, holeRadius * 2, holeRadius * 2);
                region.Exclude(innerPath);
                g.SetClip(region, CombineMode.Intersect);
                outerPath.Dispose();
                innerPath.Dispose();
            }

            float startAngle = 0f;
            int colorIndex = 0;
            foreach (var p in series.Points)
            {
                float sliceValue = p.Value;
                if (sliceValue <= 0) continue;
                float sweepAngle = sliceValue / totalValue * 360f * Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);
                Color color = p.Color != Color.Empty ? p.Color : palette[colorIndex % palette.Count];
                var brush = PaintersFactory.GetSolidBrush(color);
                var pen = PaintersFactory.GetPen(axisColor, 1);
                g.FillPie(brush, pieRect, startAngle, sweepAngle);
                g.DrawPie(pen, pieRect, startAngle, sweepAngle);
                startAngle += sweepAngle;
                colorIndex++;
            }

            // Restore the clip so legend/other elements render outside the doughnut
            if (isDoughnut) { g.Clip = originalClip; }

            // Data labels in the centre of each slice
            if (options?.ShowDataLabels == true && series.ShowLabel)
            {
                DrawSliceLabels(g, series, pieRect, totalValue, textColor);
            }
        }

        private static void DrawSliceLabels(Graphics g, ChartDataSeries series,
            Rectangle pieRect, float totalValue, Color textColor)
        {
            var labelFont = BeepThemesManager.ToFont("Segoe UI", 8f, FontWeight.Normal, FontStyle.Regular);

            float cx = pieRect.Left + pieRect.Width / 2f;
            float cy = pieRect.Top + pieRect.Height / 2f;
            float labelRadius = Math.Min(pieRect.Width, pieRect.Height) * 0.35f;
            float startAngle = 0f;

            foreach (var p in series.Points)
            {
                if (p.Value <= 0) { startAngle += 0; continue; }
                float sweepAngle = p.Value / totalValue * 360f;
                float midAngle = startAngle + sweepAngle / 2f;
                float rad = (float)(midAngle * Math.PI / 180d);
                float lx = cx + (float)Math.Cos(rad) * labelRadius;
                float ly = cy + (float)Math.Sin(rad) * labelRadius;

                string label = !string.IsNullOrEmpty(p.Label)
                    ? p.Label
                    : $"{p.Value / totalValue * 100f:F0}%";
                var sz = TextRenderer.MeasureText(g, label, labelFont);
                TextRenderer.DrawText(g, label, labelFont,
                    new Point((int)(lx - sz.Width / 2f), (int)(ly - sz.Height / 2f)), textColor);

                startAngle += sweepAngle;
            }
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit)
        {
            var series = data?.FirstOrDefault();
            if (series == null || !series.Visible || series.Points == null || !series.Points.Any()) return;

            float totalValue = series.Points.Sum(p => p.Value);
            if (totalValue <= 0) return;

            int pieDiameter = Math.Min(plotRect.Width, plotRect.Height) - 20;
            if (pieDiameter <= 0) return;

            Rectangle pieRect = new Rectangle(
                plotRect.Left + (plotRect.Width - pieDiameter) / 2,
                plotRect.Top + (plotRect.Height - pieDiameter) / 2,
                pieDiameter,
                pieDiameter);

            float centerX = pieRect.Left + (pieRect.Width / 2f);
            float centerY = pieRect.Top + (pieRect.Height / 2f);
            float anchorRadius = pieRect.Width * 0.35f;

            float startAngle = 0f;
            const int hitSize = 16;
            for (int pointIndex = 0; pointIndex < series.Points.Count; pointIndex++)
            {
                float sliceValue = series.Points[pointIndex].Value;
                if (sliceValue <= 0) continue;

                float sweepAngle = sliceValue / totalValue * 360f;
                float midRadians = (float)((startAngle + (sweepAngle / 2f)) * (Math.PI / 180d));

                float anchorX = centerX + (float)Math.Cos(midRadians) * anchorRadius;
                float anchorY = centerY + (float)Math.Sin(midRadians) * anchorRadius;

                var hitRect = new Rectangle((int)anchorX - (hitSize / 2), (int)anchorY - (hitSize / 2), hitSize, hitSize);
                owner.AddHitArea($"PieSlice_{pointIndex}", hitRect, null, () =>
                    notifyAreaHit?.Invoke($"PieSlice_{pointIndex}", hitRect));

                startAngle += sweepAngle;
            }
        }
    }
}

