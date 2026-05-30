using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for Accent Edge buttons (Image 1 - Column 1, Middle)
    /// White pill with colored edge strip (top or bottom), icon on right
    /// </summary>
    public class AccentEdgeButtonPainter : BaseButtonPainter
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
                DrawShadow(g, bounds, tokens.BorderRadius, 6, Color.FromArgb(25, 0, 0, 0));
            }

            // White background
            using (GraphicsPath bgPath = ButtonShapeHelper.CreateShapePath(context.Shape, bounds, tokens.BorderRadius))
            {
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(whiteBrush, bgPath);
                }
            }

            // Colored edge strip (bottom)
            Color edgeColor = context.State == Enums.AdvancedButtonState.Disabled 
                ? context.DisabledBackground 
                : context.SolidBackground;
            
            if (context.State == Enums.AdvancedButtonState.Hover)
                edgeColor = context.HoverBackground;
            else if (context.State == Enums.AdvancedButtonState.Pressed)
                edgeColor = context.PressedBackground;

            int edgeHeight = Math.Max(4, bounds.Height / 6);
            Rectangle edgeBounds = new Rectangle(
                bounds.X,
                bounds.Bottom - edgeHeight,
                bounds.Width,
                edgeHeight
            );

            using (GraphicsPath edgePath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, edgeBounds, tokens.BorderRadius / 2))
            {
                using (SolidBrush edgeBrush = new SolidBrush(edgeColor))
                {
                    g.FillPath(edgeBrush, edgePath);
                }
            }

            // Light border
            using (GraphicsPath borderPath = ButtonShapeHelper.CreateShapePath(context.Shape, bounds, tokens.BorderRadius))
            {
                using (Pen borderPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, borderPath);
                }
            }

            // Content: Text + Icon on right
            int iconAreaWidth = metrics.IconSize + metrics.PaddingHorizontal;
            Rectangle textBounds = new Rectangle(
                bounds.X + metrics.PaddingHorizontal,
                bounds.Y,
                bounds.Width - iconAreaWidth - metrics.PaddingHorizontal * 2,
                bounds.Height
            );

            Color textColor = context.State == Enums.AdvancedButtonState.Disabled ? Color.Gray : Color.FromArgb(50, 50, 50);
            DrawText(g, context, textBounds, textColor);

            // Icon on right
            if (HasPrimaryIcon(context) && !context.IsLoading)
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.Right - iconAreaWidth + (iconAreaWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
            else if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, bounds, textColor);
            }

            DrawRippleEffect(g, context);
            DrawFocusRingPrimitive(g, context);
        }
    }
}
