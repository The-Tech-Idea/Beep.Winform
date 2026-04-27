using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class DottedRingProgressPainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.DottedRing);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int dots = ProgressPainterParameterContracts.GetInt(p, "Dots", 24);
            int pad = ProgressBarDpiHelpers.Scale(owner, 6);
            var rect = ProgressRingVisualHelpers.GetSquareRingRect(bounds, pad);
            var center = new PointF(rect.X + rect.Width/2f, rect.Y + rect.Height/2f);
            float radius = Math.Min(rect.Width, rect.Height)/2f - ProgressBarDpiHelpers.Scale(owner, 6);
            int shadowOffset = ProgressRingVisualHelpers.GetShadowOffset(owner);
            var state = ProgressPainterParameterContracts.GetState(p);
            bool isIndeterminate = state != null && state.State == ProgressState.Indeterminate;
            int active;
            if (isIndeterminate)
            {
                active = (int)(dots * 0.4f);
            }
            else
            {
                float pct = owner.DisplayProgressPercentageAccessor;
                active = (int)(dots * pct + 0.5f);
            }
            var on = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                on = Color.FromArgb(ProgressRingVisualHelpers.GetDisabledAccentAlpha(p), on);
            }
            int offAlpha = ProgressRingVisualHelpers.GetTrackAlpha(p, owner.Enabled, 100, 70, 122, 92);
            var off = Color.FromArgb(offAlpha, theme.CardTextForeColor);
            int dotOffset = isIndeterminate ? (int)(state.IndeterminateOffset * dots) : 0;
            for (int i = 0; i < dots; i++)
            {
                int idx = (i + dotOffset) % dots;
                double angle = -Math.PI/2 + i * (2*Math.PI / dots);
                float x = center.X + (float)(radius * Math.Cos(angle));
                float y = center.Y + (float)(radius * Math.Sin(angle));
                int size = Math.Max(ProgressBarDpiHelpers.Scale(owner, 4), (int)(radius*0.08f));
                using var sb = new SolidBrush(Color.FromArgb(ProgressRingVisualHelpers.GetDotShadowAlpha(p, owner.Enabled), Color.Black));
                g.FillEllipse(sb, x - size/2f + shadowOffset, y - size/2f + shadowOffset, size, size);
                using var b = new SolidBrush(idx < active ? on : off);
                g.FillEllipse(b, x - size/2f, y - size/2f, size, size);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            register("RingDots", bounds);
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
    }
}
