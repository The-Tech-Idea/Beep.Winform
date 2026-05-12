using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Editors
{
    /// <summary>
    /// Provides inline editing capabilities for BeepTree cells.
    /// Supports text, combo box, checkbox, and date picker editors.
    /// </summary>
    public class BeepTreeCellEditor : IDisposable
    {
        private readonly BeepTree _owner;
        private Control _activeEditor;
        private SimpleItem _editingNode;
        private BeepTreeColumn _editingColumn;
        private int _editingColumnIndex;
        private Rectangle _editorBounds;
        private bool _isEditing = false;

        // Editor type constants
        public const string EDITOR_TEXTBOX = "TextBox";
        public const string EDITOR_COMBOBOX = "ComboBox";
        public const string EDITOR_CHECKBOX = "CheckBox";
        public const string EDITOR_DATETIMEPICKER = "DateTimePicker";

        public BeepTreeCellEditor(BeepTree owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Properties

        /// <summary>
        /// Gets whether an edit operation is currently active.
        /// </summary>
        public bool IsEditing => _isEditing;

        /// <summary>
        /// Gets the node currently being edited.
        /// </summary>
        public SimpleItem EditingNode => _editingNode;

        /// <summary>
        /// Gets the column currently being edited.
        /// </summary>
        public BeepTreeColumn EditingColumn => _editingColumn;

        #endregion

        #region Edit Methods

        /// <summary>
        /// Begins editing the specified node's cell.
        /// </summary>
        public bool BeginEdit(SimpleItem node, int columnIndex)
        {
            if (node == null || _isEditing)
                return false;

            EndEdit(false);

            // Determine column
            BeepTreeColumn column = null;
            if (_owner.IsMultiColumn && _owner.Columns != null)
            {
                var visibleColumns = _owner.Columns.GetVisibleColumns().ToList();
                if (columnIndex >= 0 && columnIndex < visibleColumns.Count)
                {
                    column = visibleColumns[columnIndex];
                }
            }

            // Calculate editor bounds
            var bounds = GetCellBounds(node, columnIndex);
            if (bounds.IsEmpty)
                return false;

            _editingNode = node;
            _editingColumn = column;
            _editingColumnIndex = columnIndex;
            _editorBounds = bounds;

            // Create appropriate editor
            var editorType = column?.EditorType ?? EDITOR_TEXTBOX;
            _activeEditor = CreateEditor(editorType, node, column);

            if (_activeEditor == null)
                return false;

            // Position and show editor
            _activeEditor.Bounds = bounds;
            _activeEditor.Visible = true;
            _owner.Controls.Add(_activeEditor);
            _activeEditor.Focus();

            // Wire up events
            WireEditorEvents(_activeEditor);

            _isEditing = true;
            return true;
        }

        /// <summary>
        /// Begins editing the first column of the specified node.
        /// </summary>
        public bool BeginEdit(SimpleItem node)
        {
            return BeginEdit(node, 0);
        }

        /// <summary>
        /// Ends the current edit operation.
        /// </summary>
        public bool EndEdit(bool acceptChanges)
        {
            if (!_isEditing || _activeEditor == null)
                return false;

            bool result = false;

            if (acceptChanges)
            {
                // Validate
                var validatingArgs = new BeepTreeNodeValidatingEventArgs(_editingNode, _editingColumn, GetEditorValue());
                _owner.OnNodeValidating(validatingArgs);

                if (validatingArgs.Cancel)
                {
                    // Validation failed, keep editing
                    return false;
                }

                // Apply changes
                ApplyEditorValue();
                result = true;
            }

            // Clean up
            CleanupEditor();

            _isEditing = false;
            _editingNode = null;
            _editingColumn = null;

            return result;
        }

        /// <summary>
        /// Cancels the current edit operation without saving changes.
        /// </summary>
        public void CancelEdit()
        {
            EndEdit(false);
        }

        #endregion

        #region Editor Creation

        private Control CreateEditor(string type, SimpleItem node, BeepTreeColumn column)
        {
            switch (type)
            {
                case EDITOR_COMBOBOX:
                    return CreateComboBoxEditor(node, column);
                case EDITOR_CHECKBOX:
                    return CreateCheckBoxEditor(node, column);
                case EDITOR_DATETIMEPICKER:
                    return CreateDateTimePickerEditor(node, column);
                case EDITOR_TEXTBOX:
                default:
                    return CreateTextBoxEditor(node, column);
            }
        }

        private TextBox CreateTextBoxEditor(SimpleItem node, BeepTreeColumn column)
        {
            var textBox = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Text = GetCellValue(node, column),
                Font = _owner.TextFont ?? SystemFonts.DefaultFont
            };
            return textBox;
        }

        private ComboBox CreateComboBoxEditor(SimpleItem node, BeepTreeColumn column)
        {
            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = _owner.TextFont ?? SystemFonts.DefaultFont
            };

            if (column?.ComboBoxItems != null)
            {
                comboBox.Items.AddRange(column.ComboBoxItems.ToArray());
            }

            var currentValue = GetCellValue(node, column);
            comboBox.SelectedItem = currentValue;

            return comboBox;
        }

        private CheckBox CreateCheckBoxEditor(SimpleItem node, BeepTreeColumn column)
        {
            var checkBox = new CheckBox
            {
                Text = "",
                Checked = GetCellValue(node, column)?.ToString().Equals("true", StringComparison.OrdinalIgnoreCase) == true
            };
            return checkBox;
        }

        private DateTimePicker CreateDateTimePickerEditor(SimpleItem node, BeepTreeColumn column)
        {
            var picker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = _owner.TextFont ?? SystemFonts.DefaultFont
            };

            var value = GetCellValue(node, column);
            if (DateTime.TryParse(value?.ToString(), out var parsedDate))
            {
                picker.Value = parsedDate;
            }

            return picker;
        }

        #endregion

        #region Editor Events

        private void WireEditorEvents(Control editor)
        {
            editor.KeyDown += Editor_KeyDown;
            editor.LostFocus += Editor_LostFocus;

            if (editor is TextBox textBox)
            {
                textBox.Validating += (s, e) =>
                {
                    var args = new BeepTreeNodeValidatingEventArgs(_editingNode, _editingColumn, textBox.Text);
                    _owner.OnNodeValidating(args);
                    e.Cancel = args.Cancel;
                };
            }
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    EndEdit(true);
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    CancelEdit();
                    e.Handled = true;
                    break;
            }
        }

        private void Editor_LostFocus(object sender, EventArgs e)
        {
            // Delayed end edit to allow focus to move to another control
            if (_isEditing)
            {
                var timer = new Timer { Interval = 100 };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    if (_isEditing && !_owner.ContainsFocus)
                    {
                        EndEdit(true);
                    }
                };
                timer.Start();
            }
        }

        #endregion

        #region Value Management

        private string GetCellValue(SimpleItem node, BeepTreeColumn column)
        {
            if (node == null)
                return string.Empty;

            if (column != null && !string.IsNullOrEmpty(column.FieldName))
            {
                if (node.Data != null && node.Data.ContainsKey(column.FieldName))
                {
                    return node.Data[column.FieldName]?.ToString() ?? string.Empty;
                }
            }

            return node.Text ?? string.Empty;
        }

        private object GetEditorValue()
        {
            if (_activeEditor == null)
                return null;

            switch (_activeEditor)
            {
                case TextBox textBox:
                    return textBox.Text;
                case ComboBox comboBox:
                    return comboBox.SelectedItem ?? comboBox.Text;
                case CheckBox checkBox:
                    return checkBox.Checked;
                case DateTimePicker picker:
                    return picker.Value;
                default:
                    return _activeEditor.Text;
            }
        }

        private void ApplyEditorValue()
        {
            if (_editingNode == null)
                return;

            var value = GetEditorValue();

            if (_editingColumn != null && !string.IsNullOrEmpty(_editingColumn.FieldName))
            {
                // Update Data dictionary for multi-column
                if (_editingNode.Data == null)
                    _editingNode.Data = new Dictionary<string, object>();
                _editingNode.Data[_editingColumn.FieldName] = value;
            }
            else
            {
                // Update node text for single-column
                _editingNode.Text = value?.ToString() ?? string.Empty;
            }

            // Fire NodeEndEdit event
            _owner.OnNodeEndEdit(_editingNode, _editingColumn, value);
        }

        #endregion

        #region Bounds Calculation

        private Rectangle GetCellBounds(SimpleItem node, int columnIndex)
        {
            if (_owner.LayoutHelper == null)
                return Rectangle.Empty;

            var layout = _owner.LayoutHelper.GetCachedLayout();
            var nodeInfo = layout?.FirstOrDefault(n => n.Item == node);
            if (!nodeInfo.HasValue)
                return Rectangle.Empty;

            Rectangle cellRect;
            if (_owner.IsMultiColumn && columnIndex > 0)
            {
                cellRect = nodeInfo.Value.GetCellRect(columnIndex);
            }
            else
            {
                cellRect = nodeInfo.Value.TextRectContent;
            }

            return _owner.LayoutHelper.TransformToViewport(cellRect);
        }

        #endregion

        #region Cleanup

        private void CleanupEditor()
        {
            if (_activeEditor != null)
            {
                _activeEditor.KeyDown -= Editor_KeyDown;
                _activeEditor.LostFocus -= Editor_LostFocus;

                if (_owner.Controls.Contains(_activeEditor))
                {
                    _owner.Controls.Remove(_activeEditor);
                }

                _activeEditor.Dispose();
                _activeEditor = null;
            }
        }

        public void Dispose()
        {
            CleanupEditor();
        }

        #endregion
    }

    #region Event Args

    /// <summary>
    /// Event arguments for node validation during editing.
    /// </summary>
    public class BeepTreeNodeValidatingEventArgs : EventArgs
    {
        /// <summary>The node being edited.</summary>
        public SimpleItem Node { get; }

        /// <summary>The column being edited (null for single-column mode).</summary>
        public BeepTreeColumn Column { get; }

        /// <summary>The proposed new value.</summary>
        public object ProposedValue { get; }

        /// <summary>Set to true to cancel the edit.</summary>
        public bool Cancel { get; set; }

        /// <summary>Error message if validation fails.</summary>
        public string ErrorMessage { get; set; }

        public BeepTreeNodeValidatingEventArgs(SimpleItem node, BeepTreeColumn column, object proposedValue)
        {
            Node = node;
            Column = column;
            ProposedValue = proposedValue;
        }
    }

    /// <summary>
    /// Event arguments for node edit completion.
    /// </summary>
    public class BeepTreeNodeEndEditEventArgs : EventArgs
    {
        /// <summary>The node that was edited.</summary>
        public SimpleItem Node { get; }

        /// <summary>The column that was edited.</summary>
        public BeepTreeColumn Column { get; }

        /// <summary>The new value.</summary>
        public object Value { get; }

        public BeepTreeNodeEndEditEventArgs(SimpleItem node, BeepTreeColumn column, object value)
        {
            Node = node;
            Column = column;
            Value = value;
        }
    }

    #endregion
}
