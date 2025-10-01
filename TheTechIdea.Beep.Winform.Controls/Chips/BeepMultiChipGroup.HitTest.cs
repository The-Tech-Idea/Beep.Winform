using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    public partial class BeepMultiChipGroup
    {
        private bool _showUtilityRow = true;
        private Rectangle _selectAllRect;
        private Rectangle _clearAllRect;

        private void SetupChipHitAreas()
        {
            ClearHitList();

            int y = DrawingRect.Y + _titleHeight;
            int x = DrawingRect.X;

            if (_showUtilityRow)
            {
                Size sa = TextRenderer.MeasureText("Select All", Font);
                _selectAllRect = new Rectangle(x, y - 26, sa.Width + 16, 22);
                AddHitArea("Chip_SelectAll", _selectAllRect, null, () => { SelectAllUtility(); });

                Size ca = TextRenderer.MeasureText("Clear All", Font);
                _clearAllRect = new Rectangle(_selectAllRect.Right + 8, y - 26, ca.Width + 16, 22);
                AddHitArea("Chip_ClearAll", _clearAllRect, null, () => { ClearAllUtility(); });
            }

            for (int i = 0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                if (chip.Bounds.Width > 0 && chip.Bounds.Height > 0)
                {
                    int chipIndex = i;
                    AddHitArea($"Chip_{chipIndex}_{chip.Item.GuidId}", chip.Bounds, null, () => HandleChipClick(chip));
                }
            }
        }

        private void SelectAllUtility()
        {
            if (SelectionMode == ChipSelectionMode.Single)
            {
                if (_chips.Count > 0)
                {
                    foreach (var c in _chips) c.IsSelected = false;
                    _chips[0].IsSelected = true;
                    _selectedItems.Clear();
                    _selectedItems.Add(_chips[0].Item);
                }
            }
            else
            {
                foreach (var c in _chips) c.IsSelected = true;
                _selectedItems.Clear();
                foreach (var c in _chips) _selectedItems.Add(c.Item);
            }
            Invalidate();
        }

        private void ClearAllUtility()
        {
            foreach (var c in _chips) c.IsSelected = false;
            _selectedItems.Clear();
            _selectedItem = null;
            Invalidate();
        }

        private void HandleChipClick(ChipItem chip)
        {
            if (chip?.Item == null) return;

            var idx = _chips.IndexOf(chip);
            if (idx >= 0 && _closeRects.TryGetValue(idx, out var closeRect))
            {
                var mouse = PointToClient(MousePosition);
                if (closeRect.Contains(mouse))
                {
                    chip.IsSelected = false;
                    _selectedItems.Remove(chip.Item);
                    _selectedItem = _selectedItems.FirstOrDefault();
                    OnSelectedItemChanged(_selectedItem);
                    Invalidate();
                    return;
                }
            }

            switch (_selectionMode)
            {
                case ChipSelectionMode.Single:
                    foreach (var c in _chips) c.IsSelected = false;
                    chip.IsSelected = true;
                    _selectedItem = chip.Item;
                    _selectedItems.Clear();
                    _selectedItems.Add(chip.Item);
                    break;
                case ChipSelectionMode.Multiple:
                    chip.IsSelected = !chip.IsSelected;
                    if (chip.IsSelected)
                    {
                        if (!_selectedItems.Contains(chip.Item)) _selectedItems.Add(chip.Item);
                    }
                    else
                    {
                        _selectedItems.Remove(chip.Item);
                    }
                    _selectedItem = _selectedItems.FirstOrDefault();
                    break;
                case ChipSelectionMode.Toggle:
                    chip.IsSelected = !chip.IsSelected;
                    if (chip.IsSelected)
                    {
                        _selectedItem = chip.Item;
                        _selectedItems.Clear();
                        _selectedItems.Add(chip.Item);
                    }
                    else
                    {
                        _selectedItem = null;
                        _selectedItems.Clear();
                    }
                    break;
            }

            OnSelectedItemChanged(_selectedItem);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            bool clickedOnChip = _chips.Any(chip => chip.Bounds.Contains(e.Location));
            if (!clickedOnChip)
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateHoverStates(e.Location);
        }

        private void UpdateHoverStates(Point mouseLocation)
        {
            bool needsRedraw = false;
            foreach (var chip in _chips)
            {
                bool wasHovered = chip.IsHovered;
                chip.IsHovered = chip.Bounds.Contains(mouseLocation);
                if (wasHovered != chip.IsHovered) needsRedraw = true;
            }
            if (needsRedraw) Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool needsRedraw = false;
            foreach (var chip in _chips)
            {
                if (chip.IsHovered) { chip.IsHovered = false; needsRedraw = true; }
            }
            if (needsRedraw) Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab || keyData == (Keys.Shift | Keys.Tab))
            {
                // let base handle tab focus switching
                return base.ProcessCmdKey(ref msg, keyData);
            }

            if (_chips.Count == 0) return base.ProcessCmdKey(ref msg, keyData);

            bool handled = false;
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Down:
                    _focusedIndex = (_focusedIndex + 1 + _chips.Count) % _chips.Count; handled = true; break;
                case Keys.Left:
                case Keys.Up:
                    _focusedIndex = (_focusedIndex - 1 + _chips.Count) % _chips.Count; handled = true; break;
                case Keys.Space:
                case Keys.Enter:
                    if (_focusedIndex >= 0 && _focusedIndex < _chips.Count)
                    {
                        HandleChipClick(_chips[_focusedIndex]);
                        handled = true;
                    }
                    break;
            }
            if (handled) { Invalidate(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
