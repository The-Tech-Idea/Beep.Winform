using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class DotsLoaderPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.DotsLoader);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int dots = GetInt(p, "Dots", 8);
            float pct = Math.Max(0f, Math.Min(1f, owner.Value / (float)owner.Maximum));
            int active = (int)(pct * dots + 0.5f);
            int pad = 6;
            int dotSize = Math.Max(6, Math.Min(bounds.Height - pad*2, 12));
            int spacing = (bounds.Width - pad*2 - dots*dotSize) / Math.Max(1, dots - 1);
            int y = bounds.Y + (bounds.Height - dotSize)/2;
            int x = bounds.X + pad;
            var on = theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor;
            var off = Color.FromArgb(120, theme.CardTextForeColor);
            for (int i = 0; i < dots; i++)
            {
                using var b = new SolidBrush(i < active ? on : off);
                g.FillEllipse(b, new Rectangle(x, y, dotSize, dotSize));
                registerLabel(g, bounds, x, y, dotSize); // no-op placeholder for labels if needed later
                x += dotSize + spacing;
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            int dots = GetInt(p, "Dots", 8);
            int pad = 6;
            int dotSize = Math.Max(6, Math.Min(bounds.Height - pad*2, 12));
            int spacing = (bounds.Width - pad*2 - dots*dotSize) / Math.Max(1, dots - 1);
            int y = bounds.Y + (bounds.Height - dotSize)/2;
            int x = bounds.X + pad;
            for (int i = 0; i < dots; i++)
            {
                register($"Dot:{i}", new Rectangle(x, y, dotSize, dotSize));
                x += dotSize + spacing;
            }
        }

        private void registerLabel(Graphics g, Rectangle bounds, int x, int y, int dotSize) { /* reserved */ }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
    }
}
