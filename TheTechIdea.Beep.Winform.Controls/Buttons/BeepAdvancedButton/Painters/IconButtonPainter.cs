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
                    FillRoundedRectangle(g, bgBrush, buttonBounds, context.BorderRadius);
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
                DrawLoadingSpinner(g, buttonBounds, spinnerColor);
            }
            else if (!string.IsNullOrEmpty(context.ImagePainter?.ImagePath))
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

                DrawIcon(g, context, iconBounds, context.ImagePainter.ImagePath);
            }
        }
    }
}
