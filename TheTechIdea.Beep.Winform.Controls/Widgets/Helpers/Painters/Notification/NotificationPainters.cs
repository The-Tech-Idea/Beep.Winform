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
            _imagePainter.Theme = Theme;
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
            var titleFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Top };
            g.DrawString(title, titleFont, titleBrush, titleRect, titleFormat);
            
            // Draw message with proper formatting
            using var messageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var messageBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Gray));
            var messageFormat = new StringFormat { 
                Alignment = StringAlignment.Near, 
                LineAlignment = StringAlignment.Top,
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

    /// <summary>
    /// AlertBanner - Full-width alert banner with enhanced visual presentation
    /// </summary>
    internal sealed class AlertBannerPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public AlertBannerPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
            {
                var dismissRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Top + 12, 16, 16);
                WidgetRenderingHelpers.DrawCloseIcon(g, dismissRect, Color.FromArgb(150, Color.Gray));
            }
        }
    }

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

    /// <summary>
    /// ProgressAlert - Progress with message
    /// </summary>
    internal sealed class ProgressAlertPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ProgressAlertPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Content area for message
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 3 - 8
            );
            
            // Progress bar area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                8
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

            // Progress icon
            var iconRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, "activity", iconRect, 
                Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.9f);

            // Progress message with modern typography
            using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Medium);
            using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            
            var textRect = new Rectangle(ctx.ContentRect.X + 28, ctx.ContentRect.Y, 
                ctx.ContentRect.Width - 28, ctx.ContentRect.Height);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            
            string progressText = !string.IsNullOrEmpty(ctx.Title) ? ctx.Title : "Processing...";
            g.DrawString(progressText, titleFont, titleBrush, textRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Modern progress bar with gradient
            float progress = (float)(ctx.TrendPercentage / 100.0);
            var progressRect = ctx.FooterRect;
            
            // Background track\n            using var trackBrush = new SolidBrush(Color.FromArgb(30, Color.Gray));
            using var trackPath = CreateRoundedPath(progressRect, progressRect.Height / 2);
            g.FillPath(trackBrush, trackPath);
            
            // Progress fill
            if (progress > 0)
            {
                var fillWidth = (int)(progressRect.Width * progress);
                var fillRect = new Rectangle(progressRect.X, progressRect.Y, fillWidth, progressRect.Height);
                
                using var fillPath = CreateRoundedPath(fillRect, fillRect.Height / 2);
                var progressColor = ctx.AccentColor ?? Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
                
                using var fillBrush = new LinearGradientBrush(fillRect,
                    progressColor,
                    Color.FromArgb(Math.Min(255, progressColor.R + 30),
                                 Math.Min(255, progressColor.G + 30),
                                 Math.Min(255, progressColor.B + 30)),
                    LinearGradientMode.Horizontal);
                g.FillPath(fillBrush, fillPath);
            }
        }
    }

    /// <summary>
    /// StatusCard - Status card with icon
    /// </summary>
    internal sealed class StatusCardPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Icon area
            if (ctx.ShowIcon)
            {
                int iconSize = 32;
                ctx.IconRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.DrawingRect.Top + pad,
                    iconSize, iconSize
                );
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
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 3);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 8);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw status icon with background
            if (ctx.ShowIcon)
            {
                using var iconBgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                var iconBgRect = Rectangle.Inflate(ctx.IconRect, 4, 4);
                using var iconBgPath = CreateRoundedPath(iconBgRect, 6);
                g.FillPath(iconBgBrush, iconBgPath);
                
                var notificationType = ctx.CustomData.ContainsKey("NotificationType") ? 
                    (NotificationType)ctx.CustomData["NotificationType"] : NotificationType.Info;
                WidgetRenderingHelpers.DrawNotificationIcon(g, ctx.IconRect, notificationType, ctx.AccentColor);
            }
            
            // Draw title and message
            using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var messageFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            using var messageBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            
            var titleRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 18);
            var messageRect = new Rectangle(ctx.ContentRect.X, titleRect.Bottom + 2, ctx.ContentRect.Width, ctx.ContentRect.Height - 20);
            
            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                g.DrawString(ctx.Title, titleFont, titleBrush, titleRect);
            }
            
            // Draw message
            if (!string.IsNullOrEmpty(ctx.Value))
            {
                var format = new StringFormat { Trimming = StringTrimming.EllipsisWord };
                g.DrawString(ctx.Value, messageFont, messageBrush, messageRect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw timestamp or action buttons
        }
    }

    // Placeholder implementations for remaining notification painters
    internal sealed class MessageCenterPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) => ctx;
        public override void DrawBackground(Graphics g, WidgetContext ctx) { }
        public override void DrawContent(Graphics g, WidgetContext ctx) { }
        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }
    }

    internal sealed class SystemAlertPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) => ctx;
        public override void DrawBackground(Graphics g, WidgetContext ctx) { }
        public override void DrawContent(Graphics g, WidgetContext ctx) { }
        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }
    }

    internal sealed class ValidationMessagePainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) => ctx;
        public override void DrawBackground(Graphics g, WidgetContext ctx) { }
        public override void DrawContent(Graphics g, WidgetContext ctx) { }
        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }
    }

    internal sealed class InfoPanelPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) => ctx;
        public override void DrawBackground(Graphics g, WidgetContext ctx) { }
        public override void DrawContent(Graphics g, WidgetContext ctx) { }
        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }
    }

    internal sealed class WarningBadgePainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) => ctx;
        public override void DrawBackground(Graphics g, WidgetContext ctx) { }
        public override void DrawContent(Graphics g, WidgetContext ctx) { }
        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }
    }

internal sealed class SuccessBannerPainter : WidgetPainterBase
{
    public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx) => ctx;
    public override void DrawBackground(Graphics g, WidgetContext ctx) { }
    public override void DrawContent(Graphics g, WidgetContext ctx) { }
    public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }
}
