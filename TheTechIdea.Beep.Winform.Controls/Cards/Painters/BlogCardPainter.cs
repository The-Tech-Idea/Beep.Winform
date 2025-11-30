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
    /// BlogCard - Article-style layout with featured image, title, excerpt, and metadata.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class BlogCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Blog card fonts
        private Font _titleFont;
        private Font _excerptFont;
        private Font _metaFont;
        private Font _badgeFont;
        private Font _statsFont;
        
        // Blog card spacing
        private const int Padding = 16;
        private const int ImageHeightPercent = 40;
        private const int TitleHeight = 32;
        private const int MetaHeight = 18;
        private const int ExcerptMinHeight = 40;
        private const int ButtonHeight = 36;
        private const int BadgeWidth = 80;
        private const int BadgeHeight = 22;
        private const int StatsWidth = 100;
        private const int ElementGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _excerptFont?.Dispose(); } catch { }
            try { _metaFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            try { _statsFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _excerptFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _metaFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
            _statsFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Featured image at top (40% of height)
            if (ctx.ShowImage)
            {
                int imageHeight = Math.Min(
                    drawingRect.Width,
                    Math.Max(80, (int)(drawingRect.Height * ImageHeightPercent / 100f)));
                
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Top,
                    drawingRect.Width,
                    imageHeight);
            }
            
            // Category badge overlaying image (top-left)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Content area below image
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + Padding : drawingRect.Top + Padding;
            int contentWidth = drawingRect.Width - Padding * 2;
            int availableHeight = drawingRect.Bottom - contentTop - Padding;
            
            // Article title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                contentWidth,
                TitleHeight);
            
            // Author and date metadata
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                MetaHeight);
            
            // Article excerpt
            int excerptHeight = Math.Max(ExcerptMinHeight,
                availableHeight - TitleHeight - MetaHeight - (ctx.ShowButton ? ButtonHeight + ElementGap : 0) - ElementGap * 2);
            
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.SubtitleRect.Bottom + ElementGap,
                contentWidth,
                excerptHeight);
            
            // Read More button (left side)
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    (contentWidth - StatsWidth - ElementGap) / 2,
                    ButtonHeight);
            }
            
            // Engagement stats (likes, comments, shares) at right
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    drawingRect.Right - Padding - StatsWidth,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    StatsWidth,
                    ButtonHeight);
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
            // Draw category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw engagement stats
            if (ctx.ShowRating && !string.IsNullOrEmpty(ctx.StatusText) && !ctx.RatingRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(128, ctx.AccentColor));
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Far, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _statsFont, brush, ctx.RatingRect, format);
            }
            
            // Draw subtle divider line between image and content
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                using var pen = new Pen(Color.FromArgb(30, ctx.AccentColor), 1);
                g.DrawLine(pen, drawingRect.Left, ctx.ImageRect.Bottom,
                    drawingRect.Right, ctx.ImageRect.Bottom);
            }
        }
        
        private Rectangle drawingRect => _owner?.ClientRectangle ?? Rectangle.Empty;
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Category", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Category", ctx.BadgeRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _excerptFont?.Dispose();
            _metaFont?.Dispose();
            _badgeFont?.Dispose();
            _statsFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
