using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Standard numeric painter - traditional layout with buttons on left and right.
    /// Default painter for NumericStyle.Standard.
    /// Visual styling handled by BeepStyling.
    /// </summary>
    public class StandardNumericPainter : BaseModernNumericPainter
    {
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

            int buttonWidth = GetButtonWidth(context);

            // Down button (left side)
            layout.DownButtonRect = new Rectangle(
                bounds.X + 2,
                bounds.Y + 2,
                buttonWidth - 4,
                bounds.Height - 4
            );

            // Up button (right side)
            layout.UpButtonRect = new Rectangle(
                bounds.Right - buttonWidth + 2,
                bounds.Y + 2,
                buttonWidth - 4,
                bounds.Height - 4
            );

            // Text area (center, between buttons)
            int textX = layout.DownButtonRect.Right + DefaultPadding;
            int textWidth = layout.UpButtonRect.X - textX - DefaultPadding;

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

            // Paint minus icon on down button
            PaintPlusMinusIcon(g, downButtonRect, false, downColor);

            // Paint plus icon on up button
            PaintPlusMinusIcon(g, upButtonRect, true, upColor);
        }

        private int GetButtonWidth(INumericUpDownPainterContext context)
        {
            return context.ButtonSize switch
            {
                NumericSpinButtonSize.Small => 20,
                NumericSpinButtonSize.Standard => 24,
                NumericSpinButtonSize.Large => 28,
                NumericSpinButtonSize.ExtraLarge => 32,
                _ => 24
            };
        }
    }
}
