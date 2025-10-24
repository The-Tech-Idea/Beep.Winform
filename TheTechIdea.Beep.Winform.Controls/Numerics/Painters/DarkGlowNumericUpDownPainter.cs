using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Dark Glow Style painter for NumericUpDown
    /// Features: Dark backgrounds, cyan/neon glow effects, cyberpunk aesthetic
    /// </summary>
    public class DarkGlowNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            int radius;
            // Dark background
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.FromArgb(26, 32, 44);

            Color glowColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(0, 255, 200);

            Color borderColor = context.IsFocused
                ? glowColor
                : Color.FromArgb(74, 85, 104);

            // Draw glow effect when focused
            if (context.IsFocused)
            {
                // Multiple glow layers for neon effect
                for (int i = 8; i > 0; i--)
                {
                    int alpha = 15 - (i * 2);
                    using (var glowPen = new Pen(Color.FromArgb(alpha, glowColor), i * 2))
                    {
                        var glowRect = new Rectangle(
                            bounds.X - i, bounds.Y - i, 
                            bounds.Width + (i * 2), bounds.Height + (i * 2));
                         radius = context.IsRounded ? Math.Max(context.BorderRadius, 8) : 8;
                        using (var glowPath = CreateRoundedPath(glowRect, radius + i))
                        {
                            g.DrawPath(glowPen, glowPath);
                        }
                    }
                }
            }

            // Draw background
            radius = context.IsRounded ? Math.Max(context.BorderRadius, 8) : 8;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Inner glow
                using (var innerGlowPen = new Pen(Color.FromArgb(30, glowColor), 2))
                {
                    var innerRect = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
                    using (var innerPath = CreateRoundedPath(innerRect, radius - 1))
                    {
                        g.DrawPath(innerGlowPen, innerPath);
                    }
                }

                // Border
                using (var borderPen = new Pen(borderColor, 2))
                {
                    g.DrawPath(borderPen, path);
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
                int buttonWidth = GetButtonWidth(context, bounds);
                var downRect = new Rectangle(bounds.X + 3, bounds.Y + 3, buttonWidth - 3, bounds.Height - 6);
                var upRect = new Rectangle(bounds.Right - buttonWidth, bounds.Y + 3, buttonWidth - 3, bounds.Height - 6);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            var theme = context.Theme;
            Color glowColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(0, 255, 200);

            Color buttonColor = Color.FromArgb(45, 55, 72);
            Color buttonHoverColor = Color.FromArgb(74, 85, 104);
            Color buttonPressedColor = Color.FromArgb(26, 32, 44);
            Color iconColor = glowColor;

            // Down button
            DrawDarkGlowButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, glowColor);

            // Up button
            DrawDarkGlowButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, glowColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(226, 232, 240);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Consolas", 11f, FontStyle.Regular)) // Monospace for tech feel
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                // Subtle glow on text
                using (var glowBrush = new SolidBrush(Color.FromArgb(40, 0, 255, 200)))
                {
                    var glowRect = new RectangleF(textRect.X + 1, textRect.Y + 1, textRect.Width, textRect.Height);
                    g.DrawString(formattedText, font, glowBrush, glowRect, sf);
                }

                g.DrawString(formattedText, font, textBrush, textRect, sf);
            }
        }

        private void DrawDarkGlowButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color glowColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Button glow on hover
            if (hovered || pressed)
            {
                for (int i = 3; i > 0; i--)
                {
                    int alpha = 20 - (i * 5);
                    using (var glowPen = new Pen(Color.FromArgb(alpha, glowColor), i * 2))
                    {
                        var glowRect = new Rectangle(
                            rect.X - i, rect.Y - i,
                            rect.Width + (i * 2), rect.Height + (i * 2));
                        using (var glowPath = CreateRoundedPath(glowRect, 6 + i))
                        {
                            g.DrawPath(glowPen, glowPath);
                        }
                    }
                }
            }

            // Button background
            int radius = 6;
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Border with glow color
                using (var borderPen = new Pen(hovered ? glowColor : Color.FromArgb(74, 85, 104), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Button icon with glow
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Consolas", 12f, FontStyle.Bold))
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
