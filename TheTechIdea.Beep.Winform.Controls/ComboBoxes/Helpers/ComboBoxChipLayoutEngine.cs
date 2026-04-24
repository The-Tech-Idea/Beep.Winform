using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    internal static class ComboBoxChipLayoutEngine
    {
        public static IReadOnlyList<ChipLayoutItem> Compute(
            Rectangle availableBounds,
            IReadOnlyList<SimpleItem> chips,
            Graphics g,
            Font font,
            BeepComboBox owner,
            bool singleLineCollapse,
            int maxRows = 2,
            int chipPaddingXLogical = 8,
            int chipPaddingYLogical = 2,
            int gapLogical = 4,
            int closeSizeLogical = 16)
        {
            if (chips == null || chips.Count == 0 || availableBounds.Width <= 0 || availableBounds.Height <= 0)
            {
                return Array.Empty<ChipLayoutItem>();
            }

            int scaleX(int v) => owner?.ScaleLogicalX(v) ?? v;
            int scaleY(int v) => owner?.ScaleLogicalY(v) ?? v;

            int gap = Math.Max(1, scaleX(gapLogical));
            int hPad = Math.Max(1, scaleX(chipPaddingXLogical));
            int vPad = Math.Max(1, scaleY(chipPaddingYLogical));
            int closeSize = Math.Max(8, scaleX(closeSizeLogical));

            int chipHeight = Math.Max(14, Math.Min(availableBounds.Height - (gap * 2), font.Height + (vPad * 2) + scaleY(4)));
            int rowHeight = chipHeight + gap;
            int maxBottom = availableBounds.Y + (rowHeight * Math.Max(1, maxRows));

            var items = new List<ChipLayoutItem>(chips.Count + 1);
            int currentX = availableBounds.Left;
            int currentY = availableBounds.Top + Math.Max(0, (availableBounds.Height - chipHeight) / 2);
            int overflowAt = -1;

            for (int i = 0; i < chips.Count; i++)
            {
                var source = chips[i];
                string text = source?.Text ?? string.Empty;
                int textWidth = TextRenderer.MeasureText(g, text, font).Width;
                int chipWidth = textWidth + (hPad * 2) + closeSize + gap;

                if (singleLineCollapse)
                {
                    if (currentX + chipWidth > availableBounds.Right && i > 0)
                    {
                        overflowAt = i;
                        break;
                    }
                }
                else
                {
                    if (currentX + chipWidth > availableBounds.Right && currentX > availableBounds.Left)
                    {
                        currentX = availableBounds.Left;
                        currentY += chipHeight + gap;
                        if (currentY + chipHeight > maxBottom)
                        {
                            overflowAt = i;
                            break;
                        }
                    }
                }

                int width = Math.Min(chipWidth, Math.Max(1, availableBounds.Right - currentX));
                var chipRect = new Rectangle(currentX, currentY, width, chipHeight);
                var closeRect = new Rectangle(
                    chipRect.Right - closeSize - scaleX(4),
                    chipRect.Y + (chipRect.Height - closeSize) / 2,
                    closeSize,
                    closeSize);

                items.Add(new ChipLayoutItem
                {
                    Item = source,
                    ChipRect = chipRect,
                    CloseButtonRect = closeRect,
                    IsOverflow = false,
                    OverflowCount = 0
                });

                currentX += chipWidth + gap;
            }

            if (overflowAt >= 0)
            {
                int remaining = chips.Count - overflowAt;
                string overflowText = $"+{remaining}";
                int overflowWidth = TextRenderer.MeasureText(g, overflowText, font).Width + (hPad * 2);
                int width = Math.Min(overflowWidth, Math.Max(1, availableBounds.Right - currentX));

                items.Add(new ChipLayoutItem
                {
                    Item = null,
                    ChipRect = new Rectangle(currentX, currentY, width, chipHeight),
                    CloseButtonRect = Rectangle.Empty,
                    IsOverflow = true,
                    OverflowCount = remaining
                });
            }

            return items;
        }
    }
}
