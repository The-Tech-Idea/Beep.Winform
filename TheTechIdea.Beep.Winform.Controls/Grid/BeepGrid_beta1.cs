//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Linq;
//using System.Reflection;
//using System.Windows.Forms;
//using TheTechIdea.Beep.DataBase;
//using TheTechIdea.Beep.Editor;
//using TheTechIdea.Beep.Utilities;
//using TheTechIdea.Beep.Vis.Modules;
// 
//using TheTechIdea.Beep.Winform.Controls.Models;

//// Add enum for sort direction
//public enum SortDirection
//{
//    None,
//    Ascending,
//    Descending
//}

//// Add enum for grid styles
//public enum BeepGridStyle
//{
//    Classic,
//    Modern,
//    Flat,
//    Material,
//    Dark
//}

//namespace TheTechIdea.Beep.Winform.Controls.Grid
//{
//    [ToolboxItem(true)]
//    [Category("Data")]
//    [Description("High-performance grid control with virtual scrolling, custom cell rendering, and modern UI features.")]
//    [DisplayName("Beep Grid")]
//    public class BeepGrid_beta1 : BeepControl
//    {
//        #region Fields
        
//        // NEW: Grid Component References - Modern Component Architecture
//        private GridTitleHeaderPanel _titleHeaderPanel;
//        private GridColumnHeaderPanel _columnHeaderPanel;
//        private GridFilterPanel _filterPanel;
//        private GridNavigationPanel _navigationPanel;
//        private bool _useComponentArchitecture = true; // Flag to enable new architecture
        
//        // Performance optimization fields
//        private bool _needsRedraw = true;
//        private bool _layoutDirty = true;
//        private bool _isInitializing = true;
//        private bool _deferRedraw = false;
//        private Dictionary<int, Rectangle> _cachedCellBounds = new Dictionary<int, Rectangle>();
//        private Dictionary<string, Size> _cachedTextSizes = new Dictionary<string, Size>();
//        // Enhanced data source update management
//        private bool _enableTransactions = true;
//        private Dictionary<object, Dictionary<string, object>> _pendingChanges = new Dictionary<object, Dictionary<string, object>>();
//        private Dictionary<object, Dictionary<string, object>> _originalValues = new Dictionary<object, Dictionary<string, object>>();
//        private HashSet<object> _dirtyItems = new HashSet<object>();
//        private bool _batchUpdateMode = false;
//        private Dictionary<Type, HashSet<string>> _readOnlyProperties = new Dictionary<Type, HashSet<string>>();
//        private Dictionary<string, Func<object, object, bool>> _customValidators = new Dictionary<string, Func<object, object, bool>>();
//        private bool _autoCommitChanges = true;
//        private int _maxRetryAttempts = 3;
//        private bool _validateOnUpdate = true;
//        // Virtual scrolling
//        private int _virtualRowCount = 0;
//        private int _visibleRowStart = 0;
//        private int _visibleRowEnd = 0;
//        private int _virtualRowHeight = 28;
//        private int _headerHeight = 32;
        
//        // Data management
//        private object _dataSource;
//        private List<object> _filteredData = new List<object>();
//        private List<object> _originalData = new List<object>();
//        private EntityStructure _entityStructure;
//        private Type _dataType;
        
//        // Grid configuration
//        private List<BeepColumnConfig> _columns = new List<BeepColumnConfig>();
//        private Dictionary<int, BeepCellConfig> _visibleCells = new Dictionary<int, BeepCellConfig>();
//        private Rectangle _gridBounds;
//        private Rectangle _headerBounds;
//        private Rectangle _footerBounds;
//        private Rectangle _scrollableBounds;
        
//        // Selection and interaction
//        private int _selectedRowIndex = -1;
//        private int _selectedColumnIndex = -1;
//        private int _hoveredRowIndex = -1;
//        private int _hoveredColumnIndex = -1;
//        private BeepCellConfig _selectedCell;
//        private BeepCellConfig _editingCell;
//        private Control _cellEditor;
        
//        // Scrollbars
//        private BeepScrollBar _vScrollBar;
//        private BeepScrollBar _hScrollBar;
//        private int _horizontalOffset = 0;
//        private int _verticalOffset = 0;
        
//        // Filtering and sorting
//        private bool _showFilter = false;
//        private Dictionary<string, string> _columnFilters = new Dictionary<string, string>();
//        private List<Control> _filterControls = new List<Control>();
//        private string _sortColumn;
//        private bool _sortAscending = true;
        
//        // Panel management
//        private BeepPanel _headerPanel;
 
//        private BeepPanel _footerPanel;
//        private BeepLabel _titleLabel;
//        private int _panelSpacing = 2;
        
//        // Hit testing for performance
//        private List<HitTestArea> _hitAreas = new List<HitTestArea>();
        
//        // Cell rendering controls - similar to BeepSimpleGrid
//        private Dictionary<string, IBeepUIComponent> _columnDrawers = new Dictionary<string, IBeepUIComponent>();
        
//        // Add new fields for modern styling
//        private Dictionary<int, SortDirection> _columnSortStates = new Dictionary<int, SortDirection>();
//        private Dictionary<int, bool> _columnFilterStates = new Dictionary<int, bool>();
//        private int _hoveredHeaderColumn = -1;
//        private BeepGridStyle _gridStyle = BeepGridStyle.Classic;

//        // Row Selection Checkbox Fields - NEW
//        private bool _showCheckboxes = false;
//        private BeepCheckBoxBool _selectAllCheckBox;
//        private Dictionary<int, bool> _selectedRows = new Dictionary<int, bool>();
//        private HashSet<object> _selectedDataItems = new HashSet<object>();
//        private int _selectionColumnWidth = 30;
//        private bool _hasSelectionColumn = false;

//        private int _stickyWidth = 0; // Cache sticky column width
//        private Rectangle _stickyRegion;
//        private Rectangle _scrollableRegion;

//        #endregion
//        #region Navigation Fields
//        private bool _showNavigator = true;
//        private int navigatorPanelHeight = 30;
//        private Rectangle navigatorPanelRect;
//        private bool _navigatorDrawn = false;
//        private bool tooltipShown = false;
//        private int _currentPage = 1;
//        private int _totalPages = 1;

//        // Navigation events
//        public event EventHandler CallPrinter;
//        public event EventHandler SendMessage;
//        public event EventHandler ShowSearch;
//        public event EventHandler NewRecordCreated;
//        public event EventHandler SaveCalled;
//        public event EventHandler DeleteCalled;
//        public event EventHandler EditCalled;

//        #endregion
//        #region Enhanced Data Source Update Events

//        public event EventHandler<DataUpdateEventArgs> BeforeDataUpdate;
//        public event EventHandler<DataUpdateEventArgs> AfterDataUpdate;
//        public event EventHandler<DataUpdateFailedEventArgs> DataUpdateFailed;
//        public event EventHandler<ValidationFailedEventArgs> ValidationFailed;

//        #endregion
//        #region Properties
        
      
//        #region Enhanced Data Source Update Properties

//        [Browsable(true)]
//        [Category("Data")]
//        [DefaultValue(true)]
//        public bool EnableTransactions { get; set; } = true;

//        [Browsable(true)]
//        [Category("Data")]
//        [DefaultValue(true)]
//        public bool AutoCommitChanges { get; set; } = true;

//        [Browsable(true)]
//        [Category("Data")]
//        [DefaultValue(true)]
//        public bool ValidateOnUpdate { get; set; } = true;

//        [Browsable(false)]
//        public bool HasPendingChanges => _pendingChanges.Count > 0;

//        #endregion
//        [Browsable(true)]
//        [Category("Layout")]
//        [DefaultValue(true)]
//        [Description("Shows or hides the navigation bar")]
//        public bool ShowNavigator
//        {
//            get => _showNavigator;
//            set
//            {
//                if (_showNavigator != value)
//                {
//                    _showNavigator = value;
//                    _layoutDirty = true;
//                    InvalidateLayout();
//                }
//            }
//        }
//        [Browsable(true)]
//        [Category("Data")]
//        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
//        public object DataSource
//        {
//            get => _dataSource;
//            set
//            {
//                if (_dataSource != value)
//                {
//                    _dataSource = value;
//                    RefreshData();
//                }
//            }
//        }
        
//        [Browsable(true)]
//        [Category("Data")]
//        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
//        public List<BeepColumnConfig> Columns
//        {
//            get => _columns;
//            set
//            {
//                _columns = value ?? new List<BeepColumnConfig>();
//                _layoutDirty = true;
//                InvalidateLayout();
//            }
//        }
        
//        [Browsable(true)]
//        [Category("Data")]
//        public EntityStructure EntityStructure
//        {
//            get => _entityStructure;
//            set
//            {
//                _entityStructure = value;
//                if (_entityStructure != null)
//                {
//                    GenerateColumnsFromEntity();
//                }
//            }
//        }
        
//        [Browsable(true)]
//        [Category("Appearance")]
//        [DefaultValue(28)]
//        public int VirtualRowHeight
//        {
//            get => _virtualRowHeight;
//            set
//            {
//                if (_virtualRowHeight != value && value > 10)
//                {
//                    _virtualRowHeight = value;
//                    RecalculateVirtualRows();
//                    Invalidate();
//                }
//            }
//        }
        
//        [Browsable(true)]
//        [Category("Appearance")]
//        [DefaultValue(32)]
//        public int HeaderHeight
//        {
//            get => _headerHeight;
//            set
//            {
//                if (_headerHeight != value && value > 16)
//                {
//                    _headerHeight = value;
//                    _layoutDirty = true;
//                    Invalidate();
//                }
//            }
//        }
        
//        [Browsable(true)]
//        [Category("Behavior")]
//        [DefaultValue(false)]
//        public bool ShowFilter
//        {
//            get => _showFilter;
//            set
//            {
//                if (_showFilter != value)
//                {
//                    _showFilter = value;
//                    ToggleFilterPanel();
//                }
//            }
//        }
        
//        [Browsable(true)]
//        [Category("Appearance")]
//        public string TitleText
//        {
//            get => _titleLabel?.Text ?? "";
//            set
//            {
//                if (_titleLabel != null)
//                {
//                    _titleLabel.Text = value;
//                }
//            }
//        }
        
//        // Add new property for grid style
//        [Browsable(true)]
//        [Category("Appearance")]
//        [DefaultValue(BeepGridStyle.Classic)]
//        [Description("Visual style of the grid")]
//        public BeepGridStyle GridStyle
//        {
//            get => _gridStyle;
//            set
//            {
//                if (_gridStyle != value)
//                {
//                    _gridStyle = value;
//                    ApplyGridStyle();
//                    Invalidate();
//                }
//            }
//        }
        
//        [Browsable(false)]
//        public BeepCellConfig SelectedCell => _selectedCell;
        
//        [Browsable(false)]
//        public int SelectedRowIndex => _selectedRowIndex;
        
//        [Browsable(false)]
//        public int SelectedColumnIndex => _selectedColumnIndex;
        
//        [Browsable(false)]
//        public bool IsInitializing => _isInitializing;
        
//        // Row Selection Checkbox Properties - NEW
//        [Browsable(true)]
//        [Category("Behavior")]
//        [DefaultValue(false)]
//        [Description("Shows or hides row selection checkboxes")]
//        public bool ShowCheckboxes
//        {
//            get => _showCheckboxes;
//            set
//            {
//                if (_showCheckboxes != value)
//                {
//                    _showCheckboxes = value;
//                    EnsureSelectionColumn();
//                    _layoutDirty = true;
//                    Invalidate();
//                }
//            }
//        }

//        [Browsable(true)]
//        [Category("Behavior")]
//        [DefaultValue(30)]
//        [Description("Width of the selection checkbox column")]
//        public int SelectionColumnWidth
//        {
//            get => _selectionColumnWidth;
//            set
//            {
//                if (_selectionColumnWidth != value && value > 20)
//                {
//                    _selectionColumnWidth = value;
//                    if (_hasSelectionColumn)
//                    {
//                        var selColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
//                        if (selColumn != null)
//                        {
//                            selColumn.Width = value;
//                            _layoutDirty = true;
//                            Invalidate();
//                        }
//                    }
//                }
//            }
//        }

//        [Browsable(false)]
//        public Dictionary<int, bool> SelectedRows => new Dictionary<int, bool>(_selectedRows);

//        [Browsable(false)]
//        public HashSet<object> SelectedDataItems => new HashSet<object>(_selectedDataItems);
        
//        #endregion
        
//        #region Constructor
        
//        public BeepGrid_beta1() : base()
//        {
//            SetStyle(ControlStyles.AllPaintingInWmPaint | 
//                    ControlStyles.UserPaint | 
//                    ControlStyles.DoubleBuffer | 
//                    ControlStyles.ResizeRedraw |
//                    ControlStyles.Selectable, true);
            
//            TabStop = true;
            
//            InitializeComponents();
//            InitializeScrollBars();
//            InitializePanels();
            
//            MouseDown += BeepGrid_MouseDown;
//            MouseMove += BeepGrid_MouseMove;
//            MouseUp += BeepGrid_MouseUp;
//            MouseClick += BeepGrid_MouseClick;
//            MouseWheel += BeepGrid_MouseWheel;
//            KeyDown += BeepGrid_KeyDown;
//            Resize += BeepGrid_Resize;
            
//            _isInitializing = false;
//        }
        
//        #endregion
        
//        #region Initialization
        
//        private void InitializeComponents()
//        {
//            // Initialize title label
//            _titleLabel = new BeepLabel
//            {
//                Text = "BeepGrid",
//                TextAlign = ContentAlignment.MiddleLeft,
//                AutoSize = false,
//                Height = 30,
//                MenuStyle = MenuStyle,
//                IsChild = true
//            };
//        }
        
//        private void InitializeScrollBars()
//        {
//            // Vertical scrollbar
//            _vScrollBar = new BeepScrollBar
//            {
//                ScrollOrientation = Orientation.Vertical,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            _vScrollBar.Scroll += VScrollBar_Scroll;
//            Controls.Add(_vScrollBar);
            
//            // Horizontal scrollbar
//            _hScrollBar = new BeepScrollBar
//            {
//                ScrollOrientation = Orientation.Horizontal,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            _hScrollBar.Scroll += HScrollBar_Scroll;
//            Controls.Add(_hScrollBar);
//        }
        
//        private void InitializePanels()
//        {
//            // Header panel
//            _headerPanel = new BeepPanel
//            {
//                Height = 35,
//                Dock = DockStyle.Top,
//                MenuStyle = MenuStyle,
//                IsChild = true
//            };
//            _headerPanel.Controls.Add(_titleLabel);
//            Controls.Add(_headerPanel);
            
//            // Filter panel (initially hidden)
//            _filterPanel = new BeepPanel
//            {
//                Height = 30,
//                Dock = DockStyle.Top,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            Controls.Add(_filterPanel);
            
//            // Footer panel
//            _footerPanel = new BeepPanel
//            {
//                Height = 25,
//                Dock = DockStyle.Bottom,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            Controls.Add(_footerPanel);
//        }

//        #endregion
  
        
//        #region Initialization
        
//        private void InitializeComponents()
//        {
//            // Initialize title label
//            _titleLabel = new BeepLabel
//            {
//                Text = "BeepGrid",
//                TextAlign = ContentAlignment.MiddleLeft,
//                AutoSize = false,
//                Height = 30,
//                MenuStyle = MenuStyle,
//                IsChild = true
//            };
//        }
        
//        private void InitializeScrollBars()
//        {
//            // Vertical scrollbar
//            _vScrollBar = new BeepScrollBar
//            {
//                ScrollOrientation = Orientation.Vertical,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            _vScrollBar.Scroll += VScrollBar_Scroll;
//            Controls.Add(_vScrollBar);
            
//            // Horizontal scrollbar
//            _hScrollBar = new BeepScrollBar
//            {
//                ScrollOrientation = Orientation.Horizontal,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            _hScrollBar.Scroll += HScrollBar_Scroll;
//            Controls.Add(_hScrollBar);
//        }
        
//        private void InitializePanels()
//        {
//            // Header panel
//            _headerPanel = new BeepPanel
//            {
//                Height = 35,
//                Dock = DockStyle.Top,
//                MenuStyle = MenuStyle,
//                IsChild = true
//            };
//            _headerPanel.Controls.Add(_titleLabel);
//            Controls.Add(_headerPanel);
            
//            // Filter panel (initially hidden)
//            _filterPanel = new BeepPanel
//            {
//                Height = 30,
//                Dock = DockStyle.Top,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            Controls.Add(_filterPanel);
            
//            // Footer panel
//            _footerPanel = new BeepPanel
//            {
//                Height = 25,
//                Dock = DockStyle.Bottom,
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                Visible = false
//            };
//            Controls.Add(_footerPanel);
//        }

//        #endregion
//        #region Public Data Update API

//        public void AddCustomValidator(string propertyName, Func<object, object, bool> validator)
//        {
//            _customValidators[propertyName] = validator;
//        }

//        public void CommitAllChanges()
//        {
//            var itemsToCommit = _dirtyItems.ToList();
//            foreach (var item in itemsToCommit)
//                CommitChanges(item);
//        }

//        public void RollbackAllChanges()
//        {
//            var itemsToRollback = _dirtyItems.ToList();
//            foreach (var item in itemsToRollback)
//                RollbackChanges(item);
//        }

//        private void CommitChanges(object dataItem)
//        {
//            _pendingChanges.Remove(dataItem);
//            _originalValues.Remove(dataItem);
//            _dirtyItems.Remove(dataItem);
//        }

//        private void RollbackChanges(object dataItem)
//        {
//            if (_originalValues.TryGetValue(dataItem, out var originalProps))
//            {
//                foreach (var prop in originalProps)
//                {
//                    var property = dataItem.GetType().GetProperty(prop.Key);
//                    if (property?.CanWrite == true)
//                    {
//                        try { property.SetValue(dataItem, prop.Value); }
//                        catch (Exception ex) { //MiscFunctions.SendLog($"Rollback error: {ex.Message}"); }
//                    }
//                }
//            }
//            CommitChanges(dataItem);
//        }

//        #endregion
//        #region Data Management
//        #region Enhanced Update Helper Methods

//        private bool UpdateDirectly(object dataItem, string propertyName, object newValue)
//        {
//            try
//            {
//                if (dataItem is DataRow dataRow)
//                {
//                    dataRow[propertyName] = newValue ?? DBNull.Value;
//                    return true;
//                }
//                else
//                {
//                    var property = dataItem.GetType().GetProperty(propertyName);
//                    if (property?.CanWrite == true)
//                    {
//                        var convertedValue = MiscFunctions.ConvertValueToPropertyType(property.PropertyType, newValue);
//                        property.SetValue(dataItem, convertedValue);
//                        return true;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                //MiscFunctions.SendLog($"Update failed: {ex.Message}");
//                throw;
//            }
//            return false;
//        }

//        private bool ValidateValue(string propertyName, object value, object dataItem)
//        {
//            // Check custom validators
//            if (_customValidators.TryGetValue(propertyName, out var validator))
//                return validator(dataItem, value);

//            // Check column-specific validation from BeepColumnConfig
//            var column = _columns.FirstOrDefault(c => c.ColumnName == propertyName);
//            if (column != null && column.IsRequired && (value == null || value == DBNull.Value))
//            {
//                OnValidationFailed(new ValidationFailedEventArgs("Required field cannot be empty", propertyName, value));
//                return false;
//            }

//            return true;
//        }

//        private void TrackChange(object dataItem, string propertyName, object oldValue, object newValue)
//        {
//            if (!_originalValues.ContainsKey(dataItem))
//                _originalValues[dataItem] = new Dictionary<string, object>();

//            if (!_originalValues[dataItem].ContainsKey(propertyName))
//                _originalValues[dataItem][propertyName] = oldValue;

//            _dirtyItems.Add(dataItem);
//        }

//        private bool AreValuesEqual(object value1, object value2)
//        {
//            if (value1 == null && value2 == null) return true;
//            if (value1 == null || value2 == null) return false;
//            if (value1 == DBNull.Value && value2 == DBNull.Value) return true;
//            if (value1 == DBNull.Value || value2 == DBNull.Value) return false;
//            return value1.Equals(value2);
//        }

//        #endregion
//        private bool UpdateDataSourceEnhanced(int rowIndex, int columnIndex, object newValue, bool force = false)
//        {
//            if (rowIndex < 0 || rowIndex >= _filteredData.Count) return false;

//            var dataItem = _filteredData[rowIndex];
//            var column = _columns[columnIndex];

//            // Get current value
//            object currentValue = GetCellValue(dataItem, column);

//            // Skip if same value
//            if (AreValuesEqual(currentValue, newValue)) return true;

//            // Validate if enabled
//            if (_validateOnUpdate && !ValidateValue(column.ColumnName, newValue, dataItem))
//                return false;

//            // Perform update with retry logic
//            for (int attempt = 0; attempt < _maxRetryAttempts; attempt++)
//            {
//                try
//                {
//                    if (UpdateDirectly(dataItem, column.ColumnName, newValue))
//                    {
//                        if (_enableTransactions)
//                            TrackChange(dataItem, column.ColumnName, currentValue, newValue);
//                        return true;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    if (attempt == _maxRetryAttempts - 1)
//                        OnDataUpdateFailed(new DataUpdateFailedEventArgs(dataItem, column.ColumnName, newValue, ex));
//                }
//            }

//            return false;
//        }
//        private void RefreshData()
//        {
//            if (_dataSource == null)
//            {
//                _originalData.Clear();
//                _filteredData.Clear();
//                _virtualRowCount = 0;
//                Invalidate();
//                return;
//            }
            
//            SuspendLayout();
//            try
//            {
//                // Convert data source to list
//                _originalData = ConvertDataSourceToList(_dataSource);
                
//                // Determine data type
//                if (_originalData.Count > 0)
//                {
//                    _dataType = _originalData[0].GetType();
                    
//                    // Auto-generate columns if none exist
//                    if (_columns.Count == 0)
//                    {
//                        GenerateColumnsFromType(_dataType);
//                    }
//                }
                
//                // Apply current filters
//                ApplyFilters();
                
//                // Update virtual row count
//                _virtualRowCount = _filteredData.Count;
                
//                // Recalculate layout
//                RecalculateLayout();
                
//                // Update scrollbars
//                UpdateScrollBars();
//            }
//            finally
//            {
//                ResumeLayout();
//                Invalidate();
//            }
//        }
        
//        private List<object> ConvertDataSourceToList(object dataSource)
//        {
//            var result = new List<object>();
            
//            if (dataSource is IEnumerable enumerable && !(dataSource is string))
//            {
//                foreach (var item in enumerable)
//                {
//                    result.Add(item);
//                }
//            }
//            else if (dataSource is DataTable dataTable)
//            {
//                foreach (DataRow row in dataTable.Rows)
//                {
//                    result.Add(row);
//                }
//            }
//            else
//            {
//                result.Add(dataSource);
//            }
            
//            return result;
//        }
        
//        private void GenerateColumnsFromType(Type type)
//        {
//            _columns.Clear();
            
//            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                               .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);
            
//            int index = 0;
//            foreach (var prop in properties)
//            {
//                var column = new BeepColumnConfig
//                {
//                    ColumnName = prop.Name,
//                    ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(prop.Name),
//                    PropertyTypeName = prop.PropertyType.AssemblyQualifiedName,
//                    Width = 120,
//                    RowIndex = index++,
//                    Visible = true,
//                    CellEditor = MapPropertyTypeToCellEditor(prop.PropertyType)
//                };
                
//                _columns.Add(column);
//            }
            
//            _layoutDirty = true;
//        }
        
//        private void GenerateColumnsFromEntity()
//        {
//            if (_entityStructure?.Fields == null) return;
            
//            _columns.Clear();
            
//            int index = 0;
//            foreach (var field in _entityStructure.Fields)
//            {
//                var column = new BeepColumnConfig
//                {
//                    ColumnName = field.fieldname,
//                    ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.fieldname),
//                    PropertyTypeName = field.fieldtype,
//                    Width = 120,
//                    RowIndex = index++,
//                    Visible = true
//                };
                
//                var fieldType = Type.GetType(field.fieldtype) ?? typeof(string);
//                column.CellEditor = MapPropertyTypeToCellEditor(fieldType);
                
//                _columns.Add(column);
//            }
            
//            _layoutDirty = true;
//        }
        
//        private BeepColumnType MapPropertyTypeToCellEditor(Type type)
//        {
//            if (type == typeof(bool)) return BeepColumnType.CheckBoxBool;
//            if (type == typeof(DateTime)) return BeepColumnType.DateTime;
//            if (type == typeof(int) || type == typeof(long) || 
//                type == typeof(float) || type == typeof(double) || 
//                type == typeof(decimal)) return BeepColumnType.NumericUpDown;
//            if (type.IsEnum) return BeepColumnType.ComboBox;
            
//            return BeepColumnType.Text;
//        }

//        #endregion
//        #region Row Selection Checkbox Implementation

//        /// <summary>
//        /// Ensures the selection column exists when ShowCheckboxes is enabled
//        /// </summary>
//        private void EnsureSelectionColumn()
//        {
//            if (_showCheckboxes)
//            {
//                // Check if selection column already exists
//                var existingSelColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
//                if (existingSelColumn == null)
//                {
//                    // Create selection column
//                    var selectionColumn = new BeepColumnConfig
//                    {
//                        ColumnName = "_Selection",
//                        ColumnCaption = "☑",
//                        Width = _selectionColumnWidth,
//                        IsSelectionCheckBox = true,
//                        CellEditor = BeepColumnType.CheckBoxBool,
//                        ReadOnly = false,
//                        Visible = true,
//                        RowIndex = 0,
//                        Sticked = true, // Keep visible during horizontal scroll
//                        Resizable = DataGridViewTriState.False,
//                        SortMode = DataGridViewColumnSortMode.NotSortable
//                    };

//                    // Insert at the beginning
//                    _columns.Insert(0, selectionColumn);

//                    // Update indices of other columns
//                    for (int i = 1; i < _columns.Count; i++)
//                    {
//                        _columns[i].RowIndex = i;
//                    }

//                    _hasSelectionColumn = true;
//                }

//                // Initialize select all checkbox if not exists
//                if (_selectAllCheckBox == null)
//                {
//                    InitializeSelectAllCheckBox();
//                }
//            }
//            else
//            {
//                // Remove selection column if it exists
//                var selectionColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
//                if (selectionColumn != null)
//                {
//                    _columns.Remove(selectionColumn);

//                    // Update indices
//                    for (int i = 0; i < _columns.Count; i++)
//                    {
//                        _columns[i].RowIndex = i;
//                    }

//                    _hasSelectionColumn = false;
//                }

//                // Dispose select all checkbox
//                if (_selectAllCheckBox != null)
//                {
//                    if (Controls.Contains(_selectAllCheckBox))
//                    {
//                        Controls.Remove(_selectAllCheckBox);
//                    }
//                    _selectAllCheckBox.Dispose();
//                    _selectAllCheckBox = null;
//                }

//                // Clear selection data
//                _selectedRows.Clear();
//                _selectedDataItems.Clear();
//            }
//        }

//        /// <summary>
//        /// Initializes the select all checkbox in the header
//        /// </summary>
//        private void InitializeSelectAllCheckBox()
//        {
//            _selectAllCheckBox = new BeepCheckBoxBool
//            {
//                HideText = true,
//                Text = "",
//                Size = new Size(20, 20),
//                MenuStyle = MenuStyle,
//                IsChild = true,
//                CheckedValue = true,
//                UncheckedValue = false,
//                CurrentValue = false
//            };

//            _selectAllCheckBox.StateChanged += SelectAllCheckBox_StateChanged;
//            Controls.Add(_selectAllCheckBox);
//            _selectAllCheckBox.BringToFront();
//        }

//        /// <summary>
//        /// Handles select all checkbox state changes
//        /// </summary>
//        private void SelectAllCheckBox_StateChanged(object sender, EventArgs e)
//        {
//            if (_selectAllCheckBox == null) return;

//            bool selectAll = _selectAllCheckBox.CurrentValue;

//            // Clear current selections
//            _selectedRows.Clear();
//            _selectedDataItems.Clear();

//            if (selectAll)
//            {
//                // Select all visible rows
//                for (int i = 0; i < _filteredData.Count; i++)
//                {
//                    _selectedRows[i] = true;
//                    _selectedDataItems.Add(_filteredData[i]);
//                }
//            }

//            // Raise selection changed event
//            OnSelectedRowsChanged();

//            // Refresh display
//            Invalidate();
//        }

//        /// <summary>
//        /// Toggles selection for individual row
//        /// </summary>
//        /// <param name="rowIndex">RowIndex of the row to toggle</param>
//        public void ToggleRowSelection(int rowIndex)
//        {
//            if (rowIndex < 0 || rowIndex >= _filteredData.Count) return;

//            var dataItem = _filteredData[rowIndex];

//            if (_selectedRows.ContainsKey(rowIndex))
//            {
//                // Deselect
//                _selectedRows.Remove(rowIndex);
//                _selectedDataItems.Remove(dataItem);
//            }
//            else
//            {
//                // Select
//                _selectedRows[rowIndex] = true;
//                _selectedDataItems.Add(dataItem);
//            }

//            // Update select all checkbox state
//            UpdateSelectAllCheckboxState();

//            // Raise event
//            OnSelectedRowsChanged();

//            // Refresh display
//            Invalidate();
//        }

//        /// <summary>
//        /// Updates the select all checkbox state based on current selections
//        /// </summary>
//        private void UpdateSelectAllCheckboxState()
//        {
//            if (_selectAllCheckBox == null || _filteredData.Count == 0) return;

//            // Temporarily disable event to avoid recursion
//            _selectAllCheckBox.StateChanged -= SelectAllCheckBox_StateChanged;

//            int selectedCount = _selectedRows.Count;
//            int totalCount = _filteredData.Count;

//            if (selectedCount == 0)
//            {
//                _selectAllCheckBox.CurrentValue = false;
//            }
//            else if (selectedCount == totalCount)
//            {
//                _selectAllCheckBox.CurrentValue = true;
//            }
//            else
//            {
//                // Indeterminate state - some selected
//                // For BeepCheckBoxBool, we'll use false but could add indeterminate support
//                _selectAllCheckBox.CurrentValue = false;
//            }

//            // Re-enable event
//            _selectAllCheckBox.StateChanged += SelectAllCheckBox_StateChanged;
//        }

//        /// <summary>
//        /// Gets all selected data items
//        /// </summary>
//        /// <returns>List of selected data objects</returns>
//        public List<object> GetSelectedDataItems()
//        {
//            return _selectedDataItems.ToList();
//        }

//        /// <summary>
//        /// Selects all rows
//        /// </summary>
//        public void SelectAllRows()
//        {
//            if (_selectAllCheckBox != null)
//            {
//                _selectAllCheckBox.CurrentValue = true;
//            }
//        }

//        /// <summary>
//        /// Deselects all rows
//        /// </summary>
//        public void DeselectAllRows()
//        {
//            if (_selectAllCheckBox != null)
//            {
//                _selectAllCheckBox.CurrentValue = false;
//            }
//        }

//        /// <summary>
//        /// Event raised when row selection changes
//        /// </summary>
//        public event EventHandler SelectedRowsChanged;

//        /// <summary>
//        /// Raises the SelectedRowsChanged event
//        /// </summary>
//        protected virtual void OnSelectedRowsChanged()
//        {
//            SelectedRowsChanged?.Invoke(this, EventArgs.Empty);
//        }

//        #endregion
//        #region Layout and Rendering
//        private void DrawNavigationButton(Graphics g, string buttonName, string imagePath, Rectangle buttonRect)
//        {
//            try
//            {
//                // Create a temporary BeepButton for consistent styling
//                var tempButton = new BeepButton
//                {
//                    ImagePath = imagePath,
//                    ImageAlign = ContentAlignment.MiddleCenter,
//                    HideText = true,
//                    IsFrameless = true,
//                    Size = buttonRect.Size,
//                    IsChild = true,
//                    MenuStyle = MenuStyle,
//                    MaxImageSize = new Size(buttonRect.Width - 4, buttonRect.Height - 4)
//                };

//                // Apply current theme
//                tempButton.ApplyTheme(_currentTheme);

//                // Draw the button using BeepButton's Draw method
//                tempButton.Draw(g, buttonRect);

//                // Add hit area for click detection
//                AddHitArea(buttonName, buttonRect);

//                // Clean up
//                tempButton.Dispose();
//            }
//            catch (Exception ex)
//            {
//                // Fallback: draw a simple rectangle if button creation fails
//                using (var brush = new SolidBrush(_currentTheme.ButtonBackColor))
//                using (var pen = new Pen(_currentTheme.ButtonBorderColor))
//                {
//                    g.FillRectangle(brush, buttonRect);
//                    g.DrawRectangle(pen, buttonRect);
//                }

//                // Still add hit area
//                AddHitArea(buttonName, buttonRect);
//            }
//        }
//        private void RecalculateLayout()
//        {
//            if (Width <= 0 || Height <= 0) return;

//            var clientBounds = ClientRectangle;
//            int yOffset = 0;

//            // Header bounds
//            if (_headerPanel.Visible)
//            {
//                _headerBounds = new Rectangle(0, yOffset, clientBounds.Width, _headerPanel.Height);
//                yOffset += _headerPanel.Height + _panelSpacing;
//            }

//            // Filter bounds
//            if (_filterPanel.Visible)
//            {
//                yOffset += _filterPanel.Height + _panelSpacing;
//            }

//            // Navigation panel bounds (NEW)
//            int navigatorHeight = 0;
//            if (_showNavigator)
//            {
//                navigatorHeight = navigatorPanelHeight + _panelSpacing;
//                navigatorPanelRect = new Rectangle(0, clientBounds.Height - navigatorHeight - (_footerPanel.Visible ? _footerPanel.Height : 0),
//                                                 clientBounds.Width, navigatorPanelHeight);
//            }

//            // Footer bounds
//            int footerHeight = _footerPanel.Visible ? _footerPanel.Height + _panelSpacing : 0;

//            // Scrollbar space
//            int vScrollWidth = _vScrollBar.Visible ? _vScrollBar.Width : 0;
//            int hScrollHeight = _hScrollBar.Visible ? _hScrollBar.Height : 0;

//            // Calculate grid bounds (accounting for navigator)
//            _gridBounds = new Rectangle(
//                0,
//                yOffset,
//                clientBounds.Width - vScrollWidth,
//                clientBounds.Height - yOffset - footerHeight - hScrollHeight - navigatorHeight
//            );

//            // Calculate header and scrollable bounds within grid
//            _headerBounds = new Rectangle(_gridBounds.X, _gridBounds.Y, _gridBounds.Width, _headerHeight);
//            _scrollableBounds = new Rectangle(
//                _gridBounds.X,
//                _gridBounds.Y + _headerHeight,
//                _gridBounds.Width,
//                _gridBounds.Height - _headerHeight
//            );

//            RecalculateVirtualRows();
//            PositionScrollBars();
//            _layoutDirty = false;
//        }

//        private void RecalculateVirtualRows()
//        {
//            if (_scrollableBounds.Height <= 0) return;
            
//            int visibleRowCount = _scrollableBounds.Height / _virtualRowHeight;
            
//            _visibleRowStart = Math.Max(0, _verticalOffset / _virtualRowHeight);
//            _visibleRowEnd = Math.Min(_virtualRowCount, _visibleRowStart + visibleRowCount + 1);
            
//            // Cache visible cell bounds for performance
//            CacheVisibleCellBounds();
//        }
        
//        private void CacheVisibleCellBounds()
//        {
//            _cachedCellBounds.Clear();
            
//            if (_columns.Count == 0) return;
            
//            int xOffset = -_horizontalOffset;
            
//            for (int rowIndex = _visibleRowStart; rowIndex < _visibleRowEnd; rowIndex++)
//            {
//                int yPos = _scrollableBounds.Y + (rowIndex - _visibleRowStart) * _virtualRowHeight;
                
//                for (int colIndex = 0; colIndex < _columns.Count; colIndex++)
//                {
//                    if (!_columns[colIndex].Visible) continue;
                    
//                    var cellBounds = new Rectangle(
//                        _scrollableBounds.X + xOffset,
//                        yPos,
//                        _columns[colIndex].Width,
//                        _virtualRowHeight
//                    );
                    
//                    int key = rowIndex << 16 | colIndex; // Pack row and column into key
//                    _cachedCellBounds[key] = cellBounds;
                    
//                    xOffset += _columns[colIndex].Width;
//                }
                
//                xOffset = -_horizontalOffset; // Reset for next row
//            }
//        }

//        protected override void DrawContent(Graphics g)
//        {
//            base.DrawContent(g);

//            if (_layoutDirty)
//            {
//                RecalculateLayout();
//            }

//            // High-quality rendering
//            g.SmoothingMode = SmoothingMode.AntiAlias;
//            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

//            DrawGridBackground(g);
//            DrawColumnHeaders(g);
//            DrawDataRows(g);
//            DrawGridLines(g);
//            DrawSelection(g);

//            // Draw navigation bar (NEW)
//            if (_showNavigator && !navigatorPanelRect.IsEmpty)
//            {
//                DrawNavigationRow(g, navigatorPanelRect);
//            }
//        }

//        private void DrawGridBackground(Graphics g)
//        {
//            using (var brush = new SolidBrush(_currentTheme.GridBackColor))
//            {
//                g.FillRectangle(brush, _gridBounds);
//            }
//        }
        
//        private void DrawColumnHeaders(Graphics g)
//        {
//            if (_columns.Count == 0) return;
            
//            using (var headerBrush = new SolidBrush(_currentTheme.GridHeaderBackColor))
//            using (var textBrush = new SolidBrush(_currentTheme.GridHeaderForeColor))
//            using (var borderPen = new Pen(_currentTheme.GridLineColor))
//            {
//                g.FillRectangle(headerBrush, _headerBounds);
                
//                int xOffset = _headerBounds.X - _horizontalOffset;
                
//                for (int i = 0; i < _columns.Count; i++)
//                {
//                    var column = _columns[i];
//                    if (!column.Visible) continue;
                    
//                    var headerRect = new Rectangle(xOffset, _headerBounds.Y, column.Width, _headerBounds.Height);
                    
//                    // Skip if completely outside visible area
//                    if (headerRect.Right < _headerBounds.Left || headerRect.Left > _headerBounds.Right)
//                    {
//                        xOffset += column.Width;
//                        continue;
//                    }
                    
//                    // Clip to visible area
//                    var clippedRect = Rectangle.Intersect(headerRect, _headerBounds);
//                    if (!clippedRect.IsEmpty)
//                    {
//                        // Draw header background
//                        if (i == _hoveredColumnIndex)
//                        {
//                            using (var hoverBrush = new SolidBrush(_currentTheme.GridHeaderHoverBackColor))
//                            {
//                                g.FillRectangle(hoverBrush, clippedRect);
//                            }
//                        }
                        
//                        // Use BeepLabel for header text rendering
//                        var headerLabel = new BeepLabel
//                        {
//                            Text = column.ColumnCaption,
//                            TextAlign = ContentAlignment.MiddleCenter,
//                            MenuStyle = MenuStyle,
//                            IsChild = true,
//                            BackColor = _currentTheme.GridHeaderBackColor,
//                            ForeColor = _currentTheme.GridHeaderForeColor
//                        };
//                        headerLabel.ApplyTheme(_currentTheme);
//                        headerLabel.Draw(g, clippedRect);
                        
//                        // Draw border
//                        g.DrawRectangle(borderPen, clippedRect);
//                    }
                    
//                    xOffset += column.Width;
//                }
//            }
//        }
        
//        private void DrawDataRows(Graphics g)
//        {
//            if (_filteredData.Count == 0 || _columns.Count == 0) return;
            
//            using (var selectedBrush = new SolidBrush(_currentTheme.GridRowSelectedBackColor))
//            using (var hoverBrush = new SolidBrush(_currentTheme.GridRowHoverBackColor))
//            {
//                for (int rowIndex = _visibleRowStart; rowIndex < _visibleRowEnd; rowIndex++)
//                {
//                    if (rowIndex >= _filteredData.Count) break;
                    
//                    var dataItem = _filteredData[rowIndex];
//                    bool isSelected = rowIndex == _selectedRowIndex;
//                    bool isHovered = rowIndex == _hoveredRowIndex;
                    
//                    // Draw row background
//                    if (isSelected || isHovered)
//                    {
//                        var rowBounds = new Rectangle(
//                            _scrollableBounds.X,
//                            _scrollableBounds.Y + (rowIndex - _visibleRowStart) * _virtualRowHeight,
//                            _scrollableBounds.Width,
//                            _virtualRowHeight
//                        );
                        
//                        var bgBrush = isSelected ? selectedBrush : hoverBrush;
//                        g.FillRectangle(bgBrush, rowBounds);
//                    }
                    
//                    // Draw cells using BeepControls
//                    DrawRowCells(g, dataItem, rowIndex);
//                }
//            }
//        }
        
//        private void DrawRowCells(Graphics g, object dataItem, int rowIndex)
//        {
//            int xOffset = _scrollableBounds.X - _horizontalOffset;
//            int yPos = _scrollableBounds.Y + (rowIndex - _visibleRowStart) * _virtualRowHeight;
            
//            for (int colIndex = 0; colIndex < _columns.Count; colIndex++)
//            {
//                var column = _columns[colIndex];
//                if (!column.Visible) continue;
                
//                var cellBounds = new Rectangle(xOffset, yPos, column.Width, _virtualRowHeight);
                
//                // Skip if outside visible area
//                if (cellBounds.Right < _scrollableBounds.Left || cellBounds.Left > _scrollableBounds.Right)
//                {
//                    xOffset += column.Width;
//                    continue;
//                }
                
//                // Clip to scrollable area
//                var clippedBounds = Rectangle.Intersect(cellBounds, _scrollableBounds);
//                if (clippedBounds.IsEmpty)
//                {
//                    xOffset += column.Width;
//                    continue;
//                }
                
//                // Get cell value
//                object cellValue = GetCellValue(dataItem, column);
                
//                // Render using BeepControls based on column type
//                RenderCellWithBeepControls(g, cellValue, clippedBounds, column, rowIndex);
                
//                xOffset += column.Width;
//            }
//        }

//        private void RenderCellWithBeepControls(Graphics g, object value, Rectangle bounds, BeepColumnConfig column, int rowIndex = -1)
//        {
//            var innerBounds = new Rectangle(bounds.X + 2, bounds.Y + 1, bounds.Width - 4, bounds.Height - 2);

//            // Handle selection checkbox column specially
//            if (column.IsSelectionCheckBox && _showCheckboxes)
//            {
//                // Get checkbox state for this row
//                bool isSelected = rowIndex >= 0 && _selectedRows.ContainsKey(rowIndex);

//                // Create temporary checkbox for rendering
//                var checkBox = new BeepCheckBoxBool
//                {
//                    HideText = true,
//                    Text = "",
//                    CurrentValue = isSelected,
//                    MenuStyle = MenuStyle,
//                    IsChild = true,
//                    Size = new Size(Math.Min(20, innerBounds.Width), Math.Min(20, innerBounds.Height)),
//                    ForeColor = _currentTheme.CheckBoxForeColor,
//                    BackColor = _currentTheme.CheckBoxBackColor
//                };

//                checkBox.ApplyTheme(_currentTheme);

//                // Center the checkbox in the cell
//                var checkboxBounds = new Rectangle(
//                    innerBounds.X + (innerBounds.Width - checkBox.Width) / 2,
//                    innerBounds.Y + (innerBounds.Height - checkBox.Height) / 2,
//                    checkBox.Width,
//                    checkBox.Height
//                );

//                checkBox.Draw(g, checkboxBounds);
//                checkBox.Dispose();
//                return; // Exit early for selection column
//            }

//            // Handle null or DBNull values
//            if (value == null || value == DBNull.Value)
//            {
//                return;
//            }

//            // Get or create the appropriate BeepControl for this column
//            if (!_columnDrawers.TryGetValue(column.ColumnName, out IBeepUIComponent columnEditor))
//            {
//                columnEditor = CreateCellControlForRendering(column);
//                _columnDrawers[column.ColumnName] = columnEditor;
//            }

//            if (columnEditor != null)
//            {
//                // Update the control with current value and theme
//                UpdateCellControl(columnEditor, column, value);

//                // Render based on column type using BeepControls
//                switch (column.CellEditor)
//                {
//                    case BeepColumnType.CheckBoxBool:
//                        if (columnEditor is BeepCheckBoxBool checkBox)
//                        {
//                            checkBox.CurrentValue = Convert.ToBoolean(value);
//                            checkBox.ForeColor = _currentTheme.CheckBoxForeColor;
//                            checkBox.BackColor = _currentTheme.CheckBoxBackColor;
//                            checkBox.Draw(g, innerBounds);
//                        }
//                        break;

//                    case BeepColumnType.CheckBoxChar:
//                        if (columnEditor is BeepCheckBoxBool charCheckBox)
//                        {
//                            // Handle Y/N, T/F, 1/0 character values
//                            bool isChecked = value.ToString().ToUpper() == "Y" ||
//                                           value.ToString().ToUpper() == "T" ||
//                                           value.ToString() == "1";
//                            charCheckBox.CurrentValue = isChecked;
//                            charCheckBox.ForeColor = _currentTheme.CheckBoxForeColor;
//                            charCheckBox.BackColor = _currentTheme.CheckBoxBackColor;
//                            charCheckBox.Draw(g, innerBounds);
//                        }
//                        break;

//                    case BeepColumnType.CheckBoxString:
//                        if (columnEditor is BeepCheckBoxBool stringCheckBox)
//                        {
//                            // Handle string values like "true"/"false", "yes"/"no"
//                            string stringValue = value.ToString().ToLowerInvariant();
//                            bool isChecked = stringValue == "true" || stringValue == "yes" ||
//                                           stringValue == "checked" || stringValue == "1";
//                            stringCheckBox.CurrentValue = isChecked;
//                            stringCheckBox.ForeColor = _currentTheme.CheckBoxForeColor;
//                            stringCheckBox.BackColor = _currentTheme.CheckBoxBackColor;
//                            stringCheckBox.Draw(g, innerBounds);
//                        }
//                        break;

//                    case BeepColumnType.ProgressBar:
//                        if (columnEditor is BeepProgressBar progressBar)
//                        {
//                            progressBar.Value = Convert.ToInt32(value);
//                            progressBar.ForeColor = _currentTheme.GridForeColor;
//                            progressBar.BackColor = _currentTheme.GridBackColor;
//                            progressBar.Draw(g, innerBounds);
//                        }
//                        break;

//                    case BeepColumnType.DateTime:
//                        if (columnEditor is BeepLabel dateLabel)
//                        {
//                            var dateStr = value is DateTime dt ? dt.ToString(column.Format ?? "g") : value.ToString();
//                            dateLabel.Text = dateStr;
//                            dateLabel.ForeColor = _currentTheme.GridForeColor;
//                            dateLabel.BackColor = _currentTheme.GridBackColor;
//                            dateLabel.Draw(g, innerBounds);
//                        }
//                        break;

//                    case BeepColumnType.Image:
//                        if (columnEditor is BeepImage image)
//                        {
//                            image.ImagePath = value.ToString();
//                            var imageSize = new Size(
//                                Math.Min(column.MaxImageWidth, innerBounds.Width),
//                                Math.Min(column.MaxImageHeight, innerBounds.Height)
//                            );
//                            var imageBounds = new Rectangle(
//                                innerBounds.X + (innerBounds.Width - imageSize.Width) / 2,
//                                innerBounds.Y + (innerBounds.Height - imageSize.Height) / 2,
//                                imageSize.Width,
//                                imageSize.Height
//                            );
//                            image.DrawImage(g, imageBounds);
//                        }
//                        break;

//                    case BeepColumnType.ComboBox:
//                        if (columnEditor is BeepLabel comboLabel)
//                        {
//                            // Display the selected item text or value
//                            string displayText = value.ToString();
//                            if (column.Items != null)
//                            {
//                                var selectedItem = column.Items.FirstOrDefault(i =>
//                                    i.Value?.ToString() == value.ToString());
//                                if (selectedItem != null)
//                                {
//                                    displayText = selectedItem.DisplayField ?? selectedItem.Text ?? value.ToString();
//                                }
//                            }
//                            comboLabel.Text = displayText;
//                            comboLabel.ForeColor = _currentTheme.GridForeColor;
//                            comboLabel.BackColor = _currentTheme.GridBackColor;
//                            comboLabel.Draw(g, innerBounds);
//                        }
//                        break;

//                    case BeepColumnType.ListBox:
//                        if (columnEditor is BeepLabel listLabel)
//                        {
//                            listLabel.Text = value.ToString();
//                            listLabel.ForeColor = _currentTheme.GridForeColor;
//                            listLabel.BackColor = _currentTheme.GridBackColor;
//                            listLabel.Draw(g, innerBounds);
//                        }
//                        break;

//                    case BeepColumnType.NumericUpDown:
//                        if (columnEditor is BeepLabel numericLabel)
//                        {
//                            // Format numeric values based on column configuration
//                            string numericText = value.ToString();
//                            if (!string.IsNullOrEmpty(column.Format))
//                            {
//                                try
//                                {
//                                    if (decimal.TryParse(value.ToString(), out decimal numValue))
//                                    {
//                                        numericText = numValue.ToString(column.Format);
//                                    }
//                                }
//                                catch
//                                {
//                                    numericText = value.ToString();
//                                }
//                            }
//                            numericLabel.Text = numericText;
//                            numericLabel.ForeColor = _currentTheme.GridForeColor;
//                            numericLabel.BackColor = _currentTheme.GridBackColor;
//                            numericLabel.TextAlign = ContentAlignment.MiddleRight; // Right-align numbers
//                            numericLabel.Draw(g, innerBounds);
//                        }
//                        break;

//                    default:
//                        // Default to BeepLabel for text
//                        if (columnEditor is BeepLabel textLabel)
//                        {
//                            string displayText = value.ToString();
//                            if (!string.IsNullOrEmpty(column.Format))
//                            {
//                                try
//                                {
//                                    displayText = string.Format(column.Format, value);
//                                }
//                                catch
//                                {
//                                    displayText = value.ToString();
//                                }
//                            }
//                            textLabel.Text = displayText;
//                            textLabel.ForeColor = _currentTheme.GridForeColor;
//                            textLabel.BackColor = _currentTheme.GridBackColor;
//                            textLabel.Draw(g, innerBounds);
//                        }
//                        break;
//                }
//            }
//        }

//        private IBeepUIComponent CreateCellControlForRendering(BeepColumnConfig column)
//        {
//            switch (column.CellEditor)
//            {
//                case BeepColumnType.CheckBoxBool:
//                    return new BeepCheckBoxBool 
//                    { 
//                        MenuStyle = MenuStyle, 
//                        IsChild = true,
//                        HideText = true,
//                        Text = string.Empty
//                    };
                    
//                case BeepColumnType.ProgressBar:
//                    return new BeepProgressBar 
//                    { 
//                        MenuStyle = MenuStyle, 
//                        IsChild = true 
//                    };
                    
//                case BeepColumnType.Image:
//                    return new BeepImage 
//                    { 
//                        MenuStyle = MenuStyle, 
//                        IsChild = true 
//                    };
                    
//                default:
//                    return new BeepLabel 
//                    { 
//                        MenuStyle = MenuStyle, 
//                        IsChild = true,
//                        TextAlign = ContentAlignment.MiddleLeft
//                    };
//            }
//        }
        
//        private void UpdateCellControl(IBeepUIComponent control, BeepColumnConfig column, object value)
//        {
//            if (control == null) return;
            
//            switch (control)
//            {
//                case BeepLabel label:
//                    label.Text = value?.ToString() ?? "";
//                    label.ForeColor = _currentTheme.GridForeColor;
//                    label.BackColor = _currentTheme.GridBackColor;
//                    break;
                    
//                case BeepCheckBoxBool checkBox:
//                    checkBox.CurrentValue = Convert.ToBoolean(value);
//                    checkBox.ForeColor = _currentTheme.GridForeColor;
//                    checkBox.BackColor = _currentTheme.GridBackColor;
//                    break;
                    
//                case BeepProgressBar progressBar:
//                    progressBar.Value = Convert.ToInt32(value);
//                    progressBar.ForeColor = _currentTheme.GridForeColor;
//                    progressBar.BackColor = _currentTheme.GridBackColor;
//                    break;
                    
//                case BeepImage image:
//                    image.ImagePath = value?.ToString() ?? "";
//                    break;
//            }
//        }
        
//        private object GetCellValue(object dataItem, BeepColumnConfig column)
//        {
//            try
//            {
//                if (dataItem is DataRow dataRow)
//                {
//                    return dataRow[column.ColumnName];
//                }
//                else
//                {
//                    var property = dataItem.GetType().GetProperty(column.ColumnName);
//                    return property?.GetValue(dataItem);
//                }
//            }
//            catch
//            {
//                return null;
//            }
//        }
        
//        private void DrawGridLines(Graphics g)
//        {
//            using (var linePen = new Pen(_currentTheme.GridLineColor))
//            {
//                // Vertical lines
//                int xOffset = _scrollableBounds.X - _horizontalOffset;
//                for (int i = 0; i < _columns.Count; i++)
//                {
//                    if (!_columns[i].Visible) continue;
                    
//                    xOffset += _columns[i].Width;
//                    if (xOffset > _scrollableBounds.X && xOffset < _scrollableBounds.Right)
//                    {
//                        g.DrawLine(linePen, xOffset, _scrollableBounds.Y, xOffset, _scrollableBounds.Bottom);
//                    }
//                }
                
//                // Horizontal lines
//                for (int row = _visibleRowStart; row <= _visibleRowEnd; row++)
//                {
//                    int y = _scrollableBounds.Y + (row - _visibleRowStart) * _virtualRowHeight;
//                    if (y >= _scrollableBounds.Y && y <= _scrollableBounds.Bottom)
//                    {
//                        g.DrawLine(linePen, _scrollableBounds.X, y, _scrollableBounds.Right, y);
//                    }
//                }
//            }
//        }
        
//        private void DrawSelection(Graphics g)
//        {
//            if (_selectedCell != null)
//            {
//                int key = _selectedRowIndex << 16 | _selectedColumnIndex;
//                if (_cachedCellBounds.TryGetValue(key, out Rectangle cellBounds))
//                {
//                    using (var selectionPen = new Pen(_currentTheme.AccentColor, 2))
//                    {
//                        g.DrawRectangle(selectionPen, cellBounds);
//                    }
//                }
//            }
//        }
        
//        #endregion
        
//        #region Scrolling
        
//        private void UpdateScrollBars()
//        {
//            UpdateVerticalScrollBar();
//            UpdateHorizontalScrollBar();
//            PositionScrollBars();
//        }
        
//        private void UpdateVerticalScrollBar()
//        {
//            int totalHeight = _virtualRowCount * _virtualRowHeight;
//            int visibleHeight = _scrollableBounds.Height;
            
//            if (totalHeight > visibleHeight)
//            {
//                _vScrollBar.Visible = true;
//                _vScrollBar.Maximum = totalHeight - visibleHeight;
//                _vScrollBar.LargeChange = visibleHeight;
//                _vScrollBar.SmallChange = _virtualRowHeight;
//                _vScrollBar.Value = Math.Min(_verticalOffset, _vScrollBar.Maximum);
//            }
//            else
//            {
//                _vScrollBar.Visible = false;
//                _verticalOffset = 0;
//            }
//        }
        
//        private void UpdateHorizontalScrollBar()
//        {
//            int totalWidth = _columns.Where(c => c.Visible).Sum(c => c.Width);
//            int visibleWidth = _scrollableBounds.Width;
            
//            if (totalWidth > visibleWidth)
//            {
//                _hScrollBar.Visible = true;
//                _hScrollBar.Maximum = totalWidth - visibleWidth;
//                _hScrollBar.LargeChange = visibleWidth;
//                _hScrollBar.SmallChange = 50;
//                _hScrollBar.Value = Math.Min(_horizontalOffset, _hScrollBar.Maximum);
//            }
//            else
//            {
//                _hScrollBar.Visible = false;
//                _horizontalOffset = 0;
//            }
//        }
        
//        private void PositionScrollBars()
//        {
//            if (_vScrollBar.Visible)
//            {
//                _vScrollBar.Bounds = new Rectangle(
//                    Width - _vScrollBar.Width,
//                    _gridBounds.Y,
//                    _vScrollBar.Width,
//                    _gridBounds.Height - (_hScrollBar.Visible ? _hScrollBar.Height : 0)
//                );
//            }
            
//            if (_hScrollBar.Visible)
//            {
//                _hScrollBar.Bounds = new Rectangle(
//                    _gridBounds.X,
//                    _gridBounds.Bottom - _hScrollBar.Height,
//                    _gridBounds.Width - (_vScrollBar.Visible ? _vScrollBar.Width : 0),
//                    _hScrollBar.Height
//                );
//            }
//        }
        
//        #endregion
        
//        #region Event Handlers
        
//        private void VScrollBar_Scroll(object sender, EventArgs e)
//        {
//            _verticalOffset = _vScrollBar.Value;
//            RecalculateVirtualRows();
//            Invalidate(_scrollableBounds);
//        }
        
//        private void HScrollBar_Scroll(object sender, EventArgs e)
//        {
//            _horizontalOffset = _hScrollBar.Value;
//            InvalidateLayout();
//        }
        
//        private void BeepGrid_MouseDown(object sender, MouseEventArgs e)
//        {
//            Focus();
            
//            if (e.Button == MouseButtons.Left)
//            {
//                var hitInfo = HitTest(e.Location);
//                if (hitInfo.Type == HitTestType.Cell)
//                {
//                    SelectCell(hitInfo.RowIndex, hitInfo.ColumnIndex);
//                }
//            }
//        }
        
//        private void BeepGrid_MouseMove(object sender, MouseEventArgs e)
//        {
//            var hitInfo = HitTest(e.Location);
            
//            if (hitInfo.Type == HitTestType.Cell)
//            {
//                if (_hoveredRowIndex != hitInfo.RowIndex || _hoveredColumnIndex != hitInfo.ColumnIndex)
//                {
//                    _hoveredRowIndex = hitInfo.RowIndex;
//                    _hoveredColumnIndex = hitInfo.ColumnIndex;
//                    Invalidate();
//                }
//            }
//            else
//            {
//                if (_hoveredRowIndex != -1 || _hoveredColumnIndex != -1)
//                {
//                    _hoveredRowIndex = -1;
//                    _hoveredColumnIndex = -1;
//                    Invalidate();
//                }
//            }
//        }
        
//        private void BeepGrid_MouseUp(object sender, MouseEventArgs e)
//        {
//            // Handle mouse up events
//        }

//        private void BeepGrid_MouseClick(object sender, MouseEventArgs e)
//        {
//            if (e.Button == MouseButtons.Left)
//            {
//                // Check if click is in navigation area
//                if (_showNavigator && navigatorPanelRect.Contains(e.Location))
//                {
//                    if (HitTest(e.Location, out HitTestArea hitArea))
//                    {
//                        HandleNavigationButtonClick(hitArea.Name);
//                        return;
//                    }
//                }

//                // Handle row selection checkbox clicks
//                if (_showCheckboxes)
//                {
//                    var hitInfo = HitTest(e.Location);
//                    if (hitInfo.Type == HitTestType.Cell && hitInfo.ColumnIndex == 0)
//                    {
//                        // Check if it's the selection column
//                        var column = _columns[hitInfo.ColumnIndex];
//                        if (column.IsSelectionCheckBox)
//                        {
//                            ToggleRowSelection(hitInfo.RowIndex);
//                            return;
//                        }
//                    }
//                    // Handle select all checkbox click in header
//                    else if (hitInfo.Type == HitTestType.ColumnHeader && hitInfo.ColumnIndex == 0)
//                    {
//                        var column = _columns[hitInfo.ColumnIndex];
//                        if (column.IsSelectionCheckBox && _selectAllCheckBox != null)
//                        {
//                            _selectAllCheckBox.CurrentValue = !_selectAllCheckBox.CurrentValue;
//                            return;
//                        }
//                    }
//                }

//            }

//            if (e.Button == MouseButtons.Right)
//            {
//                // Handle right-click context menu
//                ShowContextMenu(e.Location);
//            }
//        }

//        private void BeepGrid_MouseWheel(object sender, MouseEventArgs e)
//        {
//            if (_vScrollBar.Visible)
//            {
//                int newValue = _vScrollBar.Value - e.Delta / 120 * _virtualRowHeight * 3;
//                newValue = Math.Max(0, Math.Min(newValue, _vScrollBar.Maximum));
//                _vScrollBar.Value = newValue;
//                VScrollBar_Scroll(_vScrollBar, EventArgs.Empty);
//            }
//        }
        
//        private void BeepGrid_KeyDown(object sender, KeyEventArgs e)
//        {
//            switch (e.KeyCode)
//            {
//                case Keys.Up:
//                    MoveSelection(0, -1);
//                    e.Handled = true;
//                    break;
//                case Keys.Down:
//                    MoveSelection(0, 1);
//                    e.Handled = true;
//                    break;
//                case Keys.Left:
//                    MoveSelection(-1, 0);
//                    e.Handled = true;
//                    break;
//                case Keys.Right:
//                    MoveSelection(1, 0);
//                    e.Handled = true;
//                    break;
//                case Keys.Enter:
//                case Keys.F2:
//                    BeginEdit();
//                    e.Handled = true;
//                    break;
//                case Keys.Escape:
//                    CancelEdit();
//                    e.Handled = true;
//                    break;
//            }
//        }
        
//        private void BeepGrid_Resize(object sender, EventArgs e)
//        {
//            _layoutDirty = true;
//            BeginInvoke(new Action(() => {
//                RecalculateLayout();
//                UpdateScrollBars();
//                Invalidate();
//            }));
//        }
        
//        #endregion
        
//        #region Selection and Navigation
        
//        private void SelectCell(int rowIndex, int columnIndex)
//        {
//            if (rowIndex < 0 || rowIndex >= _virtualRowCount || 
//                columnIndex < 0 || columnIndex >= _columns.Count ||
//                !_columns[columnIndex].Visible)
//            {
//                return;
//            }
            
//            _selectedRowIndex = rowIndex;
//            _selectedColumnIndex = columnIndex;
            
//            // Create or update selected cell
//            _selectedCell = new BeepCellConfig
//            {
//                RowIndex = rowIndex,
//                ColumnIndex = columnIndex,
//                CellValue = GetCellValue(_filteredData[rowIndex], _columns[columnIndex])
//            };
            
//            // Ensure cell is visible
//            EnsureCellVisible(rowIndex, columnIndex);
            
//            // Raise selection events
//            OnCellSelected();
            
//            Invalidate();
//        }
        
//        private void MoveSelection(int deltaColumn, int deltaRow)
//        {
//            if (_selectedCell == null || _selectedRowIndex < 0 || _selectedColumnIndex < 0) return;
            
//            int newRow = _selectedRowIndex + deltaRow;
//            int newCol = _selectedColumnIndex + deltaColumn;
            
//            // Find next visible column
//            while (newCol >= 0 && newCol < _columns.Count && !_columns[newCol].Visible)
//            {
//                newCol += Math.Sign(deltaColumn);
//            }
            
//            SelectCell(newRow, newCol);
//        }
        
//        private void EnsureCellVisible(int rowIndex, int columnIndex)
//        {
//            // Ensure row is visible
//            int rowY = rowIndex * _virtualRowHeight;
//            if (rowY < _verticalOffset)
//            {
//                _verticalOffset = rowY;
//                _vScrollBar.Value = _verticalOffset;
//                RecalculateVirtualRows();
//            }
//            else if (rowY + _virtualRowHeight > _verticalOffset + _scrollableBounds.Height)
//            {
//                _verticalOffset = rowY + _virtualRowHeight - _scrollableBounds.Height;
//                _vScrollBar.Value = Math.Min(_verticalOffset, _vScrollBar.Maximum);
//                RecalculateVirtualRows();
//            }
            
//            // Ensure column is visible
//            int columnX = _columns.Take(columnIndex).Where(c => c.Visible).Sum(c => c.Width);
//            if (columnX < _horizontalOffset)
//            {
//                _horizontalOffset = columnX;
//                _hScrollBar.Value = _horizontalOffset;
//            }
//            else if (columnX + _columns[columnIndex].Width > _horizontalOffset + _scrollableBounds.Width)
//            {
//                _horizontalOffset = columnX + _columns[columnIndex].Width - _scrollableBounds.Width;
//                _hScrollBar.Value = Math.Min(_horizontalOffset, _hScrollBar.Maximum);
//            }
            
//            InvalidateLayout();
//        }

//        #endregion
//        #region Navigation Event Handlers
//        private void HandleNavigationButtonClick(string buttonName)
//        {
//            switch (buttonName)
//            {
//                case "FindButton":
//                    ShowSearch?.Invoke(this, EventArgs.Empty);
//                    break;

//                case "EditButton":
//                    if (_selectedRowIndex >= 0)
//                    {
//                        BeginEdit();
//                    }
//                    EditCalled?.Invoke(this, EventArgs.Empty);
//                    break;

//                case "PrinterButton":
//                    CallPrinter?.Invoke(this, EventArgs.Empty);
//                    break;

//                case "MessageButton":
//                    SendMessage?.Invoke(this, EventArgs.Empty);
//                    break;

//                case "SaveButton":
//                    SaveCalled?.Invoke(this, EventArgs.Empty);
//                    break;

//                case "NewButton":
//                    CreateNewRecord();
//                    NewRecordCreated?.Invoke(this, EventArgs.Empty);
//                    break;

//                case "RemoveButton":
//                    DeleteCurrentRecord();
//                    DeleteCalled?.Invoke(this, EventArgs.Empty);
//                    break;

//                case "RollbackButton":
//                    RollbackChanges();
//                    break;

//                // Record navigation
//                case "FirstRecordButton":
//                    SelectFirstRecord();
//                    break;

//                case "PreviousRecordButton":
//                    SelectPreviousRecord();
//                    break;

//                case "NextRecordButton":
//                    SelectNextRecord();
//                    break;

//                case "LastRecordButton":
//                    SelectLastRecord();
//                    break;

//                // Page navigation
//                case "FirstPageButton":
//                    NavigateToFirstPage();
//                    break;

//                case "PrevPageButton":
//                    NavigateToPreviousPage();
//                    break;

//                case "NextPageButton":
//                    NavigateToNextPage();
//                    break;

//                case "LastPageButton":
//                    NavigateToLastPage();
//                    break;
//            }
//        }
//        #endregion
//        #region Hit Testing

//        private GridHitTestInfo HitTest(Point location)
//        {
//            // Test header area
//            if (_headerBounds.Contains(location))
//            {
//                int colIndex = GetColumnIndexFromX(location.X);
//                return new GridHitTestInfo(HitTestType.ColumnHeader, -1, colIndex);
//            }
            
//            // Test scrollable area
//            if (_scrollableBounds.Contains(location))
//            {
//                int rowIndex = GetRowIndexFromY(location.Y);
//                int colIndex = GetColumnIndexFromX(location.X);
                
//                if (rowIndex >= 0 && rowIndex < _virtualRowCount && 
//                    colIndex >= 0 && colIndex < _columns.Count)
//                {
//                    return new GridHitTestInfo(HitTestType.Cell, rowIndex, colIndex);
//                }
//            }
            
//            return new GridHitTestInfo(HitTestType.None, -1, -1);
//        }
        
//        private int GetRowIndexFromY(int y)
//        {
//            if (!_scrollableBounds.Contains(new Point(_scrollableBounds.X, y))) return -1;
            
//            int relativeY = y - _scrollableBounds.Y;
//            int rowIndex = relativeY / _virtualRowHeight + _visibleRowStart;
            
//            return rowIndex < _virtualRowCount ? rowIndex : -1;
//        }
        
//        private int GetColumnIndexFromX(int x)
//        {
//            int adjustedX = x + _horizontalOffset;
//            int currentX = _gridBounds.X;
            
//            for (int i = 0; i < _columns.Count; i++)
//            {
//                if (!_columns[i].Visible) continue;
                
//                if (adjustedX >= currentX && adjustedX < currentX + _columns[i].Width)
//                {
//                    return i;
//                }
//                currentX += _columns[i].Width;
//            }
            
//            return -1;
//        }
        
//        #endregion
        
//        #region Filtering
        
//        private void ToggleFilterPanel()
//        {
//            _filterPanel.Visible = _showFilter;
            
//            if (_showFilter)
//            {
//                CreateFilterControls();
//            }
//            else
//            {
//                ClearFilterControls();
//            }
            
//            _layoutDirty = true;
//            Invalidate();
//        }
        
//        private void CreateFilterControls()
//        {
//            ClearFilterControls();
            
//            int xOffset = 5;
            
//            foreach (var column in _columns.Where(c => c.Visible))
//            {
//                var filterBox = new BeepTextBox
//                {
//                    PlaceholderText = $"Filter {column.ColumnCaption}",
//                    Width = column.Width - 2,
//                    Height = _filterPanel.Height - 4,
//                    Location = new Point(xOffset, 2),
//                    MenuStyle = MenuStyle,
//                    IsChild = true
//                };
                
//                filterBox.TextChanged += (s, e) => OnFilterChanged(column.ColumnName, filterBox.Text);
                
//                _filterControls.Add(filterBox);
//                _filterPanel.Controls.Add(filterBox);
                
//                xOffset += column.Width;
//            }
//        }
        
//        private void ClearFilterControls()
//        {
//            foreach (var control in _filterControls)
//            {
//                _filterPanel.Controls.Remove(control);
//                control.Dispose();
//            }
//            _filterControls.Clear();
//        }
        
//        private void OnFilterChanged(string columnName, string filterValue)
//        {
//            if (string.IsNullOrWhiteSpace(filterValue))
//            {
//                _columnFilters.Remove(columnName);
//            }
//            else
//            {
//                _columnFilters[columnName] = filterValue.ToLowerInvariant();
//            }
            
//            ApplyFilters();
//        }
        
//        private void ApplyFilters()
//        {
//            if (_columnFilters.Count == 0)
//            {
//                _filteredData = new List<object>(_originalData);
//            }
//            else
//            {
//                _filteredData = _originalData.Where(item => MatchesFilters(item)).ToList();
//            }
            
//            _virtualRowCount = _filteredData.Count;
//            _selectedRowIndex = -1;
//            _selectedColumnIndex = -1;
//            _selectedCell = null;
            
//            RecalculateVirtualRows();
//            UpdateScrollBars();
//            Invalidate();
//        }
        
//        private bool MatchesFilters(object item)
//        {
//            foreach (var filter in _columnFilters)
//            {
//                var column = _columns.FirstOrDefault(c => c.ColumnName == filter.Key);
//                if (column == null) continue;
                
//                var cellValue = GetCellValue(item, column);
//                if (cellValue == null) return false;
                
//                string cellText = cellValue.ToString().ToLowerInvariant();
//                if (!cellText.Contains(filter.Value)) return false;
//            }
            
//            return true;
//        }
        
//        #endregion
        
//        #region Cell Editing
        
//        private void BeginEdit()
//        {
//            if (_selectedCell == null || _selectedRowIndex < 0 || _selectedColumnIndex < 0) return;
            
//            var column = _columns[_selectedColumnIndex];
//            if (column.ReadOnly) return;
            
//            // Get cell bounds for editor positioning
//            int key = _selectedRowIndex << 16 | _selectedColumnIndex;
//            if (!_cachedCellBounds.TryGetValue(key, out Rectangle cellBounds)) return;
            
//            // Create appropriate editor control
//            _cellEditor = CreateCellEditor(column, _selectedCell.CellValue);
//            if (_cellEditor == null) return;
            
//            _cellEditor.Bounds = cellBounds;
//            _cellEditor.Leave += CellEditor_Leave;
//            _cellEditor.KeyDown += CellEditor_KeyDown;
            
//            Controls.Add(_cellEditor);
//            _cellEditor.BringToFront();
//            _cellEditor.Focus();
            
//            _editingCell = _selectedCell;
//        }
        
//        private Control CreateCellEditor(BeepColumnConfig column, object value)
//        {
//            switch (column.CellEditor)
//            {
//                case BeepColumnType.Text:
//                    var textBox = new BeepTextBox
//                    {
//                        Text = value?.ToString() ?? "",
//                        MenuStyle = MenuStyle,
//                        IsChild = true
//                    };
//                    return textBox;
                    
//                case BeepColumnType.CheckBoxBool:
//                    var checkBox = new BeepCheckBoxBool
//                    {
//                        CurrentValue = Convert.ToBoolean(value),
//                        MenuStyle = MenuStyle,
//                        IsChild = true
//                    };
//                    return checkBox;
                    
//                case BeepColumnType.ComboBox:
//                    var comboBox = new BeepComboBox
//                    {
//                        MenuStyle = MenuStyle,
//                        IsChild = true
//                    };
//                    if (column.Items != null)
//                    {
//                        comboBox.ListItems = new BindingList<SimpleItem>(column.Items);
//                        comboBox.SelectedItem = column.Items.FirstOrDefault(i => 
//                            i.Value?.ToString() == value?.ToString());
//                    }
//                    return comboBox;
                    
//                default:
//                    return new BeepTextBox
//                    {
//                        Text = value?.ToString() ?? "",
//                        MenuStyle = MenuStyle,
//                        IsChild = true
//                    };
//            }
//        }
        
//        private void CellEditor_Leave(object sender, EventArgs e)
//        {
//            EndEdit();
//        }
        
//        private void CellEditor_KeyDown(object sender, KeyEventArgs e)
//        {
//            switch (e.KeyCode)
//            {
//                case Keys.Enter:
//                    EndEdit();
//                    MoveSelection(0, 1); // Move to next row
//                    e.Handled = true;
//                    break;
//                case Keys.Escape:
//                    CancelEdit();
//                    e.Handled = true;
//                    break;
//                case Keys.Tab:
//                    EndEdit();
//                    MoveSelection(e.Shift ? -1 : 1, 0); // Move to next/previous column
//                    e.Handled = true;
//                    break;
//            }
//        }
        
//        private void EndEdit()
//        {
//            if (_cellEditor == null || _editingCell == null) return;
            
//            // Get new value from editor
//            object newValue = GetEditorValue(_cellEditor);
            
//            // Update data source
//            UpdateDataSource(_selectedRowIndex, _selectedColumnIndex, newValue);
            
//            // Update cell value
//            _editingCell.CellValue = newValue;
            
//            // Clean up editor
//            Controls.Remove(_cellEditor);
//            _cellEditor.Dispose();
//            _cellEditor = null;
//            _editingCell = null;
            
//            // Refresh display
//            Invalidate();
            
//            // Raise value changed event
//            OnCellValueChanged();
//        }
        
//        private void CancelEdit()
//        {
//            if (_cellEditor == null) return;
            
//            Controls.Remove(_cellEditor);
//            _cellEditor.Dispose();
//            _cellEditor = null;
//            _editingCell = null;
//        }
        
//        private object GetEditorValue(Control editor)
//        {
//            switch (editor)
//            {
//                case BeepTextBox textBox:
//                    return textBox.Text;
//                case BeepCheckBoxBool checkBox:
//                    return checkBox.CurrentValue;
//                case BeepComboBox comboBox:
//                    return comboBox.SelectedItem?.Value;
//                default:
//                    return editor.Text;
//            }
//        }

//        private void UpdateDataSource(int rowIndex, int columnIndex, object newValue)
//        {
//            UpdateDataSourceEnhanced(rowIndex, columnIndex, newValue);
//        }
//        #endregion

//        #region Context Menu

//        private void ShowContextMenu(Point location)
//        {
//            // Implement context menu functionality
//            var contextMenu = new ContextMenuStrip();
            
//            contextMenu.Items.Add("Copy", null, (s, e) => CopySelectedCell());
//            contextMenu.Items.Add("Paste", null, (s, e) => PasteToSelectedCell());
//            contextMenu.Items.Add("-");
//            contextMenu.Items.Add("Edit", null, (s, e) => BeginEdit());
//            contextMenu.Items.Add("-");
//            contextMenu.Items.Add("Filter by Value", null, (s, e) => FilterBySelectedValue());
            
//            contextMenu.Show(this, location);
//        }
        
//        private void CopySelectedCell()
//        {
//            if (_selectedCell?.CellValue != null)
//            {
//                Clipboard.SetText(_selectedCell.CellValue.ToString());
//            }
//        }
        
//        private void PasteToSelectedCell()
//        {
//            if (_selectedCell != null && Clipboard.ContainsText())
//            {
//                var clipboardText = Clipboard.GetText();
//                UpdateDataSource(_selectedRowIndex, _selectedColumnIndex, clipboardText);
//                _selectedCell.CellValue = clipboardText;
//                Invalidate();
//                OnCellValueChanged();
//            }
//        }
        
//        private void FilterBySelectedValue()
//        {
//            if (_selectedCell?.CellValue != null && _selectedColumnIndex >= 0)
//            {
//                var column = _columns[_selectedColumnIndex];
//                _columnFilters[column.ColumnName] = _selectedCell.CellValue.ToString().ToLowerInvariant();
//                ApplyFilters();
                
//                // Update filter control if visible
//                if (_showFilter && _filterControls.Count > _selectedColumnIndex)
//                {
//                    if (_filterControls[_selectedColumnIndex] is BeepTextBox filterBox)
//                    {
//                        filterBox.Text = _selectedCell.CellValue.ToString();
//                    }
//                }
//            }
//        }
        
//        #endregion
        
//        #region Events
        
//        public event EventHandler CellSelected;
//        public event EventHandler CellValueChanged;
//        public event EventHandler<GridHitTestInfo> CellClicked;
//        public event EventHandler<GridHitTestInfo> CellDoubleClicked;
        
//        protected virtual void OnCellSelected()
//        {
//            CellSelected?.Invoke(this, EventArgs.Empty);
//        }
        
//        protected virtual void OnCellValueChanged()
//        {
//            CellValueChanged?.Invoke(this, EventArgs.Empty);
//        }
        
//        #endregion
        
//        #region MenuStyle Support
        
//        public override void ApplyTheme()
//        {
//            base.ApplyTheme();
            
//            // Apply theme to child controls
//            _titleLabel?.ApplyTheme(_currentTheme);
//            _headerPanel?.ApplyTheme(_currentTheme);
//            _filterPanel?.ApplyTheme(_currentTheme);
//            _footerPanel?.ApplyTheme(_currentTheme);
//            _vScrollBar?.ApplyTheme(_currentTheme);
//            _hScrollBar?.ApplyTheme(_currentTheme);
            
//            foreach (var control in _filterControls)
//            {
//                if (control is IBeepUIComponent beepControl)
//                {
//                    beepControl.ApplyTheme(_currentTheme);
//                }
//            }
            
//            // Apply theme to column editors
//            foreach (var editor in _columnDrawers.Values)
//            {
//                editor?.ApplyTheme(_currentTheme);
//            }
            
//            Invalidate();
//        }
        
//        #endregion
        
//        #region Helper Methods
        
//        private void InvalidateLayout()
//        {
//            _layoutDirty = true;
//            BeginInvoke(new Action(() => {
//                if (_layoutDirty)
//                {
//                    RecalculateLayout();
//                    Invalidate();
//                }
//            }));
//        }
        
//        private void ApplyGridStyle()
//        {
//            switch (_gridStyle)
//            {
//                case BeepGridStyle.Modern:
//                    _headerHeight = Math.Max(_headerHeight, 36);
//                    break;
//                case BeepGridStyle.Material:
//                    _headerHeight = Math.Max(_headerHeight, 40);
//                    break;
//                case BeepGridStyle.Dark:
//                    _headerHeight = Math.Max(_headerHeight, 34);
//                    break;
//            }
//        }

//        #endregion
//        #region Hit Testing Methods
//        private void AddHitArea(string name, Rectangle bounds)
//        {
//            _hitAreas.Add(new HitTestArea
//            {
//                Name = name,
//                Bounds = bounds
//            });
//        }

//        private bool HitTest(Point location, out HitTestArea hitTest)
//        {
//            hitTest = null;
//            foreach (var area in _hitAreas)
//            {
//                if (area.Bounds.Contains(location))
//                {
//                    hitTest = area;
//                    return true;
//                }
//            }
//            return false;
//        }

//        private void ClearHitAreas()
//        {
//            _hitAreas.Clear();
//        }
//        #endregion
//        #region Navigation Drawing
//        private void DrawNavigationRow(Graphics g, Rectangle rect)
//        {
//            // Clear hit areas for navigation buttons
//            ClearHitAreas();

//            // Fill background
//            using (var brush = new SolidBrush(_currentTheme.GridHeaderBackColor))
//            {
//                g.FillRectangle(brush, rect);
//            }

//            // Draw top border line
//            using (var pen = new Pen(_currentTheme.GridLineColor))
//            {
//                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
//            }

//            // Calculate layout positions
//            int buttonHeight = 24;
//            int buttonWidth = 24;
//            int padding = 6;
//            int y = rect.Top + (rect.Height - buttonHeight) / 2;
//            int x = rect.Left + padding;

//            // Draw navigation buttons (left side)
//            DrawNavigationButton(g, "FindButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            DrawNavigationButton(g, "EditButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.062-pencil.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            DrawNavigationButton(g, "PrinterButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.072-printer.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            DrawNavigationButton(g, "MessageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.083-share.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            DrawNavigationButton(g, "SaveButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.036-floppy disk.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            DrawNavigationButton(g, "NewButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.068-plus.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            DrawNavigationButton(g, "RemoveButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.035-eraser.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            DrawNavigationButton(g, "RollbackButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.005-back arrow.svg",
//                new Rectangle(x, y, buttonWidth, buttonHeight));
//            x += buttonWidth + padding;

//            // Draw record counter (center) with navigation buttons
//            string recordCounter = _filteredData != null && _filteredData.Any()
//                ? $"{_selectedRowIndex + 1} - {_filteredData.Count}"
//                : "0 - 0";

//            // Calculate center area for record counter display
//            using (var font = new Font(Font.FontFamily, 9f))
//            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
//            {
//                SizeF textSize = TextUtils.MeasureText(g,recordCounter, font);
//                float recordX = rect.Left + (rect.Width - textSize.Width) / 2;

//                // Add record navigation buttons around the counter
//                int navButtonWidth = 20;
//                int navButtonSpacing = 6;

//                // First Record button
//                DrawNavigationButton(g, "FirstRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg",
//                    new Rectangle((int)recordX - navButtonWidth * 2 - navButtonSpacing * 2, y, navButtonWidth, buttonHeight));

//                // Previous Record button
//                DrawNavigationButton(g, "PreviousRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg",
//                    new Rectangle((int)recordX - navButtonWidth - navButtonSpacing, y, navButtonWidth, buttonHeight));

//                // Draw the record counter text
//                g.DrawString(recordCounter, font, brush, recordX, y + (buttonHeight - textSize.Height) / 2);

//                // Next Record button
//                DrawNavigationButton(g, "NextRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg",
//                    new Rectangle((int)(recordX + textSize.Width + navButtonSpacing), y, navButtonWidth, buttonHeight));

//                // Last Record button
//                DrawNavigationButton(g, "LastRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg",
//                    new Rectangle((int)(recordX + textSize.Width + navButtonWidth + navButtonSpacing * 2), y, navButtonWidth, buttonHeight));
//            }

//            // Draw pagination controls (right side)
//            int pageButtonX = rect.Right - padding - buttonWidth;

//            DrawNavigationButton(g, "LastPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg",
//                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
//            pageButtonX -= buttonWidth + padding;

//            DrawNavigationButton(g, "NextPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg",
//                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
//            pageButtonX -= buttonWidth + padding;

//            // Calculate total pages
//            int visibleRowsPerPage = _scrollableBounds.Height / _virtualRowHeight;
//            if (visibleRowsPerPage <= 0) visibleRowsPerPage = 1;
//            _totalPages = _filteredData != null ?
//                (int)Math.Ceiling((double)_filteredData.Count / visibleRowsPerPage) : 1;
//            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));

//            string pageCounter = $"{_currentPage} of {_totalPages}";

//            using (var font = new Font(Font.FontFamily, 9f))
//            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
//            {
//                SizeF textSize = TextUtils.MeasureText(g,pageCounter, font);
//                pageButtonX -= (int)textSize.Width + padding;
//                g.DrawString(pageCounter, font, brush, pageButtonX, y + (buttonHeight - textSize.Height) / 2);
//            }

//            pageButtonX -= buttonWidth + padding;
//            DrawNavigationButton(g, "PrevPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg",
//                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
//            pageButtonX -= buttonWidth + padding;

//            DrawNavigationButton(g, "FirstPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg",
//                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
//        }
//        #endregion
//        #region Individual Navigation Methods
//        private void CreateNewRecord()
//        {
//            // Add a new empty record to the data source
//            if (_dataSource is IList list && !list.IsReadOnly)
//            {
//                try
//                {
//                    // Create new instance of the data type
//                    object newItem = null;
//                    if (_dataType != null)
//                    {
//                        newItem = Activator.CreateInstance(_dataType);
//                    }
//                    else if (list.Count > 0)
//                    {
//                        // Try to create based on existing item
//                        var firstItem = list[0];
//                        newItem = Activator.CreateInstance(firstItem.GetType());
//                    }

//                    if (newItem != null)
//                    {
//                        list.Add(newItem);
//                        RefreshData();

//                        // Select the new record
//                        SelectCell(_virtualRowCount - 1, 0);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error creating new record: {ex.Message}", "BeepGrid",
//                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                }
//            }
//        }

//        private void DeleteCurrentRecord()
//        {
//            if (_selectedRowIndex >= 0 && _selectedRowIndex < _filteredData.Count)
//            {
//                var result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
//                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//                if (result == DialogResult.Yes)
//                {
//                    try
//                    {
//                        var itemToDelete = _filteredData[_selectedRowIndex];

//                        // Remove from original data source
//                        if (_dataSource is IList list && !list.IsReadOnly)
//                        {
//                            list.Remove(itemToDelete);
//                        }

//                        RefreshData();

//                        // Adjust selection
//                        if (_selectedRowIndex >= _virtualRowCount)
//                        {
//                            SelectCell(_virtualRowCount - 1, _selectedColumnIndex);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Error deleting record: {ex.Message}", "BeepGrid",
//                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    }
//                }
//            }
//        }

//        private void RollbackChanges()
//        {
//            // Refresh data from original source to undo changes
//            RefreshData();
//            Invalidate();
//        }

//        private void SelectFirstRecord()
//        {
//            if (_virtualRowCount > 0)
//            {
//                SelectCell(0, Math.Max(0, _selectedColumnIndex));
//            }
//        }

//        private void SelectPreviousRecord()
//        {
//            if (_selectedRowIndex > 0)
//            {
//                SelectCell(_selectedRowIndex - 1, _selectedColumnIndex);
//            }
//        }

//        private void SelectNextRecord()
//        {
//            if (_selectedRowIndex < _virtualRowCount - 1)
//            {
//                SelectCell(_selectedRowIndex + 1, _selectedColumnIndex);
//            }
//        }

//        private void SelectLastRecord()
//        {
//            if (_virtualRowCount > 0)
//            {
//                SelectCell(_virtualRowCount - 1, Math.Max(0, _selectedColumnIndex));
//            }
//        }

//        private void NavigateToFirstPage()
//        {
//            if (_currentPage > 1)
//            {
//                _currentPage = 1;
//                NavigateToPage(_currentPage);
//            }
//        }

//        private void NavigateToPreviousPage()
//        {
//            if (_currentPage > 1)
//            {
//                _currentPage--;
//                NavigateToPage(_currentPage);
//            }
//        }

//        private void NavigateToNextPage()
//        {
//            if (_currentPage < _totalPages)
//            {
//                _currentPage++;
//                NavigateToPage(_currentPage);
//            }
//        }

//        private void NavigateToLastPage()
//        {
//            if (_currentPage < _totalPages)
//            {
//                _currentPage = _totalPages;
//                NavigateToPage(_currentPage);
//            }
//        }

//        private void NavigateToPage(int pageNumber)
//        {
//            // Calculate which records should be visible for this page
//            int visibleRowsPerPage = _scrollableBounds.Height / _virtualRowHeight;
//            if (visibleRowsPerPage <= 0) visibleRowsPerPage = 1;

//            int startRow = (pageNumber - 1) * visibleRowsPerPage;

//            // Scroll to show the page
//            int targetOffset = startRow * _virtualRowHeight;
//            _verticalOffset = Math.Min(targetOffset, _vScrollBar.Maximum);
//            _vScrollBar.Value = _verticalOffset;

//            RecalculateVirtualRows();

//            // Select first visible row of the page
//            if (startRow < _virtualRowCount)
//            {
//                SelectCell(startRow, Math.Max(0, _selectedColumnIndex));
//            }

//            Invalidate();
//        }
//        #endregion
//        #region Event Handlers

//        protected virtual void OnBeforeDataUpdate(DataUpdateEventArgs e)
//        {
//            BeforeDataUpdate?.Invoke(this, e);
//        }

//        protected virtual void OnAfterDataUpdate(DataUpdateEventArgs e)
//        {
//            AfterDataUpdate?.Invoke(this, e);
//        }

//        protected virtual void OnDataUpdateFailed(DataUpdateFailedEventArgs e)
//        {
//            DataUpdateFailed?.Invoke(this, e);
//        }

//        protected virtual void OnValidationFailed(ValidationFailedEventArgs e)
//        {
//            ValidationFailed?.Invoke(this, e);
//        #endregion
//        #region Dispose

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                ClearFilterControls();
                
//                _cellEditor?.Dispose();
//                _cellEditor = null;
                
//                _vScrollBar?.Dispose();
//                _hScrollBar?.Dispose();
//                _headerPanel?.Dispose();
//                _filterPanel?.Dispose();
//                _footerPanel?.Dispose();
//                _titleLabel?.Dispose();
                
//                _cachedCellBounds?.Clear();
//                _cachedTextSizes?.Clear();
//                _hitAreas?.Clear();
                
//                // Dispose column editors
//                foreach (var editor in _columnDrawers.Values)
//                {
//                    if (editor is IDisposable disposable)
//                    {
//                        disposable.Dispose();
//                    }
//                }
//                _columnDrawers?.Clear();
//            }
            
//            base.Dispose(disposing);
//        }
        
//        #endregion
//    }
    

//}
