using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    internal sealed class KanbanListProjectCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.ListKanban);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var status = GetString(p, BeepProjectCard.ParamStatus, owner.Status);
            var tags = GetStringArray(p, BeepProjectCard.ParamTags);

            var inner = Inset(bounds, 10);

            using (var b = new SolidBrush(theme.PrimaryColor.IsEmpty ? System.Drawing.Color.DodgerBlue : theme.PrimaryColor))
                g.FillRectangle(b, new Rectangle(inner.X, inner.Y, 4, inner.Height));

            var textRect = new Rectangle(inner.X + 8, inner.Y, inner.Width - 8, 28);
            DrawTitle(g, textRect, theme, title);

            var subRect = new Rectangle(inner.X + 8, textRect.Bottom - 4, inner.Width - 8, 20);
            DrawSubtitle(g, subRect, theme, subtitle);

            using var small =  BeepThemesManager.ToFont(theme.SmallText);
            int x = inner.X + 8; int y = subRect.Bottom + 4;
            foreach (var t in tags)
            {
                var sz = TextRenderer.MeasureText(t, small);
                var chip = new Rectangle(x, y, sz.Width + 10, sz.Height);
                using var b2 = new SolidBrush(System.Drawing.Color.FromArgb(20, theme.SecondaryColor.IsEmpty ? System.Drawing.Color.MediumPurple : theme.SecondaryColor));
                g.FillRectangle(b2, chip);
                TextRenderer.DrawText(g, t, small, chip, theme.CardTextForeColor, TextFormatFlags.Left);
                x += chip.Width + 6;
                if (x > inner.Right - 60) break;
            }

            var bar = new Rectangle(inner.X + 8, inner.Bottom - 8, inner.Width - 16, 4);
            using (var bb = new SolidBrush(System.Drawing.Color.FromArgb(40, theme.CardTextForeColor))) g.FillRectangle(bb, bar);
            int w = (int)(bar.Width * (progress / 100f));
            using (var fb = new SolidBrush(theme.PrimaryColor.IsEmpty ? System.Drawing.Color.DodgerBlue : theme.PrimaryColor)) g.FillRectangle(fb, new Rectangle(bar.X, bar.Y, w, bar.Height));

            TextRenderer.DrawText(g, status, small, new Rectangle(bar.Right - 120, inner.Y, 120, small.Height + 2), theme.CardTextForeColor, TextFormatFlags.Right);
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var inner = Inset(bounds, 10);
            var bar = new Rectangle(inner.X + 8, inner.Bottom - 8, inner.Width - 16, 4);
            register("CardBody", new Rectangle(inner.X + 8, inner.Y, inner.Width - 8, inner.Height));
            register("ProgressBar", bar);
        }
    }
}
