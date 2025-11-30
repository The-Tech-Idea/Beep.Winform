using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// VideoCard - Video thumbnail with play button overlay, duration, and metadata.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class VideoCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Video card fonts
        private Font _titleFont;
        private Font _channelFont;
        private Font _statsFont;
        private Font _durationFont;
        private Font _badgeFont;
        
        // Video card spacing
        private const int Padding = 12;
        private const int ThumbnailRatio = 56; // 16:9 aspect ratio height percentage
        private const int PlayButtonSize = 56;
        private const int DurationWidth = 50;
        private const int DurationHeight = 22;
        private const int TitleHeight = 44;
        private const int ChannelHeight = 18;
        private const int StatsHeight = 16;
        private const int ChannelAvatarSize = 36;
        private const int BadgeWidth = 60;
        private const int BadgeHeight = 20;
        private const int ElementGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _channelFont?.Dispose(); } catch { }
            try { _statsFont?.Dispose(); } catch { }
            try { _durationFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _channelFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _statsFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _durationFont = new Font(fontFamily, 9f, FontStyle.Bold);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Video thumbnail (16:9 aspect ratio)
            int thumbnailHeight = Math.Min(
                (int)(drawingRect.Width * ThumbnailRatio / 100.0),
                Math.Max(100, (int)(drawingRect.Height * 0.55)));
            
            ctx.ImageRect = new Rectangle(
                drawingRect.Left,
                drawingRect.Top,
                drawingRect.Width,
                thumbnailHeight);
            
            // Play button (centered on thumbnail)
            ctx.ButtonRect = new Rectangle(
                ctx.ImageRect.Left + (ctx.ImageRect.Width - PlayButtonSize) / 2,
                ctx.ImageRect.Top + (ctx.ImageRect.Height - PlayButtonSize) / 2,
                PlayButtonSize,
                PlayButtonSize);
            
            // Duration badge (bottom-right of thumbnail)
            ctx.RatingRect = new Rectangle(
                ctx.ImageRect.Right - Padding - DurationWidth,
                ctx.ImageRect.Bottom - Padding - DurationHeight,
                DurationWidth,
                DurationHeight);
            
            // Quality/Live badge (top-right of thumbnail)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.ImageRect.Right - Padding - BadgeWidth,
                    ctx.ImageRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Content area below thumbnail
            int contentTop = ctx.ImageRect.Bottom + Padding;
            int contentLeft = drawingRect.Left + Padding;
            int contentWidth = drawingRect.Width - Padding * 2;
            
            // Channel avatar (optional)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    contentLeft,
                    contentTop,
                    ChannelAvatarSize,
                    ChannelAvatarSize);
                contentLeft = ctx.StatusRect.Right + ElementGap;
                contentWidth -= ChannelAvatarSize + ElementGap;
            }
            
            // Video title (2 lines)
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                contentTop,
                contentWidth,
                TitleHeight);
            
            // Channel name
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                ChannelHeight);
            
            // View count and upload date
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.SubtitleRect.Bottom + 2,
                contentWidth,
                StatsHeight);
            
            ctx.ShowButton = true; // Play button
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw play button overlay
            DrawPlayButton(g, ctx);
            
            // Draw duration badge
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.RatingRect.IsEmpty)
            {
                using var bgBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.RatingRect, 4);
                g.FillPath(bgBrush, bgPath);
                
                using var textBrush = new SolidBrush(Color.White);
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _durationFont, textBrush, ctx.RatingRect, format);
            }
            
            // Draw quality/live badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw channel avatar
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var avatarBrush = new SolidBrush(Color.FromArgb(60, ctx.AccentColor));
                g.FillEllipse(avatarBrush, ctx.StatusRect);
                
                using var borderPen = new Pen(Color.FromArgb(40, ctx.AccentColor), 2);
                g.DrawEllipse(borderPen, ctx.StatusRect);
            }
            
            // Draw view count and date
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.ParagraphRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _statsFont, brush, ctx.ParagraphRect, format);
            }
            
            // Draw gradient overlay on thumbnail bottom
            if (!ctx.ImageRect.IsEmpty)
            {
                var gradientRect = new Rectangle(
                    ctx.ImageRect.Left,
                    ctx.ImageRect.Bottom - 50,
                    ctx.ImageRect.Width,
                    50);
                
                using var gradientBrush = new LinearGradientBrush(
                    new Point(gradientRect.Left, gradientRect.Top),
                    new Point(gradientRect.Left, gradientRect.Bottom),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(120, 0, 0, 0));
                
                g.FillRectangle(gradientBrush, gradientRect);
            }
        }
        
        private void DrawPlayButton(Graphics g, LayoutContext ctx)
        {
            if (ctx.ButtonRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Semi-transparent circle background
            using var circleBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
            g.FillEllipse(circleBrush, ctx.ButtonRect);
            
            // Play triangle
            int triangleSize = ctx.ButtonRect.Width / 3;
            int cx = ctx.ButtonRect.Left + ctx.ButtonRect.Width / 2;
            int cy = ctx.ButtonRect.Top + ctx.ButtonRect.Height / 2;
            
            // Offset slightly to the right for visual centering
            int offsetX = 3;
            
            Point[] triangle = new Point[]
            {
                new Point(cx - triangleSize / 2 + offsetX, cy - triangleSize / 2),
                new Point(cx - triangleSize / 2 + offsetX, cy + triangleSize / 2),
                new Point(cx + triangleSize / 2 + offsetX, cy)
            };
            
            using var triangleBrush = new SolidBrush(Color.White);
            g.FillPolygon(triangleBrush, triangle);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Play button hit area
            if (!ctx.ButtonRect.IsEmpty)
            {
                owner.AddHitArea("Play", ctx.ButtonRect, null,
                    () => notifyAreaHit?.Invoke("Play", ctx.ButtonRect));
            }
            
            // Thumbnail hit area (whole video area)
            if (!ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Thumbnail", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Thumbnail", ctx.ImageRect));
            }
            
            // Channel avatar hit area
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                owner.AddHitArea("Channel", ctx.StatusRect, null,
                    () => notifyAreaHit?.Invoke("Channel", ctx.StatusRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _channelFont?.Dispose();
            _statsFont?.Dispose();
            _durationFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

