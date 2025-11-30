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
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _nameFont?.Dispose(); } catch { }
            try { _dateFont?.Dispose(); } catch { }
            try { _reviewFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            try { _helpfulFont?.Dispose(); } catch { }
            try { _quoteFont?.Dispose(); } catch { }
            
            _nameFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _dateFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _reviewFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _helpfulFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _quoteFont = new Font("Georgia", 36f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // User avatar (top-left)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + Padding,
                    AvatarSize,
                    AvatarSize);
            }
            
            // User info area (right of avatar)
            int infoLeft = drawingRect.Left + Padding + (ctx.ShowImage ? AvatarSize + ContentGap : 0);
            int infoWidth = drawingRect.Width - Padding * 2 - (ctx.ShowImage ? AvatarSize + ContentGap : 0);
            
            // User name
            ctx.HeaderRect = new Rectangle(
                infoLeft,
                drawingRect.Top + Padding,
                infoWidth - RatingWidth - ElementGap,
                NameHeight);
            
            // Review date
            ctx.SubtitleRect = new Rectangle(
                infoLeft,
                ctx.HeaderRect.Bottom + 2,
                infoWidth / 2,
                DateHeight);
            
            // Star rating (top-right)
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    drawingRect.Right - Padding - RatingWidth,
                    drawingRect.Top + Padding,
                    RatingWidth,
                    RatingHeight);
            }
            
            // Verification badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    infoLeft,
                    ctx.SubtitleRect.Bottom + ElementGap,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Review text
            int reviewTop = Math.Max(
                ctx.ShowImage ? ctx.ImageRect.Bottom : ctx.SubtitleRect.Bottom,
                ctx.SubtitleRect.Bottom) + 
                (string.IsNullOrEmpty(ctx.BadgeText1) ? ElementGap * 2 : BadgeHeight + ElementGap * 2);
            
            int reviewHeight = Math.Max(ReviewMinHeight,
                drawingRect.Height - (reviewTop - drawingRect.Top) - Padding * 2 - 
                (ctx.ShowButton ? ButtonHeight + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                reviewTop,
                drawingRect.Width - Padding * 2,
                reviewHeight);
            
            // Helpful button and count
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    ButtonWidth,
                    ButtonHeight);
                
                // Helpful count display
                if (!string.IsNullOrEmpty(ctx.StatusText))
                {
                    ctx.StatusRect = new Rectangle(
                        ctx.ButtonRect.Right + ElementGap * 2,
                        ctx.ButtonRect.Top,
                        drawingRect.Right - Padding - ctx.ButtonRect.Right - ElementGap * 2,
                        ButtonHeight);
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
                using var pen = new Pen(Color.FromArgb(50, ctx.AccentColor), 2);
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
            
            _nameFont?.Dispose();
            _dateFont?.Dispose();
            _reviewFont?.Dispose();
            _badgeFont?.Dispose();
            _helpfulFont?.Dispose();
            _quoteFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
