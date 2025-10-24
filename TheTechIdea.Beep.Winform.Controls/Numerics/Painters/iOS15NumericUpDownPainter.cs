using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// iOS 15 Style painter for NumericUpDown
    /// Features: Translucent backgrounds, pill-shaped buttons, SF Pro fonts, subtle shadows
    /// </summary>
    public class iOS15NumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // iOS 15 translucent background
            Color backColor = context.UseThemeColors && theme != null
                ? Color.FromArgb(240, theme.BackgroundColor)
                : Color.FromArgb(240, 248, 248, 250);

            Color iOSBlue = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(0, 122, 255);

            Color borderColor = context.IsFocused
                ? iOSBlue
                : (context.IsHovered
                    ? Color.FromArgb(200, 200, 210)
                    : Color.FromArgb(210, 210, 220));

            // Draw translucent background with rounded corners
            int radius = context.IsRounded ? context.BorderRadius : 10;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle inner shadow for depth
                if (!context.IsFocused)
                {
                    using (var shadowPen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
                    {
                        var innerRect = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
                        using (var innerPath = CreateRoundedPath(innerRect, radius - 1))
                        {
                            g.DrawPath(shadowPen, innerPath);
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

            // Draw pill-shaped buttons
            if (context.ShowSpinButtons)
            {
                int buttonWidth = GetButtonWidth(context, bounds);
                var downRect = new Rectangle(bounds.X + 4, bounds.Y + 4, buttonWidth - 8, bounds.Height - 8);
                var upRect = new Rectangle(bounds.Right - buttonWidth + 4, bounds.Y + 4, buttonWidth - 8, bounds.Height - 8);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            Color buttonColor = Color.FromArgb(0, 122, 255); // iOS blue
            Color buttonHoverColor = Color.FromArgb(230, 240, 255);
            Color buttonPressedColor = Color.FromArgb(200, 220, 255);
            Color iconColor = Color.White;

            // Down button - pill shaped
            DrawiOS15Button(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor);

            // Up button - pill shaped
            DrawiOS15Button(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(30, 30, 40);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("SF Pro Text", 11f, FontStyle.Regular))
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

        private void DrawiOS15Button(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor)
        {
            // Pill-shaped button (full rounded)
            int radius = rect.Height / 2;

            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Subtle shadow for elevation
            if (!pressed)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(25, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(rect.X + 1, rect.Y + 2, rect.Width, rect.Height);
                    using (var shadowPath = CreateRoundedPath(shadowRect, radius))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }

            // Button background
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);
            }

            // Button text
            Color textColor = (pressed || hovered) ? backColor : foreColor;
            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("SF Pro Display", 13f, FontStyle.Regular))
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
