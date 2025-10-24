using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Gradient Modern Style painter for NumericUpDown
    /// Features: Vibrant gradients, purple-pink colors, glowing buttons, modern colorful aesthetic
    /// </summary>
    public class GradientModernNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Gradient colors
            Color startColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(138, 43, 226);

            Color endColor = context.UseThemeColors && theme != null
                ? theme.AccentColor
                : Color.FromArgb(219, 39, 119);

            Color borderColor = context.IsFocused
                ? Color.FromArgb(236, 72, 153)
                : Color.FromArgb(200, 150, 220);

            // Draw background with vibrant gradient
            int radius = context.IsRounded ? Math.Max(context.BorderRadius, 12) : 12;
            using (var path = CreateRoundedPath(bounds, radius))
            {
                // Gradient background
                using (var gradientBrush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(250, 245, 255), // Very light purple
                    Color.FromArgb(255, 240, 250), // Very light pink
                    45f)) // Diagonal gradient
                {
                    g.FillPath(gradientBrush, path);
                }

                // Gradient border overlay
                using (var borderBrush = new LinearGradientBrush(
                    bounds,
                    startColor,
                    endColor,
                    LinearGradientMode.Horizontal))
                using (var borderPen = new Pen(borderBrush, context.IsFocused ? 3 : 2))
                {
                    g.DrawPath(borderPen, path);
                }

                // Glow effect when focused
                if (context.IsFocused)
                {
                    for (int i = 3; i > 0; i--)
                    {
                        int alpha = 30 - (i * 8);
                        using (var glowBrush = new LinearGradientBrush(
                            new Rectangle(bounds.X - i, bounds.Y - i, bounds.Width + (i * 2), bounds.Height + (i * 2)),
                            Color.FromArgb(alpha, startColor),
                            Color.FromArgb(alpha, endColor),
                            LinearGradientMode.Horizontal))
                        using (var glowPen = new Pen(glowBrush, i * 2))
                        {
                            var glowRect = new Rectangle(bounds.X - i, bounds.Y - i, bounds.Width + (i * 2), bounds.Height + (i * 2));
                            using (var glowPath = CreateRoundedPath(glowRect, radius + i))
                            {
                                g.DrawPath(glowPen, glowPath);
                            }
                        }
                    }
                }
            }

            // Draw value text if not editing
            if (!context.IsEditing && context.ShowSpinButtons)
            {
                var textRect = GetTextRect(context, bounds);
                string formattedText = FormatValue(context);
                PaintValueText(g, context, textRect, formattedText);
            }

            // Draw buttons
            if (context.ShowSpinButtons)
            {
                int buttonWidth = GetButtonWidth(context, bounds) + 2;
                var downRect = new Rectangle(bounds.X + 4, bounds.Y + 4, buttonWidth, bounds.Height - 8);
                var upRect = new Rectangle(bounds.Right - buttonWidth - 4, bounds.Y + 4, buttonWidth, bounds.Height - 8);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            var theme = context.Theme;
            Color startColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(138, 43, 226);

            Color endColor = context.UseThemeColors && theme != null
                ? theme.AccentColor
                : Color.FromArgb(219, 39, 119);

            Color iconColor = Color.White;

            // Down button
            DrawGradientButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                startColor, endColor, iconColor);

            // Up button
            DrawGradientButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                startColor, endColor, iconColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color gradientStart = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(138, 43, 226);

            Color gradientEnd = context.UseThemeColors && theme != null
                ? theme.AccentColor
                : Color.FromArgb(219, 39, 119);

            // Gradient text color
            using (var textBrush = new LinearGradientBrush(
                textRect,
                gradientStart,
                gradientEnd,
                LinearGradientMode.Horizontal))
            using (var font = new Font("Segoe UI", 11f, FontStyle.Bold))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(formattedText, font, textBrush, textRect, sf);
            }
        }

        private void DrawGradientButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color startColor, Color endColor, Color iconColor)
        {
            // Button with gradient fill
            int radius = rect.Height / 2; // Pill shape
            using (var path = CreateRoundedPath(rect, radius))
            {
                if (hovered || pressed)
                {
                    // Vibrant gradient on hover/press
                    Color adjustedStart = pressed 
                        ? Color.FromArgb(Math.Max(0, startColor.R - 30), Math.Max(0, startColor.G - 30), Math.Max(0, startColor.B - 30))
                        : Color.FromArgb(Math.Min(255, startColor.R + 20), Math.Min(255, startColor.G + 20), Math.Min(255, startColor.B + 20));
                    
                    Color adjustedEnd = pressed
                        ? Color.FromArgb(Math.Max(0, endColor.R - 30), Math.Max(0, endColor.G - 30), Math.Max(0, endColor.B - 30))
                        : Color.FromArgb(Math.Min(255, endColor.R + 20), Math.Min(255, endColor.G + 20), Math.Min(255, endColor.B + 20));

                    using (var gradientBrush = new LinearGradientBrush(
                        rect,
                        adjustedStart,
                        adjustedEnd,
                        LinearGradientMode.Horizontal))
                    {
                        g.FillPath(gradientBrush, path);
                    }

                    // Glow on hover
                    if (hovered)
                    {
                        for (int i = 2; i > 0; i--)
                        {
                            int alpha = 40 - (i * 15);
                            using (var glowBrush = new LinearGradientBrush(
                                new Rectangle(rect.X - i, rect.Y - i, rect.Width + (i * 2), rect.Height + (i * 2)),
                                Color.FromArgb(alpha, startColor),
                                Color.FromArgb(alpha, endColor),
                                LinearGradientMode.Horizontal))
                            using (var glowPen = new Pen(glowBrush, i * 2))
                            {
                                var glowRect = new Rectangle(rect.X - i, rect.Y - i, rect.Width + (i * 2), rect.Height + (i * 2));
                                using (var glowPath = CreateRoundedPath(glowRect, radius + i))
                                {
                                    g.DrawPath(glowPen, glowPath);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Subtle gradient when not hovered
                    using (var gradientBrush = new LinearGradientBrush(
                        rect,
                        Color.FromArgb(230, 210, 240),
                        Color.FromArgb(255, 230, 245),
                        LinearGradientMode.Horizontal))
                    {
                        g.FillPath(gradientBrush, path);
                    }
                }
            }

            // Button icon
            Color finalIconColor = (hovered || pressed) ? Color.White : Color.FromArgb(138, 43, 226);
            using (var textBrush = new SolidBrush(finalIconColor))
            using (var font = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, textBrush, rect, sf);
            }
        }

        private string FormatValue(INumericUpDownPainterContext context)
        {
            string valueStr = context.DecimalPlaces > 0
                ? context.Value.ToString($"N{context.DecimalPlaces}")
                : (context.ThousandsSeparator ? context.Value.ToString("N0") : context.Value.ToString());

            return context.DisplayMode switch
            {
                NumericUpDownDisplayMode.Percentage => $"{valueStr}%",
                NumericUpDownDisplayMode.Currency => $"${valueStr}",
                NumericUpDownDisplayMode.CustomUnit => $"{context.Prefix}{valueStr}{context.Suffix} {context.Unit}".Trim(),
                _ => valueStr
            };
        }
    }
}
