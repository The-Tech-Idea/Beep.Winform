using TheTechIdea.Beep.Vis.Modules;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Drawing.Printing;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Winform.Controls.Filter;
using TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns;
using TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns.CustomDataGridViewColumns;
using TheTechIdea.Beep.Winform.Controls.Grid.DesignerForm;

using TheTechIdea.Beep.Logger;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;
using Newtonsoft.Json.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    /// <summary>
    /// The BeepGrid class is a custom control that extends the functionality of a DataGridView with additional features such as sorting, filtering, and theming.
    /// </summary>
    /// 
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepGrid))] // Optional//"Resources.BeepButtonIcon.bmp"
    [Category("Beep Controls")]
    [DisplayName("Beep Grid")]
    public partial class BeepGrid : BeepControl, IDataGridView
    {
        public IDMEEditor DMEEditor { get; set; }
        public EntityStructure EntityStructure { get; set; }
        TableLayoutPanel layoutPanel;
        private float originalFilterPanelHeight = 28;
        private float originalTotalPanelHeight = 28;

        private string _gridId;
        private int padding = 2;
        private Size buttonwidth=new Size(15, 15);
        /// <summary>
        /// Unique identifier for each BeepGrid instance, persisted across design and runtime.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Design"), Description("Unique identifier for the grid.")]
        public string GridId
        {
            get => _gridId;
            set => _gridId = value;
        }
        #region "Properties"

        /// <summary>
        /// Indicates if the grid is in query mode (typically when loading or updating data).
        /// </summary>
        bool InQuery = true;

        /// <summary>
        /// Tracks whether a column is currently being resized.
        /// </summary>
        private bool isResizing = false;

        /// <summary>
        /// Tracks if the left side of a column is being resized.
        /// </summary>
        private bool isResizingLeft = false;

        /// <summary>
        /// Stores the initial mouse position when resizing begins.
        /// </summary>
        private Point initialMousePosition;

        /// <summary>
        /// Stores the initial width of the column being resized.
        /// </summary>
        private int initialColumnWidth;

        /// <summary>
        /// Stores the initial left position of the label when resizing.
        /// </summary>
        private int initialLabelLeft;

        /// <summary>
        /// Reference to the label being resized.
        /// </summary>
        private Label resizingLabel;

        /// <summary>
        /// The current column being sorted.
        /// </summary>
        private DataGridViewColumn SortColumn = null;

        /// <summary>
        /// The current label being used for sorting.
        /// </summary>
        private Label SortColumnLabel = null;

        /// <summary>
        /// The current direction of sorting (Ascending, Descending, or None).
        /// </summary>
        SortOrder Currentdirection = SortOrder.None;

        /// <summary>
        /// Indicates if sorting is currently applied on the grid.
        /// </summary>
        private bool IsSorting = false;

        /// <summary>
        /// A dictionary mapping header labels to their corresponding sort icons.
        /// </summary>
        private Dictionary<Label, PictureBox> sortIcons = new Dictionary<Label, PictureBox>();

        /// <summary>
        /// A dictionary mapping header labels to their corresponding DataGridView columns.
        /// </summary>
        private Dictionary<Label, DataGridViewColumn> headerColumnMapping = new Dictionary<Label, DataGridViewColumn>();

        /// <summary>
        /// Event handler for cell painting, allowing custom cell rendering.
        /// </summary>
        public EventHandler<DataGridViewCellPaintingEventArgs> CellPainting { get; set; }

        /// <summary>
        /// Stores configuration details for each column, such as filters and totals.
        /// </summary>
        private List<BeepGridColumnConfig> columnConfigs = new List<BeepGridColumnConfig>();
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(ColumnConfigCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<BeepGridColumnConfig> ColumnsConfigurations
        {
            get => columnConfigs;
            set
            {
                columnConfigs = value;
                Invalidate(); // Redraw grid with new columns
            }
        }
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TitleFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                UseThemeFont = false;
                if (UseThemeFont)
                {
                    _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                    Titlelabel.UseThemeFont = true;
                }
                else
                {
                    Titlelabel.TextFont = _textFont;
                }
                if (ShowHeaderPanel)
                {
                    layoutPanel.RowStyles[0].Height = Titlelabel.PreferredSize.Height + (2 * (padding + 2)); // Adjust height dynamically
                }
             //   Invalidate();


            }
        }
        /// <summary>
        /// A list of TextBox controls for filtering columns.
        /// </summary>
        private List<TextBox> filterTextBoxes { get; set; } = new List<TextBox>();

        /// <summary>
        /// A list of MaskedTextBox controls for displaying column totals.
        /// </summary>
        private List<MaskedTextBox> TotalTextBoxes { get; set; } = new List<MaskedTextBox>();

        /// <summary>
        /// A list of GridControls used for managing filters and totals.
        /// </summary>
        private List<GridControls> gridControls { get; set; } = new List<GridControls>();

        /// <summary>
        /// A dictionary to hold filters for each column.
        /// </summary>
        private Dictionary<string, string> columnFilters = new Dictionary<string, string>();

        /// <summary>
        /// Gets the main DataGridView control.
        /// </summary>
        public DataGridView GridView
        {
            get { return dataGridView1; }
        }

        /// <summary>
        /// Gets the label used for displaying the grid's title.
        /// </summary>
        public BeepLabel Title
        {
            get { return Titlelabel; }
        }

        /// <summary>
        /// Raised when a request to print is made.
        /// </summary>
        public event EventHandler<BindingSource> CallPrinter;

        /// <summary>
        /// Raised when a message is sent.
        /// </summary>
        public event EventHandler<BindingSource> SendMessage;

        /// <summary>
        /// Raised when a search is requested.
        /// </summary>
        public event EventHandler<BindingSource> ShowSearch;

        /// <summary>
        /// Raised when a new record is created.
        /// </summary>
        public event EventHandler<BindingSource> NewRecordCreated;

        /// <summary>
        /// Raised when a save operation is requested.
        /// </summary>
        public event EventHandler<BindingSource> SaveCalled;

        /// <summary>
        /// Raised when a delete operation is requested.
        /// </summary>
        public event EventHandler<BindingSource> DeleteCalled;

        /// <summary>
        /// Stores the default border style for the grid.
        /// </summary>
        private System.Windows.Forms.BorderStyle IsBorderStyle = BorderStyle.FixedSingle;
        private Color _backColor;
        private BorderStyle _borderStyle;
        private BeepTheme _theme;

        /// <summary>
        /// The background color of the grid.
        /// </summary>
        public new Color BackColor
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    dataGridView1.BackgroundColor = value;
                    customHeaderPanel.BackColor = value;
                    filterPanel.BackColor = value;
                    Totalspanel.BackColor = value;
                    base.BackColor = value;
                }
            }
        }

        /// <summary>
        /// The border style of the grid.
        /// </summary>
        public new BorderStyle BorderStyle
        {
            get { return _borderStyle; }
            set
            {
                if (_borderStyle != value)
                {
                    _borderStyle = value;
                    dataGridView1.BorderStyle = value;
                    customHeaderPanel.BorderStyle = value;
                    filterPanel.BorderStyle = value;
                    Totalspanel.BorderStyle = value;
                    base.BorderStyle = value;
                }
            }
        }

        /// <summary>
        /// Controls the visibility of the totals panel.
        /// </summary>
        /// 

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTotalsPanel
        {
            get
            {
                return Totalspanel.Visible;
            }
            set
            {
                Totalspanel.Visible = value;
                layoutPanel.RowStyles[4].Height = value ? originalTotalPanelHeight : 0; // Collapse row if false
            }
        }

        /// <summary>
        /// Controls the visibility of the navigator panel.
        /// </summary>
        ///  [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowNavigatorPanel
        {
            get
            {
                return BindingNavigator.Visible;
            }
            set
            {
                BindingNavigator.Visible = value;
                layoutPanel.RowStyles[5].Height = value ? 28 : 0; // Fixed height for navigator
            }
        }

        /// <summary>
        /// Controls the visibility of the header panel.
        /// </summary>
        ///  [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowHeaderPanel
        {
            get
            {
                return Toppanel.Visible;
            }
            set
            {
                Toppanel.Visible = value;

                layoutPanel.RowStyles[0].Height = value ? Titlelabel.PreferredSize.Height+(2*(padding+2)) : 0; // Adjust height dynamically
            }
        }


        /// <summary>
        /// Controls the visibility of the custom column header panel.
        /// </summary>
        ///  [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowColumnHeaderPanel
        {
            get
            {
                return customHeaderPanel.Visible;
            }
            set
            {
                customHeaderPanel.Visible = value;
                layoutPanel.RowStyles[1].Height = value ? 31 : 0; // Fixed height for column header panel
            }
        }

        /// <summary>
        /// Controls the visibility of the filter panel.
        /// </summary>
        ///  [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowFilterPanel
        {
            get
            {
                return filterPanel.Visible;
            }
            set
            {
                filterPanel.Visible = value;
                layoutPanel.RowStyles[3].Height = value ? originalFilterPanelHeight : 0; // Collapse row if false
            }
        }

        /// <summary>
        /// Gets or sets the BindingSource used by the grid's navigator.
        /// </summary>
        public BindingSource bindingSource
        {
            get { return BindingNavigator.bindingSource; }
            set { BindingNavigator.bindingSource = value; }
        }

        /// <summary>
        /// Specifies whether to show a verification message before deleting records.
        /// </summary>
        public bool VerifyDelete { get; set; } = true;

        #endregion "Properties"
        public  void SetConfig(IDMEEditor pDMEEditor, IDMLogger plogger, IUtil putil, string[] args, IPassedArgs e, IErrorsInfo per)
        {
          
            BindingNavigator.SetConfig(pDMEEditor, plogger, putil, args, e, per);
          //  DMEEditor = pDMEEditor;
            dataGridView1.AllowUserToAddRows = false;
         //   util = putil;
            WireAllControls();
            ShowFilterPanel = false;
            ShowTotalsPanel = false;
            InQuery = true;
        }
        public BeepGrid():base()
        {
            CreateComponent();
            Margin = new Padding(1);
            columnConfigs = new List<BeepGridColumnConfig>();
            this.Resize += BeepGrid_Resize;
            dataGridView1.Scroll += DataGridView_Scroll; // Handle the Scroll event
          //  dataGridView1.Resize += DataGridView_Resize;
            //   dataGridView1.ColumnAdded += DataGridView1_ColumnAdded;
            //   dataGridView1.ColumnRemoved += DataGridView1_ColumnRemoved;
            dataGridView1.ColumnWidthChanged += DataGridView_ColumnWidthChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.CellBeginEdit += DataGridView1_CellBeginEdit;
            dataGridView1.DataError += DataGridView1_DataError;
            // dataGridView1.Dock = DockStyle.Fill;
            // this.DoubleBuffered = true;
            // this.Resize += BeepControlResize;
            CSVExportbutton.Click += CSVExportbutton_Click;

            this.Printbutton.Click += Printbutton_Click;
            this.TotalShowbutton.Click += TotalShowbutton_Click;
            this.FilterShowbutton.Click += FilterShowbutton_Click;
            //  this.FilterpictureBox1.Click += FilterpictureBox1_Click;
            // this.Totalbutton.Click += Totalbutton_Click;
            this.dataGridView1.CellValidating += DataGridView1_CellValidating;
            BindingNavigator.bindingSource = new BindingSource();
            // WireAllControls();
            this.BackColor = _backColor;
         //   this.BorderStyle = _borderStyle;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            dataGridView1.ColumnHeadersVisible = false; // Hide default column headers
                                                        // Initialize customHeaderPanel
                                                        //  customHeaderPanel.Height = 30; // Adjust as needed
                                                        //  customHeaderPanel.Dock = DockStyle.Top;
                                                        //Controls.Add(customHeaderPanel);
                                                        //  customHeaderPanel.BringToFront();
       //     typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(dataGridView1, true, null);
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.RowHeadersVisible = false;


            //  dataGridView1.GridColor = Color.FromArgb(45, 45, 45);
            dataGridView1.DefaultCellStyle.Padding = new Padding(5, 2, 5, 2);
            dataGridView1.RowHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.CellPainting += DataGridView1_CellPainting;

            // Set initial styles
            BackColor = dataGridView1.BackColor;
         
            dataGridView1.VirtualMode = false;
            UpdateCustomHeaders();
            // Check if GridId is already assigned, if not, create a new one.
            if (string.IsNullOrEmpty(_gridId))
            {
                GenerateUniqueGridId();
            }

            // Load the layout for the grid
            LoadGridLayout();
        }

        #region "Grid Properties"

        /// <summary>
        /// Gets or sets the data source for the grid.
        /// </summary>
        public object DataSource
        {
            get { return dataGridView1.DataSource; }
            set
            {
                dataGridView1.DataSource = value;
                if (bindingSource == null)
                {
                    bindingSource = new BindingSource();
                }
                bindingSource.DataSource = value;
                SetDataSource(value);
            }
        }
       
        /// <summary>
        /// Gets the collection of columns in the grid.
        /// </summary>
        [Editor(typeof(DataGridViewColumnEditor), typeof(UITypeEditor))]
        public DataGridViewColumnCollection Columns
        {
            get { return dataGridView1.Columns; }
        }

       
        /// <summary>
        /// Gets the collection of rows in the grid.
        /// </summary>
        public DataGridViewRowCollection Rows => dataGridView1.Rows;

        /// <summary>
        /// Accesses a specific cell in the grid using column and row indices.
        /// </summary>
        /// <param name="col">The index of the column.</param>
        /// <param name="invert">The index of the row.</param>
        /// <returns>The specified cell in the grid.</returns>
        public DataGridViewCell this[int col, int invert] => dataGridView1[col, invert];

        /// <summary>
        /// Gets or sets a value indicating whether the user can add rows to the grid.
        /// </summary>
        public bool AllowUserToAddRows
        {
            get { return dataGridView1.AllowUserToAddRows; }
            set { dataGridView1.AllowUserToAddRows = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can delete rows from the grid.
        /// </summary>
        public bool AllowUserToDeleteRows
        {
            get { return dataGridView1.AllowUserToDeleteRows; }
            set { dataGridView1.AllowUserToDeleteRows = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid is read-only.
        /// </summary>
        public bool ReadOnly
        {
            get { return dataGridView1.ReadOnly; }
            set { dataGridView1.ReadOnly = value; }
        }

        // Expose Methods

        /// <summary>
        /// Clears the current selection in the grid.
        /// </summary>
        public void ClearSelection() => dataGridView1.ClearSelection();

        /// <summary>
        /// Sorts the grid by the specified column in the given sort direction.
        /// </summary>
        /// <param name="dataGridViewColumn">The column to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        public void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction) => dataGridView1.Sort(dataGridViewColumn, direction);

        /// <summary>
        /// Automatically resizes the specified column to fit its content.
        /// </summary>
        /// <param name="columnIndex">The index of the column to resize.</param>
        public void AutoResizeColumn(int columnIndex) => dataGridView1.AutoResizeColumn(columnIndex);

        /// <summary>
        /// Automatically resizes all columns to fit their content.
        /// </summary>
        public void AutoResizeColumns() => dataGridView1.AutoResizeColumns();

        // Expose Events

        /// <summary>
        /// Occurs when a cell is clicked in the grid.
        /// </summary>
        public event DataGridViewCellEventHandler CellClick
        {
            add { dataGridView1.CellClick += value; }
            remove { dataGridView1.CellClick -= value; }
        }

        /// <summary>
        /// Occurs when a cell is double-clicked in the grid.
        /// </summary>
        public event DataGridViewCellEventHandler CellDoubleClick
        {
            add { dataGridView1.CellDoubleClick += value; }
            remove { dataGridView1.CellDoubleClick -= value; }
        }

        /// <summary>
        /// Occurs when a cell needs to be formatted for display in the grid.
        /// </summary>
        public event DataGridViewCellFormattingEventHandler CellFormatting
        {
            add { dataGridView1.CellFormatting += value; }
            remove { dataGridView1.CellFormatting -= value; }
        }

        /// <summary>
        /// Occurs when the state of a row in the grid changes.
        /// </summary>
        public event DataGridViewRowStateChangedEventHandler RowStateChanged
        {
            add { dataGridView1.RowStateChanged += value; }
            remove { dataGridView1.RowStateChanged -= value; }
        }

        /// <summary>
        /// Occurs when a data error is encountered in the grid.
        /// </summary>
        public event DataGridViewDataErrorEventHandler DataError
        {
            add { dataGridView1.DataError += value; }
            remove { dataGridView1.DataError -= value; }
        }

        #endregion
        #region "Data Binding and Change"

        /// <summary>
        /// Wires up event handlers for all controls in the form to handle clicks, mouse movement, and other interactions.
        /// </summary>
        private void WireAllControls()
        {
            foreach (Control c in this.Controls)
            {
                c.Click += (sender, e) => { this.InvokeOnClick(this, e); };
                c.MouseUp += (sender, e) => { this.OnMouseUp(e); };
                c.MouseDown += (sender, e) => { this.OnMouseDown(e); };
                c.MouseMove += (sender, e) => { this.OnMouseMove(e); };
            }
        }

        /// <summary>
        /// Sets the data source for the grid and resets the bindings.
        /// </summary>
        /// <param name="value">The data source object to bind to the grid.</param>
        /// <returns>Returns true if the data source was set successfully, otherwise false.</returns>
        private bool SetDataSource(object value)
        {
            try
            {
                InQuery = true;
                BindingNavigator.bindingSource.DataSource = value;
                BindingNavigator.bindingSource.ResetBindings(true);
                this.BindingNavigator.HightlightColor = Color.Yellow;
                dataGridView1.DataSource = BindingNavigator.bindingSource;
                InQuery = false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// <summary>
        /// Dynamically creates columns in the grid based on the provided entity structure.
        /// </summary>
        public void CreateColumnsForEntity()
        {
            dataGridView1.SuspendLayout();
            dataGridView1.Columns.Clear();
            if (dataGridView1 == null || EntityStructure == null || EntityStructure.Fields == null)
            {
                dataGridView1.ResumeLayout();
                return;
            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AllowUserToResizeColumns = true;
            try
            {
                foreach (var field in EntityStructure.Fields)
                {
                    DataGridViewColumn column = null;

                    // Create appropriate column type based on the field's data type
                    switch (Type.GetType(field.fieldtype))
                    {
                        case Type type when type == typeof(string):
                            column = new DataGridViewTextBoxColumn();
                            break;
                        case Type type when type == typeof(int) || type == typeof(long) || type == typeof(short):
                            column = new BeepDataGridViewNumericColumn(); // Custom Numeric Column
                            column.ValueType = type;
                            break;
                        case Type type when type == typeof(decimal) || type == typeof(double) || type == typeof(float):
                            column = new BeepDataGridViewNumericColumn(); // Custom Numeric Column
                            column.ValueType = type;
                            break;
                        case Type type when type == typeof(bool):
                            column = new BeepDataGridViewThreeStateCheckBoxColumn(); // Custom Three State Checkbox Column
                            break;
                        case Type type when type == typeof(DateTime):
                            column = new BeepDataGridViewDateTimePickerColumn
                            {
                                ValueType = type,
                                DefaultCellStyle = { Format = "g" } // General date-time format
                            };
                            break;
                        case Type type when type.IsEnum:
                            column = new BeepDataGridViewComboBoxColumn(); // Custom ComboBox Column with cascading support
                            //{
                            //    DataSource = Enum.GetValues(type),
                            //    ValueType = type
                            //};
                            break;
                        case Type type when type == typeof(Guid):
                            column = new DataGridViewTextBoxColumn();
                            break;
                        case Type type when type == typeof(object):
                            column = new DataGridViewTextBoxColumn
                            {
                                ValueType = typeof(string) // Display ObjectId as a string
                            };
                            break;
                        case Type type when type == typeof(float) || type == typeof(double) || type == typeof(decimal):
                            column = new BeepDataGridViewProgressBarColumn(); // Custom ProgressBar Column
                            break;
                        case Type type when type == typeof(List<string>): // or any List-based structure
                            column = new BeepDataGridViewMultiColumnColumn(); // Custom MultiColumn ComboBox
                            break;
                    }

                    if (column != null)
                    {
                        column.DataPropertyName = field.fieldname;
                        column.Name = field.fieldname;
                        column.HeaderText = field.fieldname;
                        column.Tag = Guid.NewGuid().ToString();
                        dataGridView1.Columns.Add(column);
                        AddColumnConfigurations(column, column.Index, column.Width, field.fieldname, field.fieldname);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding columns in Grid for Entity {EntityStructure.EntityName}: {ex.Message}");
            }
            dataGridView1.ResumeLayout();
        }


        /// <summary>
        /// Resets the grid data and entity structure, and updates the grid columns based on the provided entity structure.
        /// </summary>
        /// <param name="data">The new data to bind to the grid.</param>
        /// <param name="entity">The entity structure defining the columns.</param>
        /// <returns>An <see cref="IErrorsInfo"/> object representing the result of the operation.</returns>
        public IErrorsInfo ResetData(object data, EntityStructure entity)
        {
            if (entity != null)
            {
                EntityStructure = entity;
            }
            else
            {
                DMEEditor.AddLogMessage("Beep", "Entity Structure is Null", DateTime.Now, 0, "", Errors.Failed);
                return DMEEditor.ErrorObject;
            }

            dataGridView1.Columns.Clear();
            if (BindingNavigator.bindingSource != null && EntityStructure != null)
            {
                columnConfigs.Clear();
                filterPanel.Controls.Clear();
                Totalspanel.Controls.Clear();
                gridControls.Clear();

                CreateColumnsForEntity();
                UpdateCustomHeaders();
                Title.Text = EntityStructure.EntityName;
                Titlelabel.Text = EntityStructure.EntityName;
            }
            BindingNavigator.bindingSource.ResetBindings(false);
            DataSource = data;
            return DMEEditor.ErrorObject;
        }

        /// <summary>
        /// Resets the grid columns based on the provided entity structure.
        /// </summary>
        /// <param name="entity">The entity structure defining the columns.</param>
        /// <returns>An <see cref="IErrorsInfo"/> object representing the result of the operation.</returns>
        public IErrorsInfo ResetEntity(EntityStructure entity)
        {
            if (entity != null)
            {
                EntityStructure = entity;
            }
            else
            {
                DMEEditor.AddLogMessage("Beep", "Entity Structure is Null", DateTime.Now, 0, "", Errors.Failed);
                return DMEEditor.ErrorObject;
            }

            dataGridView1.Columns.Clear();
            if (BindingNavigator.bindingSource != null && EntityStructure != null)
            {
                columnConfigs.Clear();
                filterPanel.Controls.Clear();
                Totalspanel.Controls.Clear();
                gridControls.Clear();

                CreateColumnsForEntity();
                UpdateCustomHeaders();
                Title.Text = EntityStructure.EntityName;
                Titlelabel.Text = EntityStructure.EntityName;
            }
            BindingNavigator.bindingSource.ResetBindings(false);
            return DMEEditor.ErrorObject;
        }

        /// <summary>
        /// Handles the validation of the grid cell when it loses focus or the edit is being committed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the validation information.</param>
        private void DataGridView1_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (dataGridView1.IsCurrentCellDirty)
                {
                    dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error in Cell Validation in Grid: {ex.Message}", DateTime.Now, 0, "", Errors.Failed);
            }
        }

        /// <summary>
        /// Event handler triggered when cell editing begins in the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the column and row index.</param>
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (InQuery)
            {
                return;
            }
            decimal oldValue;
            if (dataGridView1.Columns[e.ColumnIndex] is BeepDataGridViewNumericColumn)
            {
                BeepGridColumnConfig cfg = columnConfigs[e.ColumnIndex];
                string cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    decimal.TryParse(cellValue, out oldValue);
                }
                else
                {
                    oldValue = 0;
                }
                cfg.OldValue = oldValue;
            }
        }

        /// <summary>
        /// Event handler triggered when cell editing ends in the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the column and row index.</param>
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (InQuery)
            {
                return;
            }
            if (columnConfigs != null && columnConfigs.Count > 0)
            {
                BeepGridColumnConfig cfg = columnConfigs[e.ColumnIndex];
                if (dataGridView1.Columns[e.ColumnIndex] is BeepDataGridViewNumericColumn)
                {
                    decimal newValue;
                    string cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                    if (!string.IsNullOrEmpty(cellValue) && decimal.TryParse(cellValue, out newValue))
                    {
                        cfg.Total = cfg.Total - cfg.OldValue + newValue;
                    }
                    else
                    {
                        cfg.Total = cfg.Total - cfg.OldValue;
                    }
                }
            }
        }

        /// <summary>
        /// Event handler triggered when a total TextBox value changes, recalculating the total for the corresponding column.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TotalTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            int idx = (int)txt.Tag;
            BeepGridColumnConfig cfg = columnConfigs[idx];
            decimal total = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Sum(row => Convert.ToDecimal(row.Cells[cfg.Index].Value));
        }

        /// <summary>
        /// Event handler triggered when a filter TextBox value changes, updating the filters applied to the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void FilterTextBox_TextChanged(object? sender, EventArgs e)
        {
            TextBox filterBox = sender as TextBox;
            var a = columnConfigs.Find(p => p.GuidID == filterBox.Tag.ToString());
            if (a == null)
            {
                return;
            }
            string columnName = a.ColumnName;

            // Update the filter for the column in the dictionary
            if (string.IsNullOrWhiteSpace(filterBox.Text))
            {
                columnFilters[columnName] = "";
            }
            else
            {
                columnFilters[columnName] = $"{columnName} LIKE '%{filterBox.Text}%'";
            }

            // Rebuild and apply the complete filter string
            ApplyFilters();
        }

        /// <summary>
        /// Applies the current filters to the BindingSource of the grid.
        /// </summary>
        private void ApplyFilters()
        {
            string completeFilter = string.Join(" AND ", columnFilters.Values.Where(filter => !string.IsNullOrEmpty(filter)));
            Console.WriteLine("Applying Filter: " + completeFilter);
            if (string.IsNullOrEmpty(completeFilter))
            {
                BindingNavigator.bindingSource.RemoveFilter();
            }
            else
            {
                BindingNavigator.bindingSource.Filter = completeFilter;
            }
            dataGridView1.DataSource = BindingNavigator.bindingSource;
        }

        public void RefreshGrid()
        {
            // Assuming GridView is the DataGridView instance inside BeepGrid
            if (GridView != null)
            {
                GridView.Refresh(); // Refresh the UI
                GridView.Invalidate(); // Redraw the grid
                GridView.Update(); // Update the control immediately

                // Optionally reset bindings or re-fetch data if necessary
                // GridView.DataSource = ... // Update your data source if required
            }
        }
        #endregion "Data Binding and Change"
        #region "Visual Styles"

        /// <summary>
        /// Updates the custom header labels for the DataGridView columns and applies sorting icons.
        /// </summary>
        private void UpdateCustomHeaders()
        {
            customHeaderPanel.Controls.Clear();
            sortIcons.Clear();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                Label headerLabel = new Label
                {
                    Text = column.HeaderText,
                    Width = column.Width,
                    Height = customHeaderPanel.Height,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = this.BorderStyle,
                    Left = column.DisplayIndex * column.Width,
                    Tag = column
                };
                headerColumnMapping[headerLabel] = column;

                // PictureBox for Sort Icon
                PictureBox sortIcon = new PictureBox
                {
                    Width = 16,
                    Height = 16,
                    Top = 2,
                    Left = headerLabel.Width - 18, // Adjust as needed
                    Visible = false,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                headerLabel.Controls.Add(sortIcon);
                sortIcons[headerLabel] = sortIcon;
                headerLabel.Click += CustomHeaderLabel_Click;
                customHeaderPanel.Controls.Add(headerLabel);
            }
        }

        /// <summary>
        /// Updates the sort icons in the headers based on the current sort direction.
        /// </summary>
        /// <param name="sortedLabel">The label of the sorted column.</param>
        /// <param name="sortDirection">The direction of sorting.</param>
        private void UpdateSortIcons(Label sortedLabel, SortOrder sortDirection)
        {
            foreach (var headerLabel in sortIcons.Keys)
            {
                if (sortIcons[headerLabel] != null)
                {
                    sortIcons[headerLabel].Visible = headerLabel == sortedLabel;
                    if (sortIcons[headerLabel].Visible)
                    {
                        switch (sortDirection)
                        {
                            case SortOrder.None:
                                sortIcons[headerLabel].Image = null;
                                break;
                            case SortOrder.Ascending:
                                sortIcons[headerLabel].Image = Properties.Resources.SortAscending;
                                break;
                            case SortOrder.Descending:
                                sortIcons[headerLabel].Image = Properties.Resources.SortDescending;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the scroll event for the DataGridView to adjust header and panel positions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ScrollEventArgs"/> that contains the event data.</param>
        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                UpdateHeaderAndPanelPositions();
            }
        }

        
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            // apply theme for grid
            this.GridView.BackgroundColor = _currentTheme.GridBackColor;
            this.GridView.DefaultCellStyle.BackColor = _currentTheme.GridBackColor;
            this.GridView.DefaultCellStyle.ForeColor = _currentTheme.GridForeColor;
            this.GridView.DefaultCellStyle.SelectionForeColor= _currentTheme.GridRowSelectedForeColor;
            this.GridView.DefaultCellStyle.SelectionBackColor = _currentTheme.GridRowSelectedBackColor;
            this.GridView.ColumnHeadersDefaultCellStyle.BackColor = _currentTheme.GridHeaderBackColor;
            this.GridView.ColumnHeadersDefaultCellStyle.ForeColor = _currentTheme.GridHeaderForeColor;
            this.GridView.GridColor = _currentTheme.GridLineColor;
            this.GridView.AlternatingRowsDefaultCellStyle.BackColor = _currentTheme.AltRowBackColor;
            this.GridView.AlternatingRowsDefaultCellStyle.ForeColor = _currentTheme.GridForeColor;
            this.GridView.DefaultCellStyle.SelectionBackColor = _currentTheme.GridRowSelectedBackColor;
            this.GridView.DefaultCellStyle.SelectionForeColor = _currentTheme.GridRowSelectedForeColor;

            // apply theme for header
            this.Toppanel.BackColor = _currentTheme.BackColor;
            this.customHeaderPanel.BackColor = _currentTheme.GridHeaderBackColor;
            this.filterPanel.BackColor = _currentTheme.GridHeaderBackColor;
            this.Totalspanel.BackColor = _currentTheme.GridHeaderBackColor;
            //apply theme for buttons
            this.FilterShowbutton.Theme = Theme;
            this.TotalShowbutton.Theme = Theme;
            this.Printbutton.Theme = Theme;
            this.CSVExportbutton.Theme = Theme;
            //this.BindingNavigator.Theme = Theme;
            this.Titlelabel.Theme = Theme;
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                Titlelabel.UseThemeFont = true;
            }
            else
            {
                Titlelabel.TextFont = _textFont;
            }
            if(ShowHeaderPanel)
            {
                layoutPanel.RowStyles[0].Height = Titlelabel.PreferredSize.Height + (2 * (padding + 2)); // Adjust height dynamically
            }
           
            if (IsChild)
            {
                BackColor = _currentTheme.ButtonBackColor;
            }
         



        }
        /// <summary>
        /// Changes the border style for the column headers and updates their positions.
        /// </summary>
        /// <param name="borderStyle">The new border style to apply.</param>
        private void ChangeBorderStyleForColumnHeaders(BorderStyle borderStyle)
        {
            UpdateHeaderAndPanelPositions();
        }

        /// <summary>
        /// Applies the initial styles for the DataGridView, including background color and border styles.
        /// </summary>
        private void ApplyInitialStyles()
        {
            ApplyTheme();
        }

        /// <summary>
        /// Handles the CellPainting event for custom cell painting in the DataGridView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewCellPaintingEventArgs"/> that contains the event data.</param>
        private void DataGridView1_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            CellPainting?.Invoke(sender, e);
            if (!e.Handled)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
            }
        }

        /// <summary>
        /// Handles the ColumnWidthChanged event for updating header and panel positions when a column's width changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewColumnEventArgs"/> that contains the event data.</param>
        private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            UpdateHeaderAndPanelPositions();
        }

        /// <summary>
        /// Updates the positions and sizes of headers, filter controls, and total controls based on the current grid layout.
        /// </summary>
        private void UpdateHeaderAndPanelPositions()
        {
            int offset = dataGridView1.HorizontalScrollingOffset;
            int xPos = -offset;

            foreach (Control header in customHeaderPanel.Controls)
            {
                if (header is Label column)
                {
                    header.Left = xPos;
                    header.Width = column.Width;
                    column.BorderStyle = this.BorderStyle;
                    if (sortIcons.ContainsKey(column))
                    {
                        sortIcons[column].Left = header.Width - 18; // Adjust as needed
                    }

                    var col = dataGridView1.Columns[column.Text];
                    string guid = (String)col.Tag;
                    columnConfigs[columnConfigs.FindIndex(p => p.GuidID == guid)].Width = col.Width;
                    column.Width = col.Width;
                    xPos += column.Width;
                }
            }

            xPos = -offset;
            foreach (Control filter in filterPanel.Controls)
            {
                if (filter is TextBox textBox)
                {
                    string guid = (String)filter.Tag;
                    GridControls gridctls = gridControls[gridControls.FindIndex(p => p.GuidID == guid)];
                    int wdth = columnConfigs[columnConfigs.FindIndex(p => p.GuidID == gridctls.GuidID)].Width;
                    filter.Left = xPos;
                    filter.Width = wdth;
                    textBox.BorderStyle = this.BorderStyle;
                    xPos += wdth;
                }
            }

            xPos = -offset;
            foreach (Control total in Totalspanel.Controls)
            {
                if (total is TextBox textBox)
                {
                    GridControls gridctls = gridControls[gridControls.FindIndex(p => p.GuidID == total.Tag.ToString())];
                    int wdth = columnConfigs[columnConfigs.FindIndex(p => p.GuidID == gridctls.GuidID)].Width;
                    total.Left = xPos;
                    total.Width = wdth;
                    textBox.BorderStyle = this.BorderStyle;
                    xPos += wdth;
                }
            }
        }

        /// <summary>
        /// Adds column configuration settings for a specified column, including filters and totals.
        /// </summary>
        /// <param name="column">The column to configure.</param>
        /// <param name="index">The index of the column.</param>
        /// <param name="width">The width of the column.</param>
        /// <param name="name">The name of the column.</param>
        /// <param name="headertext">The header text of the column.</param>
        private void AddColumnConfigurations(DataGridViewColumn column, int index, int width, string name, string headertext)
        {
            if (columnConfigs.Count > 0 && columnConfigs.Exists(p => p.Index == index))
            {
                return;
            }
            TextBox FilterBox = new TextBox()
            {
                BorderStyle = this.BorderStyle,
                Width = width,
                Top = 2,
                Height = 18,
                Tag = column.Tag
            };
            TextBox TotalBox = new TextBox()
            {
                BorderStyle = this.BorderStyle,
                Width = width,
                Top = Topx,
                Height = 18,
                Tag = column.Tag
            };
            GridControls controls = new GridControls()
            {
                Index = index,
                FilterBox = FilterBox,
                TotalBox = TotalBox,
                GuidID = column.Tag.ToString()
            };
            BeepGridColumnConfig cfg = new BeepGridColumnConfig()
            {
                Index = index,
                ColumnName = name,
                ColumnCaption = headertext,
                Filter = "",
                HasTotal = false,
                IsFiltered = false,
                IsSorted = false,
                IsFilteOn = false,
                IsTotalOn = false,
                GuidID = column.Tag.ToString()
            };

            FilterBox.TextChanged += FilterTextBox_TextChanged;
            TotalBox.TextChanged += TotalTextBox_TextChanged;
            filterPanel.Controls.Add(FilterBox);
            Totalspanel.Controls.Add(TotalBox);
            columnConfigs.Add(cfg);
            gridControls.Add(controls);
        }

        /// <summary>
        /// Handles the ColumnRemoved event to remove column configurations, filters, and totals when a column is removed from the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewColumnEventArgs"/> that contains the event data.</param>
        private void DataGridView1_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            int cfgidx = columnConfigs.FindIndex(p => p.Index == e.Column.Index);
            if (cfgidx > -1)
            {
                TextBox filterTextBox = gridControls[cfgidx].FilterBox;
                if (filterTextBox != null)
                {
                    filterTextBox.TextChanged -= FilterTextBox_TextChanged;
                    filterPanel.Controls.Remove(filterTextBox);
                }
                TextBox totalTextBox = gridControls[cfgidx].TotalBox;
                if (totalTextBox != null)
                {
                    totalTextBox.TextChanged -= TotalTextBox_TextChanged;
                    Totalspanel.Controls.Remove(totalTextBox);
                }
                columnConfigs.RemoveAt(cfgidx);
                gridControls.RemoveAt(cfgidx);
            }
        }

        #endregion "Visual Styles"
        #region "Sizing Functions"

        /// <summary>
        /// Handles the resize event of the control, forcing it to redraw and update all invalidated regions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void BeepGrid_Resize(object? sender, EventArgs e)
        {
           
            UpdateDrawingRect();
            layoutPanel.Left = DrawingRect.Left+padding;
            layoutPanel.Top = DrawingRect.Top + padding;
            layoutPanel.Width = DrawingRect.Width - (padding * 2);
            layoutPanel.Height = DrawingRect.Height - (padding * 2);

            //dataGridView1.Left = DrawingRect.Left+padding;
            //dataGridView1.Top = DrawingRect.Top + padding;
            //dataGridView1.Width=DrawingRect.Width-(padding*2);
            //dataGridView1.Height = DrawingRect.Height - (padding * 2);

            //customHeaderPanel.Left = dataGridView1.Left;
            //customHeaderPanel.Width = dataGridView1.Width;
            //filterPanel.Left = dataGridView1.Left;
            //filterPanel.Width = dataGridView1.Width;
            //Totalspanel.Left = dataGridView1.Left;
            //Totalspanel.Width = dataGridView1.Width;
            UpdateHeaderAndPanelPositions();
            // Invalidate the DataGridView and control surfaces to trigger a repaint.
           // dataGridView1.Invalidate();
            //this.Invalidate();  // Invalidates the entire surface of the control and causes it to be redrawn
            //this.Update();      // Forces the control to immediately repaint any invalidated regions
        }

        /// <summary>
        /// Handles the resize event for the DataGridView, adjusting the widths of custom header and panel elements.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void DataGridView_Resize(object sender, EventArgs e)
        {
            // Adjusts the widths of the custom header panel, filter panel, and totals panel to match the DataGridView width.
            customHeaderPanel.Width = dataGridView1.Width;
            filterPanel.Width = dataGridView1.Width;
            Totalspanel.Width = dataGridView1.Width;
            UpdateHeaderAndPanelPositions();
        }

        #endregion "Sizing Functions"
        #region "Print"
        PrintDocument printDocument = new PrintDocument();
        private int Topx;
        private int currentRow;
      

        /// <summary>
        /// Handles the click event for the CSV export button, exporting the DataGridView content to a CSV file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void CSVExportbutton_Click(object? sender, EventArgs e)
        {
            GridHelpers.ExportGridToCSV(dataGridView1);
        }

        /// <summary>
        /// Handles the click event for the Print button, printing the DataGridView content.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void Printbutton_Click(object sender, EventArgs e)
        {
            GridHelpers.PrintGrid(DMEEditor, dataGridView1, Title.Text, DateTime.Now.ToShortDateString(), "", true);
        }

        /// <summary>
        /// Configures the print document's orientation to landscape or portrait.
        /// </summary>
        /// <param name="landscape">If true, sets the page orientation to landscape; otherwise, sets it to portrait.</param>
        public void ConfigurePrintDocument(bool landscape)
        {
            printDocument.DefaultPageSettings.Landscape = landscape;
        }

        /// <summary>
        /// Handles the printing of the DataGridView, including headers and rows, across multiple pages if necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PrintPageEventArgs"/> that contains the event data.</param>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int topMargin = e.MarginBounds.Top;
            int yPos = topMargin; // Start yPos at topMargin
            int leftMargin = e.MarginBounds.Left;

            // Drawing column headers
            int headerHeight = dataGridView1.ColumnHeadersHeight;
            yPos += headerHeight; // Move yPos below header

            // Draw each column header
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                DataGridViewColumn column = dataGridView1.Columns[i];
                Rectangle headerRect = new Rectangle(leftMargin, topMargin, column.Width, headerHeight);
                e.Graphics.FillRectangle(Brushes.LightGray, headerRect);  // Header background
                e.Graphics.DrawRectangle(Pens.Black, headerRect);  // Header border

                // Set header text format
                StringFormat strFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                // Draw header text
                e.Graphics.DrawString(column.HeaderText,
                                      column.InheritedStyle.Font,
                                      Brushes.Black,
                                      headerRect,
                                      strFormat);
                leftMargin += column.Width; // Move left margin to next column position
            }

            // Reset left margin after headers
            leftMargin = e.MarginBounds.Left;

            // Print rows
            while (currentRow < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[currentRow];
                yPos += dataGridView1.RowTemplate.Height; // Increment yPos for the row

                // Check page bounds
                if (yPos + dataGridView1.RowTemplate.Height > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true; // More pages if row exceeds page bounds
                    break; // Break the loop to handle next page
                }

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    DataGridViewCell cell = row.Cells[i];
                    Rectangle cellBounds = new Rectangle(leftMargin, yPos, dataGridView1.Columns[i].Width, dataGridView1.RowTemplate.Height);

                    // Drawing cell background
                    using (SolidBrush cellBackground = new SolidBrush(cell.InheritedStyle.BackColor))
                    {
                        e.Graphics.FillRectangle(cellBackground, cellBounds);
                    }

                    // Drawing cell borders
                    e.Graphics.DrawRectangle(Pens.Black, cellBounds);

                    // Drawing text
                    using (SolidBrush cellForeBrush = new SolidBrush(cell.InheritedStyle.ForeColor))
                    {
                        e.Graphics.DrawString(cell.FormattedValue.ToString(), cell.InheritedStyle.Font, cellForeBrush, cellBounds);
                    }

                    leftMargin += dataGridView1.Columns[i].Width; // Move left margin to next cell
                }

                leftMargin = e.MarginBounds.Left; // Reset left margin after the row
                currentRow++; // Move to next row
            }

            if (currentRow >= dataGridView1.Rows.Count)
            {
                e.HasMorePages = false;
                currentRow = 0; // Reset the row counter
            }
        }

        #endregion
        #region "Click and Mouse Events"

        /// <summary>
        /// Handles DataGridView data error events, logging error messages in the DMEEditor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewDataErrorEventArgs"/> that contains the event data.</param>
        private void DataGridView1_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            DMEEditor.AddLogMessage("Error", $"Error in Grid {e.Exception.Message}", DateTime.Now, 0, "", Errors.Failed);
        }

        /// <summary>
        /// Invalidates the DataGridView and forces it to be redrawn, ensuring that any invalid regions are updated.
        /// </summary>
        public void Invalidate()
        {
            dataGridView1.Invalidate();
            this.Invalidate();  // Invalidates the entire surface of the control and causes it to be redrawn
            this.Update();      // Forces the control to immediately repaint any invalidated regions
        }

        /// <summary>
        /// Handles the MouseDown event for custom column header resizing, initializing the resize operation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        private void CustomHeaderLabel_MouseDown(object sender, MouseEventArgs e)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AllowUserToResizeColumns = true;
            var headerLabel = sender as Label;
            if (headerLabel == null) return;

            if (e.Button == MouseButtons.Left)
            {
                initialMousePosition = e.Location;
                initialColumnWidth = headerLabel.Width;
                initialLabelLeft = headerLabel.Left;
                isResizing = true;
                resizingLabel = headerLabel;

                // Determine if resizing from left or right edge
                isResizingLeft = e.X < headerLabel.Width / 2;
            }
        }

        /// <summary>
        /// Handles the MouseMove event for custom column header resizing, updating the column width during resize.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        private void CustomHeaderLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing && resizingLabel != null)
            {
                if (isResizingLeft)
                {
                    int newLeft = initialLabelLeft + (e.X - initialMousePosition.X);
                    int newWidth = initialColumnWidth - (e.X - initialMousePosition.X);

                    if (newWidth > 0)
                    {
                        resizingLabel.Width = newWidth;
                        resizingLabel.Left = newLeft;
                        var column = headerColumnMapping[resizingLabel];
                        column.Width = newWidth;
                        UpdateHeaderAndPanelPositions();
                    }
                }
                else
                {
                    int newWidth = initialColumnWidth + (e.X - initialMousePosition.X);

                    if (newWidth > 0)
                    {
                        resizingLabel.Width = newWidth;
                        var column = headerColumnMapping[resizingLabel];
                        column.Width = newWidth;
                        UpdateHeaderAndPanelPositions();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseUp event, finalizing the column resize operation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        private void CustomHeaderLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isResizing)
            {
                isResizing = false;
                resizingLabel = null;
            }
        }

        /// <summary>
        /// Handles the click event for sorting when a column header is clicked, toggling the sort direction.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void CustomHeaderLabel_Click(object sender, EventArgs e)
        {
            if (isResizing)
            {
                return;
            }
            var headerLabel = sender as Label;
            if (headerLabel == null) return;

            if (!headerColumnMapping.TryGetValue(headerLabel, out var column))
                return;

            var newSortDirection = (Currentdirection == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            DataGridViewColumn oldColumn = dataGridView1.Columns[column.Name];
            if (SortColumn != oldColumn)
            {
                if (SortColumn != null)
                {
                    Currentdirection = SortOrder.None;
                    SortColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    UpdateSortIcons(SortColumnLabel, SortOrder.None);
                }
                SortColumn = oldColumn;
                IsSorting = true;
                SortColumnLabel = headerLabel;
            }

            if (Currentdirection == SortOrder.None || oldColumn.SortMode == DataGridViewColumnSortMode.NotSortable)
            {
                oldColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                newSortDirection = SortOrder.Ascending;
            }
            else if (Currentdirection == SortOrder.Ascending)
            {
                newSortDirection = SortOrder.Descending;
            }
            else if (Currentdirection == SortOrder.Descending)
            {
                oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                Currentdirection = SortOrder.None;
                BindingNavigator.bindingSource.RemoveSort();
                UpdateSortIcons(headerLabel, SortOrder.None);
                return;
            }
            Currentdirection = newSortDirection;

            dataGridView1.Sort(SortColumn, newSortDirection == SortOrder.Ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending);
            UpdateSortIcons(headerLabel, newSortDirection);
        }

        /// <summary>
        /// Handles the click event for showing or hiding the filter panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void FilterShowbutton_Click(object sender, EventArgs e)
        {
            this.filterPanel.Visible = !filterPanel.Visible;
        }

        /// <summary>
        /// Handles the click event for showing or hiding the totals panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void TotalShowbutton_Click(object sender, EventArgs e)
        {
            this.Totalspanel.Visible = !Totalspanel.Visible;
        }

        /// <summary>
        /// Handles the click event for opening the filter configuration dialog for the current entity structure.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void FilterpictureBox1_Click(object sender, EventArgs e)
        {
            if (EntityStructure != null)
            {
                using (BeepFilter frmfilter = new BeepFilter())
                {
                    frmfilter.SetFilters(EntityStructure);
                    if (frmfilter.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        EntityStructure = frmfilter.Entity;
                    }
                }
            }
        }

        #endregion
        #region "Layout Load and Save"
        /// <summary>
        /// Generates a unique GridId based on the current timestamp or GUID.
        /// This is used to uniquely identify each grid dropped in the form.
        /// </summary>
        private void GenerateUniqueGridId()
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                // Ensure unique ID is generated at design time
                _gridId = "BeepGrid_" + Guid.NewGuid().ToString("N");
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Ensure consistent GridId at runtime and design time
            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                LoadGridLayout();  // Load layout specific to this grid based on GridId
            }
        }
        public void LoadGridLayout()
        {
            string layoutFilePath = $"{GridId}_layout.json";
            if (File.Exists(layoutFilePath))
            {
                LoadColumnLayoutFromFile(layoutFilePath);  // Load layout if file exists
            }
        }

        public void SaveGridLayout()
        {
            string layoutFilePath = $"{GridId}_layout.json";
            SaveColumnLayoutToFile(layoutFilePath);  // Save layout to file
        }
        public void ApplyColumnConfigurations()
        {
            this.Columns.Clear(); // Clear existing columns before applying the configuration

            foreach (var config in columnConfigs)
            {
                DataGridViewColumn column = null;

                // Handle different types of columns
                switch (config.ColumnType)
                {
                    case "BeepDataGridViewComboBoxColumn":
                        column = new BeepDataGridViewComboBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                            DataSourceName = config.DataSourceName,
                            DisplayMember = config.DisplayMember,
                            ValueMember = config.ValueMember
                        };

                        // Check for custom cascading map
                        //if (column is BeepDataGridViewComboBoxColumn comboBoxColumn && config.CascadingMap != null)
                        //{
                        //    comboBoxColumn.CascadingMap = config.CascadingMap;
                        //}
                        break;

                    case "BeepDataGridViewNumericColumn":
                        column = new BeepDataGridViewNumericColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                            // You can add more numeric-specific properties here if needed
                        };
                        break;

                    case "BeepDataGridViewProgressBarColumn":
                        column = new BeepDataGridViewProgressBarColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                            ProgressBarColor = config.ProgressBarColor
                        };
                        break;

                    case "BeepDataGridViewSvgColumn":
                        column = new BeepDataGridViewSvgColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewThreeStateCheckBoxColumn":
                        column = new BeepDataGridViewThreeStateCheckBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width
                        };
                        break;

                    case "BeepDataGridViewSliderColumn":
                        column = new BeepDataGridViewSliderColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewRatingColumn":
                        column = new BeepDataGridViewRatingColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                            FilledStarColor = config.FilledStarColor,
                            EmptyStarColor = config.EmptyStarColor,
                            MaxStars = config.MaxStars
                        };
                        break;

                    case "BeepDataGridViewSwitchColumn":
                        column = new BeepDataGridViewSwitchColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewMultiColumnColumn":
                        column = new BeepDataGridViewMultiColumnColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewImageComboBoxColumn":
                        column = new BeepDataGridViewImageComboBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width,
                        };
                        break;

                    default:
                        // Fallback to DataGridViewTextBoxColumn if type is not found
                        column = new DataGridViewTextBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.ColumnCaption,
                            Width = config.Width
                        };
                        break;
                }

                if (column != null)
                {
                    this.Columns.Add(column); // Add the column to the grid
                }
            }
        }
        public void CaptureCurrentLayout()
        {
            // Clear any previous configurations
            columnConfigs.Clear();

            // Iterate over each column in the DataGridView
            foreach (DataGridViewColumn column in this.Columns)
            {
                var config = new BeepGridColumnConfig
                {
                    Name = column.Name,
                    ColumnCaption = column.HeaderText,
                    Width = column.Width,
                    Visible = column.Visible,
                    DisplayIndex = column.DisplayIndex,
                    ReadOnly = column.ReadOnly,
                    SortMode = column.SortMode,
                    AutoSizeMode = column.AutoSizeMode,
                    Resizable = column.Resizable,
                    DividerWidth = column.DividerWidth,
                    Frozen = column.Frozen,  // Capture if the column is frozen
                    MinimumWidth = column.MinimumWidth, // Minimum width of the column
                };

                // Handle ComboBox columns
                if (column is DataGridViewComboBoxColumn comboBoxColumn)
                {
                    config.ColumnType = nameof(DataGridViewComboBoxColumn);
                    config.DisplayMember = comboBoxColumn.DisplayMember;
                    config.ValueMember = comboBoxColumn.ValueMember;
                    //config.LookupList = new List<ColumnLookupList>();

                    // Capture ComboBox rootnodeitems
                    //foreach (var item in comboBoxColumn.Items)
                    //{
                    //    config.LookupList.Add(new ColumnLookupList
                    //    {
                    //        Display = item.ToString(),
                    //        Value = item
                    //    });
                    //}
                }
                // Handle Rating columns
                else if (column is BeepDataGridViewRatingColumn ratingColumn)
                {
                    config.ColumnType = nameof(BeepDataGridViewRatingColumn);
                    config.MaxStars = ratingColumn.MaxStars;
                    config.FilledStarColor = ratingColumn.FilledStarColor;
                    config.EmptyStarColor = ratingColumn.EmptyStarColor;
                }
                // Handle ProgressBar columns
                else if (column is BeepDataGridViewProgressBarColumn progressBarColumn)
                {
                    config.ColumnType = nameof(BeepDataGridViewProgressBarColumn);
                    config.ProgressBarColor = progressBarColumn.ProgressBarColor;
                    config.ProgressBarMaxValue = progressBarColumn.Maximum;
                    config.ProgressBarMinValue = progressBarColumn.Minimum;
                    config.ProgressBarStep = progressBarColumn.Step;
                }
                // Handle Numeric columns
                else if (column is BeepDataGridViewNumericColumn numericColumn)
                {
                    config.ColumnType = nameof(BeepDataGridViewNumericColumn);
                    config.Format = numericColumn.DefaultCellStyle.Format;
                }
                // Handle custom columns
                else if (column is BeepDataGridViewSvgColumn svgColumn)
                {
                    config.ColumnType = nameof(BeepDataGridViewSvgColumn);
                    // Additional SVG properties can be captured here if necessary
                }
                else if (column is BeepDataGridViewThreeStateCheckBoxColumn threeStateColumn)
                {
                    config.ColumnType = nameof(BeepDataGridViewThreeStateCheckBoxColumn);
                }
                else if (column is BeepDataGridViewSliderColumn sliderColumn)
                {
                    config.ColumnType = nameof(BeepDataGridViewSliderColumn);
                    config.Minimum = sliderColumn.Minimum;
                    config.Maximum = sliderColumn.Maximum;
                }

                // Add column configuration to the list
                columnConfigs.Add(config);
            }
        }
        public void SaveColumnLayoutToFile(string filePath)
        {
            var json = JsonConvert.SerializeObject(columnConfigs, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public void LoadColumnLayoutFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                columnConfigs = JsonConvert.DeserializeObject<List<BeepGridColumnConfig>>(json);
                ApplyColumnConfigurations(); // Rebuild the grid with loaded columns
            }
        }
        #endregion "Layout Load and Save"
        #region "Drawing and Layout"


        private void SetRowVisibility(TableLayoutPanel table, int rowIndex, bool isVisible, float originalHeight)
        {
            if (isVisible)
            {
                // Restore original height
                table.RowStyles[rowIndex].Height = originalHeight;
                table.GetControlFromPosition(0, rowIndex).Visible = true;
            }
            else
            {
                // Collapse the row
                table.RowStyles[rowIndex].Height = 0;
                table.GetControlFromPosition(0, rowIndex).Visible = false;
            }
        }

        private void CreateComponent()
        {
            // Create TableLayoutPanel
            layoutPanel = new TableLayoutPanel();
            layoutPanel.Padding = new Padding(0);
            layoutPanel.Margin = new Padding(0);
            layoutPanel.ColumnCount = 1;
            layoutPanel.RowCount = 6; // Define sections
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));  // Top Panel
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));  // Custom Header Panel
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Data Grid (Expands)
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // Filter Panel (Fixed, can be hidden)
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // Totals Panel (Fixed, can be hidden)
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // Bottom Panel (Fixed)

            // Initialize Panels
            Toppanel = new Panel { BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill };
            InitializeTopPanelControls(); // Helper method to initialize buttons

            customHeaderPanel = new Panel { BackColor = Color.Magenta, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill };

            filterPanel = new Panel { BackColor = Color.Khaki, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill, Visible = true };

            Totalspanel = new Panel { BackColor = Color.LawnGreen, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Fill, Visible = true };

            dataGridView1 = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            Bottompanel = new Panel { BackColor = Color.White, Dock = DockStyle.Fill };
            BindingNavigator = new BeepbindingNavigator { Dock = DockStyle.Fill };
            Bottompanel.Controls.Add(BindingNavigator);

            // Add controls to TableLayoutPanel
            layoutPanel.Controls.Add(Toppanel, 0, 0);
            layoutPanel.Controls.Add(customHeaderPanel, 0, 1);
            layoutPanel.Controls.Add(dataGridView1, 0, 2);
            layoutPanel.Controls.Add(filterPanel, 0, 3);
            layoutPanel.Controls.Add(Totalspanel, 0, 4);
          
            layoutPanel.Controls.Add(Bottompanel, 0, 5);

            // Add TableLayoutPanel to BeepGrid
            Controls.Add(layoutPanel);
            UpdateDrawingRect();
            layoutPanel.Left = DrawingRect.Left + padding;
            layoutPanel.Top = DrawingRect.Top + padding;
            layoutPanel.Width = DrawingRect.Width - (padding * 2);
            layoutPanel.Height = DrawingRect.Height - (padding * 2);
           
            // Resize event (No need to adjust manually anymore!)
        }

        // Helper method for top panel buttons
        private void InitializeTopPanelControls()
        {
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
            {
                ColumnCount = 6,
                RowCount = 1,
                Dock = DockStyle.Fill
            };
            tableLayoutPanel.Padding = new Padding(0);
            tableLayoutPanel.Margin = new Padding(0);
            Toppanel.Controls.Add(tableLayoutPanel);
            // Set column widths
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            // add column for Title for the reset of the space
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            // Set row height
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));

            CSVExportbutton = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.export.svg", Size = buttonwidth, MaxImageSize = new Size(buttonwidth.Width - 1, buttonwidth.Height - 1), ImageAlign = ContentAlignment.MiddleCenter, HideText = true, IsFramless = true, ApplyThemeOnImage = false, IsChild = true, Dock = DockStyle.Fill };
            TotalShowbutton = new BeepButton {ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.sum.svg", Size = buttonwidth, MaxImageSize = new Size(buttonwidth.Width - 1, buttonwidth.Height - 1), ImageAlign = ContentAlignment.MiddleCenter, HideText = true, IsFramless = true, ApplyThemeOnImage = false , IsChild = true , Dock = DockStyle.Fill };
            Sharebutton = new BeepButton {ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.share.svg", Size = buttonwidth, MaxImageSize = new Size(buttonwidth.Width - 1, buttonwidth.Height - 1), ImageAlign = ContentAlignment.MiddleCenter, HideText = true, IsFramless = true, ApplyThemeOnImage = false, IsChild = true, Dock = DockStyle.Fill };
            Printbutton = new BeepButton {ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg", Size = buttonwidth, MaxImageSize = new Size(buttonwidth.Width - 1, buttonwidth.Height - 1), ImageAlign = ContentAlignment.MiddleCenter, HideText = true, IsFramless = true , ApplyThemeOnImage = false, IsChild = true, Dock = DockStyle.Fill };
            FilterShowbutton = new BeepButton {ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg", Size = buttonwidth, MaxImageSize = new Size(buttonwidth.Width - 1, buttonwidth.Height - 1), ImageAlign = ContentAlignment.MiddleCenter, HideText = true, IsFramless = true , ApplyThemeOnImage = false, IsChild = true ,Dock= DockStyle.Fill};

            Titlelabel = new BeepLabel { Text = "Title", TextAlign = ContentAlignment.MiddleCenter,ImageAlign= ContentAlignment.MiddleRight , IsFramless=true,IsChild=true, Dock = DockStyle.Fill };
            // Add buttons to the TableLayoutPanel
            tableLayoutPanel.Controls.Add(CSVExportbutton, 0, 0);
            tableLayoutPanel.Controls.Add(TotalShowbutton, 1, 0);
            tableLayoutPanel.Controls.Add(Sharebutton, 2, 0);
            tableLayoutPanel.Controls.Add(Printbutton, 3, 0);
            tableLayoutPanel.Controls.Add(FilterShowbutton, 4, 0);
            tableLayoutPanel.Controls.Add(Titlelabel, 5, 0);

        }

        #endregion "Drawing and Layout"

    }
}
