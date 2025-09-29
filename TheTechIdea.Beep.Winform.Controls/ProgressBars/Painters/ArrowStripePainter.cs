using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class ArrowStripePainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.ArrowStripe);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = bounds; rect.Inflate(-owner.BorderThickness, -owner.BorderThickness);

            using var back = new SolidBrush(owner.BackColor);
            g.FillRectangle(back, rect);

            int count = GetInt(p, "ArrowCount", 40);
            int gap = GetInt(p, "ArrowGap", 2);
            int skew = GetInt(p, "ArrowSkew", 8);
            Color baseColor = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            var gradient = GetBool(p, "ArrowGradient", true);

            float pct = owner.DisplayProgressPercentageAccessor;
            int filledW = (int)(pct * rect.Width);

            using var fill = new SolidBrush(baseColor);
            int x = rect.Left;
            while (x < rect.Left + filledW)
            {
                var pts = new[]
                {
                    new Point(x, rect.Bottom),
                    new Point(Math.Min(rect.Right, x + skew), rect.Top),
                    new Point(Math.Min(rect.Right, x + skew + gap), rect.Top),
                    new Point(Math.Min(rect.Right, x + gap), rect.Bottom)
                };
                using var gp = new GraphicsPath();
                gp.AddPolygon(pts);
                if (gradient)
                {
                    using var lgb = new LinearGradientBrush(new Rectangle(x, rect.Top, skew+gap, rect.Height),
                        Color.FromArgb(200, baseColor), Color.FromArgb(255, baseColor), LinearGradientMode.Horizontal);
                    g.FillPath(lgb, gp);
                }
                else g.FillPath(fill, gp);
                x += skew + gap;
            }

            using var borderPen = new Pen(theme.ProgressBarBorderColor != Color.Empty ? theme.ProgressBarBorderColor : theme.BorderColor, 1);
            g.DrawRectangle(borderPen, rect);
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register) { }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static bool GetBool(IReadOnlyDictionary<string, object> p, string key, bool fallback)
            => p != null && p.TryGetValue(key, out var v) && v is bool b ? b : fallback;
    }
}
