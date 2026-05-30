using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Gradient Sub-button (Image 1 - Column 1, DOWNLOAD)
    /// Large gradient pill with a smaller overlapping white button below
    /// </summary>
    public class GradientSubButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle bounds = context.Bounds;

            // Shadow
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, tokens.BorderRadius, 8, Color.FromArgb(30, 0, 0, 0));
            }

            // Main gradient button area (top 70%)
            int mainHeight = (int)(bounds.Height * 0.65);
            Rectangle mainBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                bounds.Width,
                mainHeight
            );

            // Gradient colors
            Color startColor = context.SolidBackground;
            Color endColor = context.HoverBackground;
            if (context.State == Enums.AdvancedButtonState.Disabled)
            {
                startColor = context.DisabledBackground;
                endColor = Color.Gray;
            }

            // Draw main gradient button
            using (GraphicsPath mainPath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, mainBounds, tokens.BorderRadius))
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(mainBounds, startColor, endColor, LinearGradientMode.Horizontal))
            {
                g.FillPath(gradientBrush, mainPath);
            }

            // Main text
            Color mainTextColor = context.State == Enums.AdvancedButtonState.Disabled ? Color.Gray : Color.White;
            Rectangle mainTextBounds = new Rectangle(
                mainBounds.X + metrics.PaddingHorizontal,
                mainBounds.Y,
                mainBounds.Width - metrics.PaddingHorizontal * 2,
                mainBounds.Height
            );
            DrawText(g, context, mainTextBounds, mainTextColor);

            // Sub-button (white overlapping, bottom area)
            int subButtonHeight = (int)(bounds.Height * 0.45);
            int subButtonWidth = (int)(bounds.Width * 0.6);
            Rectangle subBounds = new Rectangle(
                bounds.X + (bounds.Width - subButtonWidth) / 2,
                bounds.Y + mainHeight - subButtonHeight / 2,
                subButtonWidth,
                subButtonHeight
            );

            using (GraphicsPath subPath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, subBounds, tokens.BorderRadius / 2))
            {
                // White background
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(whiteBrush, subPath);
                }

                // Border
                using (Pen borderPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, subPath);
                }
            }

            // Sub-button text (e.g., "CLICK HERE")
            string subText = "CLICK HERE";
            if (!string.IsNullOrEmpty(context.Text) && context.Text.Contains("|"))
            {
                string[] parts = context.Text.Split('|');
                if (parts.Length >= 2)
                    subText = parts[1].Trim();
            }

            Color subTextColor = Color.FromArgb(50, 50, 50);
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                using (Brush textBrush = new SolidBrush(subTextColor))
                using (Font subFont = GetDerivedTextFont(context.TextFont, sizeScale: 0.8f))
                {
                    g.DrawString(subText, subFont, textBrush, subBounds, sf);
                }
            }

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }
    }
}
