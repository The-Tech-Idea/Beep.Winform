using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for FAB (Floating Action Button) - circular, elevated
    /// </summary>
    public class FABButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var metrics = GetMetrics(context);
            var tokens = AdvancedButtonPaintContract.CreateTokens(context);
            Rectangle buttonBounds = context.Bounds;

            // Make it circular
            int size = Math.Min(buttonBounds.Width, buttonBounds.Height);
            Rectangle circleBounds = new Rectangle(
                buttonBounds.X + (buttonBounds.Width - size) / 2,
                buttonBounds.Y + (buttonBounds.Height - size) / 2,
                size,
                size
            );

            // Draw shadow (elevated)
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawCircleShadow(g, circleBounds, context.ShadowBlur * 2, context.ShadowColor);
            }

            // Draw background circle
            Color bgColor = GetBackgroundColor(context);
            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                g.FillEllipse(bgBrush, circleBounds);
            }

            // Draw ripple effect (circular clip)
            if (context.RippleActive)
            {
                using (GraphicsPath clipPath = new GraphicsPath())
                {
                    clipPath.AddEllipse(circleBounds);
                    g.SetClip(clipPath);
                    DrawRippleEffect(g, context);
                    g.ResetClip();
                }
            }

            // Draw icon
            Color iconColor = GetForegroundColor(context);
            
            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, circleBounds, iconColor);
            }
            else if (HasPrimaryIcon(context))
            {
                int iconSize = Math.Max(tokens.IconSize, size / 2);
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - iconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - iconSize) / 2,
                    iconSize,
                    iconSize
                );
                DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
            }
            else
            {
                // Fallback: Draw "+" icon when no icon provided (standard FAB behavior)
                DrawFallbackPlusIcon(g, circleBounds, iconColor);
            }

            DrawFocusRingPrimitive(g, context);
        }

        private void DrawCircleShadow(Graphics g, Rectangle bounds, int shadowBlur, Color shadowColor)
        {
            for (int i = shadowBlur; i > 0; i--)
            {
                int alpha = (int)((shadowColor.A / (float)shadowBlur) * (shadowBlur - i + 1));
                Color layerColor = Color.FromArgb(alpha, shadowColor);

                Rectangle shadowBounds = new Rectangle(
                    bounds.X - i / 2,
                    bounds.Y + i / 4,
                    bounds.Width + i,
                    bounds.Height + i
                );

                using (Brush brush = new SolidBrush(layerColor))
                {
                    g.FillEllipse(brush, shadowBounds);
                }
            }
        }

        /// <summary>
        /// Draw a fallback plus icon for FAB buttons without icons
        /// </summary>
        private void DrawFallbackPlusIcon(Graphics g, Rectangle bounds, Color color)
        {
            int thickness = Math.Max(3, bounds.Width / 10);
            int size = bounds.Width / 2;
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;

            using (Pen pen = new Pen(color, thickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Horizontal line
                g.DrawLine(pen, centerX - size / 2, centerY, centerX + size / 2, centerY);
                // Vertical line
                g.DrawLine(pen, centerX, centerY - size / 2, centerX, centerY + size / 2);
            }
        }
    }
}
