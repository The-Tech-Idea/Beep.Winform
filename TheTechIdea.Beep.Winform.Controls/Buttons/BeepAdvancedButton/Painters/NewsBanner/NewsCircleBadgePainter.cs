using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    public class NewsCircleBadgePainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            string[] parts = SplitText(context.Text);
            string badgeNumber = parts[0];
            string mainText = parts.Length > 1 ? parts[1] : string.Empty;

            Color badgeColor = context.SolidBackground;
            Color bannerColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.White;

            int circleSize = (int)(bounds.Height * 1.3);
            int circleOverlap = circleSize / 4;
            Rectangle circleBounds = new Rectangle(
                bounds.X - circleOverlap,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize, circleSize
            );

            Rectangle bannerBounds = new Rectangle(
                bounds.X + circleSize / 2 - circleOverlap,
                bounds.Y,
                bounds.Width - circleSize / 2 + circleOverlap,
                bounds.Height
            );

            using (GraphicsPath bannerPath = CreateRoundedRect(bannerBounds, metrics.BorderRadius, false, true, false, true))
            using (SolidBrush bannerBrush = new SolidBrush(bannerColor))
            {
                g.FillPath(bannerBrush, bannerPath);
            }

            using (GraphicsPath circlePath = new GraphicsPath())
            {
                circlePath.AddEllipse(circleBounds);
                using (SolidBrush circleBrush = new SolidBrush(badgeColor))
                {
                    g.FillPath(circleBrush, circlePath);
                }
                using (Pen borderPen = new Pen(Color.White, 3))
                {
                    g.DrawEllipse(borderPen, circleBounds);
                }
            }

            if (!string.IsNullOrEmpty(badgeNumber))
            {
                using (Font badgeFont = GetDerivedTextFont(context, styleOverride: System.Drawing.FontStyle.Bold, sizeDelta: 2f))
                using (SolidBrush badgeBrush = new SolidBrush(Color.White))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(badgeNumber, badgeFont, badgeBrush, circleBounds, sf);
                }
            }

            if (!string.IsNullOrEmpty(mainText))
            {
                Rectangle textBounds = new Rectangle(
                    bannerBounds.X + metrics.PaddingHorizontal,
                    bannerBounds.Y,
                    bannerBounds.Width - metrics.PaddingHorizontal * 2,
                    bannerBounds.Height
                );

                Color textColor = bannerColor == Color.White ? badgeColor : Color.White;
                using (Font mainFont = GetDerivedTextFont(context, styleOverride: System.Drawing.FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(textColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
                {
                    g.DrawString(mainText, mainFont, textBrush, textBounds, sf);
                }
            }

            DrawRippleEffect(g, context);
        }

        private GraphicsPath CreateRoundedRect(Rectangle rect, int radius, bool tl, bool tr, bool bl, bool br)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            if (tl) path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            if (tr) path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            if (br) path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            if (bl) path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);
            path.CloseFigure();
            return path;
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text)) return new[] { "24", "BREAKING NEWS" };
            if (text.Contains("|")) return text.Split('|');
            if (text.Contains(" - ")) return text.Split(new[] { " - " }, System.StringSplitOptions.None);
            return new[] { text, string.Empty };
        }
    }
}
