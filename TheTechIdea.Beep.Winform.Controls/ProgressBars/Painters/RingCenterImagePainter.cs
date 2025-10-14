using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class RingCenterImagePainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.RingCenterImage);
        private readonly BeepImage _center = new BeepImage { IsChild = true, ApplyThemeOnImage = true, PreserveSvgBackgrounds = true };

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int thickness = GetInt(p, "Thickness", Math.Max(6, bounds.Height/8));
            int pad = thickness/2 + 2;
            var ringRect = new Rectangle(bounds.X + pad, bounds.Y + pad, bounds.Width - pad*2, bounds.Height - pad*2);
            using var backPen = new Pen(Color.FromArgb(40, theme.CardTextForeColor), thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var forePen = new Pen(theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawArc(backPen, ringRect, -90, 360);
            float pct = Math.Max(0f, Math.Min(1f, owner.Value / (float)Math.Max(1, owner.Maximum)));
            g.DrawArc(forePen, ringRect, -90, 360f * pct);

            // center image and optional text
            string iconPath = GetString(p, "CenterIconPath", null);
            int iconSize = GetInt(p, "CenterIconSize", Math.Max(16, ringRect.Height/3));
            if (!string.IsNullOrEmpty(iconPath))
            {
                var r = new Rectangle(ringRect.X + (ringRect.Width - iconSize)/2, ringRect.Y + (ringRect.Height - iconSize)/2, iconSize, iconSize);
                _center.ImagePath = iconPath; _center.Size = new Size(iconSize, iconSize); _center.BackColor = Color.Transparent; _center.ForeColor = theme.PrimaryColor; _center.DrawImage(g, r);
            }
            string txt = GetString(p, "CenterText", string.Empty);
            if (!string.IsNullOrEmpty(txt))
            {
                using var f = new Font("Segoe UI", Math.Max(8, ringRect.Height/8f), FontStyle.Bold);
                var sz = TextUtils.MeasureText(g, txt, f);
                var pt = new PointF(ringRect.X + (ringRect.Width - sz.Width)/2, ringRect.Y + (ringRect.Height - sz.Height)/2 + iconSize/2f + 4);
                using var br = new SolidBrush(theme.CardTextForeColor);
                g.DrawString(txt, f, br, pt);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            register("Ring", bounds);
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback)
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;
    }
}
