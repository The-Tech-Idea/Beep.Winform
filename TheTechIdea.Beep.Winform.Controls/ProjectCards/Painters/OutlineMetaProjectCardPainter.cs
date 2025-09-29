using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    // Light card with rounded outline and meta rows (due date/status) like the 3rd screenshot
    internal sealed class OutlineMetaProjectCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.OutlineMeta);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var status = GetString(p, BeepProjectCard.ParamStatus, owner.Status);
            var inner = Inset(bounds, 12);

            using (var pen = new Pen(Color.FromArgb(180, theme.InactiveBorderColor.IsEmpty ? Color.LightGray : theme.InactiveBorderColor), 1))
            using (var path = new GraphicsPath())
            {
                int r = 10;
                path.AddArc(bounds.X, bounds.Y, r, r, 180, 90);
                path.AddArc(bounds.Right - r, bounds.Y, r, r, 270, 90);
                path.AddArc(bounds.Right - r, bounds.Bottom - r, r, r, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - r, r, r, 90, 90);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }

            DrawTitle(g, new Rectangle(inner.X, inner.Y, inner.Width, 24), theme, title);
            DrawSubtitle(g, new Rectangle(inner.X, inner.Y + 22, inner.Width, 18), theme, subtitle);

            using var small = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            TextRenderer.DrawText(g, status, small, new Rectangle(inner.X, inner.Bottom - small.Height - 26, inner.Width/2, small.Height + 2), theme.CardTextForeColor);

            var bar = new Rectangle(inner.X, inner.Bottom - 14, inner.Width, 6);
            using (var back = new SolidBrush(Color.FromArgb(40, theme.CardTextForeColor))) g.FillRectangle(back, bar);
            int w = (int)(bar.Width * (progress / 100f));
            using (var fore = new SolidBrush(theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor)) g.FillRectangle(fore, new Rectangle(bar.X, bar.Y, w, bar.Height));
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var inner = Inset(bounds, 12);
            var bar = new Rectangle(inner.X, inner.Bottom - 14, inner.Width, 6);
            register("Outline", bounds);
            register("ProgressBar", bar);
        }
    }
}
