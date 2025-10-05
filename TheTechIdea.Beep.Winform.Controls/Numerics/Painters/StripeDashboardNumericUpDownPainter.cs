using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Numerics.Painters;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using System.Drawing.Text;
/// <summary>
/// Stripe Dashboard style painter for NumericUpDown
/// Features: Professional palette, subtle rounded corners, clean lines, payment-focused design
/// </summary>
public class StripeDashboardNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Stripe white background
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.White;

            Color stripePurple = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(99, 91, 255);

            Color borderColor = context.IsFocused
                ? stripePurple
                : Color.FromArgb(223, 225, 230);

            // Draw background with Stripe's 6px radius
            int radius = context.IsRounded ? Math.Min(context.BorderRadius, 6) : 6;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Stripe focus with subtle shadow
                if (context.IsFocused)
                {
                    using (var shadowPen = new Pen(Color.FromArgb(60, 99, 91, 255), 4))
                    {
                        var shadowRect = new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4);
                        using (var shadowPath = CreateRoundedPath(shadowRect, radius + 2))
                        {
                            g.DrawPath(shadowPen, shadowPath);
                        }
                    }
                }

                // Border
                using (var borderPen = new Pen(borderColor, 1))
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
            Color buttonColor = Color.FromArgb(248, 249, 252); // Stripe light background
            Color buttonHoverColor = Color.FromArgb(245, 246, 250);
            Color buttonPressedColor = Color.FromArgb(240, 242, 247);
            Color iconColor = Color.FromArgb(66, 84, 102); // Stripe text
            Color borderColor = Color.FromArgb(223, 225, 230);

            // Down button
            DrawStripeButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawStripeButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            Color textColor = Color.FromArgb(50, 50, 93); // Stripe dark text

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI", 10.5f, FontStyle.Regular))
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

        private void DrawStripeButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Stripe button with 4px radius
            int radius = 4;
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle border separator
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
            using (var font = new Font("Segoe UI", 10f, FontStyle.Regular))
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

