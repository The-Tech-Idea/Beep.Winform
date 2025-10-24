using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Tailwind Card Style painter for NumericUpDown
    /// Features: Card-like container, shadows, Tailwind color palette, utility-first design
    /// </summary>
    public class TailwindCardNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            int radius;
            var theme = context.Theme;
            
            // Tailwind white card background
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.White;

            Color tailwindBlue = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(59, 130, 246);

            Color borderColor = context.IsFocused
                ? tailwindBlue
                : Color.FromArgb(229, 231, 235);

            // Draw shadow (card elevation)
            if (context.EnableShadow)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width, bounds.Height);
                     radius = context.IsRounded ? Math.Min(context.BorderRadius, 8) : 8;
                    using (var shadowPath = CreateRoundedPath(shadowRect, radius))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }

            // Draw card background with Tailwind's rounded-lg (8px)
             radius = context.IsRounded ? Math.Min(context.BorderRadius, 8) : 8;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Tailwind ring focus (ring-2)
                if (context.IsFocused)
                {
                    using (var ringPen = new Pen(Color.FromArgb(147, 197, 253), 3)) // Tailwind blue-300
                    {
                        var ringRect = new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4);
                        using (var ringPath = CreateRoundedPath(ringRect, radius + 2))
                        {
                            g.DrawPath(ringPen, ringPath);
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
            Color buttonColor = Color.FromArgb(249, 250, 251); // Tailwind gray-50
            Color buttonHoverColor = Color.FromArgb(243, 244, 246); // Tailwind gray-100
            Color buttonPressedColor = Color.FromArgb(229, 231, 235); // Tailwind gray-200
            Color iconColor = Color.FromArgb(55, 65, 81); // Tailwind gray-700
            Color borderColor = Color.FromArgb(229, 231, 235); // Tailwind gray-200

            // Down button
            DrawTailwindButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawTailwindButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(31, 41, 55);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Inter", 10f, FontStyle.Regular)) // Tailwind default font
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                // Fallback to Segoe UI if Inter not available
                var actualFont = font;
                try
                {
                    actualFont = new Font("Inter", 10f, FontStyle.Regular);
                }
                catch
                {
                    actualFont = new Font("Segoe UI", 10f, FontStyle.Regular);
                }

                g.DrawString(formattedText, actualFont, textBrush, textRect, sf);
            }
        }

        private void DrawTailwindButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Tailwind button with 6px radius (rounded-md)
            int radius = 6;
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
