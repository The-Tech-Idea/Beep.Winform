using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filtering
{
    /// <summary>
    /// Manages on-demand activation of the search textbox overlay in the unified toolbar.
    /// Only creates a real control when the user clicks the painted search box.
    /// </summary>
    internal class FilterEditorHelper
    {
        private readonly BeepGridPro _grid;
        private BeepTextBox? _searchEditor;

        public FilterEditorHelper(BeepGridPro grid) { _grid = grid; }

        public void ActivateSearchEditor(Rectangle bounds)
        {
            if (_searchEditor == null)
            {
                _searchEditor = new BeepTextBox
                {
                    IsChild = true,
                    IsFrameless = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = _grid.Font
                };
                _searchEditor.LostFocus += OnSearchEditorLostFocus;
                _searchEditor.KeyDown += OnSearchEditorKeyDown;
                _searchEditor.TextChanged += OnSearchEditorTextChanged;
                _grid.Controls.Add(_searchEditor);
            }

            // Position editor to match the search box rect exactly
            _searchEditor.Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            _searchEditor.Text = _grid.ToolbarState.SearchText;
            _searchEditor.Visible = true;
            _searchEditor.Focus();
            _searchEditor.SelectAll();
        }

        private void OnSearchEditorTextChanged(object? sender, EventArgs e)
        {
            // Trigger toolbar repaint to update painted search text in sync
            _grid.ToolbarState.SearchText = _searchEditor?.Text ?? string.Empty;
            _grid.SafeInvalidate(_grid.Layout.ToolbarRect);
        }

        private void OnSearchEditorKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitSearch();
                _searchEditor!.Visible = false;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _grid.ToolbarState.SearchText = _searchEditor?.Text ?? string.Empty;
                _searchEditor!.Visible = false;
                _grid.SafeInvalidate(_grid.Layout.ToolbarRect);
            }
        }

        private void OnSearchEditorLostFocus(object? sender, EventArgs e)
        {
            CommitSearch();
            _searchEditor!.Visible = false;
            _grid.SafeInvalidate(_grid.Layout.ToolbarRect);
        }

        private void CommitSearch()
        {
            if (_searchEditor != null)
            {
                _grid.ToolbarState.SearchText = _searchEditor.Text;
                _grid.ApplyQuickFilter(_searchEditor.Text);
            }
        }

        public void HideSearchEditor()
        {
            if (_searchEditor != null)
            {
                // Commit current text before hiding
                _grid.ToolbarState.SearchText = _searchEditor.Text;
                _searchEditor.Visible = false;
                _grid.SafeInvalidate(_grid.Layout.ToolbarRect);
            }
        }

        public void Dispose()
        {
            if (_searchEditor != null)
            {
                _searchEditor.LostFocus -= OnSearchEditorLostFocus;
                _searchEditor.KeyDown -= OnSearchEditorKeyDown;
                _searchEditor.TextChanged -= OnSearchEditorTextChanged;
                _searchEditor.Dispose();
                _searchEditor = null;
            }
        }
    }
}
