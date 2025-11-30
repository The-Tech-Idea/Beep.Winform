using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// NotificationCard - Notification item with icon, message, and timestamp.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class NotificationCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Notification card fonts
        private Font _titleFont;
        private Font _messageFont;
        private Font _timeFont;
        private Font _badgeFont;
        
        // Notification card spacing
        private const int Padding = 14;
        private const int IconSize = 40;
        private const int UnreadDotSize = 10;
        private const int TitleHeight = 20;
        private const int MessageHeight = 36;
        private const int TimeHeight = 16;
        private const int BadgeWidth = 50;
        private const int BadgeHeight = 18;
        private const int ElementGap = 6;
        private const int ContentGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _messageFont?.Dispose(); } catch { }
            try { _timeFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 10f, FontStyle.Bold);
            _messageFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _timeFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Unread indicator dot (left edge)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left + (Padding - UnreadDotSize) / 2,
                    drawingRect.Top + (drawingRect.Height - UnreadDotSize) / 2,
                    UnreadDotSize,
                    UnreadDotSize);
            }
            
            int contentLeft = drawingRect.Left + Padding + (ctx.ShowStatus ? UnreadDotSize / 2 : 0);
            
            // Notification icon/avatar
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    contentLeft,
                    drawingRect.Top + (drawingRect.Height - IconSize) / 2,
                    IconSize,
                    IconSize);
                contentLeft = ctx.ImageRect.Right + ContentGap;
            }
            
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - Padding;
            
            // Timestamp (top-right)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Right - Padding - 60,
                drawingRect.Top + Padding,
                60,
                TimeHeight);
            
            // Notification count badge (optional)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Bottom - Padding - BadgeHeight,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Title (sender/source)
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth - 70, // Leave room for timestamp
                TitleHeight);
            
            // Message preview
            int messageMaxHeight = Math.Max(MessageHeight, 
                drawingRect.Height - Padding * 2 - TitleHeight - TimeHeight - ElementGap * 2);
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                messageMaxHeight);
            
            // Action time at bottom
            ctx.RatingRect = new Rectangle(
                contentLeft,
                drawingRect.Bottom - Padding - TimeHeight,
                contentWidth - (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : BadgeWidth + ElementGap),
                TimeHeight);
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Draw unread notification background tint
            if (ctx.ShowStatus && !ctx.DrawingRect.IsEmpty)
            {
                using var bgBrush = new SolidBrush(Color.FromArgb(8, ctx.AccentColor));
                g.FillRectangle(bgBrush, ctx.DrawingRect);
            }
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw unread indicator dot
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var brush = new SolidBrush(ctx.AccentColor);
                g.FillEllipse(brush, ctx.StatusRect);
            }
            
            // Draw notification icon with category color
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawNotificationIcon(g, ctx);
            }
            
            // Draw timestamp
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Far, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _timeFont, brush, ctx.SubtitleRect, format);
            }
            
            // Draw badge (notification count)
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw action time (e.g., "2 hours ago")
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.RatingRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _timeFont, brush, ctx.RatingRect, format);
            }
        }
        
        private void DrawNotificationIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw circular background
            using var bgBrush = new SolidBrush(Color.FromArgb(30, ctx.StatusColor != Color.Empty ? ctx.StatusColor : ctx.AccentColor));
            g.FillEllipse(bgBrush, ctx.ImageRect);
            
            // Draw icon border
            using var borderPen = new Pen(Color.FromArgb(50, ctx.StatusColor != Color.Empty ? ctx.StatusColor : ctx.AccentColor), 2);
            g.DrawEllipse(borderPen, ctx.ImageRect);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Full card is clickable
            if (!ctx.DrawingRect.IsEmpty)
            {
                owner.AddHitArea("Notification", ctx.DrawingRect, null,
                    () => notifyAreaHit?.Invoke("Notification", ctx.DrawingRect));
            }
            
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _messageFont?.Dispose();
            _timeFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

