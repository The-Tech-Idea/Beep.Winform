using System;
using System.ComponentModel;
using System.Diagnostics;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Desktop.Common.Converters;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Desktop.Common
{
    [Serializable]
    public class BeepColumnConfig : INotifyPropertyChanged
    {
        public BeepColumnConfig()
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
        private bool _isRowID;
        [Category("Data")]
        [Description("Indicates whether the column is a RowID.")]
        public bool IsRowID
        {
            get => _isRowID;
            set { _isRowID = value; OnPropertyChanged(nameof(IsRowID)); }
        }
        private bool _isPrimaryKey;
        [Category("Data")]
        [Description("Indicates whether the column is a primary key.")]
        public bool IsPrimaryKey
        {
            get => _isPrimaryKey;
            set { _isPrimaryKey = value; OnPropertyChanged(nameof(IsPrimaryKey)); }
        }
        private bool _isunbound= false;
        [Category("Data")]
        [Description("Indicates whether the column is unbound.")]
        public bool IsUnbound
        {
            get => _isunbound;
            set { _isunbound = value; OnPropertyChanged(nameof(IsUnbound)); }
        }
        private bool _isForeignKey;
        [Category("Data")]
        [Description("Indicates whether the column is a foreign key.")]
        public bool IsForeignKey
        {
            get => _isForeignKey;
            set { _isForeignKey = value; OnPropertyChanged(nameof(IsForeignKey)); }
        }
        private bool _isAutoIncrement;
        [Category("Data")]
        [Description("Indicates whether the column is an auto-incrementing identity.")]
        public bool IsAutoIncrement
        {
            get => _isAutoIncrement;
            set { _isAutoIncrement = value; OnPropertyChanged(nameof(IsAutoIncrement)); }
        }
        private bool _isUnique;
        [Category("Data")]
        [Description("Indicates whether the column is unique.")]
        public bool IsUnique
        {
            get => _isUnique;
            set { _isUnique = value; OnPropertyChanged(nameof(IsUnique)); }
        }
        private bool _isNullable;
        [Category("Data")]
        [Description("Indicates whether the column allows null values.")]
        public bool IsNullable
        {
            get => _isNullable;
            set { _isNullable = value; OnPropertyChanged(nameof(IsNullable)); }
        }
        private bool _isRowNumColumn = false;
        [Category("Behavior")]
        [Description("Indicates whether the column is an Rownum column.")]
        public bool IsRowNumColumn
        {
            get => _isRowNumColumn;
            set { _isRowNumColumn = value; OnPropertyChanged(nameof(IsRowNumColumn)); }
        }
        private bool _isSelectionCheckBox = false;
        [Category("Behavior")]
        [Description("Indicates whether the column is a selection checkbox.")]
        public bool IsSelectionCheckBox
        {
            get => _isSelectionCheckBox;
            set { _isSelectionCheckBox = value; OnPropertyChanged(nameof(IsSelectionCheckBox)); }
        }

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

        private BeepColumnType _cellEditor = BeepColumnType.Text;
        [Category("Appearance")]
        [Description("The editor type for the column cells.")]
        public BeepColumnType CellEditor
        {
            get => _cellEditor;
            set { _cellEditor = value; OnPropertyChanged(nameof(CellEditor)); }
        }
        private bool ShouldSerializeCellEditor() => _cellEditor != BeepColumnType.Text;

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

        private bool _issticked=false;
        [Category("Behavior")]
        [Description("Indicates whether the column is Sticked.")]
        public bool Sticked
        {
            get => _issticked;
            set { _issticked = value; OnPropertyChanged(nameof(Sticked)); }
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
        private string _enumsourcetypename;
        [Category("Data")]
        [Description("The Enum Source Type Name for the column.")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public string EnumSourceType
        {
            get => _enumsourcetypename;
            set
            {
                _enumsourcetypename = value;
                FillitemsFromEnum();
                OnPropertyChanged(nameof(EnumSourceType));
            }
        }
        private DateTime _date;
        [Category("Data")]
        [Description("The Date for the column.")]
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date)); }
        }
        private AggregationType aggregationType;
        [Category("Data")]
        [Description("The Aggregation Type for the column.")]
        public AggregationType AggregationType
        {
            get => aggregationType;
            set { aggregationType = value; OnPropertyChanged(nameof(AggregationType)); }
        }
        //[Category("Data")]
        //[Description("Click to select an enum type.")]
        //[EditorBrowsable(EditorBrowsableState.Always)]
        //public string SelectEnumType
        //{
        //    get => "Click to Select...";
        //    set
        //    {
        //        if (value == "Click to Select...")
        //        {
        //            using (EnumSelectorForm form = new EnumSelectorForm())
        //            {
        //                form.SelectedEnumType = EnumSourceType;
        //                if (form.ShowDialog() == DialogResult.OK)
        //                {
        //                    EnumSourceType = form.SelectedEnumType;
        //                }
        //            }
        //        }
        //    }
        //}
        private void FillitemsFromEnum()
        {
            _items.Clear();
            if (!string.IsNullOrEmpty(EnumSourceType))
            {
                Type enumType = Type.GetType(EnumSourceType);
                if (enumType != null && enumType.IsEnum)
                {
                    foreach (var item in Enum.GetValues(enumType))
                    {
                        Items.Add(new SimpleItem { Display = item.ToString(), Value = item });
                    }
                    Debug.WriteLine($"Filled items from enum: {EnumSourceType}, Count: {Items.Count}");
                }
                else
                {
                    Debug.WriteLine($"Invalid or non-enum type: {EnumSourceType}");
                }
            }
        }
        private string _parentColumnName;
        [Category("Data")]
        [Description("The Parent Column Name for the column.")]
        public string ParentColumnName
        {
            get => _parentColumnName;
            set { _parentColumnName = value; OnPropertyChanged(nameof(ParentColumnName)); }
        }
        private object _defaultValue;
        [Category("Data")]
        [Description("The Default Value for the column.")]
        public object DefaultValue
        {
            get => _defaultValue;
            set { _defaultValue = value; OnPropertyChanged(nameof(DefaultValue)); }
        }
        private string _customControlName;
        [Category("Data")]
        [Description("The Custom Control Name for the column.")]
        public string CustomControlName
        {
            get => _customControlName;
            set { _customControlName = value; OnPropertyChanged(nameof(CustomControlName)); }
        }
        private string _querytogetvalues;
        [Category("Data")]
        [Description("The Query to get values for the column.")]
        public string QueryToGetValues
        {
            get => _querytogetvalues;
            set { _querytogetvalues = value; OnPropertyChanged(nameof(QueryToGetValues)); }
        }
        private decimal _oldvalue;
        [Category("Data")]
        [Description("The Old Value for the column.")]
        public decimal OldValue
        {
            get => _oldvalue;
            set { _oldvalue = value; OnPropertyChanged(nameof(OldValue)); }
        }
        private int _maxValue;
        [Category("Data")]
        [Description("The Maximum Value for the column.")]
        public int MaxValue
        {
            get => _maxValue;
            set { _maxValue = value; OnPropertyChanged(nameof(MaxValue)); }
        }
        private int _minValue;
        [Category("Data")]
        [Description("The Minimum Value for the column.")]
        public int MinValue
        {
            get => _minValue;
            set { _minValue = value; OnPropertyChanged(nameof(MinValue)); }
        }
        private int _decimalPlaces;
        [Category("Data")]
        [Description("The Decimal Places for the column.")]
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set { _decimalPlaces = value; OnPropertyChanged(nameof(DecimalPlaces)); }
        }
        private int _maximageheight;
        [Category("Data")]
        [Description("The Maximum Image Height for the column.")]
        public int MaxImageHeight
        {
            get => _maximageheight;
            set { _maximageheight = value; OnPropertyChanged(nameof(MaxImageHeight)); }
        }
        private int _maximagewidth;
        [Category("Data")]
        [Description("The Maximum Image Width for the column.")]
        public int MaxImageWidth
        {
            get => _maximagewidth;
            set { _maximagewidth = value; OnPropertyChanged(nameof(MaxImageWidth)); }
        }
        private string _imagepath;
        [Category("Data")]
        [Description("The Image Path for the column.")]
        public string ImagePath
        {
            get => _imagepath;
            set { _imagepath = value; OnPropertyChanged(nameof(ImagePath)); }
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
    public enum BeepColumnType
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
        ListOfValue,
        NumericUpDown,
        Radio,
        Custom
    }
    [Serializable]
    public class BeepGridColumnConfigCollection : BindingList<BeepColumnConfig>
    {
        public BeepGridColumnConfigCollection() : base() { }
    }

}