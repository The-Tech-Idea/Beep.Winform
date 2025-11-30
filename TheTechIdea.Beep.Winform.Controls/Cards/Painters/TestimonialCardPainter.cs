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
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _quoteMarkFont?.Dispose(); } catch { }
            try { _quoteFont?.Dispose(); } catch { }
            try { _nameFont?.Dispose(); } catch { }
            try { _titleFont?.Dispose(); } catch { }
            
            _quoteMarkFont = new Font("Georgia", 42f, FontStyle.Bold);
            _quoteFont = new Font(fontFamily, 11f, FontStyle.Italic);
            _nameFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _titleFont = new Font(fontFamily, 9f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Quote area at top (50% of height)
            int quoteHeight = Math.Max(60, (int)(drawingRect.Height * QuoteHeightPercent / 100f));
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding + 20, // Indent for quote mark
                drawingRect.Top + Padding + QuoteMarkSize / 2,
                drawingRect.Width - Padding * 2 - 20,
                quoteHeight);
            
            // Avatar at bottom-left
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Bottom - Padding - AvatarSize,
                AvatarSize,
                AvatarSize);
            
            // Name and title to the right of avatar
            int nameLeft = ctx.ImageRect.Right + ElementGap * 2;
            int nameWidth = Math.Max(80, drawingRect.Width - nameLeft - Padding);
            
            ctx.HeaderRect = new Rectangle(
                nameLeft,
                ctx.ImageRect.Top + 4,
                nameWidth,
                NameHeight);
            
            ctx.SubtitleRect = new Rectangle(
                nameLeft,
                ctx.HeaderRect.Bottom + 2,
                nameWidth,
                TitleHeight);
            
            // Rating below name
            ctx.RatingRect = new Rectangle(
                nameLeft,
                ctx.SubtitleRect.Bottom + ElementGap / 2,
                100,
                RatingHeight);
            
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
                ctx.DrawingRect.Left + Padding - 5, 
                ctx.DrawingRect.Top + Padding - 10);
            
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
            
            _quoteMarkFont?.Dispose();
            _quoteFont?.Dispose();
            _nameFont?.Dispose();
            _titleFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
