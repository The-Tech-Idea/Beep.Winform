using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    [Serializable]
    public class BeepGridColumnConfig : INotifyPropertyChanged
    {
        public BeepGridColumnConfig()
        {
            GuidID = Guid.NewGuid().ToString();
            _items = new List<SimpleItem>(); // Initialize to avoid null
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(ColumnCaption) ? ColumnCaption :
                   !string.IsNullOrEmpty(ColumnName) ? ColumnName :
                   "Unnamed Column";
        }

        #region Properties
       // [XmlIgnore]
        // public Type PropertyType { get; set; }
        private string _propertyTypeName = "Column"; // Default value
        [Browsable(false)]
        public string PropertyTypeName
        {
            get => _propertyTypeName;
            set => _propertyTypeName = value;
        }

        private string _columnCaption = "Column"; // Default value
        [Category("Data")]
        [Description("The display name of the column.")]
        public string ColumnCaption
        {
            get => _columnCaption;
            set { _columnCaption = value; OnPropertyChanged(nameof(ColumnCaption)); }
        }
        private bool ShouldSerializeColumnCaption() => _columnCaption != "Column";

        private string _columnName;
        [Category("Data")]
        [Description("The name of the column in the data source.")]
        public string ColumnName
        {
            get => _columnName;
            set { _columnName = value; OnPropertyChanged(nameof(ColumnName)); }
        }

        private int _width = 100;
        [Category("Layout")]
        [Description("The width of the column in pixels.")]
        public int Width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(nameof(Width)); }
        }
        private bool ShouldSerializeWidth() => _width != 100;

        private bool _visible = true;
        [Category("Behavior")]
        [Description("Indicates whether the column is visible.")]
        public bool Visible
        {
            get => _visible;
            set { _visible = value; OnPropertyChanged(nameof(Visible)); }
        }
        private bool ShouldSerializeVisible() => _visible != true;

        private DbFieldCategory _columnType = DbFieldCategory.String;
        [Category("Data")]
        [Description("The data type of the column.")]
        public DbFieldCategory ColumnType
        {
            get => _columnType;
            set { _columnType = value; OnPropertyChanged(nameof(ColumnType)); }
        }
        private bool ShouldSerializeColumnType() => _columnType != DbFieldCategory.String;

        private BeepGridColumnType _cellEditor = BeepGridColumnType.Text;
        [Category("Appearance")]
        [Description("The editor type for the column cells.")]
        public BeepGridColumnType CellEditor
        {
            get => _cellEditor;
            set { _cellEditor = value; OnPropertyChanged(nameof(CellEditor)); }
        }
        private bool ShouldSerializeCellEditor() => _cellEditor != BeepGridColumnType.Text;

        private string _format;
        [Category("Appearance")]
        [Description("The format string for displaying data (e.g., 'N2' for numbers).")]
        public string Format
        {
            get => _format;
            set { _format = value; OnPropertyChanged(nameof(Format)); }
        }

        private string _filter;
        [Category("Filtering")]
        [Description("The filter condition applied to this column.")]
        public string Filter
        {
            get => _filter;
            set { _filter = value; OnPropertyChanged(nameof(Filter)); }
        }

        private bool _isFiltered;
        [Category("Filtering")]
        [Description("Indicates whether the column is currently filtered.")]
        public bool IsFiltered
        {
            get => _isFiltered;
            set { _isFiltered = value; OnPropertyChanged(nameof(IsFiltered)); }
        }

        private bool _isSorted;
        [Category("Sorting")]
        [Description("Indicates whether the column is currently sorted.")]
        public bool IsSorted
        {
            get => _isSorted;
            set { _isSorted = value; OnPropertyChanged(nameof(IsSorted)); }
        }

        private bool _isFilteOn;
        [Category("Filtering")]
        [Description("Indicates whether filtering is enabled for this column.")]
        public bool IsFilteOn
        {
            get => _isFilteOn;
            set { _isFilteOn = value; OnPropertyChanged(nameof(IsFilteOn)); }
        }

        private bool _isTotalOn;
        [Category("Aggregation")]
        [Description("Indicates whether totaling is enabled for this column.")]
        public bool IsTotalOn
        {
            get => _isTotalOn;
            set { _isTotalOn = value; OnPropertyChanged(nameof(IsTotalOn)); }
        }

        private bool _hasTotal;
        [Category("Aggregation")]
        [Description("Indicates whether the column should display a total.")]
        public bool HasTotal
        {
            get => _hasTotal;
            set { _hasTotal = value; OnPropertyChanged(nameof(HasTotal)); }
        }

        private decimal _total;
        [Category("Aggregation")]
        [Description("The total value for the column (if applicable).")]
        public decimal Total
        {
            get => _total;
            set { _total = value; OnPropertyChanged(nameof(Total)); }
        }

        private decimal _oldValue;
        [Category("Data")]
        [Description("The previous value of the column (for tracking changes).")]
        public decimal OldValue
        {
            get => _oldValue;
            set { _oldValue = value; OnPropertyChanged(nameof(OldValue)); }
        }

        private int _index;
        [Category("Layout")]
        [Description("The index of the column in the grid.")]
        public int Index
        {
            get => _index;
            set { _index = value; OnPropertyChanged(nameof(Index)); }
        }

        private bool _readOnly;
        [Category("Behavior")]
        [Description("Indicates whether the column is read-only.")]
        public bool ReadOnly
        {
            get => _readOnly;
            set { _readOnly = value; OnPropertyChanged(nameof(ReadOnly)); }
        }

        private string _guidID;
        [Browsable(false)]
        public string GuidID
        {
            get => _guidID;
            set { _guidID = value; OnPropertyChanged(nameof(GuidID)); }
        }

        private DataGridViewColumnSortMode _sortMode = DataGridViewColumnSortMode.Automatic;
        [Category("Sorting")]
        [Description("Defines how the column can be sorted.")]
        public DataGridViewColumnSortMode SortMode
        {
            get => _sortMode;
            set { _sortMode = value; OnPropertyChanged(nameof(SortMode)); }
        }
        private bool ShouldSerializeSortMode() => _sortMode != DataGridViewColumnSortMode.Automatic;

        private DataGridViewTriState _resizable = DataGridViewTriState.True;
        [Category("Layout")]
        [Description("Indicates whether the column can be resized.")]
        public DataGridViewTriState Resizable
        {
            get => _resizable;
            set { _resizable = value; OnPropertyChanged(nameof(Resizable)); }
        }
        private bool ShouldSerializeResizable() => _resizable != DataGridViewTriState.True;

        private DataGridViewAutoSizeColumnMode _autoSizeMode = DataGridViewAutoSizeColumnMode.None;
        [Category("Layout")]
        [Description("Defines how the column auto-sizes.")]
        public DataGridViewAutoSizeColumnMode AutoSizeMode
        {
            get => _autoSizeMode;
            set { _autoSizeMode = value; OnPropertyChanged(nameof(AutoSizeMode)); }
        }
        private bool ShouldSerializeAutoSizeMode() => _autoSizeMode != DataGridViewAutoSizeColumnMode.None;

        private ColumnHeaderStyle _headerStyle;
        [Category("Appearance")]
        [Description("Style settings for the column header.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ColumnHeaderStyle HeaderStyle
        {
            get => _headerStyle;
            set { _headerStyle = value; OnPropertyChanged(nameof(HeaderStyle)); }
        }

        private List<SimpleItem> _items;
        [Category("Data")]
        [Description("The list of items for the ComboBox editor.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(nameof(Items)); }
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnPropertyChanged Error: {ex.Message}");
            }
        }
        #endregion
    }

    [Serializable]
    public class ColumnHeaderStyle
    {
        private string _typography = "Arial";
        [Description("The font family for the header text.")]
        public string Typography
        {
            get => _typography;
            set => _typography = value;
        }

        private float _fontSize = 10f;
        [Description("The font size for the header text.")]
        public float FontSize
        {
            get => _fontSize;
            set => _fontSize = value;
        }

        private Color _foreColor = Color.Black;
        [Description("The text color of the header.")]
        public Color ForeColor
        {
            get => _foreColor;
            set => _foreColor = value;
        }

        private Color _backColor = Color.LightGray;
        [Description("The background color of the header.")]
        public Color BackColor
        {
            get => _backColor;
            set => _backColor = value;
        }
    }

    [Serializable]
    public class BeepGridColumnConfigCollection : BindingList<BeepGridColumnConfig>
    {
        public BeepGridColumnConfigCollection() : base() { }
    }



    public enum BeepGridColumnType
    {
        Text,
        CheckBoxBool,
        CheckBoxChar,
        CheckBoxString,
        ComboBox,
        DateTime,
        Image,
        ProgressBar,
        Rating,
        StarRating,
        Button,
        Link,
        Switch,
        ListBox,
        NumericUpDown,
        Custom
    }
}