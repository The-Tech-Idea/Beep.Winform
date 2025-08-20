using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Refactored, helper-driven grid control inspired by BeepSimpleGrid.")]
    [DisplayName("Beep Grid Pro")]
    [ComplexBindingProperties("DataSource", "DataMember")] // Enable designer complex data binding support
    public partial class BeepGridPro : BeepControl
    {
        internal Helpers.GridLayoutHelper Layout { get; }
        internal Helpers.GridDataHelper Data { get; }
        internal Helpers.GridRenderHelper Render { get; }
        internal Helpers.GridSelectionHelper Selection { get; }
        internal Helpers.GridInputHelper Input { get; }
        internal Helpers.GridScrollHelper Scroll { get; }
        internal Helpers.GridSortFilterHelper SortFilter { get; }
        internal Helpers.GridEditHelper Edit { get; }
        internal Helpers.GridThemeHelper ThemeHelper { get; }
        internal Helpers.GridNavigatorHelper Navigator { get; }

        private GridUnitOfWorkBinder _uowBinder;
        private object _uow;

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
                    EnsureBinder();
                    _uowBinder.Attach(_uow);
                    Navigator.SetUnitOfWork(_uow);
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [AttributeProvider(typeof(IListSource))] // Show BindingSource and list-like components in designer
        [DefaultValue(null)]
        public object DataSource 
        {
            get => Data.DataSource;
            set { Data.Bind(value); Navigator.BindTo(value); Layout.Recalculate(); Invalidate(); }
        }

        // Optional DataMember to support ComplexBindingProperties at design-time
        private string _dataMember = string.Empty;
        [Browsable(true)]
        [Category("Data")]
        [DefaultValue("")]
        public string DataMember 
        { 
            get => _dataMember; 
            set 
            { 
                if (_dataMember != value) 
                { 
                    _dataMember = value ?? string.Empty; 
                    // Rebind to apply the new DataMember when possible
                    if (Data.DataSource != null) 
                    { 
                        Data.Bind(Data.DataSource); 
                        Invalidate(); 
                    }
                } 
            } 
        }

        // Expose columns collection for design-time editing and serialization
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        public BeepGridColumnConfigCollection Columns => Data.Columns;

        [Browsable(false)]
        public BindingList<BeepRowConfig> Rows => Data.Rows;

        [Browsable(true)]
        [Category("Layout")] 
        public int RowHeight 
        { 
            get => Layout.RowHeight;
            set { Layout.RowHeight = Math.Max(18, value); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public int ColumnHeaderHeight 
        { 
            get => Layout.ColumnHeaderHeight; 
            set { Layout.ColumnHeaderHeight = Math.Max(22, value); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public bool ShowColumnHeaders 
        { 
            get => Layout.ShowColumnHeaders; 
            set { Layout.ShowColumnHeaders = value; Invalidate(); }
        }

        // Owner-drawn navigator footer like BeepSimpleGrid (not a child control)
        private bool _showNavigator = true;
        [Browsable(true)]
        [Category("Layout")] 
        [DefaultValue(true)]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set { if (_showNavigator != value) { _showNavigator = value; Layout.Recalculate(); Invalidate(); } }
        }

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeColumns { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeRows { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowCheckBox { get; set; } = false;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        public bool AutoFillColumns { get; set; } = false;

        public BeepGridPro()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            // Disable base-frame right border and borders so DrawingRect uses full client area
            ShowRightBorder = false;
            ShowAllBorders = false;
            IsFrameless = true;

            Layout = new Helpers.GridLayoutHelper(this);
            Data = new Helpers.GridDataHelper(this);
            Render = new Helpers.GridRenderHelper(this);
            Selection = new Helpers.GridSelectionHelper(this);
            Input = new Helpers.GridInputHelper(this);
            Scroll = new Helpers.GridScrollHelper(this);
            SortFilter = new Helpers.GridSortFilterHelper(this);
            Edit = new Helpers.GridEditHelper(this);
            ThemeHelper = new Helpers.GridThemeHelper(this);
            Navigator = new Helpers.GridNavigatorHelper(this);

            // Subscribe to column model changes so designer edits are reflected immediately
            HookColumnsCollection(Data.Columns);

            RowHeight = 25;
            ColumnHeaderHeight = 28;
            ShowColumnHeaders = true;

            this.MouseDown += (s, e) => Input.HandleMouseDown(e);
            this.MouseMove += (s, e) => Input.HandleMouseMove(e);
            this.MouseUp += (s, e) => Input.HandleMouseUp(e);
            this.MouseWheel += (s, e) => Scroll.HandleMouseWheel(e);
            this.KeyDown += (s, e) => Input.HandleKeyDown(e);
        }

        private void HookColumnsCollection(BeepGridColumnConfigCollection cols)
        {
            if (cols == null) return;
            cols.ListChanged -= Columns_ListChanged; // avoid duplicates
            cols.ListChanged += Columns_ListChanged;
            // Subscribe to existing items
            foreach (var col in cols)
            {
                if (col is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged -= Column_PropertyChanged;
                    inpc.PropertyChanged += Column_PropertyChanged;
                }
            }
        }

        private void Columns_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Subscribe to added items
            if (e.ListChangedType == ListChangedType.ItemAdded && e.NewIndex >= 0 && e.NewIndex < Data.Columns.Count)
            {
                var col = Data.Columns[e.NewIndex];
                if (col is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged -= Column_PropertyChanged;
                    inpc.PropertyChanged += Column_PropertyChanged;
                }
            }
            else if (e.ListChangedType == ListChangedType.Reset)
            {
                // Re-hook all
                HookColumnsCollection(Data.Columns);
            }

            SafeRecalculate();
            Invalidate();
        }

        private void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SafeRecalculate();
            Invalidate();
        }

        private void SafeRecalculate()
        {
            if (Layout != null && !Layout.IsCalculating)
                Layout.Recalculate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            SafeRecalculate();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            UpdateDrawingRect();
            Layout.EnsureCalculated();
            Render.Draw(e.Graphics);
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            ThemeHelper.ApplyTheme();
            Invalidate();
        }

        public void AutoGenerateColumns()
        {
            Data.AutoGenerateColumns();
            Invalidate();
        }

        public void RefreshGrid()
        {
            Data.RefreshRows();
            Invalidate();
        }

        public void SelectCell(int rowIndex, int columnIndex)
        {
            Selection.SelectCell(rowIndex, columnIndex);
            Invalidate();
        }

        public void AttachNavigator(BeepBindingNavigator navigator, object dataSource)
        {
            Navigator.Attach(navigator, dataSource);
        }

        // Expose action methods for owner-drawn navigator
        public void MoveFirst() => Navigator.MoveFirst();
        public void MovePrevious() => Navigator.MovePrevious();
        public void MoveNext() => Navigator.MoveNext();
        public void MoveLast() => Navigator.MoveLast();
        public void InsertNew() => Navigator.InsertNew();
        public void DeleteCurrent() => Navigator.DeleteCurrent();
        public void Save() => Navigator.Save();
        public void Cancel() => Navigator.Cancel();

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a cell value is changed by the editor.")]
        public event EventHandler<BeepCellEventArgs> CellValueChanged;

        internal void OnCellValueChanged(BeepCellConfig cell)
        {
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(cell));
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            // Ensure hooks are active after designer rehydration
            HookColumnsCollection(Data.Columns);
            if (DesignMode && Data.DataSource != null && Data.Columns.Count == 0)
            {
                Data.AutoGenerateColumns();
                SafeRecalculate();
                Invalidate();
            }
        }

        private void EnsureBinder()
        {
            _uowBinder ??= new GridUnitOfWorkBinder(this);
        }

        public void RebindUow()
        {
            if (_uow == null) return;
            EnsureBinder();
            _uowBinder.Attach(_uow);
        }
    }

    public partial class BeepGridPro : BeepControl
    {
        [Browsable(false)]
        public IReadOnlyList<BeepRowConfig> SelectedRows => Data.Rows.Where(r => r.IsSelected).ToList();

        [Browsable(false)]
        public IReadOnlyList<int> SelectedRowIndices => Data.Rows
            .Select((r, i) => new { r, i })
            .Where(x => x.r.IsSelected)
            .Select(x => x.i)
            .ToList();

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when the row selection changes (checkbox or active cell). RowIndex = -1 means bulk change.")]
        public event EventHandler<BeepRowSelectedEventArgs> RowSelectionChanged;

        internal void OnRowSelectionChanged(int rowIndex)
        {
            BeepRowConfig row = (rowIndex >= 0 && rowIndex < Data.Rows.Count) ? Data.Rows[rowIndex] : null;
            RowSelectionChanged?.Invoke(this, new BeepRowSelectedEventArgs(rowIndex, row));
        }

        internal void OnRowSelectionChanged(BeepRowConfig row)
        {
            int idx = row != null ? Data.Rows.IndexOf(row) : -1;
            OnRowSelectionChanged(idx);
        }
    }
}
