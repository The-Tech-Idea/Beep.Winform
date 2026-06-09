using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing mouse and keyboard input event handlers for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region Mouse & Keyboard Event Handlers
        /// <summary>
        /// Handles mouse down events for grid interaction.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"BeepGridPro.OnMouseDown at {e.Location}, TopFilterRect={Layout.TopFilterRect}, ShowTopFilterPanel={ShowTopFilterPanel}");
            // === ADD THESE TWO LINES ===
            Layout.EnsureCalculated();
            ScrollBars?.UpdateBars();
            if (e.Button == MouseButtons.Right)
            {
                ShowGridContextMenu(e);
                base.OnMouseDown(e);
                return;
            }
            bool handledByScrollbar = ScrollBars?.HandleMouseDown(e.Location, e.Button) ?? false;
            System.Diagnostics.Debug.WriteLine($"handledByScrollbar={handledByScrollbar}, Input is null: {Input == null}, Input type: {Input?.GetType().FullName}");
            if (!handledByScrollbar)
            {
                System.Diagnostics.Debug.WriteLine($"About to call Input.HandleMouseDown, method exists: {Input.GetType().GetMethod("HandleMouseDown") != null}");
                Input.HandleMouseDown(e);
                System.Diagnostics.Debug.WriteLine("Input.HandleMouseDown returned");
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Handles mouse move events for hover effects and dragging.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Layout.EnsureCalculated();
            ScrollBars?.UpdateBars();
            // Always forward mouse-move to the input helper so the toolbar
            // hover state stays in sync (even during scrollbar drag, since
            // the mouse position still tells us whether the cursor is over
            // a toolbar button).
            ScrollBars?.HandleMouseMove(e.Location);
            Input.HandleMouseMove(e);
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handles mouse up events.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // === ADD THESE TWO LINES ===
            Layout.EnsureCalculated();
            ScrollBars?.UpdateBars();
            bool handledByScrollbar = ScrollBars?.HandleMouseUp(e.Location, e.Button) ?? false;
            if (!handledByScrollbar)
            {
                Input.HandleMouseUp(e);
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Handles mouse wheel events for scrolling.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (ContextMenus.ContextMenuManager.IsAnyMenuActive)
            {
                base.OnMouseWheel(e);
                return;
            }
            ScrollBars?.HandleMouseWheel(e);
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Handles keyboard input for navigation and editing.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Global shortcuts handled at the control level so they work
            // regardless of which helper currently owns focus.
            if (ShowToolbar && !e.Handled && Layout != null && !Layout.ToolbarRect.IsEmpty)
            {
                // Ctrl+F → focus toolbar search.
                if (e.Control && e.KeyCode == Keys.F)
                {
                    FocusToolbarSearch();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }
                // / and F3 → focus toolbar search (Excel / Sheets convention).
                // Skip when the editor already has focus so the user can type
                // a slash or press F3 without having their partial text
                // cleared by a second FocusToolbarSearch().
                if (!FilterEditor.IsSearchEditorFocused())
                {
                    if (e.KeyCode == Keys.Divide || e.KeyCode == Keys.OemQuestion)
                    {
                        FocusToolbarSearch();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        return;
                    }
                    // F3 → focus toolbar search.  Shift+F3 is reserved.
                    if (e.KeyCode == Keys.F3 && !e.Shift)
                    {
                        FocusToolbarSearch();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        return;
                    }
                }
                // Per-button shortcuts (Insert → Add, F2 → Edit, Delete → Delete,
                // etc.) — only fire when no child editor has focus.
                if (!FilterEditor.IsSearchEditorFocused())
                {
                    if (TryFireToolbarShortcut(e))
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        return;
                    }
                }
                // Esc while the search box has focus → cancel + defocus.
                // The FilterEditorHelper handles the SearchHasFocus reset,
                // the focus restore, and the invalidation internally so
                // every hide path goes through the same teardown.
                if (e.KeyCode == Keys.Escape && ToolbarState.SearchHasFocus)
                {
                    FilterEditor.HideSearchEditor();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            if (!ContextMenus.ContextMenuManager.IsAnyMenuActive)
            {
                Input.HandleKeyDown(e);
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// If <paramref name="e"/> matches a toolbar button's <see cref="ToolbarButtonItem.Shortcut"/>,
        /// fire the button click and return true.  The default modifier-key handling
        /// is opt-in: a button with <see cref="Keys.F2"/> fires on plain F2; a button
        /// with <see cref="Keys.Insert"/> fires on plain Insert; etc.  Hosts that need
        /// Ctrl-style shortcuts can extend the <see cref="ToolbarButtonItem.Shortcut"/>
        /// value to include modifiers (e.g. <c>Keys.Control | Keys.N</c>) in a later pass.
        /// </summary>
        private bool TryFireToolbarShortcut(KeyEventArgs e)
        {
            var state = ToolbarState;
            if (state == null) return false;
            // Strip modifier bits so the dictionary lookup matches the simple
            // Keys.Insert / Keys.F2 / Keys.Delete values stored on each button.
            var key = e.KeyData & Keys.KeyCode;
            var btn = state.FindButtonByShortcut(key);
            if (btn == null) return false;
            // Honor visibility and overflow — hidden / overflowed buttons
            // don't get keyboard shortcuts.
            if (!btn.IsVisible || btn.IsOverflow) return false;
            Input.HandleToolbarButtonClick(btn.Key);
            return true;
        }

        /// <summary>
        /// Activates the toolbar search box and focuses it, so the user can
        /// start typing immediately.  Wired to Ctrl+F, "/" and F3 in
        /// <see cref="OnKeyDown"/>.  When the editor already has focus, the
        /// call is a no-op (so hotkey re-activation does not select-all and
        /// overwrite the user's partial text).
        /// </summary>
        public void FocusToolbarSearch()
        {
            if (!ShowToolbar || Layout == null || Layout.ToolbarRect.IsEmpty) return;
            // When the search editor already has focus we must not call
            // ActivateSearchEditor again; it calls SelectAll() which
            // would overwrite the user's in-progress text.  Ctrl+F
            // arrives from the control level even when the editor has
            // focus, so the guard must live here as well as in
            // OnKeyDown (the OnKeyDown guard catches direct "/" and F3;
            // this guard catches Ctrl+F and any programmatic callers).
            if (FilterEditor.IsSearchEditorFocused()) return;
            ToolbarState.SearchHasFocus = true;
            FilterEditor.ActivateSearchEditor(ToolbarState.SearchBoxRect);
            SafeInvalidate(Layout.ToolbarRect);
        }

        /// <summary>
        /// Returns the tooltip text for the toolbar button at the given
        /// control-relative <paramref name="location"/>, or an empty
        /// string when no button is under the cursor.  Hosts can bind
        /// this to a <see cref="ToolTip"/> to get rich tooltips.
        /// </summary>
        public string GetToolbarButtonTooltipAt(Point location)
        {
            if (!ShowToolbar || ToolbarState == null) return string.Empty;
            var state = ToolbarState;
            // Named element hit-tests first so the user can hover the
            // search box, filter, advanced, clear-filter chip, and
            // overflow button separately from the action/export list.
            if (state.SearchBoxRect.Contains(location)) return "Search rows";
            if (state.FilterButtonRect.Contains(location)) return "Open column filter";
            if (state.AdvancedButtonRect.Contains(location)) return "Open advanced filter";
            if (state.ClearFilterRect.Contains(location) && state.IsFilterActive) return "Clear active filter";
            if (state.OverflowButtonRect.Contains(location)) return "Show overflow buttons";

            // Fall back to the unified hit-test for the action / export
            // button lists.  Reusing HitTest() instead of duplicating the
            // loop ensures the tooltip and the click handler agree on
            // what is hoverable.
            var key = state.HitTest(location);
            return state.GetTooltipForButton(key ?? string.Empty);
        }

        /// <summary>
        /// Refreshes the toolbar tooltip after <paramref name="location"/>
        /// changed.  The tooltip is set on a single hidden child control
        /// sized to the toolbar rect, so WinForms' standard
        /// <see cref="ToolTip"/> machinery handles the popup without
        /// requiring one tooltip per painted button.
        /// </summary>
        internal void UpdateToolbarTooltip(Point location)
        {
            if (_toolbarTooltip == null) return;
            if (Layout == null || Layout.ToolbarRect.IsEmpty) return;
            if (!Layout.ToolbarRect.Contains(location))
            {
                if (_lastTooltipText.Length > 0)
                {
                    _toolbarTooltip.SetToolTip(this, string.Empty);
                    _lastTooltipText = string.Empty;
                    _lastTooltipKey = string.Empty;
                }
                return;
            }
            // Compute the hover key once and reuse it for both the dedupe
            // check and the tooltip text.  Calling GetToolbarButtonTooltipAt
            // would re-run the same hit-test twice.
            var key = ToolbarState?.HitTest(location) ?? string.Empty;
            if (string.Equals(key, _lastTooltipKey, System.StringComparison.Ordinal)) return;
            _lastTooltipKey = key;
            // Recompute the text from scratch because the named-element
            // hover keys (search/filter/advanced/clear/overflow) are
            // distinct from the action/export button keys.
            _lastTooltipText = GetToolbarButtonTooltipAt(location);
            _toolbarTooltip.SetToolTip(this, _lastTooltipText);
        }

        /// <summary>
        /// Intercept Tab/Shift+Tab so they navigate cells instead of moving focus out of the grid.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab || keyData == (Keys.Shift | Keys.Tab))
            {
                var e = new KeyEventArgs(keyData);
                Input.HandleKeyDown(e);
                if (e.Handled)
                    return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        #endregion
    }
}
