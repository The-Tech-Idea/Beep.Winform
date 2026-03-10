using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class DotsLoaderPainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.DotsLoader);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int dots = ProgressPainterParameterContracts.GetInt(p, "Dots", 8);
            float pct = owner.DisplayProgressPercentageAccessor;
            int active = (int)(pct * dots + 0.5f);
            int pad = ProgressBarDpiHelpers.Scale(owner, 6);
            int minDot = ProgressBarDpiHelpers.Scale(owner, 6);
            int maxDot = ProgressBarDpiHelpers.Scale(owner, 12);
            int dotSize = Math.Max(minDot, Math.Min(bounds.Height - pad*2, maxDot));
            int spacing = (bounds.Width - pad*2 - dots*dotSize) / Math.Max(1, dots - 1);
            int y = bounds.Y + (bounds.Height - dotSize)/2;
            int x = bounds.X + pad;
            var on = theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                on = Color.FromArgb(120, on);
            }
            var off = Color.FromArgb(owner.Enabled ? 120 : 80, theme.CardTextForeColor);
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
            int dots = ProgressPainterParameterContracts.GetInt(p, "Dots", 8);
            int pad = ProgressBarDpiHelpers.Scale(owner, 6);
            int minDot = ProgressBarDpiHelpers.Scale(owner, 6);
            int maxDot = ProgressBarDpiHelpers.Scale(owner, 12);
            int dotSize = Math.Max(minDot, Math.Min(bounds.Height - pad*2, maxDot));
            int spacing = (bounds.Width - pad*2 - dots*dotSize) / Math.Max(1, dots - 1);
            int y = bounds.Y + (bounds.Height - dotSize)/2;
            int x = bounds.X + pad;
            for (int i = 0; i < dots; i++)
            {
                register($"Dot:{i}", new Rectangle(x, y, dotSize, dotSize));
                x += dotSize + spacing;
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

        private void registerLabel(Graphics g, Rectangle bounds, int x, int y, int dotSize) { /* reserved */ }
    }
}
