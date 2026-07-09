using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class CartesianAxisPainter : IChartAxisPainter
    {
        private const int MinAxisMarginLogical = 36;

        public AxisLayout AdjustPlotRect(Graphics g, AxisLayout ctx)
        {
            var font = ctx.LabelFont ?? SystemFonts.DefaultFont;
            float dpi = ctx.DpiScale > 0f ? ctx.DpiScale : 1f;
            int S(int px) => DpiScalingHelper.ScaleValue(px, dpi);

            // Measure actual Y-axis label widths
            int leftPad = S(MinAxisMarginLogical);
            if (ctx.LeftAxisType == AxisType.Numeric && ctx.YMax != ctx.YMin)
            {
                int maxW = 0;
                foreach (var v in new[] { ctx.YMin, ctx.YMax, (ctx.YMin + ctx.YMax) / 2f,
                                          ctx.YMin + (ctx.YMax - ctx.YMin) * 0.25f,
                                          ctx.YMin + (ctx.YMax - ctx.YMin) * 0.75f })
                {
                    var sz = TextRenderer.MeasureText(g, FormatAxisValue(v), font,
                        System.Drawing.Size.Empty, TextFormatFlags.NoPadding);
                    if (sz.Width > maxW) maxW = sz.Width;
                }
                leftPad = Math.Max(S(MinAxisMarginLogical), maxW + S(16));
            }
            else if (ctx.LeftAxisType == AxisType.Text && ctx.YCategories?.Count > 0)
            {
                int maxW = ctx.YCategories.Keys.Max(k =>
                    TextRenderer.MeasureText(g, k, font, System.Drawing.Size.Empty,
                        TextFormatFlags.NoPadding).Width);
                leftPad = Math.Max(S(MinAxisMarginLogical), maxW + S(16));
            }

            if (!string.IsNullOrEmpty(ctx.YTitle))
                leftPad += S(18);

            // Measure X-axis label height, accounting for rotation
            int bottomPad = S(MinAxisMarginLogical);
            float angle = ctx.XLabelAngle;
            float angleRad = angle * MathF.PI / 180f;
            int labelFontHeight = font.Height;
            int maxLabelW = 0;
            if (ctx.BottomAxisType == AxisType.Numeric && ctx.XMax != ctx.XMin)
            {
                foreach (var v in new[] { ctx.XMin, ctx.XMax, (ctx.XMin + ctx.XMax) / 2f })
                {
                    var sz = TextRenderer.MeasureText(g, FormatAxisValue(v), font,
                        System.Drawing.Size.Empty, TextFormatFlags.NoPadding);
                    if (sz.Width > maxLabelW) maxLabelW = sz.Width;
                }
            }
            else if (ctx.BottomAxisType == AxisType.Text && ctx.XCategories?.Count > 0)
            {
                maxLabelW = ctx.XCategories.Keys.Max(k =>
                    TextRenderer.MeasureText(g, k, font, System.Drawing.Size.Empty,
                        TextFormatFlags.NoPadding).Width);
            }

            float absSin = Math.Abs(MathF.Sin(angleRad));
            float absCos = Math.Abs(MathF.Cos(angleRad));
            int labelH = (int)(maxLabelW * absSin + labelFontHeight * absCos);
            if (labelH < labelFontHeight) labelH = labelFontHeight;
            bottomPad = Math.Max(S(MinAxisMarginLogical), labelH + S(14));

            if (!string.IsNullOrEmpty(ctx.XTitle))
                bottomPad += S(20);

            // Right-side legend space — measured from actual names
            int rightPad = S(16);
            if (ctx.ShowLegend && ctx.LegendPlacement == LegendPlacement.Right && ctx.LegendItemCount > 0)
            {
                int maxTextW = S(40);
                if (ctx.XCategories != null && ctx.XCategories.Count > 0)
                {
                    foreach (var k in ctx.XCategories.Keys)
                    {
                        var sz = TextRenderer.MeasureText(g, k, font,
                            System.Drawing.Size.Empty, TextFormatFlags.NoPadding);
                        if (sz.Width > maxTextW) maxTextW = sz.Width;
                    }
                }
                rightPad = maxTextW + S(15 + 6 + 16); // swatch + gap + padding
            }

            int left   = ctx.Bounds.Left   + leftPad;
            int right  = ctx.Bounds.Right  - rightPad;
            int top    = ctx.Bounds.Top    + S(12);
            int bottom = ctx.Bounds.Bottom - bottomPad;
            ctx.PlotRect = Rectangle.FromLTRB(left, top, right, bottom);
            return ctx;
        }

        /// <summary>
        /// Formats a float value for display on an axis, keeping
        /// the string short (max 8 chars) so labels don't
        /// dominate the margin measurement.
        /// </summary>
        private static string FormatAxisValue(float v)
        {
            if (Math.Abs(v) >= 1e6f || (Math.Abs(v) > 0 && Math.Abs(v) < 1e-4f))
                return v.ToString("0.##e0");
            if (Math.Abs(v) >= 1000f)
                return v.ToString("#,##0");
            if (v == MathF.Floor(v))
                return v.ToString("F0");
            return v.ToString("F2");
        }

        public void DrawAxes(Graphics g, AxisLayout ctx)
        {
            var axisPen = PaintersFactory.GetPen(ctx.AxisColor,1);
            g.DrawLine(axisPen, ctx.PlotRect.Left, ctx.PlotRect.Bottom, ctx.PlotRect.Right, ctx.PlotRect.Bottom);
            g.DrawLine(axisPen, ctx.PlotRect.Left, ctx.PlotRect.Bottom, ctx.PlotRect.Left, ctx.PlotRect.Top);
            if (!string.IsNullOrEmpty(ctx.XTitle))
            {
                var size = TextUtils.MeasureText(g, ctx.XTitle, ctx.TitleFont);
                var x = ctx.PlotRect.Left + (ctx.PlotRect.Width - size.Width) /2f;
                TextRenderer.DrawText(g, ctx.XTitle, ctx.TitleFont,
                    new Point((int)x, ctx.PlotRect.Bottom + 8), ctx.TextColor,
                    TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
            }
            if (!string.IsNullOrEmpty(ctx.YTitle))
            {
                var textBrush = PaintersFactory.GetSolidBrush(ctx.TextColor);
                var size = TextUtils.MeasureText(g, ctx.YTitle, ctx.TitleFont);
                float x = ctx.PlotRect.Left - size.Height -12;
                float y = ctx.PlotRect.Top + (ctx.PlotRect.Height + size.Width) /2f;
                g.TranslateTransform(x, y);
                g.RotateTransform(-90);
                g.DrawString(ctx.YTitle, ctx.TitleFont, textBrush,0,0);
                g.ResetTransform();
            }
        }

        public void DrawTicks(Graphics g, AxisLayout ctx)
        {
            var labelBrush = PaintersFactory.GetSolidBrush(ctx.TextColor);
            var tickPen = PaintersFactory.GetPen(ctx.GridColor,1);
            int xLabelInterval = Math.Max(1, ctx.XLabelInterval);
            int yLabelInterval = Math.Max(1, ctx.YLabelInterval);

            if (ctx.BottomAxisType == AxisType.Numeric)
            {
                DrawNumericTicks(g, ctx, isXAxis: true, ctx.XMin, ctx.XMax,5, xLabelInterval, ctx.LabelFont, labelBrush, tickPen, ctx.XLabelAngle);
            }
            else if (ctx.BottomAxisType == AxisType.Date)
            {
                DrawDateTicks(g, ctx, isXAxis: true, ctx.XMin, ctx.XMax, ctx.XDateMin,5, xLabelInterval, ctx.LabelFont, labelBrush, tickPen, ctx.XLabelAngle);
            }
            else
            {
                DrawTextTicks(g, ctx, isXAxis: true, ctx.XCategories, xLabelInterval, ctx.LabelFont, labelBrush, tickPen, ctx.XLabelAngle);
            }

            if (ctx.LeftAxisType == AxisType.Numeric)
            {
                DrawNumericTicks(g, ctx, isXAxis: false, ctx.YMin, ctx.YMax,5, yLabelInterval, ctx.LabelFont, labelBrush, tickPen, ctx.YLabelAngle);
            }
            else if (ctx.LeftAxisType == AxisType.Date)
            {
                DrawDateTicks(g, ctx, isXAxis: false, ctx.YMin, ctx.YMax, ctx.YDateMin,5, yLabelInterval, ctx.LabelFont, labelBrush, tickPen, ctx.YLabelAngle);
            }
            else
            {
                DrawTextTicks(g, ctx, isXAxis: false, ctx.YCategories, yLabelInterval, ctx.LabelFont, labelBrush, tickPen, ctx.YLabelAngle);
            }
        }

        private void DrawNumericTicks(Graphics g, AxisLayout ctx, bool isXAxis, float minVal, float maxVal, int desiredTicks, int labelInterval, Font font, Brush textBrush, Pen grid, float labelAngle)
        {
            if (maxVal <= minVal) return;
            float range = maxVal - minVal;
            float step = NiceStep(range / desiredTicks);
            float start = (float)Math.Floor(minVal / step) * step;
            int tickIndex = 0;
            for (float v = start; v <= maxVal; v += step)
            {
                if ((tickIndex++ % labelInterval) != 0)
                    continue;

                if (isXAxis)
                {
                    float x = ctx.PlotRect.Left + (v - minVal) / range * ctx.PlotRect.Width;
                    g.DrawLine(grid, x, ctx.PlotRect.Top, x, ctx.PlotRect.Bottom);
                    DrawRotated(g, v.ToString("0.##"), font, textBrush, x, ctx.PlotRect.Bottom +4, labelAngle, true);
                }
                else
                {
                    float y = ctx.PlotRect.Bottom - (v - minVal) / range * ctx.PlotRect.Height;
                    g.DrawLine(grid, ctx.PlotRect.Left, y, ctx.PlotRect.Right, y);
                    DrawRotated(g, v.ToString("0.##"), font, textBrush, ctx.PlotRect.Left -6, y, labelAngle, false);
                }
            }
        }

        private void DrawDateTicks(Graphics g, AxisLayout ctx, bool isXAxis, float minVal, float maxVal, DateTime dateMin, int desiredTicks, int labelInterval, Font font, Brush textBrush, Pen grid, float labelAngle)
        {
            if (maxVal <= minVal) return;
            float range = maxVal - minVal;

            // Determine granularity
            TimeSpan span = TimeSpan.FromDays(range);
            Func<DateTime, string> format = d => d.ToString("MM/dd");
            double stepUnits;
            if (span.TotalDays <=2) { format = d => d.ToString("HH:mm"); stepUnits =1.0 /24.0; }
            else if (span.TotalDays <=31) { format = d => d.ToString("MM/dd"); stepUnits = Math.Max(1, (int)(range / desiredTicks)); }
            else if (span.TotalDays <=365) { format = d => d.ToString("MMM yyyy"); stepUnits =30; }
            else { format = d => d.ToString("yyyy"); stepUnits =365; }

            float step = (float)stepUnits;
            float start = (float)Math.Floor(minVal / step) * step;
            int tickIndex = 0;

            for (float v = start; v <= maxVal; v += step)
            {
                if ((tickIndex++ % labelInterval) != 0)
                    continue;

                DateTime tickDate = dateMin.AddDays(v);
                string label = format(tickDate);
                if (isXAxis)
                {
                    float x = ctx.PlotRect.Left + (v - minVal) / range * ctx.PlotRect.Width;
                    g.DrawLine(grid, x, ctx.PlotRect.Top, x, ctx.PlotRect.Bottom);
                    DrawRotated(g, label, font, textBrush, x, ctx.PlotRect.Bottom +4, labelAngle, true);
                }
                else
                {
                    float y = ctx.PlotRect.Bottom - (v - minVal) / range * ctx.PlotRect.Height;
                    g.DrawLine(grid, ctx.PlotRect.Left, y, ctx.PlotRect.Right, y);
                    DrawRotated(g, label, font, textBrush, ctx.PlotRect.Left -6, y, labelAngle, false);
                }
            }
        }

        private void DrawTextTicks(Graphics g, AxisLayout ctx, bool isXAxis, System.Collections.Generic.Dictionary<string, int> categories, int labelInterval, Font font, Brush textBrush, Pen grid, float labelAngle)
        {
            if (categories == null || categories.Count ==0) return;
            int tickIndex = 0;
            foreach (var kvp in categories.OrderBy(c => c.Value))
            {
                if ((tickIndex++ % labelInterval) != 0)
                    continue;

                float v = kvp.Value;
                string label = kvp.Key;
                if (isXAxis)
                {
                    float x = ctx.PlotRect.Left + (v - ctx.XMin) / (ctx.XMax - ctx.XMin) * ctx.PlotRect.Width;
                    g.DrawLine(grid, x, ctx.PlotRect.Top, x, ctx.PlotRect.Bottom);
                    DrawRotated(g, label, font, textBrush, x, ctx.PlotRect.Bottom +4, labelAngle, true);
                }
                else
                {
                    float y = ctx.PlotRect.Bottom - (v - ctx.YMin) / (ctx.YMax - ctx.YMin) * ctx.PlotRect.Height;
                    g.DrawLine(grid, ctx.PlotRect.Left, y, ctx.PlotRect.Right, y);
                    DrawRotated(g, label, font, textBrush, ctx.PlotRect.Left -6, y, labelAngle, false);
                }
            }
        }

        private static void DrawRotated(Graphics g, string text, Font font, Brush brush, float x, float y, float angle, bool centerX)
        {
            if (angle ==0)
            {
                var size = TextUtils.MeasureText(g, text, font);
                Color color = brush is SolidBrush sb ? sb.Color : SystemColors.ControlText;
                TextRenderer.DrawText(g, text, font,
                    new Point((int)(centerX ? x - size.Width /2 : x - size.Width -4), (int)y),
                    color, TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
                return;
            }
            var size2 = TextUtils.MeasureText(g, text, font);
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            // TextRenderer cannot honour GDI+ transforms, so DrawString is retained for rotated text
            g.DrawString(text, font, brush, centerX ? -size2.Width /2 : -size2.Width,0);
            g.ResetTransform();
        }

        private static float NiceStep(float roughStep)
        {
            float exp = (float)Math.Floor(Math.Log10(roughStep));
            float fraction = roughStep / (float)Math.Pow(10, exp);
            float niceFraction = fraction <=1 ?1 : fraction <=2 ?2 : fraction <=5 ?5 :10;
            return niceFraction * (float)Math.Pow(10, exp);
        }

        public void UpdateHitAreas(BaseControl owner, AxisLayout ctx, Action<string, Rectangle> notifyAreaHit)
        {
            var xAxisArea = new Rectangle(ctx.PlotRect.Left, ctx.PlotRect.Bottom, ctx.PlotRect.Width,24);
            var yAxisArea = new Rectangle(ctx.PlotRect.Left -24, ctx.PlotRect.Top,24, ctx.PlotRect.Height);
            owner.AddHitArea("XAxis", xAxisArea, null, () => notifyAreaHit?.Invoke("XAxis", xAxisArea));
            owner.AddHitArea("YAxis", yAxisArea, null, () => notifyAreaHit?.Invoke("YAxis", yAxisArea));
        }
    }
}
