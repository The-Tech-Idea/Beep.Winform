using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Currency painter - displays value with currency symbol and dropdown.
    /// Matches the "Wallet balance" control with USD dropdown.
    /// Visual styling handled by BeepStyling.
    /// </summary>
    public class CurrencyPainter : BaseModernNumericPainter
    {
        private const int CurrencyIconWidth = 32;
        private const int DropdownWidth = 60;
        private const int CurrencySpacing = 8;

        public override NumericLayoutInfo CalculateLayout(INumericUpDownPainterContext context, Rectangle bounds)
        {
            var layout = new NumericLayoutInfo { ShowButtons = false };  // Currency uses dropdown, not spin buttons

            // Layout: [icon] [value] [USD dropdown]
            
            // Currency icon area (left)
            layout.CustomArea1 = new Rectangle(
                bounds.X + DefaultPadding,
                bounds.Y,
                CurrencyIconWidth,
                bounds.Height
            );

            // Dropdown area (right)
            layout.CustomArea2 = new Rectangle(
                bounds.Right - DropdownWidth - DefaultPadding,
                bounds.Y + DefaultPadding,
                DropdownWidth,
                bounds.Height - (DefaultPadding * 2)
            );

            // Value text area (center)
            int textX = layout.CustomArea1.Right + CurrencySpacing;
            int textWidth = layout.CustomArea2.X - textX - CurrencySpacing;

            layout.TextRect = new Rectangle(
                textX,
                bounds.Y,
                textWidth,
                bounds.Height
            );

            return layout;
        }

        public override string FormatValue(INumericUpDownPainterContext context)
        {
            // Format currency with thousands separator and 2 decimal places
            string valueStr = context.Value.ToString("N2");
            
            // Split into whole and decimal parts for better formatting
            var parts = valueStr.Split('.');
            if (parts.Length == 2)
            {
                return $"{parts[0]}.{parts[1]}";
            }

            return valueStr;
        }

        public override void PaintButtonIcons(Graphics g, INumericUpDownPainterContext context,
            Rectangle upButtonRect, Rectangle downButtonRect)
        {
            // Currency doesn't use spin buttons, but we paint the currency icon and dropdown indicator
            var layout = CalculateLayout(context, new Rectangle(0, 0, 100, 100));  // Get layout for areas

            // Paint currency icon (wallet/money icon) in CustomArea1
            if (layout.CustomArea1 != Rectangle.Empty)
            {
                PaintCurrencyIcon(g, layout.CustomArea1, context);
            }

            // Paint dropdown indicator in CustomArea2
            if (layout.CustomArea2 != Rectangle.Empty)
            {
                PaintDropdownIndicator(g, layout.CustomArea2, context);
            }
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context,
            Rectangle textRect, string formattedText)
        {
            if (string.IsNullOrEmpty(formattedText)) return;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color textColor = GetTextColor(context);
            Color decimalColor = context.Theme?.SecondaryTextColor ?? Color.FromArgb(150, 150, 150);

            using (var font = new Font("Segoe UI", 11f, FontStyle.Bold))
            using (var decimalFont = new Font("Segoe UI", 9f, FontStyle.Regular))
            {
                // Split value into whole and decimal parts
                var parts = formattedText.Split('.');
                
                if (parts.Length == 2)
                {
                    // Measure whole part
                    SizeF wholeSize = g.MeasureString(parts[0], font);
                    
                    // Draw whole part
                    using (var wholeBrush = new SolidBrush(textColor))
                    {
                        g.DrawString(parts[0], font, wholeBrush,
                            new PointF(textRect.X, textRect.Y + (textRect.Height - wholeSize.Height) / 2));
                    }

                    // Draw decimal part (smaller, lighter)
                    using (var decimalBrush = new SolidBrush(decimalColor))
                    {
                        g.DrawString($".{parts[1]}", decimalFont, decimalBrush,
                            new PointF(textRect.X + wholeSize.Width, textRect.Y + (textRect.Height - wholeSize.Height) / 2 + 2));
                    }
                }
                else
                {
                    // Draw normally if no decimal
                    base.PaintValueText(g, context, textRect, formattedText);
                }
            }
        }

        private void PaintCurrencyIcon(Graphics g, Rectangle rect, INumericUpDownPainterContext context)
        {
            Color iconColor = GetIconColor(context, false, false);

            // Use credit card SVG icon from SvgsUI for currency
            string currencySvg = SvgsUI.CreditCard;

            // Paint the SVG icon with tint
            PaintSvgIcon(g, rect, currencySvg, iconColor, 0.8f);
        }

        private void PaintDropdownIndicator(Graphics g, Rectangle rect, INumericUpDownPainterContext context)
        {
            Color textColor = GetTextColor(context);

            using (var font = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            })
            {
                // Draw "USD" or currency code
                Rectangle textRect = new Rectangle(rect.X, rect.Y, rect.Width - 20, rect.Height);
                g.DrawString("USD", font, brush, textRect, sf);

                // Draw dropdown arrow using SVG
                Rectangle arrowRect = new Rectangle(rect.Right - 16, rect.Y, 16, rect.Height);
                Color arrowColor = GetIconColor(context, false, false);
                PaintSvgIcon(g, arrowRect, SvgsUI.ChevronDown, arrowColor, 0.7f);
            }
        }
    }
}
