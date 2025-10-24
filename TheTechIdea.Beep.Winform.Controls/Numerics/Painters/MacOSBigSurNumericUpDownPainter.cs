using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// macOS Big Sur Style painter for NumericUpDown
    /// Features: Vibrancy effects, subtle gradients, SF Pro fonts, desktop-focused design
    /// </summary>
    public class MacOSBigSurNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // macOS Big Sur translucent vibrancy
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.FromArgb(252, 252, 252);

            Color macOSBlue = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(0, 122, 255);

            Color borderColor = context.IsFocused
                ? macOSBlue
                : Color.FromArgb(210, 210, 215);

            // Draw background with macOS rounded Style
            int radius = context.IsRounded ? Math.Max(context.BorderRadius, 6) : 6;
            using (var path = CreateRoundedPath(bounds, radius))
            {
                // Subtle gradient background
                using (var gradientBrush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(252, 252, 252),
                    Color.FromArgb(246, 246, 248),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradientBrush, path);
                }

                // Inner shadow for depth
                using (var shadowPen = new Pen(Color.FromArgb(15, 0, 0, 0), 1))
                {
                    var shadowRect = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
                    using (var shadowPath = CreateRoundedPath(shadowRect, radius - 1))
                    {
                        g.DrawPath(shadowPen, shadowPath);
                    }
                }

                // Border with focus ring
                if (context.IsFocused)
                {
                    // Outer glow ring
                    using (var glowPen = new Pen(Color.FromArgb(80, 0, 122, 255), 3))
                    {
                        var glowRect = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2);
                        using (var glowPath = CreateRoundedPath(glowRect, radius + 1))
                        {
                            g.DrawPath(glowPen, glowPath);
                        }
                    }
                }

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
            Color buttonColor = Color.FromArgb(248, 248, 250);
            Color buttonHoverColor = Color.FromArgb(242, 242, 245);
            Color buttonPressedColor = Color.FromArgb(235, 235, 238);
            Color iconColor = Color.FromArgb(60, 60, 67);
            Color borderColor = Color.FromArgb(210, 210, 215);

            // Down button
            DrawMacOSButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawMacOSButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(60, 60, 67);

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

        private void DrawMacOSButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // macOS rounded button with gradient
            int radius = 5;
            using (var path = CreateRoundedPath(rect, radius))
            {
                // Gradient button
                using (var gradientBrush = new LinearGradientBrush(
                    rect,
                    bgColor,
                    Color.FromArgb(Math.Max(0, bgColor.R - 8), Math.Max(0, bgColor.G - 8), Math.Max(0, bgColor.B - 8)),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(gradientBrush, path);
                }

                // Inner highlight
                using (var highlightPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
                {
                    var highlightRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height / 2);
                    using (var highlightPath = CreateRoundedPath(highlightRect, radius - 1))
                    {
                        g.DrawPath(highlightPen, highlightPath);
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
            using (var font = new Font("SF Pro Text", 12f, FontStyle.Regular))
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
