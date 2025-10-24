using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Material Design 3 Style painter for NumericUpDown
    /// Features: Filled container, rounded corners, Material You color system
    /// </summary>
    public class Material3NumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            Color backColor = context.UseThemeColors && theme != null
                ? Color.FromArgb(245, theme.BackgroundColor)
                : Color.FromArgb(245, 245, 250);

            Color primaryColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(103, 80, 164);

            Color borderColor = context.IsFocused
                ? primaryColor
                : (context.IsHovered
                    ? Color.FromArgb(200, 200, 210)
                    : Color.FromArgb(220, 220, 230));

            // Draw filled background with rounded corners
            using (var bgBrush = new SolidBrush(backColor))
            {
                if (context.IsRounded && context.BorderRadius > 0)
                {
                    using (var path = CreateRoundedPath(bounds, context.BorderRadius))
                    {
                        g.FillPath(bgBrush, path);

                        // Draw border
                        int borderWidth = context.IsFocused ? 2 : 1;
                        using (var borderPen = new Pen(borderColor, borderWidth))
                        {
                            g.DrawPath(borderPen, path);
                        }
                    }
                }
                else
                {
                    g.FillRectangle(bgBrush, bounds);
                    using (var borderPen = new Pen(borderColor, context.IsFocused ? 2 : 1))
                    {
                        g.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
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

            // Draw spin buttons
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
            Color buttonColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(103, 80, 164);
            Color buttonHoverColor = Color.FromArgb(240, buttonColor);
            Color buttonPressedColor = Color.FromArgb(200, buttonColor);
            Color iconColor = Color.White;

            // Down button
            DrawMaterial3Button(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor);

            // Up button
            DrawMaterial3Button(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(50, 50, 70);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI", 11f, FontStyle.Regular))
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

        private void DrawMaterial3Button(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Material 3 uses elevated buttons with shadows
            if (!pressed && hovered)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                    using (var shadowPath = CreateRoundedPath(shadowRect, 8))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }

            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, 8))
            {
                g.FillPath(bgBrush, path);
            }

            using (var textBrush = new SolidBrush(foreColor))
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
