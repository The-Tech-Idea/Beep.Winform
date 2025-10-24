using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Notion Minimal Style painter for NumericUpDown
    /// Features: Subtle grays, minimal borders, clean spacing, workspace aesthetic
    /// </summary>
    public class NotionMinimalNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Notion subtle background
            Color backColor = Color.FromArgb(251, 251, 250); // Notion warm white
            Color borderColor = context.IsFocused
                ? Color.FromArgb(55, 53, 47) // Notion dark gray
                : Color.FromArgb(233, 233, 231); // Notion light gray

            // Draw background with minimal 3px radius
            int radius = context.IsRounded ? Math.Min(context.BorderRadius, 3) : 3;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Very subtle border
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
                var downRect = new Rectangle(bounds.X + 1, bounds.Y + 1, buttonWidth - 1, bounds.Height - 2);
                var upRect = new Rectangle(bounds.Right - buttonWidth, bounds.Y + 1, buttonWidth - 1, bounds.Height - 2);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            Color buttonColor = Color.FromArgb(251, 251, 250); // Same as background
            Color buttonHoverColor = Color.FromArgb(243, 243, 242); // Notion subtle hover
            Color buttonPressedColor = Color.FromArgb(237, 237, 235);
            Color iconColor = Color.FromArgb(120, 119, 116); // Notion medium gray
            Color borderColor = Color.FromArgb(233, 233, 231);

            // Down button
            DrawNotionButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawNotionButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            var theme = context.Theme;
            Color textColor = context.UseThemeColors && theme != null
                ? theme.TextBoxForeColor
                : Color.FromArgb(55, 53, 47);

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

        private void DrawNotionButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Notion minimal button
            int radius = 2; // Very subtle
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle separator only on hover/press
                if (hovered || pressed)
                {
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
            }

            // Button icon
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
