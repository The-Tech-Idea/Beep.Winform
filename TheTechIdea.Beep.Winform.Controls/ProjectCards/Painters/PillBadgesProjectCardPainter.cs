using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    // Card that emphasizes pill badges (e.g., category/status) under the title
    internal sealed class PillBadgesProjectCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.PillBadges);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var tags = GetStringArray(p, BeepProjectCard.ParamTags);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var inner = Inset(bounds, 12);

            DrawTitle(g, new Rectangle(inner.X, inner.Y, inner.Width, 24), theme, title);
            DrawSubtitle(g, new Rectangle(inner.X, inner.Y + 20, inner.Width, 18), theme, subtitle);

            using var small = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            int x = inner.X; int y = inner.Y + 44; int h = small.Height + 6;
            for (int i = 0; i < tags.Length; i++)
            {
                var t = tags[i];
                var sz = TextRenderer.MeasureText(t, small);
                var pill = new Rectangle(x, y, sz.Width + 16, h);
                using var b = new SolidBrush(System.Drawing.Color.FromArgb(24, theme.SecondaryColor.IsEmpty ? System.Drawing.Color.DarkOrchid : theme.SecondaryColor));
                using var pen = new Pen(theme.SecondaryColor.IsEmpty ? System.Drawing.Color.DarkOrchid : theme.SecondaryColor, 1);
                g.FillRectangle(b, pill);
                g.DrawRectangle(pen, pill);
                TextRenderer.DrawText(g, t, small, pill, theme.CardTextForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                // register hit areas later via UpdateHitAreas
                x += pill.Width + 8;
                if (x > inner.Right - 60) break;
            }

            var bar = new Rectangle(inner.X, inner.Bottom - 10, inner.Width, 6);
            using (var back = new SolidBrush(System.Drawing.Color.FromArgb(40, theme.CardTextForeColor))) g.FillRectangle(back, bar);
            int w = (int)(bar.Width * (progress / 100f));
            using (var fore = new SolidBrush(theme.PrimaryColor.IsEmpty ? System.Drawing.Color.DodgerBlue : theme.PrimaryColor)) g.FillRectangle(fore, new Rectangle(bar.X, bar.Y, w, bar.Height));
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var tags = GetStringArray(p, BeepProjectCard.ParamTags);
            var inner = Inset(bounds, 12);
            using var small = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            int x = inner.X; int y = inner.Y + 44; int h = small.Height + 6;
            for (int i = 0; i < tags.Length; i++)
            {
                var t = tags[i];
                var sz = TextRenderer.MeasureText(t, small);
                var pill = new Rectangle(x, y, sz.Width + 16, h);
                register($"Pill:{t}", pill);
                x += pill.Width + 8;
                if (x > inner.Right - 60) break;
            }
            var bar = new Rectangle(inner.X, inner.Bottom - 10, inner.Width, 6);
            register("ProgressBar", bar);
        }
    }
}
