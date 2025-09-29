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
    /// ToastNotification - Pop-up toast messages with enhanced visual presentation
    /// </summary>
    internal sealed class ToastNotificationPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ToastNotificationPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Icon area
            if (ctx.ShowIcon)
            {
                ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, 20, 20);
            }

            // Dismiss button area
            bool isDismissible = ctx.CustomData.ContainsKey("IsDismissible") && (bool)ctx.CustomData["IsDismissible"];
            Rectangle dismissRect = Rectangle.Empty;
            if (isDismissible)
            {
                dismissRect = new Rectangle(ctx.DrawingRect.Right - pad - 16, ctx.DrawingRect.Top + pad, 16, 16);
            }

            // Content area
            int contentLeft = ctx.ShowIcon ? ctx.IconRect.Right + 8 : ctx.DrawingRect.Left + pad;
            int contentRight = isDismissible ? dismissRect.Left - 8 : ctx.DrawingRect.Right - pad;

            ctx.ContentRect = new Rectangle(
                contentLeft,
                ctx.DrawingRect.Top + pad,
                contentRight - contentLeft,
                ctx.DrawingRect.Height - pad * 2
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Toast background with shadow
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);

            // Accent border
            using var accentPen = new Pen(ctx.AccentColor, 2);
            g.DrawPath(accentPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            // Get notification data
            string notificationType = ctx.CustomData.ContainsKey("NotificationType") ?
                ctx.CustomData["NotificationType"].ToString() : "info";
            string title = ctx.Title ?? "Notification";
            string message = ctx.Value ?? "This is a sample notification message";

            DrawModernNotification(g, ctx, notificationType, title, message);
        }

        private void DrawModernNotification(Graphics g, WidgetContext ctx, string notificationType, string title, string message)
        {
            // Draw notification icon with modern styling
            if (ctx.ShowIcon)
            {
                DrawNotificationIcon(g, ctx.IconRect, notificationType);
            }

            // Calculate text areas with proper spacing
            var titleRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 18);
            var messageRect = new Rectangle(ctx.ContentRect.X, titleRect.Bottom + 4, ctx.ContentRect.Width, ctx.ContentRect.Height - 22);

            // Draw title with enhanced typography
            using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var titleFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            g.DrawString(title, titleFont, titleBrush, titleRect, titleFormat);

            // Draw message with proper formatting
            using var messageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var messageBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray));
            var messageFormat = new StringFormat {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisWord
            };
            g.DrawString(message, messageFont, messageBrush, messageRect, messageFormat);
        }

        private void DrawNotificationIcon(Graphics g, Rectangle iconRect, string notificationType)
        {
            // Icon background with notification type color
            var (iconName, iconColor) = GetNotificationStyle(notificationType);

            using var iconBgBrush = new SolidBrush(Color.FromArgb(20, iconColor));
            g.FillRoundedRectangle(iconBgBrush, iconRect, 10);

            // Notification icon
            var iconInnerRect = Rectangle.Inflate(iconRect, -4, -4);
            _imagePainter.DrawSvg(g, iconName, iconInnerRect, iconColor, 0.9f);
        }

        private (string iconName, Color iconColor) GetNotificationStyle(string notificationType)
        {
            return notificationType.ToLower() switch
            {
                "success" => ("check-circle", Color.FromArgb(76, 175, 80)),
                "warning" => ("alert-triangle", Color.FromArgb(255, 193, 7)),
                "error" => ("x-circle", Color.FromArgb(244, 67, 54)),
                "info" => ("info", Color.FromArgb(33, 150, 243)),
                _ => ("bell", Color.FromArgb(120, 120, 120))
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw modern dismiss button
            bool isDismissible = ctx.CustomData.ContainsKey("IsDismissible") && (bool)ctx.CustomData["IsDismissible"];
            if (isDismissible)
            {
                var dismissRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Top + 8, 20, 20);

                // Dismiss button background
                using var dismissBgBrush = new SolidBrush(Color.FromArgb(10, Color.Gray));
                g.FillRoundedRectangle(dismissBgBrush, dismissRect, 4);

                // Dismiss icon
                var iconRect = Rectangle.Inflate(dismissRect, -4, -4);
                _imagePainter.DrawSvg(g, "x", iconRect,
                    Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray), 0.8f);
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}