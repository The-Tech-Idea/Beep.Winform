using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Windows 11 Mica Style painter for NumericUpDown
    /// Features: Translucent Mica material, acrylic blur effect, WinUI 3 styling
    /// </summary>
    public class Windows11MicaNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Windows 11 Mica translucent background
            Color baseBack = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.FromArgb(249, 249, 249);
            Color backColor = Color.FromArgb(245, baseBack.R, baseBack.G, baseBack.B);

            Color winBlue = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(0, 120, 212);

            Color borderColor = context.IsFocused
                ? winBlue
                : Color.FromArgb(160, 160, 160);

            // Draw Mica-Style background with subtle radius
            int radius = context.IsRounded ? Math.Min(context.BorderRadius, 8) : 8;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle layered effect for Mica
                using (var layerBrush = new SolidBrush(Color.FromArgb(10, 255, 255, 255)))
                {
                    var layerRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 2);
                    using (var layerPath = CreateRoundedPath(layerRect, radius))
                    {
                        g.FillPath(layerBrush, layerPath);
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
            Color buttonColor = Color.FromArgb(240, 249, 249, 249);
            Color buttonHoverColor = Color.FromArgb(251, 251, 251);
            Color buttonPressedColor = Color.FromArgb(245, 245, 245);
            Color iconColor = Color.FromArgb(32, 32, 32);
            Color borderColor = Color.FromArgb(220, 220, 220);

            // Down button
            DrawMicaButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawMicaButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(32, 32, 32);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI Variable", 10.5f, FontStyle.Regular)) // Windows 11 font
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

        private void DrawMicaButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Mica button with rounded corners
            int radius = 6;
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle layered highlight
                if (hovered || pressed)
                {
                    using (var highlightBrush = new SolidBrush(Color.FromArgb(15, 255, 255, 255)))
                    {
                        var highlightRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
                        using (var highlightPath = CreateRoundedPath(highlightRect, radius))
                        {
                            g.FillPath(highlightBrush, highlightPath);
                        }
                    }
                }

                // Border
                using (var borderPen = new Pen(borderColor, 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Button icon
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe Fluent Icons", 10f, FontStyle.Regular)) // Windows 11 icons
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
