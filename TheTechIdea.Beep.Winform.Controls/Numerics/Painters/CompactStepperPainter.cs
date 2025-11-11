using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Compact stepper painter - vertical up/down arrows on the right side.
    /// Matches the "Quantity" and "Width" controls in modern UI designs.
    /// Visual styling handled by BeepStyling.
    /// </summary>
    public class CompactStepperPainter : BaseModernNumericPainter
    {
        private const int CompactButtonWidth = 20;
        private const int CompactButtonHeight = 16;
        private const int ButtonSpacing = 2;

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

            // Compact vertical buttons on the right
            int totalButtonWidth = CompactButtonWidth + DefaultPadding;
            int buttonAreaHeight = (CompactButtonHeight * 2) + ButtonSpacing;
            int buttonY = bounds.Y + (bounds.Height - buttonAreaHeight) / 2;

            // Up button (top)
            layout.UpButtonRect = new Rectangle(
                bounds.Right - totalButtonWidth,
                buttonY,
                CompactButtonWidth,
                CompactButtonHeight
            );

            // Down button (bottom)
            layout.DownButtonRect = new Rectangle(
                bounds.Right - totalButtonWidth,
                buttonY + CompactButtonHeight + ButtonSpacing,
                CompactButtonWidth,
                CompactButtonHeight
            );

            // Text area (left of buttons)
            layout.TextRect = new Rectangle(
                bounds.X + DefaultPadding,
                bounds.Y,
                bounds.Width - totalButtonWidth - DefaultPadding,
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

            // Paint up arrow
            PaintArrowIcon(g, upButtonRect, true, upColor);

            // Paint down arrow
            PaintArrowIcon(g, downButtonRect, false, downColor);
        }

        public override string FormatValue(INumericUpDownPainterContext context)
        {
            // For compact stepper, format based on NumericStyle
            return context.NumericStyle switch
            {
                NumericStyle.Integer => context.Value.ToString("N0"),
                NumericStyle.Decimal => context.Value.ToString($"N{context.DecimalPlaces}"),
                NumericStyle.Percentage => $"{context.Value:N1}%",
                _ => base.FormatValue(context)
            };
        }
    }
}
