using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class ChevronStepsPainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.ChevronSteps);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int steps = GetInt(p, "Steps", 4);
            int current = GetInt(p, "Current", Math.Max(0, (int)Math.Round(owner.DisplayProgressPercentageAccessor * (steps - 1))));
            string[] labels = GetStringArray(p, "Labels") ?? new string[steps];

            int chevronW = Math.Max(ProgressBarDpiHelpers.Scale(owner, 40), bounds.Width / steps);
            int chevronH = bounds.Height - ProgressBarDpiHelpers.Scale(owner, 4);
            int x = bounds.X;
            var activeColor = theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                activeColor = Color.FromArgb(120, activeColor);
            }
            var inactiveColor = Color.FromArgb(owner.Enabled ? 40 : 24, theme.CardTextForeColor);
            var inactiveTextColor = owner.Enabled ? theme.CardTextForeColor : Color.FromArgb(120, theme.CardTextForeColor);

            for (int i = 0; i < steps; i++)
            {
                var fill = i <= current ? activeColor : inactiveColor;
                var textColor = i <= current
                    ? (theme.OnPrimaryColor.IsEmpty ? Color.White : theme.OnPrimaryColor)
                    : inactiveTextColor;
                if (!owner.Enabled && i <= current)
                {
                    textColor = Color.FromArgb(160, textColor);
                }
                using var b = new SolidBrush(fill);
                using var pth = MakeChevron(new Rectangle(x, bounds.Y + ProgressBarDpiHelpers.Scale(owner, 2), chevronW, chevronH), i == steps - 1, ProgressBarDpiHelpers.Scale(owner, 12));
                g.FillPath(b, pth);
                using var small = ProgressBarFontHelpers.GetProgressBarLabelFont(owner, owner.ControlStyle, theme);
                var text = labels[i] ?? $"Step {i+1}";
                SizeF szF = TextUtils.MeasureText(text, small);
                var sz = new Size((int)szF.Width, (int)szF.Height);
                TextRenderer.DrawText(g, text, small, new Rectangle(x, bounds.Y + (chevronH - sz.Height)/2 + ProgressBarDpiHelpers.Scale(owner, 2), chevronW, sz.Height), textColor, TextFormatFlags.HorizontalCenter);
                x += chevronW - ProgressBarDpiHelpers.Scale(owner, 8); // slight overlap
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            int steps = GetInt(p, "Steps", 4);
            int chevronW = Math.Max(ProgressBarDpiHelpers.Scale(owner, 40), bounds.Width / steps);
            int chevronH = bounds.Height - ProgressBarDpiHelpers.Scale(owner, 4);
            int x = bounds.X;
            for (int i = 0; i < steps; i++)
            {
                var r = new Rectangle(x, bounds.Y + ProgressBarDpiHelpers.Scale(owner, 2), chevronW, chevronH);
                register($"Step:{i}", r);
                x += chevronW - ProgressBarDpiHelpers.Scale(owner, 8);
            }
        }

        public void Paint(Graphics g, ProgressPainterContext context, BeepProgressBar owner)
        {
            if (context == null)
            {
                return;
            }

            Paint(g, context.Bounds, context.Theme, owner, context.Parameters);
        }

        public void UpdateHitAreas(ProgressPainterContext context, BeepProgressBar owner, Action<string, Rectangle> register)
        {
            if (context == null)
            {
                return;
            }

            UpdateHitAreas(owner, context.Bounds, context.Theme, context.Parameters, register);
        }

        private static GraphicsPath MakeChevron(Rectangle r, bool last, int tailInset)
        {
            var path = new GraphicsPath();
            int midY = r.Y + r.Height/2;
            if (last)
            {
                path.AddPolygon(new[] { new Point(r.X, r.Y), new Point(r.Right, midY), new Point(r.X, r.Bottom) });
            }
            else
            {
                path.AddPolygon(new[] { new Point(r.X, r.Y), new Point(r.Right - tailInset, r.Y), new Point(r.Right, midY), new Point(r.Right - tailInset, r.Bottom), new Point(r.X, r.Bottom) });
            }
            return path;
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static string[] GetStringArray(IReadOnlyDictionary<string, object> p, string key)
            => p != null && p.TryGetValue(key, out var v) && v is string[] sa ? sa : null;
    }
}
