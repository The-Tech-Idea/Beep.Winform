using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Models
{
    /// <summary>
    /// Defines a column in a multi-column BeepTree.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BeepTreeColumn
    {
        private string _name;
        private string _caption;
        private string _fieldName;
        private int _width = 100;
        private int _minWidth = 20;
        private int _maxWidth = 10000;
        private bool _visible = true;
        private bool _readOnly = false;
        private bool _sortable = true;
        private bool _filterable = true;
        private Type _dataType = typeof(string);
        private string _formatString = string.Empty;
        private ContentAlignment _alignment = ContentAlignment.MiddleLeft;
        private string _editorType = "TextBox";
        private bool _isFixed = false;
        private int _sortOrder = 0; // 0 = not sorted, 1 = primary, 2 = secondary, etc.
        private BeepTreeSortDirection _sortDirection = BeepTreeSortDirection.None;

        /// <summary>
        /// Gets or sets the unique name of the column.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The unique name of the column.")]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Gets or sets the display caption of the column header.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The display caption of the column header.")]
        public string Caption
        {
            get => _caption ?? _name;
            set => _caption = value;
        }

        /// <summary>
        /// Gets or sets the field name in the data source that this column binds to.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The field name in the data source that this column binds to.")]
        public string FieldName
        {
            get => _fieldName;
            set => _fieldName = value;
        }

        /// <summary>
        /// Gets or sets the width of the column in pixels.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The width of the column in pixels.")]
        [DefaultValue(100)]
        public int Width
        {
            get => _width;
            set => _width = Math.Max(_minWidth, Math.Min(_maxWidth, value));
        }

        /// <summary>
        /// Gets or sets the minimum width of the column.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The minimum width of the column.")]
        [DefaultValue(20)]
        public int MinWidth
        {
            get => _minWidth;
            set => _minWidth = Math.Max(0, value);
        }

        /// <summary>
        /// Gets or sets the maximum width of the column.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The maximum width of the column.")]
        [DefaultValue(10000)]
        public int MaxWidth
        {
            get => _maxWidth;
            set => _maxWidth = Math.Max(_minWidth, value);
        }

        /// <summary>
        /// Gets or sets whether the column is visible.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether the column is visible.")]
        [DefaultValue(true)]
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        /// <summary>
        /// Gets or sets whether the column is read-only.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether the column is read-only.")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _readOnly;
            set => _readOnly = value;
        }

        /// <summary>
        /// Gets or sets whether the column can be sorted.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether the column can be sorted.")]
        [DefaultValue(true)]
        public bool Sortable
        {
            get => _sortable;
            set => _sortable = value;
        }

        /// <summary>
        /// Gets or sets whether the column can be filtered.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether the column can be filtered.")]
        [DefaultValue(true)]
        public bool Filterable
        {
            get => _filterable;
            set => _filterable = value;
        }

        /// <summary>
        /// Gets whether the column currently has an active filter applied.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFiltered { get; internal set; } = false;

        /// <summary>
        /// Gets or sets the data type of the column.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The data type of the column.")]
        public Type DataType
        {
            get => _dataType;
            set => _dataType = value ?? typeof(string);
        }

        /// <summary>
        /// Gets or sets the format string for displaying values.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The format string for displaying values (e.g., 'C2' for currency, 'dd/MM/yyyy' for dates).")]
        public string FormatString
        {
            get => _formatString;
            set => _formatString = value;
        }

        /// <summary>
        /// Gets or sets the text alignment for cells in this column.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text alignment for cells in this column.")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment Alignment
        {
            get => _alignment;
            set => _alignment = value;
        }

        /// <summary>
        /// Gets or sets the editor type for inline editing.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The editor type for inline editing (TextBox, ComboBox, DatePicker, CheckBox, Numeric).")]
        [DefaultValue("TextBox")]
        public string EditorType
        {
            get => _editorType;
            set => _editorType = value;
        }

        /// <summary>
        /// Gets or sets the items for a ComboBox editor.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The items to display in a ComboBox editor.")]
        public List<string> ComboBoxItems { get; set; }

        /// <summary>
        /// Gets or sets whether the column is fixed (pinned) and does not scroll horizontally.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Whether the column is fixed (pinned) and does not scroll horizontally.")]
        [DefaultValue(false)]
        public bool IsFixed
        {
            get => _isFixed;
            set => _isFixed = value;
        }

        /// <summary>
        /// Gets or sets the sort priority (0 = not sorted, 1 = primary, 2 = secondary, etc.).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SortOrder
        {
            get => _sortOrder;
            set => _sortOrder = value;
        }

        /// <summary>
        /// Gets or sets the current sort direction.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeSortDirection SortDirection
        {
            get => _sortDirection;
            set => _sortDirection = value;
        }

        /// <summary>
        /// Gets the actual display text for the column header.
        /// </summary>
        [Browsable(false)]
        public string DisplayText => Caption ?? Name ?? FieldName ?? "Column";

        /// <summary>
        /// Returns a string representation of the column.
        /// </summary>
        public override string ToString()
        {
            return $"{DisplayText} ({Width}px)";
        }
    }

    /// <summary>
    /// Specifies the sort direction for a column.
    /// </summary>
    public enum BeepTreeSortDirection
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }
}
