using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// AlertBanner - Banner-style alerts
    /// </summary>
    internal sealed class AlertBannerPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = drawingRect; // Full width banner

            // Icon area
            if (ctx.ShowIcon)
            {
                ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 24, 24);
            }

            // Content area
            int contentLeft = ctx.ShowIcon ? ctx.IconRect.Right + 12 : ctx.DrawingRect.Left + pad;
            ctx.ContentRect = new Rectangle(
                contentLeft,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - pad,
                ctx.DrawingRect.Height - pad * 2
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Banner background
            using var bgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
            g.FillRectangle(bgBrush, ctx.DrawingRect);

            // Left accent border
            using var accentBrush = new SolidBrush(ctx.AccentColor);
            g.FillRectangle(accentBrush, new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, 4, ctx.DrawingRect.Height));
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw notification icon
            if (ctx.ShowIcon)
            {
                var notificationType = ctx.CustomData.ContainsKey("NotificationType") ?
                    (NotificationType)ctx.CustomData["NotificationType"] : NotificationType.Info;
                WidgetRenderingHelpers.DrawNotificationIcon(g, ctx.IconRect, notificationType, ctx.AccentColor);
            }

            // Draw message
            using var messageFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var messageBrush = new SolidBrush(Color.FromArgb(180, Color.Black));

            var format = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisWord };
            string fullMessage = !string.IsNullOrEmpty(ctx.Title) ? $"{ctx.Title}: {ctx.Value}" : ctx.Value;
            g.DrawString(fullMessage, messageFont, messageBrush, ctx.ContentRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw action buttons or dismiss
        }
    }
}