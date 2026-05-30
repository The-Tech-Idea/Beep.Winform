using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Icon-only button - circular icon badge with subtle hover background.
    /// Matches the circular icon button style from reference images.
    /// </summary>
    public class IconButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Subtle background on hover/pressed
            if (context.State == Enums.AdvancedButtonState.Hover ||
                context.State == Enums.AdvancedButtonState.Pressed)
            {
                int alpha = context.State == Enums.AdvancedButtonState.Pressed ? 50 : 25;
                Color bgColor = Color.FromArgb(alpha, context.SolidBackground);
                using (var path = GetShapePath(bounds, context))
                using (var bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            DrawRippleEffect(g, context);

            // Centered icon
            Color iconColor = context.State == Enums.AdvancedButtonState.Disabled
                ? context.DisabledForeground
                : context.SolidBackground;

            if (HasPrimaryIcon(context) && !context.IsLoading)
            {
                DrawIcon(g, context,
                    new Rectangle(bounds.X + (bounds.Width - metrics.IconSize) / 2,
                        bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                        metrics.IconSize, metrics.IconSize),
                    GetPrimaryIconPath(context));
            }
            else if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, bounds, iconColor);
            }

            DrawFocusRingPrimitive(g, context);
        }
    }
}
