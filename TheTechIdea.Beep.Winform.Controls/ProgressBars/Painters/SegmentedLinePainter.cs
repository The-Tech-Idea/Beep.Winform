using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class SegmentedLinePainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.Segmented);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode =SmoothingMode.AntiAlias;
            int segments = GetInt(p, "Segments", owner.Segments);
            int pad = 6;
            var line = new Rectangle(bounds.X + pad, bounds.Y + bounds.Height/2 - 3, bounds.Width - pad*2, 6);
            using (var back = new SolidBrush(Color.FromArgb(30, theme.CardTextForeColor))) g.FillRectangle(back, line);
            float pct = Math.Max(0f, Math.Min(1f, owner.Value / (float)Math.Max(1, owner.Maximum)));
            int filledW = (int)(line.Width * pct);
            using (var fore = new SolidBrush(theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor))
                g.FillRectangle(fore, new Rectangle(line.X, line.Y, filledW, line.Height));

            // segment ticks
            using var pen = new Pen(Color.FromArgb(120, theme.CardTextForeColor), 1);
            for (int i = 1; i < segments; i++)
            {
                int x = line.X + (line.Width * i) / segments;
                g.DrawLine(pen, x, line.Y, x, line.Bottom);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            int segments = GetInt(p, "Segments", owner.Segments);
            int pad = 6;
            var line = new Rectangle(bounds.X + pad, bounds.Y + bounds.Height/2 - 3, bounds.Width - pad*2, 6);
            for (int i = 0; i < segments; i++)
            {
                int x1 = line.X + (line.Width * i) / segments;
                int x2 = line.X + (line.Width * (i+1)) / segments;
                register($"Step:{i}", new Rectangle(x1, line.Y-4, x2-x1, line.Height+8));
            }
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
    }
}
