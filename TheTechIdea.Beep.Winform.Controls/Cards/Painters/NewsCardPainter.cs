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
    /// NewsCard - News article with thumbnail, headline, and source.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class NewsCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // News card fonts
        private Font _headlineFont;
        private Font _sourceFont;
        private Font _timeFont;
        private Font _badgeFont;
        
        // News card spacing
        private const int Padding = 12;
        private const int ThumbnailWidth = 120;
        private const int ThumbnailMinHeight = 80;
        private const int HeadlineHeight = 48;
        private const int SourceHeight = 18;
        private const int TimeHeight = 16;
        private const int BadgeWidth = 70;
        private const int BadgeHeight = 20;
        private const int ElementGap = 8;
        private const int ContentGap = 14;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_headlineFont = titleFont;
            _sourceFont = captionFont;
            _timeFont = captionFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int thumbnailWidth = DpiScalingHelper.ScaleValue(ThumbnailWidth, _owner);
            int thumbnailMinHeight = DpiScalingHelper.ScaleValue(ThumbnailMinHeight, _owner);
            int headlineHeight = DpiScalingHelper.ScaleValue(HeadlineHeight, _owner);
            int sourceHeight = DpiScalingHelper.ScaleValue(SourceHeight, _owner);
            int timeHeight = DpiScalingHelper.ScaleValue(TimeHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Thumbnail on left
            if (ctx.ShowImage)
            {
                int thumbnailHeight = Math.Max(thumbnailMinHeight, drawingRect.Height - padding * 2);
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Top + padding,
                    thumbnailWidth,
                    thumbnailHeight);
            }
            
            int contentLeft = drawingRect.Left + padding + (ctx.ShowImage ? thumbnailWidth + contentGap : 0);
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - padding;
            
            // Breaking/Live badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
                contentWidth -= badgeWidth + elementGap;
            }
            
            // Headline (title)
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + padding,
                contentWidth,
                headlineHeight);
            
            // Source name with icon
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + elementGap,
                contentWidth / 2,
                sourceHeight);
            
            // Time ago
            ctx.StatusRect = new Rectangle(
                ctx.SubtitleRect.Right + elementGap,
                ctx.SubtitleRect.Top,
                contentWidth / 2 - elementGap,
                timeHeight);
            
            // Category/tags
            if (ctx.Tags != null)
            {
                ctx.TagsRect = new Rectangle(
                    contentLeft,
                    drawingRect.Bottom - padding - DpiScalingHelper.ScaleValue(20, _owner),
                    contentWidth + (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : badgeWidth + elementGap),
                    DpiScalingHelper.ScaleValue(20, _owner));
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
            // Draw breaking/live badge with pulsing effect
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                bool isLive = ctx.BadgeText1.ToUpper().Contains("LIVE") || ctx.BadgeText1.ToUpper().Contains("BREAKING");
                Color badgeBack = isLive ? Color.FromArgb(220, 53, 69) : ctx.Badge1BackColor;
                Color badgeFore = isLive ? Color.White : ctx.Badge1ForeColor;
                
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, badgeBack, badgeFore, _badgeFont);
                
                // Live indicator dot
                if (isLive)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    var dotRect = new Rectangle(ctx.BadgeRect.Left + 6, ctx.BadgeRect.Top + (ctx.BadgeRect.Height - 8) / 2, 8, 8);
                    using var dotBrush = new SolidBrush(Color.White);
                    g.FillEllipse(dotBrush, dotRect);
                }
            }
            
            // Draw source with icon
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                DrawSourceInfo(g, ctx);
            }
            
            // Draw time ago
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _timeFont, brush, ctx.StatusRect, format);
            }
            
            // Draw category tags
            if (ctx.Tags != null && !ctx.TagsRect.IsEmpty)
            {
                CardRenderingHelpers.DrawChips(g, _owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags, _timeFont);
            }
            
            // Draw thumbnail overlay gradient
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                var gradientRect = new Rectangle(
                    ctx.ImageRect.Right - 30,
                    ctx.ImageRect.Top,
                    30,
                    ctx.ImageRect.Height);
                
                using var gradientBrush = new LinearGradientBrush(
                    new Point(gradientRect.Left, gradientRect.Top),
                    new Point(gradientRect.Right, gradientRect.Top),
                    Color.FromArgb(0, 255, 255, 255),
                    Color.FromArgb(200, 255, 255, 255));
                
                g.FillRectangle(gradientBrush, gradientRect);
            }
        }
        
        private void DrawSourceInfo(Graphics g, LayoutContext ctx)
        {
            // Source icon placeholder
            int iconSize = 14;
            var iconRect = new Rectangle(ctx.SubtitleRect.Left, ctx.SubtitleRect.Top + 2, iconSize, iconSize);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var iconBrush = new SolidBrush(Color.FromArgb(60, ctx.AccentColor));
            g.FillEllipse(iconBrush, iconRect);
            
            // Source name
            var textRect = new Rectangle(
                iconRect.Right + 6,
                ctx.SubtitleRect.Top,
                ctx.SubtitleRect.Width - iconSize - 6,
                ctx.SubtitleRect.Height);
            
            using var textBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.SubtitleText, _sourceFont, textBrush, textRect, format);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Thumbnail hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Thumbnail", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Thumbnail", ctx.ImageRect));
            }
            
            // Headline hit area (main clickable)
            if (!ctx.HeaderRect.IsEmpty)
            {
                owner.AddHitArea("Headline", ctx.HeaderRect, null,
                    () => notifyAreaHit?.Invoke("Headline", ctx.HeaderRect));
            }
            
            // Source hit area
            if (!ctx.SubtitleRect.IsEmpty)
            {
                owner.AddHitArea("Source", ctx.SubtitleRect, null,
                    () => notifyAreaHit?.Invoke("Source", ctx.SubtitleRect));
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

