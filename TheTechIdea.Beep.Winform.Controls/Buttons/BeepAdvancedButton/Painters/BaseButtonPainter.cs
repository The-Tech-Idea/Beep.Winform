using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Base painter with common painting functionality
    /// </summary>
    public abstract class BaseButtonPainter : IAdvancedButtonPainter
    {
        public abstract void Paint(AdvancedButtonPaintContext context);

        #region "Protected Helper Methods"

        protected GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // Top left
            path.AddArc(arc, 180, 90);
            
            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }

        protected void DrawShadow(Graphics g, Rectangle bounds, int radius, int shadowBlur, Color shadowColor)
        {
            if (shadowBlur <= 0) return;

            // Draw multiple layers for blur effect
            for (int i = shadowBlur; i > 0; i--)
            {
                int alpha = (int)((shadowColor.A / (float)shadowBlur) * (shadowBlur - i + 1));
                Color layerColor = Color.FromArgb(alpha, shadowColor);
                
                Rectangle shadowBounds = new Rectangle(
                    bounds.X + i / 2,
                    bounds.Y + i / 2,
                    bounds.Width,
                    bounds.Height
                );

                using (GraphicsPath path = GetRoundedRectanglePath(shadowBounds, radius))
                using (Brush brush = new SolidBrush(layerColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        protected void FillRoundedRectangle(Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = GetRoundedRectanglePath(bounds, radius))
            {
                g.FillPath(brush, path);
            }
        }

        protected void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = GetRoundedRectanglePath(bounds, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        protected void DrawRippleEffect(Graphics g, AdvancedButtonPaintContext context)
        {
            if (!context.RippleActive) return;

            // Calculate ripple radius
            float maxDistance = (float)Math.Sqrt(
                Math.Pow(context.Bounds.Width, 2) + 
                Math.Pow(context.Bounds.Height, 2)
            );
            float rippleRadius = maxDistance * context.RippleProgress;
            
            // Calculate alpha based on progress (fade out as it expands)
            int alpha = (int)(50 * (1 - context.RippleProgress));
            
            using (GraphicsPath clipPath = GetRoundedRectanglePath(context.Bounds, context.BorderRadius))
            {
                g.SetClip(clipPath);
                
                using (Brush rippleBrush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
                {
                    g.FillEllipse(rippleBrush,
                        context.RippleCenter.X - rippleRadius,
                        context.RippleCenter.Y - rippleRadius,
                        rippleRadius * 2,
                        rippleRadius * 2
                    );
                }
                
                g.ResetClip();
            }
        }

        protected void DrawIcon(Graphics g, AdvancedButtonPaintContext context, Rectangle iconBounds, string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath)) return;

            var imagePainter = context.ImagePainter;
            if (imagePainter != null)
            {
                // Set up the painter for this icon
                imagePainter.ImagePath = iconPath;
                imagePainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                imagePainter.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
                
                // Apply theme colors if needed
                if (imagePainter.ApplyThemeOnImage)
                {
                    imagePainter.FillColor = context.SolidForeground;
                    imagePainter.StrokeColor = context.SolidForeground;
                    imagePainter.ApplyThemeToSvg();
                }
                
                // Draw the image
                imagePainter.DrawImage(g, iconBounds);
            }
        }

        protected void DrawText(Graphics g, AdvancedButtonPaintContext context, Rectangle textBounds, Color textColor)
        {
            if (string.IsNullOrEmpty(context.Text)) return;

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap;

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(context.Text, context.Font, textBrush, textBounds, sf);
                }
            }
        }

        protected void DrawLoadingSpinner(Graphics g, Rectangle bounds, Color color)
        {
            // Simple rotating spinner
            int spinnerSize = Math.Min(bounds.Width, bounds.Height) / 2;
            Rectangle spinnerBounds = new Rectangle(
                bounds.X + (bounds.Width - spinnerSize) / 2,
                bounds.Y + (bounds.Height - spinnerSize) / 2,
                spinnerSize,
                spinnerSize
            );

            using (Pen spinnerPen = new Pen(color, 2))
            {
                spinnerPen.StartCap = LineCap.Round;
                spinnerPen.EndCap = LineCap.Round;
                
                // Draw arc (animated in actual implementation)
                g.DrawArc(spinnerPen, spinnerBounds, 0, 270);
            }
        }

        protected Color GetBackgroundColor(AdvancedButtonPaintContext context)
        {
            return context.State switch
            {
                AdvancedButtonState.Disabled => context.DisabledBackground,
                AdvancedButtonState.Pressed => context.PressedBackground,
                AdvancedButtonState.Hover => context.HoverBackground,
                _ => context.SolidBackground
            };
        }

        protected Color GetForegroundColor(AdvancedButtonPaintContext context)
        {
            return context.State switch
            {
                AdvancedButtonState.Disabled => context.DisabledForeground,
                AdvancedButtonState.Pressed => context.SolidForeground,
                AdvancedButtonState.Hover => context.HoverForeground,
                _ => context.SolidForeground
            };
        }

        protected AdvancedButtonMetrics GetMetrics(AdvancedButtonPaintContext context)
        {
            return AdvancedButtonMetrics.GetMetrics(context.ButtonSize);
        }

        #endregion
    }
}
