using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class RadialSegmentedPainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.RadialSegmented);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int segments = ProgressPainterParameterContracts.GetInt(p, "Segments", 24);
            float gap = ProgressPainterParameterContracts.GetFloat(p, "GapAngle", 2f);
            float start = ProgressPainterParameterContracts.GetFloat(p, "StartAngle", -90f);
            float totalSweep = ProgressPainterParameterContracts.GetFloat(p, "SweepAngle", 360f);
            int requestedThickness = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "Thickness", Math.Max(6, Math.Min(bounds.Width, bounds.Height) / 10)));
            int thickness = ProgressRingVisualHelpers.GetClampedThickness(owner, bounds, requestedThickness);
            var color = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                color = Color.FromArgb(ProgressRingVisualHelpers.GetDisabledAccentAlpha(p), color);
            }
            int offAlpha = ProgressRingVisualHelpers.GetTrackAlpha(p, owner.Enabled, 40, 24, 54, 34);
            var off = Color.FromArgb(offAlpha, color);

            int inset = thickness + ProgressBarDpiHelpers.Scale(owner, 4);
            var rect = ProgressRingVisualHelpers.GetSquareRingRect(bounds, inset);
            int shadowOffset = ProgressRingVisualHelpers.GetShadowOffset(owner);
            float pct = owner.DisplayProgressPercentageAccessor;
            int active = (int)Math.Round(segments * pct);

            float sweepPer = (totalSweep - segments * gap) / segments;
            using var penOn = new Pen(color, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var penOff = new Pen(off, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var shadowPen = new Pen(Color.FromArgb(ProgressRingVisualHelpers.GetDotShadowAlpha(p, owner.Enabled), Color.Black), thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            float ang = start;
            for (int i = 0; i < segments; i++)
            {
                g.DrawArc(shadowPen, rect.X + shadowOffset, rect.Y + shadowOffset, rect.Width, rect.Height, ang, sweepPer);
                var pen = i < active ? penOn : penOff;
                g.DrawArc(pen, rect, ang, sweepPer);
                ang += sweepPer + gap;
            }

            // Center text
            var txt = ProgressPainterParameterContracts.GetString(p, "CenterText", $"{(int)(pct*100)}%");
            using var f = ProgressBarFontHelpers.GetBoldFont(owner, owner.ControlStyle);
            using var br = new SolidBrush(owner.Enabled ? theme.CardTextForeColor : Color.FromArgb(ProgressRingVisualHelpers.GetDisabledTextAlpha(p), theme.CardTextForeColor));
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(txt, f, br, rect, sf);
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            register("Ring", bounds);
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
