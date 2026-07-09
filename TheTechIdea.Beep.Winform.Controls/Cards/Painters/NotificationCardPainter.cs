using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _messageFont = bodyFont;
            _timeFont = captionFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int unreadDotSize = DpiScalingHelper.ScaleValue(UnreadDotSize, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int messageHeight = DpiScalingHelper.ScaleValue(MessageHeight, _owner);
            int timeHeight = DpiScalingHelper.ScaleValue(TimeHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Unread indicator dot (left edge)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left + (padding - unreadDotSize) / 2,
                    drawingRect.Top + (drawingRect.Height - unreadDotSize) / 2,
                    unreadDotSize,
                    unreadDotSize);
            }
            
            int contentLeft = drawingRect.Left + padding + (ctx.ShowStatus ? unreadDotSize / 2 : 0);
            
            // Notification icon/avatar
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    contentLeft,
                    drawingRect.Top + (drawingRect.Height - iconSize) / 2,
                    iconSize,
                    iconSize);
                contentLeft = ctx.ImageRect.Right + contentGap;
            }
            
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - padding;
            
            // Timestamp (top-right)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Right - padding - DpiScalingHelper.ScaleValue(60, _owner),
                drawingRect.Top + padding,
                DpiScalingHelper.ScaleValue(60, _owner),
                timeHeight);
            
            // Notification count badge (optional)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Bottom - padding - badgeHeight,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Title (sender/source)
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + padding,
                contentWidth - DpiScalingHelper.ScaleValue(70, _owner), // Leave room for timestamp
                titleHeight);
            
            // Message preview
            int messageMaxHeight = Math.Max(messageHeight, 
                drawingRect.Height - padding * 2 - titleHeight - timeHeight - elementGap * 2);
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + elementGap / 2,
                contentWidth,
                messageMaxHeight);
            
            // Action time at bottom
            ctx.RatingRect = new Rectangle(
                contentLeft,
                drawingRect.Bottom - padding - timeHeight,
                contentWidth - (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : badgeWidth + elementGap),
                timeHeight);
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Draw unread notification background tint
            if (ctx.ShowStatus && !ctx.DrawingRect.IsEmpty)
            {
                g.FillRectangle(CardPaintCache.Brush(Color.FromArgb(8, ctx.AccentColor)), ctx.DrawingRect);
            }
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw unread indicator dot
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(CardPaintCache.Brush(ctx.AccentColor), ctx.StatusRect);
            }
            
            // Draw notification icon with category color
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawNotificationIcon(g, ctx);
            }
            
            // Draw timestamp
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.SubtitleRect.IsEmpty)
            {
                TextRenderer.DrawText(g, ctx.StatusText, _timeFont, ctx.SubtitleRect,
                    Color.FromArgb(100, _theme?.CardTextForeColor ?? Color.Black),
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
                TextRenderer.DrawText(g, ctx.SubtitleText, _timeFont, ctx.RatingRect,
                    Color.FromArgb(120, _theme?.CardTextForeColor ?? Color.Black),
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }
        
        private void DrawNotificationIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw circular background
            g.FillEllipse(CardPaintCache.Brush(Color.FromArgb(30, ctx.StatusColor != Color.Empty ? ctx.StatusColor : ctx.AccentColor)), ctx.ImageRect);

            // Draw icon border
            g.DrawEllipse(CardPaintCache.Pen(Color.FromArgb(50, ctx.StatusColor != Color.Empty ? ctx.StatusColor : ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner)), ctx.ImageRect);
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
_disposed = true;
        }
        
        #endregion
    }
}

