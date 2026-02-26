using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDataBlock
    {
        private readonly Dictionary<string, BeepLabel> _recordFieldLabels = new(StringComparer.OrdinalIgnoreCase);
        private int _recordLabelWidth = 150;
        private int _recordRowSpacing = 8;
        private int _recordColumnCount = 1;
        private int _recordEditorHeight = 30;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(150)]
        [Description("Label width used by RecordControls layout.")]
        public int RecordLabelWidth
        {
            get => _recordLabelWidth;
            set
            {
                _recordLabelWidth = Math.Max(80, value);
                if (ViewMode == DataBlockViewMode.RecordControls)
                {
                    ApplyRecordLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(8)]
        [Description("Vertical spacing between rows in RecordControls layout.")]
        public int RecordRowSpacing
        {
            get => _recordRowSpacing;
            set
            {
                _recordRowSpacing = Math.Max(0, value);
                if (ViewMode == DataBlockViewMode.RecordControls)
                {
                    ApplyRecordLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(1)]
        [Description("Number of columns in RecordControls layout.")]
        public int RecordColumnCount
        {
            get => _recordColumnCount;
            set
            {
                _recordColumnCount = Math.Max(1, value);
                if (ViewMode == DataBlockViewMode.RecordControls)
                {
                    ApplyRecordLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(30)]
        [Description("Editor height in RecordControls layout.")]
        public int RecordEditorHeight
        {
            get => _recordEditorHeight;
            set
            {
                _recordEditorHeight = Math.Max(22, value);
                if (ViewMode == DataBlockViewMode.RecordControls)
                {
                    ApplyRecordLayout();
                }
            }
        }

        private void EnsureGridControl()
        {
            if (_gridView != null)
            {
                return;
            }

            _gridView = Controls.OfType<BeepGridPro>().FirstOrDefault();
            if (_gridView != null)
            {
                return;
            }

            _gridView = new BeepGridPro
            {
                Name = string.IsNullOrWhiteSpace(Name) ? "DataBlockGrid" : $"{Name}_Grid",
                Dock = DockStyle.Fill,
                Visible = false
            };

            Controls.Add(_gridView);
        }

        private void UpdateGridDataBinding()
        {
            if (_gridView == null)
            {
                return;
            }

            try
            {
                _gridView.DataSource = Data?.Units;
                ConfigureGridColumnsFromFieldSelections();
            }
            catch
            {
                // Ignore grid binding errors in design-time.
            }
        }

        internal void ConfigureGridColumnsFromFieldSelections()
        {
            if (_gridView?.Columns == null || _gridView.Columns.Count == 0)
            {
                return;
            }

            var resolvedEditors = ResolveIncludedFieldEditorTypes();
            var configuredFields = _fieldSelections
                .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                .Select(x => x.FieldName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var visibleFields = resolvedEditors.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var column in _gridView.Columns)
            {
                if (column.IsRowID || column.IsRowNumColumn || column.IsSelectionCheckBox)
                {
                    continue;
                }

                if (configuredFields.Count == 0)
                {
                    column.Visible = true;
                    continue;
                }

                column.Visible = visibleFields.Contains(column.ColumnName);
            }
        }

        private void ApplyViewMode()
        {
            bool showGrid = _viewMode == DataBlockViewMode.BeepGridPro;

            if (showGrid)
            {
                EnsureGridControl();
                UpdateGridDataBinding();
            }

            if (_gridView != null)
            {
                _gridView.Visible = showGrid;
                if (showGrid)
                {
                    _gridView.BringToFront();
                }
            }

            foreach (var component in UIComponents.Values)
            {
                if (component is Control winFormsControl)
                {
                    winFormsControl.Visible = !showGrid;
                }
            }

            if (showGrid)
            {
                SetRecordLabelsVisible(false);
                return;
            }

            ApplyRecordLayout();
            SetRecordLabelsVisible(true);
        }

        private void ApplyRecordLayout()
        {
            var controls = GetRecordControlsInDisplayOrder();
            if (controls.Count == 0)
            {
                RemoveOrphanedLabels(new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                return;
            }

            int padding = 10;
            int availableWidth = Math.Max(320, ClientSize.Width - (padding * 2));
            int colCount = Math.Max(1, RecordColumnCount);
            int colWidth = Math.Max(220, availableWidth / colCount);
            int rowHeight = Math.Max(22, RecordEditorHeight);
            int fieldSpacing = 8;
            int y = padding;

            var fieldSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < controls.Count; i++)
            {
                var control = controls[i];
                var fieldName = GetFieldNameForControl(control);
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    continue;
                }

                fieldSet.Add(fieldName);

                int row = i / colCount;
                int col = i % colCount;
                int colX = padding + (col * colWidth);
                y = padding + (row * (rowHeight + RecordRowSpacing + 4));

                var label = GetOrCreateFieldLabel(fieldName);
                label.Bounds = new System.Drawing.Rectangle(colX, y, Math.Min(RecordLabelWidth, colWidth - 60), rowHeight);
                label.Text = fieldName;
                label.Visible = true;

                int editorLeft = label.Right + fieldSpacing;
                int editorWidth = Math.Max(120, colX + colWidth - editorLeft - fieldSpacing);
                control.Bounds = new System.Drawing.Rectangle(editorLeft, y, editorWidth, rowHeight);
                control.Visible = true;
            }

            RemoveOrphanedLabels(fieldSet);
        }

        private List<Control> GetRecordControlsInDisplayOrder()
        {
            var result = new List<Control>();
            foreach (var component in Components.Where(x => !string.IsNullOrWhiteSpace(x.BoundProperty)))
            {
                if (UIComponents.TryGetValue(component.GUID, out var uiComponent) && uiComponent is Control control)
                {
                    result.Add(control);
                }
            }

            return result;
        }

        private string GetFieldNameForControl(Control control)
        {
            var component = UIComponents.Values.FirstOrDefault(x => ReferenceEquals(x, control));
            return component?.BoundProperty ?? component?.ComponentName ?? control.Name;
        }

        private BeepLabel GetOrCreateFieldLabel(string fieldName)
        {
            if (_recordFieldLabels.TryGetValue(fieldName, out var existing) && !existing.IsDisposed)
            {
                return existing;
            }

            var label = new BeepLabel
            {
                Name = $"{Name}_{fieldName}_Label",
                Text = fieldName,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            Controls.Add(label);
            label.BringToFront();
            _recordFieldLabels[fieldName] = label;
            return label;
        }

        private void RemoveOrphanedLabels(HashSet<string> activeFields)
        {
            var toRemove = _recordFieldLabels.Keys
                .Where(k => !activeFields.Contains(k))
                .ToList();

            foreach (var key in toRemove)
            {
                if (_recordFieldLabels.TryGetValue(key, out var label))
                {
                    Controls.Remove(label);
                    label.Dispose();
                }

                _recordFieldLabels.Remove(key);
            }
        }

        private void SetRecordLabelsVisible(bool visible)
        {
            foreach (var label in _recordFieldLabels.Values)
            {
                label.Visible = visible;
                if (visible)
                {
                    label.BringToFront();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (ViewMode == DataBlockViewMode.RecordControls)
            {
                ApplyRecordLayout();
            }
        }
    }
}
