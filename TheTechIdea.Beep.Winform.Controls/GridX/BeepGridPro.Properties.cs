using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using Math = System.Math;
using TheTechIdea.Beep.Winform.Controls.GridX.Selection;
using navigationStyle = TheTechIdea.Beep.Winform.Controls.GridX.Painters.navigationStyle;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.UOW;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro : BaseControl
    {
        // Data management fields
        internal Type _entityType = null;
        internal List<object> _fullData
        {
            get => DataController.FullData;
            set => DataController.FullData = value ?? new();
        }
        internal int _dataOffset
        {
            get => DataController.DataOffset;
            set => DataController.DataOffset = value;
        }
        private object? _uow;
        private IUnitofWork? _typedUow;
        private IUnitOfWorkWrapper? _uowWrapper;
        private object? _regularDataSource;
        private System.Windows.Forms.Timer? _autoSizeDebounceTimer;
        private AutoSizeTriggerMode _autoSizeTriggerMode = AutoSizeTriggerMode.OnDataBind;
        private int _autoSizeDebounceMilliseconds = 120;
        private string _gridTitle = "Grid";
        private bool _useInlineQuickSearch = true;

        [Browsable(false)]
        internal Type EntityType => _entityType;
        internal void SetEntityType(Type entityType) => _entityType = entityType;

        [Browsable(true)]
        [Category("Data")]
        [Description("Assign an IUnitofWork<T> instance. When set, its Units will be used as the grid data source and kept in sync.")]
        [TypeConverter(typeof(UnitOfWorksConverter))]
        public object? Uow
        {
            get => _uow;
            set
            {
                if (!ReferenceEquals(_uow, value))
                {
                    _uow = value;
                    _typedUow = value as IUnitofWork;
                    _uowWrapper = value as IUnitOfWorkWrapper;

                  

                    Navigator.SetUnitOfWork(_typedUow, _uowWrapper);
                    _uowBinder?.Attach(_typedUow, _uowWrapper);

                    // When UOW mode is disabled, return to regular DataSource mode.
                    if (_typedUow == null && _uowWrapper == null)
                    {
                        DataController.BindComplete(_regularDataSource);
                        if (!DesignMode) SafeInvalidate();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        public object? DataSource
        {
            get => _regularDataSource ?? Data.DataSource;
            set
            {
                // Store old value to detect changes
                object? oldValue = _regularDataSource;
                _regularDataSource = value;

                // UOW mode is authoritative. Keep regular DataSource value for fallback.
                if (_typedUow != null || _uowWrapper != null)
                {
                    return;
                }

                // Always rebind when value changes (including null)
                bool valueChanged = !ReferenceEquals(oldValue, value);
                bool isNull = value == null;

                // Rebind if: value changed, or setting to null, or BindingSource scenario
                if (valueChanged || isNull || value is BindingSource)
                {
                    if (isNull)
                    {
                        // Clear the grid when DataSource is set to null
                        ClearGrid();
                    }
                    else
                    {
                        DataController.BindVirtualized(value);
                        if (EnableVirtualization)
                        {
                            Data.Rows.Clear();
                            var columnNames = Data.Columns.Select(c => c.ColumnName).ToList();
                            VirtualDataSource = CreateVirtualDataSource(value, columnNames);
                            int viewportHeight = Math.Max(1, Layout.RowsRect.Height > 0 ? Layout.RowsRect.Height : Height);
                            RowVirtualizer.UpdateWindow(Scroll.VerticalOffset, viewportHeight, RowHeight);
                            Layout.Recalculate();
                        }
                    }
                    SafeInvalidate();
                }
            }
        }

        /// <summary>
        /// Clears all data and non-system columns from the grid.
        /// </summary>
        public void ClearGrid()
        {
            Data.Rows.Clear();

            if (EnableVirtualization)
            {
                VirtualDataSource = null;
            }

            var systemColumns = Data.Columns.Where(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID).ToList();
            Data.Columns.Clear();
            foreach (var col in systemColumns)
            {
                Data.Columns.Add(col);
            }

            // Clear data and navigator
            DataController.BindToNull();

            // Clear selection
            Selection?.Clear();

            SafeInvalidate();
        }

        /// <summary>
        /// Refreshes the grid from its current DataSource.
        /// </summary>
        public void RefreshData()
        {
            if (_regularDataSource != null)
            {
                if (EnableVirtualization)
                {
                    DataController.SyncFullData();
                    RowVirtualizer.Refresh();
                }
                else
                {
                    DataController.BindDataOnly(_regularDataSource);
                }
                Layout.Recalculate();
                SafeInvalidate();
            }
        }

        private string _dataMember = string.Empty;
        [Browsable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [TypeConverter(typeof(BeepDataMemberConverter))]
        public string DataMember
        {
            get => _dataMember;
            set
            {
                if (_dataMember != value)
                {
                    _dataMember = value ?? string.Empty;
                    if (Data.DataSource != null)
                    {
                        DataController.BindComplete(Data.DataSource);
                        SafeInvalidate();
                    }
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        public BeepGridColumnConfigCollection Columns => Data.Columns;

        [Browsable(false)]
        public BindingList<BeepRowConfig> Rows => Data.Rows;

        [Browsable(false)]
        [Description("The number of rows that can fit in the current visible area (dynamic page size for pagination)")]
        public int VisibleRowCapacity => Render?.GetVisibleRowCapacity() ?? 10;

        // ===== Layout & Appearance & Behavior properties =====
        [Browsable(true)]
        [Category("Layout")]
        public int RowHeight
        {
            get => Layout.RowHeight;
            set
            {
                int min = BeepLayoutMetrics.MinTouchTarget.ScaleValue(this);
                if (Layout.RowHeight != System.Math.Max(min, value))
                {
                    Layout.RowHeight = System.Math.Max(min, value);
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public int ColumnHeaderHeight
        {
            get => Layout.ColumnHeaderHeight;
            set
            {
                int min = BeepLayoutMetrics.MinTouchTarget.ScaleValue(this);
                if (Layout.ColumnHeaderHeight != System.Math.Max(min, value))
                {
                    Layout.ColumnHeaderHeight = System.Math.Max(min, value);
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public bool ShowColumnHeaders
        {
            get => Layout.ShowColumnHeaders;
            set
            {
                if (Layout.ShowColumnHeaders != value)
                {
                    Layout.ShowColumnHeaders = value;
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Shows a modern filter panel above column headers for per-column filtering. Mutually exclusive with ShowToolbar.")]
        public bool ShowTopFilterPanel
        {
            get => Layout.ShowTopFilterPanel;
            set
            {
                if (Layout.ShowTopFilterPanel != value)
                {
                    Layout.ShowTopFilterPanel = value;
                    // Mutual exclusion with the unified toolbar (Pass 18).
                    if (value && _showToolbar) _showToolbar = false;
                    Layout.Recalculate();
                    if (!value)
                    {
                        HideInlineQuickSearch();
                    }
                    else
                    {
                        EnsureInlineQuickSearchVisible();
                    }
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Uses an inline BeepFilter QuickSearch control instead of the painted search box in the top filter panel.")]
        public bool UseInlineQuickSearch
        {
            get => _useInlineQuickSearch;
            set
            {
                if (_useInlineQuickSearch != value)
                {
                    _useInlineQuickSearch = value;
                    if (_useInlineQuickSearch)
                    {
                        EnsureInlineQuickSearchVisible();
                    }
                    else
                    {
                        HideInlineQuickSearch();
                    }
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue(34)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Height in pixels of the top filter panel rendered above column headers.")]
        public int TopFilterPanelHeight
        {
            get => Layout.TopFilterHeight;
            set
            {
                int clamped = System.Math.Max(BeepLayoutMetrics.MinTouchTarget.ScaleValue(this), value);
                if (Layout.TopFilterHeight != clamped)
                {
                    Layout.TopFilterHeight = clamped;
                    Layout.Recalculate();
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        // Owner-drawn navigator footer like BeepSimpleGrid (not a child control)
        private bool _showNavigator = true;
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(true)]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set
            {
                if (_showNavigator != value)
                {
                    _showNavigator = value;
                    Layout.Recalculate();
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        private BeepGridStyle _gridStyle = BeepGridStyle.Bootstrap;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style preset for the grid, inspired by popular JavaScript frameworks.")]
        [DefaultValue(BeepGridStyle.Default)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public BeepGridStyle GridStyle
        {
            get => _gridStyle;
            set
            {
                if (_gridStyle != value)
                {
                    _gridStyle = value;
                    ApplyGridStyle();
                    SafeInvalidate();
                    Refresh();
                    if (DesignMode && Site != null)
                    {
                        var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                        changeService?.OnComponentChanged(this, null, null, null);
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue("Grid")]
        [Description("Title displayed in the toolbar.")]
        public string GridTitle
        {
            get => _gridTitle;
            set
            {
                var next = value ?? string.Empty;
                if (!string.Equals(_gridTitle, next, System.StringComparison.Ordinal))
                {
                    _gridTitle = next;
                    // Sync to toolbar state
                    if (_toolbarState != null)
                        _toolbarState.GridTitle = next;
                    if (!DesignMode) SafeInvalidate();
                }
            }
        }

        private bool _showGridTitle = true;
        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue(true)]
        [Description("Shows or hides the grid title in the unified toolbar.")]
        public bool ShowGridTitle
        {
            get => _showGridTitle;
            set
            {
                if (_showGridTitle == value) return;
                _showGridTitle = value;
                if (_toolbarState != null) _toolbarState.ShowGridTitle = value;
                if (!DesignMode)
                {
                    if (Layout != null && !Layout.ToolbarRect.IsEmpty) SafeInvalidate(Layout.ToolbarRect);
                    else SafeInvalidate();
                }
            }
        }

        private string _searchPlaceholder = "Search...";
        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue("Search...")]
        [Description("Placeholder text shown in the toolbar search box when empty.")]
        public string SearchPlaceholder
        {
            get => _searchPlaceholder;
            set
            {
                var next = value ?? string.Empty;
                if (!string.Equals(_searchPlaceholder, next, System.StringComparison.Ordinal)) { _searchPlaceholder = next; if (!DesignMode) SafeInvalidate(); }
            }
        }

        private bool _showFilterButton = false;
        /// <summary>
        /// Show the standalone quick Filter button in the toolbar
        /// (in addition to the Advanced multi-criteria button).  Off
        /// by default; the Advanced button covers all multi-criteria
        /// cases, and the per-column filter icons in the column
        /// headers cover the single-column case.  Set true if you
        /// want the funnel icon that opens the quick column=value
        /// dialog.
        /// </summary>
        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue(false)]
        [Description("Show the standalone quick Filter button in the toolbar (the Advanced multi-criteria button is always shown).")]
        public bool ShowFilterButton
        {
            get => _showFilterButton;
            set
            {
                if (_showFilterButton == value) return;
                _showFilterButton = value;
                if (_toolbarState != null) _toolbarState.ShowFilterButton = value;
                if (Layout != null) Layout.Recalculate();
                if (!DesignMode) SafeInvalidate();
            }
        }

        private navigationStyle _navigationStyle = navigationStyle.Standard;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Navigation bar Style - choose from 12 framework-inspired designs")]
        [DefaultValue(navigationStyle.Standard)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public navigationStyle NavigationStyle
        {
            get => _navigationStyle;
            set
            {
                if (_navigationStyle != value)
                {
                    _navigationStyle = value;
                    NavigatorPainter.NavigationStyle = value;
                    if (_usePainterNavigation)
                    {
                        if (value == navigationStyle.None) Layout.NavigatorHeight = 0; else Layout.NavigatorHeight = NavigatorPainter.GetRecommendedNavigatorHeight();
                        Layout.Recalculate();
                    }
                    SafeInvalidate();
                    Refresh();
                    if (DesignMode && Site != null)
                    {
                        var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                        changeService?.OnComponentChanged(this, null, null, null);
                    }
                }
            }
        }

        private bool _usePainterNavigation = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use modern painter-based navigation (true) or legacy button-based navigation (false)")]
        [DefaultValue(true)]
        public bool UsePainterNavigation
        {
            get => _usePainterNavigation;
            set
            {
                if (_usePainterNavigation != value)
                {
                    _usePainterNavigation = value;
                    NavigatorPainter.UsePainterNavigation = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(HeaderIconVisibility.Always)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Controls sort icon visibility in column headers: Always, HoverOnly, or Hidden.")]
        public HeaderIconVisibility SortIconVisibility
        {
            get => Render.SortIconVisibility;
            set
            {
                if (Render.SortIconVisibility != value)
                {
                    Render.SortIconVisibility = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(HeaderIconVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Controls filter icon visibility in column headers: Always, HoverOnly, or Hidden. Icons are hidden while ShowTopFilterPanel is enabled.")]
        public HeaderIconVisibility FilterIconVisibility
        {
            get => Render.FilterIconVisibility;
            set
            {
                if (Render.FilterIconVisibility != value)
                {
                    Render.FilterIconVisibility = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Use a dedicated focused-row style instead of hover style for the active row.")]
        public bool UseDedicatedFocusedRowStyle
        {
            get => Render.UseDedicatedFocusedRowStyle;
            set
            {
                if (Render.UseDedicatedFocusedRowStyle != value)
                {
                    Render.UseDedicatedFocusedRowStyle = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Override focused-row background color. Leave empty to derive from theme focus color.")]
        public Color FocusedRowBackColor
        {
            get => Render.FocusedRowBackColor;
            set
            {
                if (Render.FocusedRowBackColor != value)
                {
                    Render.FocusedRowBackColor = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Draw fill overlay on the focused cell.")]
        public bool ShowFocusedCellFill
        {
            get => Render.ShowFocusedCellFill;
            set
            {
                if (Render.ShowFocusedCellFill != value)
                {
                    Render.ShowFocusedCellFill = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Override focused-cell fill color. Leave empty to use theme focus color.")]
        public Color FocusedCellFillColor
        {
            get => Render.FocusedCellFillColor;
            set
            {
                if (Render.FocusedCellFillColor != value)
                {
                    Render.FocusedCellFillColor = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(36)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Alpha opacity (0-255) used for focused-cell fill overlay.")]
        public int FocusedCellFillOpacity
        {
            get => Render.FocusedCellFillOpacity;
            set
            {
                int clamped = System.Math.Max(0, System.Math.Min(255, value));
                if (Render.FocusedCellFillOpacity != clamped)
                {
                    Render.FocusedCellFillOpacity = clamped;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Draw border outline around focused cell.")]
        public bool ShowFocusedCellBorder
        {
            get => Render.ShowFocusedCellBorder;
            set
            {
                if (Render.ShowFocusedCellBorder != value)
                {
                    Render.ShowFocusedCellBorder = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Override focused-cell border color. Leave empty to use theme focus indicator color.")]
        public Color FocusedCellBorderColor
        {
            get => Render.FocusedCellBorderColor;
            set
            {
                if (Render.FocusedCellBorderColor != value)
                {
                    Render.FocusedCellBorderColor = value;
                    SafeInvalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(2f)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Focused-cell border width in pixels.")]
        public float FocusedCellBorderWidth
        {
            get => Render.FocusedCellBorderWidth;
            set
            {
                float clamped = System.Math.Max(0f, value);
                if (System.Math.Abs(Render.FocusedCellBorderWidth - clamped) > 0.001f)
                {
                    Render.FocusedCellBorderWidth = clamped;
                    SafeInvalidate();
                }
            }
        }

        private GridLayoutPreset _layoutPreset = GridLayoutPreset.Default;
        [Browsable(true)]
        [Category("Layout")]
        [Description("Structural layout preset (spacing, stripes, header effects). Not colors.")]
        [DefaultValue(GridLayoutPreset.Default)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public GridLayoutPreset LayoutPreset
        {
            get => _layoutPreset;
            set
            {
                if (_layoutPreset == value) return;
                _layoutPreset = value;
                ApplyLayoutPreset(value);
                SafeInvalidate();
                Refresh();
                if (DesignMode && Site != null)
                {
                    var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    changeService?.OnComponentChanged(this, null, null, null);
                }
            }
        }

        public void ApplyLayoutPreset(GridLayoutPreset preset)
        {
            IGridLayoutPreset impl = preset switch
            {
                GridLayoutPreset.Clean => new CleanTableLayoutHelper(),
                GridLayoutPreset.Dense => new DenseTableLayoutHelper(),
                GridLayoutPreset.Striped => new StripedTableLayoutHelper(),
                GridLayoutPreset.Borderless => new BorderlessTableLayoutHelper(),
                GridLayoutPreset.HeaderBold => new HeaderBoldTableLayoutHelper(),
                GridLayoutPreset.MaterialHeader => new MaterialHeaderTableLayoutHelper(),
                GridLayoutPreset.Card => new CardTableLayoutHelper(),
                GridLayoutPreset.ComparisonTable => new ComparisonTableLayoutHelper(),
                GridLayoutPreset.MatrixSimple => new MatrixSimpleTableLayoutHelper(),
                GridLayoutPreset.MatrixStriped => new MatrixStripedTableLayoutHelper(),
                GridLayoutPreset.PricingTable => new PricingTableLayoutHelper(),
                GridLayoutPreset.Material3Surface => new Material3SurfaceTableLayoutHelper(),
                GridLayoutPreset.Material3Compact => new Material3CompactTableLayoutHelper(),
                GridLayoutPreset.Material3List => new Material3ListTableLayoutHelper(),
                GridLayoutPreset.Fluent2Standard => new Fluent2StandardTableLayoutHelper(),
                GridLayoutPreset.Fluent2Card => new Fluent2CardTableLayoutHelper(),
                GridLayoutPreset.TailwindProse => new TailwindProseTableLayoutHelper(),
                GridLayoutPreset.TailwindDashboard => new TailwindDashboardTableLayoutHelper(),
                GridLayoutPreset.AGGridAlpine => new AGGridAlpineTableLayoutHelper(),
                GridLayoutPreset.AGGridBalham => new AGGridBalhamTableLayoutHelper(),
                GridLayoutPreset.AntDesignStandard => new AntDesignStandardTableLayoutHelper(),
                GridLayoutPreset.AntDesignCompact => new AntDesignCompactTableLayoutHelper(),
                GridLayoutPreset.DataTablesStandard => new DataTablesStandardTableLayoutHelper(),
                _ => new DefaultTableLayoutHelper()
            };
            this.ApplyLayoutPreset(impl);
        }

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeColumns { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeRows { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow reordering columns by dragging headers.")]
        public bool AllowColumnReorder { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow selecting multiple rows via checkbox column. If false, only one row can be selected at a time.")]
        public bool MultiSelect { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get => Data.Columns.Any(c => c.IsSelectionCheckBox && c.Visible);
            set
            {
                if (ShowCheckBox != value)
                {
                    Data.EnsureSystemColumns();
                    var selColumn = Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
                    if (selColumn != null)
                    {
                        selColumn.Visible = value;
                        Layout.Recalculate();
                        if (!DesignMode) SafeInvalidate();
                    }
                }
            }
        }

        private BeepColumnConfig _selectioncheckboxColumn;
        public BeepColumnConfig SelectionCheckBoxColumn
        {
            get
            {
                if (_selectioncheckboxColumn == null)
                {
                    _selectioncheckboxColumn = Data.Columns.FirstOrDefault(r => r.IsSelectionCheckBox)!;
                    if (_selectioncheckboxColumn == null)
                    {
                        Data.EnsureSystemColumns();
                        _selectioncheckboxColumn = Data.Columns.FirstOrDefault(r => r.IsSelectionCheckBox)!;
                    }
                }
                return _selectioncheckboxColumn!;
            }
        }

        private BeepColumnConfig _rowIDColumn;
        public BeepColumnConfig RowIDColumn
        {
            get
            {
                if (_rowIDColumn == null)
                {
                    _rowIDColumn = Data.Columns.FirstOrDefault(c => c.IsRowID)!;
                    if (_rowIDColumn == null)
                    {
                        Data.EnsureSystemColumns();
                        _rowIDColumn = Data.Columns.FirstOrDefault(c => c.IsRowID)!;
                    }
                }
                return _rowIDColumn!;
            }
        }

        private BeepColumnConfig _rowNumberColumn;
        public BeepColumnConfig RowNumberColumn
        {
            get
            {
                if (_rowNumberColumn == null)
                {
                    _rowNumberColumn = Data.Columns.FirstOrDefault(c => c.IsRowNumColumn)!;
                    if (_rowNumberColumn == null)
                    {
                        Data.EnsureSystemColumns();
                        _rowNumberColumn = Data.Columns.FirstOrDefault(c => c.IsRowNumColumn)!;
                    }
                }
                return _rowNumberColumn!;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(DataGridViewAutoSizeColumnsMode.None)]
        public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode { get; set; } = DataGridViewAutoSizeColumnsMode.None;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(AutoSizeTriggerMode.OnDataBind)]
        [Description("Controls when auto-size is triggered: Manual, OnDataBind, OnEditCommit, OnSortFilter, AlwaysDebounced.")]
        public AutoSizeTriggerMode AutoSizeTriggerMode
        {
            get => _autoSizeTriggerMode;
            set => _autoSizeTriggerMode = value;
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(120)]
        [Description("Debounce interval in milliseconds used by AlwaysDebounced auto-size mode.")]
        public int AutoSizeDebounceMilliseconds
        {
            get => _autoSizeDebounceMilliseconds;
            set => _autoSizeDebounceMilliseconds = System.Math.Max(16, value);
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        public bool AutoSizeRowsToContent { get; set; } = false;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(2)]
        [Description("Padding added to auto-sized row heights (in pixels)")]
        public int RowAutoSizePadding { get; set; } = BeepLayoutMetrics.GridCellPadding;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(true)]
        [Description("Whether to use DPI-aware scaling for row height calculations")]
        public bool UseDpiAwareRowHeights { get; set; } = true;
        /// <summary>
        /// Gets or sets the accessible name for the grid control
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Name of the control used by accessibility client applications")]
        public new string AccessibleName
        {
            get => base.AccessibleName;
            set
            {
                base.AccessibleName = value;
                SafeInvalidate();
            }
        }

        /// <summary>
        /// Gets or sets the accessible description for the grid control
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Description of the control used by accessibility client applications")]
        public new string AccessibleDescription
        {
            get => base.AccessibleDescription;
            set
            {
                base.AccessibleDescription = value;
                SafeInvalidate();
            }
        }

        #region Selection Properties

        private BeepGridSelectionMode _selectionMode = BeepGridSelectionMode.FullRowSelect;

        /// <summary>
        /// Gets or sets the selection behaviour of the grid.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Determines how rows and cells can be selected.")]
        [DefaultValue(BeepGridSelectionMode.FullRowSelect)]
        public BeepGridSelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                if (_selectionMode != value)
                {
                    _selectionMode = value;
                    Selection.Strategy = value switch
                    {
                        BeepGridSelectionMode.CellSelect => CellSelectionStrategy.Instance,
                        BeepGridSelectionMode.FullRowSelect => RowSelectionStrategy.Instance,
                        BeepGridSelectionMode.FullColumnSelect => ColumnSelectionStrategy.Instance,
                        _ => RowSelectionStrategy.Instance
                    };
                    Selection.ClearSelection();
                    SafeInvalidate();
                }
            }
        }

        /// <summary>
        /// Gets a wrapper around the currently-selected row, or <see langword="null"/> if
        /// no row is selected.  Supports <c>CurrentRow?.Cells["col"]?.Value</c> access.
        /// </summary>
        [Browsable(false)]
        public BeepGridCurrentRow? CurrentRow
        {
            get
            {
                int idx = Selection.RowIndex;
                if (idx < 0 || idx >= Data.Rows.Count) return null;
                return new BeepGridCurrentRow(Data.Rows[idx]) { Index = idx };
            }
        }

        /// <summary>
        /// Gets the zero-based index of the currently-selected row, or -1 when
        /// nothing is selected.
        /// </summary>
        [Browsable(false)]
        public int CurrentRowIndex => Selection.RowIndex;

        #endregion

        #region Toolbar Properties

        private bool _showToolbar = true;
        private readonly Toolbar.BeepGridToolbarState _toolbarState = new();
        private Toolbar.BeepGridToolbarPainter _toolbarPainter;
        private Filtering.FilterEditorHelper _filterEditor;
        private System.Windows.Forms.ToolTip? _toolbarTooltip;
        private string _lastTooltipText = string.Empty;
        private string _lastTooltipKey = string.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Shows or hides the unified toolbar (actions + search + filter + export).")]
        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get => _showToolbar;
            set
            {
                if (_showToolbar != value)
                {
                    _showToolbar = value;
                    // Consolidation: toolbar and top filter panel are mutually exclusive.
                    // When toolbar is shown, hide the legacy filter panel (and its inline quick search).
                    if (value && Layout.ShowTopFilterPanel)
                    {
                        Layout.ShowTopFilterPanel = false;
                        HideInlineQuickSearch();
                    }
                    // When toolbar is hidden, optionally show the legacy filter panel.
                    if (!value && !Layout.ShowTopFilterPanel)
                    {
                        Layout.ShowTopFilterPanel = true;
                        EnsureInlineQuickSearchVisible();
                    }
                    // Tear down the search editor when the toolbar disappears
                    // so the editor (a child control) doesn't stay visible
                    // after its painted host is gone.
                    if (!value) _filterEditor?.HideSearchEditor();
                    Layout.Recalculate();
                    SafeInvalidate();
                }
            }
        }

        [Browsable(false)]
        public Toolbar.BeepGridToolbarState ToolbarState => _toolbarState;

        /// <summary>
        /// True when an in-place cell editor is currently active.
        /// Hosts can use this to disable toolbar buttons (e.g.
        /// don't let the user add/delete while an edit is in
        /// progress) or to update a status bar with "Editing..."
        /// text.  Mirrors <see cref="DataGridView.IsCurrentCellInEditMode"/>.
        /// </summary>
        [Browsable(false)]
        public bool IsEditing => Edit?.IsEditing ?? false;

        [Browsable(false)]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Toolbar.BeepGridToolbarPainter ToolbarPainter => _toolbarPainter;

        [Browsable(false)]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal Filtering.FilterEditorHelper FilterEditor => _filterEditor;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Background color of the toolbar. Color.Empty = use theme BackgroundColor.")]
        public Color ToolbarBackColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Foreground color of toolbar icons and text. Color.Empty = use theme ForeColor.")]
        public Color ToolbarForeColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Placeholder text color in the search box. Color.Empty = use theme DisabledForeColor.")]
        public Color ToolbarPlaceholderColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Background color of the search box. Color.Empty = use theme BackgroundColor.")]
        public Color ToolbarSearchBackColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Background color of the search box when focused. Color.Empty = use theme SelectedBackgroundColor.")]
        public Color ToolbarSearchFocusBackColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Border color of the search box. Color.Empty = use theme BorderColor.")]
        public Color ToolbarBorderColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Background color of toolbar buttons on hover. Color.Empty = use theme HoverBackgroundColor.")]
        public Color ToolbarButtonHoverBackColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Background color of toolbar buttons when pressed. Color.Empty = use theme PressedBackgroundColor.")]
        public Color ToolbarButtonPressedBackColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "")]
        [Description("Color of separator lines between toolbar sections. Color.Empty = use theme BorderColor.")]
        public Color ToolbarSeparatorColor { get; set; } = Color.Empty;

        private bool _showToolbarTooltips = true;
        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue(true)]
        [Description("Show a tooltip when hovering a toolbar button.")]
        public bool ShowToolbarTooltips
        {
            get => _showToolbarTooltips;
            set
            {
                if (_showToolbarTooltips == value) return;
                _showToolbarTooltips = value;
                if (_toolbarTooltip != null)
                {
                    if (!value) _toolbarTooltip.SetToolTip(this, string.Empty);
                }
            }
        }

        /// <summary>
        /// The WinForms <see cref="ToolTip"/> used to surface toolbar
        /// button tooltips.  Exposed so hosts can customize the
        /// appearance (balloon, colour, etc.) at runtime.
        /// </summary>
        [Browsable(false)]
        public System.Windows.Forms.ToolTip ToolbarTooltip => _toolbarTooltip
            ?? throw new System.InvalidOperationException("ToolbarTooltip is not yet initialized (called before the BeepGridPro constructor).");

        /// <summary>
        /// Show or hide a single toolbar button by key (e.g. "add", "edit",
        /// "delete", "import", "export", "print", "clearfilter", "overflow").
        /// Hidden buttons are still part of overflow detection so they
        /// participate in the layout calculation.  No-op if the key is unknown.
        /// </summary>
        public void SetToolbarButtonVisible(string key, bool visible)
        {
            if (string.IsNullOrEmpty(key) || _toolbarState == null) return;
            var btn = _toolbarState.FindButton(key);
            if (btn == null) return;
            if (btn.IsVisible == visible) return;
            btn.IsVisible = visible;
            Layout?.Recalculate();
            SafeInvalidate();
        }

        /// <summary>
        /// Returns whether the given toolbar button key is currently visible.
        /// Useful for parent forms that need to keep toolbar buttons in sync
        /// with selection state (e.g. disable Delete when nothing is selected).
        /// </summary>
        public bool IsToolbarButtonVisible(string key)
        {
            if (string.IsNullOrEmpty(key) || _toolbarState == null) return false;
            return _toolbarState.FindButton(key)?.IsVisible ?? false;
        }

        /// <summary>Show or hide the toolbar "New" (Add) button. Default: false.</summary>
        [Browsable(true)]
        [Category("Toolbar")]
        [Description("Shows or hides the 'New' (Add) toolbar button.")]
        [DefaultValue(false)]
        public bool ShowAddButton
        {
            get => IsToolbarButtonVisible(Toolbar.BeepGridToolbarState.KeyAdd);
            set => SetToolbarButtonVisible(Toolbar.BeepGridToolbarState.KeyAdd, value);
        }

        /// <summary>Show or hide the toolbar "Edit" button. Default: false.</summary>
        [Browsable(true)]
        [Category("Toolbar")]
        [Description("Shows or hides the 'Edit' toolbar button.")]
        [DefaultValue(false)]
        public bool ShowEditButton
        {
            get => IsToolbarButtonVisible(Toolbar.BeepGridToolbarState.KeyEdit);
            set => SetToolbarButtonVisible(Toolbar.BeepGridToolbarState.KeyEdit, value);
        }

        /// <summary>Show or hide the toolbar "Delete" button. Default: false.</summary>
        [Browsable(true)]
        [Category("Toolbar")]
        [Description("Shows or hides the 'Delete' toolbar button.")]
        [DefaultValue(false)]
        public bool ShowDeleteButton
        {
            get => IsToolbarButtonVisible(Toolbar.BeepGridToolbarState.KeyDelete);
            set => SetToolbarButtonVisible(Toolbar.BeepGridToolbarState.KeyDelete, value);
        }

        #endregion
    }
}
