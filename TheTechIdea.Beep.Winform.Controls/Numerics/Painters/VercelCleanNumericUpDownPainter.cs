using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Vercel Clean Style painter for NumericUpDown
    /// Features: High contrast, bold borders, sharp corners, monochrome geometric design
    /// </summary>
    public class VercelCleanNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Vercel white background with strong contrast
            Color backColor = context.UseThemeColors && theme != null
                ? theme.BackgroundColor
                : Color.White;

            Color vercelBlack = context.UseThemeColors && theme != null
                ? theme.PrimaryColor
                : Color.FromArgb(0, 0, 0);

            Color borderColor = context.IsFocused
                ? vercelBlack
                : Color.FromArgb(234, 234, 234);

            // Draw background with minimal 4px radius (Vercel Style)
            int radius = context.IsRounded ? Math.Min(context.BorderRadius, 4) : 4;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Strong border
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
                var downRect = new Rectangle(bounds.X + 1, bounds.Y + 1, buttonWidth - 1, bounds.Height - 2);
                var upRect = new Rectangle(bounds.Right - buttonWidth, bounds.Y + 1, buttonWidth - 1, bounds.Height - 2);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            Color buttonColor = Color.White;
            Color buttonHoverColor = Color.FromArgb(250, 250, 250);
            Color buttonPressedColor = Color.FromArgb(245, 245, 245);
            Color iconColor = Color.FromArgb(0, 0, 0); // Pure black
            Color borderColor = Color.FromArgb(234, 234, 234);

            // Down button
            DrawVercelButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawVercelButton(g, upButtonRect, "+", 
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
            using (var font = new Font("Inter", 10f, FontStyle.Regular)) // Vercel uses Inter
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                // Fallback to Segoe UI if Inter not available
                Font actualFont;
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

        private void DrawVercelButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Vercel clean button with minimal radius
            int radius = 3;
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Strong separator border
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

            // Button icon with high contrast
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe UI", 11f, FontStyle.Bold)) // Bold for Vercel Style
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
