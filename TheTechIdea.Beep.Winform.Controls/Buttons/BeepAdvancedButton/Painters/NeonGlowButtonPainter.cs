using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Painter for neon/glow effect buttons with glowing borders and icons
    /// Creates modern cyberpunk-style buttons with neon lighting effects
    /// Common uses: Gaming UI, dark mode interfaces, futuristic designs
    /// </summary>
    public class NeonGlowButtonPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle buttonBounds = context.Bounds;

            // Get neon glow color (use solid background or custom glow color)
            Color glowColor = context.GlowColor;

            // Get glow intensity
            int glowIntensity = context.IsHovered ? 255 : (context.IsPressed ? 200 : 180);
            int glowLayers = context.IsHovered ? 5 : 3;

            // Draw multi-layer glow effect (outer glow)
            DrawNeonGlow(g, buttonBounds, glowColor, glowIntensity, glowLayers);

            // Draw dark background (nearly transparent for neon effect)
            Color bgColor = context.BackgroundColor; // Very dark, semi-transparent

            using (Brush bgBrush = new SolidBrush(bgColor))
            {
                ButtonShapeHelper.FillShape(g, ButtonShape.Pill, buttonBounds, buttonBounds.Height / 2, bgBrush);
            }

            // Draw glowing border (main neon line)
            DrawGlowingBorder(g, buttonBounds, glowColor, glowIntensity);

            // Determine if icon is on right (arrow/chevron style)
            bool iconOnRight = !string.IsNullOrEmpty(context.IconRight) || 
                              context.Text.Contains("NOW") || 
                              context.Text.Contains("LOGIN") || 
                              context.Text.Contains("MORE");

            // Calculate layout
            CalculateNeonLayout(context, metrics, buttonBounds, iconOnRight, 
                out Rectangle textBounds, out Rectangle iconBounds, out Rectangle iconCircleBounds);

            // Draw icon circle section (glowing circle on right)
            if (iconOnRight && !iconCircleBounds.IsEmpty)
            {
                DrawGlowingIconCircle(g, iconCircleBounds, glowColor, glowIntensity);
            }

            // Draw text with glow effect
            if (!string.IsNullOrEmpty(context.Text))
            {
                DrawGlowingText(g, context, textBounds, glowColor, glowIntensity);
            }

            // Draw icon with glow
            if (!iconBounds.IsEmpty)
            {
                string iconPath = iconOnRight ? context.IconRight : context.IconLeft;
                if (string.IsNullOrEmpty(iconPath))
                    iconPath = context.ImagePainter?.ImagePath;

                if (!string.IsNullOrEmpty(iconPath))
                {
                    DrawGlowingIcon(g, context, iconBounds, iconPath, glowColor, glowIntensity);
                }
            }
        }

        /// <summary>
        /// Draw multi-layer outer glow effect
        /// </summary>
        private void DrawNeonGlow(Graphics g, Rectangle bounds, Color glowColor, int intensity, int layers)
        {
            for (int i = layers; i > 0; i--)
            {
                int offset = i * 3;
                int alpha = (intensity / layers) / (i + 1);

                Rectangle glowBounds = new Rectangle(
                    bounds.X - offset,
                    bounds.Y - offset,
                    bounds.Width + (offset * 2),
                    bounds.Height + (offset * 2)
                );

                using (GraphicsPath glowPath = CreatePillPath(glowBounds))
                using (PathGradientBrush glowBrush = new PathGradientBrush(glowPath))
                {
                    glowBrush.CenterColor = Color.FromArgb(alpha, glowColor);
                    glowBrush.SurroundColors = new Color[] { Color.FromArgb(0, glowColor) };
                    glowBrush.FocusScales = new PointF(0.7f, 0.7f);

                    g.FillPath(glowBrush, glowPath);
                }
            }
        }

        /// <summary>
        /// Draw glowing border (main neon outline)
        /// </summary>
        private void DrawGlowingBorder(Graphics g, Rectangle bounds, Color glowColor, int intensity)
        {
            // Inner bright line
            using (Pen brightPen = new Pen(Color.FromArgb(intensity, glowColor), 2))
            {
                brightPen.Alignment = PenAlignment.Inset;
                ButtonShapeHelper.DrawShape(g, ButtonShape.Pill, bounds, bounds.Height / 2, brightPen);
            }

            // Outer glow line
            Rectangle outerBounds = new Rectangle(
                bounds.X - 1,
                bounds.Y - 1,
                bounds.Width + 2,
                bounds.Height + 2
            );

            using (Pen glowPen = new Pen(Color.FromArgb(intensity / 2, glowColor), 3))
            {
                glowPen.Alignment = PenAlignment.Outset;
                ButtonShapeHelper.DrawShape(g, ButtonShape.Pill, outerBounds, outerBounds.Height / 2, glowPen);
            }
        }

        /// <summary>
        /// Calculate layout for neon button (text and icon positioning)
        /// </summary>
        private void CalculateNeonLayout(AdvancedButtonPaintContext context, AdvancedButtonMetrics metrics,
            Rectangle bounds, bool iconOnRight, out Rectangle textBounds, out Rectangle iconBounds, 
            out Rectangle iconCircleBounds)
        {
            int circleSize = (int)(bounds.Height * 0.7);
            int circlePadding = (bounds.Height - circleSize) / 2;
            int textPadding = metrics.PaddingHorizontal + 10;

            if (iconOnRight)
            {
                // Icon circle on right
                iconCircleBounds = new Rectangle(
                    bounds.Right - circleSize - circlePadding,
                    bounds.Y + circlePadding,
                    circleSize,
                    circleSize
                );

                // Icon inside circle
                int iconSize = circleSize / 2;
                iconBounds = new Rectangle(
                    iconCircleBounds.X + (iconCircleBounds.Width - iconSize) / 2,
                    iconCircleBounds.Y + (iconCircleBounds.Height - iconSize) / 2,
                    iconSize,
                    iconSize
                );

                // Text on left
                textBounds = new Rectangle(
                    bounds.X + textPadding,
                    bounds.Y,
                    bounds.Width - textPadding - circleSize - circlePadding - 15,
                    bounds.Height
                );
            }
            else
            {
                // Icon circle on left
                iconCircleBounds = new Rectangle(
                    bounds.X + circlePadding,
                    bounds.Y + circlePadding,
                    circleSize,
                    circleSize
                );

                // Icon inside circle
                int iconSize = circleSize / 2;
                iconBounds = new Rectangle(
                    iconCircleBounds.X + (iconCircleBounds.Width - iconSize) / 2,
                    iconCircleBounds.Y + (iconCircleBounds.Height - iconSize) / 2,
                    iconSize,
                    iconSize
                );

                // Text on right
                textBounds = new Rectangle(
                    iconCircleBounds.Right + 15,
                    bounds.Y,
                    bounds.Width - circleSize - circlePadding - textPadding - 15,
                    bounds.Height
                );
            }
        }

        /// <summary>
        /// Draw glowing icon circle section
        /// </summary>
        private void DrawGlowingIconCircle(Graphics g, Rectangle circleBounds, Color glowColor, int intensity)
        {
            // Multi-layer circle glow
            for (int i = 3; i > 0; i--)
            {
                int offset = i * 2;
                int alpha = (intensity / 3) / (i + 1);

                Rectangle glowCircle = new Rectangle(
                    circleBounds.X - offset,
                    circleBounds.Y - offset,
                    circleBounds.Width + (offset * 2),
                    circleBounds.Height + (offset * 2)
                );

                using (GraphicsPath circlePath = new GraphicsPath())
                {
                    circlePath.AddEllipse(glowCircle);

                    using (PathGradientBrush glowBrush = new PathGradientBrush(circlePath))
                    {
                        glowBrush.CenterColor = Color.FromArgb(alpha, glowColor);
                        glowBrush.SurroundColors = new Color[] { Color.FromArgb(0, glowColor) };
                        glowBrush.FocusScales = new PointF(0.6f, 0.6f);

                        g.FillPath(glowBrush, circlePath);
                    }
                }
            }

            // Dark circle background
            using (Brush circleBg = new SolidBrush(Color.FromArgb(30, 15, 20, 40)))
            {
                g.FillEllipse(circleBg, circleBounds);
            }

            // Glowing circle border
            using (Pen circlePen = new Pen(Color.FromArgb(intensity, glowColor), 2))
            {
                g.DrawEllipse(circlePen, circleBounds);
            }

            // Inner glow
            Rectangle innerCircle = new Rectangle(
                circleBounds.X + 1,
                circleBounds.Y + 1,
                circleBounds.Width - 2,
                circleBounds.Height - 2
            );

            using (Pen innerPen = new Pen(Color.FromArgb(intensity / 2, glowColor), 1))
            {
                g.DrawEllipse(innerPen, innerCircle);
            }
        }

        /// <summary>
        /// Draw text with glow effect
        /// </summary>
        private void DrawGlowingText(Graphics g, AdvancedButtonPaintContext context, Rectangle textBounds, 
            Color glowColor, int intensity)
        {
            if (string.IsNullOrEmpty(context.Text)) return;

            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;

                // Draw text glow layers
                for (int i = 0; i < 3; i++)
                {
                    int alpha = (intensity / 3) / (i + 1);
                    using (Brush glowBrush = new SolidBrush(Color.FromArgb(alpha, glowColor)))
                    {
                        Rectangle glowTextBounds = new Rectangle(
                            textBounds.X - i,
                            textBounds.Y - i,
                            textBounds.Width,
                            textBounds.Height
                        );
                        g.DrawString(context.Text, context.Font, glowBrush, glowTextBounds, format);
                    }
                }

                // Draw main text (bright)
                using (Brush textBrush = new SolidBrush(Color.FromArgb(255, Color.White)))
                {
                    g.DrawString(context.Text, context.Font, textBrush, textBounds, format);
                }
            }
        }

        /// <summary>
        /// Draw icon with glow effect
        /// </summary>
        private void DrawGlowingIcon(Graphics g, AdvancedButtonPaintContext context, Rectangle iconBounds, 
            string iconPath, Color glowColor, int intensity)
        {
            // For now, draw a simple glowing icon representation
            // In production, this would load and tint the actual icon
            
            // Icon glow layers
            for (int i = 2; i > 0; i--)
            {
                int offset = i * 2;
                int alpha = (intensity / 2) / (i + 1);

                Rectangle glowIconBounds = new Rectangle(
                    iconBounds.X - offset,
                    iconBounds.Y - offset,
                    iconBounds.Width + (offset * 2),
                    iconBounds.Height + (offset * 2)
                );

                using (Brush glowBrush = new SolidBrush(Color.FromArgb(alpha, glowColor)))
                {
                    // Draw icon representation (would be actual icon in production)
                    DrawIconShape(g, glowBrush, glowIconBounds, iconPath);
                }
            }

            // Draw main icon
            using (Brush iconBrush = new SolidBrush(Color.FromArgb(intensity, Color.White)))
            {
                DrawIconShape(g, iconBrush, iconBounds, iconPath);
            }
        }

        /// <summary>
        /// Draw icon shape based on icon type (simplified for demo)
        /// </summary>
        private void DrawIconShape(Graphics g, Brush brush, Rectangle bounds, string iconPath)
        {
            // Simplified icon shapes - in production would load actual icons
            string iconType = iconPath.ToLower();

            if (iconType.Contains("user") || iconType.Contains("account"))
            {
                // User icon - circle with head
                int headSize = bounds.Height / 3;
                Rectangle head = new Rectangle(
                    bounds.X + (bounds.Width - headSize) / 2,
                    bounds.Y + bounds.Height / 6,
                    headSize,
                    headSize
                );
                g.FillEllipse(brush, head);
            }
            else if (iconType.Contains("play") || iconType.Contains("start"))
            {
                // Play triangle
                Point[] triangle = new Point[]
                {
                    new Point(bounds.X + bounds.Width / 4, bounds.Y),
                    new Point(bounds.Right, bounds.Y + bounds.Height / 2),
                    new Point(bounds.X + bounds.Width / 4, bounds.Bottom)
                };
                g.FillPolygon(brush, triangle);
            }
            else if (iconType.Contains("cart") || iconType.Contains("buy"))
            {
                // Shopping cart
                Rectangle cart = new Rectangle(bounds.X, bounds.Y, bounds.Width - 4, bounds.Height - 4);
                g.FillRectangle(brush, cart);
            }
            else if (iconType.Contains("arrow") || iconType.Contains("chevron") || iconType.Contains("more"))
            {
                // Chevron/Arrow right
                using (Pen arrowPen = new Pen(brush, 3))
                {
                    arrowPen.StartCap = LineCap.Round;
                    arrowPen.EndCap = LineCap.Round;
                    
                    Point p1 = new Point(bounds.X + bounds.Width / 3, bounds.Y + 4);
                    Point p2 = new Point(bounds.Right - 4, bounds.Y + bounds.Height / 2);
                    Point p3 = new Point(bounds.X + bounds.Width / 3, bounds.Bottom - 4);
                    
                    g.DrawLine(arrowPen, p1, p2);
                    g.DrawLine(arrowPen, p2, p3);
                }
            }
            else if (iconType.Contains("file") || iconType.Contains("register"))
            {
                // Document/File icon
                Rectangle doc = new Rectangle(bounds.X + 2, bounds.Y, bounds.Width - 6, bounds.Height - 2);
                g.FillRectangle(brush, doc);
            }
            else
            {
                // Default - simple rectangle
                g.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Create pill-shaped path
        /// </summary>
        private GraphicsPath CreatePillPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            int radius = bounds.Height / 2;
            int diameter = radius * 2;

            // Left semicircle
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 90, 180);
            
            // Top line
            path.AddLine(bounds.X + radius, bounds.Y, bounds.Right - radius, bounds.Y);
            
            // Right semicircle
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 180);
            
            // Bottom line
            path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.X + radius, bounds.Bottom);
            
            path.CloseFigure();
            return path;
        }
    }
}
