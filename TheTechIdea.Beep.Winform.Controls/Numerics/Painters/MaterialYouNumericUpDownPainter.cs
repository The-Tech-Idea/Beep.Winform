using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Material You style painter for NumericUpDown
    /// Features: Dynamic theming, large touch targets, bold colors, prominent buttons
    /// </summary>
    public class MaterialYouNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Material You tonal surface
            Color backColor = context.UseThemeColors && theme != null
                ? Color.FromArgb(250, theme.BackgroundColor)
                : Color.FromArgb(250, 245, 250);

            Color accentColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(103, 80, 164);

            // Draw filled background with large radius
            int radius = context.IsRounded ? Math.Max(context.BorderRadius, 16) : 16; // Material You loves large radius
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // No border in Material You when not focused
                if (context.IsFocused)
                {
                    using (var borderPen = new Pen(accentColor, 3))
                    {
                        g.DrawPath(borderPen, path);
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

            // Draw prominent buttons
            if (context.ShowSpinButtons)
            {
                int buttonWidth = GetButtonWidth(context, bounds) + 4; // Larger buttons
                var downRect = new Rectangle(bounds.X + 4, bounds.Y + 4, buttonWidth, bounds.Height - 8);
                var upRect = new Rectangle(bounds.Right - buttonWidth - 4, bounds.Y + 4, buttonWidth, bounds.Height - 8);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            var theme = context.Theme;
            Color accentColor = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(103, 80, 164);
            Color buttonHoverColor = Color.FromArgb(30, accentColor);
            Color buttonPressedColor = Color.FromArgb(60, accentColor);
            Color iconColor = accentColor;

            // Down button - Material You prominent style
            DrawMaterialYouButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                accentColor, iconColor, buttonHoverColor, buttonPressedColor);

            // Up button
            DrawMaterialYouButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                accentColor, iconColor, buttonHoverColor, buttonPressedColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(30, 30, 40);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI Variable", 12f, FontStyle.Regular)) // Material You uses larger text
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

        private void DrawMaterialYouButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color accentColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor)
        {
            // Material You large rounded buttons
            int radius = rect.Height / 2; // Full pill shape

            if (hovered || pressed)
            {
                Color bgColor = pressed ? pressedBackColor : hoverBackColor;
                using (var bgBrush = new SolidBrush(bgColor))
                using (var path = CreateRoundedPath(rect, radius))
                {
                    g.FillPath(bgBrush, path);
                }
            }

            // Icon with bold presence
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe UI", 14f, FontStyle.Bold)) // Larger, bolder icons
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
