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
                    // and suppresses the border.
                    IsFrameless = true,
                    // IsChild makes ApplyTheme give the editor the parent's BackColor, which is
                    // what we want: a solid, theme-driven fill that matches the grid.
                    IsChild = true,
                    // The toolbar painter already draws the box AND its focus ring. IsFrameless
                    // suppresses the static border but not the focus animation, which drew a
                    // second rounded border just inside the painted one -- two nested boxes.
                    // The painter owns all the chrome; the editor supplies only text and caret.
                    EnableFocusAnimation = false,
                    ShowFocusIndicator = false,
                    BorderRadius = 0,
                    // Explicitly the parent's BackColor -- NOT Color.Transparent.
                    //
                    // Transparent never worked here anyway (ApplyTheme overwrites it because
                    // IsChild is set) and it rendered black, because a transparent BackColor makes
                    // the control paint its parent's background itself rather than showing it
                    // through. It also forces the child to repaint whenever the parent repaints,
                    // which is the coupling behind the flicker. A solid parent-matched fill is
                    // both stable and correct; the painter matches this colour for the box.
                    BackColor = _grid.BackColor,
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
        /// The editor's actual background colour, for the toolbar painter to fill the search box
        /// with while the editor is up.
        /// <para>
        /// The painter matches the editor rather than the other way round on purpose. BeepTextBox
        /// owns its BackColor -- ApplyTheme rewrites it from the theme (and, because IsChild is
        /// set, from the parent's colour) at times outside this class's control. Assigning it from
        /// here is either overwritten moments later or, if re-asserted from BackColorChanged, spins
        /// against ApplyTheme; an earlier attempt at that hung the control. Reading the colour and
        /// painting to match is stable, needs no coordination, and cannot loop.
        /// </para>
        /// </summary>
        internal Color? SearchEditorBackColor
            => _searchEditor != null && !_searchEditor.IsDisposed && _searchEditor.Visible
                ? _searchEditor.BackColor
                : null;

        /// <summary>
        /// Returns the editor bounds inside the painted search box, leaving
        /// the icon column on the left untouched.  Width is shrunk by the
        /// <see cref="BeepGridToolbarState.SearchIconWidth"/> scaled to DPI
        /// so the editor text aligns with the painted text rect.
        /// </summary>
        private Rectangle InnerEditorBounds(Rectangle bounds)
        {
            // The editor occupies exactly the rectangle the painter draws text into - one
            // definition, in BeepGridToolbarPainter.SearchTextArea. Computing it here as well is
            // what let the two disagree at the right edge, so text jumped by a few pixels when the
            // editor opened.
            //
            // Sitting strictly inside the painted border also matters: the editor used to take the
            // box's full height and run to its right edge, so its opaque fill covered the border
            // along the top, bottom and right and the box looked like it had lost three sides.
            float dpiScale = _grid.DeviceDpi / 96f;
            return Toolbar.BeepGridToolbarPainter.SearchTextArea(
                bounds, _grid.ToolbarState.SearchIconWidth, dpiScale);
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

            // No box to sit in. This used to return and leave the editor visible wherever it last
            // was, which is what put a floating text box at the wrong place after minimizing or
            // maximizing: minimizing collapses the client area, the toolbar layout resets, and the
            // search box rectangle becomes empty - so the editor was simply abandoned rather than
            // repositioned. The same now happens legitimately on a narrow toolbar, where the search
            // box is dropped on purpose.
            if (toolbarBounds.IsEmpty || searchBoxBounds.IsEmpty)
            {
                SuspendForLayout();
                return;
            }

            _searchEditor.Bounds = InnerEditorBounds(searchBoxBounds);
        }

        /// <summary>
        /// Puts the editor away because there is nowhere to draw it, keeping what was typed.
        /// </summary>
        /// <remarks>
        /// Deliberately not <see cref="HideSearchEditor"/>. That treats the hide as the user
        /// cancelling, and hands focus back to the grid - both wrong here. The user did not cancel
        /// anything; the window was minimized or the toolbar ran out of room. And calling
        /// <c>Focus()</c> while a form is minimizing is at best pointless.
        /// <para>
        /// The typed text is carried into <c>ToolbarState.SearchText</c> so it survives, is painted
        /// in the box once there is room again, and is there to resume from when the user reopens
        /// the editor.
        /// </para>
        /// </remarks>
        internal void SuspendForLayout()
        {
            if (_searchEditor == null || _searchEditor.IsDisposed || !_searchEditor.Visible)
                return;

            _grid.ToolbarState.SearchText = _searchEditor.Text;
            _grid.ToolbarState.SearchHasFocus = false;
            _searchEditor.Visible = false;
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

        /// <summary>
        /// True while the editor is on screen, whether or not it currently holds focus.
        /// The toolbar painter reads this to suppress the painted search text so the editor is
        /// the only thing drawing in the text area.
        /// </summary>
        internal bool IsSearchEditorVisible
            => _searchEditor != null && !_searchEditor.IsDisposed && _searchEditor.Visible;
    }
}
