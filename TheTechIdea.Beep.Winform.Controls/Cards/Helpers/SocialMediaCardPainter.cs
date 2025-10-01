using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// SocialMediaCard - For social media posts, feed items, announcements
    /// </summary>
    internal sealed class SocialMediaCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = Inset(drawingRect, 6);

            int avatar = 40;
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, avatar, avatar);

            int headerLeft = ctx.ImageRect.Right + 12;
            int headerRight = Math.Max(headerLeft, ctx.DrawingRect.Right - pad - 80 - 8);
            int headerWidth = Math.Max(0, headerRight - headerLeft);
            ctx.HeaderRect = new Rectangle(headerLeft, ctx.DrawingRect.Top + pad, headerWidth, 18);
            ctx.SubtitleRect = new Rectangle(headerLeft, ctx.HeaderRect.Bottom + 2, headerWidth, 14);

            ctx.StatusRect = new Rectangle(ctx.DrawingRect.Right - pad - 80, ctx.DrawingRect.Top + pad, 75, 16);

            int contentTop = ctx.ImageRect.Bottom + 12;
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad, contentTop, ctx.DrawingRect.Width - pad * 2, Math.Max(40, ctx.DrawingRect.Bottom - contentTop - pad - 28));

            int buttonY = ctx.DrawingRect.Bottom - pad - 24;
            ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, buttonY, 60, 20);
            ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + 12, buttonY, 60, 20);

            ctx.TagsRect = new Rectangle(ctx.DrawingRect.Left + pad, Math.Min(ctx.ParagraphRect.Bottom + 8, buttonY - 24), ctx.DrawingRect.Width - pad * 2, 20);

            ctx.ShowSecondaryButton = true;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw timestamp
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                using var timeFont = new Font(Owner.Font.FontFamily, 7.5f, FontStyle.Regular);
                using var timeBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, timeFont, timeBrush, ctx.StatusRect, format);
            }
            
            // Draw hashtags/mentions
            CardRenderingHelpers.DrawChips(g, Owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags);
            
            // Draw engagement icons (like, share, etc.)
            if (ctx.ShowButton)
            {
                // Draw like icon
                using var iconBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                using var iconFont = new Font(Owner.Font.FontFamily, 8f);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("? Like", iconFont, iconBrush, ctx.ButtonRect, format);
                
                if (ctx.ShowSecondaryButton)
                {
                    g.DrawString("? Share", iconFont, iconBrush, ctx.SecondaryButtonRect, format);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Add clickable areas for social interactions
            if (!ctx.ButtonRect.IsEmpty)
                owner.AddHitArea("Like", ctx.ButtonRect, null, () => notifyAreaHit?.Invoke("Like", ctx.ButtonRect));
            
            if (!ctx.SecondaryButtonRect.IsEmpty)
                owner.AddHitArea("Share", ctx.SecondaryButtonRect, null, () => notifyAreaHit?.Invoke("Share", ctx.SecondaryButtonRect));
            
            if (!ctx.ImageRect.IsEmpty)
                owner.AddHitArea("Profile", ctx.ImageRect, null, () => notifyAreaHit?.Invoke("Profile", ctx.ImageRect));
        }
    }
}