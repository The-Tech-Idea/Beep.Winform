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
    /// InteractiveCard - Hover/download/contact card with icon, metadata, and interaction indicators.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class InteractiveCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Interactive card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _metaFont;
        private Font _statsFont;
        private Font _badgeFont;
        
        // Interactive card spacing
        private const int Padding = 16;
        private const int IconSize = 52;
        private const int TitleHeight = 24;
        private const int DescMinHeight = 30;
        private const int MetaHeight = 18;
        private const int BadgeWidth = 70;
        private const int BadgeHeight = 22;
        private const int ButtonHeight = 38;
        private const int ChevronSize = 18;
        private const int ElementGap = 8;
        private const int ContentGap = 14;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _metaFont?.Dispose(); } catch { }
            try { _statsFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 12f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _metaFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _statsFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Icon or thumbnail (left side)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
            }
            
            // Content area (right of icon)
            int contentLeft = drawingRect.Left + Padding + (ctx.ShowImage ? IconSize + ContentGap : 0);
            int contentWidth = drawingRect.Width - Padding * 2 - (ctx.ShowImage ? IconSize + ContentGap : 0) - ChevronSize - ElementGap;
            
            // Title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth - BadgeWidth - ElementGap,
                TitleHeight);
            
            // Status badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth - ChevronSize - ElementGap,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Description
            int descHeight = Math.Max(DescMinHeight,
                drawingRect.Height - (ctx.HeaderRect.Bottom - drawingRect.Top + ElementGap) - Padding * 2 - 
                (ctx.ShowButton ? ButtonHeight + ElementGap : 0) - 
                (!string.IsNullOrEmpty(ctx.SubtitleText) || ctx.ShowRating ? MetaHeight + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap,
                contentWidth,
                descHeight);
            
            // Metadata (file size, type, date, etc.)
            if (!string.IsNullOrEmpty(ctx.SubtitleText) || ctx.ShowRating)
            {
                ctx.SubtitleRect = new Rectangle(
                    contentLeft,
                    ctx.ParagraphRect.Bottom + ElementGap,
                    contentWidth / 2,
                    MetaHeight);
                
                // Stats area (downloads, views, etc.)
                ctx.RatingRect = new Rectangle(
                    ctx.SubtitleRect.Right + ElementGap,
                    ctx.SubtitleRect.Top,
                    contentWidth - ctx.SubtitleRect.Width - ElementGap,
                    MetaHeight);
            }
            
            // Action buttons
            if (ctx.ShowButton)
            {
                int buttonTop = drawingRect.Bottom - Padding - ButtonHeight;
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (contentWidth - Padding) / 2;
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonTop, buttonWidth, ButtonHeight);
                    ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + Padding, buttonTop, buttonWidth, ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonTop, contentWidth, ButtonHeight);
                }
            }
            
            // Hover effect area (full card)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = ctx.DrawingRect;
            }
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw status/category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw hover state overlay
            if (ctx.ShowStatus && ctx.StatusColor != Color.Empty && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(6, ctx.AccentColor));
                g.FillRectangle(brush, ctx.StatusRect);
            }
            
            // Draw icon background (for download/file cards)
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                using var bgBrush = new SolidBrush(Color.FromArgb(25, ctx.AccentColor));
                using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 8);
                g.FillPath(bgBrush, bgPath);
                
                using var borderPen = new Pen(Color.FromArgb(50, ctx.AccentColor), 2);
                g.DrawPath(borderPen, bgPath);
            }
            
            // Draw metadata
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(140, ctx.AccentColor));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _metaFont, brush, ctx.SubtitleRect, format);
            }
            
            // Draw stats (downloads, views)
            if (ctx.ShowRating && !string.IsNullOrEmpty(ctx.StatusText) && !ctx.RatingRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(110, ctx.AccentColor));
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Far, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _statsFont, brush, ctx.RatingRect, format);
            }
            
            // Draw chevron indicator (right side)
            DrawChevron(g, ctx);
        }
        
        private void DrawChevron(Graphics g, LayoutContext ctx)
        {
            int chevronX = ctx.DrawingRect.Right - Padding - ChevronSize / 2;
            int chevronY = ctx.DrawingRect.Top + (ctx.DrawingRect.Height - ChevronSize) / 2;
            
            using var pen = new Pen(Color.FromArgb(70, ctx.AccentColor), 2);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            // Draw right-pointing chevron
            g.DrawLine(pen, chevronX - 4, chevronY + 4, chevronX + 2, chevronY + ChevronSize / 2);
            g.DrawLine(pen, chevronX + 2, chevronY + ChevronSize / 2, chevronX - 4, chevronY + ChevronSize - 4);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
            
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Icon", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Icon", ctx.ImageRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _descFont?.Dispose();
            _metaFont?.Dispose();
            _statsFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
