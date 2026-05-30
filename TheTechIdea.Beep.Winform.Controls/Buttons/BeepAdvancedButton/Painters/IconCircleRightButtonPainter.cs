using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Icon Circle Right buttons (Image 1 - Column 3, SEARCH/BUY NOW)
    /// Colored rounded rect with white circle containing icon on the right side
    /// </summary>
    public class IconCircleRightButtonPainter : BaseButtonPainter
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
                DrawShadow(g, bounds, tokens.BorderRadius, 6, Color.FromArgb(30, 0, 0, 0));
            }

            // Background color
            Color bgColor = context.State == Enums.AdvancedButtonState.Disabled 
                ? context.DisabledBackground 
                : context.SolidBackground;
            if (context.State == Enums.AdvancedButtonState.Hover)
                bgColor = context.HoverBackground;
            else if (context.State == Enums.AdvancedButtonState.Pressed)
                bgColor = context.PressedBackground;

            // Draw main background
            using (GraphicsPath bgPath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, bounds, tokens.BorderRadius))
            {
                using (SolidBrush bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, bgPath);
                }
            }

            // White circle on the right
            int circleSize = Math.Min(bounds.Height - 8, metrics.IconSize + 12);
            int circleMargin = 6;
            Rectangle circleBounds = new Rectangle(
                bounds.Right - circleSize - circleMargin,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            using (GraphicsPath circlePath = new GraphicsPath())
            {
                circlePath.AddEllipse(circleBounds);
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(whiteBrush, circlePath);
                }
            }

            // Icon inside circle (colored)
            if (HasPrimaryIcon(context) && !context.IsLoading)
            {
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - metrics.IconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
            else if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, circleBounds, bgColor);
            }

            // Text on left (white)
            int textRight = circleBounds.X - metrics.PaddingHorizontal;
            Rectangle textBounds = new Rectangle(
                bounds.X + metrics.PaddingHorizontal,
                bounds.Y,
                textRight - bounds.X - metrics.PaddingHorizontal,
                bounds.Height
            );
            DrawText(g, context, textBounds, Color.White);

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }
    }
}
