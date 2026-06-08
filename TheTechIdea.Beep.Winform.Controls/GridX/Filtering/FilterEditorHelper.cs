using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filtering
{
    /// <summary>
    /// Manages on-demand activation of the search textbox overlay in the unified toolbar.
    /// Only creates a real control when the user clicks the painted search box.
    /// The editor is sized to the right side of the search box (excluding
    /// the search icon) and uses a transparent background so the painted
    /// background, rounded border, and search icon all remain visible.
    /// </summary>
    internal class FilterEditorHelper
    {
        private readonly BeepGridPro _grid;
        private BeepTextBox? _searchEditor;
        // Set to true by the Escape key handler so the impending LostFocus
        // event knows to skip the commit.  Without this flag Escape was
        // committed because hiding the editor fires LostFocus which called
        // CommitSearch.
        private bool _isCancelling;

        public FilterEditorHelper(BeepGridPro grid) { _grid = grid; }

        public void ActivateSearchEditor(Rectangle bounds)
        {
            if (_searchEditor == null)
            {
                _searchEditor = new BeepTextBox
                {
                    // Frameless so the editor's border does not stack on top
                    // of the painter's rounded border.  IsFrameless is read
                    // by the BaseControl painter (ClassicBaseControlPainter)
                    // and suppresses the border.  The BeepTextBox still
                    // paints its own background, so we set BackColor=Transparent
                    // below so the painted search box background shows
                    // through.
                    IsFrameless = true,
                    IsChild = true,
                    // The BackColor is the only "background" the BeepTextBox
                    // itself fills.  Transparent lets the painted rounded
                    // background show through so the user sees ONE clean
                    // search box, not two stacked fills.
                    BackColor = Color.Transparent,
                    // Zero padding so the editor's text starts at the
                    // editor's left edge, which the host has already
                    // inset by SearchIconWidth to match the painted
                    // placeholder position.
                    Padding = new Padding(0),
                    Margin = new Padding(0),
                    Font = _grid.Font
                };
                _searchEditor.LostFocus += OnSearchEditorLostFocus;
                _searchEditor.KeyDown += OnSearchEditorKeyDown;
                _searchEditor.TextChanged += OnSearchEditorTextChanged;
                _grid.Controls.Add(_searchEditor);
            }

            // The editor is sized to the right side of the search box,
            // excluding the icon column.  This keeps the search icon
            // visible to the left of the caret and aligns the editor's
            // text with the painted placeholder / text position.
            var inner = InnerEditorBounds(bounds);
            _searchEditor.Bounds = inner;
            _searchEditor.Text = _grid.ToolbarState.SearchText;
            _isCancelling = false;
            _searchEditor.Visible = true;
            _searchEditor.Focus();
            _searchEditor.SelectAll();
        }

        /// <summary>
        /// Returns the editor bounds inside the painted search box, leaving
        /// the icon column on the left untouched.  Width is shrunk by the
        /// <see cref="BeepGridToolbarState.SearchIconWidth"/> scaled to DPI
        /// so the editor text aligns with the painted text rect.
        /// </summary>
        private Rectangle InnerEditorBounds(Rectangle bounds)
        {
            int pad = (int)(_grid.ToolbarState.SearchIconWidth * (_grid.DeviceDpi / 96f));
            return new Rectangle(
                bounds.X + pad, bounds.Y,
                Math.Max(0, bounds.Width - pad), bounds.Height);
        }

        private void OnSearchEditorTextChanged(object? sender, EventArgs e)
        {
            // Trigger toolbar repaint to update the painted search text in
            // sync with the editor.  We do NOT commit on every keystroke —
            // commit happens on Enter or focus loss.
            _grid.ToolbarState.SearchText = _searchEditor?.Text ?? string.Empty;
            _grid.SafeInvalidate(_grid.Layout.ToolbarRect);
        }

        private void OnSearchEditorKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitAndHide();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // Mark the upcoming LostFocus as a cancel so the commit is
                // skipped.  We still keep the editor's text in
                // ToolbarState.SearchText so the painted search reflects
                // what the user typed (without applying the filter).
                _isCancelling = true;
                if (_searchEditor != null)
                {
                    _grid.ToolbarState.SearchText = _searchEditor.Text;
                }
                DeactivateAndReturnFocus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void OnSearchEditorLostFocus(object? sender, EventArgs e)
        {
            if (_isCancelling)
            {
                _isCancelling = false;
                return;
            }
            CommitAndHide();
        }

        private void CommitAndHide()
        {
            if (_searchEditor == null) return;
            CommitSearch();
            DeactivateAndReturnFocus();
        }

        private void CommitSearch()
        {
            if (_searchEditor == null) return;
            var text = _searchEditor.Text;
            _grid.ToolbarState.SearchText = text;
            _grid.ApplyQuickFilter(text);
        }

        /// <summary>
        /// Hides the search editor and hands focus back to the grid so
        /// the next keypress is processed by the grid's OnKeyDown
        /// (which decides what to do based on the new state).  Without
        /// this, hiding a control in WinForms leaves focus in limbo
        /// and the user has to click somewhere to recover keyboard
        /// navigation.
        /// </summary>
        private void DeactivateAndReturnFocus()
        {
            if (_searchEditor == null) return;
            _searchEditor.Visible = false;
            _grid.ToolbarState.SearchHasFocus = false;
            // Move focus to the grid so subsequent keys route through
            // BeepGridPro.OnKeyDown.  Safe to call when the grid
            // already has focus (Focus() is idempotent).
            if (_grid.IsHandleCreated && !_grid.IsDisposed)
            {
                _grid.Focus();
            }
            _grid.SafeInvalidate(_grid.Layout.ToolbarRect);
        }

        public void HideSearchEditor()
        {
            // Always reset the state's focus flag, even when the editor
            // has never been created, so callers can use this as a
            // "make sure the search box isn't focused" teardown.
            _grid.ToolbarState.SearchHasFocus = false;
            if (_searchEditor == null) return;
            // Treat externally-initiated hide as a cancel: the caller is
            // closing the search box without confirming the typed text.
            _isCancelling = true;
            _grid.ToolbarState.SearchText = _searchEditor.Text;
            DeactivateAndReturnFocus();
        }

        /// <summary>
        /// Re-fits the editor bounds to a freshly-laid-out search box.
        /// Called by <see cref="BeepGridPro.OnResize"/> so the editor
        /// tracks the painted search box when the grid is resized.
        /// When the editor is hidden or the toolbar is collapsed the
        /// call is a no-op.
        /// </summary>
        public void ResizeIfActive(Rectangle toolbarBounds, Rectangle searchBoxBounds)
        {
            if (_searchEditor == null) return;
            if (!_searchEditor.Visible) return;
            if (toolbarBounds.IsEmpty || searchBoxBounds.IsEmpty) return;
            _searchEditor.Bounds = InnerEditorBounds(searchBoxBounds);
        }

        public void Dispose()
        {
            if (_searchEditor == null) return;
            _searchEditor.LostFocus -= OnSearchEditorLostFocus;
            _searchEditor.KeyDown -= OnSearchEditorKeyDown;
            _searchEditor.TextChanged -= OnSearchEditorTextChanged;
            _searchEditor.Dispose();
            _searchEditor = null;
        }

        /// <summary>
        /// True while the search textbox is the active focused control.
        /// Used by the keyboard handler to decide whether per-button
        /// shortcuts (Insert, F2, Delete) should fire or be left alone
        /// to the text editor.
        /// </summary>
        public bool IsSearchEditorFocused()
            => _searchEditor != null && !_searchEditor.IsDisposed && _searchEditor.Focused;
    }
}
