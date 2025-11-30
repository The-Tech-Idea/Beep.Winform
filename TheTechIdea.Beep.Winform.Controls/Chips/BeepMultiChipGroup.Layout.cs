using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    public partial class BeepMultiChipGroup
    {
        // _focusedIndex is defined in BeepMultiChipGroup.Properties.cs

        private void UpdateChipBounds()
        {
            if (_chips == null || !_chips.Any()) return;

            UpdateDrawingRect();
            _renderOptions.Font = this.Font;
            _renderOptions.Theme = _currentTheme;
            _renderOptions.CornerRadius = _chipCornerRadius;
            _renderOptions.Gap = _chipPadding;
            _renderOptions.ShowBorders = _showChipBorders;
            _renderOptions.BorderWidth = _chipBorderWidth;
            _renderOptions.Size = _chipSize;
            _renderOptions.Style = _chipStyle;
            _painter?.UpdateTheme(_currentTheme);

            var availableRect = DrawingRect;
            int x = availableRect.X;
            int y = availableRect.Y + _titleHeight;
            int maxHeightInRow = 0;
            int totalHeight = y;

            for (int i = 0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                Size desired;
                using (var g = CreateGraphics())
                {
                    desired = _painter?.MeasureChip(chip.Item, g, _renderOptions) ?? new Size(60, 32);
                }

                if (x + desired.Width + _chipPadding > availableRect.Right && x > availableRect.X)
                {
                    x = availableRect.X;
                    y += maxHeightInRow + _chipPadding;
                    maxHeightInRow = 0;
                }

                chip.Bounds = new Rectangle(x, y, desired.Width, desired.Height);
                x += desired.Width + _chipPadding;
                maxHeightInRow = Math.Max(maxHeightInRow, desired.Height);
                totalHeight = y + maxHeightInRow;
            }

            if (AutoSize)
            {
                int requiredHeight = totalHeight - availableRect.Y + Padding.Bottom + _titleHeight;
                Size = new Size(Width, Math.Max(Height, requiredHeight + Padding.Vertical));
            }

            SetupChipHitAreas();
        }
    }
}
