using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Gradient buttons with Icon Badges (Image 2 style).
    /// Features a gradient background, white icon circle on the left, dark text, and a chevron.
    /// </summary>
    public class GradientBadgeButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle bounds = context.Bounds;

            // 1. Draw Shadow (Soft, floating effect)
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                // Enhanced shadow for floating look
                DrawShadow(g, bounds, tokens.BorderRadius, 12, Color.FromArgb(40, 0, 0, 0));
            }

            // 2. Draw Gradient Background
            Color startColor = context.SolidBackground;
            Color endColor = context.HoverBackground; // Use hover color as gradient end

            if (context.State == Enums.AdvancedButtonState.Disabled)
            {
                startColor = context.DisabledBackground;
                endColor = Color.Gray;
            }

            using (GraphicsPath bgPath = ButtonShapeHelper.CreateShapePath(context.Shape, bounds, tokens.BorderRadius))
            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Horizontal))
            {
                g.FillPath(gradientBrush, bgPath);
            }

            // 3. Draw Ripple
            DrawRippleEffect(g, context);

            // 4. Draw Content Layout
            // Layout: [White Badge Icon] [Text] [Chevron]
            
            // Badge Area: Left side, circular, white background
            int badgeSize = metrics.IconSize + 8; // Slightly larger than icon
            int badgeMargin = 6;
            Rectangle badgeBounds = new Rectangle(
                bounds.X + badgeMargin,
                bounds.Y + (bounds.Height - badgeSize) / 2,
                badgeSize,
                badgeSize
            );

            // Draw White Badge Circle
            using (GraphicsPath badgePath = new GraphicsPath())
            {
                badgePath.AddEllipse(badgeBounds);
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(whiteBrush, badgePath);
                }
            }

            // Draw Icon inside Badge (Dark/Black)
            if (HasPrimaryIcon(context) && !context.IsLoading)
            {
                Rectangle iconBounds = new Rectangle(
                    badgeBounds.X + (badgeSize - metrics.IconSize) / 2,
                    badgeBounds.Y + (badgeSize - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
            else if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, badgeBounds, Color.DimGray);
            }

            // Text Area: To the right of badge
            int textStartX = badgeBounds.Right + 10;
            Rectangle textBounds = new Rectangle(
                textStartX,
                bounds.Y,
                bounds.Width - (textStartX - bounds.X) - metrics.IconSize, // Leave space for chevron
                bounds.Height
            );

            // Draw Text (Dark/Black)
            Color textColor = context.State == Enums.AdvancedButtonState.Disabled ? Color.Gray : Color.FromArgb(50, 50, 50);
            DrawText(g, context, textBounds, textColor);

            // Chevron Area: Far right
            Rectangle chevronBounds = new Rectangle(
                bounds.Right - metrics.IconSize - badgeMargin,
                bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                metrics.IconSize,
                metrics.IconSize
            );

            // Draw Chevron (White)
            DrawChevron(g, chevronBounds, Color.White);

            // 5. Focus Ring
            DrawFocusRingPrimitive(g, context);
        }

        /// <summary>
        /// Draws a simple right-pointing chevron '>'
        /// </summary>
        private void DrawChevron(Graphics g, Rectangle bounds, Color color)
        {
            using (Pen pen = new Pen(color, 2.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int midY = bounds.Y + bounds.Height / 2;
                int startX = bounds.X + 4;
                int endX = bounds.Right - 4;

                // Draw '>' shape
                g.DrawLine(pen, startX, bounds.Y + 4, endX - 4, midY);
                g.DrawLine(pen, endX - 4, midY, startX, bounds.Bottom - 4);
            }
        }
    }
}
