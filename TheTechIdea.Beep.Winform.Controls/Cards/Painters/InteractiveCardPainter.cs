using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// InteractiveCardPainter - For hover cards, download cards, contact cards, and interactive elements
    /// Emphasizes interaction states with visual feedback
    /// </summary>
    internal sealed class InteractiveCardPainter : CardPainterBase
    {
        private Font _badgeFont;
        private Font _metaFont;
        private Font _statsFont;
        private int pad = DefaultPad;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            try { _metaFont?.Dispose(); } catch { }
            try { _statsFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            _metaFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            _statsFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Icon or thumbnail
            if (ctx.ShowImage)
            {
                int imageSize = 48;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, imageSize, imageSize);
            }

            // Content area (to the right of icon)
            int contentLeft = ctx.DrawingRect.Left + pad + (ctx.ShowImage ? 58 : 0);
            int contentWidth = ctx.DrawingRect.Width - pad * 2 - (ctx.ShowImage ? 58 : 0);

            // Title
            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, HeaderHeight);

            // Description or file info
            ctx.ParagraphRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 4, contentWidth, 
                Math.Max(20, ctx.DrawingRect.Height - (ctx.HeaderRect.Bottom - ctx.DrawingRect.Top + 4) - pad * 2 - (ctx.ShowButton ? ButtonHeight + 8 : 0) - (ctx.ShowRating || !string.IsNullOrEmpty(ctx.SubtitleText) ? 24 : 0)));

            // Metadata (file size, type, date, etc.)
            if (!string.IsNullOrEmpty(ctx.SubtitleText) || ctx.ShowRating)
            {
                ctx.SubtitleRect = new Rectangle(contentLeft, ctx.ParagraphRect.Bottom + 6, contentWidth / 2, 16);
            }

            // Status badges (New, Popular, Recommended, etc.)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - pad - 70, ctx.DrawingRect.Top + pad, 65, 20);
            }

            // Primary action button (Download, Contact, View, Open)
            if (ctx.ShowButton)
            {
                int buttonY = ctx.DrawingRect.Bottom - pad - ButtonHeight;
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (contentWidth - pad) / 2;
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonY, buttonWidth, ButtonHeight);
                    ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + pad, buttonY, buttonWidth, ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(contentLeft, buttonY, contentWidth, ButtonHeight);
                }
            }

            // Interaction indicator (hover effect area - full card)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = ctx.DrawingRect; // Full card for hover effects
            }

            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw status/category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw hover state overlay (subtle highlight)
            // Note: This would be controlled by hover state in actual implementation
            if (ctx.ShowStatus && ctx.StatusColor != Color.Empty)
            {
                var hoverBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(8, ctx.AccentColor));
                g.FillRectangle(hoverBrush, ctx.StatusRect);
            }

            // Draw file type icon background (for download cards)
            if (ctx.ShowImage)
            {
                var iconBg = PaintersFactory.GetSolidBrush(Color.FromArgb(30, ctx.AccentColor));
                g.FillRectangle(iconBg, ctx.ImageRect);

                var iconBorder = PaintersFactory.GetPen(Color.FromArgb(60, ctx.AccentColor), 2);
                g.DrawRectangle(iconBorder, ctx.ImageRect);
            }

            // Draw metadata with icon (for contact cards: phone, email, etc.)
            if (!string.IsNullOrEmpty(ctx.SubtitleText))
            {
                var metaBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(140, ctx.AccentColor));
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _metaFont, metaBrush, ctx.SubtitleRect, format);
            }

            // Draw download/interaction count or status text
            if (ctx.ShowRating && !string.IsNullOrEmpty(ctx.StatusText))
            {
                int statsRight = ctx.DrawingRect.Right - pad;
                int statsY = ctx.SubtitleRect.Y;
                var statsBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(110, ctx.AccentColor));
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                var statsRect = new Rectangle(ctx.SubtitleRect.Right, statsY, statsRight - ctx.SubtitleRect.Right, 16);
                g.DrawString(ctx.StatusText, _statsFont, statsBrush, statsRect, format);
            }

            // Draw interactive arrow or chevron indicator
            if (ctx.ShowButton)
            {
                int chevronSize = 16;
                int chevronX = ctx.DrawingRect.Right - pad - chevronSize - 4;
                int chevronY = ctx.DrawingRect.Top + (ctx.DrawingRect.Height - chevronSize) / 2;
                
                var chevronPen = PaintersFactory.GetPen(Color.FromArgb(80, ctx.AccentColor), 2);
                // Draw right-pointing chevron
                g.DrawLine(chevronPen, chevronX, chevronY + 4, chevronX + 6, chevronY + chevronSize / 2);
                g.DrawLine(chevronPen, chevronX + 6, chevronY + chevronSize / 2, chevronX, chevronY + chevronSize - 4);
            }
        }
    }
}
