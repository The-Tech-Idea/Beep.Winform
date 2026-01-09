using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Painters
{
    internal sealed class EnergyActivityPainter : BaseStatCardPainter
    {
        public override string Key => nameof(StatCardPainterKind.EnergyActivity);

        public override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, IBeepTheme theme, BeepStatCard owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var pad = 10;
            var inner = Inset(bounds, pad);

            var header = string.IsNullOrEmpty(owner.HeaderText) ? GetString(p, "Header", "Avg. Energy Activity") : owner.HeaderText;
            var valueText = string.IsNullOrEmpty(owner.ValueText) ? GetString(p, "Value", "0 kcal") : owner.ValueText;
            var labels = GetStringArray(p, "Labels");
            var s1 = GetFloatArray(p, "Series");
            var s2 = GetFloatArray(p, "Series2");

            DrawHeader(g, inner, theme, owner, header);
            var headerHeight = 26;
            var valueRect = new Rectangle(inner.X, inner.Y + headerHeight, inner.Width, 28);
            DrawValue(g, valueRect, theme, owner, valueText, 1.2f);

            var chartArea = new Rectangle(inner.X, valueRect.Bottom + 4, inner.Width, Math.Max(28, inner.Height - (valueRect.Bottom - inner.Y) - 16));
            DrawGroupedHorizontalBars(g, chartArea, labels, s1, s2, theme);
        }

        private static void DrawGroupedHorizontalBars(Graphics g, Rectangle area, string[] labels, float[] s1, float[] s2, IBeepTheme theme)
        {
            int n = Math.Max(Math.Max(labels?.Length ?? 0, s1?.Length ?? 0), s2?.Length ?? 0);
            if (n == 0) return;

            float max = 1f;
            for (int i = 0; i < n; i++)
            {
                max = Math.Max(max, (i < s1?.Length ? s1[i] : 0) + (i < s2?.Length ? s2[i] : 0));
            }

            int rowH = Math.Max(18, area.Height / n);
            int barHeight = Math.Max(6, rowH - 8);
            int y = area.Y + 4;

            using var fntRaw =  BeepThemesManager.ToFont(theme.SmallText);
            using var fnt = new System.Drawing.Font(fntRaw.FontFamily, fntRaw.Size, System.Drawing.FontStyle.Regular);
            var c1 = theme.PrimaryColor.IsEmpty ? System.Drawing.Color.RoyalBlue : theme.PrimaryColor;
            var c2 = theme.SecondaryColor.IsEmpty ? System.Drawing.Color.MediumPurple : theme.SecondaryColor;

            for (int i = 0; i < n; i++)
            {
                float v1 = (i < s1?.Length ? s1[i] : 0);
                float v2 = (i < s2?.Length ? s2[i] : 0);
                int totalWidth = (int)Math.Round(((v1 + v2) / max) * area.Width);
                int w1 = (int)Math.Round((v1 / Math.Max(1f, v1 + v2)) * totalWidth);
                int w2 = Math.Max(0, totalWidth - w1);

                int barY = y + (rowH - barHeight) / 2;
                var rect1 = new Rectangle(area.X, barY, w1, barHeight);
                var rect2 = new Rectangle(area.X + w1, barY, w2, barHeight);

                using (var b1 = new SolidBrush(System.Drawing.Color.FromArgb(200, c1))) g.FillRectangle(b1, rect1);
                using (var b2 = new SolidBrush(System.Drawing.Color.FromArgb(160, c2))) g.FillRectangle(b2, rect2);

                // label left
                if (labels != null && i < labels.Length)
                {
                    var label = labels[i];
                    SizeF sizeF = TextUtils.MeasureText(label, fnt, int.MaxValue);
                    var size = new Size((int)sizeF.Width, (int)sizeF.Height);
                    TextRenderer.DrawText(g, label, fnt, new System.Drawing.Point(area.X, barY - size.Height), theme.CardTextForeColor);
                }
                y += rowH;
            }
        }
    }
}
