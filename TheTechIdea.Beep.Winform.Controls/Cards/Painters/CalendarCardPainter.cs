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
    /// CalendarCard - Calendar-style event with large date block, time, and status bar.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class CalendarCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Calendar card fonts
        private Font _monthFont;
        private Font _dayFont;
        private Font _titleFont;
        private Font _timeFont;
        private Font _descFont;
        private Font _badgeFont;
        
        // Calendar card spacing
        private const int Padding = 16;
        private const int DateBlockSize = 64;
        private const int StatusBarHeight = 4;
        private const int TitleHeight = 26;
        private const int TimeHeight = 18;
        private const int BadgeWidth = 120;
        private const int BadgeHeight = 22;
        private const int ButtonHeight = 36;
        private const int ElementGap = 8;
        private const int ContentGap = 16;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _monthFont?.Dispose(); } catch { }
            try { _dayFont?.Dispose(); } catch { }
            try { _titleFont?.Dispose(); } catch { }
            try { _timeFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _monthFont = new Font(fontFamily, 9f, FontStyle.Bold);
            _dayFont = new Font(fontFamily, 22f, FontStyle.Bold);
            _titleFont = new Font(fontFamily, 12f, FontStyle.Bold);
            _timeFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Status indicator bar at top
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Top,
                    drawingRect.Width,
                    StatusBarHeight);
            }
            
            int contentTop = ctx.ShowStatus ? ctx.StatusRect.Bottom + Padding : drawingRect.Top + Padding;
            
            // Large date block on left (Material Design inspired)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    contentTop,
                    DateBlockSize,
                    DateBlockSize);
            }
            
            // Content area to the right of date block
            int contentLeft = ctx.ShowImage ? ctx.ImageRect.Right + ContentGap : drawingRect.Left + Padding;
            int contentWidth = drawingRect.Width - Padding * 2 - (ctx.ShowImage ? DateBlockSize + ContentGap : 0);
            
            // Event title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                contentTop,
                contentWidth,
                TitleHeight);
            
            // Time range or duration
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                TimeHeight);
            
            // Location/category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    contentLeft,
                    ctx.SubtitleRect.Bottom + ElementGap,
                    Math.Min(BadgeWidth, contentWidth),
                    BadgeHeight);
            }
            
            // Event description
            int descTop = ctx.SubtitleRect.Bottom + (string.IsNullOrEmpty(ctx.BadgeText1) ? ElementGap : BadgeHeight + ElementGap * 2);
            int descHeight = Math.Max(30, drawingRect.Height - descTop - Padding - (ctx.ShowButton ? ButtonHeight + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                descTop,
                contentWidth,
                descHeight);
            
            // Action button (RSVP, Join, Mark Complete)
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    contentLeft,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    contentWidth,
                    ButtonHeight);
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
            // Draw status accent bar
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.StatusColor);
                g.FillRectangle(brush, ctx.StatusRect);
            }
            
            // Draw date block with calendar-style layout
            if (ctx.ShowImage && !string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.ImageRect.IsEmpty)
            {
                // Draw date block background
                using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 8);
                using var bgBrush = new SolidBrush(Color.FromArgb(15, ctx.AccentColor));
                g.FillPath(bgBrush, bgPath);
                
                // Draw border
                using var borderPen = new Pen(ctx.AccentColor, 2);
                g.DrawPath(borderPen, bgPath);
                
                // Parse date (expecting format like "MAR 15" or "MAR/15" or "15")
                var dateParts = ctx.SubtitleText.Split(new[] { '\n', ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);
                
                using var textBrush = new SolidBrush(ctx.AccentColor);
                
                if (dateParts.Length >= 2)
                {
                    // Month at top
                    var monthRect = new RectangleF(ctx.ImageRect.X, ctx.ImageRect.Y + 10, ctx.ImageRect.Width, 14);
                    var monthFormat = new StringFormat { Alignment = StringAlignment.Center };
                    g.DrawString(dateParts[0].ToUpper(), _monthFont, textBrush, monthRect, monthFormat);
                    
                    // Day number below
                    var dayRect = new RectangleF(ctx.ImageRect.X, ctx.ImageRect.Y + 24, ctx.ImageRect.Width, 32);
                    var dayFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(dateParts[1], _dayFont, textBrush, dayRect, dayFormat);
                }
                else
                {
                    // Single value - draw centered
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(ctx.SubtitleText, _dayFont, textBrush, ctx.ImageRect, format);
                }
            }
            
            // Draw location/category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("DateBlock", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("DateBlock", ctx.ImageRect));
            }
            
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Location", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Location", ctx.BadgeRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _monthFont?.Dispose();
            _dayFont?.Dispose();
            _titleFont?.Dispose();
            _timeFont?.Dispose();
            _descFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
