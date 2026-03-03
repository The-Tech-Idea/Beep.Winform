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
    /// ContentCard - Banner image at top with content below, tags, and action button.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ContentCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Content card fonts
        private Font _titleFont;
        private Font _excerptFont;
        private Font _badgeFont;
        private Font _metaFont;
        
        // Content card spacing
        private const int Padding = 16;
        private const int BannerHeightPercent = 45;
        private const int TitleHeight = 28;
        private const int ExcerptHeight = 40;
        private const int TagsHeight = 24;
        private const int ButtonHeight = 36;
        private const int ButtonWidth = 100;
        private const int BadgeWidth = 60;
        private const int BadgeHeight = 22;
        private const int ElementGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _excerptFont = bodyFont;
            _badgeFont = captionFont;
            _metaFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int excerptHeightMin = DpiScalingHelper.ScaleValue(ExcerptHeight, _owner);
            int tagsHeight = DpiScalingHelper.ScaleValue(TagsHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int buttonWidth = DpiScalingHelper.ScaleValue(ButtonWidth, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Full-width banner image at top (45% of height)
            int bannerHeight = Math.Max(60, Math.Min(
                (int)(drawingRect.Height * BannerHeightPercent / 100f),
                drawingRect.Height - (padding * 3)));
            
            ctx.ImageRect = new Rectangle(
                drawingRect.Left,
                drawingRect.Top,
                drawingRect.Width,
                bannerHeight);
            
            // Badge overlay on banner (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Content area below banner
            int contentTop = ctx.ImageRect.Bottom + padding;
            int contentWidth = drawingRect.Width - padding * 2;
            int availableHeight = drawingRect.Bottom - contentTop - padding;
            
            // Title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                contentWidth,
                titleHeight);
            
            // Subtitle/meta info (author, date)
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + elementGap / 2,
                contentWidth,
                DpiScalingHelper.ScaleValue(18, _owner));
            
            // Excerpt/description
            int excerptHeight = Math.Max(excerptHeightMin, availableHeight - titleHeight - tagsHeight - buttonHeight - elementGap * 4);
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.SubtitleRect.Bottom + elementGap,
                contentWidth,
                excerptHeight);
            
            // Tags row
            ctx.TagsRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.ParagraphRect.Bottom + elementGap,
                contentWidth - buttonWidth - elementGap,
                tagsHeight);
            
            // Action button at bottom-right
            ctx.ButtonRect = new Rectangle(
                drawingRect.Right - padding - buttonWidth,
                drawingRect.Bottom - padding - buttonHeight,
                buttonWidth,
                buttonHeight);
            
            ctx.ShowSecondaryButton = false;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw badge overlay on banner
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw tags/chips
            if (ctx.Tags != null && ctx.Tags.Count > 0 && !ctx.TagsRect.IsEmpty)
            {
                CardRenderingHelpers.DrawChips(g, _owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags, _metaFont);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
            
            if (!ctx.TagsRect.IsEmpty)
            {
                owner.AddHitArea("Tags", ctx.TagsRect, null,
                    () => notifyAreaHit?.Invoke("Tags", ctx.TagsRect));
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
