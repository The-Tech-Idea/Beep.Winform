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
    /// AlertBanner - Banner-Style alerts with hit areas and hover
    /// </summary>
    internal sealed class AlertBannerPainter : WidgetPainterBase
    {
        private Rectangle _bannerRectCache;
        private Rectangle _iconRectCache;
        private Rectangle _dismissRectCache;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect; // Full width banner preferred
            ctx.DrawingRect = baseRect;

            // Icon area
            if (ctx.ShowIcon)
            {
                ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 24, 24);
                _iconRectCache = ctx.IconRect;
            }
            else
            {
                _iconRectCache = Rectangle.Empty;
            }

            // Content area
            int contentLeft = ctx.ShowIcon ? ctx.IconRect.Right + 12 : ctx.DrawingRect.Left + pad;
            ctx.ContentRect = new Rectangle(
                contentLeft,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - pad,
                ctx.DrawingRect.Height - pad * 2
            );

            // Optional dismiss button (X) on the right
            bool dismissible = ctx.CustomData.ContainsKey("Dismissible") && (bool)ctx.CustomData["Dismissible"];
            _dismissRectCache = dismissible
                ? new Rectangle(ctx.DrawingRect.Right - pad - 18, ctx.DrawingRect.Top + pad + 3, 18, 18)
                : Rectangle.Empty;

            _bannerRectCache = ctx.DrawingRect;
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
            if (ctx.ShowIcon && !_iconRectCache.IsEmpty)
            {
                var notificationType = ctx.CustomData.ContainsKey("NotificationType") ?
                    (NotificationType)ctx.CustomData["NotificationType"] : NotificationType.Info;
                WidgetRenderingHelpers.DrawNotificationIcon(g, ctx.IconRect, notificationType, ctx.AccentColor);
            }

            // Draw message
            using var messageFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Regular);
            using var messageBrush = new SolidBrush(Color.FromArgb(180, Theme?.ForeColor ?? Color.Black));

            var format = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisWord };
            string fullMessage = !string.IsNullOrEmpty(ctx.Title) ? $"{ctx.Title}: {ctx.Value}" : ctx.Value;
            g.DrawString(fullMessage, messageFont, messageBrush, ctx.ContentRect, format);

            // Hover effect for banner
            if (IsAreaHovered("AlertBanner_Banner"))
            {
                using var hover = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(hover, ctx.DrawingRect);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw dismiss (X) button if available
            if (!_dismissRectCache.IsEmpty)
            {
                bool hovered = IsAreaHovered("AlertBanner_Dismiss");
                using var pen = new Pen(hovered ? (Theme?.PrimaryColor ?? Color.Blue) : (Theme?.ForeColor ?? Color.Black), 1.5f);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLine(pen, _dismissRectCache.Left + 4, _dismissRectCache.Top + 4, _dismissRectCache.Right - 4, _dismissRectCache.Bottom - 4);
                g.DrawLine(pen, _dismissRectCache.Right - 4, _dismissRectCache.Top + 4, _dismissRectCache.Left + 4, _dismissRectCache.Bottom - 4);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!_bannerRectCache.IsEmpty)
            {
                owner.AddHitArea("AlertBanner_Banner", _bannerRectCache, null, () =>
                {
                    ctx.CustomData["BannerClicked"] = true;
                    notifyAreaHit?.Invoke("AlertBanner_Banner", _bannerRectCache);
                    Owner?.Invalidate();
                });
            }
            if (!_iconRectCache.IsEmpty)
            {
                owner.AddHitArea("AlertBanner_Icon", _iconRectCache, null, () =>
                {
                    ctx.CustomData["IconClicked"] = true;
                    notifyAreaHit?.Invoke("AlertBanner_Icon", _iconRectCache);
                    Owner?.Invalidate();
                });
            }
            if (!_dismissRectCache.IsEmpty)
            {
                owner.AddHitArea("AlertBanner_Dismiss", _dismissRectCache, null, () =>
                {
                    ctx.CustomData["Dismissed"] = true;
                    notifyAreaHit?.Invoke("AlertBanner_Dismiss", _dismissRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}