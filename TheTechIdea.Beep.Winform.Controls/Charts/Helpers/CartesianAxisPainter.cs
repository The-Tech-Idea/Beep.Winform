using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class CartesianAxisPainter : IChartAxisPainter
    {
        public AxisLayout AdjustPlotRect(Graphics g, AxisLayout ctx)
        {
            int left = ctx.Bounds.Left + 48;
            int right = ctx.Bounds.Right - 48;
            int top = ctx.Bounds.Top + 48;
            int bottom = ctx.Bounds.Bottom - 48;
            ctx.PlotRect = Rectangle.FromLTRB(left, top, right, bottom);
            return ctx;
        }

        public void DrawAxes(Graphics g, AxisLayout ctx)
        {
            using var axisPen = new Pen(ctx.AxisColor, 1);
            g.DrawLine(axisPen, ctx.PlotRect.Left, ctx.PlotRect.Bottom, ctx.PlotRect.Right, ctx.PlotRect.Bottom);
            g.DrawLine(axisPen, ctx.PlotRect.Left, ctx.PlotRect.Bottom, ctx.PlotRect.Left, ctx.PlotRect.Top);
            using var textBrush = new SolidBrush(ctx.TextColor);
            if (!string.IsNullOrEmpty(ctx.XTitle))
            {
                var size = g.MeasureString(ctx.XTitle, ctx.TitleFont);
                var x = ctx.PlotRect.Left + (ctx.PlotRect.Width - size.Width) / 2f;
                g.DrawString(ctx.XTitle, ctx.TitleFont, textBrush, x, ctx.PlotRect.Bottom + 8);
            }
            if (!string.IsNullOrEmpty(ctx.YTitle))
            {
                var size = g.MeasureString(ctx.YTitle, ctx.TitleFont);
                float x = ctx.PlotRect.Left - size.Height - 12;
                float y = ctx.PlotRect.Top + (ctx.PlotRect.Height + size.Width) / 2f;
                g.TranslateTransform(x, y);
                g.RotateTransform(-90);
                g.DrawString(ctx.YTitle, ctx.TitleFont, textBrush, 0, 0);
                g.ResetTransform();
            }
        }

        public void DrawTicks(Graphics g, AxisLayout ctx)
        {
            using var labelBrush = new SolidBrush(ctx.TextColor);
            using var tickPen = new Pen(ctx.GridColor, 1);

            if (ctx.BottomAxisType == AxisType.Numeric)
            {
                DrawNumericTicks(g, ctx, isXAxis: true, ctx.XMin, ctx.XMax, 5, ctx.LabelFont, labelBrush, tickPen, ctx.XLabelAngle);
            }
            else if (ctx.BottomAxisType == AxisType.Date)
            {
                DrawDateTicks(g, ctx, isXAxis: true, ctx.XMin, ctx.XMax, ctx.XDateMin, 5, ctx.LabelFont, labelBrush, tickPen, ctx.XLabelAngle);
            }
            else
            {
                DrawTextTicks(g, ctx, isXAxis: true, ctx.XCategories, ctx.LabelFont, labelBrush, tickPen, ctx.XLabelAngle);
            }

            if (ctx.LeftAxisType == AxisType.Numeric)
            {
                DrawNumericTicks(g, ctx, isXAxis: false, ctx.YMin, ctx.YMax, 5, ctx.LabelFont, labelBrush, tickPen, ctx.YLabelAngle);
            }
            else if (ctx.LeftAxisType == AxisType.Date)
            {
                DrawDateTicks(g, ctx, isXAxis: false, ctx.YMin, ctx.YMax, ctx.YDateMin, 5, ctx.LabelFont, labelBrush, tickPen, ctx.YLabelAngle);
            }
            else
            {
                DrawTextTicks(g, ctx, isXAxis: false, ctx.YCategories, ctx.LabelFont, labelBrush, tickPen, ctx.YLabelAngle);
            }
        }

        private void DrawNumericTicks(Graphics g, AxisLayout ctx, bool isXAxis, float minVal, float maxVal, int desiredTicks, Font font, Brush textBrush, Pen grid, float labelAngle)
        {
            if (maxVal <= minVal) return;
            float range = maxVal - minVal;
            float step = NiceStep(range / desiredTicks);
            float start = (float)Math.Floor(minVal / step) * step;
            for (float v = start; v <= maxVal; v += step)
            {
                if (isXAxis)
                {
                    float x = ctx.PlotRect.Left + (v - minVal) / range * ctx.PlotRect.Width;
                    g.DrawLine(grid, x, ctx.PlotRect.Top, x, ctx.PlotRect.Bottom);
                    DrawRotated(g, v.ToString("0.##"), font, textBrush, x, ctx.PlotRect.Bottom + 4, labelAngle, true);
                }
                else
                {
                    float y = ctx.PlotRect.Bottom - (v - minVal) / range * ctx.PlotRect.Height;
                    g.DrawLine(grid, ctx.PlotRect.Left, y, ctx.PlotRect.Right, y);
                    DrawRotated(g, v.ToString("0.##"), font, textBrush, ctx.PlotRect.Left - 6, y, labelAngle, false);
                }
            }
        }

        private void DrawDateTicks(Graphics g, AxisLayout ctx, bool isXAxis, float minVal, float maxVal, DateTime dateMin, int desiredTicks, Font font, Brush textBrush, Pen grid, float labelAngle)
        {
            if (maxVal <= minVal) return;
            float range = maxVal - minVal;

            // Determine granularity
            TimeSpan span = TimeSpan.FromDays(range);
            Func<DateTime, string> format = d => d.ToString("MM/dd");
            double stepUnits;
            if (span.TotalDays <= 2) { format = d => d.ToString("HH:mm"); stepUnits = 1.0/24.0; }
            else if (span.TotalDays <= 31) { format = d => d.ToString("MM/dd"); stepUnits = Math.Max(1, (int)(range / desiredTicks)); }
            else if (span.TotalDays <= 365) { format = d => d.ToString("MMM yyyy"); stepUnits = 30; }
            else { format = d => d.ToString("yyyy"); stepUnits = 365; }

            float step = (float)stepUnits;
            float start = (float)Math.Floor(minVal / step) * step;

            for (float v = start; v <= maxVal; v += step)
            {
                DateTime tickDate = dateMin.AddDays(v);
                string label = format(tickDate);
                if (isXAxis)
                {
                    float x = ctx.PlotRect.Left + (v - minVal) / range * ctx.PlotRect.Width;
                    g.DrawLine(grid, x, ctx.PlotRect.Top, x, ctx.PlotRect.Bottom);
                    DrawRotated(g, label, font, textBrush, x, ctx.PlotRect.Bottom + 4, labelAngle, true);
                }
                else
                {
                    float y = ctx.PlotRect.Bottom - (v - minVal) / range * ctx.PlotRect.Height;
                    g.DrawLine(grid, ctx.PlotRect.Left, y, ctx.PlotRect.Right, y);
                    DrawRotated(g, label, font, textBrush, ctx.PlotRect.Left - 6, y, labelAngle, false);
                }
            }
        }

        private void DrawTextTicks(Graphics g, AxisLayout ctx, bool isXAxis, System.Collections.Generic.Dictionary<string,int> categories, Font font, Brush textBrush, Pen grid, float labelAngle)
        {
            if (categories == null || categories.Count == 0) return;
            foreach (var kvp in categories)
            {
                float v = kvp.Value;
                string label = kvp.Key;
                if (isXAxis)
                {
                    float x = ctx.PlotRect.Left + (v - ctx.XMin) / (ctx.XMax - ctx.XMin) * ctx.PlotRect.Width;
                    g.DrawLine(grid, x, ctx.PlotRect.Top, x, ctx.PlotRect.Bottom);
                    DrawRotated(g, label, font, textBrush, x, ctx.PlotRect.Bottom + 4, labelAngle, true);
                }
                else
                {
                    float y = ctx.PlotRect.Bottom - (v - ctx.YMin) / (ctx.YMax - ctx.YMin) * ctx.PlotRect.Height;
                    g.DrawLine(grid, ctx.PlotRect.Left, y, ctx.PlotRect.Right, y);
                    DrawRotated(g, label, font, textBrush, ctx.PlotRect.Left - 6, y, labelAngle, false);
                }
            }
        }

        private static void DrawRotated(Graphics g, string text, Font font, Brush brush, float x, float y, float angle, bool centerX)
        {
            if (angle == 0)
            {
                var size = g.MeasureString(text, font);
                g.DrawString(text, font, brush, centerX ? x - size.Width / 2 : x - size.Width - 4, y);
                return;
            }
            var size2 = g.MeasureString(text, font);
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.DrawString(text, font, brush, centerX ? -size2.Width / 2 : -size2.Width, 0);
            g.ResetTransform();
        }

        private static float NiceStep(float roughStep)
        {
            float exp = (float)Math.Floor(Math.Log10(roughStep));
            float fraction = roughStep / (float)Math.Pow(10, exp);
            float niceFraction = fraction <= 1 ? 1 : fraction <= 2 ? 2 : fraction <= 5 ? 5 : 10;
            return niceFraction * (float)Math.Pow(10, exp);
        }

        public void UpdateHitAreas(BaseControl owner, AxisLayout ctx, Action<string, Rectangle> notifyAreaHit)
        {
            var xAxisArea = new Rectangle(ctx.PlotRect.Left, ctx.PlotRect.Bottom, ctx.PlotRect.Width, 24);
            var yAxisArea = new Rectangle(ctx.PlotRect.Left - 24, ctx.PlotRect.Top, 24, ctx.PlotRect.Height);
            owner.AddHitArea("XAxis", xAxisArea, null, () => notifyAreaHit?.Invoke("XAxis", xAxisArea));
            owner.AddHitArea("YAxis", yAxisArea, null, () => notifyAreaHit?.Invoke("YAxis", yAxisArea));
        }
    }
}
