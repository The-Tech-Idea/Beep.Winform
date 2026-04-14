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
    /// EventCard - Event display with accent bar, date block, and tags.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class EventCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Event card fonts
        private Font _titleFont;
        private Font _locationFont;
        private Font _descFont;
        private Font _dateFont;
        private Font _badgeFont;
        
        // Event card spacing
        private const int Padding = 12;
        private const int AccentBarWidth = 5;
        private const int DateBlockWidth = 60;
        private const int DateBlockHeight = 54;
        private const int TitleHeight = 22;
        private const int LocationHeight = 18;
        private const int DescHeight = 40;
        private const int TagsHeight = 24;
        private const int ButtonHeight = 28;
        private const int ButtonWidth = 80;
        private const int ElementGap = 8;
        private const int ContentGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _locationFont = captionFont;
            _descFont = bodyFont;
            _dateFont = titleFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int accentBarWidth = DpiScalingHelper.ScaleValue(AccentBarWidth, _owner);
            int dateBlockWidth = DpiScalingHelper.ScaleValue(DateBlockWidth, _owner);
            int dateBlockHeight = DpiScalingHelper.ScaleValue(DateBlockHeight, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int locationHeight = DpiScalingHelper.ScaleValue(LocationHeight, _owner);
            int descHeightMin = DpiScalingHelper.ScaleValue(DescHeight, _owner);
            int tagsHeight = DpiScalingHelper.ScaleValue(TagsHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int buttonWidth = DpiScalingHelper.ScaleValue(ButtonWidth, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Accent bar on left edge
            ctx.StatusRect = new Rectangle(
                drawingRect.Left,
                drawingRect.Top,
                accentBarWidth,
                drawingRect.Height);
            
            // Date block at top-left (after accent bar)
            ctx.ImageRect = new Rectangle(
                ctx.StatusRect.Right + padding,
                drawingRect.Top + padding,
                dateBlockWidth,
                dateBlockHeight);
            
            // Content area to the right of date block
            int contentLeft = ctx.ImageRect.Right + contentGap;
            int contentWidth = Math.Max(DpiScalingHelper.ScaleValue(80, _owner), drawingRect.Width - contentLeft - padding);
            
            // Event title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + padding,
                contentWidth,
                titleHeight);
            
            // Location/venue
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + elementGap / 2,
                contentWidth,
                locationHeight);
            
            // Description
            int descHeight = Math.Max(descHeightMin, drawingRect.Height - padding * 2 - titleHeight - locationHeight - tagsHeight - buttonHeight - elementGap * 4);
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.SubtitleRect.Bottom + elementGap,
                contentWidth,
                descHeight);
            
            // Tags row
            ctx.TagsRect = new Rectangle(
                contentLeft,
                ctx.ParagraphRect.Bottom + elementGap,
                contentWidth - buttonWidth - elementGap,
                tagsHeight);
            
            // Action button (RSVP, Join, etc.)
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Right - padding - buttonWidth,
                    drawingRect.Bottom - padding - buttonHeight,
                    buttonWidth,
                    buttonHeight);
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
            // Draw accent bar with rounded corners
            if (!ctx.StatusRect.IsEmpty)
            {
                using var path = CardRenderingHelpers.CreateRoundedPath(ctx.StatusRect, 2);
                using var brush = new SolidBrush(ctx.AccentColor);
                g.FillPath(brush, path);
            }
            
            // Draw date/time block
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.ImageRect.IsEmpty)
            {
                // Background for date block
                using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 8);
                using var bgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.FillPath(bgBrush, bgPath);
                
                // Border
                using var borderPen = new Pen(Color.FromArgb(50, ctx.AccentColor), 1);
                g.DrawPath(borderPen, bgPath);
                
                // Draw date text centered
                using var textBrush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _dateFont, textBrush, ctx.ImageRect, format);
            }

            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                using var subtitleBrush = new SolidBrush(subtitleColor);
                var subtitleFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _locationFont, subtitleBrush, ctx.SubtitleRect, subtitleFormat);
            }
            
            // Draw event category tags
            if (ctx.Tags != null && ctx.Tags.Count > 0 && !ctx.TagsRect.IsEmpty)
            {
                CardRenderingHelpers.DrawChips(g, _owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags, _locationFont);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("DateBlock", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("DateBlock", ctx.ImageRect));
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
