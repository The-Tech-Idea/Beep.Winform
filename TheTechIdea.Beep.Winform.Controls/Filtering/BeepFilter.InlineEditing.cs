using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// The inline value editor: the real <see cref="Control"/> that appears over a criterion while
    /// its value is being typed.
    /// </summary>
    /// <remarks>
    /// Moved out of <c>BeepFilter.cs</c>, which had grown to 1,358 lines. This block is a cohesive
    /// concern - create the editor, place it over the criterion, commit or cancel, convert the typed
    /// text to the column's type, tear it down - and it was the largest single thing in a region
    /// named "Filter Management Methods".
    ///
    /// That region was expected to duplicate <c>FilterCriteria</c>, <c>FilterValidationHelper</c> or
    /// <c>FilterEngine</c>. It does not: it is UI interaction, and it belongs to the control. The
    /// expectation was recorded in plans/03 and is corrected there.
    /// </remarks>
    public partial class BeepFilter
    {
        private bool BeginInlineValueEdit(int index, Rectangle bounds)
        {
            if (index < 0 || index >= _activeFilter.Criteria.Count)
            {
                return false;
            }

            var criterion = _activeFilter.Criteria[index];
            if (criterion == null)
            {
                return false;
            }

            if (criterion.Operator == FilterOperator.IsNull || criterion.Operator == FilterOperator.IsNotNull)
            {
                criterion.Value = string.Empty;
                criterion.Value2 = string.Empty;
                OnFilterModified(index);
                RecalculateLayout();
                Invalidate();
                return true;
            }

            EnsureInlineValueEditor();
            if (_inlineValueEditor == null)
            {
                return false;
            }

            var editBounds = NormalizeInlineEditorBounds(bounds, index);
            _inlineEditIndex = index;

            if (criterion.Operator == FilterOperator.Between || criterion.Operator == FilterOperator.NotBetween)
            {
                var left = criterion.Value?.ToString() ?? string.Empty;
                var right = criterion.Value2?.ToString() ?? string.Empty;
                _inlineValueEditor.Text = string.IsNullOrWhiteSpace(right) ? left : $"{left} | {right}";
            }
            else
            {
                _inlineValueEditor.Text = criterion.Value?.ToString() ?? string.Empty;
            }

            _inlineValueEditor.Bounds = editBounds;
            _inlineValueEditor.Visible = true;
            _inlineValueEditor.BringToFront();
            _inlineValueEditor.Focus();
            _inlineValueEditor.SelectAll();

            return true;
        }

        private Rectangle NormalizeInlineEditorBounds(Rectangle bounds, int index)
        {
            var editBounds = bounds;

            if (editBounds.Width <= 0 || editBounds.Height <= 0)
            {
                if (_currentLayout.ValueDropdownRects != null && index >= 0 && index < _currentLayout.ValueDropdownRects.Length)
                {
                    editBounds = _currentLayout.ValueDropdownRects[index];
                }
                else if (_currentLayout.RowRects != null && index >= 0 && index < _currentLayout.RowRects.Length)
                {
                    editBounds = _currentLayout.RowRects[index];
                }
                else
                {
                    editBounds = new Rectangle(8, 8, Math.Max(120, Width - 16), 28);
                }
            }

            if (editBounds.Height < 24)
            {
                editBounds.Height = 24;
            }

            if (editBounds.Width < 120)
            {
                editBounds.Width = 120;
            }

            editBounds.Inflate(-1, -1);

            if (editBounds.Right > ClientRectangle.Right)
            {
                editBounds.X = Math.Max(0, ClientRectangle.Right - editBounds.Width);
            }

            if (editBounds.Bottom > ClientRectangle.Bottom)
            {
                editBounds.Y = Math.Max(0, ClientRectangle.Bottom - editBounds.Height);
            }

            return editBounds;
        }

        private void EnsureInlineValueEditor()
        {
            if (_inlineValueEditor != null)
            {
                return;
            }

            _inlineValueEditor = new BeepTextBox
            {
                Visible = false,
                IsChild = true,
                Theme = Theme,
                TabStop = true
            };

            _inlineValueEditor.KeyDown += InlineValueEditor_KeyDown;
            _inlineValueEditor.LostFocus += InlineValueEditor_LostFocus;
            Controls.Add(_inlineValueEditor);
        }

        private void InlineValueEditor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommitInlineValueEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                CancelInlineValueEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void InlineValueEditor_LostFocus(object? sender, EventArgs e)
        {
            CommitInlineValueEdit();
        }

        private void CommitInlineValueEdit()
        {
            if (_isCommittingInlineEdit)
            {
                return;
            }

            if (_inlineValueEditor == null || !_inlineValueEditor.Visible)
            {
                return;
            }

            if (_inlineEditIndex < 0 || _inlineEditIndex >= _activeFilter.Criteria.Count)
            {
                HideInlineValueEditor();
                return;
            }

            _isCommittingInlineEdit = true;
            try
            {
                var criterion = _activeFilter.Criteria[_inlineEditIndex];
                var rawText = _inlineValueEditor.Text ?? string.Empty;

                if (_isQuickSearchInlineEdit)
                {
                    criterion.Operator = FilterOperator.Contains;
                    if (string.IsNullOrWhiteSpace(criterion.ColumnName))
                    {
                        var available = AvailableColumns;
                        criterion.ColumnName = available.Count > 0 ? available[0].ColumnName : "All Columns";
                    }

                    criterion.Value = rawText.Trim();
                    criterion.Value2 = string.Empty;
                    OnFilterChanged();
                    RecalculateLayout();
                    Invalidate();
                    return;
                }

                if (criterion.Operator == FilterOperator.Between || criterion.Operator == FilterOperator.NotBetween)
                {
                    var parts = rawText.Split(new[] { "|", ".." }, StringSplitOptions.None);
                    var left = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                    var right = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                    criterion.Value = ConvertInlineText(left, criterion.ColumnName);
                    criterion.Value2 = ConvertInlineText(right, criterion.ColumnName);
                }
                else
                {
                    criterion.Value = ConvertInlineText(rawText.Trim(), criterion.ColumnName);
                }

                OnFilterModified(_inlineEditIndex);
                RecalculateLayout();
                Invalidate();
            }
            finally
            {
                HideInlineValueEditor();
                _isCommittingInlineEdit = false;
            }
        }

        private object ConvertInlineText(string text, string columnName)
        {
            var column = AvailableColumns?.Find(c => c.ColumnName == columnName);
            var targetType = column?.DataType ?? typeof(string);

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (targetType == typeof(string)) return text;

            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (nonNullableType == typeof(int) && int.TryParse(text, out var i)) return i;
                if (nonNullableType == typeof(long) && long.TryParse(text, out var l)) return l;
                if (nonNullableType == typeof(decimal) && decimal.TryParse(text, out var m)) return m;
                if (nonNullableType == typeof(double) && double.TryParse(text, out var d)) return d;
                if (nonNullableType == typeof(float) && float.TryParse(text, out var f)) return f;
                if (nonNullableType == typeof(bool) && bool.TryParse(text, out var b)) return b;
                if (nonNullableType == typeof(DateTime) && DateTime.TryParse(text, out var dt)) return dt;

                return Convert.ChangeType(text, nonNullableType);
            }
            catch (Exception ex) when (ex is InvalidCastException
                                       || ex is FormatException
                                       || ex is OverflowException)
            {
                // The user is mid-type: "12" on the way to "12.5", or a date not yet complete.
                // Keeping the raw text is correct, and these are the only failures that mean
                // "not convertible yet" rather than "something is broken".
                return text;
            }
        }

        private void CancelInlineValueEdit()
        {
            HideInlineValueEditor();
            Focus();
        }

        private void HideInlineValueEditor()
        {
            if (_inlineValueEditor != null)
            {
                _inlineValueEditor.Visible = false;
            }

            _inlineEditIndex = -1;
            _isQuickSearchInlineEdit = false;
        }
    }
}
