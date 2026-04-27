using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBreadCrumb
    {
        private void DrawLegacyChevronLayout(Graphics graphics)
        {
            int count = ListItems.Count;
            int totalLen = orientation == System.Windows.Forms.Orientation.Horizontal
                ? DrawingRect.Width
                : DrawingRect.Height;
            int crossLen = orientation == System.Windows.Forms.Orientation.Horizontal
                ? DrawingRect.Height
                : DrawingRect.Width;
            int stepLen = totalLen / System.Math.Max(1, count);
            int arrowSize = crossLen / 4;

            for (int i = 0; i < count; i++)
            {
                int x = orientation == System.Windows.Forms.Orientation.Horizontal
                    ? DrawingRect.Left + (i * stepLen)
                    : DrawingRect.Left;
                int y = orientation == System.Windows.Forms.Orientation.Horizontal
                    ? DrawingRect.Top
                    : DrawingRect.Top + (i * stepLen);

                DrawLegacyChevronStep(graphics, i, count, x, y, stepLen, crossLen, arrowSize);
            }

            RegisterChevronHitAreas();
        }

        private void DrawLegacyChevronText(Graphics graphics, int stepIndex, int x, int y, int stepLen, int crossLen)
        {
            if (orientation != System.Windows.Forms.Orientation.Horizontal || stepIndex < 0 || stepIndex >= ListItems.Count)
            {
                return;
            }

            if (!ShouldShowStepLabel(stepIndex))
            {
                return;
            }

            var headerText = ListItems[stepIndex].Name ?? string.Empty;
            var subText = ListItems[stepIndex].Text ?? string.Empty;
            StepState state = stepIndex == selectedIndex ? StepState.Active : StepState.Pending;
            Font headerFont = StepperFontHelpers.GetStepLabelFont(this, ControlStyle, state, _textFont, this);
            Font subFont = StepperFontHelpers.GetStepTextFont(this, ControlStyle, _textFont, this);

            var headerSize = TextUtils.MeasureText(graphics, headerText, headerFont);
            var subSize = TextUtils.MeasureText(graphics, subText, subFont);
            float totalTextHeight = headerSize.Height + subSize.Height;
            float startY = y + (crossLen - totalTextHeight) / 2f;
            float headerX = x + (stepLen - headerSize.Width) / 2f;
            float subX = x + (stepLen - subSize.Width) / 2f;

            Color foreColor = StepperThemeHelpers.GetStepLabelColor(_currentTheme, UseThemeColors, state);
            if (StepperAccessibilityHelpers.IsHighContrastMode())
            {
                var (_, _, _, _, _, highContrastTextColor, _) = StepperAccessibilityHelpers.GetHighContrastColors();
                foreColor = highContrastTextColor;
            }
            else
            {
                foreColor = StepperAccessibilityHelpers.AdjustForContrast(foreColor, BackColor);
            }

            using var brush = new SolidBrush(foreColor);
            graphics.DrawString(headerText, headerFont, brush, headerX, startY);
            graphics.DrawString(subText, subFont, brush, subX, startY + headerSize.Height);
        }

        private Point[] GetChevronPolygonPoints(int index, int count, int x, int y, int stepLen, int crossLen, int arrowSize)
        {
            if (orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (index == 0)
                {
                    return new[]
                    {
                        new Point(x, y),
                        new Point(x + stepLen - arrowSize, y),
                        new Point(x + stepLen, y + crossLen / 2),
                        new Point(x + stepLen - arrowSize, y + crossLen),
                        new Point(x, y + crossLen),
                        new Point(x, y)
                    };
                }

                if (index == count - 1)
                {
                    return new[]
                    {
                        new Point(x, y),
                        new Point(x + stepLen, y),
                        new Point(x + stepLen, y + crossLen),
                        new Point(x, y + crossLen),
                        new Point(x + arrowSize, y + crossLen / 2),
                        new Point(x, y)
                    };
                }

                return new[]
                {
                    new Point(x, y),
                    new Point(x + stepLen - arrowSize, y),
                    new Point(x + stepLen, y + crossLen / 2),
                    new Point(x + stepLen - arrowSize, y + crossLen),
                    new Point(x, y + crossLen),
                    new Point(x + arrowSize, y + crossLen / 2),
                    new Point(x, y)
                };
            }

            if (index == 0)
            {
                return new[]
                {
                    new Point(x, y),
                    new Point(x + crossLen, y),
                    new Point(x + crossLen / 2, y + stepLen),
                    new Point(x, y + stepLen)
                };
            }

            if (index == count - 1)
            {
                return new[]
                {
                    new Point(x + crossLen / 2, y),
                    new Point(x + crossLen, y + stepLen - arrowSize),
                    new Point(x + crossLen, y + stepLen),
                    new Point(x, y + stepLen)
                };
            }

            return new[]
            {
                new Point(x + crossLen / 2, y),
                new Point(x + crossLen, y + arrowSize),
                new Point(x + crossLen, y + stepLen - arrowSize),
                new Point(x + crossLen / 2, y + stepLen),
                new Point(x, y + stepLen - arrowSize),
                new Point(x, y + arrowSize)
            };
        }
    }
}
