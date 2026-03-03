using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    internal static class StepperLayoutHelper
    {
        public static Rectangle ComputeContentBounds(Rectangle bounds, bool reserveBottomArea, int reservedHeight, int padding = 0)
        {
            var padded = Rectangle.Inflate(bounds, -padding, -padding);
            if (!reserveBottomArea || reservedHeight <= 0)
            {
                return padded;
            }

            int finalHeight = System.Math.Max(0, padded.Height - reservedHeight);
            return new Rectangle(padded.Left, padded.Top, padded.Width, finalHeight);
        }

        public static Point ComputeCenteredStartPoint(
            Rectangle bounds,
            Orientation orientation,
            Size stepSize,
            int stepCount,
            int spacing)
        {
            int stepTotalSize = orientation == Orientation.Horizontal ? stepSize.Width : stepSize.Height;
            int totalLength = (stepTotalSize + spacing) * stepCount - spacing;

            return orientation == Orientation.Horizontal
                ? new Point(bounds.Left + (bounds.Width - totalLength) / 2, bounds.Top + (bounds.Height - stepSize.Height) / 2)
                : new Point(bounds.Left + (bounds.Width - stepSize.Width) / 2, bounds.Top + (bounds.Height - totalLength) / 2);
        }
    }
}
