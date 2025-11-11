using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Inline stepper painter - horizontal +/- buttons with text in between.
    /// Matches typography controls and width selectors with inline buttons.
    /// Visual styling handled by BeepStyling.
    /// </summary>
    public class InlineStepperPainter : BaseModernNumericPainter
    {
        private const int InlineButtonSize = 24;
        private const int InlineSpacing = 4;

        public override NumericLayoutInfo CalculateLayout(INumericUpDownPainterContext context, Rectangle bounds)
        {
            var layout = new NumericLayoutInfo { ShowButtons = context.ShowSpinButtons };

            if (!context.ShowSpinButtons)
            {
                layout.TextRect = new Rectangle(
                    bounds.X + DefaultPadding,
                    bounds.Y,
                    bounds.Width - (DefaultPadding * 2),
                    bounds.Height
                );
                return layout;
            }

            // Layout: [- button] [text] [+ button]
            int buttonY = bounds.Y + (bounds.Height - InlineButtonSize) / 2;

            // Down button (left side, minus)
            layout.DownButtonRect = new Rectangle(
                bounds.X + DefaultPadding,
                buttonY,
                InlineButtonSize,
                InlineButtonSize
            );

            // Up button (right side, plus)
            layout.UpButtonRect = new Rectangle(
                bounds.Right - InlineButtonSize - DefaultPadding,
                buttonY,
                InlineButtonSize,
                InlineButtonSize
            );

            // Text area (center, between buttons)
            int textX = layout.DownButtonRect.Right + InlineSpacing;
            int textWidth = layout.UpButtonRect.X - textX - InlineSpacing;

            layout.TextRect = new Rectangle(
                textX,
                bounds.Y,
                textWidth,
                bounds.Height
            );

            return layout;
        }

        public override void PaintButtonIcons(Graphics g, INumericUpDownPainterContext context,
            Rectangle upButtonRect, Rectangle downButtonRect)
        {
            if (upButtonRect == Rectangle.Empty || downButtonRect == Rectangle.Empty)
                return;

            // Get icon colors based on state
            Color upColor = GetIconColor(context, context.UpButtonHovered, context.UpButtonPressed);
            Color downColor = GetIconColor(context, context.DownButtonHovered, context.DownButtonPressed);

            // Paint plus icon on up button
            PaintPlusMinusIcon(g, upButtonRect, true, upColor);

            // Paint minus icon on down button
            PaintPlusMinusIcon(g, downButtonRect, false, downColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context,
            Rectangle textRect, string formattedText)
        {
            if (string.IsNullOrEmpty(formattedText)) return;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color textColor = GetTextColor(context);

            using (var textBrush = new SolidBrush(textColor))
            using (var font = GetFont(context))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,  // Centered text for inline style
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(formattedText, font, textBrush, textRect, sf);
            }
        }
    }
}
