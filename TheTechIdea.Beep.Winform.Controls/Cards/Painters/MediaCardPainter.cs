using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// MediaCardPainter - For media-heavy content (images, videos, galleries)
    /// Large media display with minimal text overlay or caption
    /// </summary>
    internal sealed class MediaCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Large media area (image/video) - takes most of the card
            if (ctx.ShowImage)
            {
                int mediaHeight = Math.Max(120, (int)(ctx.DrawingRect.Height * 0.7));
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, mediaHeight);
            }

            // Media type badge (Photo, Video, Gallery, etc.)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - 70, ctx.DrawingRect.Top + 8, 60, 22);
            }

            // Content area below media (caption/description)
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + pad : ctx.DrawingRect.Top + pad;
            int contentHeight = ctx.DrawingRect.Bottom - contentTop - pad;

            // Title/caption
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, contentTop, 
                ctx.DrawingRect.Width - pad * 2, Math.Min(HeaderHeight, contentHeight / 2));

            // Subtitle (date, location, author)
            ctx.SubtitleRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 2, 
                ctx.DrawingRect.Width - pad * 2, 14);

            // Stats (views, likes, downloads)
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.SubtitleRect.Bottom + 4, 
                    ctx.DrawingRect.Width - pad * 2, 16);
            }

            // Play button for video (centered overlay on image)
            if (ctx.ShowButton && !string.IsNullOrEmpty(ctx.BadgeText2) && ctx.BadgeText2.Contains("Video"))
            {
                int playSize = 48;
                ctx.ButtonRect = new Rectangle(
                    ctx.ImageRect.Left + (ctx.ImageRect.Width - playSize) / 2,
                    ctx.ImageRect.Top + (ctx.ImageRect.Height - playSize) / 2,
                    playSize, playSize);
            }
            else
            {
                ctx.ShowButton = false;
            }

            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw media type badge (semi-transparent overlay)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                // Use semi-transparent background for overlay effect
                var overlayBackColor = Color.FromArgb(200, ctx.Badge1BackColor);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, overlayBackColor, ctx.Badge1ForeColor, badgeFont);
            }

            // Draw play button overlay for video
            if (ctx.ShowButton && ctx.ButtonRect != Rectangle.Empty)
            {
                // Semi-transparent circle with play icon
                using var playBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
                using var iconBrush = new SolidBrush(Color.White);
                
                // Draw circle background
                g.FillEllipse(playBrush, ctx.ButtonRect);
                
                // Draw play triangle
                int iconSize = ctx.ButtonRect.Width / 3;
                int iconLeft = ctx.ButtonRect.Left + ctx.ButtonRect.Width / 2 - iconSize / 3;
                int iconTop = ctx.ButtonRect.Top + ctx.ButtonRect.Height / 2 - iconSize / 2;
                
                Point[] playTriangle = new Point[]
                {
                    new Point(iconLeft, iconTop),
                    new Point(iconLeft, iconTop + iconSize),
                    new Point(iconLeft + iconSize, iconTop + iconSize / 2)
                };
                g.FillPolygon(iconBrush, playTriangle);
            }

            // Draw media stats (views, likes)
            if (ctx.ShowRating && !string.IsNullOrEmpty(ctx.StatusText))
            {
                using var statsFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var statsBrush = new SolidBrush(Color.FromArgb(140, ctx.AccentColor));
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, statsFont, statsBrush, ctx.RatingRect, format);
            }

            // Draw subtle gradient overlay at bottom of image for better caption contrast
            if (ctx.ShowImage && ctx.ImageRect.Height > 60)
            {
                var gradientRect = new Rectangle(ctx.ImageRect.Left, ctx.ImageRect.Bottom - 40, 
                    ctx.ImageRect.Width, 40);
                using var gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(gradientRect.Left, gradientRect.Top),
                    new Point(gradientRect.Left, gradientRect.Bottom),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(100, 0, 0, 0));
                g.FillRectangle(gradientBrush, gradientRect);
            }
        }
    }
}
