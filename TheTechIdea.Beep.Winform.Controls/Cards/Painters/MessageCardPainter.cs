using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// MessageCard - Chat message bubble style with avatar and timestamp.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class MessageCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Message card fonts
        private Font _senderFont;
        private Font _messageFont;
        private Font _timeFont;
        private Font _statusFont;
        
        // Message card spacing
        private const int Padding = 12;
        private const int AvatarSize = 36;
        private const int BubbleRadius = 16;
        private const int BubblePadding = 12;
        private const int SenderHeight = 18;
        private const int TimeHeight = 14;
        private const int StatusIconSize = 14;
        private const int ElementGap = 6;
        private const int ContentGap = 10;
        private const int TailSize = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_senderFont = bodyFont;
            _messageFont = bodyFont;
            _timeFont = captionFont;
            _statusFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int avatarSize = DpiScalingHelper.ScaleValue(AvatarSize, _owner);
            int bubbleRadius = DpiScalingHelper.ScaleValue(BubbleRadius, _owner);
            int bubblePadding = DpiScalingHelper.ScaleValue(BubblePadding, _owner);
            int senderHeight = DpiScalingHelper.ScaleValue(SenderHeight, _owner);
            int timeHeight = DpiScalingHelper.ScaleValue(TimeHeight, _owner);
            int statusIconSize = DpiScalingHelper.ScaleValue(StatusIconSize, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Determine if this is an outgoing message (use ShowRating as indicator)
            bool isOutgoing = ctx.ShowRating;
            
            // Avatar position
            if (ctx.ShowImage)
            {
                int avatarX = isOutgoing 
                    ? drawingRect.Right - padding - avatarSize 
                    : drawingRect.Left + padding;
                
                ctx.ImageRect = new Rectangle(
                    avatarX,
                    drawingRect.Top + padding,
                    avatarSize,
                    avatarSize);
            }
            
            // Calculate bubble area
            int bubbleLeft, bubbleWidth;
            if (isOutgoing)
            {
                bubbleWidth = Math.Min(drawingRect.Width - padding * 3 - (ctx.ShowImage ? avatarSize + contentGap : 0), 
                    (int)(drawingRect.Width * 0.75));
                bubbleLeft = drawingRect.Right - padding - (ctx.ShowImage ? avatarSize + contentGap : 0) - bubbleWidth;
            }
            else
            {
                bubbleLeft = drawingRect.Left + padding + (ctx.ShowImage ? avatarSize + contentGap : 0);
                bubbleWidth = Math.Min(drawingRect.Width - padding * 3 - (ctx.ShowImage ? avatarSize + contentGap : 0),
                    (int)(drawingRect.Width * 0.75));
            }
            
            // Sender name (above bubble for incoming messages)
            if (!isOutgoing)
            {
                ctx.HeaderRect = new Rectangle(
                    bubbleLeft + bubblePadding,
                    drawingRect.Top + padding,
                    bubbleWidth - bubblePadding * 2,
                    senderHeight);
            }
            
            // Message bubble
            int bubbleTop = isOutgoing ? drawingRect.Top + padding : ctx.HeaderRect.Bottom + elementGap / 2;
            int bubbleHeight = Math.Max(DpiScalingHelper.ScaleValue(40, _owner), drawingRect.Height - (bubbleTop - drawingRect.Top) - padding - timeHeight - elementGap);
            
            ctx.ParagraphRect = new Rectangle(
                bubbleLeft,
                bubbleTop,
                bubbleWidth,
                bubbleHeight);
            
            // Timestamp and status (below bubble)
            ctx.SubtitleRect = new Rectangle(
                isOutgoing ? ctx.ParagraphRect.Right - DpiScalingHelper.ScaleValue(80, _owner) : ctx.ParagraphRect.Left,
                ctx.ParagraphRect.Bottom + elementGap / 2,
                DpiScalingHelper.ScaleValue(80, _owner),
                timeHeight);
            
            // Read status indicator (for outgoing)
            if (isOutgoing)
            {
                ctx.StatusRect = new Rectangle(
                    ctx.SubtitleRect.Right + DpiScalingHelper.ScaleValue(4, _owner),
                    ctx.SubtitleRect.Top,
                    statusIconSize,
                    statusIconSize);
            }
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            bool isOutgoing = ctx.ShowRating;
            
            // Draw message bubble
            DrawMessageBubble(g, ctx, isOutgoing);
            
            // Draw avatar
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var bgBrush = new SolidBrush(Color.FromArgb(40, ctx.AccentColor));
                g.FillEllipse(bgBrush, ctx.ImageRect);
                
            using var borderPen = new Pen(Color.FromArgb(30, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner));
                g.DrawEllipse(borderPen, ctx.ImageRect);
            }
            
            // Draw timestamp
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var format = new StringFormat 
                { 
                    Alignment = isOutgoing ? StringAlignment.Far : StringAlignment.Near,
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _timeFont, brush, ctx.SubtitleRect, format);
            }
            
            // Draw read status checkmarks for outgoing messages
            if (isOutgoing && ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                DrawReadStatus(g, ctx);
            }
        }
        
        private void DrawMessageBubble(Graphics g, LayoutContext ctx, bool isOutgoing)
        {
            if (ctx.ParagraphRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Bubble color
            Color bubbleColor = isOutgoing 
                ? ctx.AccentColor 
                : Color.FromArgb(240, 240, 240);
            
            // Draw bubble with tail
            using var bubblePath = CreateBubblePath(ctx.ParagraphRect, DpiScalingHelper.ScaleValue(BubbleRadius, _owner), isOutgoing);
            using var bubbleBrush = new SolidBrush(bubbleColor);
            g.FillPath(bubbleBrush, bubblePath);
        }
        
        private GraphicsPath CreateBubblePath(Rectangle rect, int radius, bool tailOnRight)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            
            // Top-left corner
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            
            // Top-right corner
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            
            // Right side and tail
            if (tailOnRight)
            {
                int tailSize = DpiScalingHelper.ScaleValue(TailSize, _owner);
                path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom - radius - tailSize);
                // Tail
                path.AddLine(rect.Right, rect.Bottom - radius - tailSize, rect.Right + tailSize, rect.Bottom - radius);
                path.AddLine(rect.Right + tailSize, rect.Bottom - radius, rect.Right, rect.Bottom - radius + DpiScalingHelper.ScaleValue(4, _owner));
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            }
            else
            {
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            }
            
            // Bottom-left corner and tail
            if (!tailOnRight)
            {
                int tailSize = DpiScalingHelper.ScaleValue(TailSize, _owner);
                path.AddLine(rect.Right - radius, rect.Bottom, rect.Left + radius + tailSize, rect.Bottom);
                // Tail
                path.AddLine(rect.Left + radius + tailSize, rect.Bottom, rect.Left - tailSize, rect.Bottom - radius);
                path.AddLine(rect.Left - tailSize, rect.Bottom - radius, rect.Left, rect.Bottom - radius - tailSize);
                path.AddLine(rect.Left, rect.Bottom - radius - tailSize, rect.Left, rect.Top + radius);
            }
            else
            {
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            }
            
            path.CloseFigure();
            return path;
        }
        
        private void DrawReadStatus(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Double checkmark for "read"
            using var pen = new Pen(ctx.StatusColor != Color.Empty ? ctx.StatusColor : Color.FromArgb(52, 168, 83), DpiScalingHelper.ScaleValue(2, _owner));
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            int x = ctx.StatusRect.Left;
            int y = ctx.StatusRect.Top + ctx.StatusRect.Height / 2;
            
            // First checkmark
            g.DrawLine(pen, x, y, x + DpiScalingHelper.ScaleValue(3, _owner), y + DpiScalingHelper.ScaleValue(3, _owner));
            g.DrawLine(pen, x + DpiScalingHelper.ScaleValue(3, _owner), y + DpiScalingHelper.ScaleValue(3, _owner), x + DpiScalingHelper.ScaleValue(8, _owner), y - DpiScalingHelper.ScaleValue(2, _owner));
            
            // Second checkmark (offset)
            g.DrawLine(pen, x + DpiScalingHelper.ScaleValue(4, _owner), y, x + DpiScalingHelper.ScaleValue(7, _owner), y + DpiScalingHelper.ScaleValue(3, _owner));
            g.DrawLine(pen, x + DpiScalingHelper.ScaleValue(7, _owner), y + DpiScalingHelper.ScaleValue(3, _owner), x + DpiScalingHelper.ScaleValue(12, _owner), y - DpiScalingHelper.ScaleValue(2, _owner));
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Message bubble hit area
            if (!ctx.ParagraphRect.IsEmpty)
            {
                owner.AddHitArea("Message", ctx.ParagraphRect, null,
                    () => notifyAreaHit?.Invoke("Message", ctx.ParagraphRect));
            }
            
            // Avatar hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Avatar", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Avatar", ctx.ImageRect));
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

