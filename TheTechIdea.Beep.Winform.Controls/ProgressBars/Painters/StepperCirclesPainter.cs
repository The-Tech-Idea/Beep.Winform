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
    internal sealed class StepperCirclesPainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.StepperCircles);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int steps = GetInt(p, "Steps", 4);
            int current = GetInt(p, "Current", Math.Max(0, (int)Math.Round(owner.DisplayProgressPercentageAccessor * (steps - 1))));
            string[] labels = GetStringArray(p, "Labels");

            int minDot = ProgressBarDpiHelpers.Scale(owner, 10);
            int maxDot = ProgressBarDpiHelpers.Scale(owner, 16);
            int dotDiameter = Math.Max(minDot, Math.Min(bounds.Height / 2, maxDot));
            int spacing = (bounds.Width - dotDiameter * 2) / Math.Max(1, steps - 1);
            int cy = bounds.Y + bounds.Height / 2;
            var activeColor = theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                activeColor = Color.FromArgb(120, activeColor);
            }
            var inactiveColor = Color.FromArgb(owner.Enabled ? 120 : 80, theme.CardTextForeColor);
            var labelColor = owner.Enabled ? theme.CardTextForeColor : Color.FromArgb(120, theme.CardTextForeColor);

            for (int i = 0; i < steps; i++)
            {
                int cx = bounds.X + dotDiameter + i * spacing;
                var color = i <= current ? activeColor : inactiveColor;
                using (var pen = new Pen(color, ProgressBarDpiHelpers.Scale(owner, 2))) g.DrawLine(pen, i == 0 ? cx : bounds.X + dotDiameter + (i - 1) * spacing, cy, cx, cy);
                using (var b = new SolidBrush(color)) g.FillEllipse(b, cx - dotDiameter/2, cy - dotDiameter/2, dotDiameter, dotDiameter);
                if (labels != null && i < labels.Length)
                {
                    using var small = ProgressBarFontHelpers.GetProgressBarLabelFont(owner, owner.ControlStyle, theme);
                    SizeF szF = TextUtils.MeasureText(labels[i], small);
                    var sz = new Size((int)szF.Width, (int)szF.Height);
                    TextRenderer.DrawText(g, labels[i], small, new Point(cx - sz.Width/2, cy + dotDiameter + ProgressBarDpiHelpers.Scale(owner, 2)), labelColor);
                }
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            int steps = GetInt(p, "Steps", 4);
            int minDot = ProgressBarDpiHelpers.Scale(owner, 10);
            int maxDot = ProgressBarDpiHelpers.Scale(owner, 16);
            int dotDiameter = Math.Max(minDot, Math.Min(bounds.Height / 2, maxDot));
            int spacing = (bounds.Width - dotDiameter * 2) / Math.Max(1, steps - 1);
            int cy = bounds.Y + bounds.Height / 2;
            for (int i = 0; i < steps; i++)
            {
                int cx = bounds.X + dotDiameter + i * spacing;
                var r = new Rectangle(cx - dotDiameter, cy - dotDiameter, dotDiameter*2, dotDiameter*2);
                register($"Step:{i}", r);
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

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static string[] GetStringArray(IReadOnlyDictionary<string, object> p, string key)
            => p != null && p.TryGetValue(key, out var v) && v is string[] sa ? sa : null;
    }
}
