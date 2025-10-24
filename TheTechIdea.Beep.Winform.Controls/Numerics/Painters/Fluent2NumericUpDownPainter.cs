using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Microsoft Fluent 2 Style painter for NumericUpDown
    /// Features: Subtle borders, clean design, rounded corners, focus ring
    /// </summary>
    public class Fluent2NumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Fluent 2 clean white/gray background
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.White;

            Color fluentBlue = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(0, 120, 212);

            Color borderColor = context.IsFocused
                ? fluentBlue
                : (context.IsHovered
                    ? Color.FromArgb(160, 160, 170)
                    : Color.FromArgb(200, 200, 210));

            // Draw background
            int radius = context.IsRounded ? Math.Min(context.BorderRadius, 4) : 4; // Fluent uses subtle radius
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Border
                using (var borderPen = new Pen(borderColor, 1))
                {
                    g.DrawPath(borderPen, path);
                }

                // Focus ring (double border)
                if (context.IsFocused)
                {
                    var focusRect = new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4);
                    using (var focusPath = CreateRoundedPath(focusRect, radius + 2))
                    using (var focusPen = new Pen(Color.FromArgb(80, borderColor), 2))
                    {
                        g.DrawPath(focusPen, focusPath);
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
                int buttonWidth = GetButtonWidth(context, bounds);
                var downRect = new Rectangle(bounds.X + 2, bounds.Y + 2, buttonWidth - 4, bounds.Height - 4);
                var upRect = new Rectangle(bounds.Right - buttonWidth + 2, bounds.Y + 2, buttonWidth - 4, bounds.Height - 4);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            var theme = context.Theme;
            Color buttonColor = Color.FromArgb(243, 243, 243); // Fluent gray
            Color buttonHoverColor = Color.FromArgb(233, 233, 233);
            Color buttonPressedColor = Color.FromArgb(210, 210, 210);
            Color iconColor = Color.FromArgb(50, 50, 50);

            // Down button
            DrawFluent2Button(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor);

            // Up button
            DrawFluent2Button(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(32, 32, 32);

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

        private void DrawFluent2Button(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Fluent 2 subtle rounded corners
            int radius = 4;

            // Button background
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle border
                using (var borderPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Button icon
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe UI", 11f, FontStyle.Bold))
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
