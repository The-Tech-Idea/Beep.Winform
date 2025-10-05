using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Ant Design style painter for NumericUpDown
    /// Features: Clean borders, Ant blue accent, 2px focus border, subtle hover states
    /// </summary>
    public class AntDesignNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Ant Design white background
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.White;

            Color antBlue = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(24, 144, 255);

            Color borderColor = context.IsFocused
                ? antBlue
                : (context.IsHovered
                    ? Color.FromArgb(64, 169, 255)
                    : Color.FromArgb(217, 217, 217));

            // Draw background with subtle radius
            int radius = 2; // Ant Design uses very subtle radius
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Border with 2px when focused
                int borderWidth = context.IsFocused ? 2 : 1;
                using (var borderPen = new Pen(borderColor, borderWidth))
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
                var downRect = new Rectangle(bounds.X + 1, bounds.Y + 1, buttonWidth - 2, bounds.Height - 2);
                var upRect = new Rectangle(bounds.Right - buttonWidth + 1, bounds.Y + 1, buttonWidth - 2, bounds.Height - 2);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            var theme = context.Theme;
            Color iconColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(24, 144, 255);

            Color buttonColor = Color.White;
            Color buttonHoverColor = Color.FromArgb(240, 247, 255); // Ant light blue
            Color buttonPressedColor = Color.FromArgb(230, 244, 255);
            Color borderColor = Color.FromArgb(217, 217, 217);

            // Down button
            DrawAntButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawAntButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(0, 0, 0);

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

        private void DrawAntButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Button background with 2px radius
            int radius = 2;
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Border separator
                using (var borderPen = new Pen(borderColor, 1))
                {
                    if (rect.X < 10) // Down button (left)
                    {
                        g.DrawLine(borderPen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                    }
                    else // Up button (right)
                    {
                        g.DrawLine(borderPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                    }
                }
            }

            // Button text
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
