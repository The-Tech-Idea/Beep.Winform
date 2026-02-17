using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing dialog and filter-related methods for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        // Inline QuickSearch: real BeepComboBox (column selector) + BeepTextBox (search input)
        private BeepComboBox? _inlineQSCombo;
        private BeepTextBox? _inlineQSText;
        private Rectangle _inlineQuickSearchPaintRect = Rectangle.Empty;
        private Rectangle _inlineQSComboRect = Rectangle.Empty;
        private Rectangle _inlineQSTextRect = Rectangle.Empty;
        private bool _inlineQuickSearchIsLive;
        private string _inlineQSSelectedColumn = "All Columns";

        // Layout constants for inline quick search
        private const int InlineQSSpacing = 4;
        private const int InlineQSComboMinWidth = 100;
        private const int InlineQSComboMaxWidth = 160;
        private const float InlineQSComboWidthRatio = 0.30f;
        private const int InlineQSPadding = 2;

        #region Dialog Methods
        /// <summary>
        /// Shows an editor dialog for the currently selected cell.
        /// </summary>
        public void ShowCellEditor()
        {
            if (Selection.HasSelection)
            {
                var cell = Data.Rows[Selection.RowIndex].Cells[Selection.ColumnIndex];
                Dialog.ShowEditorDialog(cell);
            }
        }

        /// <summary>
        /// Shows the filter dialog to configure grid filtering.
        /// </summary>
        public void ShowFilterDialog(string? preferredColumnName = null, string? preferredFilterText = null)
        {
            Dialog.ShowFilterDialog(preferredColumnName, preferredFilterText);
        }

        /// <summary>
        /// Shows an inline criterion editor popup anchored near a toolbar chip/tag.
        /// </summary>
        public void ShowInlineCriterionEditor(string columnName, Point? anchorClientPoint = null)
        {
            Dialog.ShowInlineCriterionEditor(columnName, anchorClientPoint);
        }

        /// <summary>
        /// Toggles inline quick-search visibility. When controls are already visible,
        /// focuses the text box; when hidden, activates them.
        /// </summary>
        public void ShowSearchDialog()
        {
            if (_inlineQuickSearchIsLive)
            {
                // Already live — just focus the text box
                _inlineQSText?.Focus();
                return;
            }

            ActivateInlineQuickSearch();
        }

        /// <summary>
        /// Shows the column configuration dialog to customize column visibility and settings.
        /// </summary>
        public void ShowColumnConfigDialog()
        {
            Dialog.ShowColumnConfigDialog();
        }
        #endregion

        #region Inline QuickSearch

        /// <summary>
        /// Splits the given rect into combo (left) and text (right) sub-rects.
        /// </summary>
        private void CalculateInlineQSSubRects(Rectangle rect)
        {
            if (rect.IsEmpty) return;

            int innerY = rect.Y + InlineQSPadding;
            int innerH = Math.Max(20, rect.Height - InlineQSPadding * 2);

            int comboWidth = (int)(rect.Width * InlineQSComboWidthRatio);
            comboWidth = Math.Max(InlineQSComboMinWidth, Math.Min(InlineQSComboMaxWidth, comboWidth));

            int textX = rect.X + comboWidth + InlineQSSpacing;
            int textWidth = Math.Max(80, rect.Right - textX - InlineQSPadding);

            _inlineQSComboRect = new Rectangle(rect.X, innerY, comboWidth, innerH);
            _inlineQSTextRect = new Rectangle(textX, innerY, textWidth, innerH);
        }

        /// <summary>
        /// Called from the filter panel painter during each paint pass.
        /// Paints the inline quick-search using Draw(g, rect) on the combo + text controls.
        /// Real controls are hidden; they are only shown on click via ActivateInlineQuickSearch.
        /// </summary>
        internal void PaintInlineQuickSearch(Graphics g, Rectangle rect)
        {
            if (rect.IsEmpty) return;

            _inlineQuickSearchPaintRect = rect;
            CalculateInlineQSSubRects(rect);

            EnsureInlineQuickSearchControls();

            // Only update live (visible) control bounds during paint
            if (_inlineQuickSearchIsLive)
            {
                if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed && _inlineQSCombo.Visible)
                {
                    _inlineQSCombo.Bounds = _inlineQSComboRect;
                    _inlineQSCombo.BringToFront();
                }

                if (_inlineQSText != null && !_inlineQSText.IsDisposed && _inlineQSText.Visible)
                {
                    _inlineQSText.Bounds = _inlineQSTextRect;
                    _inlineQSText.BringToFront();
                }
            }

            // Draw static images of the controls into the provided rect
            if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed)
            {
                _inlineQSCombo.Draw(g, _inlineQSComboRect);
            }

            if (_inlineQSText != null && !_inlineQSText.IsDisposed)
            {
                _inlineQSText.Draw(g, _inlineQSTextRect);
            }
        }

        /// <summary>
        /// Activates the inline quick-search controls and focuses the text box.
        /// If not yet positioned by the painter, uses stored/fallback rect.
        /// </summary>
        internal void ActivateInlineQuickSearch()
        {
            if (!ShowTopFilterPanel)
            {
                ShowTopFilterPanel = true;
            }

            EnsureInlineQuickSearchControls();
            if (_inlineQSCombo == null || _inlineQSText == null) return;

            // If not yet live, position and show controls
            if (!_inlineQuickSearchIsLive)
            {
                var targetRect = _inlineQuickSearchPaintRect;
                if (targetRect.IsEmpty)
                {
                    if (Render.TopFilterCellRects.TryGetValue(BaseFilterPanelPainter.SearchActionKey, out var searchRect))
                        targetRect = searchRect;
                }
                if (targetRect.IsEmpty) return;

                _inlineQuickSearchPaintRect = targetRect;
                CalculateInlineQSSubRects(targetRect);

                _inlineQSCombo.Bounds = _inlineQSComboRect;
                _inlineQSText.Bounds = _inlineQSTextRect;
                _inlineQSCombo.Visible = true;
                _inlineQSText.Visible = true;
                _inlineQSCombo.BringToFront();
                _inlineQSText.BringToFront();
                _inlineQuickSearchIsLive = true;
            }

            SyncInlineQSColumnList();
            SyncInlineQSFromActiveFilter();
            _inlineQSText.Focus();
        }

        /// <summary>
        /// Hides the inline quick-search controls (paint keeps drawing).
        /// </summary>
        internal void DeactivateInlineQuickSearch()
        {
            _inlineQuickSearchIsLive = false;
          
            if (_inlineQSText != null && !_inlineQSText.IsDisposed)
                _inlineQSText.Visible = false;
            SafeInvalidate();
        }

        internal bool TryGetInlineQuickSearchBounds(out Rectangle bounds)
        {
            bounds = _inlineQuickSearchPaintRect;
            return !bounds.IsEmpty && _inlineQuickSearchIsLive;
        }

        /// <summary>
        /// Returns the painted quick-search rectangle for hit-testing.
        /// </summary>
        internal Rectangle GetInlineQuickSearchPaintRect()
        {
            return _inlineQuickSearchPaintRect;
        }

        #region Individual Control Management for Combo Box

        /// <summary>
        /// Hides the combo box control.
        /// </summary>
        internal void HideInlineQSCombo()
        {
            if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed)
            {
                _inlineQSCombo.Visible = false;
                System.Diagnostics.Debug.WriteLine("Combo box hidden");
            }
           // CheckIfBothControlsHidden();
        }

        /// <summary>
        /// Shows the combo box control.
        /// </summary>
        internal void ShowInlineQSCombo()
        {
            if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed)
            {
                _inlineQSCombo.Visible = true;
                _inlineQuickSearchIsLive = true;
                System.Diagnostics.Debug.WriteLine("Combo box shown");
            }
        }

        /// <summary>
        /// Checks if the click location is ON the combo box control.
        /// </summary>
        internal bool IsClickOnInlineQSCombo(Point clickLocation)
        {
            // Check actual control state, not just the live flag
            if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed && _inlineQSCombo.Visible && _inlineQSCombo.Enabled)
            {
                bool isOn = _inlineQSCombo.Bounds.Contains(clickLocation);
                System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is {(isOn ? "ON" : "NOT ON")} combo bounds {_inlineQSCombo.Bounds}");
                return isOn;
            }
            System.Diagnostics.Debug.WriteLine($"Combo box is not available for click detection (Disposed={_inlineQSCombo?.IsDisposed}, Visible={_inlineQSCombo?.Visible}, Enabled={_inlineQSCombo?.Enabled})");
            return false;
        }

        /// <summary>
        /// Hides combo box if click is outside its bounds.
        /// </summary>
        internal void HideInlineQSComboIfClickedOutside(Point clickLocation)
        {
            // Check actual control state
            if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed && _inlineQSCombo.Visible)
            {
                if (!_inlineQSCombo.Bounds.Contains(clickLocation))
                {
                    System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is OUTSIDE combo bounds {_inlineQSCombo.Bounds} - hiding combo");
                    HideInlineQSCombo();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is INSIDE combo bounds - keeping visible");
                }
            }
        }

        /// <summary>
        /// Shows combo box if click is on its painted rect (works even when real control is hidden).
        /// </summary>
        internal void ShowInlineQSComboIfClickedOn(Point clickLocation)
        {
            if (!_inlineQSComboRect.IsEmpty && _inlineQSComboRect.Contains(clickLocation))
            {
                EnsureInlineQuickSearchControls();
                if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed && !_inlineQSCombo.Visible)
                {
                    SyncInlineQSColumnList();
                    _inlineQSCombo.Bounds = _inlineQSComboRect;
                    ShowInlineQSCombo();
                    _inlineQSCombo.BringToFront();
                    System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is ON combo painted rect {_inlineQSComboRect} - showing combo");
                }
            }
        }

        #endregion

        #region Individual Control Management for Text Box

        /// <summary>
        /// Hides the text box control.
        /// </summary>
        internal void HideInlineQSText()
        {
            if (_inlineQSText != null && !_inlineQSText.IsDisposed)
            {
                _inlineQSText.Visible = false;
                System.Diagnostics.Debug.WriteLine("Text box hidden");
            }
            CheckIfBothControlsHidden();
        }

        /// <summary>
        /// Shows the text box control.
        /// </summary>
        internal void ShowInlineQSText()
        {
            if (_inlineQSText != null && !_inlineQSText.IsDisposed)
            {
                _inlineQSText.Visible = true;
                _inlineQuickSearchIsLive = true;
                System.Diagnostics.Debug.WriteLine("Text box shown");
            }
        }

        /// <summary>
        /// Checks if the click location is ON the text box control.
        /// </summary>
        internal bool IsClickOnInlineQSText(Point clickLocation)
        {
            // Check actual control state, not just the live flag
            if (_inlineQSText != null && !_inlineQSText.IsDisposed && _inlineQSText.Visible && _inlineQSText.Enabled)
            {
                bool isOn = _inlineQSText.Bounds.Contains(clickLocation);
                System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is {(isOn ? "ON" : "NOT ON")} textbox bounds {_inlineQSText.Bounds}");
                return isOn;
            }
            System.Diagnostics.Debug.WriteLine($"Text box is not available for click detection (Disposed={_inlineQSText?.IsDisposed}, Visible={_inlineQSText?.Visible}, Enabled={_inlineQSText?.Enabled})");
            return false;
        }

        /// <summary>
        /// Hides text box if click is outside its bounds.
        /// </summary>
        internal void HideInlineQSTextIfClickedOutside(Point clickLocation)
        {
            // Check actual control state
            if (_inlineQSText != null && !_inlineQSText.IsDisposed && _inlineQSText.Visible)
            {
                if (!_inlineQSText.Bounds.Contains(clickLocation))
                {
                    System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is OUTSIDE textbox bounds {_inlineQSText.Bounds} - hiding textbox");
                    HideInlineQSText();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is INSIDE textbox bounds - keeping visible");
                }
            }
        }

        /// <summary>
        /// Shows text box if click is on its painted rect (works even when real control is hidden).
        /// </summary>
        internal void ShowInlineQSTextIfClickedOn(Point clickLocation)
        {
            if (!_inlineQSTextRect.IsEmpty && _inlineQSTextRect.Contains(clickLocation))
            {
                EnsureInlineQuickSearchControls();
                if (_inlineQSText != null && !_inlineQSText.IsDisposed && !_inlineQSText.Visible)
                {
                    _inlineQSText.Bounds = _inlineQSTextRect;
                    ShowInlineQSText();
                    _inlineQSText.BringToFront();
                    _inlineQSText.Focus();
                    System.Diagnostics.Debug.WriteLine($"Click {clickLocation} is ON text painted rect {_inlineQSTextRect} - showing text");
                }
            }
        }

        #endregion

        #region Shared Helper

        /// <summary>
        /// Checks if both controls are hidden and updates the live flag.
        /// </summary>
        private void CheckIfBothControlsHidden()
        {
            bool comboVisible = _inlineQSCombo != null && !_inlineQSCombo.IsDisposed && _inlineQSCombo.Visible;
            bool textVisible = _inlineQSText != null && !_inlineQSText.IsDisposed && _inlineQSText.Visible;

            if (!comboVisible && !textVisible)
            {
                _inlineQuickSearchIsLive = false;
                System.Diagnostics.Debug.WriteLine("Both controls hidden - marking as not live");
            }
        }

        #endregion

        #region Legacy Combined Methods (for backward compatibility)

        internal void HideInlineQuickSearch()
        {
            HideInlineQSCombo();
            HideInlineQSText();
        }

        internal bool IsClickOnInlineQuickSearchControls(Point clickLocation)
        {
            return IsClickOnInlineQSCombo(clickLocation) || IsClickOnInlineQSText(clickLocation);
        }

        internal void HideInlineQuickSearchIfClickedOutside(Point clickLocation)
        {
            HideInlineQSComboIfClickedOutside(clickLocation);
            HideInlineQSTextIfClickedOutside(clickLocation);
        }

        #endregion

        /// <summary>
        /// Updates the control positions from the painter's search rect.
        /// NOTE: Positioning is now handled directly in PaintInlineQuickSearch() to ensure
        /// perfect synchronization between paint location and control bounds during resize/scroll.
        /// This method is kept for backward compatibility but is effectively a no-op.
        /// </summary>
        internal void PositionInlineQuickSearchControl()
        {
            // Position updates are now handled synchronously in PaintInlineQuickSearch()
            // to prevent alignment issues during resize
        }

        internal void EnsureInlineQuickSearchVisible()
        {
            if (!UseInlineQuickSearch)
            {
                HideInlineQuickSearch();
                return;
            }

            if (!ShowTopFilterPanel) return;

            EnsureInlineQuickSearchControls();
            SafeInvalidate();
        }

        /// <summary>
        /// Creates the BeepComboBox + BeepTextBox controls if they don't exist yet.
        /// Controls start hidden and are made visible by PaintInlineQuickSearch once
        /// the painter provides the correct rect during the first paint pass.
        /// </summary>
        private void EnsureInlineQuickSearchControls()
        {
            if (_inlineQSCombo != null && !_inlineQSCombo.IsDisposed &&
                _inlineQSText != null && !_inlineQSText.IsDisposed)
            {
                return;
            }

            // --- Column selector combo ---
            if (_inlineQSCombo == null || _inlineQSCombo.IsDisposed)
            {
                _inlineQSCombo = new BeepComboBox
                {
                    Name = "inlineQSCombo",
                    IsChild = true,
                    IsFrameless = true,
                    Theme = Theme,
                    Visible = false,
                    TabStop = true,
                    Enabled = true  // Explicitly enable for mouse events
                };

                SyncInlineQSColumnList();

                // Select "All Columns" by default
                var allItem = _inlineQSCombo.ListItems.FirstOrDefault(
                    i => string.Equals(i.Text, "All Columns", System.StringComparison.OrdinalIgnoreCase));
                if (allItem != null)
                    _inlineQSCombo.SelectedItem = allItem;

                _inlineQSCombo.SelectedItemChanged += InlineQSCombo_SelectedItemChanged;
                _inlineQSCombo.LostFocus += InlineQSControl_LostFocus;
                Controls.Add(_inlineQSCombo);
                
                System.Diagnostics.Debug.WriteLine("Combo box created and added to Controls");
            }

            // --- Search text box ---
            if (_inlineQSText == null || _inlineQSText.IsDisposed)
            {
                _inlineQSText = new BeepTextBox
                {
                    Name = "inlineQSText",
                    IsChild = true,
                    IsFrameless = true,
                    Theme = Theme,
                    PlaceholderText = "Search...",
                    Visible = false,
                    TabStop = true,
                    Enabled = true  // Explicitly enable for mouse events
                };

                _inlineQSText.TextChanged += InlineQSText_TextChanged;
                _inlineQSText.KeyDown += InlineQSText_KeyDown;
                _inlineQSText.LostFocus += InlineQSControl_LostFocus;
                Controls.Add(_inlineQSText);
                
                System.Diagnostics.Debug.WriteLine("Text box created and added to Controls");
            }
        }

        /// <summary>
        /// Populates the combo box with "All Columns" + visible data columns.
        /// </summary>
        private void SyncInlineQSColumnList()
        {
            if (_inlineQSCombo == null || _inlineQSCombo.IsDisposed) return;

            var items = new BindingList<SimpleItem>();
            items.Add(new SimpleItem { Text = "All Columns", Value = "All Columns" });

            foreach (var col in Data.Columns.Where(
                c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID && c.Visible))
            {
                string display = string.IsNullOrWhiteSpace(col.ColumnCaption) ? col.ColumnName : col.ColumnCaption;
                items.Add(new SimpleItem { Text = display, Value = col.ColumnName });
            }

            _inlineQSCombo.ListItems = items;
        }

        /// <summary>
        /// Reads the current ActiveFilter and syncs the combo selection + text box text.
        /// </summary>
        private void SyncInlineQSFromActiveFilter()
        {
            if (_inlineQSCombo == null || _inlineQSText == null) return;

            string searchText = string.Empty;
            string selectedColumn = "All Columns";

            if (ActiveFilter != null && ActiveFilter.Criteria.Count > 0)
            {
                var criterion = ActiveFilter.Criteria[0];
                searchText = criterion.Value?.ToString() ?? string.Empty;
                selectedColumn = string.IsNullOrWhiteSpace(criterion.ColumnName) ? "All Columns" : criterion.ColumnName;
            }

            _inlineQSSelectedColumn = selectedColumn;

            // Set combo selection
            var match = _inlineQSCombo.ListItems.FirstOrDefault(
                i => string.Equals(i.Value?.ToString(), selectedColumn, System.StringComparison.OrdinalIgnoreCase)
                  || string.Equals(i.Text, selectedColumn, System.StringComparison.OrdinalIgnoreCase));
            if (match != null && _inlineQSCombo.SelectedItem != match)
                _inlineQSCombo.SelectedItem = match;

            // Set text
            if (_inlineQSText.Text != searchText)
                _inlineQSText.Text = searchText;
        }

        // --- Event handlers ---

        private void InlineQSCombo_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            _inlineQSSelectedColumn = e.SelectedItem.Value?.ToString() ?? e.SelectedItem.Text ?? "All Columns";
            ApplyInlineQuickSearchFilter();
        }

        private void InlineQSText_TextChanged(object? sender, System.EventArgs e)
        {
            ApplyInlineQuickSearchFilter();
        }

        private void InlineQSText_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DeactivateInlineQuickSearch();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Handles LostFocus for inline quick search controls.
        /// Only hides controls if focus leaves BOTH the combo and textbox.
        /// </summary>
        private void InlineQSControl_LostFocus(object? sender, System.EventArgs e)
        {
            // Don't hide - the HandleMouseDown click-outside logic handles hiding.
            // LostFocus was killing the combo when clicking from textbox to combo
            // because the deferred check ran after the combo's popup stole focus.
        }

        /// <summary>
        /// Applies the current combo + text value as a quick filter on the grid.
        /// </summary>
        private void ApplyInlineQuickSearchFilter()
        {
            string text = _inlineQSText?.Text ?? string.Empty;
            string? columnName = _inlineQSSelectedColumn;

            if (string.Equals(columnName, "All Columns", System.StringComparison.OrdinalIgnoreCase))
            {
                columnName = null;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                ClearFilter();
            }
            else
            {
                ApplyQuickFilter(text, columnName);
            }
        }

        internal void DisposeInlineQuickSearchControl()
        {
            _inlineQuickSearchIsLive = false;

            if (_inlineQSCombo != null)
            {
                _inlineQSCombo.SelectedItemChanged -= InlineQSCombo_SelectedItemChanged;
                _inlineQSCombo.LostFocus -= InlineQSControl_LostFocus;
                if (!_inlineQSCombo.IsDisposed)
                    _inlineQSCombo.Dispose();
                _inlineQSCombo = null;
            }

            if (_inlineQSText != null)
            {
                _inlineQSText.TextChanged -= InlineQSText_TextChanged;
                _inlineQSText.KeyDown -= InlineQSText_KeyDown;
                _inlineQSText.LostFocus -= InlineQSControl_LostFocus;
                if (!_inlineQSText.IsDisposed)
                    _inlineQSText.Dispose();
                _inlineQSText = null;
            }
        }

        #endregion

        #region Filter and Sort Methods
        /// <summary>
        /// Enables Excel-like filter functionality by adding filter icons to column headers.
        /// This method can be expanded to add filter dropdowns to column headers.
        /// </summary>
        public void EnableExcelFilter()
        {
            // This method can be expanded to add filter dropdowns to column headers
            // For now, it's a placeholder that ensures the grid is ready for filtering
            foreach (var col in Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
            {
                col.ShowFilterIcon = true;
            }
            SafeInvalidate();
        }

        /// <summary>
        /// Toggles the sort direction for the specified column index.
        /// </summary>
        /// <param name="columnIndex">The index of the column to sort.</param>
        public void ToggleColumnSort(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Data.Columns.Count)
                return;

            var column = Data.Columns[columnIndex];
            if (column == null)
                return;

            // Toggle sort direction
            var newDirection = column.SortDirection == SortDirection.Ascending 
                ? SortDirection.Descending 
                : SortDirection.Ascending;

            // Clear previous sort indicators
            foreach (var col in Data.Columns)
            {
                col.IsSorted = false;
                col.ShowSortIcon = false;
            }

            // Set new sort
            column.IsSorted = true;
            column.ShowSortIcon = true;
            column.SortDirection = newDirection;

            // Apply the sort
            SortFilter.Sort(column.ColumnName, newDirection);

            // Refresh the grid
            SafeInvalidate();
        }
        #endregion
    }
}
