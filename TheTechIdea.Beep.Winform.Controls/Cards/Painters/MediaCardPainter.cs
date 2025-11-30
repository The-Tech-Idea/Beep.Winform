using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// MediaCard - Large media display with overlay caption and play button for video.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class MediaCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Media card fonts
        private Font _captionFont;
        private Font _metaFont;
        private Font _badgeFont;
        private Font _statsFont;
        
        // Media card spacing
        private const int Padding = 12;
        private const int MediaHeightPercent = 70;
        private const int CaptionHeight = 24;
        private const int MetaHeight = 16;
        private const int StatsHeight = 16;
        private const int BadgeWidth = 60;
        private const int BadgeHeight = 24;
        private const int PlayButtonSize = 56;
        private const int GradientHeight = 50;
        private const int ElementGap = 6;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _captionFont?.Dispose(); } catch { }
            try { _metaFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            try { _statsFont?.Dispose(); } catch { }
            
            _captionFont = new Font(fontFamily, 12f, FontStyle.Bold);
            _metaFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
            _statsFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Large media area (70% of card)
            if (ctx.ShowImage)
            {
                int mediaHeight = Math.Max(100, (int)(drawingRect.Height * MediaHeightPercent / 100f));
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Top,
                    drawingRect.Width,
                    mediaHeight);
            }
            
            // Media type badge (Photo, Video, Gallery)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Play button centered on media (for video)
            bool isVideo = !string.IsNullOrEmpty(ctx.BadgeText2) && 
                          ctx.BadgeText2.Contains("Video", StringComparison.OrdinalIgnoreCase);
            if (ctx.ShowButton && isVideo && ctx.ShowImage)
            {
                ctx.ButtonRect = new Rectangle(
                    ctx.ImageRect.Left + (ctx.ImageRect.Width - PlayButtonSize) / 2,
                    ctx.ImageRect.Top + (ctx.ImageRect.Height - PlayButtonSize) / 2,
                    PlayButtonSize,
                    PlayButtonSize);
            }
            else
            {
                ctx.ShowButton = false;
            }
            
            // Content area below media
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + Padding : drawingRect.Top + Padding;
            int contentWidth = drawingRect.Width - Padding * 2;
            
            // Caption/title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                contentWidth,
                CaptionHeight);
            
            // Meta info (date, location, author)
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                MetaHeight);
            
            // Stats (views, likes, downloads)
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    ctx.HeaderRect.Left,
                    ctx.SubtitleRect.Bottom + ElementGap,
                    contentWidth,
                    StatsHeight);
            }
            
            ctx.ShowSecondaryButton = false;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw gradient overlay at bottom of media for caption contrast
            if (ctx.ShowImage && ctx.ImageRect.Height > GradientHeight)
            {
                var gradientRect = new Rectangle(
                    ctx.ImageRect.Left,
                    ctx.ImageRect.Bottom - GradientHeight,
                    ctx.ImageRect.Width,
                    GradientHeight);
                
                using var gradientBrush = new LinearGradientBrush(
                    new Point(gradientRect.Left, gradientRect.Top),
                    new Point(gradientRect.Left, gradientRect.Bottom),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(120, 0, 0, 0));
                g.FillRectangle(gradientBrush, gradientRect);
            }
            
            // Draw media type badge with semi-transparent background
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                var overlayBackColor = Color.FromArgb(200, ctx.Badge1BackColor);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    overlayBackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw play button overlay for video
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Semi-transparent circle background
                using var circleBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
                g.FillEllipse(circleBrush, ctx.ButtonRect);
                
                // Play triangle icon
                int iconSize = ctx.ButtonRect.Width / 3;
                int iconLeft = ctx.ButtonRect.Left + ctx.ButtonRect.Width / 2 - iconSize / 3;
                int iconTop = ctx.ButtonRect.Top + ctx.ButtonRect.Height / 2 - iconSize / 2;
                
                var playTriangle = new Point[]
                {
                    new Point(iconLeft, iconTop),
                    new Point(iconLeft, iconTop + iconSize),
                    new Point(iconLeft + iconSize, iconTop + iconSize / 2)
                };
                
                using var iconBrush = new SolidBrush(Color.White);
                g.FillPolygon(iconBrush, playTriangle);
            }
            
            // Draw media stats
            if (ctx.ShowRating && !string.IsNullOrEmpty(ctx.StatusText) && !ctx.RatingRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(140, ctx.AccentColor));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _statsFont, brush, ctx.RatingRect, format);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
            
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                owner.AddHitArea("PlayButton", ctx.ButtonRect, null,
                    () => notifyAreaHit?.Invoke("PlayButton", ctx.ButtonRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _captionFont?.Dispose();
            _metaFont?.Dispose();
            _badgeFont?.Dispose();
            _statsFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
