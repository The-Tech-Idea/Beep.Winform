using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class LinearBadgePainter : IProgressPainter, IProgressPainterV2
    {
        private static readonly LinearProgressPainter _linear = new LinearProgressPainter();

        public string Key => nameof(ProgressPainterKind.LinearBadge);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            _linear.Paint(g, bounds, theme, owner, p);

            // compute badge position along the progress line
            float pct = owner.DisplayProgressPercentageAccessor;
            var rect = bounds;
            int badgeRadius = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "BadgeRadius", Math.Max(10, rect.Height)));
            int offsetY = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "BadgeOffsetY", -badgeRadius - 6));
            string text = ProgressPainterParameterContracts.GetString(p, "BadgeText", $"{(int)(pct * 100)}%");
            var back = ProgressPainterParameterContracts.GetColor(p, "BadgeBackColor", theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor);
            var fore = ProgressPainterParameterContracts.GetColor(p, "BadgeForeColor", theme.OnPrimaryColor.IsEmpty ? Color.White : theme.OnPrimaryColor);
            if (!owner.Enabled)
            {
                back = Color.FromArgb(120, back);
                fore = Color.FromArgb(170, fore);
            }

            int cx = rect.Left + (int)(pct * rect.Width);
            cx = Math.Max(rect.Left + badgeRadius, Math.Min(rect.Right - badgeRadius, cx));
            int cy = rect.Top + offsetY;
            var ellipse = new Rectangle(cx - badgeRadius, cy - badgeRadius, badgeRadius * 2, badgeRadius * 2);
            using (var shadow = new SolidBrush(Color.FromArgb(40, 0, 0, 0))) g.FillEllipse(shadow, ellipse.X + ProgressBarDpiHelpers.Scale(owner, 2), ellipse.Y + ProgressBarDpiHelpers.Scale(owner, 3), ellipse.Width, ellipse.Height);
            using (var b = new SolidBrush(back)) g.FillEllipse(b, ellipse);
            using (var f = ProgressBarFontHelpers.GetBoldFont(owner, owner.ControlStyle))
            using (var tb = new SolidBrush(fore))
            {
                var sz = TextUtils.MeasureText(g, text, f);
                g.DrawString(text, f, tb, new PointF(ellipse.X + (ellipse.Width - sz.Width)/2f, ellipse.Y + (ellipse.Height - sz.Height)/2f));
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            // no clickable areas
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
