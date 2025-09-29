using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models; // BeepImage

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class LinearTrackerIconPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.LinearTrackerIcon);
        private readonly BeepImage _icon = new BeepImage { IsChild = true, ApplyThemeOnImage = true, PreserveSvgBackgrounds = true };

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // draw base bar
            var linear = new LinearProgressPainter();
            linear.Paint(g, bounds, theme, owner, p);

            // tracker icon parameters
            string iconPath = GetString(p, "TrackerIconPath", string.Empty);
            int iconSize = GetInt(p, "TrackerIconSize", Math.Max(14, bounds.Height));
            string easing = GetString(p, "TrackerEasing", "linear");

            float pct = owner.DisplayProgressPercentageAccessor;
            float te = ApplyEasing(pct, easing);

            var rect = bounds; rect.Inflate(-owner.BorderThickness, -owner.BorderThickness);
            int cx = rect.Left + (int)(te * rect.Width);
            var iconRect = new Rectangle(cx - iconSize/2, rect.Top - iconSize - 4, iconSize, iconSize);

            if (!string.IsNullOrEmpty(iconPath))
            {
                _icon.ImagePath = iconPath;
                _icon.Size = new Size(iconSize, iconSize);
                _icon.BackColor = owner.BackColor;
                _icon.ForeColor = theme.PrimaryColor;
                _icon.DrawImage(g, iconRect);
            }
            else
            {
                using var pen = new Pen(theme.PrimaryColor, 2) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(pen, cx, rect.Top - 8, cx, rect.Bottom + 8);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register) { }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback)
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;
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
