using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for icon-only buttons
    /// </summary>
    public class IconButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var metrics = GetMetrics(context);
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle buttonBounds = context.Bounds;

            // Subtle background on hover/pressed
            if (context.State == Enums.AdvancedButtonState.Hover || 
                context.State == Enums.AdvancedButtonState.Pressed)
            {
                Color bgColor = context.State == Enums.AdvancedButtonState.Pressed
                    ? Color.FromArgb(40, context.SolidBackground)
                    : Color.FromArgb(20, context.SolidBackground);
                    
                using (Brush bgBrush = new SolidBrush(bgColor))
                {
                    FillRoundedRectangle(g, bgBrush, buttonBounds, tokens.BorderRadius);
                }
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Draw icon centered
            if (context.IsLoading)
            {
                Color spinnerColor = context.State == Enums.AdvancedButtonState.Disabled
                    ? context.DisabledForeground
                    : context.SolidBackground;
                DrawLoadingSpinner(g, context, buttonBounds, spinnerColor);
            }
            else if (HasPrimaryIcon(context))
            {
                Rectangle iconBounds = new Rectangle(
                    buttonBounds.X + (buttonBounds.Width - metrics.IconSize) / 2,
                    buttonBounds.Y + (buttonBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );

                Color iconColor = context.State == Enums.AdvancedButtonState.Disabled
                    ? context.DisabledForeground
                    : context.SolidBackground;

                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
            else
            {
                // Fallback: Draw ellipsis icon when no icon provided
                Color iconColor = context.State == Enums.AdvancedButtonState.Disabled
                    ? context.DisabledForeground
                    : context.SolidBackground;

                Rectangle iconBounds = new Rectangle(
                    buttonBounds.X + (buttonBounds.Width - metrics.IconSize) / 2,
                    buttonBounds.Y + (buttonBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );

                DrawFallbackEllipsisIcon(g, iconBounds, iconColor);
            }

            DrawFocusRingPrimitive(g, context);
        }

        /// <summary>
        /// Draw a fallback ellipsis icon (three dots)
        /// </summary>
        private void DrawFallbackEllipsisIcon(Graphics g, Rectangle bounds, Color color)
        {
            int dotSize = Math.Max(3, bounds.Width / 5);
            int gap = dotSize;
            int totalWidth = dotSize * 3 + gap * 2;
            int startX = bounds.X + (bounds.Width - totalWidth) / 2;
            int centerY = bounds.Y + bounds.Height / 2 - dotSize / 2;

            using (Brush brush = new SolidBrush(color))
            {
                // Draw three dots horizontally
                g.FillEllipse(brush, startX, centerY, dotSize, dotSize);
                g.FillEllipse(brush, startX + dotSize + gap, centerY, dotSize, dotSize);
                g.FillEllipse(brush, startX + (dotSize + gap) * 2, centerY, dotSize, dotSize);
            }
        }
    }
}
