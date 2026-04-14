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
    /// ReviewCard - User review with avatar, rating, verification badge, and helpful actions.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ReviewCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Review card fonts
        private Font _nameFont;
        private Font _dateFont;
        private Font _reviewFont;
        private Font _badgeFont;
        private Font _helpfulFont;
        private Font _quoteFont;
        
        // Review card spacing
        private const int Padding = 16;
        private const int AvatarSize = 48;
        private const int NameHeight = 20;
        private const int DateHeight = 16;
        private const int RatingWidth = 90;
        private const int RatingHeight = 20;
        private const int BadgeWidth = 110;
        private const int BadgeHeight = 20;
        private const int ReviewMinHeight = 50;
        private const int ButtonHeight = 36;
        private const int ButtonWidth = 100;
        private const int ElementGap = 8;
        private const int ContentGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_nameFont = titleFont;
            _dateFont = captionFont;
            _reviewFont = bodyFont;
            _badgeFont = captionFont;
            _helpfulFont = bodyFont;
            _quoteFont = titleFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int avatarSize = DpiScalingHelper.ScaleValue(AvatarSize, _owner);
            int nameHeight = DpiScalingHelper.ScaleValue(NameHeight, _owner);
            int dateHeight = DpiScalingHelper.ScaleValue(DateHeight, _owner);
            int ratingWidth = DpiScalingHelper.ScaleValue(RatingWidth, _owner);
            int ratingHeight = DpiScalingHelper.ScaleValue(RatingHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int reviewMinHeight = DpiScalingHelper.ScaleValue(ReviewMinHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int buttonWidth = DpiScalingHelper.ScaleValue(ButtonWidth, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // User avatar (top-left)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Top + padding,
                    avatarSize,
                    avatarSize);
            }
            
            // User info area (right of avatar)
            int infoLeft = drawingRect.Left + padding + (ctx.ShowImage ? avatarSize + contentGap : 0);
            int infoWidth = drawingRect.Width - padding * 2 - (ctx.ShowImage ? avatarSize + contentGap : 0);
            
            // User name
            ctx.HeaderRect = new Rectangle(
                infoLeft,
                drawingRect.Top + padding,
                infoWidth - ratingWidth - elementGap,
                nameHeight);
            
            // Review date
            ctx.SubtitleRect = new Rectangle(
                infoLeft,
                ctx.HeaderRect.Bottom + 2,
                infoWidth / 2,
                dateHeight);
            
            // Star rating (top-right)
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    drawingRect.Right - padding - ratingWidth,
                    drawingRect.Top + padding,
                    ratingWidth,
                    ratingHeight);
            }
            
            // Verification badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    infoLeft,
                    ctx.SubtitleRect.Bottom + elementGap,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Review text
            int reviewTop = Math.Max(
                ctx.ShowImage ? ctx.ImageRect.Bottom : ctx.SubtitleRect.Bottom,
                ctx.SubtitleRect.Bottom) + 
                (string.IsNullOrEmpty(ctx.BadgeText1) ? elementGap * 2 : badgeHeight + elementGap * 2);
            
            int reviewHeight = Math.Max(reviewMinHeight,
                drawingRect.Height - (reviewTop - drawingRect.Top) - padding * 2 - 
                (ctx.ShowButton ? buttonHeight + elementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                reviewTop,
                drawingRect.Width - padding * 2,
                reviewHeight);
            
            // Helpful button and count
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Bottom - padding - buttonHeight,
                    buttonWidth,
                    buttonHeight);
                
                // Helpful count display
                if (!string.IsNullOrEmpty(ctx.StatusText))
                {
                    ctx.StatusRect = new Rectangle(
                        ctx.ButtonRect.Right + elementGap * 2,
                        ctx.ButtonRect.Top,
                        drawingRect.Right - padding - ctx.ButtonRect.Right - elementGap * 2,
                        buttonHeight);
                }
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
            // Draw star rating
            if (ctx.ShowRating && ctx.Rating > 0 && !ctx.RatingRect.IsEmpty)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }

            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                using var subtitleBrush = new SolidBrush(subtitleColor);
                var subtitleFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _dateFont, subtitleBrush, ctx.SubtitleRect, subtitleFormat);
            }
            
            // Draw verification badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw helpful count
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(120, ctx.AccentColor));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _helpfulFont, brush, ctx.StatusRect, format);
            }
            
            // Draw avatar border
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                using var pen = new Pen(Color.FromArgb(50, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawEllipse(pen, ctx.ImageRect);
            }
            
            // Draw decorative quote mark
            if (!ctx.ParagraphRect.IsEmpty && ctx.ParagraphRect.Height > 40)
            {
                using var brush = new SolidBrush(Color.FromArgb(12, ctx.AccentColor));
                g.DrawString("\"", _quoteFont, brush, 
                    ctx.ParagraphRect.Left - 6, ctx.ParagraphRect.Top - 14);
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
