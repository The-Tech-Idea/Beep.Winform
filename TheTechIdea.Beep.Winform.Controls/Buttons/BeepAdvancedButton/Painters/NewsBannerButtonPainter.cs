using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for news/broadcast banner buttons with badges and labels
    /// Creates TV-style breaking news, live badges, and broadcast banners
    /// Common uses: News tickers, live badges, broadcast labels, notification banners
    /// </summary>
    public class NewsBannerButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Determine banner style from context
            NewsBannerStyle style = DetermineBannerStyle(context);

            // Draw shadow
            if (context.ShowShadow && context.State != AdvancedButtonState.Disabled)
            {
                DrawBannerShadow(g, buttonBounds, style);
            }

            // Draw banner based on style
            switch (style)
            {
                case NewsBannerStyle.CircleBadgeLeft:
                    DrawCircleBadgeLeft(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.RectangleBadgeLeft:
                    DrawRectangleBadgeLeft(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.AngledBadgeLeft:
                    DrawAngledBadgeLeft(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.ChevronRight:
                    DrawChevronRight(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.ChevronBoth:
                    DrawChevronBoth(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.FlagLeft:
                    DrawFlagLeft(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.AngledTwoTone:
                    DrawAngledTwoTone(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.SlantedEdges:
                    DrawSlantedEdges(g, context, metrics, buttonBounds);
                    break;

                case NewsBannerStyle.PillWithIcon:
                    DrawPillWithIcon(g, context, metrics, buttonBounds);
                    break;

                default:
                    DrawRectangleBadgeLeft(g, context, metrics, buttonBounds);
                    break;
            }
        }

        /// <summary>
        /// Determine banner style from context properties
        /// </summary>
        private NewsBannerStyle DetermineBannerStyle(AdvancedButtonPaintContext context)
        {
            if (context.NewsBannerStyle != null)
            {
                return context.NewsBannerStyle;
            }

            // Auto-detect based on text or properties
            string text = context.Text?.ToUpper() ?? "";
            
            if (text.Contains("24") || text.Contains("HOUR"))
                return NewsBannerStyle.CircleBadgeLeft;
            
            if (text.Contains("LIVE") && text.Contains("NEWS"))
                return NewsBannerStyle.AngledTwoTone;
            
            if (text.Contains("BREAKING") && text.Contains("NEWS"))
                return NewsBannerStyle.ChevronRight;
            
            if (text.Contains("SPORT"))
                return NewsBannerStyle.RectangleBadgeLeft;
            
            if (text.Contains("WORLD"))
                return NewsBannerStyle.ChevronBoth;
            
            if (text.Contains("FOOTBALL") || text.Contains("SOCCER"))
                return NewsBannerStyle.PillWithIcon;

            return NewsBannerStyle.RectangleBadgeLeft;
        }

        #region "Banner Style Renderers"

        /// <summary>
        /// Circle badge on left (24 NEWS style)
        /// </summary>
        private void DrawCircleBadgeLeft(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int circleSize = (int)(bounds.Height * 1.3);
            int circleOverlap = (int)(circleSize * 0.15);

            // Draw circle badge (left)
            Rectangle circleBounds = new Rectangle(
                bounds.X - circleOverlap,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            Color badgeColor = context.IconBackgroundColor != Color.Empty 
                ? context.IconBackgroundColor 
                : DarkenColor(context.SolidBackground, 30);

            using (Brush circleBrush = new SolidBrush(badgeColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Circle border
            using (Pen circlePen = new Pen(Color.White, 3))
            {
                g.DrawEllipse(circlePen, circleBounds);
            }

            // Draw main banner section
            Rectangle mainBounds = new Rectangle(
                bounds.X + circleSize - circleOverlap - 10,
                bounds.Y,
                bounds.Width - circleSize + circleOverlap + 10,
                bounds.Height
            );

            using (Brush mainBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw badge text (24, etc)
            string badgeText = ExtractBadgeText(context.Text);
            if (!string.IsNullOrEmpty(badgeText))
            {
                using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size + 2, FontStyle.Bold))
                using (Brush badgeTextBrush = new SolidBrush(Color.White))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(badgeText, badgeFont, badgeTextBrush, circleBounds, format);
                }
            }

            // Draw main text
            string mainText = RemoveBadgeText(context.Text);
            Rectangle textBounds = new Rectangle(
                mainBounds.X + 10,
                mainBounds.Y,
                mainBounds.Width - 20,
                mainBounds.Height
            );
            DrawText(g, context, textBounds, context.SolidBackground, mainText);
        }

        /// <summary>
        /// Rectangle badge on left (BREAKING NEWS style)
        /// </summary>
        private void DrawRectangleBadgeLeft(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            string[] parts = SplitBannerText(context.Text);
            int badgeWidth = MeasureTextWidth(g, parts[0], context.Font) + 20;

            // Draw badge section
            Rectangle badgeBounds = new Rectangle(bounds.X, bounds.Y, badgeWidth, bounds.Height);
            
            Color badgeColor = context.IconBackgroundColor != Color.Empty 
                ? context.IconBackgroundColor 
                : context.SolidBackground;

            using (Brush badgeBrush = new SolidBrush(badgeColor))
            {
                g.FillRectangle(badgeBrush, badgeBounds);
            }

            // Draw main section
            Rectangle mainBounds = new Rectangle(
                bounds.X + badgeWidth,
                bounds.Y,
                bounds.Width - badgeWidth,
                bounds.Height
            );

            Color mainColor = context.SecondaryColor != null
                ? context.SecondaryColor
                : Color.White;

            using (Brush mainBrush = new SolidBrush(mainColor))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw badge text
            using (Brush badgeTextBrush = new SolidBrush(Color.White))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(parts[0], context.Font, badgeTextBrush, badgeBounds, format);
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + 10,
                mainBounds.Y,
                mainBounds.Width - 20,
                mainBounds.Height
            );
            Color textColor = mainColor == Color.White ? Color.Black : Color.White;
            DrawText(g, context, textBounds, textColor, parts[1]);
        }

        /// <summary>
        /// Angled/slanted badge on left (yellow lightning style)
        /// </summary>
        private void DrawAngledBadgeLeft(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int badgeWidth = bounds.Height;
            int angle = 15;

            // Draw angled badge
            using (GraphicsPath badgePath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + badgeWidth, bounds.Y),
                    new Point(bounds.X + badgeWidth + angle, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                badgePath.AddPolygon(points);

                Color badgeColor = context.SolidBackground;
                using (Brush badgeBrush = new SolidBrush(badgeColor))
                {
                    g.FillPath(badgeBrush, badgePath);
                }
            }

            // Draw main section
            Rectangle mainBounds = new Rectangle(
                bounds.X + badgeWidth + angle,
                bounds.Y,
                bounds.Width - badgeWidth - angle,
                bounds.Height
            );

            using (Brush mainBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw icon in badge (if provided)
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (badgeWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + 10,
                mainBounds.Y,
                mainBounds.Width - 20,
                mainBounds.Height
            );
            DrawText(g, context, textBounds, context.SolidBackground);
        }

        /// <summary>
        /// Right chevron/arrow pointing (BREAKING NEWS with arrow)
        /// </summary>
        private void DrawChevronRight(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int chevronWidth = 30;

            using (GraphicsPath mainPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.Right - chevronWidth, bounds.Y),
                    new Point(bounds.Right, bounds.Y + bounds.Height / 2),
                    new Point(bounds.Right - chevronWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                mainPath.AddPolygon(points);

                using (Brush mainBrush = new SolidBrush(context.SolidBackground))
                {
                    g.FillPath(mainBrush, mainPath);
                }
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                bounds.X + 15,
                bounds.Y,
                bounds.Width - chevronWidth - 25,
                bounds.Height
            );
            DrawText(g, context, textBounds, Color.White);
        }

        /// <summary>
        /// Chevrons on both sides (WORLD NEWS hexagon style)
        /// </summary>
        private void DrawChevronBoth(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int chevronWidth = 25;

            using (GraphicsPath mainPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X + chevronWidth, bounds.Y),
                    new Point(bounds.Right - chevronWidth, bounds.Y),
                    new Point(bounds.Right, bounds.Y + bounds.Height / 2),
                    new Point(bounds.Right - chevronWidth, bounds.Bottom),
                    new Point(bounds.X + chevronWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Y + bounds.Height / 2)
                };
                mainPath.AddPolygon(points);

                using (Brush mainBrush = new SolidBrush(context.SolidBackground))
                {
                    g.FillPath(mainBrush, mainPath);
                }
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                bounds.X + chevronWidth + 10,
                bounds.Y,
                bounds.Width - (chevronWidth * 2) - 20,
                bounds.Height
            );
            DrawText(g, context, textBounds, Color.White);
        }

        /// <summary>
        /// Flag shape on left (angled tab)
        /// </summary>
        private void DrawFlagLeft(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int flagWidth = (int)(bounds.Height * 1.2);
            int flagPoint = 20;

            // Draw flag section
            using (GraphicsPath flagPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + flagWidth, bounds.Y),
                    new Point(bounds.X + flagWidth - flagPoint, bounds.Y + bounds.Height / 2),
                    new Point(bounds.X + flagWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                flagPath.AddPolygon(points);

                using (Brush flagBrush = new SolidBrush(context.SolidBackground))
                {
                    g.FillPath(flagBrush, flagPath);
                }
            }

            // Draw main section
            Rectangle mainBounds = new Rectangle(
                bounds.X + flagWidth - flagPoint,
                bounds.Y,
                bounds.Width - flagWidth + flagPoint,
                bounds.Height
            );

            using (Brush mainBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Split and draw text
            string[] parts = SplitBannerText(context.Text);
            
            // Flag text
            Rectangle flagTextBounds = new Rectangle(
                bounds.X + 10,
                bounds.Y,
                flagWidth - flagPoint - 10,
                bounds.Height
            );
            using (Brush flagTextBrush = new SolidBrush(Color.White))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(parts[0], context.Font, flagTextBrush, flagTextBounds, format);
            }

            // Main text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + 10,
                mainBounds.Y,
                mainBounds.Width - 20,
                mainBounds.Height
            );
            DrawText(g, context, textBounds, context.SolidBackground, parts[1]);
        }

        /// <summary>
        /// Two-tone angled design (LIVE + BREAKING NEWS style)
        /// </summary>
        private void DrawAngledTwoTone(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            string[] parts = SplitBannerText(context.Text);
            int section1Width = MeasureTextWidth(g, parts[0], context.Font) + 30;
            int angleWidth = 30;

            // Section 1 (LIVE/MORNING/etc)
            using (GraphicsPath section1Path = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + section1Width, bounds.Y),
                    new Point(bounds.X + section1Width + angleWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                section1Path.AddPolygon(points);

                Color section1Color = context.IconBackgroundColor != Color.Empty
                    ? context.IconBackgroundColor
                    : DarkenColor(context.SolidBackground, 20);

                using (Brush section1Brush = new SolidBrush(section1Color))
                {
                    g.FillPath(section1Brush, section1Path);
                }
            }

            // Section 2 (BREAKING NEWS/etc)
            Rectangle section2Bounds = new Rectangle(
                bounds.X + section1Width + angleWidth - 5,
                bounds.Y,
                bounds.Width - section1Width - angleWidth + 5,
                bounds.Height
            );

            using (Brush section2Brush = new SolidBrush(context.SolidBackground))
            {
                g.FillRectangle(section2Brush, section2Bounds);
            }

            // Draw section 1 text
            Rectangle section1TextBounds = new Rectangle(
                bounds.X + 10,
                bounds.Y,
                section1Width - 10,
                bounds.Height
            );
            using (Brush text1Brush = new SolidBrush(Color.White))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(parts[0], context.Font, text1Brush, section1TextBounds, format);
            }

            // Draw section 2 text
            Rectangle section2TextBounds = new Rectangle(
                section2Bounds.X + 10,
                section2Bounds.Y,
                section2Bounds.Width - 20,
                section2Bounds.Height
            );
            DrawText(g, context, section2TextBounds, Color.White, parts[1]);
        }

        /// <summary>
        /// Slanted edges on both sides
        /// </summary>
        private void DrawSlantedEdges(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int slant = 20;

            using (GraphicsPath mainPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X + slant, bounds.Y),
                    new Point(bounds.Right - slant, bounds.Y),
                    new Point(bounds.Right, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                mainPath.AddPolygon(points);

                using (Brush mainBrush = new SolidBrush(context.SolidBackground))
                {
                    g.FillPath(mainBrush, mainPath);
                }
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                bounds.X + slant + 10,
                bounds.Y,
                bounds.Width - (slant * 2) - 20,
                bounds.Height
            );
            DrawText(g, context, textBounds, Color.White);
        }

        /// <summary>
        /// Pill shape with circular icon badge (FOOTBALL NEWS style)
        /// </summary>
        private void DrawPillWithIcon(Graphics g, AdvancedButtonPaintContext context, 
            AdvancedButtonMetrics metrics, Rectangle bounds)
        {
            int iconSize = (int)(bounds.Height * 1.2);
            int iconOverlap = (int)(iconSize * 0.2);

            // Draw main pill
            using (GraphicsPath pillPath = CreatePillPath(bounds))
            {
                using (Brush pillBrush = new SolidBrush(context.SolidBackground))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw icon circle badge
            Rectangle iconCircle = new Rectangle(
                bounds.X - iconOverlap,
                bounds.Y + (bounds.Height - iconSize) / 2,
                iconSize,
                iconSize
            );

            Color iconBgColor = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : Color.White;

            using (Brush iconBrush = new SolidBrush(iconBgColor))
            {
                g.FillEllipse(iconBrush, iconCircle);
            }

            // Icon border
            using (Pen iconPen = new Pen(context.SolidBackground, 3))
            {
                g.DrawEllipse(iconPen, iconCircle);
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    iconCircle.X + (iconCircle.Width - metrics.IconSize) / 2,
                    iconCircle.Y + (iconCircle.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                bounds.X + iconSize - iconOverlap + 10,
                bounds.Y,
                bounds.Width - iconSize + iconOverlap - 20,
                bounds.Height
            );
            DrawText(g, context, textBounds, Color.White);
        }

        #endregion

        #region "Helper Methods"

        private void DrawBannerShadow(Graphics g, Rectangle bounds, NewsBannerStyle style)
        {
            Rectangle shadowBounds = new Rectangle(
                bounds.X + 2,
                bounds.Y + 3,
                bounds.Width,
                bounds.Height
            );

            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                g.FillRectangle(shadowBrush, shadowBounds);
            }
        }

        private string[] SplitBannerText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "", "" };

            // Split on common delimiters
            if (text.Contains("|"))
                return text.Split('|');
            
            if (text.Contains(" - "))
                return text.Split(new[] { " - " }, StringSplitOptions.None);

            // Auto-split on known patterns
            string[] keywords = { "BREAKING", "LIVE", "NEWS", "SPECIAL", "WORLD", "SPORT", "24" };
            foreach (string keyword in keywords)
            {
                if (text.Contains(keyword))
                {
                    int index = text.IndexOf(keyword);
                    if (index > 0)
                        return new[] { text.Substring(0, index).Trim(), text.Substring(index).Trim() };
                    else if (text.Length > keyword.Length)
                        return new[] { keyword, text.Substring(keyword.Length).Trim() };
                }
            }

            return new[] { text, "" };
        }

        private string ExtractBadgeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Extract numbers or short keywords
            if (text.Contains("24"))
                return "24";
            
            string[] parts = text.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0 && parts[0].Length <= 3)
                return parts[0];

            return "";
        }

        private string RemoveBadgeText(string text)
        {
            string badge = ExtractBadgeText(text);
            if (string.IsNullOrEmpty(badge))
                return text;

            return text.Replace(badge, "").Trim();
        }

        private int MeasureTextWidth(Graphics g, string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            SizeF size = g.MeasureString(text, font);
            return (int)size.Width;
        }

        private void DrawText(Graphics g, AdvancedButtonPaintContext context, Rectangle bounds, Color color, string overrideText = null)
        {
            string text = overrideText ?? context.Text;
            if (string.IsNullOrEmpty(text))
                return;

            using (Brush textBrush = new SolidBrush(color))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;

                g.DrawString(text, context.Font, textBrush, bounds, format);
            }
        }

        private GraphicsPath CreatePillPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int radius = bounds.Height / 2;
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 90, 180);
            path.AddLine(bounds.X + radius, bounds.Y, bounds.Right - radius, bounds.Y);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 180);
            path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.X + radius, bounds.Bottom);
            path.CloseFigure();

            return path;
        }

        private Color DarkenColor(Color color, int percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, color.R - (color.R * percent / 100)),
                Math.Max(0, color.G - (color.G * percent / 100)),
                Math.Max(0, color.B - (color.B * percent / 100))
            );
        }

        #endregion
    }

    /// <summary>
    /// News banner layout styles
    /// </summary>
    public enum NewsBannerStyle
    {
        /// <summary>Circle badge on left with 24, clock, etc</summary>
        CircleBadgeLeft,

        /// <summary>Rectangle badge section on left</summary>
        RectangleBadgeLeft,

        /// <summary>Angled/slanted badge on left</summary>
        AngledBadgeLeft,

        /// <summary>Right side chevron/arrow point</summary>
        ChevronRight,

        /// <summary>Chevrons on both left and right (hexagon)</summary>
        ChevronBoth,

        /// <summary>Flag shape on left</summary>
        FlagLeft,

        /// <summary>Two-tone sections with angled divider</summary>
        AngledTwoTone,

        /// <summary>Slanted edges on both sides</summary>
        SlantedEdges,

        /// <summary>Pill shape with circular icon badge</summary>
        PillWithIcon
    }
}
