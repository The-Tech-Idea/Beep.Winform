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
    /// ListCard - Horizontal list item with avatar, title, subtitle, and rating/badge.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ListCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // List card fonts
        private Font _titleFont;
        private Font _subtitleFont;
        private Font _descFont;
        private Font _badgeFont;
        
        // List card spacing
        private const int Padding = 12;
        private const int AvatarSize = 44;
        private const int TitleHeight = 22;
        private const int SubtitleHeight = 18;
        private const int RatingWidth = 100;
        private const int RatingHeight = 18;
        private const int BadgeWidth = 80;
        private const int BadgeHeight = 20;
        private const int ElementGap = 6;
        private const int ContentGap = 14;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _subtitleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _subtitleFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Avatar (vertically centered on left)
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + (drawingRect.Height - AvatarSize) / 2,
                AvatarSize,
                AvatarSize);
            
            // Content area (right of avatar)
            int contentLeft = ctx.ImageRect.Right + ContentGap;
            int rightAreaWidth = Math.Max(RatingWidth, BadgeWidth) + Padding;
            int contentWidth = Math.Max(80, drawingRect.Width - contentLeft - rightAreaWidth);
            
            // Title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth,
                TitleHeight);
            
            // Subtitle (role, position, etc.)
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                SubtitleHeight);
            
            // Description/additional info
            int descHeight = Math.Max(0, drawingRect.Bottom - ctx.SubtitleRect.Bottom - Padding - ElementGap);
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.SubtitleRect.Bottom + ElementGap,
                contentWidth,
                descHeight);
            
            // Rating stars (top-right) OR Badge
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    drawingRect.Right - Padding - RatingWidth,
                    drawingRect.Top + Padding,
                    RatingWidth,
                    RatingHeight);
            }
            else if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // No buttons for list cards
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
            // Draw rating stars
            if (ctx.ShowRating && ctx.Rating > 0 && !ctx.RatingRect.IsEmpty)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
            // Draw badge (if no rating)
            else if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw avatar border
            if (!ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Color.FromArgb(40, ctx.AccentColor), 2);
                g.DrawEllipse(pen, ctx.ImageRect);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (ctx.ShowRating && !ctx.RatingRect.IsEmpty)
            {
                owner.AddHitArea("Rating", ctx.RatingRect, null,
                    () => notifyAreaHit?.Invoke("Rating", ctx.RatingRect));
            }
            
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
            
            if (!ctx.ImageRect.IsEmpty)
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
            
            _titleFont?.Dispose();
            _subtitleFont?.Dispose();
            _descFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

