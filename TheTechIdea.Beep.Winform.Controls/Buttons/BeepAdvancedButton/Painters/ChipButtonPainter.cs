using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for chip/tag style buttons (pills with hashtags, removable tags, badges)
    /// Used for: hashtag chips, category tags, filter chips, badges
    /// </summary>
    public class ChipButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Determine chip style based on context
            bool isRemovable = context.ShowCloseIcon;
            bool isBadge = context.IsBadge;

            // Draw shadow for elevated chips
            if (context.ShowShadow && context.State != Enums.AdvancedButtonState.Disabled)
            {
                DrawShadow(g, buttonBounds, buttonBounds.Height / 2, context.ShadowBlur, context.ShadowColor);
            }

            // Draw chip background (always pill-shaped)
            Color bgColor = GetChipBackgroundColor(context);
            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                ButtonShapeHelper.FillShape(g, Enums.ButtonShape.Pill, buttonBounds, buttonBounds.Height / 2, bgBrush);
            }

            // Draw border for outlined chips
            if (context.ShowBorder || context.IsOutlined)
            {
                Color borderColor = GetBorderColor(context);
                using (Pen borderPen = new Pen(borderColor, context.BorderThickness))
                {
                    ButtonShapeHelper.DrawShape(g, Enums.ButtonShape.Pill, buttonBounds, buttonBounds.Height / 2, borderPen);
                }
            }

            // Draw ripple effect on hover/press
            DrawRippleEffect(g, context);

            // Calculate content layout
            CalculateChipLayout(context, metrics, isRemovable, out Rectangle iconBounds, 
                out Rectangle textBounds, out Rectangle closeBounds);

            // Draw loading spinner or content
            if (context.IsLoading)
            {
                DrawLoadingSpinner(g, context, buttonBounds, GetForegroundColor(context));
            }
            else
            {
                // Draw left icon (if present) or fallback hashtag icon
                Color textColor = GetForegroundColor(context);
                if (HasPrimaryIcon(context) && !iconBounds.IsEmpty)
                {
                    DrawIcon(g, context, iconBounds, GetPrimaryIconPath(context));
                }
                else if (!iconBounds.IsEmpty && IsHashtagText(context.Text))
                {
                    // Fallback: Draw "#" icon for hashtag-style chips
                    DrawFallbackHashtagIcon(g, iconBounds, textColor);
                }

                // Draw text (hashtag, label, count)
                DrawChipText(g, context, textBounds, textColor);

                // Draw close/remove icon (X button on right)
                if (isRemovable && !closeBounds.IsEmpty)
                {
                    DrawCloseIcon(g, closeBounds, textColor, context.IsHovered);
                }

                // Draw badge indicator (if badge style)
                if (isBadge && !string.IsNullOrEmpty(context.BadgeText))
                {
                    DrawBadgeIndicator(g, buttonBounds, context.BadgeText, context.BadgeColor, context.TextFont);
                }
            }

            DrawFocusRingPrimitive(g, context);
        }

        /// <summary>
        /// Get chip background color based on state
        /// </summary>
        private Color GetChipBackgroundColor(AdvancedButtonPaintContext context)
        {
            if (context.State == Enums.AdvancedButtonState.Disabled)
            {
                return Color.FromArgb(229, 231, 235); // Gray-200
            }

            if (context.IsPressed)
            {
                return DarkenColor(context.SolidBackground, 30);
            }

            if (context.IsHovered)
            {
                return DarkenColor(context.SolidBackground, 15);
            }

            if (context.IsSelected || context.IsToggled)
            {
                return context.SolidBackground;
            }

            // Default chip background
            return context.IsOutlined ? Color.Transparent : context.SolidBackground;
        }

        /// <summary>
        /// Calculate layout for chip content (icon, text, close button)
        /// </summary>
        private void CalculateChipLayout(AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics,
            bool isRemovable, out Rectangle iconBounds, out Rectangle textBounds, out Rectangle closeBounds)
        {
            Rectangle bounds = context.Bounds;
            int padding = metrics.PaddingHorizontal;
            int closeIconSize = 16;
            int gap = 6;

            bool hasIcon = HasPrimaryIcon(context);
            
            int currentX = bounds.X + padding;

            // Left icon bounds
            if (hasIcon)
            {
                iconBounds = new Rectangle(
                    currentX,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                currentX += metrics.IconSize + gap;
            }
            else
            {
                iconBounds = Rectangle.Empty;
            }

            // Close icon bounds (if removable)
            int textWidth = bounds.Width - (currentX - bounds.X) - padding;
            if (isRemovable)
            {
                closeBounds = new Rectangle(
                    bounds.Right - padding - closeIconSize,
                    bounds.Y + (bounds.Height - closeIconSize) / 2,
                    closeIconSize,
                    closeIconSize
                );
                textWidth -= (closeIconSize + gap);
            }
            else
            {
                closeBounds = Rectangle.Empty;
            }

            // Text bounds (remaining space)
            textBounds = new Rectangle(
                currentX,
                bounds.Y,
                textWidth,
                bounds.Height
            );
        }

        /// <summary>
        /// Draw chip text (supports hashtags and regular labels)
        /// </summary>
        private void DrawChipText(Graphics g, AdvancedButtonPaintContext context, Rectangle textBounds, Color textColor)
        {
            if (string.IsNullOrEmpty(context.Text) || textBounds.IsEmpty) return;

            var safeFont = context.TextFont ?? FontManagement.BeepFontManager.DefaultFont;
            using (Brush textBrush = new SolidBrush(textColor))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;

                g.DrawString(context.Text, safeFont, textBrush, textBounds, format);
            }
        }

        /// <summary>
        /// Draw close/remove icon (X button)
        /// </summary>
        private void DrawCloseIcon(Graphics g, Rectangle closeBounds, Color iconColor, bool isHovered)
        {
            // Draw circle background on hover
            if (isHovered)
            {
                using (Brush hoverBrush = new SolidBrush(Color.FromArgb(50, iconColor)))
                {
                    g.FillEllipse(hoverBrush, closeBounds);
                }
            }

            // Draw X
            using (Pen pen = new Pen(iconColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int padding = 4;
                g.DrawLine(pen, 
                    closeBounds.X + padding, 
                    closeBounds.Y + padding,
                    closeBounds.Right - padding, 
                    closeBounds.Bottom - padding);
                g.DrawLine(pen, 
                    closeBounds.Right - padding, 
                    closeBounds.Y + padding,
                    closeBounds.X + padding, 
                    closeBounds.Bottom - padding);
            }
        }

        /// <summary>
        /// Draw badge indicator (small circle with number/dot)
        /// </summary>
        private void DrawBadgeIndicator(Graphics g, Rectangle buttonBounds, string badgeText, Color badgeColor, Font baseFont)
        {
            int badgeSize = 20;
            Rectangle badgeBounds = new Rectangle(
                buttonBounds.Right - badgeSize / 2,
                buttonBounds.Y - badgeSize / 2,
                badgeSize,
                badgeSize
            );

            // Draw badge circle
            using (Brush badgeBrush = new SolidBrush(badgeColor))
            {
                g.FillEllipse(badgeBrush, badgeBounds);
            }

            // Draw badge border
            using (Pen borderPen = new Pen(Color.White, 2))
            {
                g.DrawEllipse(borderPen, badgeBounds);
            }

            // Draw badge text/count
            if (!string.IsNullOrEmpty(badgeText))
            {
                using (Brush textBrush = new SolidBrush(Color.White))
                using (StringFormat format = new StringFormat())
                using (Font badgeFont = GetDerivedTextFont(baseFont, styleOverride: FontStyle.Bold, sizeDelta: 8f - baseFont.Size))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString(badgeText, badgeFont, textBrush, badgeBounds, format);
                }
            }
        }

        /// <summary>
        /// Darken a color by a percentage
        /// </summary>
        private Color DarkenColor(Color color, int percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, color.R - (color.R * percent / 100)),
                Math.Max(0, color.G - (color.G * percent / 100)),
                Math.Max(0, color.B - (color.B * percent / 100))
            );
        }

        /// <summary>
        /// Get border color for outlined chips
        /// </summary>
        private Color GetBorderColor(AdvancedButtonPaintContext context)
        {
            if (context.IsSelected || context.IsToggled)
            {
                return context.SolidBackground;
            }

            return context.BorderColor != Color.Empty ? context.BorderColor : context.SolidForeground;
        }

        /// <summary>
        /// Check if text represents a hashtag
        /// </summary>
        private bool IsHashtagText(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.TrimStart().StartsWith("#");
        }

        /// <summary>
        /// Draw a fallback hashtag icon for chip buttons
        /// </summary>
        private void DrawFallbackHashtagIcon(Graphics g, Rectangle bounds, Color color)
        {
            int padding = 2;
            int thickness = Math.Max(2, bounds.Width / 8);
            int innerWidth = bounds.Width - padding * 2;
            int innerHeight = bounds.Height - padding * 2;

            using (Pen pen = new Pen(color, thickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Two vertical lines (slightly tilted for style)
                int vx1 = bounds.X + padding + innerWidth / 3;
                int vx2 = bounds.X + padding + (innerWidth * 2) / 3;
                int tilt = innerWidth / 8;

                g.DrawLine(pen, vx1 + tilt, bounds.Y + padding, vx1 - tilt, bounds.Bottom - padding);
                g.DrawLine(pen, vx2 + tilt, bounds.Y + padding, vx2 - tilt, bounds.Bottom - padding);

                // Two horizontal lines
                int hy1 = bounds.Y + padding + innerHeight / 3;
                int hy2 = bounds.Y + padding + (innerHeight * 2) / 3;
                g.DrawLine(pen, bounds.X + padding, hy1, bounds.Right - padding, hy1);
                g.DrawLine(pen, bounds.X + padding, hy2, bounds.Right - padding, hy2);
            }
        }
    }
}
