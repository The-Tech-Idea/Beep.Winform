using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for flat web-style button variants inspired by classic UI kits.
    /// </summary>
    public class FlatWebButtonPainter : BaseButtonPainter
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

            FlatWebVariant variant = DetermineVariant(context);
            switch (variant)
            {
                case FlatWebVariant.LeftBadgeAction:
                    DrawLeftBadgeAction(g, context, metrics, bounds);
                    break;
                case FlatWebVariant.RightNotchSearch:
                    DrawRightNotchSearch(g, context, metrics, bounds);
                    break;
                case FlatWebVariant.SegmentedIconAction:
                    DrawSegmentedIconAction(g, context, metrics, bounds);
                    break;
                case FlatWebVariant.SearchPillNotch:
                    DrawSearchPillNotch(g, context, metrics, bounds);
                    break;
                case FlatWebVariant.ToolbarSegment:
                    DrawToolbarSegment(g, context, metrics, bounds);
                    break;
                case FlatWebVariant.RightArrowTagSearch:
                    DrawRightArrowTagSearch(g, context, metrics, bounds);
                    break;
                case FlatWebVariant.LeftPointTagSearch:
                    DrawLeftPointTagSearch(g, context, metrics, bounds);
                    break;
                case FlatWebVariant.MagnifierBubbleLeft:
                    DrawMagnifierBubbleLeft(g, context, metrics, bounds);
                    break;
                default:
                    DrawLeftBadgeAction(g, context, metrics, bounds);
                    break;
            }

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }

        private static FlatWebVariant DetermineVariant(AdvancedButtonPaintContext context)
        {
            if (context.FlatWebVariant != FlatWebVariant.Auto)
            {
                return context.FlatWebVariant;
            }

            string text = (context.Text ?? string.Empty).ToUpperInvariant();
            bool hasLeft = !string.IsNullOrEmpty(context.IconLeft) || HasPrimaryIcon(context);
            bool hasRight = !string.IsNullOrEmpty(context.IconRight);

            if (text.Contains("SEARCH"))
            {
                if (text.Contains("POINT") || text.Contains("TAG"))
                {
                    return FlatWebVariant.LeftPointTagSearch;
                }

                if (text.Contains("FIND") || text.Contains("MAG"))
                {
                    return FlatWebVariant.MagnifierBubbleLeft;
                }

                return FlatWebVariant.RightNotchSearch;
            }

            if (context.Shape == ButtonShape.Pill)
            {
                return FlatWebVariant.SearchPillNotch;
            }

            if (hasLeft && hasRight)
            {
                return FlatWebVariant.SegmentedIconAction;
            }

            if (hasRight)
            {
                return FlatWebVariant.RightNotchSearch;
            }

            if (text.Contains("MENU") || text.Contains("SETTINGS") || text.Contains("TOOLBAR"))
            {
                return FlatWebVariant.ToolbarSegment;
            }

            if (text.Contains("SEND") || text.Contains("NEXT") || text.Contains("GO"))
            {
                return FlatWebVariant.RightArrowTagSearch;
            }

            return FlatWebVariant.LeftBadgeAction;
        }

        private void DrawLeftBadgeAction(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int badgeWidth = Math.Max(26, (int)Math.Round(bounds.Height * 0.82f));
            Rectangle badgeBounds = new Rectangle(bounds.X, bounds.Y, badgeWidth, bounds.Height);
            Rectangle bodyBounds = new Rectangle(bounds.X + badgeWidth, bounds.Y, bounds.Width - badgeWidth, bounds.Height);

            Color badgeColor = context.SolidBackground;
            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(236, 238, 241);
            Color bodyText = Color.FromArgb(70, 74, 80);

            using (Brush badgeBrush = new SolidBrush(badgeColor))
            {
                g.FillRectangle(badgeBrush, badgeBounds);
            }

            using (Brush bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            string leftIcon = !string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context);
            if (!string.IsNullOrEmpty(leftIcon))
            {
                Rectangle iconBounds = new Rectangle(
                    badgeBounds.X + (badgeBounds.Width - metrics.IconSize) / 2,
                    badgeBounds.Y + (badgeBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, leftIcon);
            }

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.22f));
            Rectangle textBounds = new Rectangle(bodyBounds.X + inset, bodyBounds.Y, bodyBounds.Width - (inset * 2), bodyBounds.Height);
            DrawTextLine(g, context, textBounds, bodyText, StringAlignment.Near, FontStyle.Regular);

            if (!string.IsNullOrEmpty(context.IconRight))
            {
                Rectangle rightIcon = new Rectangle(
                    bodyBounds.Right - metrics.IconSize - inset,
                    bodyBounds.Y + (bodyBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, rightIcon, context.IconRight);
            }

            using Pen outline = new Pen(Color.FromArgb(28, 0, 0, 0), 1);
            g.DrawRectangle(outline, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
        }

        private void DrawRightNotchSearch(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int notchWidth = Math.Max(28, (int)Math.Round(bounds.Height * 0.88f));
            int notchInset = Math.Max(8, (int)Math.Round(bounds.Height * 0.24f));

            Rectangle bodyBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - notchWidth + notchInset, bounds.Height);
            Rectangle notchBounds = new Rectangle(bodyBounds.Right - notchInset, bounds.Y, notchWidth, bounds.Height);

            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(234, 236, 239);
            Color accent = context.SolidBackground;
            Color textColor = Color.FromArgb(75, 79, 85);

            using (Brush bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            using (GraphicsPath notchPath = new GraphicsPath())
            {
                int angle = Math.Max(10, (int)Math.Round(bounds.Height * 0.30f));
                notchPath.AddPolygon(new[]
                {
                    new Point(notchBounds.X + angle, notchBounds.Y),
                    new Point(notchBounds.Right, notchBounds.Y),
                    new Point(notchBounds.Right, notchBounds.Bottom),
                    new Point(notchBounds.X + angle, notchBounds.Bottom),
                    new Point(notchBounds.X, notchBounds.Y + notchBounds.Height / 2)
                });

                using Brush notchBrush = new SolidBrush(accent);
                g.FillPath(notchBrush, notchPath);
            }

            Rectangle textBounds = new Rectangle(
                bodyBounds.X + notchInset,
                bodyBounds.Y,
                bodyBounds.Width - (notchInset * 2),
                bodyBounds.Height);
            DrawTextLine(g, context, textBounds, textColor, StringAlignment.Near, FontStyle.Regular);

            string rightIcon = !string.IsNullOrEmpty(context.IconRight)
                ? context.IconRight
                : (!string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context));

            if (!string.IsNullOrEmpty(rightIcon))
            {
                Rectangle iconBounds = new Rectangle(
                    notchBounds.X + (notchBounds.Width - metrics.IconSize) / 2 + (int)Math.Round(bounds.Height * 0.08f),
                    notchBounds.Y + (notchBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, rightIcon);
            }
        }

        private void DrawSegmentedIconAction(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int segmentWidth = Math.Max(24, bounds.Width / 3);
            Rectangle left = new Rectangle(bounds.X, bounds.Y, segmentWidth, bounds.Height);
            Rectangle center = new Rectangle(left.Right, bounds.Y, bounds.Width - (segmentWidth * 2), bounds.Height);
            Rectangle right = new Rectangle(center.Right, bounds.Y, bounds.Right - center.Right, bounds.Height);

            Color c1 = context.SolidBackground;
            Color c2 = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(243, 245, 247);
            Color c3 = Color.FromArgb(Math.Max(0, c1.R - 25), Math.Max(0, c1.G - 25), Math.Max(0, c1.B - 25));

            using (Brush b1 = new SolidBrush(c1)) g.FillRectangle(b1, left);
            using (Brush b2 = new SolidBrush(c2)) g.FillRectangle(b2, center);
            using (Brush b3 = new SolidBrush(c3)) g.FillRectangle(b3, right);

            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconL = new Rectangle(left.X + (left.Width - metrics.IconSize) / 2, left.Y + (left.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize);
                DrawIcon(g, context, iconL, context.IconLeft);
            }

            DrawTextLine(g, context, center, Color.FromArgb(70, 74, 80), StringAlignment.Center, FontStyle.Bold);

            if (!string.IsNullOrEmpty(context.IconRight))
            {
                Rectangle iconR = new Rectangle(right.X + (right.Width - metrics.IconSize) / 2, right.Y + (right.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize);
                DrawIcon(g, context, iconR, context.IconRight);
            }
        }

        private void DrawSearchPillNotch(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(240, 242, 244);
            Color accent = context.SolidBackground;
            int radius = bounds.Height / 2;
            int notchWidth = Math.Max(26, (int)Math.Round(bounds.Height * 0.9f));

            using (GraphicsPath pillPath = GetRoundedRectanglePath(bounds, radius))
            using (Brush bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillPath(bodyBrush, pillPath);
            }

            Rectangle notchBounds = new Rectangle(bounds.X, bounds.Y, notchWidth, bounds.Height);
            using (GraphicsPath notchPath = new GraphicsPath())
            {
                int angle = Math.Max(10, (int)Math.Round(bounds.Height * 0.32f));
                notchPath.AddPolygon(new[]
                {
                    new Point(notchBounds.X, notchBounds.Y),
                    new Point(notchBounds.Right - angle, notchBounds.Y),
                    new Point(notchBounds.Right, notchBounds.Y + notchBounds.Height / 2),
                    new Point(notchBounds.Right - angle, notchBounds.Bottom),
                    new Point(notchBounds.X, notchBounds.Bottom)
                });
                using Brush notchBrush = new SolidBrush(accent);
                g.FillPath(notchBrush, notchPath);
            }

            string leftIcon = !string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context);
            if (!string.IsNullOrEmpty(leftIcon))
            {
                Rectangle iconBounds = new Rectangle(
                    notchBounds.X + (notchBounds.Width - metrics.IconSize) / 2,
                    notchBounds.Y + (notchBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, leftIcon);
            }

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.22f));
            Rectangle textBounds = new Rectangle(
                notchBounds.Right + inset,
                bounds.Y,
                bounds.Width - notchBounds.Width - (inset * 2),
                bounds.Height);
            DrawTextLine(g, context, textBounds, Color.FromArgb(76, 80, 87), StringAlignment.Near, FontStyle.Regular);
        }

        private void DrawToolbarSegment(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            Color baseColor = context.SolidBackground;
            Color midColor = Color.FromArgb(Math.Max(0, baseColor.R - 20), Math.Max(0, baseColor.G - 20), Math.Max(0, baseColor.B - 20));
            Color endColor = Color.FromArgb(Math.Max(0, baseColor.R - 35), Math.Max(0, baseColor.G - 35), Math.Max(0, baseColor.B - 35));

            int seg = Math.Max(24, bounds.Width / 4);
            Rectangle left = new Rectangle(bounds.X, bounds.Y, seg, bounds.Height);
            Rectangle middle = new Rectangle(left.Right, bounds.Y, bounds.Width - (seg * 2), bounds.Height);
            Rectangle right = new Rectangle(middle.Right, bounds.Y, bounds.Right - middle.Right, bounds.Height);

            using (Brush b1 = new SolidBrush(baseColor)) g.FillRectangle(b1, left);
            using (Brush b2 = new SolidBrush(midColor)) g.FillRectangle(b2, middle);
            using (Brush b3 = new SolidBrush(endColor)) g.FillRectangle(b3, right);

            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconL = new Rectangle(left.X + (left.Width - metrics.IconSize) / 2, left.Y + (left.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize);
                DrawIcon(g, context, iconL, context.IconLeft);
            }

            DrawTextLine(g, context, middle, Color.White, StringAlignment.Center, FontStyle.Bold);

            string rightIcon = !string.IsNullOrEmpty(context.IconRight) ? context.IconRight : GetPrimaryIconPath(context);
            if (!string.IsNullOrEmpty(rightIcon))
            {
                Rectangle iconR = new Rectangle(right.X + (right.Width - metrics.IconSize) / 2, right.Y + (right.Height - metrics.IconSize) / 2, metrics.IconSize, metrics.IconSize);
                DrawIcon(g, context, iconR, rightIcon);
            }
        }

        private void DrawRightArrowTagSearch(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int tagWidth = Math.Max(28, (int)Math.Round(bounds.Height * 0.94f));
            int tip = Math.Max(10, (int)Math.Round(bounds.Height * 0.30f));
            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.20f));

            Rectangle bodyBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - tagWidth + tip, bounds.Height);
            Rectangle tagBounds = new Rectangle(bodyBounds.Right - tip, bounds.Y, tagWidth, bounds.Height);

            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(236, 238, 241);
            Color accent = context.SolidBackground;
            Color textColor = Color.FromArgb(73, 78, 84);

            using (Brush bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            using (GraphicsPath tagPath = new GraphicsPath())
            {
                tagPath.AddPolygon(new[]
                {
                    new Point(tagBounds.X, tagBounds.Y),
                    new Point(tagBounds.Right - tip, tagBounds.Y),
                    new Point(tagBounds.Right, tagBounds.Y + (tagBounds.Height / 2)),
                    new Point(tagBounds.Right - tip, tagBounds.Bottom),
                    new Point(tagBounds.X, tagBounds.Bottom)
                });
                using Brush tagBrush = new SolidBrush(accent);
                g.FillPath(tagBrush, tagPath);
            }

            string iconPath = !string.IsNullOrEmpty(context.IconRight)
                ? context.IconRight
                : (!string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context));
            if (!string.IsNullOrEmpty(iconPath))
            {
                Rectangle iconBounds = new Rectangle(
                    tagBounds.X + (tagBounds.Width - metrics.IconSize) / 2 - (tip / 4),
                    tagBounds.Y + (tagBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, iconPath);
            }

            Rectangle textBounds = new Rectangle(
                bodyBounds.X + inset,
                bodyBounds.Y,
                bodyBounds.Width - (inset * 2),
                bodyBounds.Height);
            DrawTextLine(g, context, textBounds, textColor, StringAlignment.Near, FontStyle.Regular);
        }

        private void DrawLeftPointTagSearch(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int tagWidth = Math.Max(28, (int)Math.Round(bounds.Height * 0.94f));
            int tip = Math.Max(10, (int)Math.Round(bounds.Height * 0.30f));
            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.20f));

            Rectangle tagBounds = new Rectangle(bounds.X, bounds.Y, tagWidth, bounds.Height);
            Rectangle bodyBounds = new Rectangle(tagBounds.Right - tip, bounds.Y, bounds.Width - tagWidth + tip, bounds.Height);

            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(236, 238, 241);
            Color accent = context.SolidBackground;
            Color textColor = Color.FromArgb(73, 78, 84);

            using (Brush bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            using (GraphicsPath tagPath = new GraphicsPath())
            {
                tagPath.AddPolygon(new[]
                {
                    new Point(tagBounds.X + tip, tagBounds.Y),
                    new Point(tagBounds.Right, tagBounds.Y),
                    new Point(tagBounds.Right, tagBounds.Bottom),
                    new Point(tagBounds.X + tip, tagBounds.Bottom),
                    new Point(tagBounds.X, tagBounds.Y + (tagBounds.Height / 2))
                });
                using Brush tagBrush = new SolidBrush(accent);
                g.FillPath(tagBrush, tagPath);
            }

            string iconPath = !string.IsNullOrEmpty(context.IconLeft) ? context.IconLeft : GetPrimaryIconPath(context);
            if (!string.IsNullOrEmpty(iconPath))
            {
                Rectangle iconBounds = new Rectangle(
                    tagBounds.X + (tagBounds.Width - metrics.IconSize) / 2 + (tip / 5),
                    tagBounds.Y + (tagBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, iconPath);
            }

            Rectangle textBounds = new Rectangle(
                bodyBounds.X + inset,
                bodyBounds.Y,
                bodyBounds.Width - (inset * 2),
                bodyBounds.Height);
            DrawTextLine(g, context, textBounds, textColor, StringAlignment.Near, FontStyle.Regular);
        }

        private void DrawMagnifierBubbleLeft(Graphics g, AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int bubble = Math.Max(26, (int)Math.Round(bounds.Height * 0.96f));
            int overlap = Math.Max(8, (int)Math.Round(bounds.Height * 0.26f));
            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.20f));
            int radius = Math.Max(8, (int)Math.Round(bounds.Height * 0.42f));

            Rectangle bubbleBounds = new Rectangle(bounds.X, bounds.Y + (bounds.Height - bubble) / 2, bubble, bubble);
            Rectangle bodyBounds = new Rectangle(bounds.X + bubble - overlap, bounds.Y, bounds.Width - bubble + overlap, bounds.Height);

            Color accent = context.SolidBackground;
            Color bodyColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.FromArgb(236, 238, 241);
            Color textColor = Color.FromArgb(73, 78, 84);

            using (GraphicsPath bodyPath = GetRoundedRectanglePath(bodyBounds, radius))
            using (Brush bodyBrush = new SolidBrush(bodyColor))
            {
                g.FillPath(bodyBrush, bodyPath);
            }

            using (GraphicsPath bubblePath = new GraphicsPath())
            {
                bubblePath.AddEllipse(bubbleBounds);
                int tip = Math.Max(6, (int)Math.Round(bounds.Height * 0.20f));
                bubblePath.AddPolygon(new[]
                {
                    new Point(bubbleBounds.Right - tip, bubbleBounds.Y + (bubbleBounds.Height / 2) - tip),
                    new Point(bubbleBounds.Right + tip, bubbleBounds.Y + (bubbleBounds.Height / 2)),
                    new Point(bubbleBounds.Right - tip, bubbleBounds.Y + (bubbleBounds.Height / 2) + tip)
                });
                using Brush bubbleBrush = new SolidBrush(accent);
                g.FillPath(bubbleBrush, bubblePath);
            }

            string iconPath = !string.IsNullOrEmpty(context.IconLeft)
                ? context.IconLeft
                : (!string.IsNullOrEmpty(context.IconRight) ? context.IconRight : GetPrimaryIconPath(context));
            if (!string.IsNullOrEmpty(iconPath))
            {
                Rectangle iconBounds = new Rectangle(
                    bubbleBounds.X + (bubbleBounds.Width - metrics.IconSize) / 2,
                    bubbleBounds.Y + (bubbleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize);
                DrawIcon(g, context, iconBounds, iconPath);
            }

            Rectangle textBounds = new Rectangle(
                bodyBounds.X + inset + (overlap / 2),
                bodyBounds.Y,
                bodyBounds.Width - inset - (overlap / 2),
                bodyBounds.Height);
            DrawTextLine(g, context, textBounds, textColor, StringAlignment.Near, FontStyle.Regular);
        }

        private void DrawTextLine(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds, Color color, StringAlignment align, FontStyle style)
        {
            if (string.IsNullOrEmpty(context.Text))
            {
                return;
            }

            using Font textFont = GetDerivedTextFont(context.TextFont, styleOverride: style);
            using Brush textBrush = new SolidBrush(color);
            using StringFormat sf = new StringFormat
            {
                Alignment = align,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
            g.DrawString(context.Text, textFont, textBrush, bounds, sf);
        }
    }
}
