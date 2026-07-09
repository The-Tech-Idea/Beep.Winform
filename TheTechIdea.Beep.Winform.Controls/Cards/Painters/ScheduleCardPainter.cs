using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_timeFont = captionFont;
            _titleFont = titleFont;
            _descFont = bodyFont;
            _locationFont = captionFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int timeBlockWidth = DpiScalingHelper.ScaleValue(TimeBlockWidth, _owner);
            int timeBlockHeight = DpiScalingHelper.ScaleValue(TimeBlockHeight, _owner);
            int accentBarWidth = DpiScalingHelper.ScaleValue(AccentBarWidth, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descHeight = DpiScalingHelper.ScaleValue(DescHeight, _owner);
            int locationHeight = DpiScalingHelper.ScaleValue(LocationHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int contentGap = DpiScalingHelper.ScaleValue(ContentGap, _owner);
            
            // Accent bar on left
            ctx.StatusRect = new Rectangle(
                drawingRect.Left,
                drawingRect.Top,
                accentBarWidth,
                drawingRect.Height);
            
            // Time block (left side after accent)
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + accentBarWidth + padding,
                drawingRect.Top + (drawingRect.Height - timeBlockHeight) / 2,
                timeBlockWidth,
                timeBlockHeight);
            
            int contentLeft = ctx.ImageRect.Right + contentGap;
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - padding;
            
            // Status badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
                contentWidth -= badgeWidth + elementGap;
            }
            
            // Title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + padding,
                contentWidth,
                titleHeight);
            
            // Description/attendees
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + elementGap / 2,
                contentWidth + (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : badgeWidth + elementGap),
                descHeight);
            
            // Location with icon
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.ParagraphRect.Bottom + elementGap,
                contentWidth + (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : badgeWidth + elementGap),
                locationHeight);
            
            // Tags (labels, categories)
            if (ctx.Tags != null)
            {
                ctx.TagsRect = new Rectangle(
                    contentLeft,
                    drawingRect.Bottom - padding - DpiScalingHelper.ScaleValue(20, _owner),
                    contentWidth,
                    DpiScalingHelper.ScaleValue(20, _owner));
            }
            
                    ctx.ShowImage = false;
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
                g.FillRectangle(CardPaintCache.Brush(ctx.StatusColor != Color.Empty ? ctx.StatusColor : ctx.AccentColor), ctx.StatusRect);
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
                CardRenderingHelpers.DrawChips(g, _owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags, _locationFont);
            }
        }
        
        private void DrawTimeBlock(Graphics g, LayoutContext ctx)
        {
            if (ctx.ImageRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw time block background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, DpiScalingHelper.ScaleValue(8, _owner));
            g.FillPath(CardPaintCache.Brush(Color.FromArgb(20, ctx.AccentColor)), bgPath);

            // Draw border
            g.DrawPath(CardPaintCache.Pen(Color.FromArgb(40, ctx.AccentColor), DpiScalingHelper.ScaleValue(1, _owner)), bgPath);

            // Draw time text (use StatusText for time range)
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                // Split time into start and end
                string[] timeParts = ctx.StatusText.Split(new[] { '-', '–', '—' }, StringSplitOptions.RemoveEmptyEntries);

                if (timeParts.Length >= 2)
                {
                    // Start time
                    var startRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Y + DpiScalingHelper.ScaleValue(8, _owner), ctx.ImageRect.Width, DpiScalingHelper.ScaleValue(20, _owner));
                    TextRenderer.DrawText(g, timeParts[0].Trim(), _timeFont, startRect, ctx.AccentColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

                    // Separator
                    var sepRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Y + ctx.ImageRect.Height / 2 - DpiScalingHelper.ScaleValue(2, _owner), ctx.ImageRect.Width, DpiScalingHelper.ScaleValue(12, _owner));
                    TextRenderer.DrawText(g, "—", _descFont, sepRect, Color.FromArgb(100, ctx.AccentColor),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

                    // End time
                    var endRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Bottom - DpiScalingHelper.ScaleValue(28, _owner), ctx.ImageRect.Width, DpiScalingHelper.ScaleValue(20, _owner));
                    TextRenderer.DrawText(g, timeParts[1].Trim(), _timeFont, endRect, ctx.AccentColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
                else
                {
                    // Single time
                    TextRenderer.DrawText(g, ctx.StatusText, _timeFont, ctx.ImageRect, ctx.AccentColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }
        
        private void DrawLocationInfo(Graphics g, LayoutContext ctx)
        {
            // Location pin icon
            int iconSize = DpiScalingHelper.ScaleValue(14, _owner);
            var iconRect = new Rectangle(ctx.SubtitleRect.Left, ctx.SubtitleRect.Top + DpiScalingHelper.ScaleValue(2, _owner), iconSize, iconSize);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw pin icon
            var iconPen = CardPaintCache.Pen(Color.FromArgb(120, ctx.AccentColor), 1.5f);
            int cx = iconRect.Left + iconRect.Width / 2;
            int cy = iconRect.Top + iconRect.Height / 2;

            // Pin shape
            g.DrawEllipse(iconPen, cx - DpiScalingHelper.ScaleValue(4, _owner), cy - DpiScalingHelper.ScaleValue(5, _owner), DpiScalingHelper.ScaleValue(8, _owner), DpiScalingHelper.ScaleValue(8, _owner));
            Point[] pinBottom = new Point[]
            {
                new Point(cx - DpiScalingHelper.ScaleValue(3, _owner), cy + DpiScalingHelper.ScaleValue(1, _owner)),
                new Point(cx, cy + DpiScalingHelper.ScaleValue(6, _owner)),
                new Point(cx + DpiScalingHelper.ScaleValue(3, _owner), cy + DpiScalingHelper.ScaleValue(1, _owner))
            };
            g.DrawLines(iconPen, pinBottom);

            // Location text
            var textRect = new Rectangle(
                iconRect.Right + DpiScalingHelper.ScaleValue(4, _owner),
                ctx.SubtitleRect.Top,
                ctx.SubtitleRect.Width - iconSize - DpiScalingHelper.ScaleValue(4, _owner),
                ctx.SubtitleRect.Height);

            TextRenderer.DrawText(g, ctx.SubtitleText, _locationFont, textRect, Color.FromArgb(120, _theme?.CardTextForeColor ?? Color.Black),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
_disposed = true;
        }
        
        #endregion
    }
}

