using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    internal sealed class RichCourseCardPainter : BaseProjectCardPainter
    {
        public override string Key => nameof(ProjectCardPainterKind.RichCourse);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, System.Collections.Generic.IReadOnlyDictionary<string, object> p)
        {
            var title = GetString(p, BeepProjectCard.ParamTitle, owner.Title);
            var subtitle = GetString(p, BeepProjectCard.ParamSubtitle, owner.Subtitle);
            var progress = GetFloat(p, BeepProjectCard.ParamProgress, owner.Progress);
            var days = (int)(p != null && p.TryGetValue(BeepProjectCard.ParamDaysLeft, out var v) && v is System.IConvertible ? System.Convert.ToInt32(v) : owner.DaysLeft);

            var inner = Inset(bounds, 12);

            using var small = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            TextRenderer.DrawText(g, subtitle, small, new Rectangle(inner.X, inner.Y, inner.Width - 20, small.Height + 2), theme.CardTextForeColor);

            var tRect = new Rectangle(inner.X, inner.Y + small.Height + 6, inner.Width, 28);
            DrawTitle(g, tRect, theme, title);

            int chipH = small.Height + 6;
            var chipRect = new Rectangle(inner.X, inner.Bottom - chipH - 28, 140, chipH);
            using (var b = new SolidBrush(System.Drawing.Color.FromArgb(18, theme.PrimaryColor))) g.FillRectangle(b, chipRect);
            TextRenderer.DrawText(g, $"{progress:F0}% completed", small, chipRect, theme.CardTextForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            var barRect = new Rectangle(inner.X, inner.Bottom - 18, inner.Width, 6);
            using (var back = new SolidBrush(System.Drawing.Color.FromArgb(40, theme.CardTextForeColor))) g.FillRectangle(back, barRect);
            int w = (int)(barRect.Width * (progress / 100f));
            using (var fore = new SolidBrush(theme.PrimaryColor.IsEmpty ? System.Drawing.Color.SeaGreen : theme.PrimaryColor)) g.FillRectangle(fore, new Rectangle(barRect.X, barRect.Y, w, barRect.Height));

            TextRenderer.DrawText(g, days > 0 ? $"{days} days left" : "Due", small, new Rectangle(inner.Right - 100, inner.Bottom - chipH - 28, 100, chipH), theme.CardTextForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        public override void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, System.Collections.Generic.IReadOnlyDictionary<string, object> p, System.Action<string, Rectangle> register)
        {
            var inner = Inset(bounds, 12);
            using var small = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            int chipH = small.Height + 6;
            var chipRect = new Rectangle(inner.X, inner.Bottom - chipH - 28, 140, chipH);
            var barRect = new Rectangle(inner.X, inner.Bottom - 18, inner.Width, 6);
            register("ProgressChip", chipRect);
            register("ProgressBar", barRect);
        }
    }
}
