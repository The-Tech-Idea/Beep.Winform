using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class ChevronStepsPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.ChevronSteps);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int steps = GetInt(p, "Steps", 4);
            int current = GetInt(p, "Current", 1);
            string[] labels = GetStringArray(p, "Labels") ?? new string[steps];

            int chevronW = Math.Max(40, bounds.Width / steps);
            int chevronH = bounds.Height - 4;
            int x = bounds.X;

            for (int i = 0; i < steps; i++)
            {
                var fill = i < current ? (theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor) : Color.FromArgb(40, theme.CardTextForeColor);
                using var b = new SolidBrush(fill);
                using var pth = MakeChevron(new Rectangle(x, bounds.Y + 2, chevronW, chevronH), i == steps - 1);
                g.FillPath(b, pth);
                using var small =  BeepThemesManager.ToFont(theme.SmallText);
                var text = labels[i] ?? $"Step {i+1}";
                SizeF szF = TextUtils.MeasureText(text, small);
                var sz = new Size((int)szF.Width, (int)szF.Height);
                TextRenderer.DrawText(g, text, small, new Rectangle(x, bounds.Y + (chevronH - sz.Height)/2 + 2, chevronW, sz.Height), Color.White, TextFormatFlags.HorizontalCenter);
                x += chevronW - 8; // slight overlap
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            int steps = GetInt(p, "Steps", 4);
            int chevronW = Math.Max(40, bounds.Width / steps);
            int chevronH = bounds.Height - 4;
            int x = bounds.X;
            for (int i = 0; i < steps; i++)
            {
                var r = new Rectangle(x, bounds.Y + 2, chevronW, chevronH);
                register($"Step:{i}", r);
                x += chevronW - 8;
            }
        }

        private static GraphicsPath MakeChevron(Rectangle r, bool last)
        {
            var path = new GraphicsPath();
            int midY = r.Y + r.Height/2;
            if (last)
            {
                path.AddPolygon(new[] { new Point(r.X, r.Y), new Point(r.Right, midY), new Point(r.X, r.Bottom) });
            }
            else
            {
                path.AddPolygon(new[] { new Point(r.X, r.Y), new Point(r.Right - 12, r.Y), new Point(r.Right, midY), new Point(r.Right - 12, r.Bottom), new Point(r.X, r.Bottom) });
            }
            return path;
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static string[] GetStringArray(IReadOnlyDictionary<string, object> p, string key)
            => p != null && p.TryGetValue(key, out var v) && v is string[] sa ? sa : null;
    }
}
