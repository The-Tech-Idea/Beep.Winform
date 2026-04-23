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
                _grid.Controls.Add(_searchEditor);
            }

            _searchEditor.Bounds = bounds;
            _searchEditor.Text = _grid.ToolbarState.SearchText;
            _searchEditor.Visible = true;
            _searchEditor.Focus();
            _searchEditor.SelectAll();
        }

        public void HideSearchEditor()
        {
            if (_searchEditor != null)
            {
                _searchEditor.Visible = false;
            }
        }

        private void OnSearchEditorKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitSearch();
                _searchEditor!.Visible = false;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _searchEditor!.Visible = false;
            }
        }

        private void OnSearchEditorLostFocus(object? sender, EventArgs e)
        {
            CommitSearch();
            _searchEditor!.Visible = false;
        }

        private void CommitSearch()
        {
            if (_searchEditor != null)
            {
                _grid.ToolbarState.SearchText = _searchEditor.Text;
                _grid.ApplyQuickFilter(_searchEditor.Text);
            }
        }

        public void Dispose()
        {
            if (_searchEditor != null)
            {
                _searchEditor.LostFocus -= OnSearchEditorLostFocus;
                _searchEditor.KeyDown -= OnSearchEditorKeyDown;
                _searchEditor.Dispose();
                _searchEditor = null;
            }
        }
    }
}
