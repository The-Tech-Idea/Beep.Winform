using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Marquees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Painters
{
    public abstract class MarqueePainterBase : IMarqueeItemRenderer
    {
        public abstract string Name { get; }
        public abstract Size   Measure(Graphics g, MarqueeItem item, MarqueeRenderContext ctx);
        public abstract void   Draw(Graphics g, MarqueeItem item, RectangleF dest, MarqueeRenderContext ctx);

        protected static readonly StringFormat CenteredFormatInstance = new StringFormat
        {
            Alignment     = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming      = StringTrimming.EllipsisCharacter
        };

        protected static readonly StringFormat BadgeFormatInstance = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming      = StringTrimming.None
        };

        protected Color ResolveBackColor(MarqueeItem item, MarqueeRenderContext ctx)
        {
            if (item.BackgroundColor != Color.Transparent) return item.BackgroundColor;
            return ctx.UseThemeColors && ctx.Theme != null
                ? ctx.Theme.PanelBackColor
                : ctx.DefaultBackColor;
        }

        protected Color ResolveForeColor(MarqueeItem item, MarqueeRenderContext ctx)
        {
            if (item.TextColor != Color.Transparent) return item.TextColor;
            return ctx.UseThemeColors && ctx.Theme != null
                ? ctx.Theme.ForeColor
                : ctx.DefaultForeColor;
        }

        protected Color ResolveBorderColor(MarqueeRenderContext ctx)
        {
            return ctx.UseThemeColors && ctx.Theme != null
                ? ctx.Theme.BorderColor
                : ColorUtils.MapSystemColor(SystemColors.ControlDark);
        }

        protected static void FillRoundedRect(Graphics g, RectangleF r, float radius,
            Color fill, Color? border = null, float borderWidth = 1f)
        {
            if (r.Width <= 0 || r.Height <= 0) return;
            using var path = BuildRoundedPath(r, radius);
            using var brush = new SolidBrush(fill);
            g.FillPath(brush, path);
            if (border.HasValue)
                using (var pen = new Pen(border.Value, borderWidth))
                    g.DrawPath(pen, path);
        }

        protected static void DrawBadge(Graphics g, string text, PointF origin,
            Color badgeColor, Font font)
        {
            if (string.IsNullOrEmpty(text) || font == null) return;
            var sz = g.MeasureString(text, font);
            float pw = sz.Width + 8, ph = sz.Height + 2;
            var r = new RectangleF(origin.X, origin.Y, pw, ph);
            FillRoundedRect(g, r, ph / 2f, badgeColor);
            using var fg = new SolidBrush(ColorUtils.GetContrastColor(badgeColor));
            g.DrawString(text, font, fg, r, BadgeFormatInstance);
        }

        protected static GraphicsPath BuildRoundedPath(RectangleF r, float radius)
        {
            float d = radius * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X,            r.Y,             d, d, 180, 90);
            p.AddArc(r.Right - d,    r.Y,             d, d, 270, 90);
            p.AddArc(r.Right - d,    r.Bottom - d,    d, d, 0,   90);
            p.AddArc(r.X,            r.Bottom - d,    d, d, 90,  90);
            p.CloseFigure();
            return p;
        }

        public virtual void DrawFadeEdges(Graphics g, RectangleF controlBounds, MarqueeRenderContext ctx)
        {
            if (!ctx.FadeEdges || ctx.FadeWidth <= 0) return;

            bool horizontal = ctx.Direction == MarqueeScrollDirection.LeftToRight
                           || ctx.Direction == MarqueeScrollDirection.RightToLeft;

            Color back = ctx.DefaultBackColor;
            if (ctx.UseThemeColors && ctx.Theme != null) back = ctx.Theme.PanelBackColor;

            if (horizontal)
            {
                float fw = Math.Min(ctx.FadeWidth, controlBounds.Width / 3f);
                var lr = new RectangleF(controlBounds.X, controlBounds.Y, fw, controlBounds.Height);
                using var lb = new LinearGradientBrush(lr, back, Color.Transparent, 0f);
                g.FillRectangle(lb, lr);
                var rr = new RectangleF(controlBounds.Right - fw, controlBounds.Y, fw, controlBounds.Height);
                using var rb = new LinearGradientBrush(rr, Color.Transparent, back, 0f);
                g.FillRectangle(rb, rr);
            }
            else
            {
                float fh = Math.Min(ctx.FadeWidth, controlBounds.Height / 3f);
                var tr = new RectangleF(controlBounds.X, controlBounds.Y, controlBounds.Width, fh);
                using var tb = new LinearGradientBrush(tr, back, Color.Transparent, 90f);
                g.FillRectangle(tb, tr);
                var br = new RectangleF(controlBounds.X, controlBounds.Bottom - fh, controlBounds.Width, fh);
                using var bb = new LinearGradientBrush(br, Color.Transparent, back, 90f);
                g.FillRectangle(bb, br);
            }
        }

        protected static StringFormat CenteredFormat() => CenteredFormatInstance;
    }
}
