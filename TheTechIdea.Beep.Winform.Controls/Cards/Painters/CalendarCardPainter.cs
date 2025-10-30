using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// CalendarCardPainter - For calendar events, schedules, and tasks
    /// Emphasizes date/time information with structured layout
    /// </summary>
    internal sealed class CalendarCardPainter : CardPainterBase
    {
        private Font _monthFont;
        private Font _dayFont;
        private Font _badgeFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _monthFont?.Dispose(); } catch { }
            try { _dayFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            _monthFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            _dayFont = new Font(Owner.Font.FontFamily, 18f, FontStyle.Bold);
            _badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Large date block on the left (Material Design inspired)
            if (ctx.ShowImage)
            {
                int dateBlockSize = 60;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, dateBlockSize, dateBlockSize);
            }

            // Content area (to the right of date block)
            int contentLeft = ctx.DrawingRect.Left + pad + (ctx.ShowImage ? 70 : 0);
            int contentWidth = ctx.DrawingRect.Width - pad * 2 - (ctx.ShowImage ? 70 : 0);

            // Event title
            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, HeaderHeight);

            // Time range or duration
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 4, contentWidth, 16);

            // Location or category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + 6, Math.Min(120, contentWidth), 20);
            }

            // Event description
            int descTop = ctx.SubtitleRect.Bottom + (string.IsNullOrEmpty(ctx.BadgeText1) ? 8 : 32);
            ctx.ParagraphRect = new Rectangle(contentLeft, descTop, contentWidth, Math.Max(20, ctx.DrawingRect.Height - (descTop - ctx.DrawingRect.Top) - pad * 2 - (ctx.ShowButton ? ButtonHeight + 8 : 0)));

            // Status indicator bar (top or bottom)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, 4);
            }

            // Action button (RSVP, Join, View Details, Mark Complete)
            if (ctx.ShowButton)
            {
                int buttonY = Math.Max(ctx.DrawingRect.Bottom - pad - ButtonHeight, ctx.ParagraphRect.Bottom + 8);
                ctx.ButtonRect = new Rectangle(contentLeft, buttonY, contentWidth, ButtonHeight);
            }

            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw date block with calendar-Style layout
            if (ctx.ShowImage && !string.IsNullOrEmpty(ctx.SubtitleText))
            {
                // Split date text (expecting format like "MAR\n15" or just "15")
                var dateParts = ctx.SubtitleText.Split('\n', ' ', '/');
                if (dateParts.Length >= 2)
                {
                    // Draw month
                    var monthFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                    var monthBrush = PaintersFactory.GetSolidBrush(ctx.AccentColor);
                    g.DrawString(dateParts[0], _monthFont, monthBrush, new RectangleF(ctx.ImageRect.X, ctx.ImageRect.Y + 8, ctx.ImageRect.Width, 14), monthFormat);

                    // Draw day
                    var dayFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(dateParts[1], _dayFont, monthBrush, new RectangleF(ctx.ImageRect.X, ctx.ImageRect.Y + 20, ctx.ImageRect.Width, 32), dayFormat);
                }
                else
                {
                    // Single value - just draw centered
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    var monthBrush = PaintersFactory.GetSolidBrush(ctx.AccentColor);
                    g.DrawString(ctx.SubtitleText, _dayFont, monthBrush, ctx.ImageRect, format);
                }

                // Draw border around date block
                var borderPen = PaintersFactory.GetPen(ctx.AccentColor, 2f);
                g.DrawRectangle(borderPen, ctx.ImageRect);
            }

            // Draw category or location badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw status accent bar (event status: upcoming, ongoing, completed, cancelled)
            if (ctx.ShowStatus)
            {
                var statusBrush = PaintersFactory.GetSolidBrush(ctx.StatusColor);
                g.FillRectangle(statusBrush, ctx.StatusRect);
            }
        }
    }
}
