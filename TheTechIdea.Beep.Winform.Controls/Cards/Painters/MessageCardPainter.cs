using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _senderFont?.Dispose(); } catch { }
            try { _messageFont?.Dispose(); } catch { }
            try { _timeFont?.Dispose(); } catch { }
            try { _statusFont?.Dispose(); } catch { }
            
            _senderFont = new Font(fontFamily, 9f, FontStyle.Bold);
            _messageFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _timeFont = new Font(fontFamily, 7.5f, FontStyle.Regular);
            _statusFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Determine if this is an outgoing message (use ShowRating as indicator)
            bool isOutgoing = ctx.ShowRating;
            
            // Avatar position
            if (ctx.ShowImage)
            {
                int avatarX = isOutgoing 
                    ? drawingRect.Right - Padding - AvatarSize 
                    : drawingRect.Left + Padding;
                
                ctx.ImageRect = new Rectangle(
                    avatarX,
                    drawingRect.Top + Padding,
                    AvatarSize,
                    AvatarSize);
            }
            
            // Calculate bubble area
            int bubbleLeft, bubbleWidth;
            if (isOutgoing)
            {
                bubbleWidth = Math.Min(drawingRect.Width - Padding * 3 - (ctx.ShowImage ? AvatarSize + ContentGap : 0), 
                    (int)(drawingRect.Width * 0.75));
                bubbleLeft = drawingRect.Right - Padding - (ctx.ShowImage ? AvatarSize + ContentGap : 0) - bubbleWidth;
            }
            else
            {
                bubbleLeft = drawingRect.Left + Padding + (ctx.ShowImage ? AvatarSize + ContentGap : 0);
                bubbleWidth = Math.Min(drawingRect.Width - Padding * 3 - (ctx.ShowImage ? AvatarSize + ContentGap : 0),
                    (int)(drawingRect.Width * 0.75));
            }
            
            // Sender name (above bubble for incoming messages)
            if (!isOutgoing)
            {
                ctx.HeaderRect = new Rectangle(
                    bubbleLeft + BubblePadding,
                    drawingRect.Top + Padding,
                    bubbleWidth - BubblePadding * 2,
                    SenderHeight);
            }
            
            // Message bubble
            int bubbleTop = isOutgoing ? drawingRect.Top + Padding : ctx.HeaderRect.Bottom + ElementGap / 2;
            int bubbleHeight = Math.Max(40, drawingRect.Height - (bubbleTop - drawingRect.Top) - Padding - TimeHeight - ElementGap);
            
            ctx.ParagraphRect = new Rectangle(
                bubbleLeft,
                bubbleTop,
                bubbleWidth,
                bubbleHeight);
            
            // Timestamp and status (below bubble)
            ctx.SubtitleRect = new Rectangle(
                isOutgoing ? ctx.ParagraphRect.Right - 80 : ctx.ParagraphRect.Left,
                ctx.ParagraphRect.Bottom + ElementGap / 2,
                80,
                TimeHeight);
            
            // Read status indicator (for outgoing)
            if (isOutgoing)
            {
                ctx.StatusRect = new Rectangle(
                    ctx.SubtitleRect.Right + 4,
                    ctx.SubtitleRect.Top,
                    StatusIconSize,
                    StatusIconSize);
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
                
                using var borderPen = new Pen(Color.FromArgb(30, ctx.AccentColor), 2);
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
            using var bubblePath = CreateBubblePath(ctx.ParagraphRect, BubbleRadius, isOutgoing);
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
                path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom - radius - TailSize);
                // Tail
                path.AddLine(rect.Right, rect.Bottom - radius - TailSize, rect.Right + TailSize, rect.Bottom - radius);
                path.AddLine(rect.Right + TailSize, rect.Bottom - radius, rect.Right, rect.Bottom - radius + 4);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            }
            else
            {
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            }
            
            // Bottom-left corner and tail
            if (!tailOnRight)
            {
                path.AddLine(rect.Right - radius, rect.Bottom, rect.Left + radius + TailSize, rect.Bottom);
                // Tail
                path.AddLine(rect.Left + radius + TailSize, rect.Bottom, rect.Left - TailSize, rect.Bottom - radius);
                path.AddLine(rect.Left - TailSize, rect.Bottom - radius, rect.Left, rect.Bottom - radius - TailSize);
                path.AddLine(rect.Left, rect.Bottom - radius - TailSize, rect.Left, rect.Top + radius);
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
            using var pen = new Pen(ctx.StatusColor != Color.Empty ? ctx.StatusColor : Color.FromArgb(52, 168, 83), 1.5f);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            int x = ctx.StatusRect.Left;
            int y = ctx.StatusRect.Top + ctx.StatusRect.Height / 2;
            
            // First checkmark
            g.DrawLine(pen, x, y, x + 3, y + 3);
            g.DrawLine(pen, x + 3, y + 3, x + 8, y - 2);
            
            // Second checkmark (offset)
            g.DrawLine(pen, x + 4, y, x + 7, y + 3);
            g.DrawLine(pen, x + 7, y + 3, x + 12, y - 2);
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
            
            _senderFont?.Dispose();
            _messageFont?.Dispose();
            _timeFont?.Dispose();
            _statusFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

