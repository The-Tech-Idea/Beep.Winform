using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class LinearTrackerIconPainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.LinearTrackerIcon);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // draw base bar
            var linear = new LinearProgressPainter();
            linear.Paint(g, bounds, theme, owner, p);

            // tracker icon parameters
            string iconPath = ProgressPainterParameterContracts.GetString(p, "TrackerIconPath", string.Empty);
            int iconSize = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "TrackerIconSize", Math.Max(14, bounds.Height)));
            string easing = ProgressPainterParameterContracts.GetString(p, "TrackerEasing", "linear");

            float pct = owner.DisplayProgressPercentageAccessor;
            float te = ApplyEasing(pct, easing);

            var rect = bounds;
            int cx = rect.Left + (int)(te * rect.Width);
            var iconRect = new Rectangle(cx - iconSize/2, rect.Top - iconSize - ProgressBarDpiHelpers.Scale(owner, 4), iconSize, iconSize);

            if (!string.IsNullOrEmpty(iconPath))
            {
                // Use ProgressBarIconHelpers to paint icon with StyledImagePainter
                ProgressBarIconHelpers.PaintIcon(
                    g,
                    iconRect,
                    owner,
                    ProgressPainterKind.LinearTrackerIcon,
                    iconPath,
                    theme,
                    owner.UseThemeColors,
                    owner.Style);
            }
            else
            {
                var markerColor = theme.PrimaryColor.IsEmpty ? owner.ProgressColor : theme.PrimaryColor;
                if (!owner.Enabled)
                {
                    markerColor = Color.FromArgb(120, markerColor);
                }
                using var pen = new Pen(markerColor, ProgressBarDpiHelpers.Scale(owner, 2)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                int ext = ProgressBarDpiHelpers.Scale(owner, 8);
                g.DrawLine(pen, cx, rect.Top - ext, cx, rect.Bottom + ext);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register) { }

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
        private static float ApplyEasing(float t, string easing)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            switch (easing.Trim().ToLowerInvariant())
            {
                case "linear": return t;
                case "ease-in": return t * t;
                case "ease-out": return 1f - (1f - t) * (1f - t);
                case "ease-in-out": return t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;
                default: return t;
            }
        }
    }
}
