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
    internal sealed class RingProgressPainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.Ring);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int requestedThickness = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "Thickness", Math.Max(6, bounds.Height / 8)));
            int thickness = ProgressRingVisualHelpers.GetClampedThickness(owner, bounds, requestedThickness);
            int pad = thickness / 2 + ProgressBarDpiHelpers.Scale(owner, 2);
            var ringRect = ProgressRingVisualHelpers.GetSquareRingRect(bounds, pad);
            int shadowOffset = ProgressRingVisualHelpers.GetShadowOffset(owner);
            int trackAlpha = ProgressRingVisualHelpers.GetTrackAlpha(p, owner.Enabled, 40, 24, 54, 34);
            using var backPen = new Pen(Color.FromArgb(trackAlpha, theme.CardTextForeColor), thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var activeColor = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                activeColor = Color.FromArgb(ProgressRingVisualHelpers.GetDisabledAccentAlpha(p), activeColor);
            }
            using var forePen = new Pen(activeColor, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var shadowPen = new Pen(Color.FromArgb(ProgressRingVisualHelpers.GetRingShadowAlpha(p, owner.Enabled), Color.Black), thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            if (ringRect.Width > 0 && ringRect.Height > 0)
            {
                g.DrawArc(shadowPen, ringRect.X + shadowOffset, ringRect.Y + shadowOffset, ringRect.Width, ringRect.Height, -90, 360);
            }
            g.DrawArc(backPen, ringRect, -90, 360);
            float pct = owner.DisplayProgressPercentageAccessor;
            g.DrawArc(forePen, ringRect, -90, 360f * pct);

            // center text
            var text = ProgressPainterParameterContracts.GetString(p, "CenterText", $"{(int)(pct*100)}%");
            using var f = ProgressBarFontHelpers.GetBoldFont(owner, owner.ControlStyle);
            using var br = new SolidBrush(owner.Enabled ? theme.CardTextForeColor : Color.FromArgb(ProgressRingVisualHelpers.GetDisabledTextAlpha(p), theme.CardTextForeColor));
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, f, br, ringRect, sf);
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
