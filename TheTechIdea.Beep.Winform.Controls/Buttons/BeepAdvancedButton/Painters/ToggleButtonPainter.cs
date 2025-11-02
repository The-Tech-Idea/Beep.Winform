using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for toggle buttons with on/off states
    /// Supports Split shape with two clickable areas (e.g., "A to Z" | "Z to A")
    /// </summary>
    public class ToggleButtonPainter : BaseButtonPainter
    {
        /// <summary>
        /// Paint the toggle button with appropriate on/off state styling
        /// </summary>
        /// <param name="context">Paint context with button state and styling</param>
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Check if we should draw as split button (two areas)
            if (context.Shape == ButtonShape.Split)
            {
                DrawSplitButton(context);
            }
            else
            {
                DrawRegularToggleButton(context);
            }
        }

        /// <summary>
        /// Draw split button with two clickable areas like in the image
        /// Uses area hover/pressed states from BaseControl's input helper
        /// </summary>
        private void DrawSplitButton(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            Rectangle bounds = context.Bounds;
            int halfWidth = bounds.Width / 2;

            // Left button area
            Rectangle leftArea = new Rectangle(bounds.X, bounds.Y, halfWidth, bounds.Height);

            // Right button area  
            Rectangle rightArea = new Rectangle(bounds.X + halfWidth, bounds.Y, halfWidth, bounds.Height);

            // Determine which side is active based on IsToggled
            // IsToggled = true means LEFT side is active (e.g., "A to Z")
            // IsToggled = false means RIGHT side is active (e.g., "Z to A")
            Color leftBg = context.IsToggled ? context.SolidBackground : GetInactiveSideColor(context);
            Color rightBg = !context.IsToggled ? context.SolidBackground : GetInactiveSideColor(context);

            Color leftFg = context.IsToggled ? context.SolidForeground : context.Theme.ButtonForeColor;
            Color rightFg = !context.IsToggled ? context.SolidForeground : context.Theme.ButtonForeColor;

            // Apply hover effect from BaseControl's input helper area tracking
            if (context.LeftAreaHovered && !context.LeftAreaPressed)
            {
                // Lighten or darken left area on hover
                leftBg = context.IsToggled 
                    ? context.HoverBackground 
                    : Color.FromArgb(230, 232, 235); // Slightly darker inactive hover
            }
            
            if (context.RightAreaHovered && !context.RightAreaPressed)
            {
                // Lighten or darken right area on hover
                rightBg = !context.IsToggled 
                    ? context.HoverBackground 
                    : Color.FromArgb(230, 232, 235); // Slightly darker inactive hover
            }

            // Apply pressed effect from BaseControl's input helper
            if (context.LeftAreaPressed)
            {
                leftBg = context.PressedBackground;
            }
            
            if (context.RightAreaPressed)
            {
                rightBg = context.PressedBackground;
            }

            // Draw left button half with rounded left corners only
            using (GraphicsPath leftPath = ButtonShapeHelper.CreatePartialRoundedRectangle(leftArea, context.BorderRadius,
                   roundTopLeft: true, roundBottomLeft: true, roundTopRight: false, roundBottomRight: false))
            {
                using (SolidBrush brush = new SolidBrush(leftBg))
                {
                    g.FillPath(brush, leftPath);
                }
            }

            // Draw right button half with rounded right corners only
            using (GraphicsPath rightPath = ButtonShapeHelper.CreatePartialRoundedRectangle(rightArea, context.BorderRadius,
                   roundTopLeft: false, roundBottomLeft: false, roundTopRight: true, roundBottomRight: true))
            {
                using (SolidBrush brush = new SolidBrush(rightBg))
                {
                    g.FillPath(brush, rightPath);
                }
            }

            // Draw separator line between the two areas
            using (Pen separatorPen = new Pen(context.BorderColor, 1))
            {
                int x = leftArea.Right;
                g.DrawLine(separatorPen, x, bounds.Y + 4, x, bounds.Bottom - 4);
            }

            // Draw outer border using common shape helper
            using (GraphicsPath outerPath = ButtonShapeHelper.CreateShapePath(ButtonShape.RoundedRectangle, bounds, context.BorderRadius))
            {
                using (Pen borderPen = new Pen(context.BorderColor, context.BorderWidth))
                {
                    g.DrawPath(borderPen, outerPath);
                }
            }

            // Draw text/icons in each area
            DrawSplitContent(g, context, leftArea, rightArea, leftFg, rightFg);
        }

        /// <summary>
        /// Get color for the inactive side of split button
        /// </summary>
        private Color GetInactiveSideColor(AdvancedButtonPaintContext context)
        {
            // Light transparent background for inactive side
            return Color.FromArgb(240, 242, 245); // Very light gray
        }

        /// <summary>
        /// Draw content (text/icons) in both split areas
        /// </summary>
        private void DrawSplitContent(Graphics g, AdvancedButtonPaintContext context,
            Rectangle leftArea, Rectangle rightArea, Color leftColor, Color rightColor)
        {
            // For split buttons, we'll use the main text split by "|" delimiter
            // Or default to "ON" | "OFF" if no delimiter found
            string leftText = "A to Z";
            string rightText = "Z to A";

            if (!string.IsNullOrEmpty(context.Text) && context.Text.Contains("|"))
            {
                string[] parts = context.Text.Split('|');
                if (parts.Length >= 2)
                {
                    leftText = parts[0].Trim();
                    rightText = parts[1].Trim();
                }
            }
            else if (!string.IsNullOrEmpty(context.Text))
            {
                leftText = context.Text;
                rightText = context.Text;
            }

            // Draw left text
            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                using (SolidBrush textBrush = new SolidBrush(leftColor))
                {
                    g.DrawString(leftText, context.Font, textBrush, leftArea, sf);
                }

                using (SolidBrush textBrush = new SolidBrush(rightColor))
                {
                    g.DrawString(rightText, context.Font, textBrush, rightArea, sf);
                }
            }

            // Draw icons if provided
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    leftArea.X + 8,
                    leftArea.Y + (leftArea.Height - 16) / 2,
                    16, 16
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            if (!string.IsNullOrEmpty(context.IconRight))
            {
                Rectangle iconBounds = new Rectangle(
                    rightArea.Right - 24,
                    rightArea.Y + (rightArea.Height - 16) / 2,
                    16, 16
                );
                DrawIcon(g, context, iconBounds, context.IconRight);
            }
        }

        /// <summary>
        /// Draw regular toggle button (single area) when not using Split shape
        /// </summary>
        private void DrawRegularToggleButton(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Determine colors based on toggle state
            Color bgColor;
            Color fgColor;

            if (context.IsToggled)
            {
                // Toggled ON - use solid colors
                bgColor = context.State == AdvancedButtonState.Disabled
                    ? context.DisabledBackground
                    : (context.State == AdvancedButtonState.Hover
                        ? context.HoverBackground
                        : context.SolidBackground);
                fgColor = context.State == AdvancedButtonState.Disabled
                    ? context.DisabledForeground
                    : context.SolidForeground;
            }
            else
            {
                // Toggled OFF - use outlined style
                bgColor = context.State == AdvancedButtonState.Hover
                    ? Color.FromArgb(20, context.BorderColor)
                    : Color.Transparent;
                fgColor = context.State == AdvancedButtonState.Disabled
                    ? context.DisabledForeground
                    : context.BorderColor;
            }

            // Draw background
            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                FillRoundedRectangle(g, bgBrush, buttonBounds, context.BorderRadius);
            }

            // Draw border for OFF state
            if (!context.IsToggled)
            {
                using (Pen borderPen = new Pen(fgColor, context.BorderWidth))
                {
                    Rectangle borderBounds = new Rectangle(
                        buttonBounds.X + context.BorderWidth / 2,
                        buttonBounds.Y + context.BorderWidth / 2,
                        buttonBounds.Width - context.BorderWidth,
                        buttonBounds.Height - context.BorderWidth
                    );
                    DrawRoundedRectangle(g, borderPen, borderBounds, context.BorderRadius);
                }
            }

            // Draw ripple effect
            DrawRippleEffect(g, context);

            // Draw content
            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, buttonBounds, fgColor);
            }
            else
            {
                DrawCenteredContent(g, context, metrics, fgColor);
            }
        }

        private void DrawCenteredContent(Graphics g, AdvancedButtonPaintContext context,
            AdvancedButtonMetrics metrics, Color color)
        {
            Rectangle bounds = context.Bounds;
            bool hasIcon = context.ImagePainter != null && !string.IsNullOrEmpty(context.ImagePainter.ImagePath);
            bool hasText = !string.IsNullOrEmpty(context.Text);

            if (hasIcon && hasText)
            {
                int totalWidth = metrics.IconSize + metrics.IconTextGap + MeasureTextWidth(context);
                int startX = bounds.X + (bounds.Width - totalWidth) / 2;

                Rectangle iconBounds = new Rectangle(
                    startX,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.ImagePainter!.ImagePath);

                Rectangle textBounds = new Rectangle(
                    startX + metrics.IconSize + metrics.IconTextGap,
                    bounds.Y,
                    totalWidth - metrics.IconSize - metrics.IconTextGap,
                    bounds.Height
                );
                DrawText(g, context, textBounds, color);
            }
            else if (hasIcon)
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (bounds.Width - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.ImagePainter!.ImagePath);
            }
            else
            {
                DrawText(g, context, bounds, color);
            }
        }

        private int MeasureTextWidth(AdvancedButtonPaintContext context)
        {
            if (string.IsNullOrEmpty(context.Text)) return 0;
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                return (int)g.MeasureString(context.Text, context.Font).Width;
            }
        }
    }
}
