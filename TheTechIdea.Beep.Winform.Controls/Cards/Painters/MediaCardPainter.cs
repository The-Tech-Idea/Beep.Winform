using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_captionFont = captionFont;
            _metaFont = captionFont;
            _badgeFont = captionFont;
            _statsFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int captionHeight = DpiScalingHelper.ScaleValue(CaptionHeight, _owner);
            int metaHeight = DpiScalingHelper.ScaleValue(MetaHeight, _owner);
            int statsHeight = DpiScalingHelper.ScaleValue(StatsHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int playButtonSize = DpiScalingHelper.ScaleValue(PlayButtonSize, _owner);
            int gradientHeight = DpiScalingHelper.ScaleValue(GradientHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
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
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Play button centered on media (for video)
            bool isVideo = !string.IsNullOrEmpty(ctx.BadgeText2) && 
                          ctx.BadgeText2.Contains("Video", StringComparison.OrdinalIgnoreCase);
            bool showPlayOverlay = ctx.ShowButton && isVideo && ctx.ShowImage;
            if (showPlayOverlay)
            {
                ctx.ButtonRect = new Rectangle(
                    ctx.ImageRect.Left + (ctx.ImageRect.Width - playButtonSize) / 2,
                    ctx.ImageRect.Top + (ctx.ImageRect.Height - playButtonSize) / 2,
                    playButtonSize,
                    playButtonSize);
            }
            else
            {
                ctx.ButtonRect = Rectangle.Empty;
            }

            ctx.ShowButton = false;
            
            // Content area below media
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + padding : drawingRect.Top + padding;
            int contentWidth = drawingRect.Width - padding * 2;
            
            // Caption/title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                contentWidth,
                captionHeight);
            
            // Meta info (date, location, author)
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + elementGap / 2,
                contentWidth,
                metaHeight);
            
            // Stats (views, likes, downloads)
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    ctx.HeaderRect.Left,
                    ctx.SubtitleRect.Bottom + elementGap,
                    contentWidth,
                    statsHeight);
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
            int gradientHeight = DpiScalingHelper.ScaleValue(GradientHeight, _owner);

            // Draw gradient overlay at bottom of media for caption contrast
            if (ctx.ShowImage && ctx.ImageRect.Height > gradientHeight)
            {
                var gradientRect = new Rectangle(
                    ctx.ImageRect.Left,
                    ctx.ImageRect.Bottom - gradientHeight,
                    ctx.ImageRect.Width,
                    gradientHeight);
                
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

            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                using var subtitleBrush = new SolidBrush(subtitleColor);
                var subtitleFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _metaFont, subtitleBrush, ctx.SubtitleRect, subtitleFormat);
            }
            
            // Draw play button overlay for video
            if (!ctx.ButtonRect.IsEmpty)
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
            
            if (!ctx.ButtonRect.IsEmpty)
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
_disposed = true;
        }
        
        #endregion
    }
}
