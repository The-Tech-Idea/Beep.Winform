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
        private int _titleBottomSpacing = 4;

        private void RecalculateTitleLayout()
        {
            if (string.IsNullOrWhiteSpace(_titleText))
            {
                _titleHeight = 0;
                return;
            }

            var dpiScale = _renderOptions.DpiScale > 0 ? _renderOptions.DpiScale : DpiScalingHelper.GetDpiScaleFactor(this);
            int verticalPadding = DpiScalingHelper.ScaleValue(8, dpiScale);
            int minTitleHeight = DpiScalingHelper.ScaleValue(20, dpiScale);
            _titleBottomSpacing = DpiScalingHelper.ScaleValue(4, dpiScale);

            var fontToMeasure = _titleFont ?? _textFont ?? Font;
            using (var g = CreateGraphics())
            {
                var measured = TextRenderer.MeasureText(
                    g,
                    "Ag",
                    fontToMeasure,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

                _titleHeight = Math.Max(minTitleHeight, measured.Height + verticalPadding);
            }
        }

        private void UpdateChipBounds()
        {
            RecalculateTitleLayout();

            UpdateDrawingRect();
            SyncRenderOptions();
            _renderOptions.Font = _textFont ?? Font;
            _painter?.UpdateTheme(GetEffectiveTheme());

            var availableRect = DrawingRect;
            // ── horizontal inset: use chipPadding as left/right margin inside DrawingRect ──
            int hInset = _chipPadding;
            int chipStartX = availableRect.X + hInset;
            int chipEndX   = availableRect.Right - hInset;

            int x = chipStartX;
            int titleAreaHeight = _titleHeight > 0 ? _titleHeight + _titleBottomSpacing : 0;
            int utilityHeight = GetUtilityRowHeight();
            int utilityGap = utilityHeight > 0 ? DpiScalingHelper.ScaleValue(6, _renderOptions.DpiScale) : 0;
            int y = availableRect.Y + titleAreaHeight + utilityHeight + utilityGap;
            int maxHeightInRow = 0;
            int totalHeight = y;

            if (_chips == null || !_chips.Any())
            {
                SetupChipHitAreas();
                return;
            }

            using (var g = CreateGraphics())
            {
                for (int i = 0; i < _chips.Count; i++)
                {
                    var chip = _chips[i];
                    Size desired = _painter?.MeasureChip(chip.Item, g, _renderOptions) ?? new Size(60, 32);
                    int minTargetWidth = DpiScalingHelper.ScaleValue(44, _renderOptions.DpiScale);
                    int minTargetHeight = DpiScalingHelper.ScaleValue(28, _renderOptions.DpiScale);
                    desired = new Size(Math.Max(minTargetWidth, desired.Width), Math.Max(minTargetHeight, desired.Height));

                    // Prevent one oversized chip from forcing all subsequent chips into new rows.
                    int maxChipWidth = Math.Max(40, chipEndX - chipStartX);
                    if (desired.Width > maxChipWidth)
                        desired = new Size(maxChipWidth, desired.Height);

                    // Wrap to next row when chip would overflow the right inset boundary.
                    if (x + desired.Width > chipEndX && x > chipStartX)
                    {
                        x = chipStartX;   // reset to left inset, not DrawingRect.X
                        y += maxHeightInRow + _chipPadding;
                        maxHeightInRow = 0;
                    }

                    chip.Bounds = new Rectangle(x, y, desired.Width, desired.Height);
                    x += desired.Width + _chipPadding;
                    maxHeightInRow = Math.Max(maxHeightInRow, desired.Height);
                    totalHeight = y + maxHeightInRow;
                }
            }

            if (AutoSize)
            {
                int requiredHeight = (totalHeight - availableRect.Y) + Padding.Bottom;
                Size = new Size(Width, Math.Max(Height, requiredHeight + Padding.Vertical));
            }

            SetupChipHitAreas();
        }

        private int GetUtilityRowHeight()
        {
            if (!_showUtilityRow)
            {
                return 0;
            }

            return DpiScalingHelper.ScaleValue(24, _renderOptions.DpiScale > 0 ? _renderOptions.DpiScale : DpiScalingHelper.GetDpiScaleFactor(this));
        }
    }
}
