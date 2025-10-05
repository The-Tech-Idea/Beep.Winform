using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Chakra UI style painter for NumericUpDown
    /// Features: Accessible colors, clear focus states, warm gray palette, rounded design
    /// </summary>
    public class ChakraUINumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Chakra UI white background
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.White;

            Color chakraBlue = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(66, 153, 225);

            Color borderColor = context.IsFocused
                ? chakraBlue
                : Color.FromArgb(226, 232, 240);

            // Draw background with Chakra's 6px radius
            int radius = context.IsRounded ? Math.Min(context.BorderRadius, 6) : 6;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Chakra focus shadow (box-shadow style)
                if (context.IsFocused)
                {
                    // Outer glow
                    using (var glowPen = new Pen(Color.FromArgb(100, 66, 153, 225), 4))
                    {
                        var glowRect = new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4);
                        using (var glowPath = CreateRoundedPath(glowRect, radius + 2))
                        {
                            g.DrawPath(glowPen, glowPath);
                        }
                    }
                }

                // Border
                using (var borderPen = new Pen(borderColor, context.IsFocused ? 2 : 1))
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
                var downRect = new Rectangle(bounds.X + 2, bounds.Y + 2, buttonWidth - 2, bounds.Height - 4);
                var upRect = new Rectangle(bounds.Right - buttonWidth, bounds.Y + 2, buttonWidth - 2, bounds.Height - 4);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            Color buttonColor = Color.FromArgb(247, 250, 252); // Chakra gray.50
            Color buttonHoverColor = Color.FromArgb(237, 242, 247); // Chakra gray.100
            Color buttonPressedColor = Color.FromArgb(226, 232, 240); // Chakra gray.200
            Color iconColor = Color.FromArgb(45, 55, 72); // Chakra gray.800
            Color borderColor = Color.FromArgb(226, 232, 240); // Chakra gray.300

            // Down button
            DrawChakraButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawChakraButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(45, 55, 72);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI", 10f, FontStyle.Regular))
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

        private void DrawChakraButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Chakra button with 4px radius
            int radius = 4;
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Border separator
                using (var borderPen = new Pen(borderColor, 1))
                {
                    if (rect.X < 10) // Down button (left)
                    {
                        g.DrawLine(borderPen, rect.Right, rect.Top + 2, rect.Right, rect.Bottom - 2);
                    }
                    else // Up button (right)
                    {
                        g.DrawLine(borderPen, rect.Left, rect.Top + 2, rect.Left, rect.Bottom - 2);
                    }
                }
            }

            // Button icon
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe UI", 11f, FontStyle.Regular))
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
