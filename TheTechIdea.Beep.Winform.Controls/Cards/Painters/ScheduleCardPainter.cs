using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ScheduleCard - Schedule/agenda item with time block and details.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ScheduleCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Schedule card fonts
        private Font _timeFont;
        private Font _titleFont;
        private Font _descFont;
        private Font _locationFont;
        private Font _badgeFont;
        
        // Schedule card spacing
        private const int Padding = 14;
        private const int TimeBlockWidth = 70;
        private const int TimeBlockHeight = 60;
        private const int AccentBarWidth = 4;
        private const int TitleHeight = 24;
        private const int DescHeight = 20;
        private const int LocationHeight = 18;
        private const int BadgeWidth = 70;
        private const int BadgeHeight = 20;
        private const int ElementGap = 6;
        private const int ContentGap = 14;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _timeFont?.Dispose(); } catch { }
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _locationFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _timeFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _titleFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _locationFont = new Font(fontFamily, 8.5f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Accent bar on left
            ctx.StatusRect = new Rectangle(
                drawingRect.Left,
                drawingRect.Top,
                AccentBarWidth,
                drawingRect.Height);
            
            // Time block (left side after accent)
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + AccentBarWidth + Padding,
                drawingRect.Top + (drawingRect.Height - TimeBlockHeight) / 2,
                TimeBlockWidth,
                TimeBlockHeight);
            
            int contentLeft = ctx.ImageRect.Right + ContentGap;
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - Padding;
            
            // Status badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
                contentWidth -= BadgeWidth + ElementGap;
            }
            
            // Title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth,
                TitleHeight);
            
            // Description/attendees
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth + (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : BadgeWidth + ElementGap),
                DescHeight);
            
            // Location with icon
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.ParagraphRect.Bottom + ElementGap,
                contentWidth + (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : BadgeWidth + ElementGap),
                LocationHeight);
            
            // Tags (labels, categories)
            if (ctx.Tags != null)
            {
                ctx.TagsRect = new Rectangle(
                    contentLeft,
                    drawingRect.Bottom - Padding - 20,
                    contentWidth,
                    20);
            }
            
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
            // Draw accent bar
            if (!ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.StatusColor != Color.Empty ? ctx.StatusColor : ctx.AccentColor);
                g.FillRectangle(brush, ctx.StatusRect);
            }
            
            // Draw time block
            DrawTimeBlock(g, ctx);
            
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw location with icon
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                DrawLocationInfo(g, ctx);
            }
            
            // Draw tags
            if (ctx.Tags != null && !ctx.TagsRect.IsEmpty)
            {
                CardRenderingHelpers.DrawChips(g, _owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags);
            }
        }
        
        private void DrawTimeBlock(Graphics g, LayoutContext ctx)
        {
            if (ctx.ImageRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw time block background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 8);
            using var bgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
            g.FillPath(bgBrush, bgPath);
            
            // Draw border
            using var borderPen = new Pen(Color.FromArgb(40, ctx.AccentColor), 1);
            g.DrawPath(borderPen, bgPath);
            
            // Draw time text (use StatusText for time range)
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                // Split time into start and end
                string[] timeParts = ctx.StatusText.Split(new[] { '-', '–', '—' }, StringSplitOptions.RemoveEmptyEntries);
                
                using var timeBrush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Center 
                };
                
                if (timeParts.Length >= 2)
                {
                    // Start time
                    var startRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Y + 8, ctx.ImageRect.Width, 20);
                    g.DrawString(timeParts[0].Trim(), _timeFont, timeBrush, startRect, format);
                    
                    // Separator
                    using var sepBrush = new SolidBrush(Color.FromArgb(100, ctx.AccentColor));
                    var sepRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Y + ctx.ImageRect.Height / 2 - 2, ctx.ImageRect.Width, 12);
                    g.DrawString("—", _descFont, sepBrush, sepRect, format);
                    
                    // End time
                    var endRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Bottom - 28, ctx.ImageRect.Width, 20);
                    g.DrawString(timeParts[1].Trim(), _timeFont, timeBrush, endRect, format);
                }
                else
                {
                    // Single time
                    g.DrawString(ctx.StatusText, _timeFont, timeBrush, ctx.ImageRect, format);
                }
            }
        }
        
        private void DrawLocationInfo(Graphics g, LayoutContext ctx)
        {
            // Location pin icon
            int iconSize = 14;
            var iconRect = new Rectangle(ctx.SubtitleRect.Left, ctx.SubtitleRect.Top + 2, iconSize, iconSize);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw pin icon
            using var iconPen = new Pen(Color.FromArgb(120, ctx.AccentColor), 1.5f);
            int cx = iconRect.Left + iconRect.Width / 2;
            int cy = iconRect.Top + iconRect.Height / 2;
            
            // Pin shape
            g.DrawEllipse(iconPen, cx - 4, cy - 5, 8, 8);
            Point[] pinBottom = new Point[]
            {
                new Point(cx - 3, cy + 1),
                new Point(cx, cy + 6),
                new Point(cx + 3, cy + 1)
            };
            g.DrawLines(iconPen, pinBottom);
            
            // Location text
            var textRect = new Rectangle(
                iconRect.Right + 4,
                ctx.SubtitleRect.Top,
                ctx.SubtitleRect.Width - iconSize - 4,
                ctx.SubtitleRect.Height);
            
            using var textBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.SubtitleText, _locationFont, textBrush, textRect, format);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Time block hit area
            if (!ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("TimeBlock", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("TimeBlock", ctx.ImageRect));
            }
            
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Status", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Status", ctx.BadgeRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _timeFont?.Dispose();
            _titleFont?.Dispose();
            _descFont?.Dispose();
            _locationFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

