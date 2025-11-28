using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// MessageCenter - Modern message center with envelope icon and message list
    /// </summary>
    internal sealed class MessageCenterPainter : WidgetPainterBase
    {
        private BaseImage.ImagePainter _imagePainter;

        public MessageCenterPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            // Header with icon and title
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 32);

            // Icon area within header
            ctx.IconRect = new Rectangle(ctx.HeaderRect.Left, ctx.HeaderRect.Top + 4, 24, 24);

            // Content area for message list
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Modern clean background with subtle gradient
            using var bgBrush = new LinearGradientBrush(
                ctx.DrawingRect,
                Theme?.BackColor ?? Color.Empty,
                Theme?.PanelBackColor ?? Color.Empty,
                LinearGradientMode.Vertical
            );

            // Rounded corners
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(50, Theme?.BorderColor.R, Theme?.BorderColor.G, Theme?.BorderColor.B), 1); // Light border
            g.DrawPath(borderPen, bgPath);

            // Soft shadow
            DrawSoftShadow(g, ctx.DrawingRect, 10, layers: 4, offset: 2);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw header with icon and title
            if (ctx.ShowHeader)
            {
                // Draw envelope icon
                if (ctx.ShowIcon)
                {
                    DrawEnvelopeIcon(g, ctx.IconRect);
                }

                // Draw title
                if (!string.IsNullOrEmpty(ctx.Title))
                {
                    using var titleFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Bold);
                    using var titleBrush = new SolidBrush(Color.FromArgb(31, 41, 55)); // Dark gray
                    Rectangle titleRect = new Rectangle(ctx.IconRect.Right + 8, ctx.HeaderRect.Y, ctx.HeaderRect.Width - ctx.IconRect.Width - 8, ctx.HeaderRect.Height);
                    g.DrawString(ctx.Title, titleFont, titleBrush, titleRect);
                }
            }

            // Draw message list
            if (ctx.NotificationMessages != null)
            {
                var messages = ctx.NotificationMessages.Cast<Dictionary<string, object>>().ToList();
                DrawMessageList(g, ctx.ContentRect, messages);
            }
            else if (ctx.MessageCount > 0)
            {
                // Show message count if no detailed messages
                int count = ctx.MessageCount;
                DrawMessageCount(g, ctx.ContentRect, count);
            }
        }

        private void DrawEnvelopeIcon(Graphics g, Rectangle rect)
        {
            using var envelopePen = new Pen(Color.FromArgb(59, 130, 246), 2); // Blue

            // Envelope outline
            Point[] envelopePoints = new Point[]
            {
                new Point(rect.X + 2, rect.Y + 8),
                new Point(rect.X + 12, rect.Y + 2),
                new Point(rect.X + 22, rect.Y + 8),
                new Point(rect.X + 22, rect.Y + 20),
                new Point(rect.X + 2, rect.Y + 20),
                new Point(rect.X + 2, rect.Y + 8)
            };

            g.DrawLines(envelopePen, envelopePoints);

            // Envelope flap
            g.DrawLine(envelopePen, rect.X + 2, rect.Y + 8, rect.X + 12, rect.Y + 14);
            g.DrawLine(envelopePen, rect.X + 22, rect.Y + 8, rect.X + 12, rect.Y + 14);
        }

        private void DrawMessageList(Graphics g, Rectangle rect, List<Dictionary<string, object>> messages)
        {
            if (!messages.Any()) return;

            int itemHeight = Math.Min(36, rect.Height / Math.Max(messages.Count, 1));
            using var senderFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var subjectFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var timeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);

            for (int i = 0; i < messages.Count && i < 5; i++) // Show max 5 messages
            {
                var message = messages[i];
                int y = rect.Y + i * itemHeight;

                // Message item background (alternate colors)
                if (i % 2 == 1)
                {
                    using var itemBrush = new SolidBrush(Color.FromArgb(10, Theme?.PanelBackColor ?? Color.Empty));
                    g.FillRectangle(itemBrush, rect.X, y, rect.Width, itemHeight);
                }

                // Sender
                if (message.ContainsKey("Sender"))
                {
                    using var senderBrush = new SolidBrush(Theme?.CardTextForeColor ?? Color.Empty);
                    Rectangle senderRect = new Rectangle(rect.X + 8, y + 4, rect.Width - 80, 14);
                    g.DrawString(message["Sender"].ToString(), senderFont, senderBrush, senderRect);
                }

                // Subject
                if (message.ContainsKey("Subject"))
                {
                    using var subjectBrush = new SolidBrush(Color.FromArgb(100, Theme?.CardTextForeColor ?? Color.Empty));
                    Rectangle subjectRect = new Rectangle(rect.X + 8, y + 18, rect.Width - 80, 12);
                    string subject = message["Subject"].ToString();
                    if (subject.Length > 30) subject = subject.Substring(0, 27) + "...";
                    g.DrawString(subject, subjectFont, subjectBrush, subjectRect);
                }

                // Time
                if (message.ContainsKey("Time"))
                {
                    var timeBase = Theme?.SubLabelForColor ?? Color.Empty;
                    using var timeBrush = new SolidBrush(Color.FromArgb(80, timeBase.R, timeBase.G, timeBase.B));
                    Rectangle timeRect = new Rectangle(rect.Right - 70, y + 4, 70, 12);
                    g.DrawString(message["Time"].ToString(), timeFont, timeBrush, timeRect);
                }

                // Unread indicator
                bool isUnread = message.ContainsKey("IsUnread") && (bool)message["IsUnread"];
                if (isUnread)
                {
                    using var unreadBrush = new SolidBrush(Theme?.AccentColor ?? Color.Empty);
                    g.FillEllipse(unreadBrush, rect.X + rect.Width - 12, y + 6, 6, 6);
                }
            }
        }

        private void DrawMessageCount(Graphics g, Rectangle rect, int count)
        {
            // Draw centered message count
            using var countFont = new Font(Owner.Font.FontFamily, 24f, FontStyle.Bold);
            using var countBrush = new SolidBrush(Theme?.AccentColor ?? Color.Empty);
            using var labelFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Regular);
            var labelBase = Theme?.SubLabelForColor ?? Color.Empty;
            using var labelBrush = new SolidBrush(Color.FromArgb(100, labelBase.R, labelBase.G, labelBase.B));

            var countFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(count.ToString(), countFont, countBrush, rect, countFormat);

            // Label
            Rectangle labelRect = new Rectangle(rect.X, rect.Bottom - 20, rect.Width, 20);
            g.DrawString(count == 1 ? "Message" : "Messages", labelFont, labelBrush, labelRect, countFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Add subtle accent lines or notification badges
        }
    }
}