using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// News Bar style: Colored badge section + white/main text section
    /// Handles: BREAKING NEWS bars, LIVE badges, top banner bars
    /// </summary>
    public class NewsBarPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Parse text: "BREAKING NEWS|description" or "LIVE|WORLD NEWS"
            string[] parts = SplitText(context.Text);
            string badgeText = parts[0];
            string mainText = parts.Length > 1 ? parts[1] : "";

            Color badgeColor = context.SolidBackground;
            Color mainBgColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.White;

            // Calculate badge width based on text
            int badgeWidth = Math.Min(bounds.Width / 3, MeasureContextTextWidth(context, badgeText) + metrics.PaddingHorizontal * 2);
            badgeWidth = Math.Max(badgeWidth, bounds.Width / 4);

            // Draw badge section (left)
            Rectangle badgeBounds = new Rectangle(bounds.X, bounds.Y, badgeWidth, bounds.Height);
            using (GraphicsPath badgePath = TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers.ButtonShapeHelper.CreatePartialRoundedRectangle(badgeBounds, metrics.BorderRadius, true, false, true, false))
            using (SolidBrush badgeBrush = new SolidBrush(badgeColor))
            {
                g.FillPath(badgeBrush, badgePath);
            }

            // Draw main section (right)
            Rectangle mainBounds = new Rectangle(bounds.X + badgeWidth - metrics.BorderRadius, bounds.Y, bounds.Width - badgeWidth + metrics.BorderRadius, bounds.Height);
            using (GraphicsPath mainPath = TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers.ButtonShapeHelper.CreatePartialRoundedRectangle(mainBounds, metrics.BorderRadius, false, true, false, true))
            using (SolidBrush mainBrush = new SolidBrush(mainBgColor))
            {
                g.FillPath(mainBrush, mainPath);
            }

            // Draw badge text
            using (Font badgeFont = GetDerivedTextFont(context, styleOverride: System.Drawing.FontStyle.Bold, sizeScale: 0.85f))
            using (SolidBrush badgeTextBrush = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(badgeText, badgeFont, badgeTextBrush, badgeBounds, sf);
            }

            // Draw main text
            if (!string.IsNullOrEmpty(mainText))
            {
                Rectangle textBounds = new Rectangle(
                    mainBounds.X + metrics.PaddingHorizontal,
                    mainBounds.Y,
                    mainBounds.Width - metrics.PaddingHorizontal * 2,
                    mainBounds.Height
                );

                Color textColor = mainBgColor == Color.White ? Color.Black : Color.White;
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
            if (string.IsNullOrEmpty(text)) return new[] { "BREAKING NEWS", "" };
            if (text.Contains("|")) return text.Split('|');
            if (text.Contains(" - ")) return text.Split(new[] { " - " }, System.StringSplitOptions.None);
            return new[] { text, "" };
        }
    }
}
