using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    // Card showcasing a big icon/avatar at left, title/subtitle at right, and small progress/status
    internal sealed class AvatarTileProjectCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.AvatarTile);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var status = GetString(p, BeepProjectCard.ParamStatus, owner.Status);

            var inner = Inset(bounds, 10);
            int avatarSize = System.Math.Min(inner.Height - 8, 48);
            var avatarRect = new Rectangle(inner.X, inner.Y + (inner.Height - avatarSize) / 2, avatarSize, avatarSize);

            // Avatar circle
            using (var b = new SolidBrush(theme.SecondaryColor.IsEmpty ? System.Drawing.Color.SteelBlue : theme.SecondaryColor)) g.FillEllipse(b, avatarRect);
            using (var pen = new Pen(System.Drawing.Color.FromArgb(80, System.Drawing.Color.White), 2)) g.DrawEllipse(pen, avatarRect);

            // Text area
            var textRect = new Rectangle(avatarRect.Right + 10, inner.Y, inner.Width - avatarSize - 16, 24);
            DrawTitle(g, textRect, theme, title);
            var subRect = new Rectangle(textRect.X, textRect.Bottom - 2, textRect.Width, 18);
            DrawSubtitle(g, subRect, theme, subtitle);

            // bottom status + small progress
            using var small = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            TextRenderer.DrawText(g, status, small, new Rectangle(textRect.X, inner.Bottom - small.Height - 2, 120, small.Height), theme.CardTextForeColor);
            var bar = new Rectangle(textRect.Right - 100, inner.Bottom - 6, 100, 4);
            using (var bb = new SolidBrush(System.Drawing.Color.FromArgb(40, theme.CardTextForeColor))) g.FillRectangle(bb, bar);
            int w = (int)(bar.Width * (progress / 100f));
            using (var fb = new SolidBrush(theme.PrimaryColor.IsEmpty ? System.Drawing.Color.DodgerBlue : theme.PrimaryColor)) g.FillRectangle(fb, new Rectangle(bar.X, bar.Y, w, bar.Height));
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var inner = Inset(bounds, 10);
            int avatarSize = System.Math.Min(inner.Height - 8, 48);
            var avatarRect = new Rectangle(inner.X, inner.Y + (inner.Height - avatarSize) / 2, avatarSize, avatarSize);
            var textRect = new Rectangle(avatarRect.Right + 10, inner.Y, inner.Width - avatarSize - 16, 24);
            register("Avatar", avatarRect);
            register("Title", textRect);
        }
    }
}
