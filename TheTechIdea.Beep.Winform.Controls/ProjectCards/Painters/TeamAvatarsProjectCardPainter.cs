using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    // Grid card with title, short description, progress bar, and right-aligned avatars
    internal sealed class TeamAvatarsProjectCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.TeamAvatars);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var inner = Inset(bounds, 12);

            var titleRect = new Rectangle(inner.X, inner.Y, inner.Width - 80, 24);
            DrawTitle(g, titleRect, theme, title);

            var subRect = new Rectangle(inner.X, titleRect.Bottom - 2, inner.Width - 80, 20);
            DrawSubtitle(g, subRect, theme, subtitle);

            // right avatars placeholders (three circles)
            int avatarSize = 24; int overlap = 8;
            int ax = inner.Right - avatarSize;
            for (int i = 0; i < 3; i++)
            {
                var r = new Rectangle(ax - i * (avatarSize - overlap), inner.Y + 4, avatarSize, avatarSize);
                using var b = new SolidBrush(Color.FromArgb(80 + i * 30, theme.SecondaryColor.IsEmpty ? Color.Gray : theme.SecondaryColor));
                g.FillEllipse(b, r);
                using var pen = new Pen(Color.White, 2); g.DrawEllipse(pen, r);
            }

            var barRect = new Rectangle(inner.X, inner.Bottom - 10, inner.Width, 6);
            using (var back = new SolidBrush(Color.FromArgb(40, theme.CardTextForeColor))) g.FillRectangle(back, barRect);
            int w = (int)(barRect.Width * (progress / 100f));
            using (var fore = new SolidBrush(theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor)) g.FillRectangle(fore, new Rectangle(barRect.X, barRect.Y, w, barRect.Height));
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var inner = Inset(bounds, 12);
            var titleRect = new Rectangle(inner.X, inner.Y, inner.Width - 80, 24);
            var barRect = new Rectangle(inner.X, inner.Bottom - 10, inner.Width, 6);
            register("Title", titleRect);
            register("ProgressBar", barRect);
        }
    }
}
