using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// CommunicationCardPainter - For notifications, messages, alerts, and announcements
    /// Displays status indicators, timestamps, and action buttons
    /// </summary>
    internal sealed class CommunicationCardPainter : CardPainterBase
    {
        private Font _badgeFont;
        private Font _statusFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            try { _statusFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            _statusFont = new Font(Owner.Font.FontFamily, 7.5f, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Left icon/avatar area
            if (ctx.ShowImage)
            {
                int iconSize = 40;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, iconSize, iconSize);
            }

            // Status indicator bar on left edge
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, 4, ctx.DrawingRect.Height);
            }

            // Content area (to the right of icon)
            int contentLeft = ctx.DrawingRect.Left + pad + (ctx.ShowImage ? 50 : 0);
            int contentWidth = ctx.DrawingRect.Width - pad * 2 - (ctx.ShowImage ? 50 : 0);

            // Header (title/subject)
            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, HeaderHeight);

            // Subtitle (timestamp or sender)
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 2, contentWidth / 2, 14);

            // Badge (notification count, status, priority)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - pad - 60, ctx.DrawingRect.Top + pad, 55, 18);
            }

            // Message body
            ctx.ParagraphRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + 8, contentWidth, Math.Max(20, ctx.DrawingRect.Height - (ctx.SubtitleRect.Bottom - ctx.DrawingRect.Top) - pad * 3 - (ctx.ShowButton ? ButtonHeight + 8 : 0)));

            // Action buttons (dismiss, view, reply, etc.)
            if (ctx.ShowButton)
            {
                int buttonY = Math.Max(ctx.DrawingRect.Bottom - pad - ButtonHeight, ctx.ParagraphRect.Bottom + 8);
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (contentWidth - 8) / 2;
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonY, buttonWidth, ButtonHeight);
                    ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + 8, buttonY, buttonWidth, ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonY, contentWidth, ButtonHeight);
                }
            }

            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw status indicator bar with color coding
            if (ctx.ShowStatus)
            {
                var statusBrush = PaintersFactory.GetSolidBrush(ctx.StatusColor);
                g.FillRectangle(statusBrush, ctx.StatusRect);
            }

            // Draw notification badge (count, priority, or status)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw timestamp or additional status text
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                var statusBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(128, ctx.StatusColor));
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _statusFont, statusBrush, ctx.SubtitleRect, format);
            }
        }
    }
}
