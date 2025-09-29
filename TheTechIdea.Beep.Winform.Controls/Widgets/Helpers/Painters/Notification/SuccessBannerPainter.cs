using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// SuccessBanner - Modern success notification banner with checkmark icon (interactive)
    /// </summary>
    internal sealed class SuccessBannerPainter : WidgetPainterBase
    {
        private Rectangle _dismissRect;
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 24, 24);
            _dismissRect = new Rectangle(ctx.DrawingRect.Right - pad - 18, ctx.DrawingRect.Top + pad + 3, 18, 18);
            ctx.ContentRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3 - 20,
                ctx.DrawingRect.Height - pad * 2
            );
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new LinearGradientBrush(ctx.DrawingRect, Color.FromArgb(240, 255, 240), Color.FromArgb(220, 255, 220), LinearGradientMode.Vertical);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            using var borderPen = new Pen(Color.FromArgb(100, 34, 197, 94), 1);
            g.DrawPath(borderPen, bgPath);
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowIcon)
            {
                DrawSuccessIcon(g, ctx.IconRect);
            }
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(34, 197, 94));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }
            if (ctx.CustomData.ContainsKey("Message"))
            {
                string message = ctx.CustomData["Message"].ToString();
                using var messageFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(Color.FromArgb(120, 34, 197, 94));
                var messageRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + (ctx.ShowHeader ? 20 : 0), ctx.ContentRect.Width, ctx.ContentRect.Height - (ctx.ShowHeader ? 20 : 0));
                g.DrawString(message, messageFont, messageBrush, messageRect);
            }
        }

        private void DrawSuccessIcon(Graphics g, Rectangle rect)
        {
            using var iconPen = new Pen(Color.FromArgb(34, 197, 94), 3) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            Point[] checkPoints = new Point[] { new Point(rect.X + 6, rect.Y + 12), new Point(rect.X + 10, rect.Y + 16), new Point(rect.X + 18, rect.Y + 8) };
            g.DrawLines(iconPen, checkPoints);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (IsAreaHovered("SuccessBanner_Dismiss"))
            {
                using var pen = new Pen(Theme?.PrimaryColor ?? Color.Green, 1.5f);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLine(pen, _dismissRect.Left + 4, _dismissRect.Top + 4, _dismissRect.Right - 4, _dismissRect.Bottom - 4);
                g.DrawLine(pen, _dismissRect.Right - 4, _dismissRect.Top + 4, _dismissRect.Left + 4, _dismissRect.Bottom - 4);
            }
            else
            {
                using var pen = new Pen(Color.FromArgb(100, 34, 197, 94), 1.2f);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLine(pen, _dismissRect.Left + 4, _dismissRect.Top + 4, _dismissRect.Right - 4, _dismissRect.Bottom - 4);
                g.DrawLine(pen, _dismissRect.Right - 4, _dismissRect.Top + 4, _dismissRect.Left + 4, _dismissRect.Bottom - 4);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            owner.AddHitArea("SuccessBanner_Dismiss", _dismissRect, null, () =>
            {
                ctx.CustomData["SuccessDismissed"] = true;
                notifyAreaHit?.Invoke("SuccessBanner_Dismiss", _dismissRect);
                Owner?.Invalidate();
            });
        }
    }
}