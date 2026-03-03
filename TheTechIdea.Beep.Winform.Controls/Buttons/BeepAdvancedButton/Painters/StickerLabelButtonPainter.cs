using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for comic/sticker label variants inspired by playful chat and badge UI.
    /// </summary>
    public class StickerLabelButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle bounds = context.Bounds;
            var metrics = GetMetrics(context);

            if (context.ShowShadow && context.State != AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, 0, context.ShadowBlur, context.ShadowColor);
            }

            StickerLabelVariant variant = DetermineVariant(context);
            switch (variant)
            {
                case StickerLabelVariant.CloudTag:
                    DrawCloudTag(g, context, metrics, bounds);
                    break;
                case StickerLabelVariant.BurstBadge:
                    DrawBurstBadge(g, context, metrics, bounds);
                    break;
                case StickerLabelVariant.ComicRibbon:
                    DrawComicRibbon(g, context, metrics, bounds);
                    break;
                case StickerLabelVariant.SpeechBubble:
                default:
                    DrawSpeechBubble(g, context, metrics, bounds);
                    break;
            }

            DrawRippleEffect(g, context);
        }

        private static StickerLabelVariant DetermineVariant(AdvancedButtonPaintContext context)
        {
            if (context.StickerLabelVariant != StickerLabelVariant.Auto)
            {
                return context.StickerLabelVariant;
            }

            string text = (context.Text ?? string.Empty).ToUpperInvariant();
            if (text.Contains("WOW") || text.Contains("!!!") || text.Contains("BOOM"))
            {
                return StickerLabelVariant.BurstBadge;
            }

            if (text.Contains("...") || text.Contains("POINT") || text.Contains("CHAT"))
            {
                return StickerLabelVariant.CloudTag;
            }

            if (text.Contains("GO") || text.Contains("START") || text.Contains("LIVE"))
            {
                return StickerLabelVariant.ComicRibbon;
            }

            return StickerLabelVariant.SpeechBubble;
        }

        private void DrawSpeechBubble(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int tailHeight = Math.Max(6, (int)Math.Round(bounds.Height * 0.20f));
            int radius = Math.Max(8, (int)Math.Round(bounds.Height * 0.35f));
            Rectangle bubbleBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height - tailHeight);
            Rectangle textBounds = bubbleBounds;

            Color accent = GetBackgroundColor(context);
            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(249, 249, 249);
            Color textColor = Color.FromArgb(28, 30, 35);

            using (GraphicsPath bubblePath = GetRoundedRectanglePath(bubbleBounds, radius))
            {
                int tailWidth = Math.Max(12, (int)Math.Round(bounds.Height * 0.36f));
                int tailX = bubbleBounds.X + Math.Max(10, (int)Math.Round(bounds.Height * 0.30f));
                Point[] tail =
                {
                    new Point(tailX, bubbleBounds.Bottom - 1),
                    new Point(tailX + tailWidth, bubbleBounds.Bottom - 1),
                    new Point(tailX + (tailWidth / 3), bounds.Bottom - 1)
                };
                bubblePath.AddPolygon(tail);

                using Brush bodyBrush = new SolidBrush(bodyColor);
                using Pen borderPen = new Pen(accent, Math.Max(1, (int)Math.Round(bounds.Height * 0.06f)));
                g.FillPath(bodyBrush, bubblePath);
                g.DrawPath(borderPen, bubblePath);
            }

            string icon = !string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context);
            if (!string.IsNullOrEmpty(icon))
            {
                int badgeSize = Math.Max(18, (int)Math.Round(bubbleBounds.Height * 0.62f));
                Rectangle badge = new Rectangle(
                    bubbleBounds.X + Math.Max(6, (int)Math.Round(bounds.Height * 0.15f)),
                    bubbleBounds.Y + (bubbleBounds.Height - badgeSize) / 2,
                    badgeSize,
                    badgeSize);

                using Brush accentBrush = new SolidBrush(accent);
                g.FillEllipse(accentBrush, badge);

                Rectangle iconBounds = new Rectangle(
                    badge.X + (badge.Width - metrics.IconSize) / 2,
                    badge.Y + (badge.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, icon);

                textBounds = new Rectangle(
                    badge.Right + Math.Max(8, (int)Math.Round(bounds.Height * 0.18f)),
                    bubbleBounds.Y,
                    bubbleBounds.Right - badge.Right - Math.Max(12, (int)Math.Round(bounds.Height * 0.28f)),
                    bubbleBounds.Height);
            }
            else
            {
                int inset = Math.Max(10, (int)Math.Round(bounds.Height * 0.24f));
                textBounds = new Rectangle(
                    bubbleBounds.X + inset,
                    bubbleBounds.Y,
                    bubbleBounds.Width - (inset * 2),
                    bubbleBounds.Height);
            }

            DrawStickerText(g, context, textBounds, textColor, StringAlignment.Near, FontStyle.Bold, 0.96f);
        }

        private void DrawCloudTag(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            Color accent = GetBackgroundColor(context);
            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(251, 251, 251);
            Color textColor = Color.FromArgb(34, 36, 42);

            int radius = Math.Max(8, (int)Math.Round(bounds.Height * 0.36f));
            using (GraphicsPath bodyPath = GetRoundedRectanglePath(bounds, radius))
            using (Brush bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillPath(bodyBrush, bodyPath);
            }

            int cloudY = bounds.Y + Math.Max(1, (int)Math.Round(bounds.Height * 0.04f));
            int puff = Math.Max(8, (int)Math.Round(bounds.Height * 0.34f));
            int startX = bounds.X + Math.Max(10, (int)Math.Round(bounds.Height * 0.24f));
            for (int i = 0; i < 4; i++)
            {
                Rectangle cloud = new Rectangle(startX + (i * (puff - 3)), cloudY, puff, puff);
                using Brush cloudBrush = new SolidBrush(Color.FromArgb(244, 246, 249));
                g.FillEllipse(cloudBrush, cloud);
            }

            int tagWidth = Math.Max(28, (int)Math.Round(bounds.Height * 0.90f));
            Rectangle tagBounds = new Rectangle(bounds.X, bounds.Y, tagWidth, bounds.Height);
            using (GraphicsPath tagPath = new GraphicsPath())
            {
                int notch = Math.Max(8, (int)Math.Round(bounds.Height * 0.28f));
                tagPath.AddPolygon(new[]
                {
                    new Point(tagBounds.X, tagBounds.Y),
                    new Point(tagBounds.Right - notch, tagBounds.Y),
                    new Point(tagBounds.Right, tagBounds.Y + (tagBounds.Height / 2)),
                    new Point(tagBounds.Right - notch, tagBounds.Bottom),
                    new Point(tagBounds.X, tagBounds.Bottom)
                });

                using Brush tagBrush = new SolidBrush(accent);
                g.FillPath(tagBrush, tagPath);
            }

            string icon = !string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context);
            if (!string.IsNullOrEmpty(icon))
            {
                Rectangle iconBounds = new Rectangle(
                    tagBounds.X + (tagBounds.Width - metrics.IconSize) / 2,
                    tagBounds.Y + (tagBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, icon);
            }

            Rectangle textBounds = new Rectangle(
                tagBounds.Right + Math.Max(8, (int)Math.Round(bounds.Height * 0.20f)),
                bounds.Y,
                bounds.Right - tagBounds.Right - Math.Max(14, (int)Math.Round(bounds.Height * 0.34f)),
                bounds.Height);

            DrawStickerText(g, context, textBounds, textColor, StringAlignment.Near, FontStyle.Bold, 0.96f);
        }

        private void DrawBurstBadge(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            Color accent = GetBackgroundColor(context);
            Color burstColor = Color.FromArgb(
                accent.A,
                Math.Clamp(accent.R + 18, 0, 255),
                Math.Clamp(accent.G + 12, 0, 255),
                Math.Clamp(accent.B + 4, 0, 255));

            Point center = new Point(bounds.X + (bounds.Width / 2), bounds.Y + (bounds.Height / 2));
            int outer = Math.Max(16, (int)Math.Round(Math.Min(bounds.Width, bounds.Height) * 0.50f));
            int inner = Math.Max(10, (int)Math.Round(outer * 0.62f));

            using (GraphicsPath burstPath = BuildBurstPath(center, outer, inner, 12))
            using (Brush burstBrush = new SolidBrush(burstColor))
            using (Pen burstPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
            {
                g.FillPath(burstBrush, burstPath);
                g.DrawPath(burstPen, burstPath);
            }

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.20f));
            Rectangle labelBounds = new Rectangle(
                bounds.X + inset,
                bounds.Y + inset,
                bounds.Width - (inset * 2),
                bounds.Height - (inset * 2));

            using (GraphicsPath labelPath = GetRoundedRectanglePath(labelBounds, Math.Max(8, (int)Math.Round(bounds.Height * 0.28f))))
            using (Brush labelBrush = new SolidBrush(Color.FromArgb(252, 252, 252)))
            using (Pen labelPen = new Pen(Color.FromArgb(36, 0, 0, 0), 1))
            {
                g.FillPath(labelBrush, labelPath);
                g.DrawPath(labelPen, labelPath);
            }

            string rightIcon = !string.IsNullOrEmpty(context.IconRight) ? context.IconRight : context.IconLeft;
            if (string.IsNullOrEmpty(rightIcon))
            {
                rightIcon = GetPrimaryIconPath(context);
            }

            if (!string.IsNullOrEmpty(rightIcon))
            {
                Rectangle iconBounds = new Rectangle(
                    labelBounds.Right - metrics.IconSize - Math.Max(6, (int)Math.Round(bounds.Height * 0.14f)),
                    labelBounds.Y + (labelBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, rightIcon);
                labelBounds.Width -= (metrics.IconSize + Math.Max(8, (int)Math.Round(bounds.Height * 0.20f)));
            }

            DrawStickerText(g, context, labelBounds, Color.FromArgb(28, 30, 35), StringAlignment.Center, FontStyle.Bold, 0.94f);
        }

        private void DrawComicRibbon(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            Color accent = GetBackgroundColor(context);
            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.White;
            Color textColor = Color.FromArgb(27, 29, 34);
            int slant = Math.Max(9, (int)Math.Round(bounds.Height * 0.28f));

            int leftWidth = Math.Max(28, (int)Math.Round(bounds.Height * 1.00f));
            int rightWidth = Math.Max(28, (int)Math.Round(bounds.Height * 0.95f));
            Rectangle left = new Rectangle(bounds.X, bounds.Y, leftWidth, bounds.Height);
            Rectangle body = new Rectangle(left.Right - slant, bounds.Y, bounds.Width - leftWidth - rightWidth + (slant * 2), bounds.Height);
            Rectangle right = new Rectangle(body.Right - slant, bounds.Y, rightWidth, bounds.Height);

            using (GraphicsPath leftPath = new GraphicsPath())
            {
                leftPath.AddPolygon(new[]
                {
                    new Point(left.X, left.Y),
                    new Point(left.Right - slant, left.Y),
                    new Point(left.Right, left.Y + (left.Height / 2)),
                    new Point(left.Right - slant, left.Bottom),
                    new Point(left.X, left.Bottom)
                });
                using Brush leftBrush = new SolidBrush(accent);
                g.FillPath(leftBrush, leftPath);
            }

            using (GraphicsPath bodyPath = new GraphicsPath())
            {
                bodyPath.AddPolygon(new[]
                {
                    new Point(body.X, body.Y),
                    new Point(body.Right, body.Y),
                    new Point(body.Right - slant, body.Bottom),
                    new Point(body.X - slant, body.Bottom)
                });
                using Brush bodyBrush = new SolidBrush(bodyColor);
                g.FillPath(bodyBrush, bodyPath);
            }

            using (GraphicsPath rightPath = new GraphicsPath())
            {
                rightPath.AddPolygon(new[]
                {
                    new Point(right.X + slant, right.Y),
                    new Point(right.Right, right.Y),
                    new Point(right.Right - slant, right.Bottom),
                    new Point(right.X, right.Bottom)
                });
                using Brush rightBrush = new SolidBrush(accent);
                g.FillPath(rightBrush, rightPath);
            }

            using Pen outline = new Pen(Color.FromArgb(28, 0, 0, 0), 1);
            g.DrawRectangle(outline, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

            string leftIcon = !string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context);
            if (!string.IsNullOrEmpty(leftIcon))
            {
                Rectangle iconLeft = new Rectangle(
                    left.X + (left.Width - metrics.IconSize) / 2,
                    left.Y + (left.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconLeft, leftIcon);
            }

            string rightIcon = context.IconRight;
            if (!string.IsNullOrEmpty(rightIcon))
            {
                Rectangle iconRight = new Rectangle(
                    right.X + (right.Width - metrics.IconSize) / 2,
                    right.Y + (right.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconRight, rightIcon);
            }

            Rectangle textBounds = new Rectangle(
                body.X + Math.Max(8, (int)Math.Round(bounds.Height * 0.20f)),
                body.Y,
                body.Width - Math.Max(16, (int)Math.Round(bounds.Height * 0.40f)),
                body.Height);
            DrawStickerText(g, context, textBounds, textColor, StringAlignment.Center, FontStyle.Bold, 0.94f);
        }

        private static GraphicsPath BuildBurstPath(Point center, int outerRadius, int innerRadius, int spikes)
        {
            GraphicsPath path = new GraphicsPath();
            PointF[] points = new PointF[spikes * 2];
            float step = (float)Math.PI / spikes;

            for (int i = 0; i < points.Length; i++)
            {
                float angle = (i * step) - (float)(Math.PI / 2);
                int radius = (i % 2 == 0) ? outerRadius : innerRadius;
                points[i] = new PointF(
                    center.X + (float)(Math.Cos(angle) * radius),
                    center.Y + (float)(Math.Sin(angle) * radius));
            }

            path.AddPolygon(points);
            return path;
        }

        private void DrawStickerText(
            Graphics g,
            AdvancedButtonPaintContext context,
            Rectangle bounds,
            Color color,
            StringAlignment align,
            FontStyle style,
            float sizeScale)
        {
            string text = string.IsNullOrWhiteSpace(context.Text) ? "LABEL" : context.Text;

            using Font font = GetDerivedTextFont(context.TextFont, sizeScale: sizeScale, styleOverride: style);
            using Brush brush = new SolidBrush(color);
            using StringFormat sf = new StringFormat
            {
                Alignment = align,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            g.DrawString(text, font, brush, bounds, sf);
        }
    }
}
