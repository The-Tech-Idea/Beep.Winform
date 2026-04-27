using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    public partial class BeepMultiChipGroup
    {
        private Rectangle _selectAllRect;
        private Rectangle _clearAllRect;
        private Point _lastInteractionPoint;

        private void SetupChipHitAreas()
        {
            ClearHitList();

            int titleAreaHeight = _titleHeight > 0 ? _titleHeight + _titleBottomSpacing : 0;
            int y = DrawingRect.Y + titleAreaHeight;
            // align utility row to the same horizontal inset as the chip grid
            int x = DrawingRect.X + _chipPadding;

            if (_showUtilityRow)
            {
                int utilityHeight = GetUtilityRowHeight();
                Size sa = TextRenderer.MeasureText("Select All", Font);
                _selectAllRect = new Rectangle(x, y, sa.Width + 16, utilityHeight);
                AddHitArea("Chip_SelectAll", _selectAllRect, null, () => { SelectAllUtility(); });

                Size ca = TextRenderer.MeasureText("Clear All", Font);
                _clearAllRect = new Rectangle(_selectAllRect.Right + 8, y, ca.Width + 16, utilityHeight);
                AddHitArea("Chip_ClearAll", _clearAllRect, null, () => { ClearAllUtility(); });
                y += utilityHeight + DpiScalingHelper.ScaleValue(6, _renderOptions.DpiScale);
            }
            else
            {
                _selectAllRect = Rectangle.Empty;
                _clearAllRect = Rectangle.Empty;
            }

            for (int i = 0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                if (chip.Bounds.Width > 0 && chip.Bounds.Height > 0)
                {
                    int chipIndex = i;
                    AddHitArea($"Chip_{chipIndex}_{chip.Item.GuidId}", chip.Bounds, null, () => HandleChipClick(chip, _lastInteractionPoint, false));
                }
            }
        }

        private void SelectAllUtility()
        {
            var previousSelected = _selectedItem;
            if (SelectionMode == ChipSelectionMode.Single)
            {
                if (_chips.Count > 0)
                {
                    foreach (var c in _chips) c.IsSelected = false;
                    _chips[0].IsSelected = true;
                    _selectedItems.Clear();
                    _selectedItems.Add(_chips[0].Item);
                    _selectedItem = _chips[0].Item;
                }
            }
            else
            {
                foreach (var c in _chips) c.IsSelected = true;
                _selectedItems.Clear();
                foreach (var c in _chips) _selectedItems.Add(c.Item);
                _selectedItem = _selectedItems.FirstOrDefault();
            }
            OnSelectedItemChanged(_selectedItem);
            Invalidate();
        }

        private void ClearAllUtility()
        {
            var previousSelected = _selectedItem;
            foreach (var c in _chips) c.IsSelected = false;
            _selectedItems.Clear();
            _selectedItem = null;
            OnSelectedItemChanged(null);
            Invalidate();
        }

        private void HandleChipClick(ChipItem chip, Point clickLocation, bool keyboardAction)
        {
            if (chip?.Item == null) return;
            if (chip.IsDisabled) return;

            var idx = _chips.IndexOf(chip);
            if (!keyboardAction && idx >= 0 && _closeRects.TryGetValue(idx, out var closeRect))
            {
                if (closeRect.Contains(clickLocation))
                {
                    RemoveChip(idx);
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

        // ── Ripple animation trigger ──────────────────────────────────
        private void TriggerRipple(ChipItem chip, Point clickPoint)
        {
            if (!_enableRipple || chip.IsDisabled) return;

            var theme = GetEffectiveTheme();
            var rippleColor = theme?.FocusIndicatorColor ?? Color.FromArgb(60, Color.Black);

            _activeRipples.Add(new ChipRipple
            {
                Bounds = chip.Bounds,
                Center = new Point(clickPoint.X - chip.Bounds.X, clickPoint.Y - chip.Bounds.Y),
                Progress = 0f,
                Color = rippleColor
            });

            if (!_rippleTimer.Enabled)
                _rippleTimer.Start();
        }

        private void RippleTimer_Tick(object? sender, EventArgs e)
        {
            bool needsRedraw = false;
            for (int i = _activeRipples.Count - 1; i >= 0; i--)
            {
                var ripple = _activeRipples[i];
                ripple.Progress += 0.06f;
                if (ripple.Progress >= 1f)
                {
                    _activeRipples.RemoveAt(i);
                }
                needsRedraw = true;
            }
            if (needsRedraw) Invalidate();
            if (_activeRipples.Count == 0) _rippleTimer.Stop();
        }

        // ── Tooltip support ───────────────────────────────────────────
        private void StartTooltipTimer(int chipIndex)
        {
            if (!_showTooltip || chipIndex < 0 || chipIndex >= _chips.Count) return;
            _tooltipChipIndex = chipIndex;
            _tooltipShowing = false;
            _tooltipTimer.Stop();
            _tooltipTimer.Start();
        }

        private void HideTooltip()
        {
            _tooltipTimer?.Stop();
            if (_tooltipShowing)
            {
                _tooltipShowing = false;
                _tooltipChipIndex = -1;
                Invalidate();
            }
            else
            {
                _tooltipChipIndex = -1;
            }
        }

        private void TooltipTimer_Tick(object? sender, EventArgs e)
        {
            _tooltipTimer.Stop();
            if (_tooltipChipIndex >= 0 && _tooltipChipIndex < _chips.Count)
            {
                var chip = _chips[_tooltipChipIndex];
                if (chip.IsHovered && chip.Bounds.Width > 0)
                {
                    // Check if text is actually truncated
                    using var g = CreateGraphics();
                    var textWidth = TextRenderer.MeasureText(g, chip.Item.Text, _textFont ?? Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine).Width;
                    if (textWidth > chip.Bounds.Width - 20)
                    {
                        _tooltipShowing = true;
                        Invalidate();
                    }
                }
            }
        }

        // ── Add/remove animation ──────────────────────────────────────
        private void AnimateChipAdd(int chipIndex)
        {
            if (!_enableAddRemoveAnimation || chipIndex < 0 || chipIndex >= _chips.Count) return;
            _chipAnimations.Add(new ChipAnimation
            {
                ChipIndex = chipIndex,
                Progress = 0f,
                IsAdd = true,
                Bounds = _chips[chipIndex].Bounds
            });
            if (!_animationTimer.Enabled) _animationTimer.Start();
        }

        private void AnimateChipRemove(int chipIndex, Rectangle bounds)
        {
            if (!_enableAddRemoveAnimation) return;
            _chipAnimations.Add(new ChipAnimation
            {
                ChipIndex = chipIndex,
                Progress = 0f,
                IsAdd = false,
                Bounds = bounds
            });
            if (!_animationTimer.Enabled) _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            bool needsRedraw = false;
            for (int i = _chipAnimations.Count - 1; i >= 0; i--)
            {
                var anim = _chipAnimations[i];
                anim.Progress += 0.06f;
                if (anim.Progress >= 1f)
                {
                    _chipAnimations.RemoveAt(i);
                }
                needsRedraw = true;
            }
            if (needsRedraw) Invalidate();
            if (_chipAnimations.Count == 0) _animationTimer.Stop();
        }

        // ── Chip removal with animation ───────────────────────────────
        private void RemoveChip(int index)
        {
            if (index < 0 || index >= _chips.Count) return;
            var chip = _chips[index];
            var bounds = chip.Bounds;

            // Fire event before removing
            OnChipRemoved(chip.Item, index);

            // Animate removal
            if (_enableAddRemoveAnimation)
            {
                AnimateChipRemove(index, bounds);
            }

            chip.IsSelected = false;
            _selectedItems.Remove(chip.Item);
            _selectedItem = _selectedItems.FirstOrDefault();
            OnSelectedItemChanged(_selectedItem);

            // Remove from collections
            _chipItems.RemoveAt(index);
            _chips.RemoveAt(index);

            UpdateChipBounds();
            Invalidate();
        }

        // ── Chip addition with animation ──────────────────────────────
        private void AddChip(SimpleItem item, bool animate = true)
        {
            if (item == null) return;

            _chipItems.Add(item);
            int newIndex = _chips.Count;
            _chips.Add(new ChipItem
            {
                Item = item,
                IsSelected = false,
                IsHovered = false,
                Variant = _chipVariant,
                Color = _chipColor,
                Size = _chipSize,
                IsNew = true
            });

            UpdateChipBounds();

            if (animate)
            {
                AnimateChipAdd(newIndex);
            }

            OnChipAdded(item, newIndex);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _lastInteractionPoint = e.Location;

            // Check if clicking on a chip for drag start
            if (_allowDragReorder && e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < _chips.Count; i++)
                {
                    if (_chips[i].Bounds.Contains(e.Location) && !_chips[i].IsDisabled)
                    {
                        _dragChipIndex = i;
                        _dragStartPoint = e.Location;
                        _isDragging = false;
                        break;
                    }
                }
            }

            bool clickedOnChip = _chips.Any(chip => chip.Bounds.Contains(e.Location));
            if (!clickedOnChip)
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (!AllowInlineEditing) return;

            for (int i = 0; i < _chips.Count; i++)
            {
                if (_chips[i].Bounds.Contains(e.Location) && !_chips[i].IsDisabled)
                {
                    StartInlineEdit(i);
                    return;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _lastInteractionPoint = e.Location;

            // Handle drag state
            if (_dragChipIndex >= 0 && e.Button == MouseButtons.Left)
            {
                int dx = Math.Abs(e.X - _dragStartPoint.X);
                int dy = Math.Abs(e.Y - _dragStartPoint.Y);
                if (!_isDragging && (dx > DragThreshold || dy > DragThreshold))
                {
                    _isDragging = true;
                }

                if (_isDragging)
                {
                    _dragGhostLocation = e.Location;
                    _dropInsertIndex = ComputeDropInsertIndex(e.Location);
                    Cursor = Cursors.SizeAll;
                    Invalidate();
                    return;
                }
            }

            base.OnMouseMove(e);
            UpdateHoverStates(e.Location);

            // Tooltip management
            HideTooltip();
            for (int i = 0; i < _chips.Count; i++)
            {
                if (_chips[i].Bounds.Contains(e.Location) && !_chips[i].IsDisabled)
                {
                    StartTooltipTimer(i);
                    break;
                }
            }
        }

        private int ComputeDropInsertIndex(Point cursor)
        {
            if (_chips.Count == 0) return 0;
            for (int i = 0; i < _chips.Count; i++)
            {
                if (i == _dragChipIndex) continue;
                var chip = _chips[i];
                if (chip.Bounds.IsEmpty) continue;
                int midX = chip.Bounds.Left + chip.Bounds.Width / 2;
                if (cursor.X < midX) return i;
            }
            return _chips.Count;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // Commit drag reorder
            if (_isDragging && _dragChipIndex >= 0 && _dropInsertIndex >= 0)
            {
                int fromIndex = _dragChipIndex;
                int toIndex = _dropInsertIndex;
                if (fromIndex != toIndex)
                {
                    // Adjust target index after removal
                    if (toIndex > fromIndex) toIndex--;

                    var chip = _chips[fromIndex];
                    var item = _chipItems[fromIndex];

                    _chips.RemoveAt(fromIndex);
                    _chipItems.RemoveAt(fromIndex);

                    toIndex = Math.Max(0, Math.Min(toIndex, _chips.Count));
                    _chips.Insert(toIndex, chip);
                    _chipItems.Insert(toIndex, item);

                    UpdateChipBounds();
                    OnChipReordered(item, fromIndex, toIndex);
                    Invalidate();
                }
            }

            _dragChipIndex = -1;
            _isDragging = false;
            _dropInsertIndex = -1;
            _dragGhostLocation = Point.Empty;
            if (Cursor == Cursors.SizeAll) Cursor = Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool needsRedraw = false;
            foreach (var chip in _chips)
            {
                if (chip.IsHovered) { chip.IsHovered = false; needsRedraw = true; }
            }

            // Cancel drag if mouse leaves control
            if (_isDragging)
            {
                _dragChipIndex = -1;
                _isDragging = false;
                _dropInsertIndex = -1;
                _dragGhostLocation = Point.Empty;
                needsRedraw = true;
            }

            HideTooltip();

            if (needsRedraw) Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Check input field click
            if (_allowChipCreation && _inputRect.Contains(e.Location))
            {
                _inputTextBox?.Focus();
                return;
            }
        }

        private void UpdateHoverStates(Point mouseLocation)
        {
            bool needsRedraw = false;
            bool onInteractive = false;
            foreach (var chip in _chips)
            {
                bool wasHovered = chip.IsHovered;
                chip.IsHovered = chip.Bounds.Contains(mouseLocation) && !chip.IsDisabled;
                if (chip.IsHovered) onInteractive = true;
                if (wasHovered != chip.IsHovered) needsRedraw = true;
            }
            if (!_selectAllRect.IsEmpty && _selectAllRect.Contains(mouseLocation)) onInteractive = true;
            if (!_clearAllRect.IsEmpty && _clearAllRect.Contains(mouseLocation)) onInteractive = true;
            Cursor = onInteractive ? Cursors.Hand : Cursors.Default;
            if (needsRedraw) Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab || keyData == (Keys.Shift | Keys.Tab))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // Handle inline editing keyboard
            if (_editingChipIndex >= 0 && _editTextBox != null)
            {
                if (keyData == Keys.Escape)
                {
                    CancelInlineEdit();
                    return true;
                }
                if (keyData == Keys.Enter)
                {
                    CommitInlineEdit();
                    return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // Handle chip input keyboard
            if (_allowChipCreation && _inputTextBox != null && _inputTextBox.Focused)
            {
                if (keyData == Keys.Escape)
                {
                    _inputTextBox.Text = "";
                    _inputTextBox.Parent?.Focus();
                    return true;
                }
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
                        HandleChipClick(_chips[_focusedIndex], Point.Empty, true);
                        handled = true;
                    }
                    break;
                case Keys.Delete:
                    if (_focusedIndex >= 0 && _focusedIndex < _chips.Count && !_chips[_focusedIndex].IsSelected)
                    {
                        RemoveChip(_focusedIndex);
                        _focusedIndex = Math.Max(0, Math.Min(_focusedIndex, _chips.Count - 1));
                        handled = true;
                    }
                    break;
                case Keys.F2:
                    if (_focusedIndex >= 0 && _focusedIndex < _chips.Count && AllowInlineEditing)
                    {
                        StartInlineEdit(_focusedIndex);
                        handled = true;
                    }
                    break;
            }
            if (handled) { Invalidate(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ── Inline editing ────────────────────────────────────────────
        private void StartInlineEdit(int chipIndex)
        {
            if (chipIndex < 0 || chipIndex >= _chips.Count) return;
            var chip = _chips[chipIndex];
            if (chip.IsDisabled) return;

            _editingChipIndex = chipIndex;
            _editOriginalText = chip.Item.Text;

            if (_editTextBox == null)
            {
                _editTextBox = new TextBox
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = _textFont ?? Font,
                    TextAlign = HorizontalAlignment.Left,
                    Visible = false
                };
                _editTextBox.KeyDown += EditTextBox_KeyDown;
                _editTextBox.LostFocus += EditTextBox_LostFocus;
                Controls.Add(_editTextBox);
            }

            _editTextBox.Text = chip.Item.Text;
            _editTextBox.Bounds = chip.Bounds;
            _editTextBox.Visible = true;
            _editTextBox.Focus();
            _editTextBox.SelectAll();
        }

        private void CommitInlineEdit()
        {
            if (_editingChipIndex < 0 || _editTextBox == null) return;

            var newText = _editTextBox.Text.Trim();
            var chip = _chips[_editingChipIndex];
            var oldText = _editOriginalText;

            _editTextBox.Visible = false;

            if (!string.IsNullOrEmpty(newText) && newText != oldText)
            {
                chip.Item.Text = newText;
                chip.Item.Name = newText;
                UpdateChipBounds();
                OnChipEdited(chip.Item, _editingChipIndex, oldText, newText);
            }
            else
            {
                OnChipEdited(chip.Item, _editingChipIndex, oldText, oldText, cancelled: true);
            }

            _editingChipIndex = -1;
            Invalidate();
        }

        private void CancelInlineEdit()
        {
            if (_editingChipIndex < 0 || _editTextBox == null) return;

            var chip = _chips[_editingChipIndex];
            _editTextBox.Visible = false;
            OnChipEdited(chip.Item, _editingChipIndex, _editOriginalText, _editOriginalText, cancelled: true);
            _editingChipIndex = -1;
            Invalidate();
        }

        private void EditTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CommitInlineEdit();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                CancelInlineEdit();
            }
        }

        private void EditTextBox_LostFocus(object? sender, EventArgs e)
        {
            if (_editingChipIndex >= 0)
            {
                CommitInlineEdit();
            }
        }

        // ── Chip input field ──────────────────────────────────────────
        private void EnsureInputTextBox()
        {
            if (_inputTextBox != null) return;

            _inputTextBox = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = _textFont ?? Font,
                Visible = true,
                Text = ""
            };
            _inputTextBox.KeyDown += InputTextBox_KeyDown;
            _inputTextBox.TextChanged += InputTextBox_TextChanged;
            Controls.Add(_inputTextBox);
        }

        private void RemoveInputTextBox()
        {
            if (_inputTextBox == null) return;
            _inputTextBox.KeyDown -= InputTextBox_KeyDown;
            _inputTextBox.TextChanged -= InputTextBox_TextChanged;
            Controls.Remove(_inputTextBox);
            _inputTextBox.Dispose();
            _inputTextBox = null;
        }

        private void InputTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || _inputDelimiters.Contains((char)e.KeyCode))
            {
                e.SuppressKeyPress = true;
                CommitInputChip();
            }
            else if (e.KeyCode == Keys.Back && string.IsNullOrEmpty(_inputTextBox.Text) && _chipItems.Count > 0)
            {
                // Backspace on empty input removes last chip
                RemoveChip(_chipItems.Count - 1);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _inputTextBox.Text = "";
            }
        }

        private void InputTextBox_TextChanged(object? sender, EventArgs e)
        {
            // Could add autocomplete/suggestions here in the future
        }

        private void CommitInputChip()
        {
            if (_inputTextBox == null) return;

            var text = _inputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            // Split by delimiters to support pasting multiple chips
            var parts = text.Split(new[] { ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                var newItem = new SimpleItem
                {
                    Text = trimmed,
                    Name = trimmed,
                    GuidId = Guid.NewGuid().ToString()
                };
                AddChip(newItem, _enableAddRemoveAnimation);
            }

            _inputTextBox.Text = "";
        }
    }
}
