
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Refactored, helper-driven grid control inspired by BeepSimpleGrid.")]
    [DisplayName("Beep Grid Pro")]
    [ComplexBindingProperties("DataSource", "DataMember")] // Enable designer complex data binding support
    public  class BeepGridPro : BaseControl
    {

        // Dedicated host layer for in-place editors (DevExpress-like practice)
       // private readonly Panel _editorHost;
      //  internal Control EditorHost => _editorHost;

        internal Helpers.GridLayoutHelper Layout { get; }
        internal Helpers.GridDataHelper Data { get; }
        internal Helpers.GridRenderHelper Render { get; }
        internal Helpers.GridSelectionHelper Selection { get; }
        internal Helpers.GridInputHelper Input { get; }
        internal Helpers.GridScrollHelper Scroll { get; }
        internal Helpers.GridScrollBarsHelper ScrollBars { get; }
        internal Helpers.GridSortFilterHelper SortFilter { get; }
        internal Helpers.GridEditHelper Edit { get; }
        internal Helpers.GridThemeHelper ThemeHelper { get; }
        internal Helpers.GridNavigatorHelper Navigator { get; }
        internal Helpers.GridSizingHelper Sizing { get; }
        internal Helpers.GridDialogHelper Dialog { get; }

        // Data management fields (similar to BeepSimpleGrid)
        internal Type _entityType;
        internal List<object> _fullData;
        internal int _dataOffset = 0;

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
            set { 
                if (!ReferenceEquals(Data.DataSource, value))
                {
                    Data.Bind(value); // Bind to original data source
                    Navigator.BindTo(value); 
                    Data.InitializeData(); // Sync data after binding
                    Layout.Recalculate(); 
                    if (!DesignMode) Invalidate();
                }
            }
        }

        // Optional DataMember to support ComplexBindingProperties at design-time
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
                    // Rebind to apply the new DataMember when possible
                    if (Data.DataSource != null) 
                    { 
                        Navigator.BindTo(Data.DataSource); // Re-bind navigator with new DataMember
                        Data.Bind(Data.DataSource); // Re-bind data with original data source
                        Data.InitializeData(); // Re-sync data with new DataMember
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

        [Browsable(false)]
        internal Type EntityType => _entityType;

        internal void SetEntityType(Type entityType)
        {
            _entityType = entityType;
        }

        [Browsable(true)]
        [Category("Layout")] 
        public int RowHeight 
        { 
            get => Layout.RowHeight;
            set { 
                if (Layout.RowHeight != Math.Max(18, value))
                {
                    Layout.RowHeight = Math.Max(18, value);
                    if (!DesignMode) Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public int ColumnHeaderHeight 
        { 
            get => Layout.ColumnHeaderHeight; 
            set { 
                if (Layout.ColumnHeaderHeight != Math.Max(22, value))
                {
                    Layout.ColumnHeaderHeight = Math.Max(22, value);
                    if (!DesignMode) Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public bool ShowColumnHeaders 
        { 
            get => Layout.ShowColumnHeaders; 
            set { 
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
            set { 
                if (_showNavigator != value) 
                { 
                    _showNavigator = value; 
                    Layout.Recalculate(); 
                    if (!DesignMode) Invalidate(); 
                } 
            }
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
                    // Ensure system columns exist before setting visibility
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
     

        public BeepGridPro():base   ()
        {
            //// Enhance control styles for better performance and reduced flickering
            //SetStyle(ControlStyles.AllPaintingInWmPaint |
            //         ControlStyles.UserPaint |
            //         ControlStyles.OptimizedDoubleBuffer |
            //         ControlStyles.ResizeRedraw |
            //         ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles.Selectable, true);
            //UpdateStyles();
            Console.WriteLine("BeepGridPro constructor called");
            // Disable base-frame right border and borders so DrawingRect uses full client area
            //ShowRightBorder = false;
            //ShowAllBorders = false;
            //IsFrameless = true;
            Console.WriteLine("BeepGridPro base constructor completed");
            // Create a dedicated host layer for editors (kept only over active cell)
          
            Console.WriteLine("BeepGridPro editor host created");
            Layout = new Helpers.GridLayoutHelper(this);
            Console.WriteLine("BeepGridPro layout helper created");
            Data = new Helpers.GridDataHelper(this);
            Console.WriteLine("BeepGridPro data helper created");
            Render = new Helpers.GridRenderHelper(this);
            Console.WriteLine("BeepGridPro render helper created");
            Selection = new Helpers.GridSelectionHelper(this);
            Console.WriteLine("BeepGridPro selection helper created");
            Input = new Helpers.GridInputHelper(this);
            Console.WriteLine("BeepGridPro input helper created");
            Scroll = new Helpers.GridScrollHelper(this);
            Console.WriteLine("BeepGridPro scroll helper created");
            ScrollBars = new Helpers.GridScrollBarsHelper(this);
            Console.WriteLine("BeepGridPro scrollbars helper created"); 
            SortFilter = new Helpers.GridSortFilterHelper(this);
            Console.WriteLine("BeepGridPro sort/filter helper created");
            Edit = new Helpers.GridEditHelper(this);
            Console.WriteLine("BeepGridPro edit helper created");
            ThemeHelper = new Helpers.GridThemeHelper(this);
            Console.WriteLine("BeepGridPro theme helper created");
            Navigator = new Helpers.GridNavigatorHelper(this);
            Console.WriteLine("BeepGridPro navigator helper created");
            Sizing = new Helpers.GridSizingHelper(this);
            Console.WriteLine("BeepGridPro sizing helper created");
            Dialog = new Helpers.GridDialogHelper(this);
            Console.WriteLine("BeepGridPro dialog helper created");
            // Only subscribe to events and setup complex initialization if not in design mode
            if (!DesignMode)
            {
                // Subscribe to column model changes so designer edits are reflected immediately
                HookColumnsCollection(Data.Columns);

                this.MouseDown += (s, e) => Input.HandleMouseDown(e);
                this.MouseMove += (s, e) => Input.HandleMouseMove(e);
                this.MouseUp += (s, e) => Input.HandleMouseUp(e);
                this.MouseWheel += (s, e) => { 
                    // Handle mouse wheel exactly like BeepSimpleGrid
                    ScrollBars?.HandleMouseWheel(e); 
                    // Custom scrollbars updated automatically through drawing
                };
                this.KeyDown += (s, e) => Input.HandleKeyDown(e);

                // Enable Excel-like filter popup automatically
                this.EnableExcelFilter();
            }

            RowHeight = 25;
            ColumnHeaderHeight = 28;
            ShowColumnHeaders = true;
        }

        //// Ensure window style flags are set during handle creation
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var cp = base.CreateParams;
        //        // Temporarily remove these flags to fix editor host visibility issues
        //        // cp.Style |= WS_CLIPCHILDREN | WS_CLIPSIBLINGS;
        //        return cp;
        //    }
        //}

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
            // Skip excessive processing in design mode
            if (DesignMode) return;
            
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
            // Skip excessive processing in design mode
            if (DesignMode) return;
            
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

            // Do not stretch the editor host; it will be sized to the active cell only
            
            // Only update scrollbars if not in design mode
            if (!DesignMode)
            {
                ScrollBars?.UpdateBars();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("BeepGridPro OnPaint called");
            base.OnPaint(e);
        }

        protected override void DrawContent(Graphics g)
        {
            Console.WriteLine("BeepGridPro DrawContent START");

            // Call base for graphics setup and UpdateDrawingRect (now handled by BaseControl)
            base.DrawContent(g);

            // Clip to the DrawingRect to ensure grid draws within the intended area
            var drawingRect = DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            {
                Console.WriteLine("BeepGridPro DrawContent - Invalid DrawingRect, skipping draw");
                return;
            }

            Console.WriteLine("BeepGridPro DrawContent - DrawingRect: " + drawingRect.ToString());

            // Save the current graphics state
            var graphicsState = g.Save();

            try
            {
                // Set clipping region to DrawingRect
                g.SetClip(drawingRect);

                // Now do our custom grid drawing within the clipped area
                Console.WriteLine("BeepGridPro DrawContent called");
                Console.WriteLine("BeepGridPro DrawContent - Starting layout calculation");
                Layout?.EnsureCalculated();
                Console.WriteLine("BeepGridPro Layout calculated");
                Render?.Draw(g);
                Console.WriteLine("BeepGridPro Render completed");

                // Draw custom scrollbars after grid content
                ScrollBars?.DrawScrollBars(g);
                Console.WriteLine("BeepGridPro ScrollBars drawn");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BeepGridPro DrawContent - Grid drawing failed: {ex.Message}");
            }
            finally
            {
                // Always restore the graphics state
                g.Restore(graphicsState);
            }

            // Keep scrollbars in sync after rendering (outside clipping)
            if (!DesignMode)
            {
                ScrollBars?.UpdateBars();
            }

            Console.WriteLine("BeepGridPro DrawContent END");
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
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Ensures that system columns (checkbox, row number, row ID) are present in the grid.
        /// This method is called automatically but can be called manually if needed.
        /// </summary>
        public void EnsureSystemColumns()
        {
            Data.EnsureSystemColumns();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        public void RefreshGrid()
        {
            Data.RefreshRows();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Auto-resize columns to fit their content, similar to BeepSimpleGrid
        /// Also handles row height auto-sizing when AutoSizeColumnsMode is AllCells or AllCellsExceptHeader, or when AutoSizeRowsToContent is true
        /// </summary>
        public void AutoResizeColumnsToFitContent()
        {
            Sizing.AutoResizeColumnsToFitContent();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Auto-resize row heights to fit their content based on the tallest cell in each row
        /// </summary>
        public void AutoSizeRowsToFitContent()
        {
            Sizing.AutoSizeRowsToFitContent();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Calculate optimal width for a column based on its content
        /// </summary>
        /// <param name="column">The column to measure</param>
        /// <param name="includeHeader">Whether to include header text in measurement</param>
        /// <param name="allRows">Whether to measure all rows or just visible rows</param>
        /// <returns>Optimal width in pixels</returns>
        private int GetColumnWidth(BeepColumnConfig column, bool includeHeader, bool allRows)
        {
            return Sizing.GetColumnWidth(column, includeHeader, allRows);
        }

        /// <summary>
        /// Set a specific column width by name
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="width">New width in pixels</param>
        public void SetColumnWidth(string columnName, int width)
        {
            Sizing.SetColumnWidth(columnName, width);
            SafeRecalculate();
            ScrollBars?.UpdateBars();
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

        // Expose action methods for owner-drawn navigator - delegate to Navigator helper
        public void MoveFirst() => Navigator.MoveFirst();
        public void MovePrevious() => Navigator.MovePrevious();
        public void MoveNext() => Navigator.MoveNext();
        public void MoveLast() => Navigator.MoveLast();
        public void InsertNew() => Navigator.InsertNew();
        public void DeleteCurrent() => Navigator.DeleteCurrent();
        public void Save() => Navigator.Save();
        public void Cancel() => Navigator.Cancel();
        
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

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a save operation is requested/completed.")]
        public event EventHandler SaveCalled;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a cell value is changed by the editor.")]
        public event EventHandler<BeepCellEventArgs> CellValueChanged;

        internal void OnSaveCalled()
        {
            try { SaveCalled?.Invoke(this, EventArgs.Empty); } catch { }
        }

        /// <summary>
        /// Raise CellValueChanged event for helpers to call
        /// </summary>
        /// <param name="cell">The cell that changed</param>
        internal void OnCellValueChanged(BeepCellConfig cell)
        {
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(cell));
        }

        // BeepSimpleGrid compatibility helpers
        public BeepColumnConfig GetColumnByName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return null;
            return Data.Columns.FirstOrDefault(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
        }

        public BeepColumnConfig GetColumnByCaption(string caption)
        {
            if (string.IsNullOrWhiteSpace(caption)) return null;
            return Data.Columns.FirstOrDefault(c => string.Equals(caption, c.ColumnCaption, StringComparison.OrdinalIgnoreCase));
        }

        public BeepColumnConfig GetColumnByIndex(int index)
        {
            if (index < 0 || index >= Data.Columns.Count) return null;
            return Data.Columns[index];
        }

        public Dictionary<string, BeepColumnConfig> GetDictionaryColumns()
        {
            return Data.Columns.ToDictionary(c => c.ColumnName, c => c, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Shows an editor dialog for the currently selected cell
        /// </summary>
        public void ShowCellEditor()
        {
            if (Selection.HasSelection)
            {
                var cell = Data.Rows[Selection.RowIndex].Cells[Selection.ColumnIndex];
                Dialog.ShowEditorDialog(cell);
            }
        }

        /// <summary>
        /// Shows the filter dialog
        /// </summary>
        public void ShowFilterDialog()
        {
            Dialog.ShowFilterDialog();
        }

        /// <summary>
        /// Shows the search dialog
        /// </summary>
        public void ShowSearchDialog()
        {
            Dialog.ShowSearchDialog();
        }

        /// <summary>
        /// Shows the column configuration dialog
        /// </summary>
        public void ShowColumnConfigDialog()
        {
            Dialog.ShowColumnConfigDialog();
        }

        /// <summary>
        /// Enables Excel-like filter functionality (can be called to initialize filters)
        /// </summary>
        public void EnableExcelFilter()
        {
            // This method can be expanded to add filter dropdowns to column headers
            // For now, it's a placeholder that ensures the grid is ready for filtering
            foreach (var col in Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
            {
                col.ShowFilterIcon = true;
            }
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Handle custom scrollbar mouse interaction
            if (!DesignMode)
            {
                ScrollBars?.HandleMouseMove(e.Location);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Handle custom scrollbar mouse interaction
            if (!DesignMode)
            {
                ScrollBars?.HandleMouseDown(e.Location, e.Button);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // Handle custom scrollbar mouse interaction
            if (!DesignMode)
            {
                ScrollBars?.HandleMouseUp(e.Location, e.Button);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dialog?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
