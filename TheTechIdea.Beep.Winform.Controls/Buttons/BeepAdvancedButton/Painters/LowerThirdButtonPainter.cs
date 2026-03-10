using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for broadcast lower-third variants (headline bars, live tags, ticker strips).
    /// </summary>
    public class LowerThirdButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle bounds = context.Bounds;

            if (context.ShowShadow && context.State != AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, 0, context.ShadowBlur, context.ShadowColor);
            }

            LowerThirdVariant variant = DetermineVariant(context);
            switch (variant)
            {
                case LowerThirdVariant.HeadlineBar:
                    DrawHeadlineBar(g, context, bounds);
                    break;
                case LowerThirdVariant.LiveTagHeadline:
                    DrawLiveTagHeadline(g, context, bounds);
                    break;
                case LowerThirdVariant.ReportSplit:
                    DrawReportSplit(g, context, bounds);
                    break;
                case LowerThirdVariant.TickerStrip:
                    DrawTickerStrip(g, context, bounds);
                    break;
                case LowerThirdVariant.TickerChevron:
                    DrawTickerChevron(g, context, bounds);
                    break;
                case LowerThirdVariant.LocationHeadlineBlock:
                    DrawLocationHeadlineBlock(g, context, bounds);
                    break;
                case LowerThirdVariant.CompactLiveTag:
                    DrawCompactLiveTag(g, context, bounds);
                    break;
                case LowerThirdVariant.ReportStacked:
                    DrawReportStacked(g, context, bounds);
                    break;
                default:
                    DrawHeadlineBar(g, context, bounds);
                    break;
            }

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }

        private static LowerThirdVariant DetermineVariant(AdvancedButtonPaintContext context)
        {
            if (context.LowerThirdVariant != LowerThirdVariant.Auto)
            {
                return context.LowerThirdVariant;
            }

            string text = (context.Text ?? string.Empty).ToUpperInvariant();
            if (text.Contains("CITY LOCATION") || text.Contains("LOCATION HERE"))
            {
                return LowerThirdVariant.LocationHeadlineBlock;
            }

            if ((text.Contains("LIVE") && text.Contains("HEADLINE")) && text.Length < 44)
            {
                return LowerThirdVariant.CompactLiveTag;
            }

            if (text.Contains("RUNNING") || text.Contains(":") || text.Contains("AM") || text.Contains("PM"))
            {
                return LowerThirdVariant.TickerStrip;
            }

            if (text.Contains("LIVE") && text.Contains("REPORT"))
            {
                if (text.Contains("SECOND LINE") || text.Contains("|"))
                {
                    return LowerThirdVariant.ReportStacked;
                }
                return LowerThirdVariant.ReportSplit;
            }

            if (text.Contains("LIVE"))
            {
                return LowerThirdVariant.LiveTagHeadline;
            }

            if (text.Contains(">") || text.Contains(">>"))
            {
                return LowerThirdVariant.TickerChevron;
            }

            return LowerThirdVariant.HeadlineBar;
        }

        private void DrawHeadlineBar(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out string secondary);

            int logoWidth = Math.Max(34, (int)Math.Round(bounds.Height * 1.05f));
            int slashWidth = Math.Max(16, (int)Math.Round(bounds.Height * 0.48f));

            Rectangle logoBounds = new Rectangle(bounds.X, bounds.Y, logoWidth, bounds.Height);
            Rectangle bodyBounds = new Rectangle(logoBounds.Right, bounds.Y, bounds.Width - logoWidth - slashWidth, bounds.Height);
            Rectangle slashBounds = new Rectangle(bodyBounds.Right, bounds.Y, slashWidth, bounds.Height);

            using (Brush logoBrush = new SolidBrush(Color.FromArgb(22, 28, 40)))
            {
                g.FillRectangle(logoBrush, logoBounds);
            }

            using (Brush bodyBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            using (GraphicsPath slash = new GraphicsPath())
            {
                int angle = Math.Max(8, (int)Math.Round(bounds.Height * 0.26f));
                slash.AddPolygon(new[]
                {
                    new Point(slashBounds.X + angle, slashBounds.Y),
                    new Point(slashBounds.Right, slashBounds.Y),
                    new Point(slashBounds.Right - angle, slashBounds.Bottom),
                    new Point(slashBounds.X, slashBounds.Bottom)
                });
                using Brush slashBrush = new SolidBrush(context.SolidBackground);
                g.FillPath(slashBrush, slash);
            }

            DrawLabel(g, context, "LIVE", logoBounds, Color.White, StringAlignment.Center, FontStyle.Bold, 0.80f);

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.18f));
            Rectangle titleBounds = new Rectangle(bodyBounds.X + inset, bodyBounds.Y, bodyBounds.Width - (inset * 2), (int)Math.Round(bounds.Height * 0.58f));
            Rectangle subBounds = new Rectangle(bodyBounds.X + inset, titleBounds.Bottom - 1, bodyBounds.Width - (inset * 2), bounds.Bottom - titleBounds.Bottom);

            DrawLabel(g, context, primary, titleBounds, Color.FromArgb(22, 22, 22), StringAlignment.Near, FontStyle.Bold, 1.00f);
            DrawLabel(g, context, secondary, subBounds, Color.FromArgb(50, 54, 61), StringAlignment.Near, FontStyle.Regular, 0.82f);
        }

        private void DrawLiveTagHeadline(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out string secondary);

            int liveWidth = Math.Max(48, (int)Math.Round(bounds.Height * 1.15f));
            Rectangle liveBounds = new Rectangle(bounds.X, bounds.Y, liveWidth, bounds.Height);
            Rectangle bodyBounds = new Rectangle(liveBounds.Right, bounds.Y, bounds.Width - liveWidth, bounds.Height);

            using (Brush liveBrush = new SolidBrush(context.SolidBackground))
            {
                g.FillRectangle(liveBrush, liveBounds);
            }

            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(25, 28, 34)))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            DrawLabel(g, context, "LIVE", liveBounds, Color.White, StringAlignment.Center, FontStyle.Bold, 0.80f);

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.20f));
            Rectangle primaryBounds = new Rectangle(bodyBounds.X + inset, bodyBounds.Y, bodyBounds.Width - (inset * 2), (int)Math.Round(bounds.Height * 0.60f));
            Rectangle secondaryBounds = new Rectangle(bodyBounds.X + inset, primaryBounds.Bottom - 1, bodyBounds.Width - (inset * 2), bounds.Bottom - primaryBounds.Bottom);

            DrawLabel(g, context, primary, primaryBounds, Color.White, StringAlignment.Near, FontStyle.Bold, 1.00f);
            DrawLabel(g, context, secondary, secondaryBounds, Color.FromArgb(190, 195, 205), StringAlignment.Near, FontStyle.Regular, 0.78f);
        }

        private void DrawReportSplit(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out string secondary);

            int leftWidth = Math.Max(62, (int)Math.Round(bounds.Height * 1.60f));
            int angle = Math.Max(10, (int)Math.Round(bounds.Height * 0.30f));
            Rectangle leftBounds = new Rectangle(bounds.X, bounds.Y, leftWidth, bounds.Height);
            Rectangle rightBounds = new Rectangle(leftBounds.Right - angle, bounds.Y, bounds.Width - leftWidth + angle, bounds.Height);

            using (Brush leftBrush = new SolidBrush(Color.FromArgb(23, 27, 35)))
            {
                g.FillRectangle(leftBrush, leftBounds);
            }

            using (GraphicsPath divider = new GraphicsPath())
            {
                divider.AddPolygon(new[]
                {
                    new Point(leftBounds.Right - angle, leftBounds.Y),
                    new Point(leftBounds.Right, leftBounds.Y),
                    new Point(leftBounds.Right + angle, leftBounds.Bottom),
                    new Point(leftBounds.Right - angle, leftBounds.Bottom)
                });
                using Brush dividerBrush = new SolidBrush(context.SolidBackground);
                g.FillPath(dividerBrush, divider);
            }

            using (Brush rightBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(rightBrush, rightBounds);
            }

            DrawLabel(g, context, "LIVE REPORT", leftBounds, Color.White, StringAlignment.Center, FontStyle.Bold, 0.82f);

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.18f));
            Rectangle titleBounds = new Rectangle(rightBounds.X + inset + angle, rightBounds.Y, rightBounds.Width - (inset * 2) - angle, (int)Math.Round(bounds.Height * 0.62f));
            Rectangle subBounds = new Rectangle(rightBounds.X + inset + angle, titleBounds.Bottom - 1, rightBounds.Width - (inset * 2) - angle, bounds.Bottom - titleBounds.Bottom);
            DrawLabel(g, context, primary, titleBounds, Color.FromArgb(15, 18, 24), StringAlignment.Near, FontStyle.Bold, 1.00f);
            DrawLabel(g, context, secondary, subBounds, Color.FromArgb(72, 76, 84), StringAlignment.Near, FontStyle.Regular, 0.80f);
        }

        private void DrawTickerStrip(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out _);

            int timeWidth = Math.Max(58, (int)Math.Round(bounds.Height * 1.50f));
            Rectangle timeBounds = new Rectangle(bounds.X, bounds.Y, timeWidth, bounds.Height);
            Rectangle bodyBounds = new Rectangle(timeBounds.Right, bounds.Y, bounds.Width - timeWidth, bounds.Height);

            using (Brush timeBrush = new SolidBrush(Color.FromArgb(248, 248, 240)))
            {
                g.FillRectangle(timeBrush, timeBounds);
            }

            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(24, 28, 36)))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            using (Brush capBrush = new SolidBrush(context.SolidBackground))
            {
                g.FillRectangle(capBrush, new Rectangle(bodyBounds.Right - 4, bodyBounds.Y, 4, bodyBounds.Height));
            }

            string timeText = GetTickerTime(primary);
            string tickerText = RemoveTickerTime(primary);

            DrawLabel(g, context, timeText, timeBounds, Color.FromArgb(18, 22, 28), StringAlignment.Center, FontStyle.Bold, 0.84f);

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.18f));
            Rectangle textBounds = new Rectangle(bodyBounds.X + inset, bodyBounds.Y, bodyBounds.Width - (inset * 2), bodyBounds.Height);
            DrawLabel(g, context, tickerText, textBounds, Color.White, StringAlignment.Near, FontStyle.Regular, 0.88f);
        }

        private void DrawTickerChevron(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out _);
            int timeWidth = Math.Max(58, (int)Math.Round(bounds.Height * 1.45f));
            int chev = Math.Max(10, (int)Math.Round(bounds.Height * 0.32f));

            Rectangle timeBounds = new Rectangle(bounds.X, bounds.Y, timeWidth, bounds.Height);
            Rectangle bodyBounds = new Rectangle(timeBounds.Right - chev, bounds.Y, bounds.Width - timeWidth + chev, bounds.Height);

            using (GraphicsPath timePath = new GraphicsPath())
            {
                timePath.AddPolygon(new[]
                {
                    new Point(timeBounds.X, timeBounds.Y),
                    new Point(timeBounds.Right - chev, timeBounds.Y),
                    new Point(timeBounds.Right, timeBounds.Y + timeBounds.Height / 2),
                    new Point(timeBounds.Right - chev, timeBounds.Bottom),
                    new Point(timeBounds.X, timeBounds.Bottom)
                });
                using Brush b = new SolidBrush(Color.FromArgb(250, 250, 245));
                g.FillPath(b, timePath);
            }

            using (GraphicsPath bodyPath = new GraphicsPath())
            {
                bodyPath.AddPolygon(new[]
                {
                    new Point(bodyBounds.X, bodyBounds.Y),
                    new Point(bodyBounds.Right - chev, bodyBounds.Y),
                    new Point(bodyBounds.Right, bodyBounds.Y + bodyBounds.Height / 2),
                    new Point(bodyBounds.Right - chev, bodyBounds.Bottom),
                    new Point(bodyBounds.X, bodyBounds.Bottom)
                });
                using Brush b = new SolidBrush(Color.FromArgb(26, 30, 38));
                g.FillPath(b, bodyPath);
            }

            string timeText = GetTickerTime(primary);
            string tickerText = RemoveTickerTime(primary);
            DrawLabel(g, context, timeText, timeBounds, Color.FromArgb(16, 20, 25), StringAlignment.Center, FontStyle.Bold, 0.82f);

            Rectangle textBounds = new Rectangle(
                bodyBounds.X + Math.Max(8, (int)Math.Round(bounds.Height * 0.18f)),
                bodyBounds.Y,
                bodyBounds.Width - Math.Max(24, (int)Math.Round(bounds.Height * 0.72f)),
                bodyBounds.Height);
            DrawLabel(g, context, tickerText, textBounds, Color.White, StringAlignment.Near, FontStyle.Regular, 0.86f);
        }

        private void DrawLocationHeadlineBlock(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out string secondary);

            int topHeight = Math.Max(10, (int)Math.Round(bounds.Height * 0.18f));
            int locationWidth = Math.Max(68, (int)Math.Round(bounds.Width * 0.26f));
            int slant = Math.Max(10, (int)Math.Round(bounds.Height * 0.34f));

            Rectangle topStrip = new Rectangle(bounds.X, bounds.Y, bounds.Width, topHeight);
            Rectangle locationBounds = new Rectangle(bounds.X, bounds.Y, locationWidth, topHeight);
            Rectangle bodyBounds = new Rectangle(bounds.X, topStrip.Bottom, bounds.Width, bounds.Height - topHeight);

            using (Brush stripBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(stripBrush, topStrip);
            }

            using (Brush locationBrush = new SolidBrush(context.SolidBackground))
            {
                g.FillRectangle(locationBrush, locationBounds);
            }

            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(30, 33, 40)))
            {
                g.FillRectangle(bodyBrush, bodyBounds);
            }

            using (GraphicsPath rightCap = new GraphicsPath())
            {
                rightCap.AddPolygon(new[]
                {
                    new Point(bodyBounds.Right - slant, bodyBounds.Bottom),
                    new Point(bodyBounds.Right, bodyBounds.Bottom),
                    new Point(bodyBounds.Right, bodyBounds.Bottom - slant)
                });
                using Brush capBrush = new SolidBrush(Color.White);
                g.FillPath(capBrush, rightCap);
            }

            DrawLabel(g, context, string.IsNullOrWhiteSpace(primary) ? "CITY LOCATION HERE" : primary, locationBounds, Color.White, StringAlignment.Center, FontStyle.Bold, 0.68f);

            int inset = Math.Max(10, (int)Math.Round(bounds.Height * 0.20f));
            Rectangle headlineBounds = new Rectangle(
                bodyBounds.X + inset,
                bodyBounds.Y,
                bodyBounds.Width - inset - slant,
                bodyBounds.Height);

            string headline = string.IsNullOrWhiteSpace(secondary) || secondary == "SECOND LINE"
                ? "SAMPLE TEXT HERE"
                : secondary;
            DrawLabel(g, context, headline, headlineBounds, Color.White, StringAlignment.Near, FontStyle.Bold, 0.98f);
        }

        private void DrawCompactLiveTag(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out _);

            int liveWidth = Math.Max(34, (int)Math.Round(bounds.Height * 1.00f));
            int slant = Math.Max(8, (int)Math.Round(bounds.Height * 0.30f));

            Rectangle liveBounds = new Rectangle(bounds.X, bounds.Y, liveWidth, bounds.Height);
            Rectangle bodyBounds = new Rectangle(liveBounds.Right - slant, bounds.Y, bounds.Width - liveWidth + slant, bounds.Height);

            using (Brush liveBrush = new SolidBrush(context.SolidBackground))
            {
                g.FillRectangle(liveBrush, liveBounds);
            }

            using (GraphicsPath bodyPath = new GraphicsPath())
            {
                bodyPath.AddPolygon(new[]
                {
                    new Point(bodyBounds.X, bodyBounds.Y),
                    new Point(bodyBounds.Right - slant, bodyBounds.Y),
                    new Point(bodyBounds.Right, bodyBounds.Bottom),
                    new Point(bodyBounds.X - slant, bodyBounds.Bottom)
                });
                using Brush bodyBrush = new SolidBrush(Color.FromArgb(27, 30, 36));
                g.FillPath(bodyBrush, bodyPath);
            }

            DrawLabel(g, context, "LIVE", liveBounds, Color.White, StringAlignment.Center, FontStyle.Bold, 0.74f);

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.22f));
            Rectangle textBounds = new Rectangle(
                bodyBounds.X + inset,
                bodyBounds.Y,
                bodyBounds.Width - inset - slant,
                bodyBounds.Height);
            DrawLabel(g, context, string.IsNullOrWhiteSpace(primary) ? "HEADLINE HERE" : primary, textBounds, Color.White, StringAlignment.Near, FontStyle.Bold, 0.74f);
        }

        private void DrawReportStacked(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds)
        {
            ParsePrimarySecondary(context.Text, out string primary, out string secondary);

            int leftWidth = Math.Max(62, (int)Math.Round(bounds.Height * 1.25f));
            int slant = Math.Max(10, (int)Math.Round(bounds.Height * 0.32f));
            int topTagHeight = Math.Max(9, (int)Math.Round(bounds.Height * 0.18f));
            int sublineHeight = Math.Max(12, (int)Math.Round(bounds.Height * 0.30f));

            Rectangle leftBounds = new Rectangle(bounds.X, bounds.Y, leftWidth, bounds.Height);
            Rectangle rightBounds = new Rectangle(leftBounds.Right - slant, bounds.Y, bounds.Width - leftWidth + slant, bounds.Height);
            Rectangle topTagBounds = new Rectangle(bounds.X, bounds.Y, Math.Max(66, (int)Math.Round(bounds.Width * 0.22f)), topTagHeight);

            using (Brush leftBrush = new SolidBrush(Color.FromArgb(22, 26, 34)))
            {
                g.FillRectangle(leftBrush, leftBounds);
            }

            using (Brush rightBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(rightBrush, rightBounds);
            }

            using (Brush topTagBrush = new SolidBrush(context.SolidBackground))
            {
                g.FillRectangle(topTagBrush, topTagBounds);
            }

            using (GraphicsPath divider = new GraphicsPath())
            {
                divider.AddPolygon(new[]
                {
                    new Point(leftBounds.Right - slant, leftBounds.Y),
                    new Point(leftBounds.Right, leftBounds.Y),
                    new Point(leftBounds.Right + slant, leftBounds.Bottom),
                    new Point(leftBounds.Right - slant, leftBounds.Bottom)
                });
                using Brush dividerBrush = new SolidBrush(context.SolidBackground);
                g.FillPath(dividerBrush, divider);
            }

            int inset = Math.Max(8, (int)Math.Round(bounds.Height * 0.18f));
            Rectangle headlineBounds = new Rectangle(
                rightBounds.X + inset + slant,
                rightBounds.Y,
                rightBounds.Width - (inset * 2) - slant,
                rightBounds.Height - sublineHeight);

            Rectangle sublineBounds = new Rectangle(
                rightBounds.X + inset + slant,
                rightBounds.Bottom - sublineHeight,
                Math.Max(64, (int)Math.Round(rightBounds.Width * 0.60f)),
                sublineHeight);

            using (Brush sublineBrush = new SolidBrush(Color.FromArgb(28, 31, 37)))
            {
                g.FillRectangle(sublineBrush, sublineBounds);
            }

            DrawLabel(g, context, "LIVE REPORT", leftBounds, Color.White, StringAlignment.Center, FontStyle.Bold, 0.76f);
            DrawLabel(g, context, "Location Here", topTagBounds, Color.White, StringAlignment.Center, FontStyle.Bold, 0.58f);
            DrawLabel(g, context, string.IsNullOrWhiteSpace(primary) ? "HEADLINE HERE" : primary, headlineBounds, Color.FromArgb(15, 18, 24), StringAlignment.Near, FontStyle.Bold, 0.90f);
            DrawLabel(g, context, string.IsNullOrWhiteSpace(secondary) ? "SECOND LINE HERE" : secondary, sublineBounds, Color.White, StringAlignment.Near, FontStyle.Regular, 0.70f);
        }

        private static void ParsePrimarySecondary(string text, out string primary, out string secondary)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                primary = "HEADLINE HERE";
                secondary = "SECOND LINE";
                return;
            }

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                primary = parts.Length > 0 ? parts[0].Trim() : "HEADLINE HERE";
                secondary = parts.Length > 1 ? parts[1].Trim() : "SECOND LINE";
                return;
            }

            primary = text.Trim();
            secondary = "SECOND LINE";
        }

        private static string GetTickerTime(string primary)
        {
            string text = primary ?? string.Empty;
            string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && (parts[1].Equals("AM", StringComparison.OrdinalIgnoreCase) || parts[1].Equals("PM", StringComparison.OrdinalIgnoreCase)))
            {
                return parts[0] + " " + parts[1].ToUpperInvariant();
            }

            if (parts.Length > 0 && (parts[0].Contains(":") || parts[0].Contains(".")))
            {
                return parts[0].ToUpperInvariant();
            }

            return "07:15 AM";
        }

        private static string RemoveTickerTime(string primary)
        {
            string text = primary ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                return "Running Text Here";
            }

            string time = GetTickerTime(text);
            int idx = text.IndexOf(time, StringComparison.OrdinalIgnoreCase);
            string cleaned = idx >= 0
                ? (text.Remove(idx, time.Length)).Trim()
                : text.Trim();
            return string.IsNullOrWhiteSpace(cleaned) ? "Running Text Here" : cleaned;
        }

        private static void DrawLabel(
            Graphics g,
            AdvancedButtonPaintContext context,
            string text,
            Rectangle bounds,
            Color color,
            StringAlignment align,
            FontStyle style,
            float sizeScale)
        {
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
