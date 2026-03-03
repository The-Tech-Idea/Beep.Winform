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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _subtitleFont = titleFont;
            _descFont = bodyFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int avatarSize = DpiScalingHelper.ScaleValue(AvatarSize, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int subtitleHeight = DpiScalingHelper.ScaleValue(SubtitleHeight, _owner);
            int ratingWidth = DpiScalingHelper.ScaleValue(RatingWidth, _owner);
            int ratingHeight = DpiScalingHelper.ScaleValue(RatingHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Avatar (vertically centered on left)
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + padding,
                drawingRect.Top + (drawingRect.Height - avatarSize) / 2,
                avatarSize,
                avatarSize);
            
            // Content area (right of avatar)
            int contentLeft = ctx.ImageRect.Right + contentGap;
            int rightAreaWidth = Math.Max(ratingWidth, badgeWidth) + padding;
            int contentWidth = Math.Max(DpiScalingHelper.ScaleValue(80, _owner), drawingRect.Width - contentLeft - rightAreaWidth);
            
            // Title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + padding,
                contentWidth,
                titleHeight);
            
            // Subtitle (role, position, etc.)
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + elementGap / 2,
                contentWidth,
                subtitleHeight);
            
            // Description/additional info
            int descHeight = Math.Max(0, drawingRect.Bottom - ctx.SubtitleRect.Bottom - padding - elementGap);
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.SubtitleRect.Bottom + elementGap,
                contentWidth,
                descHeight);
            
            // Rating stars (top-right) OR Badge
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    drawingRect.Right - padding - ratingWidth,
                    drawingRect.Top + padding,
                    ratingWidth,
                    ratingHeight);
            }
            else if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
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
                using var pen = new Pen(Color.FromArgb(40, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner));
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
_disposed = true;
        }
        
        #endregion
    }
}

