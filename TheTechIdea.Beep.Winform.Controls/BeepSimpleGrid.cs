﻿

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.Reflection;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Desktop.Common.Helpers;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Shared;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;
using TheTechIdea.Beep.Winform.Controls.Grid;
using TheTechIdea.Beep.Winform.Controls.Models;



namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A grid control that displays data in a simple table format with scrollbars.")]
    [DisplayName("Beep Simple Grid")]
    public class BeepSimpleGrid : BeepControl
    {
        #region Properties
        #region Fields
        protected int headerPanelHeight = 20;
        protected int bottomagregationPanelHeight = 12;
        protected int footerPanelHeight = 12;
        protected int navigatorPanelHeight = 20;

        private Rectangle footerPanelRect;
        private Rectangle headerPanelRect;
        private Rectangle columnsheaderPanelRect;
        private Rectangle bottomagregationPanelRect;
        private Rectangle navigatorPanelRect;
        private Rectangle filterButtonRect;
        private BeepLabel titleLabel;
        private BeepButton filterButton;
        private int visibleHeight;
        private int visibleRowCount;
        private int bottomPanelY;
        private int botomspacetaken = 0;
        private int topPanelY;
        private Rectangle gridRect;
        private int _rowHeight = 25;
        private int _xOffset = 0;
        private bool _showFilterButton = true;
        private bool _resizingColumn = false;
        private bool _resizingRow = false;
        private int _resizingIndex = -1;
        private int _resizeMargin = 2;
        private Point _lastMousePos;
        private int _editingRowIndex = -1; // Add this field to track absolute row index in _fullData
        private BeepGridRow _hoveredRow = null;
        private BeepGridCell _hoveredCell = null;
        private BeepGridCell _selectedCell = null;
        private int _hoveredRowIndex = -1;
        private int _hoveredCellIndex = -1;
        private int _selectedRowIndex = -1;
        private int _hoveredColumnIndex = -1;
        private int _selectedColumnIndex = -1;
        private int _hoveredColumnHeaderIndex = -1;
        private int _selectedColumnHeaderIndex = -1;
        private int _hoveredRowHeaderIndex = -1;
        private int _selectedRowHeaderIndex = -1;
        private int _hoveredSortIconIndex = -1;
        private int _hoveredFilterIconIndex = -1;
        private int _selectedSortIconIndex = -1;
        private int _selectedFilterIconIndex = -1;
        private List<Rectangle> columnHeaderBounds = new List<Rectangle>();
        private List<Rectangle> rowHeaderBounds = new List<Rectangle>();
        private List<Rectangle> sortIconBounds = new List<Rectangle>();
        private List<Rectangle> filterIconBounds = new List<Rectangle>();
        private int _defaultcolumnheaderheight = 40;
        private int _defaultcolumnheaderwidth = 50;
        #endregion Fields
        #region Title Properties
        private TextImageRelation textImageRelation = TextImageRelation.ImageAboveText;
        private string _titletext = "Simple BeepGrid";
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titletext;
            set
            {
                _titletext = value;
                if (titleLabel != null)
                {
                    titleLabel.Text = value;
                }
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Image alignment relative to text.")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                textImageRelation = value;
                if (titleLabel != null)
                {
                    titleLabel.TextImageRelation = value;
                }
                Invalidate();
            }
        }
        private Font _textFont;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TitleTextFont
        {
            get => _textFont ?? Font;
            set
            {
                _textFont = value;
                if (titleLabel != null)
                {
                    titleLabel.TextFont = _textFont;
                }
                Invalidate();
            }
        }
        private Font _columnHeadertextFont = new Font("Arial", 8);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for column headers.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font ColumnHeaderFont
        {
            get => _columnHeadertextFont;
            set
            {
                _columnHeadertextFont = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Layout")]
        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                _rowHeight = value;
                InitializeRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        #endregion "Title Properties"
        #region "Data Source Properties"

        private object _dataSource;
        object finalData;
        private List<object> _fullData;
        private int _dataOffset = 0;
        public IEntityStructure Entity { get; set; }

        [Browsable(true)]
        [Category("Data")]
        [AttributeProvider(typeof(IListSource))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public new object DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                DataSetup();
                InitializeData();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        public string QueryFunctionName { get; set; }
        public string QueryFunction { get; set; }
        #endregion "Data Source Properties"
        #region "Appearance Properties"
        private bool columnssetupusingeditordontchange = false;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ColumnsSetupUsingEditorDontChange
        {
            get => columnssetupusingeditordontchange;
            set => columnssetupusingeditordontchange = value;
        }
        [Browsable(true)]
        [Category("Data")]
        // [Editor(typeof(BeepGridColumnConfigCollectionEditor), typeof(UITypeEditor))] // Uncomment if you have a custom editor
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<BeepGridColumnConfig> Columns
        {
            get
            {
                //   Debug.WriteLine($"Columns Getter: Count = {_columns.Count}, DesignMode = {DesignMode}");
                return _columns;
            }
            set
            {
                //   Debug.WriteLine($"Columns Setter: Count = {value?.Count}, DesignMode = {DesignMode}");
                if (value != null && value != _columns)
                {
                    _columns = value;
                    if (_columns.Any())
                    {
                        columnssetupusingeditordontchange = true;
                        //     Debug.WriteLine("Columns set via property, marking as designer-configured");
                    }
                }
                UpdateScrollBars();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DefaultColumnHeaderWidth
        {
            get => _defaultcolumnheaderwidth;
            set { _defaultcolumnheaderwidth = value; Invalidate(); }
        }

        private bool _showverticalgridlines = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowVerticalGridLines
        {
            get => _showverticalgridlines;
            set
            {
                _showverticalgridlines = value;
                Invalidate();
            }
        }
        private bool _showhorizontalgridlines = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowHorizontalGridLines
        {
            get => _showhorizontalgridlines;
            set
            {
                _showhorizontalgridlines = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowFilterButton
        {
            get => _showFilterButton;
            set
            {
                _showFilterButton = value;
                Invalidate();
            }
        }
        private bool _showSortIcons = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowSortIcons
        {
            get => _showSortIcons;
            set
            {
                _showSortIcons = value;
                Invalidate();
            }
        }
        private bool _showRowNumbers = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowRowNumbers
        {
            get => _showRowNumbers;
            set
            {
                _showRowNumbers = value;
                Invalidate();
            }
        }
        private bool _showColumnHeaders = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowColumnHeaders
        {
            get => _showColumnHeaders;
            set
            {
                _showColumnHeaders = value;
                Invalidate();
            }
        }
        private bool _showRowHeaders = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowRowHeaders
        {
            get => _showRowHeaders;
            set
            {
                _showRowHeaders = value;
                Invalidate();
            }
        }
        private bool _showNavigator = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set
            {
                _showNavigator = value;
                Invalidate();
            }
        }
        private bool _showBottomRow = false;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowBottomRow
        {
            get => _showBottomRow;
            set
            {
                _showBottomRow = value;
                Invalidate();
            }
        }
        private bool _showFooter = false;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowFooter
        {
            get => _showFooter;
            set
            {
                _showFooter = value;
                Invalidate();
            }
        }
        private bool _showHeaderPanel = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowHeaderPanel
        {
            get => _showHeaderPanel;
            set
            {
                _showHeaderPanel = value;
                Invalidate();
            }
        }
        private bool _showHeaderPanelBorder = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowHeaderPanelBorder
        {
            get => _showHeaderPanelBorder;
            set
            {
                _showHeaderPanelBorder = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ColumnHeaderHeight
        {
            get => _defaultcolumnheaderheight;
            set { _defaultcolumnheaderheight = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Layout")]
        [Description("Horizontal offset for drawing grid cells.")]
        public int XOffset
        {
            get => _xOffset;
            set
            {
                _xOffset = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        public GridDataSourceType DataSourceType { get; set; } = GridDataSourceType.Fixed;
        private string _title = "BeepSimpleGrid Title";
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        // Scrollbar Properties
        private BeepScrollBar _verticalScrollBar;
        private BeepScrollBar _horizontalScrollBar;
        private bool _showVerticalScrollBar = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowVerticalScrollBar
        {
            get => _showVerticalScrollBar;
            set
            {
                _showVerticalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        private bool _showHorizontalScrollBar = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHorizontalScrollBar
        {
            get => _showHorizontalScrollBar;
            set
            {
                _showHorizontalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }

        public bool IsEditorShown { get; private set; }
        #endregion "Appearance Properties"
        #region "Configuration Properties"
        public BindingList<BeepGridRow> Rows { get; set; } = new BindingList<BeepGridRow>();
        public BeepGridRow BottomRow { get; set; }
        public BeepBindingNavigator DataNavigator { get; set; }

        private List<BeepGridColumnConfig> _columns = new List<BeepGridColumnConfig>();
        #endregion "Configuration Properties"
        #endregion "Properties"
        #region Constructor
        public BeepSimpleGrid():base()
        {
            // This ensures child controls are painted properly
            this.SetStyle(ControlStyles.ContainerControl, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            // Ensure _columns is only initialized once
            SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse , true);
            TabStop = true;
            this.Focus();

            this.PreviewKeyDown += BeepGrid_PreviewKeyDown;

            this.KeyDown += BeepSimpleGrid_KeyDown; 
         

            Width = 200;
            Height = 200;
            // Attach event handlers for mouse actions
            this.MouseClick += BeepGrid_MouseClick;
            
            this.MouseDown += BeepGrid_MouseDown;
            this.MouseMove += BeepGrid_MouseMove;
            this.MouseUp += BeepGrid_MouseUp;
            TabKeyPressed += Tabhandler;
            
            Rows.ListChanged += Rows_ListChanged;
            ApplyThemeToChilds = false;
            // Initialize scrollbars immediately to ensure they’re never null
            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            Controls.Add(_horizontalScrollBar);
            DataNavigator = new BeepBindingNavigator
            {
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Theme = Theme,
               
            };
            ApplyThemeToChilds = false;
            Controls.Add(DataNavigator);

            titleLabel = new BeepLabel
            {
                Text = string.IsNullOrEmpty(Title) ? "Beep Grid" : Title,
                TextAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                MaxImageSize = new Size(20, 20),
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };
            titleLabel.BackColor = _currentTheme?.BackColor ?? Color.White;

            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            Controls.Add(_horizontalScrollBar);

            InitializeRows();
        }

        #endregion
        #region Initialization
        private void DataSetup()
        {
            IEntityStructure entity = null;
            object resolvedData = null; // Unified variable for final data or type resolution
          //  Debug.WriteLine($"DataSetup Started: _dataSource Type = {_dataSource?.GetType()}, DesignMode = {DesignMode}, Columns Count = {_columns.Count}");

            // Step 1: Handle different _dataSource types
            if (_dataSource == null)
            {
               // Debug.WriteLine("DataSource is null, no entity generated");
            }
            else if (_dataSource is BindingSource bindingSrc)
            {
              //  Debug.WriteLine($"BindingSource Detected: DataSource = {bindingSrc.DataSource?.GetType()}, DataMember = {bindingSrc.DataMember}");
                var dataSource = bindingSrc.DataSource;

                if (dataSource == null)
                {
                 //   Debug.WriteLine("BindingSource.DataSource is null, checking BindingSource.List");
                    resolvedData = bindingSrc.List; // Could be IList, DataView, etc.
                }
                else if (dataSource is Type type)
                {
                    // Handle Type (design-time or runtime)
                  //  Debug.WriteLine($"{(DesignMode ? "Design-time" : "Runtime")}: DataSource is Type = {type.FullName}");
                    if (!string.IsNullOrEmpty(bindingSrc.DataMember))
                    {
                        PropertyInfo dataMemberProp = type.GetProperty(bindingSrc.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (dataMemberProp != null)
                        {
                            Type itemType = GetItemTypeFromDataMember(dataMemberProp.PropertyType);
                            if (itemType != null)
                            {
                         //       Debug.WriteLine($"Resolved ItemType from DataMember '{bindingSrc.DataMember}' = {itemType.FullName}");
                                entity = EntityHelper.GetEntityStructureFromType(itemType);
                            }
                            else
                            {
                           //     Debug.WriteLine($"Could not extract item type from DataMember '{bindingSrc.DataMember}' property type: {dataMemberProp.PropertyType}");
                            }
                        }
                        else
                        {
                         //   Debug.WriteLine($"DataMember '{bindingSrc.DataMember}' not found on Type {type.FullName}");
                        }
                    }
                    else
                    {
                     //   Debug.WriteLine("No DataMember specified with Type DataSource, using Type directly");
                        entity = EntityHelper.GetEntityStructureFromType(type);
                    }
                }
                else
                {
                    // Handle instance (could be DataTable, IList, or class with DataMember)
                    if (!string.IsNullOrEmpty(bindingSrc.DataMember))
                    {
                        PropertyInfo prop = dataSource.GetType().GetProperty(bindingSrc.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (prop != null)
                        {
                            resolvedData = prop.GetValue(dataSource);
                          //  Debug.WriteLine($"Resolved data from DataMember = {resolvedData?.GetType()}");
                        }
                        else
                        {
                          //  Debug.WriteLine($"DataMember '{bindingSrc.DataMember}' not found on instance, using BindingSource.List or DataSource");
                            resolvedData = bindingSrc.List != null && bindingSrc.List.Count > 0 ? bindingSrc.List : dataSource;
                        }
                    }
                    else
                    {
                      //  Debug.WriteLine("No DataMember, attempting auto-detection or using BindingSource.List");
                        resolvedData = GetCollectionPropertyFromInstance(dataSource) ?? (bindingSrc.List != null && bindingSrc.List.Count > 0 ? bindingSrc.List : dataSource);
                      //  Debug.WriteLine($"Resolved data = {resolvedData?.GetType()}");
                    }
                }
            }
            else if (_dataSource is DataTable dataTable)
            {
              //  Debug.WriteLine("DataSource is DataTable");
                resolvedData = dataTable;
            }
            else if (_dataSource is IList iList)
            {
               // Debug.WriteLine("DataSource is IList");
                resolvedData = iList;
            }
            else
            {
              //  Debug.WriteLine($"DataSource is unrecognized type: {_dataSource.GetType()}, attempting auto-detection");
                resolvedData = GetCollectionPropertyFromInstance(_dataSource) ?? _dataSource;
            }

            // Step 2: Process resolvedData to set finalData and entity
            if (entity == null) // Entity not set yet (i.e., not a Type case)
            {
                if (resolvedData == null)
                {
                  //  Debug.WriteLine("Resolved data is null, no entity generated");
                }
                else if (resolvedData is DataTable dt)
                {
                    finalData = dt;
                    entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
                }
                else if (resolvedData is IList list && list.Count > 0)
                {
                    finalData = list;
                    Type itemType = list[0]?.GetType();
                    if (itemType != null)
                    {
                    //    Debug.WriteLine($"Extracted item type from IList: {itemType.FullName}");
                        entity = EntityHelper.GetEntityStructureFromType(itemType);
                    }
                    else
                    {
                        entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
                    }
                }
                else
                {
                   // Debug.WriteLine($"Resolved data is not a recognized collection type: {resolvedData.GetType()}, using as-is");
                    finalData = resolvedData;
                    entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
                }
            }
            else
            {
                // Entity already set from Type handling, finalData remains null (structure only)
               // Debug.WriteLine("Entity set from Type handling, finalData remains null");
            }

            //Step 3: Process entity and columns
            if (entity != null)
            {
              //  Debug.WriteLine($"New Entity: {entity.EntityName}, Existing Entity: {Entity?.EntityName}, Columns Count = {_columns.Count}");
                if (_columns.Any() )
                {
                    if (Entity != null && entity.EntityName.Equals(Entity.EntityName))
                    {
                    //    Debug.WriteLine("Preserving designer Columns, syncing fields");
                      //  SyncFields(entity);
                        SyncColumnsWithEntity(entity);
                    }
                    else
                    {
                     //   Debug.WriteLine("New Entity with designer Columns, updating Entity only");
                        Entity = entity;
                        SyncColumnsWithEntity(entity);
                    }
                }
                else
                {
                   // Debug.WriteLine("No designer Columns or not protected, regenerating");
                    Entity = entity;
                    CreateColumnsForEntity();
                }
            }
            else if (_columns.Any() && columnssetupusingeditordontchange)
            {
              //  Debug.WriteLine("No new Entity, keeping designer Columns");
            }

            //Debug.WriteLine($"DataSetup Completed: finalData = {finalData?.GetType()}, Entity = {Entity?.EntityName}, Columns Count = {_columns.Count}");
        }
        private object GetCollectionPropertyFromInstance(object instance)
        {
            if (instance == null) return null;

            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo collectionProp = null;
            int collectionCount = 0;

            foreach (var prop in properties)
            {
                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var value = prop.GetValue(instance);
                    if (value != null)
                    {
                        collectionProp = prop;
                        collectionCount++;
                     //   Debug.WriteLine($"Found collection property: {prop.Name}, Type = {prop.PropertyType}");
                    }
                }
            }

            if (collectionCount == 1 && collectionProp != null)
            {
                return collectionProp.GetValue(instance);
            }
            else if (collectionCount > 1)
            {
             //   Debug.WriteLine("Multiple collection properties found, please specify DataMember");
            }
            return null; // No single collection found or all are null
        }
        private Type GetItemTypeFromDataMember(Type propertyType)
        {
           // Debug.WriteLine($"GetItemTypeFromDataMember: PropertyType = {propertyType.FullName}");

            // Handle generic IEnumerable<T> (e.g., List<T>)
            if (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                Type[] genericArgs = propertyType.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                 //   Debug.WriteLine($"Extracted item type: {genericArgs[0].FullName}");
                    return genericArgs[0];
                }
            }

            // Handle DataTable
            if (propertyType == typeof(DataTable))
            {
              //  Debug.WriteLine("DataTable detected, returning DataRow");
                return typeof(DataRow);
            }

            // Handle arrays
            if (propertyType.IsArray)
            {
             //   Debug.WriteLine($"Array detected, element type: {propertyType.GetElementType().FullName}");
                return propertyType.GetElementType();
            }

          //  Debug.WriteLine("No item type extracted");
            return null;
        }
        private void SyncColumnsWithEntity(IEntityStructure entity)
        {
          //  Debug.WriteLine("Syncing Columns with Entity");

            // Map existing columns by name for quick lookup
            var columnMap = _columns.ToDictionary(c => c.ColumnName, c => c, StringComparer.OrdinalIgnoreCase);

            // Sync fields into columns, preserving designer settings
            foreach (var field in entity.Fields)
            {
                if (columnMap.TryGetValue(field.fieldname, out var existingColumn))
                {
                    // Update type-related properties, preserve designer settings
                    existingColumn.PropertyTypeName = field.fieldtype ?? existingColumn.PropertyTypeName ?? typeof(object).AssemblyQualifiedName;
                    existingColumn.ColumnType = MapPropertyTypeToDbFieldCategory(existingColumn.PropertyTypeName);
              //      existingColumn.CellEditor = MapPropertyTypeToCellEditor(existingColumn.PropertyTypeName);
                    // Designer settings like Width, Visible, ColumnCaption remain unchanged
                //    Debug.WriteLine($"Synced Column '{field.fieldname}': Type = {existingColumn.PropertyTypeName}");
                }
                else
                {
                    // Add new column if it doesn’t exist, with defaults
                    var newColumn = new BeepGridColumnConfig
                    {
                        ColumnName = field.fieldname,
                        ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.fieldname),
                        PropertyTypeName = field.fieldtype ?? typeof(object).AssemblyQualifiedName,
                        Width = 100,
                        Visible = true,
                        SortMode = DataGridViewColumnSortMode.Automatic,
                        Resizable = DataGridViewTriState.True,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    };
                    newColumn.ColumnType = MapPropertyTypeToDbFieldCategory(newColumn.PropertyTypeName);
                    newColumn.CellEditor = MapPropertyTypeToCellEditor(newColumn.PropertyTypeName);
                    _columns.Add(newColumn);
                  //  Debug.WriteLine($"Added new Column '{field.fieldname}': Type = {newColumn.PropertyTypeName}");
                }
            }

            // Remove columns not in the new Entity (optional, comment out if you want to keep extras)
            _columns.RemoveAll(c => !entity.Fields.Any(f => f.fieldname.Equals(c.ColumnName, StringComparison.OrdinalIgnoreCase)));

            // Re-index columns
            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].Index = i;
            }

            Invalidate();
        }
     
        public void CreateColumnsForEntity()
        {
            try
            {
                // Clear any existing columns if that's what you intend
                Columns.Clear();

                foreach (var field in Entity.Fields)
                {
                    // Create a new BeepGridColumnConfig using PropertyTypeName as a string
                    var colConfig = new BeepGridColumnConfig
                    {
                        ColumnName = field.fieldname,  // The name in the data source
                        ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.fieldname),  // The user-facing caption
                        PropertyTypeName = field.fieldtype,  // Store the string representation of the type
                        GuidID = Guid.NewGuid().ToString(),
                        Visible = true,
                        Width = 100,
                        SortMode = DataGridViewColumnSortMode.Automatic,
                        Resizable = DataGridViewTriState.True,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    };

                    // Map PropertyTypeName to ColumnType and CellEditor using existing methods
                    colConfig.ColumnType = MapPropertyTypeToDbFieldCategory(colConfig.PropertyTypeName);
                    colConfig.CellEditor = MapPropertyTypeToCellEditor(colConfig.PropertyTypeName);

                    // Additional customizations based on type (optional, if you want specific overrides)
                    Type fieldType = Type.GetType(colConfig.PropertyTypeName, throwOnError: false) ?? typeof(object);
                    if (fieldType == typeof(DateTime))
                    {
                        colConfig.Format = "g"; // Add format for DateTime if needed
                    }
                    else if (fieldType.IsEnum)
                    {
                        // Optionally populate Items with enum values
                        var values = Enum.GetValues(fieldType);
                        colConfig.Items = new List<SimpleItem>();
                        foreach (var val in values)
                        {
                            colConfig.Items.Add(new SimpleItem { Value = val, Text = val.ToString() });
                        }
                    }

                    // Add the column config to the grid’s Columns list
                    Columns.Add(colConfig);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding columns for Entity {Entity?.EntityName}: {ex.Message}");
            }
        }

        private void InitializeRows()
        {
            Rows.Clear();
            if (_fullData == null || !_fullData.Any()) return; // No rows if no data
            int visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
            int visibleRowCount = visibleHeight / _rowHeight;
            int dataRowCount = _fullData.Count;

            // Limit rows to the lesser of visible rows or data rows
            int rowCount = Math.Min(visibleRowCount, dataRowCount);
            for (int i = 0; i < rowCount; i++)
            {
                var row = new BeepGridRow();
                foreach (var col in Columns)
                {
                    // Create a new cell that will later be filled with data.
                    // Note: We no longer assign UIComponent here.
                    var cell = new BeepGridCell
                    {
                        // Do not default CellValue to string.Empty if FillVisibleRows will update it.
                        // You can leave it null or use a default if desired.
                        CellValue = null,
                        CellData = null,
                        IsEditable =true,
                        ColumnIndex = col.Index,
                        IsVisible = col.Visible,
                        RowIndex = i

                    };
                    row.Cells.Add(cell);
                }
                Rows.Add(row);
            }

            UpdateScrollBars();
        }
        private void InitializeData()
        {
            _fullData = finalData is IEnumerable<object> enumerable ? enumerable.ToList() : new List<object>();
            _dataOffset = 0;

            if (_columns.Count == 0 && _fullData.Any())
            {
                var firstItem = _fullData.First();
                var properties = firstItem.GetType().GetProperties();
                int index = 0;
                foreach (var prop in properties)
                {
                    // Use AssemblyQualifiedName for PropertyTypeName
                    string propertyTypeName = prop.PropertyType.AssemblyQualifiedName;
                    var columnConfig = new BeepGridColumnConfig
                    {
                        ColumnCaption = prop.Name,
                        ColumnName = prop.Name,
                        Width = 100,
                        Index = index++,
                        Visible = true,
                        PropertyTypeName = propertyTypeName // Full AssemblyQualifiedName
                    };

                    // Map using PropertyTypeName string
                    columnConfig.ColumnType = MapPropertyTypeToDbFieldCategory(propertyTypeName);
                    columnConfig.CellEditor = MapPropertyTypeToCellEditor(propertyTypeName);

                    _columns.Add(columnConfig);
                }
            }
            InitializeRows();
            UpdateScrollBars();
        }
        #endregion
        #region Data Filling and Navigation
        private void FillVisibleRows()
        {
            if (_fullData == null || !_fullData.Any()) return;

            for (int i = 0; i < Rows.Count; i++)
            {
                int dataIndex = _dataOffset + i;
                var row = Rows[i];
                if (dataIndex < _fullData.Count)
                {
                    var dataItem = _fullData[dataIndex];
                    for (int j = 0; j < row.Cells.Count && j < Columns.Count; j++)
                    {
                        var col = Columns[j];
                        var prop = dataItem.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                        var value = prop?.GetValue(dataItem) ?? string.Empty;
                        row.Cells[j].CellValue = value;
                        row.Cells[j].CellData = value;
                    }
                }
                else
                {
                    // Clear cells beyond data (optional, since we won’t draw these rows)
                    foreach (var cell in row.Cells)
                    {
                        cell.CellValue = null;
                        cell.CellData = null;
                    }
                }
            }
            UpdateScrollBars();
        }
        private void UpdateDataRecordFromRow(BeepGridCell editingCell)
        {
            BeepGridRow row = Rows[editingCell.RowIndex];

            if (_fullData == null || !_fullData.Any()) return;
            int rowIndex = Rows.IndexOf(row);
            if (rowIndex < 0) return;
            int dataIndex = _dataOffset + rowIndex;
            if (dataIndex < _fullData.Count)
            {
                var dataItem = _fullData[dataIndex];
                foreach (var cell in row.Cells)
                {
                    if (cell.IsDirty)
                    {
                        var prop = dataItem.GetType().GetProperty(Columns[cell.ColumnIndex].ColumnName);
                        if (prop != null)
                        {
                            object convertedValue = MiscFunctions.ConvertValueToPropertyType(prop.PropertyType, cell.CellValue);
                            prop.SetValue(dataItem, convertedValue);
                        }
                        row.IsDirty = true;
                    }
                }
            }
        }
        private void ScrollBy(int delta)
        {
            // Calculate the new offset
            int newOffset = _dataOffset + delta;

            // Calculate the maximum possible offset (so the last page of Rows is still visible)
            // For example, if _fullData.Count=100 and Rows.Count=10, 
            // then the highest offset is 90 (showing items 90..99).
            int maxOffset = Math.Max(0, _fullData.Count - Rows.Count);

            // Clamp newOffset between 0 and maxOffset
            if (newOffset < 0)
                newOffset = 0;
            else if (newOffset > maxOffset)
                newOffset = maxOffset;

            // Only do the work if the offset actually changed
            if (newOffset != _dataOffset)
            {
                _dataOffset = newOffset;
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        public void MoveNextRow()
        {
            ScrollBy(1);  // +1 means scroll down one row
        }
        public void MovePreviousRow()
        {
            ScrollBy(-1); // -1 means scroll up one row
        }
        public void MoveNextCell()
        {
            try
            {
                if (Rows == null || Rows.Count == 0 || Columns == null || Columns.Count == 0)
                {
                 //   Debug.WriteLine("MoveNextCell: No rows or columns available.");
                    return;
                }

              //  Debug.WriteLine($"MoveNextCell: Current position - Row: {_selectedRowIndex}, Col: {_selectedColumnIndex}");

                int lastVisibleColumn = GetLastVisibleColumn();
                int firstVisibleColumn = GetNextVisibleColumn(-1); // Find the first column
                int nextColumn = GetNextVisibleColumn(_selectedColumnIndex);

              //  Debug.WriteLine($"LastVisibleColumn: {lastVisibleColumn}, FirstVisibleColumn: {firstVisibleColumn}, NextColumn: {nextColumn}");

                // 🔹 If at the last column of the last row, wrap back to the first row/column
                if (_selectedRowIndex == Rows.Count - 1 && _selectedColumnIndex == lastVisibleColumn)
                {
                    if (firstVisibleColumn == -1)
                    {
              //          Debug.WriteLine("No visible columns to wrap to.");
                        return;
                    }
               //     Debug.WriteLine($"🔄 Wrapping to first row, first column: {firstVisibleColumn}");
                    SelectCell(0, firstVisibleColumn);
                    EnsureColumnVisible(firstVisibleColumn);
                    return;
                }

                // 🔹 If at the last visible column but more columns exist, scroll and move to the next column
                if (_selectedColumnIndex == lastVisibleColumn && nextColumn != -1)
                {
                //    Debug.WriteLine($"➡ Scrolling to next column: {nextColumn}");
                    EnsureColumnVisible(nextColumn);
                    SelectCell(_selectedRowIndex, nextColumn);
                    return;
                }

                // 🔹 If at the last visible column of the row, move to the first column of the next row
                if (_selectedColumnIndex == lastVisibleColumn)
                {
                    if (_selectedRowIndex < Rows.Count - 1)
                    {
               //         Debug.WriteLine($"➡ Moving to next row, first column: {firstVisibleColumn}");
                        SelectCell(_selectedRowIndex + 1, firstVisibleColumn);
                        EnsureColumnVisible(firstVisibleColumn);
                        ScrollBy(1); // Ensure next row is visible
                    }
                    else
                    {
              //          Debug.WriteLine($"🔄 Wrapping to first column in the same row: {firstVisibleColumn}");
                        SelectCell(_selectedRowIndex, firstVisibleColumn);
                        EnsureColumnVisible(firstVisibleColumn);
                    }
                }
                else
                {
                    // Move to next visible column
                    if (nextColumn == -1)
                    {
               //         Debug.WriteLine("❌ No next visible column found.");
                        return;
                    }
               //     Debug.WriteLine($"➡ Moving to next column: {nextColumn}");
                    EnsureColumnVisible(nextColumn);
                    SelectCell(_selectedRowIndex, nextColumn);
                    Focus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MoveNextCell crashed: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        // Helper method to find the last visible column
        private int GetLastVisibleColumn()
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                 //   Debug.WriteLine("GetLastVisibleColumn: Columns is null or empty.");
                    return -1;
                }

                for (int i = Columns.Count - 1; i >= 0; i--)
                {
                    if (Columns[i] != null && Columns[i].Visible)
                        return i;
                }
                return -1; // No visible columns
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLastVisibleColumn crashed: {ex.Message}");
                return -1;
            }
        }

        // Helper method to find the next visible column
        private int GetNextVisibleColumn(int currentIndex)
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                  //  Debug.WriteLine("GetNextVisibleColumn: Columns is null or empty.");
                    return -1;
                }

                for (int i = currentIndex + 1; i < Columns.Count; i++)
                {
                    if (Columns[i] != null && Columns[i].Visible)
                        return i;
                }
                return -1; // No visible column found
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetNextVisibleColumn crashed: {ex.Message}");
                return -1;
            }
        }
        public void MovePreviousCell()
        {
            // If there are no rows or columns, do nothing.
            if (Rows.Count == 0 || Columns.Count == 0)
                return;

            // If we're at the first cell, do nothing.
            if (_selectedRowIndex == 0 && _selectedColumnIndex == 0)
                return;

            if (_selectedColumnIndex > 0)
            {
                // Ensure previous column index is within bounds
                int prevColumn = Math.Max(_selectedColumnIndex - 1, 0);
                SelectCell(_selectedRowIndex, prevColumn);
                EnsureColumnVisible(prevColumn);
            }
            else if (_selectedRowIndex > 0)
            {
                // Move to last column of the previous row
                int lastColumn = Columns.Count - 1;
                SelectCell(_selectedRowIndex - 1, lastColumn);
                EnsureColumnVisible(lastColumn);
            }
        }
        private void EnsureColumnVisible(int colIndex)
        {
            try
            {
                if (colIndex < 0 || colIndex >= Columns.Count || Columns[colIndex] == null || !Columns[colIndex].Visible)
                {
                 //   Debug.WriteLine($"EnsureColumnVisible: Invalid column index {colIndex}");
                    return;
                }

                int columnLeft = 0;
                for (int i = 0; i < colIndex; i++)
                {
                    if (Columns[i] != null && Columns[i].Visible)
                        columnLeft += Columns[i].Width;
                }

                int columnRight = columnLeft + Columns[colIndex].Width;

                int visibleLeft = _xOffset;
                int visibleWidth = DrawingRect.Width - (_verticalScrollBar?.Visible == true ? _verticalScrollBar.Width : 0);
                int visibleRight = visibleLeft + visibleWidth;

                // 🔹 Adjust scroll offset based on position
                if (columnLeft < visibleLeft)
                {
                    _xOffset = columnLeft; // Scroll left
                }
                else if (columnRight > visibleRight)
                {
                    _xOffset = columnRight - visibleWidth; // Scroll right
                }

                _xOffset = Math.Max(0, Math.Min(_xOffset, _horizontalScrollBar?.Maximum ?? 0));

                if (_horizontalScrollBar != null && _horizontalScrollBar.Visible)
                {
                    _horizontalScrollBar.Value = _xOffset;
                }

                UpdateScrollBars();
                UpdateCellPositions(); // 🔹 Ensure cells get updated after scrolling
                Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EnsureColumnVisible crashed: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        private void UpdateCellPositions()
        {
            int yOffset = _dataOffset * _rowHeight;
            int xOffset = _xOffset;

            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var row = Rows[rowIndex];
                row.UpperY = rowIndex * _rowHeight - yOffset; // 🔹 Adjust row Y position

                int x = -xOffset;
                for (int colIndex = 0; colIndex < Columns.Count; colIndex++)
                {
                    if (Columns[colIndex].Visible)
                    {
                        var cell = row.Cells[colIndex];
                        cell.X = x;
                      
                        x += Columns[colIndex].Width;
                    }
                }
            }
        }
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (m.Msg == 0x0100) // WM_KEYDOWN
            {
                Keys keyData = (Keys)m.WParam | ModifierKeys;
                if (keyData == Keys.Tab)
                {
                    Debug.WriteLine("BeepSimpleGrid: Tab key previewed");
                    MoveNextCell();
                    Invalidate();
                    return true; // Consume the key
                }
            }
            return base.ProcessKeyPreview(ref m);
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            var keyCode = (keyData & Keys.KeyCode);
            bool shiftPressed = (keyData & Keys.Shift) == Keys.Shift;

            switch (keyCode)
            {
                case Keys.Tab:
                    if (shiftPressed)
                    {
                        MovePreviousCell(); // Optional: Handle Shift+Tab if desired
                    }
                    else
                    {
                        MoveNextCell();
                    }
                    Invalidate();
                    return true; // Stop default tab navigation

                default:
                    return base.ProcessDialogKey(keyData); // Let base class handle other keys
            }
        }
        private void BeepGrid_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            
            e.IsInputKey = e.KeyCode switch
            {
                Keys.Tab or Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Enter => true,
                _ => e.IsInputKey
            };
        }
        private void BeepSimpleGrid_KeyDown(object? sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true; // 🔹 Prevent default tab behavior
          
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    MoveNextCell();
                    break;
                case Keys.Tab:
                  
                    MoveNextCell();
                    break;
                case Keys.Up:
                    if (_selectedRowIndex > 0)
                    {
                        SelectCell(_selectedRowIndex - 1, _selectedColumnIndex);
                    }
                    break;
                case Keys.Down:
                    if (_selectedRowIndex < Rows.Count - 1)
                    {
                        SelectCell(_selectedRowIndex + 1, _selectedColumnIndex);
                    }
                    break;
                case Keys.Left:
                    if (_selectedColumnIndex > 0)
                    {
                        SelectCell(_selectedRowIndex, _selectedColumnIndex - 1);
                    }
                    break;
                case Keys.Right:
                    if (_selectedColumnIndex < Columns.Count - 1)
                    {
                        SelectCell(_selectedRowIndex, _selectedColumnIndex + 1);
                    }
                    break;
                case Keys.Escape:
                    CancelEditing();
                    CloseCurrentEditor();
                    break;
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus(); // Ensure the grid gets keyboard focus when clicked
        }

        #endregion
        #region Control Creation and Updating
        private BeepControl CreateCellControlForEditing(BeepGridCell cell)
        {
            // Get the column definition based on cell.ColumnIndex.
            var column = Columns[cell.ColumnIndex];

            switch (column.CellEditor)
            {
                case BeepGridColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
                case BeepGridColumnType.CheckBoxBool:
                    return new BeepCheckBoxBool { Theme = Theme,HideText=true,Text=string.Empty, IsChild = true };
                case BeepGridColumnType.CheckBoxString:
                    return new BeepCheckBoxString { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepGridColumnType.CheckBoxChar:
                    return new BeepCheckBoxChar { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepGridColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items) };
                case BeepGridColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true };
                case BeepGridColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true };
                case BeepGridColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true };
                case BeepGridColumnType.NumericUpDown:
                    return new BeepNumericUpDown { Theme = Theme, IsChild = true };
                default:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
            }
        }
        private void UpdateCellControl(IBeepUIComponent control, BeepGridColumnConfig column, object value)
        {
            if (control == null) return;
            string stringValue = value?.ToString() ?? ""; // Ensure it's always a string

            switch (control)
            {
                case BeepTextBox textBox:
                    textBox.Text = stringValue;
                    if (column != null)
                        textBox.MaskFormat = GetTextBoxMaskFormat(column.ColumnType);
                    break;

                case BeepCheckBoxBool checkBox:
                    if (value == null)
                    {
                        checkBox.State = CheckBoxState.Indeterminate; // Explicitly set Indeterminate for null
                    }
                    else
                    {
                        checkBox.CurrentValue = (bool)MiscFunctions.ConvertValueToPropertyType(typeof(bool), value);
                    }
                    break;

                case BeepCheckBoxChar checkBox:
                    if (value == null)
                    {
                        checkBox.State = CheckBoxState.Indeterminate; // Explicitly set Indeterminate for null
                    }
                    else
                    {
                        checkBox.CurrentValue = (char)MiscFunctions.ConvertValueToPropertyType(typeof(char), value);
                    }
                    break;

                case BeepCheckBoxString checkBox:
                    if (value == null)
                    {
                        checkBox.State =    CheckBoxState.Indeterminate; // Explicitly set Indeterminate for null
                    }
                    else
                    {
                        checkBox.CurrentValue = (string)MiscFunctions.ConvertValueToPropertyType(typeof(string), value);
                    }
                    break;
                case BeepComboBox comboBox:
                   
                    comboBox.Reset();
                    if (column?.Items != null)
                    {
                        comboBox.ListItems = new BindingList<SimpleItem>(column.Items);
                        var item = column.Items.FirstOrDefault(i => i.Value?.ToString() == stringValue || i.Text == stringValue);
                        if (item != null)
                        {
                            comboBox.SelectedItem = item;
                        }
                    }
                    break;

                case BeepDatePicker datePicker:
                    if (DateTime.TryParse(stringValue, out DateTime dateValue))
                    {
                        datePicker.SelectedDate = stringValue;
                    }
                    else
                    {
                        datePicker.SelectedDate = null; // Clear if invalid
                    }
                    break;

                case BeepImage image:
                    image.ImagePath = ImageListHelper.GetImagePathFromName(stringValue);
                    break;

                case BeepButton button:
                    button.Text = stringValue;
                    break;

                case BeepProgressBar progressBar:
                    progressBar.Value = int.TryParse(stringValue, out int progress) ? progress : 0;
                    break;

                case BeepStarRating starRating:
                    starRating.SelectedRating = int.TryParse(stringValue, out int rating) ? rating : 0;
                    break;

                case BeepNumericUpDown numericUpDown:
                    numericUpDown.Value = (decimal)MiscFunctions.ConvertValueToPropertyType(typeof(decimal), stringValue);
                    break;

                case BeepSwitch switchControl:
                    switchControl.Checked = bool.TryParse(stringValue, out bool switchVal) && switchVal;
                    break;

                case BeepListofValuesBox listBox:
                    listBox.SelectedKey = stringValue;
                    break;

                case BeepLabel label:
                    label.Text = stringValue;
                    break;
            }
        }
        private TextBoxMaskFormat GetTextBoxMaskFormat(DbFieldCategory columnType)
        {
            return columnType switch
            {
                DbFieldCategory.Numeric => TextBoxMaskFormat.Numeric,
                DbFieldCategory.Date => TextBoxMaskFormat.Date,
                DbFieldCategory.Timestamp => TextBoxMaskFormat.DateTime,
                DbFieldCategory.Currency => TextBoxMaskFormat.Currency,
                _ => TextBoxMaskFormat.None
            };
        }
        #endregion
        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
          //  base.OnPaint(e);
            UpdateDrawingRect();
            var g = e.Graphics;
            var drawingBounds = DrawingRect;

            bottomPanelY = drawingBounds.Bottom;
            botomspacetaken = 0;
            topPanelY = drawingBounds.Top;

            if (_showNavigator)
            {
                bottomPanelY -= navigatorPanelHeight;
                botomspacetaken += navigatorPanelHeight;
                navigatorPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), navigatorPanelHeight);
                DrawNavigationRow(g, navigatorPanelRect);
            }
            else
            {
                navigatorPanelRect = new Rectangle(-100, -100, drawingBounds.Width, navigatorPanelHeight);
                DrawNavigationRow(g, navigatorPanelRect);
            }

            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), footerPanelHeight);
                DrawFooterRow(g, footerPanelRect);
            }

            if (_showBottomRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), bottomagregationPanelHeight);
                DrawBottomAggregationRow(g, bottomagregationPanelRect);
            }

            if (_showHeaderPanel)
            {
                headerPanelHeight = titleLabel?.GetPreferredSize(Size.Empty).Height ?? headerPanelHeight;
                headerPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), headerPanelHeight);
                DrawHeaderPanel(g, headerPanelRect);
                topPanelY += headerPanelHeight;
                botomspacetaken += headerPanelHeight;
            }

            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), ColumnHeaderHeight);
                PaintColumnHeaders(g, columnsheaderPanelRect);
                topPanelY += ColumnHeaderHeight;
                botomspacetaken += ColumnHeaderHeight;
            }

            int availableHeight = drawingBounds.Height - botomspacetaken - (_horizontalScrollBar.Visible ? _horizontalScrollBar.Height : 0);
            int availableWidth = drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
            gridRect = new Rectangle(drawingBounds.Left, topPanelY, availableWidth, availableHeight);
           
            PaintRows(g, gridRect);

            if (_showverticalgridlines)
                DrawColumnBorders(g, gridRect);
            if (_showhorizontalgridlines)
                DrawRowsBorders(g, gridRect);

            PositionScrollBars();
        }
        private void DrawBottomAggregationRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.FillRectangle(brush, rect);
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                if (BottomRow != null)
                {
                    int xOffset = rect.Left + XOffset;
                    for (int i = 0; i < BottomRow.Cells.Count && i < Columns.Count; i++)
                    {
                        var cellRect = new Rectangle(xOffset, rect.Top, Columns[i].Width, rect.Height);
                        PaintCell(g, BottomRow.Cells[i], cellRect);
                        xOffset += Columns[i].Width;
                    }
                }
            }
        }
        private void DrawFooterRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.FillRectangle(brush, rect);
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                g.DrawString("Footer", Font, new SolidBrush(_currentTheme.PrimaryTextColor), rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }
        private void DrawNavigationRow(Graphics g, Rectangle rect)
        {
            DataNavigator.Location = new Point(rect.Left, rect.Top + 1);
            DataNavigator.Size = new Size(rect.Width, rect.Height);
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }
        }
        private void PaintColumnHeaders(Graphics g, Rectangle bounds)
        {
            int xOffset = bounds.Left - XOffset; // Ensure same offset as rows
            for (int i = 0; i < Columns.Count; i++)
            {
                var col = Columns[i];
                if (!col.Visible) continue;

                string headerText = col.ColumnCaption;
                var columnRect = new Rectangle(xOffset, bounds.Top, col.Width, bounds.Height);

                if (columnRect.Right >= bounds.Left && columnRect.Left < bounds.Right + Columns.Last().Width) // Only draw visible columns
                {
                    using (var textBrush = new SolidBrush(_currentTheme.ButtonForeColor))
                    {
                        g.DrawString(headerText, _columnHeadertextFont ?? Font, textBrush, columnRect,
                            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                }

                xOffset += col.Width;
               // if (xOffset > bounds.Right) break; // Stop drawing when out of bounds
            }

            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
            }
        }
        private void PaintRows(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            for (int i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                var rowRect = new Rectangle(bounds.Left, yOffset, bounds.Width, RowHeight);
                PaintRow(g, row, rowRect);
                yOffset += _rowHeight;
                if (yOffset >= bounds.Bottom) break;
            }
        }
        private void PaintRow(Graphics g, BeepGridRow row, Rectangle rowRect)
        {
            int xOffset = rowRect.Left - XOffset; // Apply scrolling offset
            for (int i = 0; i < row.Cells.Count && i < Columns.Count; i++)
            {
                var cell = row.Cells[i];
                if (!Columns[i].Visible) continue;
                // 🔹 Update BeepGridCell with new calculated position
                cell.X = xOffset;
                cell.Y = rowRect.Top;
                cell.Width = Columns[i].Width;
                cell.Height = rowRect.Height;

                var cellRect = new Rectangle(cell.X, cell.Y, cell.Width, cell.Height);

                if (cellRect.Right >= gridRect.Left && cellRect.Left < gridRect.Right) // Draw only visible cells
                {
                    PaintCell(g, cell, cellRect);
                }

                xOffset += Columns[i].Width;
              //  if (xOffset > rowRect.Right) break; // Stop drawing if outside the visible area
            }
        }
        private void PaintCell(Graphics g, BeepGridCell cell, Rectangle cellRect)
        {
            // If this cell is being edited, skip drawing so that
            // the editor control remains visible.
          //  if (_selectedCell == cell && !_columns[_selectedCell.ColumnIndex].ReadOnly) return;
            Rectangle TargetRect = cellRect;

            if (_selectedCell == cell)
            {
                using (var cellBrush = new SolidBrush(_currentTheme.ActiveBorderColor)) // ✅ Use a highlight color
                {
                    g.FillRectangle(cellBrush, TargetRect);
                }
                using (var borderPen = new Pen(_currentTheme.PrimaryTextColor, 2)) // ✅ Add a border to show selection
                {
                    g.DrawRectangle(borderPen, TargetRect);
                }
            }
            else
            {
                using (var cellBrush = new SolidBrush(_currentTheme.BackColor))
                {
                    g.FillRectangle(cellBrush, TargetRect);
                }
            }
            // Get the column editor if available
            if (!_columnEditors.TryGetValue(Columns[cell.ColumnIndex].ColumnName, out IBeepUIComponent columnEditor))

            {

                // Create a new control if it doesn't exist (failsafe)
                columnEditor = CreateCellControlForEditing(cell);
                _columnEditors[Columns[cell.ColumnIndex].ColumnName] = columnEditor;
            }

            if (columnEditor != null)
            {
             
                // 🔹 Correctly update control bounds
                var editor = (Control)columnEditor;
                editor.Bounds = new Rectangle(TargetRect.X, TargetRect.Y, TargetRect.Width, TargetRect.Height);
             
                // 🔹 Update the control value to match the cell's data
                UpdateCellControl(columnEditor, Columns[cell.ColumnIndex], cell.CellValue);

                // 🔹 Draw the editor or its representation
                switch (columnEditor)
                {
                    case BeepTextBox textBox:
                        textBox.Draw(g, TargetRect);
                        break;
                    case BeepCheckBoxBool checkBox1:
                        checkBox1.Draw(g, TargetRect );
                        break;
                    case BeepCheckBoxChar checkBox2:
                        checkBox2.Draw(g, TargetRect);
                        break;
                    case BeepCheckBoxString checkBox3:
                        checkBox3.Draw(g, TargetRect);
                        break;
                    case BeepComboBox comboBox:
                        comboBox.Draw(g, TargetRect);
                        break;
                    case BeepDatePicker datePicker:
                        datePicker.Draw(g, TargetRect);
                        break;
                    case BeepImage image:
                        image.DrawImage(g, TargetRect);
                        break;
                    case BeepButton button:
                        button.Draw(g, TargetRect);
                        break;
                    case BeepProgressBar progressBar:
                        progressBar.Draw(g, TargetRect);
                        break;
                    case BeepStarRating starRating:
                        starRating.Draw(g, TargetRect);
                        break;
                    case BeepNumericUpDown numericUpDown:
                        numericUpDown.Draw(g, TargetRect);
                        break;
                    case BeepSwitch switchControl:
                        switchControl.Draw(g, TargetRect);
                        break;
                    case BeepListofValuesBox listBox:
                        listBox.Draw(g, TargetRect);
                        break;
                    case BeepLabel label:
                        label.Draw(g, TargetRect);
                        break;
                    default:
                        g.DrawString(cell.UIComponent.ToString(), Font, new SolidBrush(_currentTheme.PrimaryTextColor),
                            TargetRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                        break;
                }
            }
        }
        private void DrawColumnBorders(Graphics g, Rectangle bounds)
        {
            int xOffset = bounds.Left - XOffset; // Adjust for scrolling
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (!Columns[i].Visible) continue;
                    int columnRight = xOffset + Columns[i].Width;

                    if (columnRight >= bounds.Left && columnRight < bounds.Right) // Ensure only visible borders are drawn
                    {
                        g.DrawLine(pen, columnRight, bounds.Top, columnRight, bounds.Bottom);
                    }

                    xOffset += Columns[i].Width;
                    if (xOffset > bounds.Right) break; // Stop drawing when out of bounds
                }
            }
        }
        private void DrawRowsBorders(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    yOffset += _rowHeight;
                    if (yOffset < bounds.Bottom)
                        g.DrawLine(pen, bounds.Left, yOffset, bounds.Right, yOffset);
                }
            }
        }
        private void DrawHeaderPanel(Graphics g, Rectangle rect)
        {
            using (var borderPen = new Pen(_currentTheme.BorderColor))
            {
                if (ShowHeaderPanelBorder)
                {
                    g.DrawLine(borderPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                }
            }
            titleLabel.TextFont = _textFont;
            titleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            titleLabel.Size = new Size(rect.Width - 2, rect.Height - 2);
            titleLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            titleLabel.BackColor = _currentTheme.BackColor;
            titleLabel.Text = TitleText;
            titleLabel.Theme = Theme;
            titleLabel.DrawToGraphics(g, rect);
        }
        #endregion
        #region Cell Editing
        public void SelectCell(int rowIndex, int columnIndex)
        {
            if (IsEditorShown)
            {
                CloseCurrentEditor();
            }
            if (rowIndex < 0 || rowIndex >= Rows.Count) return;
            if (columnIndex < 0 || columnIndex >= Columns.Count) return;

            _selectedRowIndex = rowIndex;
            _selectedColumnIndex = columnIndex;
            _selectedCell = Rows[rowIndex].Cells[columnIndex];

            // 🔹 Use updated X and Y positions
            int cellX = _selectedCell.X;
            int cellY = _selectedCell.Y;
            Point cellLocation = new Point(cellX, cellY);

            //if (_selectedCell.IsEditable)
            //{
            //    IsEditorShown = true;
            //    ShowCellEditor(_selectedCell, cellLocation);
            //}
            
            Invalidate();
        }
        public void SelectCell(BeepGridCell cell)
        {
            if (cell == null) return;
            _editingRowIndex = cell.RowIndex; // Absolute index in _fullData
            SelectCell(cell.RowIndex, cell.ColumnIndex);
        }
        private void MoveEditor()
        {
            if (_editingCell == null || _editingControl == null || _editorPopupForm == null)
            {
     //           Debug.WriteLine("MoveEditor: Skipped - null reference");
                return;
            }

            // 🔹 Get the exact rectangle of the cell **after scrolling**
            Rectangle cellRect = GetCellRectangle(_editingCell);
          //  Debug.WriteLine($"MoveEditor: cellRect.Y={cellRect.Y}, _dataOffset={_dataOffset}");

            int gridLeft = gridRect.Left;
            int gridRight = gridRect.Right;
            int gridTop = gridRect.Top;
            int gridBottom = gridRect.Bottom;

            // 🔹 Adjust for vertical scrolling (subtract scroll offset)
            cellRect.Y -= (_dataOffset * RowHeight);

            // 🔹 Check if the editor should be hidden (out of grid bounds)
            bool isFullyOutOfView =
                (cellRect.Right < gridLeft) ||  // Completely off to the left
                (cellRect.Left > gridRight) ||  // Completely off to the right
                (cellRect.Bottom < gridTop) ||  // 🔹 Scrolled out past the **top** of the grid
                (cellRect.Top > gridBottom);    // Completely off below

            if (isFullyOutOfView)
            {
          //      Debug.WriteLine("MoveEditor: Editor is out of view - Hiding");
                _editorPopupForm.Visible = false;
                return;
            }

            // 🔹 Move the editor **with scrolling**
            Point screenLocation = this.PointToScreen(new Point(cellRect.X, cellRect.Y));
            _editorPopupForm.Location = screenLocation;
            _editorPopupForm.Visible = true;
        }
        private BeepGridCell GetCellAtLocation(Point location)
        {
            // First, ensure the point is inside the grid's drawing area.
            if (!gridRect.Contains(location))
                return null;

            // Compute the Y coordinate relative to gridRect.
            int yRelative = location.Y - gridRect.Top;
            int rowIndex = yRelative / RowHeight;
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return null;
            var row = Rows[rowIndex];

            // Compute the X coordinate relative to gridRect.
            // Note: We add XOffset because our content is shifted to the right by the scroll offset.
            int xRelative = location.X - gridRect.Left + XOffset;
            int currentX = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!Columns[i].Visible)
                    continue;
                if (xRelative >= currentX && xRelative < currentX + Columns[i].Width)
                {
                    // Return the cell at column i (assuming 1-to-1 correspondence).
                    return row.Cells[i];
                }
                currentX += Columns[i].Width;
            }
            return null;
        }
        private Rectangle GetCellRectangle(BeepGridCell cell)
        {
            // Find which row and column this cell belongs to.
            int rowIndex = -1, colIndex = -1;
            for (int r = 0; r < Rows.Count; r++)
            {
                int c = Rows[r].Cells.IndexOf(cell);
                if (c != -1)
                {
                    rowIndex = r;
                    colIndex = c;
                    break;
                }
            }
            if (rowIndex == -1 || colIndex == -1)
                return Rectangle.Empty;

            // Calculate Y coordinate: gridRect.Top is the start of rows.
            int y = gridRect.Top + rowIndex * RowHeight;

            // Calculate X coordinate: start at gridRect.Left and add the widths of the previous visible columns.
            int x = gridRect.Left - XOffset;
            for (int i = 0; i < colIndex; i++)
            {
                if (Columns[i].Visible)
                    x += Columns[i].Width;
            }
            int width = Columns[colIndex].Width;
            int height = RowHeight;
            return new Rectangle(x, y, width, height);
        }
        #endregion Cell Editing
        #region Scrollbar Management
        private void OnScroll(object sender, ScrollEventArgs e)
        {
             visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
             visibleRowCount = visibleHeight / RowHeight; // 🔹 Calculate dynamically

            _dataOffset = _verticalScrollBar.Value;
            Invalidate();
            //PositionControlsForVisibleCells(); // 🔹 Update visible controls dynamically
        }
        private void UpdateScrollBars()
        {
           
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            if (_fullData == null || !_fullData.Any())
            {
                _verticalScrollBar.Visible = false;
                _horizontalScrollBar.Visible = false;
                return;
            }

            int totalRowHeight = _fullData.Count * _rowHeight;
            int totalColumnWidth = Columns.Where(o=>o.Visible).Sum(col => col.Width);
            int visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
            int visibleWidth = DrawingRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);

            // **Vertical ScrollBar Fix**
            if (_showVerticalScrollBar && totalRowHeight > visibleHeight)
            {
                int maxOffset = Math.Max(0, _fullData.Count - Rows.Count);
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = maxOffset;
                _verticalScrollBar.LargeChange = Rows.Count; // Number of visible rows
                _verticalScrollBar.SmallChange = 1;
                _verticalScrollBar.Value = Math.Min(_dataOffset, maxOffset);
                _verticalScrollBar.Visible = true;
            }
            else
            {
                _verticalScrollBar.Visible = false;
            }
            // Horizontal ScrollBar
            // Horizontal ScrollBar - NEW CALCULATION
            if (_showHorizontalScrollBar && totalColumnWidth > visibleWidth)
            {
                _horizontalScrollBar.Minimum = 0;
                // Corrected Maximum: Allow scrolling until the last column's right edge aligns with visible area's right edge
                _horizontalScrollBar.Maximum = totalColumnWidth; // Full width, not reduced by visibleWidth
                _horizontalScrollBar.SmallChange = Columns.Where(c => c.Visible).Min(c => c.Width) / 2; // Smallest column half-width
                _horizontalScrollBar.LargeChange = visibleWidth; // Full visible width
                _horizontalScrollBar.Value = Math.Max(0, Math.Min(_xOffset, _horizontalScrollBar.Maximum));
                _horizontalScrollBar.Visible = true;

                // Debug output
              //  Debug.WriteLine($"Horizontal Scroll: TotalWidth={totalColumnWidth}, VisibleWidth={visibleWidth}, Max={_horizontalScrollBar.Maximum}, Value={_horizontalScrollBar.Value}");
            }
            else
            {
                _horizontalScrollBar.Visible = false;
                _xOffset = 0; // Reset offset when no scrollbar
            }

            PositionScrollBars();
        }
        private void PositionScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            int verticalScrollWidth = _verticalScrollBar.Width;
            int horizontalScrollHeight = _horizontalScrollBar.Height;
            int visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
            int visibleWidth = DrawingRect.Width;

            if (_verticalScrollBar.Visible)
            {
                _verticalScrollBar.Location = new Point(DrawingRect.Right - verticalScrollWidth, DrawingRect.Top + (ShowHeaderPanel ? headerPanelHeight : 0) + (ShowColumnHeaders ? ColumnHeaderHeight : 0));
                _verticalScrollBar.Height = visibleHeight - (_horizontalScrollBar.Visible ? horizontalScrollHeight : 0);
            }

            if (_horizontalScrollBar.Visible)
            {
                _horizontalScrollBar.Location = new Point(DrawingRect.Left, DrawingRect.Bottom - horizontalScrollHeight - (_showNavigator ? navigatorPanelHeight : 0));
                _horizontalScrollBar.Width = visibleWidth - (_verticalScrollBar.Visible ? verticalScrollWidth : 0);
            }
        }
        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (_verticalScrollBar != null)
            {
                int maxOffset = Math.Max(0, _fullData.Count - Rows.Count);
                _dataOffset = Math.Min(_verticalScrollBar.Value, maxOffset);
              
            
                FillVisibleRows(); // Fill visible rows based on new offset 
                UpdateCellPositions(); // Update cell positions based on new offset
                if (_editingCell != null)
                {
                    if (_editingRowIndex >= 0 && _editingRowIndex >= _dataOffset && _editingRowIndex < _dataOffset + Rows.Count)
                    {
                        int newRowIndex = _editingRowIndex - _dataOffset; // Relative index in Rows
                        if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                        {
                            _editingCell = Rows[newRowIndex].Cells[_editingCell.ColumnIndex]; // Assuming ColumnIndex exists
                          //  Debug.WriteLine($"Updated _editingCell to rowIndex={newRowIndex}");
                        }
                    }
                }
            

                Invalidate();
                MoveEditor(); // 🔹 Move editor if needed
            }
        }
        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            if (_horizontalScrollBar != null)
            {
                int totalColumnWidth = Columns.Where(c => c.Visible).Sum(col => col.Width);
                int visibleWidth = DrawingRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);

                // Update _xOffset based on scrollbar value, clamped to valid range
                _xOffset = Math.Max(0, Math.Min(_horizontalScrollBar.Value, totalColumnWidth - visibleWidth));

                // Debugging output
                //  Debug.WriteLine($"Scroll Event: _xOffset={_xOffset}, ScrollValue={_horizontalScrollBar.Value}, Max={_horizontalScrollBar.Maximum}");

                UpdateCellPositions(); // Update cell positions based on new offset
                Invalidate();
                MoveEditor(); // Move editor if active
            }
        }
       
        #endregion
        #region Resizing Logic
        private void BeepGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsNearColumnBorder(e.Location, out _resizingIndex))
            {
                _resizingColumn = true;
                _lastMousePos = e.Location;
                this.Cursor = Cursors.SizeWE;
            }
            else if (IsNearRowBorder(e.Location, out _resizingIndex))
            {
                _resizingRow = true;
                _lastMousePos = e.Location;
                this.Cursor = Cursors.SizeNS;
            }
        }
        private void BeepGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizingColumn && _resizingIndex >= 0)
            {
                int deltaX = e.X - _lastMousePos.X;
                if (_resizingIndex < Columns.Count)
                {
                    Columns[_resizingIndex].Width = Math.Max(20, Columns[_resizingIndex].Width + deltaX); // Prevent negative width
                }
                _lastMousePos = e.Location;
                UpdateScrollBars();
                Invalidate();
            }
            else if (_resizingRow && _resizingIndex >= 0)
            {
                int deltaY = e.Y - _lastMousePos.Y;
                _rowHeight = Math.Max(10, _rowHeight + deltaY); // Prevent negative height
                _lastMousePos = e.Location;
                InitializeRows(); // Recreate rows with new height
                UpdateScrollBars();
                Invalidate();
            }
            else
            {
                if (IsNearColumnBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (IsNearRowBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        private void BeepGrid_MouseUp(object sender, MouseEventArgs e)
        {
            _resizingColumn = false;
            _resizingRow = false;
            this.Cursor = Cursors.Default;
            FillVisibleRows(); // Ensure data is redrawn after resizing
          //  Invalidate();
        }
        private bool IsNearColumnBorder(Point location, out int columnIndex)
        {
            // Calculate the X coordinate relative to the grid's content.
            // gridRect.Left is where the cells start; add XOffset to account for horizontal scrolling.
            int xRelative = location.X - gridRect.Left + XOffset;
            int currentX = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!Columns[i].Visible)
                    continue;
                currentX += Columns[i].Width;
                // Check if the point is within _resizeMargin pixels of this column’s right edge.
                if (Math.Abs(xRelative - currentX) <= _resizeMargin)
                {
                    columnIndex = i;
                    return true;
                }
            }
            columnIndex = -1;
            return false;
        }
        private bool IsNearRowBorder(Point location, out int rowIndex)
        {
            // Calculate the Y coordinate relative to the grid's drawing area (rows start at gridRect.Top).
            int yRelative = location.Y - gridRect.Top;
            int row = yRelative / RowHeight;
            if (row < 0 || row >= Rows.Count)
            {
                rowIndex = -1;
                return false;
            }
            // The bottom edge of this row (in relative coordinates) is at (row + 1) * RowHeight.
            int rowBottom = (row + 1) * RowHeight;
            if (Math.Abs(yRelative - rowBottom) <= _resizeMargin)
            {
                rowIndex = row;
                return true;
            }
            rowIndex = -1;
            return false;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
           // InitializeRows();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }
        #endregion
        #region Editor
        // One editor control per column (keyed by column index)
        private Dictionary<string, IBeepUIComponent> _columnEditors = new Dictionary<string, IBeepUIComponent>();
        // The currently active editor and cell being edited
        private BeepControl _editingControl = null;
        private BeepGridCell _editingCell = null;
        private IBeepComponentForm _editorPopupForm;
        private void ShowCellEditor(BeepGridCell cell, Point location)
        {
            if (!cell.IsEditable)
                return;

            int colIndex = cell.ColumnIndex;
            string columnName = Columns[colIndex].ColumnName;
            if(_editingControl!=null) 

            // Close any existing editor before opening a new one
            CloseCurrentEditor();

            _editingCell = cell;
            Size cellSize = new Size(cell.Width, cell.Height);
            // Create the popup form if it's null
            if (_editorPopupForm == null)
            {
                _editorPopupForm = new IBeepComponentForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.Manual,
                    ShowInTaskbar = false,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    TopMost = true,
                    MinimumSize = cellSize, // Force exact size
                    MaximumSize = cellSize
                };

            }
            // Apply size
            _editorPopupForm.Size = cellSize;
            _editorPopupForm.ClientSize = cellSize;
            // Retrieve or create editor control
            _editingControl = CreateCellControlForEditing(cell);
                
           

            // Ensure the control is only added once
            if (_editingControl.Parent != _editorPopupForm)
            {
                _editorPopupForm.AddComponent(_editingControl);
            }
            _editingControl.Theme = Theme;
          
            // **🔹 Force the popup form and editor control to match the exact cell size**
            // _editingControl.IsFrameless = true;
            //   _editingControl.ShowAllBorders = false;
        //    Debug.WriteLine($"Cell size: {cellSize}");
            _editorPopupForm.ClientSize = cellSize; // 👈 Ensures no extra space due to window frame
            _editingControl.Size = cellSize; // 👈 Matches cell exactly
            _editingControl.Dock = DockStyle.Fill; // 👈 Ensures it doesn't resize incorrectly
          //  Debug.WriteLine($"Editor size: {_editingControl.Size}");
         //   Debug.WriteLine($"Popup size: {_editorPopupForm.ClientSize}");
            // **🔹 Position popup exactly at the cell location (relative to BeepSimpleGrid)**
            Point screenLocation = this.PointToScreen(new Point(cell.X, cell.Y));
            _editorPopupForm.Location = screenLocation;
            _editingControl.Theme = Theme;
            // **Set initial text**
            //  _editingControl.Text = cell.CellValue;
          
            UpdateCellControl(_editingControl, Columns[colIndex], cell.CellValue);
            // 🔹 Confirm value in editor **after setting**

         //   Debug.WriteLine($"🔄 Setting BeepTextBox text before popupform is Show: {_editingControl.Text}");
          //  _editorPopupForm.SetValue(cell.CellValue);
          
            _editingControl.TabKeyPressed -= Tabhandler;
             _editingControl.TabKeyPressed += Tabhandler;
            _editingControl.EscapeKeyPressed -= Canclehandler;
            _editingControl.EscapeKeyPressed += Canclehandler;
           //    _editingControl.LostFocus -= LostFocusHandler;
           //   _editingControl.LostFocus += LostFocusHandler;
            _editorPopupForm.Show();
            Task.Delay(50).ContinueWith(t =>
            {
                _editorPopupForm.SetValue(cell.CellValue);
             //   Debug.WriteLine($"✅ After popup is fully visible, setting text: {_editingControl.Text}");
            }, TaskScheduler.FromCurrentSynchronizationContext());
            _editingControl.Focus();
            IsEditorShown = true;
           // Debug.WriteLine($"✅ after popform Show the Editor BeepTextBox text   Show : {_editingControl.Text}");
        }
        private void LostFocusHandler(object? sender, EventArgs e)
        {
            CloseCurrentEditor();
        }

        private void Canclehandler(object? sender, EventArgs e)
        {
            CancelEditing();
        }
      
        private void Tabhandler(object? sender, EventArgs e)
        {
           MoveNextCell();
        }
        private void CloseCurrentEditor()
        {
            if (_editingControl != null)
            {
                // Save the current value before closing
              //  _editingCell.IsEditable = false;
           //     Debug.WriteLine($"✅ before closing Editor Text After Update: {_editingControl.Text}");

                SaveEditedValue();
             //   Debug.WriteLine($"✅ After Save Text After : {_editingControl.Text}");
                // Remove editor control from popup (prevents premature disposal)
                if (_editorPopupForm != null && _editingControl.Parent == _editorPopupForm)
                {
                    _editorPopupForm.Controls.Remove(_editingControl);
                }

                // Close and dispose of the popup form safely
                if (_editorPopupForm != null)
                {
                    _editorPopupForm.Close();
                    _editorPopupForm.Dispose();
                    _editorPopupForm = null;
                }

                // Reset references (but don't dispose `_editingControl` yet)
                _editingCell = null;
                 _editingControl = null;

             //   Debug.WriteLine("Popup editor closed successfully.");
            }
            IsEditorShown = false;
        }
   
        private void SaveEditedValue()
        {
            if (_editorPopupForm == null || _editingCell == null)
            {
           //     Debug.WriteLine($"⚠️ Editing control or cell is null!");
                return;
            }

            object newValue = _editorPopupForm.GetValue();
          //  Debug.WriteLine($"🔄 Saving value: {newValue} (Old: {_editingCell.CellData})");

            // 🔹 Check if the new value is empty or null
            if (newValue == null || string.IsNullOrWhiteSpace(newValue.ToString()))
            {
             //   Debug.WriteLine($"⚠️ New value is empty. Skipping update.");
                return;
            }

            // 🔹 Retrieve PropertyType from the corresponding column
            BeepGridColumnConfig columnConfig = Columns.FirstOrDefault(c => c.Index == _editingCell.ColumnIndex);
            if (columnConfig == null)
            {
            //    Debug.WriteLine($"⚠️ Column config not found. Skipping update.");
                return;
            }
            Type propertyType = Type.GetType(columnConfig.PropertyTypeName, throwOnError: false) ?? typeof(string); // Default to string if null

             //🔹 Convert new value to the correct type before comparing
            object convertedNewValue = MiscFunctions.ConvertValueToPropertyType(propertyType, newValue);
            object convertedOldValue = MiscFunctions.ConvertValueToPropertyType(propertyType, _editingCell.CellData);

            // 🔹 Skip update if the new value is the same as the old value
            if (convertedNewValue != null && convertedOldValue != null && convertedNewValue.Equals(convertedOldValue))
            {
            //    Debug.WriteLine($"⚠️ New value is the same as old. Skipping update.");
                return;
            }

            // 🔹 Update cell's stored value
            _editingCell.OldValue = _editingCell.CellData;
            _editingCell.CellData =  convertedNewValue;
            _editingCell.CellValue = convertedNewValue;
            _editingCell.IsDirty = true;

            // 🔹 Update the corresponding data record
            UpdateDataRecordFromRow(_editingCell);

            // 🔹 Trigger validation if necessary
            _editingCell.ValidateCell();

           // Debug.WriteLine($"✅ Cell updated. New: {_editingCell.CellValue}");

            Invalidate(); // 🔹 Redraw grid if necessary
        }
        private void CancelEditing()
        {
            // Optionally, revert the editor's value if needed.
            CloseCurrentEditor();
        }
        private void BeepGrid_MouseClick(object sender, MouseEventArgs e)
        {
            var clickedCell = GetCellAtLocation(e.Location);
            if (_editingCell != null && clickedCell != null &&
                _editingCell.Id == clickedCell.Id)  // Compare by unique Id
            {
                // The same cell was clicked—save its current value
                SaveEditedValue();
                return;
            }
            else
            {
            //    if (_editingCell != null) _editingCell.IsSelected = false;
                CloseCurrentEditor();
                // reset the cell selection
                if (clickedCell != null )
                {
                    _editingCell = clickedCell;
                    _selectedCell = clickedCell;
                    SelectCell(_selectedCell);
                   
                    if (!_columns[_selectedCell.ColumnIndex].ReadOnly)
                    {
                        ShowCellEditor(_selectedCell, e.Location);
                    }
                  
                }
               
            }
        }
      
        #endregion Editor
        #region Events
        private void Rows_ListChanged(object sender, ListChangedEventArgs e) => Invalidate();
        #endregion
        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            this.BackColor = _currentTheme.GridBackColor;
            this.ForeColor = _currentTheme.GridForeColor;
            if (titleLabel != null)
            {
                titleLabel.Theme = Theme;
            }
            if (DataNavigator != null)
            {
                _currentTheme.ButtonBackColor = _currentTheme.GridBackColor;
                _currentTheme.ButtonForeColor = _currentTheme.GridForeColor;
                DataNavigator.Theme = Theme;
            }
            if (_verticalScrollBar != null)
            {
                _verticalScrollBar.Theme = Theme;
            }
            if (_horizontalScrollBar != null)
            {
                _horizontalScrollBar.Theme = Theme;
            }
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (cell.UIComponent is BeepControl ctrl)
                    {
                        ctrl.Theme = Theme;
                    }
                }
            }
            Invalidate();
        }
        #endregion
        #region Helper Methods
        public BeepGridColumnConfig GetColumnByName(string columnName)
        {
            // Find index first
            int index = Columns.FindIndex(c => c.ColumnName.Equals(columnName,StringComparison.InvariantCultureIgnoreCase));
            if (index == -1) return null;
            return Columns[index];
            
        }
        public Dictionary<string,BeepGridColumnConfig> GetDictionaryColumns()
        {
            Dictionary<string, BeepGridColumnConfig> dict = new Dictionary<string, BeepGridColumnConfig>();
            foreach (var col in Columns)
            {
                dict.Add(col.ColumnName, col);
            }
            return dict;
        }
        public BeepGridColumnConfig GetColumnByIndex(int index)
        {  // Find index first
            int retindex = Columns.FindIndex(c => c.Index == index);
            if (retindex == -1) return null;
            return Columns[retindex];
        }
        public BeepGridColumnConfig GetColumnByCaption(string caption)
        {
            int index = Columns.FindIndex(c => c.ColumnName.Equals(caption, StringComparison.InvariantCultureIgnoreCase));
            if (index == -1) return null;

            return Columns[index];
        }
        private DbFieldCategory MapPropertyTypeToDbFieldCategory(Type type)
        {
            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(Char)) return DbFieldCategory.Char;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            // Default to string for unknown types
            return DbFieldCategory.String;
        }

        private BeepGridColumnType MapPropertyTypeToCellEditor(Type type)
        {
            if (type == typeof(string)) return BeepGridColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepGridColumnType.NumericUpDown; // Integer-based controls
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepGridColumnType.Text; // Use a formatted text field for decimal numbers
            if (type == typeof(DateTime)) return BeepGridColumnType.DateTime;
            if (type == typeof(bool)) return BeepGridColumnType.CheckBoxBool;
            if (type == typeof(Char)) return BeepGridColumnType.CheckBoxChar;
            if (type == typeof(Guid)) return BeepGridColumnType.Text; // Could be a readonly label or a dropdown if referencing something

            // Fallback to text if the type is unknown
            return BeepGridColumnType.Text;
        }
        private DbFieldCategory MapPropertyTypeToDbFieldCategory(string propertyTypeName)
        {
            // Default to String if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
            //    Debug.WriteLine("MapPropertyTypeToDbFieldCategory: propertyTypeName is null or empty, defaulting to String");
                return DbFieldCategory.String;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
             //   Debug.WriteLine($"MapPropertyTypeToDbFieldCategory: Could not resolve type '{propertyTypeName}', defaulting to String");
                return DbFieldCategory.String;
            }

            // Map resolved Type to DbFieldCategory
            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(Char)) return DbFieldCategory.Char;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            // Default to String for unknown types
          //  Debug.WriteLine($"MapPropertyTypeToDbFieldCategory: Unknown type '{type.FullName}', defaulting to String");
            return DbFieldCategory.String;
        }
        private BeepGridColumnType MapPropertyTypeToCellEditor(string propertyTypeName)
        {
            // Default to Text if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
            //    Debug.WriteLine("MapPropertyTypeToCellEditor: propertyTypeName is null or empty, defaulting to Text");
                return BeepGridColumnType.Text;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
              //  Debug.WriteLine($"MapPropertyTypeToCellEditor: Could not resolve type '{propertyTypeName}', defaulting to Text");
                return BeepGridColumnType.Text;
            }

            // Map resolved Type to BeepGridColumnType
            if (type == typeof(string)) return BeepGridColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepGridColumnType.NumericUpDown;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepGridColumnType.Text; // Use formatted text for decimals
            if (type == typeof(DateTime)) return BeepGridColumnType.DateTime;
            if (type == typeof(bool)) return BeepGridColumnType.CheckBoxBool;
            if (type == typeof(Char)) return BeepGridColumnType.CheckBoxChar;
            if (type == typeof(Guid)) return BeepGridColumnType.Text;

            // Fallback to Text for unknown types
          //  Debug.WriteLine($"MapPropertyTypeToCellEditor: Unknown type '{type.FullName}', defaulting to Text");
            return BeepGridColumnType.Text;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_verticalScrollBar != null)
                {
                    _verticalScrollBar.Dispose();
                    _verticalScrollBar = null;
                }
                if (_horizontalScrollBar != null)
                {
                    _horizontalScrollBar.Dispose();
                    _horizontalScrollBar = null;
                }
                if(_editingControl != null)
                {
                    _editingControl.Dispose();
                    _editingControl = null;
                }
                if (_editorPopupForm != null)
                {
                    _editorPopupForm.Dispose();
                    _editorPopupForm = null;
                }
                if (titleLabel != null)
                {
                    titleLabel.Dispose();
                    titleLabel = null;
                }
                if (DataNavigator != null)
                {
                    DataNavigator.Dispose();
                    DataNavigator = null;
                }
                if (Rows != null)
                {
                    Rows.ListChanged -= Rows_ListChanged;
                    Rows.Clear();
                    Rows = null;
                }
                if (Columns != null)
                {
                    Columns.Clear();
                    Columns = null;
                }
            }
        }
    }
}