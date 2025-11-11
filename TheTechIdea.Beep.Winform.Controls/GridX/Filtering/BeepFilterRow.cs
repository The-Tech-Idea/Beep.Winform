using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filtering
{
    /// <summary>
    /// Modern filter row control for building filter expressions
    /// Uses BeepControls for a consistent modern appearance
    /// </summary>
    public class BeepFilterRow : BaseControl
    {
        private BeepComboBox _columnCombo = null!;
        private BeepComboBox _operatorCombo = null!;
        private BeepTextBox _valueTextBox = null!;
        private BeepTextBox _value2TextBox = null!; // For Between operator
        private BeepButton _removeButton = null!;
        private BeepLabel _andLabel = null!; // Shows "and" between values for Between operator

        private Type? _currentColumnType;

        /// <summary>
        /// Raised when filter changes
        /// </summary>
        public event EventHandler? FilterChanged;
        
        /// <summary>
        /// Raised when remove is requested
        /// </summary>
        public event EventHandler? RemoveRequested;

        /// <summary>
        /// Gets or sets the available columns for filtering
        /// </summary>
        public string[] AvailableColumns
        {
            get => _columnCombo.Items.Cast<SimpleItem>().Select(s => s.Name).ToArray();
            set
            {
                _columnCombo.Items.Clear();
                if (value != null)
                {
                    foreach (var col in value)
                    {
                        _columnCombo.Items.Add(new SimpleItem { Name = col, Text = col });
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected column name
        /// </summary>
        public string? SelectedColumn
        {
            get => (_columnCombo.SelectedItem as SimpleItem)?.Name;
            set
            {
                var item = _columnCombo.Items.Cast<SimpleItem>().FirstOrDefault(s => s.Name == value);
                if (item != null)
                    _columnCombo.SelectedItem = item;
            }
        }

        /// <summary>
        /// Gets or sets the selected filter operator
        /// </summary>
        public FilterOperator SelectedOperator
        {
            get
            {
                var selectedItem = _operatorCombo.SelectedItem as SimpleItem;
                if (selectedItem != null && selectedItem.Tag is FilterOperator op)
                    return op;
                return FilterOperator.Contains;
            }
            set
            {
                var item = _operatorCombo.Items.Cast<SimpleItem>()
                    .FirstOrDefault(s => s.Tag is FilterOperator op && op == value);
                if (item != null)
                    _operatorCombo.SelectedItem = item;
            }
        }

        /// <summary>
        /// Gets or sets the primary filter value
        /// </summary>
        public string Value
        {
            get => _valueTextBox.Text;
            set => _valueTextBox.Text = value;
        }

        /// <summary>
        /// Gets or sets the secondary filter value (for Between operator)
        /// </summary>
        public string Value2
        {
            get => _value2TextBox.Text;
            set => _value2TextBox.Text = value;
        }

        /// <summary>
        /// Sets the column type to determine appropriate operators
        /// </summary>
        public void SetColumnType(Type columnType)
        {
            _currentColumnType = columnType;
            LoadOperators();
        }

        /// <summary>
        /// Initializes a new instance of the BeepFilterRow class
        /// </summary>
        public BeepFilterRow()
        {
            IsFrameless = true;
            ShowAllBorders = false;
            ShowShadow = false;
            
            InitializeControls();
            LayoutControls();
            HookEvents();
        }

        private void InitializeControls()
        {
            // Column selector
            _columnCombo = new BeepComboBox
            {
                Width = 150,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };

            // Operator selector
            _operatorCombo = new BeepComboBox
            {
                Width = 180,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };

            // Primary value input
            _valueTextBox = new BeepTextBox
            {
                Width = 150,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true,
                PlaceholderText = "Enter value..."
            };

            // Secondary value input (for Between)
            _value2TextBox = new BeepTextBox
            {
                Width = 150,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true,
                PlaceholderText = "Enter value...",
                Visible = false
            };

            // "and" label between values
            _andLabel = new BeepLabel
            {
                Text = "and",
                Width = 40,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                IsChild = true,
                IsFrameless = true,
                Visible = false
            };

            // Remove button
            _removeButton = new BeepButton
            {
                Text = "✕",
                Width = 30,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };

            Controls.Add(_columnCombo);
            Controls.Add(_operatorCombo);
            Controls.Add(_valueTextBox);
            Controls.Add(_andLabel);
            Controls.Add(_value2TextBox);
            Controls.Add(_removeButton);

            // Load default operators (text)
            LoadOperators();
        }

        private void LoadOperators()
        {
            _operatorCombo.Items.Clear();

            var operators = FilterOperatorExtensions.GetOperatorsForType(_currentColumnType ?? typeof(string));
            foreach (var op in operators)
            {
                _operatorCombo.Items.Add(new SimpleItem
                {
                    Name = op.ToString(),
                    Text = $"{op.GetSymbol()} {op.GetDisplayName()}",
                    Tag = op
                });
            }

            if (_operatorCombo.Items.Count > 0)
                _operatorCombo.SelectedIndex = 0;
        }

        private void LayoutControls()
        {
            int margin = 5;
            int x = margin;
            int y = margin;

            _columnCombo.Location = new Point(x, y);
            x += _columnCombo.Width + margin;

            _operatorCombo.Location = new Point(x, y);
            x += _operatorCombo.Width + margin;

            _valueTextBox.Location = new Point(x, y);
            x += _valueTextBox.Width + margin;

            _andLabel.Location = new Point(x, y);
            x += _andLabel.Width + margin;

            _value2TextBox.Location = new Point(x, y);
            x += _value2TextBox.Width + margin;

            _removeButton.Location = new Point(x, y);
            x += _removeButton.Width + margin;

            this.Height = _columnCombo.Height + (margin * 2);
            this.Width = x;
        }

        private void HookEvents()
        {
            _columnCombo.SelectedIndexChanged += (s, e) => OnFilterChanged();
            _operatorCombo.SelectedIndexChanged += (s, e) =>
            {
                UpdateValueFieldsVisibility();
                OnFilterChanged();
            };
            _valueTextBox.TextChanged += (s, e) => OnFilterChanged();
            _value2TextBox.TextChanged += (s, e) => OnFilterChanged();
            _removeButton.Click += (s, e) => RemoveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateValueFieldsVisibility()
        {
            var op = SelectedOperator;
            bool showValue2 = (op == FilterOperator.Between || op == FilterOperator.NotBetween);
            bool showValue1 = !(op == FilterOperator.IsNull || op == FilterOperator.IsNotNull);

            _valueTextBox.Visible = showValue1;
            _andLabel.Visible = showValue2;
            _value2TextBox.Visible = showValue2;

            LayoutControls();
        }

        private void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the filter criteria from this row
        /// </summary>
        public FilterCriteria? GetFilterCriteria()
        {
            var columnName = SelectedColumn;
            if (string.IsNullOrEmpty(columnName))
                return null;

            var op = SelectedOperator;
            var value = Value;
            var value2 = Value2;

            if (op == FilterOperator.Between || op == FilterOperator.NotBetween)
            {
                return new FilterCriteria(columnName, op, value, value2);
            }
            else
            {
                return new FilterCriteria(columnName, op, value);
            }
        }

        /// <summary>
        /// Sets the filter criteria in this row
        /// </summary>
        public void SetFilterCriteria(FilterCriteria criteria)
        {
            if (criteria == null) return;

            SelectedColumn = criteria.ColumnName;
            SelectedOperator = criteria.Operator;
            Value = criteria.Value?.ToString() ?? string.Empty;
            Value2 = criteria.Value2?.ToString() ?? string.Empty;

            UpdateValueFieldsVisibility();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _columnCombo?.Dispose();
                _operatorCombo?.Dispose();
                _valueTextBox?.Dispose();
                _value2TextBox?.Dispose();
                _andLabel?.Dispose();
                _removeButton?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
