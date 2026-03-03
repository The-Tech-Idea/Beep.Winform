using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Keyboard navigation and shortcut handling for BeepListBox.
    /// ARIA listbox keyboard contract (W3C):
    ///   ↑/↓      Move focus one item
    ///   Home/End  First / last item
    ///   PgUp/PgDn One visible page
    ///   Space     Toggle checkbox / select item
    ///   Enter     Activate item (ItemActivated)
    ///   Ctrl+A    Select all (multi)
    ///   Ctrl+C    Copy selected text to clipboard
    ///   Escape    Clear selection
    ///   F2        Begin inline edit
    ///   Delete    Raise ItemDeleteRequested
    ///   Printable Type-ahead search
    /// </summary>
    public partial class BeepListBox
    {
        // ── Type-ahead state ─────────────────────────────────────────────────────

        private string _typeAheadBuffer = "";
        private Timer? _typeAheadClearTimer;

        // ── Focused item index for keyboard nav ──────────────────────────────────

        private int _focusedIndex = -1;

        /// <summary>
        /// Index of the keyboard-focused item (may differ from SelectedIndex in multi-select).
        /// </summary>
        public int FocusedIndex
        {
            get => _focusedIndex;
            private set
            {
                if (_focusedIndex != value)
                {
                    _focusedIndex = value;
                    EnsureItemVisible(_focusedIndex);
                    NotifyA11yFocused(_focusedIndex);
                    Invalidate();
                }
            }
        }

        // ── Overrides ────────────────────────────────────────────────────────────

        protected override bool IsInputKey(Keys keyData)
        {
            // Let the control consume arrow keys, page keys, and home/end
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageUp:
                case Keys.PageDown:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var visibleItems = _helper?.GetVisibleItems();
            if (visibleItems == null || visibleItems.Count == 0) return;

            int count = visibleItems.Count;
            int current = _focusedIndex < 0 ? -1 : Math.Min(_focusedIndex, count - 1);

            bool ctrl  = (e.Modifiers & Keys.Control) == Keys.Control;
            bool shift = (e.Modifiers & Keys.Shift)   == Keys.Shift;

            switch (e.KeyCode)
            {
                // ── Movement ────────────────────────────────────────────────────
                case Keys.Up:
                    MoveFocus(current > 0 ? current - 1 : 0, shift, visibleItems);
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.Down:
                    MoveFocus(current < count - 1 ? current + 1 : count - 1, shift, visibleItems);
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.Home:
                    MoveFocus(0, shift, visibleItems);
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.End:
                    MoveFocus(count - 1, shift, visibleItems);
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.PageDown:
                {
                    int page = Math.Max(1, GetVisiblePageSize());
                    MoveFocus(Math.Min(count - 1, current + page), shift, visibleItems);
                    e.Handled = e.SuppressKeyPress = true;
                    break;
                }

                case Keys.PageUp:
                {
                    int page = Math.Max(1, GetVisiblePageSize());
                    MoveFocus(Math.Max(0, current - page), shift, visibleItems);
                    e.Handled = e.SuppressKeyPress = true;
                    break;
                }

                // ── Selection / action ───────────────────────────────────────────
                case Keys.Space:
                    if (_focusedIndex >= 0 && _focusedIndex < count)
                    {
                        var item = visibleItems[_focusedIndex];
                        if (_showCheckBox) ToggleItemCheckbox(item);
                        else HandleFocusedItemSelect(item, visibleItems, shift, ctrl);
                        e.Handled = e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Enter:
                    if (_focusedIndex >= 0 && _focusedIndex < count)
                    {
                        var item = visibleItems[_focusedIndex];
                        HandleFocusedItemSelect(item, visibleItems, shift: false, ctrl: false);
                        OnItemActivated(_focusedIndex, item);
                        e.Handled = e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Escape:
                    ClearSelection();
                    _focusedIndex = -1;
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.Delete:
                    if (_focusedIndex >= 0 && _focusedIndex < count)
                    {
                        OnItemDeleteRequested(_focusedIndex, visibleItems[_focusedIndex]);
                        e.Handled = e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.F2:
                    if (AllowInlineEdit && _focusedIndex >= 0 && _focusedIndex < count)
                    {
                        StartInlineEdit(visibleItems[_focusedIndex]);
                        e.Handled = e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.A when ctrl:
                    SelectAll();
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.C when ctrl:
                    CopySelectedToClipboard();
                    e.Handled = e.SuppressKeyPress = true;
                    break;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            // Type-ahead: printable characters only
            if (char.IsControl(e.KeyChar)) return;

            EnsureTypeAheadTimer();

            _typeAheadBuffer += e.KeyChar;
            _typeAheadClearTimer!.Stop();
            _typeAheadClearTimer.Start();

            TypeAheadJump(_typeAheadBuffer);
            e.Handled = true;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button != MouseButtons.Left) return;

            var layoutCache = _layoutHelper?.GetCachedLayout();
            if (layoutCache == null) return;

            foreach (var info in layoutCache)
            {
                if (info.RowRect.Contains(e.Location))
                {
                    int idx = _listItems.IndexOf(info.Item);
                    HandleFocusedItemSelect(info.Item,
                        _helper?.GetVisibleItems() ?? new System.Collections.Generic.List<SimpleItem>(),
                        shift: false, ctrl: false);
                    OnItemActivated(idx, info.Item);

                    if (AllowInlineEdit) StartInlineEdit(info.Item);
                    break;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Update focused index from click
            var layoutCache = _layoutHelper?.GetCachedLayout();
            if (layoutCache == null) return;

            int i = 0;
            var visible = _helper?.GetVisibleItems() ?? new System.Collections.Generic.List<SimpleItem>();
            foreach (var info in layoutCache)
            {
                if (info.RowRect.Contains(e.Location))
                {
                    _focusedIndex = visible.IndexOf(info.Item);
                    break;
                }
                i++;
            }

            // Right-click context menu
            if (e.Button == MouseButtons.Right && ShowContextMenu)
            {
                ShowItemContextMenu(e.Location);
            }

            // Record drag intention for left-button drags
            if (e.Button == MouseButtons.Left)
            {
                BeginDragCheck(e);
            }
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private void MoveFocus(int newIndex, bool shift,
            System.Collections.Generic.List<SimpleItem> visibleItems)
        {
            if (newIndex < 0 || newIndex >= visibleItems.Count) return;
            FocusedIndex = newIndex;

            var item = visibleItems[newIndex];
            if (SelectionMode == SelectionModeEnum.MultiExtended && shift && _anchorItem != null)
            {
                int anchorIdx = visibleItems.IndexOf(_anchorItem);
                if (anchorIdx >= 0) SelectRange(visibleItems, anchorIdx, newIndex);
            }
            else
            {
                HandleFocusedItemSelect(item, visibleItems, shift: false, ctrl: false);
            }
        }

        private void HandleFocusedItemSelect(SimpleItem item,
            System.Collections.Generic.List<SimpleItem> visibleItems,
            bool shift, bool ctrl)
        {
            if (SelectionMode == SelectionModeEnum.MultiExtended)
            {
                if (shift && _anchorItem != null)
                {
                    int anchor = visibleItems.IndexOf(_anchorItem);
                    int target = visibleItems.IndexOf(item);
                    if (anchor >= 0 && target >= 0) SelectRange(visibleItems, anchor, target);
                }
                else if (ctrl)
                {
                    if (_selectedItems.Contains(item)) RemoveFromSelection(item);
                    else AddToSelection(item);
                }
                else
                {
                    ClearSelection();
                    AddToSelection(item);
                    _anchorItem = item;
                }
            }
            else if (SelectionMode == SelectionModeEnum.MultiSimple || MultiSelect)
            {
                if (_selectedItems.Contains(item)) RemoveFromSelection(item);
                else AddToSelection(item);
            }
            else
            {
                SelectedItem = item;
            }
        }

        private int GetVisiblePageSize()
        {
            if (_listBoxPainter == null) return 5;
            int itemH = _listBoxPainter.GetPreferredItemHeight();
            if (itemH <= 0) return 5;
            var area = GetClientArea();
            return Math.Max(1, area.Height / itemH);
        }

        private void EnsureItemVisible(int index)
        {
            if (index < 0) return;
            var visible = _helper?.GetVisibleItems();
            if (visible == null || index >= visible.Count) return;

            // Compute item Y from layout cache
            var cache = _layoutHelper?.GetCachedLayout();
            if (cache == null || cache.Count == 0) return;

            var info = cache.FirstOrDefault(i => i.Item == visible[index]);
            if (info == null) return;

            var clientArea = GetClientArea();
            if (clientArea.IsEmpty) return;

            int rowTop    = info.RowRect.Top;
            int rowBottom = info.RowRect.Bottom;

            if (rowTop < clientArea.Top + _yOffset)
            {
                _yOffset = Math.Max(0, rowTop - clientArea.Top);
                _verticalScrollBar?.GetType().GetProperty("Value")?.SetValue(_verticalScrollBar, _yOffset);
                try { _layoutHelper?.CalculateLayout(this); } catch { }
                Invalidate();
            }
            else if (rowBottom > clientArea.Top + _yOffset + clientArea.Height)
            {
                _yOffset = rowBottom - clientArea.Top - clientArea.Height;
                _verticalScrollBar?.GetType().GetProperty("Value")?.SetValue(_verticalScrollBar, _yOffset);
                try { _layoutHelper?.CalculateLayout(this); } catch { }
                Invalidate();
            }
        }

        private void EnsureTypeAheadTimer()
        {
            if (_typeAheadClearTimer != null) return;
            _typeAheadClearTimer = new Timer
            {
                Interval = ListBoxs.Tokens.ListBoxTokens.TypeAheadClearMs
            };
            _typeAheadClearTimer.Tick += (s, e) =>
            {
                _typeAheadTimer_Tick();
            };
        }

        private void _typeAheadTimer_Tick()
        {
            _typeAheadClearTimer?.Stop();
            _typeAheadBuffer = "";
        }

        private void TypeAheadJump(string buffer)
        {
            if (string.IsNullOrEmpty(buffer)) return;
            var visible = _helper?.GetVisibleItems();
            if (visible == null || visible.Count == 0) return;

            string lower = buffer.ToLowerInvariant();

            // Start search from item after current focus
            int start = (_focusedIndex + 1) % visible.Count;
            for (int pass = 0; pass < visible.Count; pass++)
            {
                int idx = (start + pass) % visible.Count;
                var item = visible[idx];
                if (item.Text?.ToLowerInvariant().StartsWith(lower) == true)
                {
                    FocusedIndex = idx;
                    SelectedItem = item;
                    break;
                }
            }
        }

        private void CopySelectedToClipboard()
        {
            var lines = new System.Text.StringBuilder();
            if (SelectionMode != SelectionModeEnum.Single && _selectedItems.Count > 0)
            {
                foreach (var item in _selectedItems) lines.AppendLine(item.Text ?? "");
            }
            else if (_selectedItem != null)
            {
                lines.Append(_selectedItem.Text ?? "");
            }
            if (lines.Length > 0)
            {
                try { Clipboard.SetText(lines.ToString().TrimEnd()); } catch { }
            }
        }

        private void ShowItemContextMenu(System.Drawing.Point location)
        {
            // Find item at point
            SimpleItem? hitItem = null;
            int hitIndex = -1;
            var visible = _helper?.GetVisibleItems();
            var cache = _layoutHelper?.GetCachedLayout();
            if (cache != null && visible != null)
            {
                foreach (var info in cache)
                {
                    if (info.RowRect.Contains(location))
                    {
                        hitItem  = info.Item;
                        hitIndex = visible.IndexOf(info.Item);
                        break;
                    }
                }
            }

            var menu = ItemContextMenu ?? BuildDefaultContextMenu(hitItem);
            if (!OnContextMenuOpening(hitIndex, hitItem, menu)) return;
            menu.Show(this, location);
        }

        private ContextMenuStrip BuildDefaultContextMenu(SimpleItem? item)
        {
            var m = new ContextMenuStrip();
            if (item != null)
            {
                m.Items.Add("Select",  null, (s, e) => SelectedItem = item);
                m.Items.Add("Copy",    null, (s, e) => { try { Clipboard.SetText(item.Text ?? ""); } catch { } });
                if (AllowInlineEdit)
                    m.Items.Add("Edit F2", null, (s, e) => StartInlineEdit(item));
                m.Items.Add("-");
                var del = new ToolStripMenuItem("Delete");
                del.Click += (s, e) =>
                {
                    int idx = _listItems.IndexOf(item);
                    OnItemDeleteRequested(idx, item);
                };
                m.Items.Add(del);
            }
            return m;
        }

        // ── Inline edit (F2 / double-click) ─────────────────────────────────────

        private TextBox? _inlineEdit;

        /// <summary>
        /// Overlays an inline TextBox over the item row for in-place editing.
        /// </summary>
        public void StartInlineEdit(SimpleItem item)
        {
            if (item == null || !AllowInlineEdit) return;

            // Find the row rect from the layout cache
            var cache = _layoutHelper?.GetCachedLayout();
            if (cache == null) return;

            var info = cache.FirstOrDefault(x => x.Item == item);
            if (info == null) return;

            CommitInlineEdit(cancel: true);  // discard any previous edit

            var rowRect = info.RowRect;
            int pad = Helpers.DpiScalingHelper.ScaleValue(4, this);
            var editRect = new System.Drawing.Rectangle(
                rowRect.Left + pad,
                rowRect.Top + pad,
                rowRect.Width - pad * 2,
                rowRect.Height - pad * 2);

            _inlineEdit = new TextBox
            {
                Bounds   = editRect,
                Text     = item.Text ?? "",
                BorderStyle = BorderStyle.None,
                Font     = _textFont,
                BackColor = _currentTheme?.PanelBackColor ?? System.Drawing.Color.White,
                ForeColor = _currentTheme?.ForeColor ?? System.Drawing.Color.Black,
                Tag      = item
            };

            string oldText = item.Text ?? "";

            _inlineEdit.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)  { CommitInlineEdit(cancel: false); e.SuppressKeyPress = true; }
                if (e.KeyCode == Keys.Escape) { CommitInlineEdit(cancel: true);  e.SuppressKeyPress = true; }
            };
            _inlineEdit.LostFocus += (s, e) => CommitInlineEdit(cancel: false);

            Controls.Add(_inlineEdit);
            _inlineEdit.BringToFront();
            _inlineEdit.Focus();
            _inlineEdit.SelectAll();
        }

        private void CommitInlineEdit(bool cancel)
        {
            if (_inlineEdit == null) return;
            var edit = _inlineEdit;
            _inlineEdit = null;

            var item = edit.Tag as SimpleItem;
            string newText = edit.Text;
            string oldText = item?.Text ?? "";

            Controls.Remove(edit);
            edit.Dispose();

            if (!cancel && item != null && newText != oldText)
            {
                item.Text = newText;
                OnItemTextChanged(item, oldText, newText);
                InvalidateLayoutCache();
            }
        }
    }
}
