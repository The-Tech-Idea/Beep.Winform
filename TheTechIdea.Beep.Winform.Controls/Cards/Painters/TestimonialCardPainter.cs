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
    /// TestimonialCard - Quote style with large quote marks, avatar, name, and rating.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class TestimonialCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Testimonial card fonts
        private Font _quoteMarkFont;
        private Font _quoteFont;
        private Font _nameFont;
        private Font _titleFont;
        
        // Testimonial card spacing
        private const int Padding = 20;
        private const int QuoteMarkSize = 48;
        private const int QuoteHeightPercent = 50;
        private const int AvatarSize = 52;
        private const int NameHeight = 22;
        private const int TitleHeight = 18;
        private const int RatingHeight = 18;
        private const int ElementGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_quoteMarkFont = titleFont;
            _quoteFont = titleFont;
            _nameFont = titleFont;
            _titleFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int quoteMarkSize = DpiScalingHelper.ScaleValue(QuoteMarkSize, _owner);
            int avatarSize = DpiScalingHelper.ScaleValue(AvatarSize, _owner);
            int nameHeight = DpiScalingHelper.ScaleValue(NameHeight, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int ratingHeight = DpiScalingHelper.ScaleValue(RatingHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Quote area at top (50% of height)
            int quoteHeight = Math.Max(60, (int)(drawingRect.Height * QuoteHeightPercent / 100f));
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding + DpiScalingHelper.ScaleValue(20, _owner), // Indent for quote mark
                drawingRect.Top + padding + quoteMarkSize / 2,
                drawingRect.Width - padding * 2 - DpiScalingHelper.ScaleValue(20, _owner),
                quoteHeight);
            
            // Avatar at bottom-left
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Bottom - padding - avatarSize,
                avatarSize,
                avatarSize);
            
            // Name and title to the right of avatar
            int nameLeft = ctx.ImageRect.Right + elementGap * 2;
            int nameWidth = Math.Max(80, drawingRect.Width - nameLeft - Padding);
            
            ctx.HeaderRect = new Rectangle(
                nameLeft,
                ctx.ImageRect.Top + 4,
                nameWidth,
                nameHeight);
            
            ctx.SubtitleRect = new Rectangle(
                nameLeft,
                ctx.HeaderRect.Bottom + 2,
                nameWidth,
                titleHeight);
            
            // Rating below name
            ctx.RatingRect = new Rectangle(
                nameLeft,
                ctx.SubtitleRect.Bottom + elementGap / 2,
                DpiScalingHelper.ScaleValue(100, _owner),
                ratingHeight);
            
            // No buttons for testimonial cards
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
            // Draw large decorative quote mark
            using var quoteBrush = new SolidBrush(Color.FromArgb(25, ctx.AccentColor));
            g.DrawString("\u201C", _quoteMarkFont, quoteBrush, 
                ctx.DrawingRect.Left + DpiScalingHelper.ScaleValue(Padding, _owner) - DpiScalingHelper.ScaleValue(5, _owner), 
                ctx.DrawingRect.Top + DpiScalingHelper.ScaleValue(Padding, _owner) - DpiScalingHelper.ScaleValue(10, _owner));

            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                using var subtitleBrush = new SolidBrush(subtitleColor);
                var subtitleFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _titleFont, subtitleBrush, ctx.SubtitleRect, subtitleFormat);
            }
            
            // Draw rating stars
            if (ctx.ShowRating && ctx.Rating > 0 && !ctx.RatingRect.IsEmpty)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (ctx.ShowRating && !ctx.RatingRect.IsEmpty)
            {
                owner.AddHitArea("Rating", ctx.RatingRect, null,
                    () => notifyAreaHit?.Invoke("Rating", ctx.RatingRect));
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
