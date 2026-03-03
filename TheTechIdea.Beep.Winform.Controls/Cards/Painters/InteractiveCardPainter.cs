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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
            _metaFont = captionFont;
            _statsFont = captionFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descMinHeight = DpiScalingHelper.ScaleValue(DescMinHeight, _owner);
            int metaHeight = DpiScalingHelper.ScaleValue(MetaHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int chevronSize = DpiScalingHelper.ScaleValue(ChevronSize, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Icon or thumbnail (left side)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Top + padding,
                    iconSize,
                    iconSize);
            }
            
            // Content area (right of icon)
            int contentLeft = drawingRect.Left + padding + (ctx.ShowImage ? iconSize + contentGap : 0);
            int contentWidth = drawingRect.Width - padding * 2 - (ctx.ShowImage ? iconSize + contentGap : 0) - chevronSize - elementGap;
            
            // Title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + padding,
                contentWidth - badgeWidth - elementGap,
                titleHeight);
            
            // Status badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth - chevronSize - elementGap,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Description
            int descHeight = Math.Max(descMinHeight,
                drawingRect.Height - (ctx.HeaderRect.Bottom - drawingRect.Top + elementGap) - padding * 2 - 
                (ctx.ShowButton ? buttonHeight + elementGap : 0) - 
                (!string.IsNullOrEmpty(ctx.SubtitleText) || ctx.ShowRating ? metaHeight + elementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + elementGap,
                contentWidth,
                descHeight);
            
            // Metadata (file size, type, date, etc.)
            if (!string.IsNullOrEmpty(ctx.SubtitleText) || ctx.ShowRating)
            {
                ctx.SubtitleRect = new Rectangle(
                    contentLeft,
                    ctx.ParagraphRect.Bottom + elementGap,
                    contentWidth / 2,
                    metaHeight);
                
                // Stats area (downloads, views, etc.)
                ctx.RatingRect = new Rectangle(
                    ctx.SubtitleRect.Right + elementGap,
                    ctx.SubtitleRect.Top,
                    contentWidth - ctx.SubtitleRect.Width - elementGap,
                    metaHeight);
            }
            
            // Action buttons
            if (ctx.ShowButton)
            {
                int buttonTop = drawingRect.Bottom - padding - buttonHeight;
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (contentWidth - padding) / 2;
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonTop, buttonWidth, buttonHeight);
                    ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + padding, buttonTop, buttonWidth, buttonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonTop, contentWidth, buttonHeight);
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
                
                using var borderPen = new Pen(Color.FromArgb(50, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner));
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
            int chevronSize = DpiScalingHelper.ScaleValue(ChevronSize, _owner);
            int chevronX = ctx.DrawingRect.Right - DpiScalingHelper.ScaleValue(Padding, _owner) - chevronSize / 2;
            int chevronY = ctx.DrawingRect.Top + (ctx.DrawingRect.Height - chevronSize) / 2;
            
            using var pen = new Pen(Color.FromArgb(70, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner));
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            // Draw right-pointing chevron
            g.DrawLine(pen, chevronX - DpiScalingHelper.ScaleValue(4, _owner), chevronY + DpiScalingHelper.ScaleValue(4, _owner), chevronX + DpiScalingHelper.ScaleValue(2, _owner), chevronY + chevronSize / 2);
            g.DrawLine(pen, chevronX + DpiScalingHelper.ScaleValue(2, _owner), chevronY + chevronSize / 2, chevronX - DpiScalingHelper.ScaleValue(4, _owner), chevronY + chevronSize - DpiScalingHelper.ScaleValue(4, _owner));
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
_disposed = true;
        }
        
        #endregion
    }
}
