using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// InfoPanel - Modern information panel with info icon (interactive expand/collapse)
    /// </summary>
    internal sealed class InfoPanelPainter : WidgetPainterBase
    {
        private Rectangle _toggleRect;
        private Rectangle _panelRect;
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            _panelRect = ctx.DrawingRect;
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 24, 24);
            _toggleRect = new Rectangle(ctx.DrawingRect.Right - pad - 18, ctx.DrawingRect.Top + pad + 3, 18, 18);
            bool collapsed = ctx.IsCollapsed;
            int contentHeight = collapsed ? 24 : ctx.DrawingRect.Height - pad * 2;
            ctx.ContentRect = new Rectangle(ctx.IconRect.Right + 12, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3 - 20, contentHeight);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new LinearGradientBrush(ctx.DrawingRect, Color.FromArgb(240, 248, 255), Color.FromArgb(220, 240, 255), LinearGradientMode.Vertical);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            using var borderPen = new Pen(Color.FromArgb(100, 59, 130, 246), 1);
            g.DrawPath(borderPen, bgPath);
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 1);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowIcon)
            {
                DrawInfoIcon(g, ctx.IconRect);
            }
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(59, 130, 246));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.ContentRect.X, ctx.ContentRect.Y);
            }
            bool collapsed = ctx.IsCollapsed;
            if (!collapsed && !string.IsNullOrEmpty(ctx.Message))
            {
                string message = ctx.Message;
                using var messageFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
                using var messageBrush = new SolidBrush(Color.FromArgb(120, 59, 130, 246));
                var messageRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + (ctx.ShowHeader ? 20 : 0), ctx.ContentRect.Width, ctx.ContentRect.Height - (ctx.ShowHeader ? 20 : 0));
                g.DrawString(message, messageFont, messageBrush, messageRect);
            }
        }

        private void DrawInfoIcon(Graphics g, Rectangle rect)
        {
            using var circleBrush = new SolidBrush(Color.FromArgb(59, 130, 246));
            g.FillEllipse(circleBrush, rect);
            using var letterFont = new Font("Arial", 14, FontStyle.Bold);
            using var letterBrush = new SolidBrush(Color.White);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("i", letterFont, letterBrush, rect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            bool collapsed = ctx.IsCollapsed;
            string glyph = collapsed ? "+" : "−"; // minus
            bool hover = IsAreaHovered("InfoPanel_Toggle");
            using var font = new Font(Owner.Font.FontFamily, 10, FontStyle.Bold);
            using var brush = new SolidBrush(hover ? (Theme?.PrimaryColor ?? Color.Blue) : (Theme?.ForeColor ?? Color.Black));
            var sz = TextUtils.MeasureText(g,glyph, font);
            g.DrawString(glyph, font, brush, _toggleRect.X + (_toggleRect.Width - sz.Width) / 2, _toggleRect.Y + (_toggleRect.Height - sz.Height) / 2);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            owner.AddHitArea("InfoPanel_Toggle", _toggleRect, null, () =>
            {
                bool collapsed = ctx.IsCollapsed;
                ctx.IsCollapsed = !collapsed;
                notifyAreaHit?.Invoke("InfoPanel_Toggle", _toggleRect);
                Owner?.Invalidate();
            });
        }
    }
}