using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;
using navigationStyle = TheTechIdea.Beep.Winform.Controls.GridX.Painters.navigationStyle;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    public partial class BeepGridPro : BaseControl
    {
        // Data management fields
        internal Type _entityType = typeof(object);
        internal List<object> _fullData = new List<object>(); // Full data set for paging/filter operations
        internal int _dataOffset = 0; // Paging offset
        private object _uow = null!;

        [Browsable(false)]
        internal Type EntityType => _entityType;
        internal void SetEntityType(Type entityType) => _entityType = entityType;

        [Browsable(true)]
        [Category("Data")]
        [Description("Assign an IUnitofWork<T> instance. When set, its Units will be used as the grid data source and kept in sync.")]
        [TypeConverter(typeof(UnitOfWorksConverter))]
        public object Uow
        {
            get => _uow;
            set
            {
                if (!ReferenceEquals(_uow, value))
                {
                    _uow = value;
                    Navigator.SetUnitOfWork(_uow);
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        public object DataSource
        {
            get => Data.DataSource;
            set
            {
                if (!ReferenceEquals(Data.DataSource, value))
                {
                    Data.Bind(value);
                    Navigator.BindTo(value);
                    Data.InitializeData();
                    Layout.Recalculate();
                    if (!DesignMode) Invalidate();
                }
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
                        Navigator.BindTo(Data.DataSource);
                        Data.Bind(Data.DataSource);
                        Data.InitializeData();
                        Invalidate();
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
                if (Layout.RowHeight != System.Math.Max(18, value))
                {
                    Layout.RowHeight = System.Math.Max(18, value);
                    if (!DesignMode) Invalidate();
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
                if (Layout.ColumnHeaderHeight != System.Math.Max(22, value))
                {
                    Layout.ColumnHeaderHeight = System.Math.Max(22, value);
                    if (!DesignMode) Invalidate();
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
                    if (!DesignMode) Invalidate();
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
                    if (!DesignMode) Invalidate();
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
                    Invalidate();
                    Refresh();
                    if (DesignMode && Site != null)
                    {
                        var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                        changeService?.OnComponentChanged(this, null, null, null);
                    }
                }
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
                    Invalidate();
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
                    Invalidate();
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
                Invalidate();
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
                        if (!DesignMode) Invalidate();
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
        [DefaultValue(false)]
        public bool AutoSizeRowsToContent { get; set; } = false;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(2)]
        [Description("Padding added to auto-sized row heights (in pixels)")]
        public int RowAutoSizePadding { get; set; } = 2;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(true)]
        [Description("Whether to use DPI-aware scaling for row height calculations")]
        public bool UseDpiAwareRowHeights { get; set; } = true;
    }
}
