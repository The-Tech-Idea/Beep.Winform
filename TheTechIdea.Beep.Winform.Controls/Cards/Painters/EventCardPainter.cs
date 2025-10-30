using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// EventCard - For displaying events, appointments, or time-based content
    /// </summary>
    internal sealed class EventCardPainter : CardPainterBase
    {
        private Font _dateFont;
        private Font _badgeFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _dateFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            _dateFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            _badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Accent bar on left
            ctx.StatusRect = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, 4, ctx.DrawingRect.Height);

            // Date block at top-left after accent
            ctx.ImageRect = new Rectangle(ctx.StatusRect.Right + 8, ctx.DrawingRect.Top + pad, 60, 50);

            int contentLeft = ctx.ImageRect.Right + 12;
            int contentWidth = Math.Max(0, ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - pad);

            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, 20);
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 4, contentWidth, 16);
            ctx.ParagraphRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + 6, contentWidth, Math.Max(20, ctx.DrawingRect.Bottom - (ctx.SubtitleRect.Bottom + 6) - pad - ButtonHeight));
            ctx.TagsRect = new Rectangle(contentLeft, ctx.ParagraphRect.Bottom + 8, Math.Max(0, contentWidth - 100), 20);

            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Right - pad - 85, ctx.DrawingRect.Bottom - pad - 28, 80, 24);
            }

            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw accent bar
            var accentBrush = PaintersFactory.GetSolidBrush(ctx.AccentColor);
            using (var accentPath = CreateRoundedPath(ctx.StatusRect, 2))
            {
                g.FillPath(accentBrush, accentPath);
            }

            // Draw date/time block
            if (!string.IsNullOrEmpty(ctx.StatusText)) // Date in StatusText
            {
                // Background for date block
                var dateBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, Color.Black));
                using (var datePath = CreateRoundedPath(ctx.ImageRect, 6))
                {
                    g.FillPath(dateBrush, datePath);
                }

                // Draw date text
                var textBrush = PaintersFactory.GetSolidBrush(Color.White);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _dateFont, textBrush, ctx.ImageRect, format);
            }

            // Draw event category tags
            CardRenderingHelpers.DrawChips(g, Owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags);
        }
    }
}
