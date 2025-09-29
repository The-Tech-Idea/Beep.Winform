using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    internal sealed class DarkTileProjectCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.DarkTile);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            // dark tile like the first screenshot (blue/black cards)
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var status = GetString(p, BeepProjectCard.ParamStatus, owner.Status);
            var accent = GetColor(p, BeepProjectCard.ParamAccent, theme.PrimaryColor.IsEmpty ? Color.MediumSlateBlue : theme.PrimaryColor);

            using var bg = new LinearGradientBrush(bounds, Color.FromArgb(24,24,28), Color.FromArgb(12,12,16), LinearGradientMode.ForwardDiagonal);
            g.FillRectangle(bg, bounds);

            // accent wave
            using (var gp = new GraphicsPath())
            {
                gp.AddBezier(bounds.Left - 40, bounds.Top + 20, bounds.Left + bounds.Width/3, bounds.Top + 10, bounds.Left + bounds.Width/2, bounds.Top + 40, bounds.Right, bounds.Top);
                gp.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Top + 80);
                gp.AddLine(bounds.Right, bounds.Top + 80, bounds.Left - 40, bounds.Top + 40);
                gp.CloseFigure();
                using var aBrush = new SolidBrush(Color.FromArgb(60, accent));
                g.FillPath(aBrush, gp);
            }

            var inner = Inset(bounds, 12);
            var titleRect = new Rectangle(inner.X, inner.Y, inner.Width, 26);
            using (var tf = new Font("Segoe UI", 12f, FontStyle.Bold))
                TextRenderer.DrawText(g, title, tf, titleRect, Color.White, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            // progress line
            int py = inner.Bottom - 18;
            using (var back = new Pen(Color.FromArgb(80, Color.White), 3)) g.DrawLine(back, inner.Left, py, inner.Right, py);
            using (var fore = new Pen(accent, 3))
            {
                int x2 = inner.Left + (int)((inner.Width) * (progress / 100f));
                g.DrawLine(fore, inner.Left, py, x2, py);
            }

            using var sf = new Font("Segoe UI", 9f, FontStyle.Regular);
            TextRenderer.DrawText(g, status, sf, new Rectangle(inner.X, py + 2, inner.Width, 18), Color.Gainsboro, TextFormatFlags.Left);
            TextRenderer.DrawText(g, $"{progress:F0}%", sf, new Rectangle(inner.Right - 40, inner.Y, 40, 18), Color.White, TextFormatFlags.Right);
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var inner = Inset(bounds, 12);
            var titleRect = new Rectangle(inner.X, inner.Y, inner.Width, 26);
            int py = inner.Bottom - 18;
            var progressLine = new Rectangle(inner.X, py - 3, inner.Width, 6);
            register("Title", titleRect);
            register("ProgressLine", progressLine);
        }
    }
}
