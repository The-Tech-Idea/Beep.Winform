using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    internal sealed class CompactProgressCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.CompactProgress);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            // header
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var tags = GetStringArray(p, BeepProjectCard.ParamTags);
            int pad = 8;
            var inner = Inset(bounds, pad);

            // top title
            var titleRect = new Rectangle(inner.X, inner.Y, inner.Width, 24);
            DrawTitle(g, titleRect, theme, title);

            // subtitle
            var subRect = new Rectangle(inner.X, titleRect.Bottom, inner.Width, 18);
            DrawSubtitle(g, subRect, theme, subtitle);

            // tags row
            int y = subRect.Bottom + 6;
            if (tags.Length > 0)
            {
                int x = inner.X;
                using var fr = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
                foreach (var t in tags)
                {
                    var size = TextRenderer.MeasureText(t, fr);
                    var chip = new Rectangle(x, y, size.Width + 10, size.Height);
                    using var b = new SolidBrush(System.Drawing.Color.FromArgb(20, theme.PrimaryColor));
                    g.FillRectangle(b, chip);
                    TextRenderer.DrawText(g, t, fr, chip, theme.CardTextForeColor, TextFormatFlags.Left);
                    x += chip.Width + 6;
                    if (x > inner.Right - 40) break;
                }
                y += 6 + fr.Height;
            }

            // progress bar
            var barRect = new Rectangle(inner.X, inner.Bottom - 14, inner.Width, 6);
            using (var back = new SolidBrush(System.Drawing.Color.FromArgb(50, theme.CardTextForeColor))) g.FillRectangle(back, barRect);
            int w = (int)(barRect.Width * (progress / 100f));
            using (var fore = new SolidBrush(theme.PrimaryColor.IsEmpty ? System.Drawing.Color.DodgerBlue : theme.PrimaryColor))
                g.FillRectangle(fore, new Rectangle(barRect.X, barRect.Y, w, barRect.Height));

            // progress text right
            using var fr2 = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            var trect = new Rectangle(barRect.Right - 40, barRect.Y - fr2.Height - 2, 40, fr2.Height + 2);
            TextRenderer.DrawText(g, $"{progress:F0}%", fr2, trect, theme.CardTextForeColor, TextFormatFlags.Right);
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            int pad = 8;
            var inner = Inset(bounds, pad);
            var titleRect = new Rectangle(inner.X, inner.Y, inner.Width, 24);
            var barRect = new Rectangle(inner.X, inner.Bottom - 14, inner.Width, 6);
            register("Title", titleRect);
            register("ProgressBar", barRect);
        }
    }
}
