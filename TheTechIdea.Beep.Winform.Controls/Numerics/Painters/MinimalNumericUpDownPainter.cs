using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Minimal style painter for NumericUpDown
    /// Features: Ultra-clean design, thin borders, no decorations, monochrome palette
    /// </summary>
    public class MinimalNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Minimal: pure white background
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.White;

            Color primaryColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(100, 100, 100);

            Color borderColor = context.IsFocused
                ? primaryColor
                : (context.IsHovered
                    ? Color.FromArgb(180, 180, 180)
                    : Color.FromArgb(220, 220, 220));

            // Draw simple background
            using (var bgBrush = new SolidBrush(backColor))
            {
                g.FillRectangle(bgBrush, bounds);

                // Thin border - no rounded corners in minimal style
                using (var borderPen = new Pen(borderColor, 1))
                {
                    g.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
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
                var downRect = new Rectangle(bounds.X, bounds.Y, buttonWidth, bounds.Height);
                var upRect = new Rectangle(bounds.Right - buttonWidth, bounds.Y, buttonWidth, bounds.Height);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            Color buttonColor = Color.White;
            Color buttonHoverColor = Color.FromArgb(250, 250, 250);
            Color buttonPressedColor = Color.FromArgb(240, 240, 240);
            Color iconColor = Color.FromArgb(80, 80, 80);
            Color borderColor = Color.FromArgb(220, 220, 220);

            // Down button - minimal flat style
            DrawMinimalButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button - minimal flat style
            DrawMinimalButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(50, 50, 50);

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

        private void DrawMinimalButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Flat button background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, rect);
            }

            // Right border separator
            using (var borderPen = new Pen(borderColor, 1))
            {
                if (rect.X == 0) // Down button (left) - draw right border
                {
                    g.DrawLine(borderPen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
                }
                else // Up button (right) - draw left border
                {
                    g.DrawLine(borderPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                }
            }

            // Button text - simple and clean
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
}
