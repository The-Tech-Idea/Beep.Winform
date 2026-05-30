using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Outlined button - thick colored border, white interior, optional chevron.
    /// Matches the "READ MORE / SUBSCRIBE" outline pill style from reference images.
    /// </summary>
    public class OutlinedButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Shadow
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, bounds, context.BorderRadius, 4, Color.FromArgb(25, 0, 0, 0));
            }

            // Border color
            Color borderColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.SolidBackground;
            if (context.State == Enums.AdvancedButtonState.Hover) borderColor = context.HoverBackground;
            if (context.State == Enums.AdvancedButtonState.Pressed) borderColor = context.PressedBackground;

            int borderWidth = Math.Max(3, context.BorderWidth);

            // White interior
            using (var path = GetShapePath(bounds, context))
            using (var whiteBrush = new SolidBrush(Color.White))
            {
                g.FillPath(whiteBrush, path);
            }

            // Thick colored border
            using (var path = GetShapePath(bounds, context))
            using (var borderPen = new Pen(borderColor, borderWidth))
            {
                borderPen.Alignment = PenAlignment.Inset;
                g.DrawPath(borderPen, path);
            }

            DrawRippleEffect(g, context);

            // Content with chevron
            int chevronSpace = metrics.IconSize + 8;
            Rectangle contentBounds = new Rectangle(
                bounds.X + metrics.PaddingHorizontal + borderWidth,
                bounds.Y,
                bounds.Width - (metrics.PaddingHorizontal + borderWidth) * 2 - chevronSpace,
                bounds.Height);

            Color textColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground : Color.FromArgb(50, 50, 50);

            if (!context.IsLoading && !string.IsNullOrEmpty(context.Text))
            {
                DrawText(g, context, contentBounds, textColor);
            }
            else if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, bounds, borderColor);
            }

            // Chevron on right
            Rectangle chevronBounds = new Rectangle(
                bounds.Right - metrics.PaddingHorizontal - borderWidth - metrics.IconSize,
                bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                metrics.IconSize, metrics.IconSize);
            if (!context.IsLoading) DrawChevron(g, chevronBounds, borderColor);

            DrawFocusRingPrimitive(g, context);
        }

        private void DrawChevron(Graphics g, Rectangle bounds, Color color)
        {
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                int midY = bounds.Y + bounds.Height / 2;
                g.DrawLine(pen, bounds.X + 4, bounds.Y + 4, bounds.Right - 6, midY);
                g.DrawLine(pen, bounds.Right - 6, midY, bounds.X + 4, bounds.Bottom - 4);
            }
        }
    }
}
