using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class StepperCirclesPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.StepperCircles);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int steps = GetInt(p, "Steps", 4);
            int current = GetInt(p, "Current", Math.Max(0, (int)Math.Round(owner.Value / (float)Math.Max(1, owner.Maximum) * (steps - 1))));
            string[] labels = GetStringArray(p, "Labels");

            int radius = Math.Max(10, Math.Min(bounds.Height / 2, 16));
            int spacing = (bounds.Width - radius * 2) / Math.Max(1, steps - 1);
            int cy = bounds.Y + bounds.Height / 2;

            for (int i = 0; i < steps; i++)
            {
                int cx = bounds.X + radius + i * spacing;
                var color = i <= current ? (theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor) : Color.FromArgb(120, theme.CardTextForeColor);
                using (var pen = new Pen(color, 2)) g.DrawLine(pen, i == 0 ? cx : bounds.X + radius + (i - 1) * spacing, cy, cx, cy);
                using (var b = new SolidBrush(color)) g.FillEllipse(b, cx - radius/2, cy - radius/2, radius, radius);
                if (labels != null && i < labels.Length)
                {
                    using var small =  BeepThemesManager.ToFont(theme.SmallText);
                    var sz = TextRenderer.MeasureText(labels[i], small);
                    TextRenderer.DrawText(g, labels[i], small, new Point(cx - sz.Width/2, cy + radius), theme.CardTextForeColor);
                }
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            int steps = GetInt(p, "Steps", 4);
            int radius = Math.Max(10, Math.Min(bounds.Height / 2, 16));
            int spacing = (bounds.Width - radius * 2) / Math.Max(1, steps - 1);
            int cy = bounds.Y + bounds.Height / 2;
            for (int i = 0; i < steps; i++)
            {
                int cx = bounds.X + radius + i * spacing;
                var r = new Rectangle(cx - radius, cy - radius, radius*2, radius*2);
                register($"Step:{i}", r);
            }
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static string[] GetStringArray(IReadOnlyDictionary<string, object> p, string key)
            => p != null && p.TryGetValue(key, out var v) && v is string[] sa ? sa : null;
    }
}
