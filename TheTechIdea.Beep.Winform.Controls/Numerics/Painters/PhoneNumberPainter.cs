using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Phone number painter - displays phone with country code dropdown.
    /// Matches the "Phone number" control with country flag dropdown.
    /// Visual styling handled by BeepStyling.
    /// </summary>
    public class PhoneNumberPainter : BaseModernNumericPainter
    {
        private const int CountryCodeWidth = 80;
        private const int FlagWidth = 24;
        private const int PhoneSpacing = 8;

        public override NumericLayoutInfo CalculateLayout(INumericUpDownPainterContext context, Rectangle bounds)
        {
            var layout = new NumericLayoutInfo { ShowButtons = false };  // Phone uses country dropdown, not spin buttons

            // Layout: [flag dropdown +353] [phone number]
            
            // Country code area (left) - includes flag and code
            layout.CustomArea1 = new Rectangle(
                bounds.X + DefaultPadding,
                bounds.Y + DefaultPadding,
                CountryCodeWidth,
                bounds.Height - (DefaultPadding * 2)
            );

            // Phone number text area (right of country code)
            int textX = layout.CustomArea1.Right + PhoneSpacing;
            int textWidth = bounds.Width - textX - DefaultPadding;

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
            // Format phone number with dashes: 123-456-7890
            string phoneStr = context.Value.ToString("0");
            
            if (phoneStr.Length >= 10)
            {
                // Format as XXX-XXX-XXXX
                return $"{phoneStr.Substring(0, 3)}-{phoneStr.Substring(3, 3)}-{phoneStr.Substring(6)}";
            }
            else if (phoneStr.Length >= 6)
            {
                // Format as XXX-XXX...
                return $"{phoneStr.Substring(0, 3)}-{phoneStr.Substring(3)}";
            }

            return phoneStr;
        }

        public override void PaintButtonIcons(Graphics g, INumericUpDownPainterContext context,
            Rectangle upButtonRect, Rectangle downButtonRect)
        {
            // Phone doesn't use spin buttons, paint country code dropdown instead
            var layout = CalculateLayout(context, new Rectangle(0, 0, 100, 100));

            if (layout.CustomArea1 != Rectangle.Empty)
            {
                PaintCountryCodeDropdown(g, layout.CustomArea1, context);
            }
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context,
            Rectangle textRect, string formattedText)
        {
            if (string.IsNullOrEmpty(formattedText)) return;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color textColor = GetTextColor(context);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI", 10f, FontStyle.Regular))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(formattedText, font, textBrush, textRect, sf);
            }
        }

        private void PaintCountryCodeDropdown(Graphics g, Rectangle rect, INumericUpDownPainterContext context)
        {
            Color textColor = GetTextColor(context);
            Color borderColor = context.Theme?.BorderColor ?? Color.FromArgb(200, 200, 200);

            // Draw flag icon area - use a generic flag or phone icon
            Rectangle flagRect = new Rectangle(rect.X, rect.Y, FlagWidth, rect.Height);
            Color flagColor = GetIconColor(context, false, false);
            
            // Use a phone icon instead of trying to draw flag
            PaintSvgIcon(g, flagRect, SvgsUI.Phone, flagColor, 0.7f);

            // Draw country code text (+353)
            using (var font = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (var brush = new SolidBrush(textColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            })
            {
                Rectangle codeRect = new Rectangle(
                    flagRect.Right + 4,
                    rect.Y,
                    rect.Width - flagRect.Width - 20,
                    rect.Height
                );
                g.DrawString(context.Prefix ?? "+1", font, brush, codeRect, sf);
            }

            // Draw dropdown arrow using SVG
            Rectangle arrowRect = new Rectangle(rect.Right - 16, rect.Y, 16, rect.Height);
            Color arrowColor = GetIconColor(context, false, false);
            PaintSvgIcon(g, arrowRect, SvgsUI.ChevronDown, arrowColor, 0.7f);

            // Draw separator line on right
            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawLine(pen, rect.Right, rect.Y + 4, rect.Right, rect.Bottom - 4);
            }
        }

        private void PaintFlagIcon(Graphics g, Rectangle rect, INumericUpDownPainterContext context)
        {
            // This method is now obsolete - using PaintCountryCodeDropdown with phone icon instead
            // Kept for backward compatibility if needed
        }
    }
}
